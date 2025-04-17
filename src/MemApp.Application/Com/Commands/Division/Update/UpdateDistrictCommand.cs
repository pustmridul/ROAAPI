using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Interfaces;
using ResApp.Application.Com.Commands.UpdateThana;
using ResApp.Application.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Res.Domain.Entities;

namespace ResApp.Application.Com.Commands.Division.Update
{
    public class UpdateDistrictCommand : IRequest<Result<DistrictDto>>
    {
         public int Id { get; set; }

        public string? EnglishName { get; set; }
        public string? BanglaName { get; set; }
        public DateTime? CreatedOn { get; set; }

        public int DistrictId { get; set; }
        public int DivisionId { get; set; }
    }

    public class UpdateDistrictCommandHandler : IRequestHandler<UpdateDistrictCommand, Result<DistrictDto>>
    {
        private readonly IMemDbContext _context;
        private readonly IMediator _mediator;
        private readonly IPermissionHandler _permissionHandler;
        private readonly ICurrentUserService _currentUserService;

        //  private readonly IWebHostEnvironment _hostingEnv;
        public UpdateDistrictCommandHandler(IMemDbContext context, IMediator mediator, IPermissionHandler permissionHandler, ICurrentUserService currentUserService)
        {
            _context = context;
            _mediator = mediator;
            _currentUserService = currentUserService;
            _permissionHandler = permissionHandler;
        }

        public async Task<Result<DistrictDto>> Handle(UpdateDistrictCommand request, CancellationToken cancellationToken)
        {
            try
            {

                if (_currentUserService?.UserId == null || _currentUserService.UserId == 0)
                {
                    throw new UnauthorizedAccessException();
                }


                var result = new Result<DistrictDto>();

                if (request.Id > 0)
                {
                    if (!await _permissionHandler.HasRolePermissionAsync(4002))
                    {
                        result.HasError = true;
                        result.Messages.Add("Edit District Permission Denied");
                        return result;
                    }
                    var checkDistrictExist = await _context.Districts
                       .FirstOrDefaultAsync(q => q.Id == request.Id);

                    if (checkDistrictExist == null)
                    {
                        result.HasError = true;
                        result.Messages.Add("Data does not exist!!!");
                        return result;
                    }


                    if (checkDistrictExist != null)
                    {
                        // Check if EnglishName or BanglaName exists in any other record
                        bool isDuplicate = await _context.Districts
                            .AnyAsync(d => d.Id != checkDistrictExist.Id &&
                                          (d.EnglishName == request.EnglishName || d.BanglaName == request.BanglaName));

                        if (isDuplicate)
                        {
                            result.HasError = true;
                            result.Messages.Add("This District Name is already existed!!!");
                            return result;
                        }


                        checkDistrictExist.EnglishName = request.EnglishName;
                        checkDistrictExist.BanglaName = request.BanglaName;
                        if(request.DivisionId>0)
                        checkDistrictExist.DivisionId = request.DivisionId;

                        _context.Districts.Update(checkDistrictExist);

                        if (await _context.SaveChangesAsync(cancellationToken) > 0)
                        {

                            result.HasError = false;
                            result.Messages.Add("District Info has been updated successfully!!");

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
                        result.Messages.Add("Create District Permission Denied");
                        return result;
                    }

                    var checkDistrictExist = await _context.Districts
                       .FirstOrDefaultAsync(q => q.EnglishName == request.EnglishName || q.BanglaName == request.BanglaName);

                    if (checkDistrictExist != null)
                    {
                        result.HasError = true;
                        result.Messages.Add("This District Name is already existed!!!");
                        return result;
                    }



                    var district = new District
                    {
                        EnglishName = request.EnglishName,
                        BanglaName = request.BanglaName,
                        DivisionId = request.DivisionId,
                        IsActive = true
                    };

                    _context.Districts.Add(district);
                    if (await _context.SaveChangesAsync(cancellationToken) > 0)
                    {

                        result.HasError = false;
                        result.Messages.Add("New District Info has been created successfully!!");

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

