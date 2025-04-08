using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using Microsoft.EntityFrameworkCore;
using ResApp.Application.Models.DTOs;
using Microsoft.AspNetCore.Hosting; // Ensure this namespace is included
using Microsoft.Extensions.Hosting;
using Res.Domain.Entities;
using MemApp.Domain.Entities;
using Microsoft.AspNetCore.Http;


namespace ResApp.Application.Com.Commands.UpdateThana
{
    public class UpdateThanaInfoCommand : IRequest<Result<ThanaDto>>
    {
        public int Id { get; set; }

        public string? EnglishName { get; set; }
        public string? BanglaName { get; set; }
        public DateTime? CreatedOn { get; set; }

        public int DistrictId { get; set; }

    }

    public class UpdateThanaInfoCommandHandler : IRequestHandler<UpdateThanaInfoCommand, Result<ThanaDto>>
    {
        private readonly IMemDbContext _context;
        private readonly IMediator _mediator;       
        private readonly IPermissionHandler _permissionHandler;
        private readonly ICurrentUserService _currentUserService;

        //  private readonly IWebHostEnvironment _hostingEnv;
        public UpdateThanaInfoCommandHandler(IMemDbContext context, IMediator mediator, IPermissionHandler permissionHandler, ICurrentUserService currentUserService)
        {
            _context = context;
            _mediator = mediator;
            _currentUserService = currentUserService;
            _permissionHandler = permissionHandler;
        }

        public async Task<Result<ThanaDto>> Handle(UpdateThanaInfoCommand request, CancellationToken cancellationToken)
        {

            try
            {

                if (_currentUserService?.UserId == null || _currentUserService.UserId == 0)
                {
                    throw new UnauthorizedAccessException();
                }
               

                var result = new Result<ThanaDto>();

                if (request.Id > 0)
                {
                    if (!await _permissionHandler.HasRolePermissionAsync(4002))
                    {
                        result.HasError = true;
                        result.Messages.Add("Edit Thana Permission Denied");
                        return result;
                    }
                    var checkThanaExist = await _context.Thanas
                       .FirstOrDefaultAsync(q => q.Id == request.Id);

                    if (checkThanaExist == null)
                    {
                        result.HasError = true;
                        result.Messages.Add("Data does not exist!!!");
                        return result;
                    }


                    if (checkThanaExist != null)
                    {
                        // Check if EnglishName or BanglaName exists in any other record
                        bool isDuplicate = await _context.Thanas
                            .AnyAsync(d => d.Id != checkThanaExist.Id &&
                                          (d.EnglishName == request.EnglishName || d.BanglaName == request.BanglaName));

                        //if (isDuplicate)
                        //{
                        //    result.HasError = true;
                        //    result.Messages.Add("This Thana Name is already existed!!!");
                        //    return result;
                        //}


                        checkThanaExist.EnglishName = request.EnglishName;
                        checkThanaExist.BanglaName = request.BanglaName;
                        checkThanaExist.DistrictId = request.DistrictId;

                        _context.Thanas.Update(checkThanaExist);

                        if (await _context.SaveChangesAsync(cancellationToken) > 0)
                        {

                            result.HasError = false;
                            result.Messages.Add("Thana Info has been updated successfully!!");

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
                        result.Messages.Add("Create Thana Permission Denied");
                        return result;
                    }

                    var checkThanaExist = await _context.Thanas
                       .FirstOrDefaultAsync(q => (q.EnglishName == request.EnglishName || q.BanglaName == request.BanglaName ) && q.DistrictId==request.DistrictId);

                    if (checkThanaExist != null)
                    {
                        result.HasError = true;
                        result.Messages.Add("This Thana Name is already existed!!!");
                        return result;
                    }



                    var thana = new Thana
                    {
                        EnglishName = request.EnglishName,
                        BanglaName = request.BanglaName,
                        DistrictId = request.DistrictId,
                        IsActive=true,
                      
                    };

                    _context.Thanas.Add(thana);
                    if (await _context.SaveChangesAsync(cancellationToken) > 0)
                    {

                        result.HasError = false;
                        result.Messages.Add("New Thana Info has been created successfully!!");

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
