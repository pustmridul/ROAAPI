using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Interfaces;
using Res.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using ResApp.Application.ROA.Union.Models;

namespace ResApp.Application.ROA.MunicipalityInfo.Command
{

    public class CreateMunicipalityCommand : IRequest<Result<UnionDto>>
    {
        public int Id { get; set; }

        public string? EnglishName { get; set; }
        public string? BanglaName { get; set; }
        public DateTime? CreatedOn { get; set; }

        public int ThanaId { get; set; }
    }

    public class CreateMunicipalityCommandHandler : IRequestHandler<CreateMunicipalityCommand, Result<UnionDto>>
    {
        private readonly IMemDbContext _context;
        private readonly IMediator _mediator;
        private readonly IPermissionHandler _permissionHandler;
        private readonly ICurrentUserService _currentUserService;

        //  private readonly IWebHostEnvironment _hostingEnv;
        public CreateMunicipalityCommandHandler(IMemDbContext context, IMediator mediator, IPermissionHandler permissionHandler, ICurrentUserService currentUserService)
        {
            _context = context;
            _mediator = mediator;
            _currentUserService = currentUserService;
            _permissionHandler = permissionHandler;
        }

        public async Task<Result<UnionDto>> Handle(CreateMunicipalityCommand request, CancellationToken cancellationToken)
        {
            try
            {

                if (_currentUserService?.UserId == null || _currentUserService.UserId == 0)
                {
                    throw new UnauthorizedAccessException();
                }


                var result = new Result<UnionDto>();

                if (request.Id > 0)
                {
                    if (!await _permissionHandler.HasRolePermissionAsync(4002))
                    {
                        result.HasError = true;
                        result.Messages.Add("Edit Zone Permission Denied");
                        return result;
                    }
                    var checkUnionExist = await _context.Municipalities
                       .FirstOrDefaultAsync(q => q.Id == request.Id, cancellationToken: cancellationToken);

                    if (checkUnionExist == null)
                    {
                        result.HasError = true;
                        result.Messages.Add("Data does not exist!!!");
                        return result;
                    }


                    if (checkUnionExist != null)
                    {
                        // Check if EnglishName or BanglaName exists in any other record
                        //bool isDuplicate = await _context.UnionInfos
                        //    .AnyAsync(d => d.Id != checkUnionExist.Id &&
                        //                  (d.EnglishName == request.EnglishName || d.BanglaName == request.BanglaName));

                        //if (isDuplicate)
                        //{
                        //    result.HasError = true;
                        //    result.Messages.Add("This Union Name is already existed!!!");
                        //    return result;
                        //}


                        checkUnionExist.EnglishName = request.EnglishName;
                        checkUnionExist.BanglaName = request.BanglaName;
                        checkUnionExist.ThanaId = request.ThanaId;

                        _context.Municipalities.Update(checkUnionExist);

                        if (await _context.SaveChangesAsync(cancellationToken) > 0)
                        {

                            result.HasError = false;
                            result.Messages.Add("Municipality Info has been updated successfully!!");

                            return result;
                        }
                        else
                        {
                            result.HasError = true;
                            result.Messages.Add("something wrong");
                            return result;
                        }

                    }
                    result.HasError = true;
                    result.Messages.Add("Data does not exist!!!");
                    return result;

                }

                else
                {
                    if (!await _permissionHandler.HasRolePermissionAsync(4001))
                    {
                        result.HasError = true;
                        result.Messages.Add("Create Munipality Permission Denied");
                        return result;
                    }

                    var checkUnionExist = await _context.ZoneInfos
                       .FirstOrDefaultAsync(q => q.EnglishName == request.EnglishName || q.BanglaName == request.BanglaName, cancellationToken: cancellationToken);

                    if (checkUnionExist != null)
                    {
                        //result.HasError = true;
                        //result.Messages.Add("This Zone Name is already existed!!!");
                        //return result;
                    }



                    var zone = new Municipality
                    {
                        EnglishName = request.EnglishName,
                        BanglaName = request.BanglaName,
                        ThanaId = request.ThanaId,
                        IsActive = true
                    };

                    _context.Municipalities.Add(zone);
                    if (await _context.SaveChangesAsync(cancellationToken) > 0)
                    {

                        result.HasError = false;
                        result.Messages.Add("New Municipality Info has been created successfully!!");

                        return result;
                    }
                    else
                    {
                        result.HasError = true;
                        result.Messages.Add("something wrong");
                        return result;
                    }
                    //return result;
                }

            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);

            }
        }
    }
}
