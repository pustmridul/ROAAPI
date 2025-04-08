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

namespace ResApp.Application.Com.Commands.Division.Update
{
    public class UpdateDivisionCommand : IRequest<Result<DivisionDto>>
    {
         public int Id { get; set; }

        public string? EnglishName { get; set; }
        public string? BanglaName { get; set; }
        public DateTime? CreatedOn { get; set; }

        public int DistrictId { get; set; }
    }

    public class UpdateDivisionCommandHandler : IRequestHandler<UpdateDivisionCommand, Result<DivisionDto>>
    {
        private readonly IMemDbContext _context;
        private readonly IMediator _mediator;
        private readonly IPermissionHandler _permissionHandler;
        private readonly ICurrentUserService _currentUserService;

        //  private readonly IWebHostEnvironment _hostingEnv;
        public UpdateDivisionCommandHandler(IMemDbContext context, IMediator mediator, IPermissionHandler permissionHandler, ICurrentUserService currentUserService)
        {
            _context = context;
            _mediator = mediator;
            _currentUserService = currentUserService;
            _permissionHandler = permissionHandler;
        }

        public async Task<Result<DivisionDto>> Handle(UpdateDivisionCommand request, CancellationToken cancellationToken)
        {

            try
            {

                if (_currentUserService?.UserId == null || _currentUserService.UserId == 0)
                {
                    throw new UnauthorizedAccessException();
                }

              //  var checkAdmin = _currentUserService.Current().UserName;

                var result = new Result<DivisionDto>();
                //if (checkAdmin != "Super Admin")
                //{
                //    result.HasError = true;
                //    result.Messages.Add("Invalid request!!!");
                //    return result;
                //}

                if (request.Id > 0)
                {
                    if (!await _permissionHandler.HasRolePermissionAsync(4002))
                    {
                        result.HasError = true;
                        result.Messages.Add("Edit Division Permission Denied");
                        return result;
                    }
                    var checkDivisionExist = await _context.Divisions
                       .FirstOrDefaultAsync(q => q.Id == request.Id);

                    if (checkDivisionExist == null)
                    {
                        result.HasError = true;
                        result.Messages.Add("Data does not exist!!!");
                        return result;
                    }


                    if (checkDivisionExist != null)
                    {
                        // Check if EnglishName or BanglaName exists in any other record
                        bool isDuplicate = await _context.Divisions
                            .AnyAsync(d => d.Id != checkDivisionExist.Id &&
                                          (d.EnglishName == request.EnglishName || d.BanglaName == request.BanglaName));

                        if (isDuplicate)
                        {
                            result.HasError = true;
                            result.Messages.Add("This Division Name is already existed!!!");
                            return result;
                        }


                        checkDivisionExist.EnglishName = request.EnglishName;
                            checkDivisionExist.BanglaName = request.BanglaName;

                            _context.Divisions.Update(checkDivisionExist);

                            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                            {

                                result.HasError = false;
                                result.Messages.Add("Division Info has been updated successfully!!");

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
                        result.Messages.Add("Create Division Permission Denied");
                        return result;
                    }

                    var checkDivisionExist = await _context.Divisions
                       .FirstOrDefaultAsync(q => q.EnglishName == request.EnglishName || q.BanglaName == request.BanglaName);

                    if (checkDivisionExist != null)
                    {
                        result.HasError = true;
                        result.Messages.Add("This Division Name is already existed!!!");
                        return result;
                    }

                    

                    var division = new Res.Domain.Entities.Division
                    {
                        EnglishName= request.EnglishName,
                        BanglaName=request.BanglaName,
                        IsActive=true
                    };

                    _context.Divisions.Add(division);
                    if (await _context.SaveChangesAsync(cancellationToken) > 0)
                    {

                        result.HasError = false;
                        result.Messages.Add("New Division Info has been created successfully!!");

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

