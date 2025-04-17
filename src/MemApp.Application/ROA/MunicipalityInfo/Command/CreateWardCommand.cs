using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Interfaces;
using Res.Domain.Entities;
using ResApp.Application.ROA.Union.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ResApp.Application.ROA.MunicipalityInfo.Models;
using Microsoft.EntityFrameworkCore;

namespace ResApp.Application.ROA.MunicipalityInfo.Command
{
   
    public class CreateWardCommand : IRequest<Result<WardDto>>
    {
        public int Id { get; set; }

        public string? EnglishName { get; set; }
        public string? BanglaName { get; set; }
        public DateTime? CreatedOn { get; set; }

        public int? ThanaId { get; set; }
        public int? MunicipalityId { get; set; }
        public int? UnionInfoId { get; set; }
    }

    public class CreateWardCommandHandler : IRequestHandler<CreateWardCommand, Result<WardDto>>
    {
        private readonly IMemDbContext _context;
        private readonly IMediator _mediator;
        private readonly IPermissionHandler _permissionHandler;
        private readonly ICurrentUserService _currentUserService;

        //  private readonly IWebHostEnvironment _hostingEnv;
        public CreateWardCommandHandler(IMemDbContext context, IMediator mediator, IPermissionHandler permissionHandler, ICurrentUserService currentUserService)
        {
            _context = context;
            _mediator = mediator;
            _currentUserService = currentUserService;
            _permissionHandler = permissionHandler;
        }

        public async Task<Result<WardDto>> Handle(CreateWardCommand request, CancellationToken cancellationToken)
        {
            try
            {

                if (_currentUserService?.UserId == null || _currentUserService.UserId == 0)
                {
                    throw new UnauthorizedAccessException();
                }


                var result = new Result<WardDto>();

                if (request.Id > 0)
                {
                    if (!await _permissionHandler.HasRolePermissionAsync(4002))
                    {
                        result.HasError = true;
                        result.Messages.Add("Edit Ward Permission Denied");
                        return result;
                    }
                    var checkWardExist = await _context.Wards
                       .FirstOrDefaultAsync(q => q.Id == request.Id, cancellationToken: cancellationToken);

                    if (checkWardExist == null)
                    {
                        result.HasError = true;
                        result.Messages.Add("Data does not exist!!!");
                        return result;
                    }


                    if (checkWardExist != null)
                    {
                        // Check if EnglishName or BanglaName exists in any other record
                        //bool isDuplicate = await _context.UnionInfos
                        //    .AnyAsync(d => d.Id != checkWardExist.Id &&
                        //                  (d.EnglishName == request.EnglishName || d.BanglaName == request.BanglaName));

                        //if (isDuplicate)
                        //{
                        //    result.HasError = true;
                        //    result.Messages.Add("This Union Name is already existed!!!");
                        //    return result;
                        //}


                        checkWardExist.EnglishName = request.EnglishName;
                        checkWardExist.BanglaName = request.BanglaName;
                        checkWardExist.ThanaId = request.ThanaId;
                        checkWardExist.MunicipalityId = request.MunicipalityId;
                        checkWardExist.UnionInfoId = request.UnionInfoId;

                        _context.Wards.Update(checkWardExist);

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

                    var checkWardExist = await _context.Wards
                       .FirstOrDefaultAsync(q => q.EnglishName == request.EnglishName || q.BanglaName == request.BanglaName, cancellationToken: cancellationToken);

                    if (checkWardExist != null)
                    {
                        //result.HasError = true;
                        //result.Messages.Add("This Zone Name is already existed!!!");
                        //return result;
                    }



                    var ward = new Ward
                    {
                        EnglishName = request.EnglishName,
                        BanglaName = request.BanglaName,
                        ThanaId = request.ThanaId,
                        MunicipalityId = request.MunicipalityId,
                        UnionInfoId = request.UnionInfoId,
                        IsActive = true
                    };

                    _context.Wards.Add(ward);
                    if (await _context.SaveChangesAsync(cancellationToken) > 0)
                    {

                        result.HasError = false;
                        result.Messages.Add("New Ward Info has been created successfully!!");

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
