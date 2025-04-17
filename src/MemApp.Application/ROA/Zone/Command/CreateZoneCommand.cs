using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Interfaces;
using Res.Domain.Entities;
using ResApp.Application.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ResApp.Application.ROA.Zone.Models;
using Microsoft.EntityFrameworkCore;

namespace ResApp.Application.ROA.Zone.Command
{
   
    public class CreateZoneCommand : IRequest<Result<ZoneDto>>
    {
        public int Id { get; set; }

        public string? EnglishName { get; set; }
        public string? BanglaName { get; set; }
        public DateTime? CreatedOn { get; set; }

        public int DistrictId { get; set; }
    }

    public class CreateZoneCommandHandler : IRequestHandler<CreateZoneCommand, Result<ZoneDto>>
    {
        private readonly IMemDbContext _context;
        private readonly IMediator _mediator;
        private readonly IPermissionHandler _permissionHandler;
        private readonly ICurrentUserService _currentUserService;

        //  private readonly IWebHostEnvironment _hostingEnv;
        public CreateZoneCommandHandler(IMemDbContext context, IMediator mediator, IPermissionHandler permissionHandler, ICurrentUserService currentUserService)
        {
            _context = context;
            _mediator = mediator;
            _currentUserService = currentUserService;
            _permissionHandler = permissionHandler;
        }

        public async Task<Result<ZoneDto>> Handle(CreateZoneCommand request, CancellationToken cancellationToken)
        {
            try
            {

                if (_currentUserService?.UserId == null || _currentUserService.UserId == 0)
                {
                    throw new UnauthorizedAccessException();
                }


                var result = new Result<ZoneDto>();

                if (request.Id > 0)
                {
                    if (!await _permissionHandler.HasRolePermissionAsync(4002))
                    {
                        result.HasError = true;
                        result.Messages.Add("Edit Zone Permission Denied");
                        return result;
                    }
                    var checkZoneExist = await _context.ZoneInfos
                       .FirstOrDefaultAsync(q => q.Id == request.Id);

                    if (checkZoneExist == null)
                    {
                        result.HasError = true;
                        result.Messages.Add("Data does not exist!!!");
                        return result;
                    }


                    if (checkZoneExist != null)
                    {
                        // Check if EnglishName or BanglaName exists in any other record
                        bool isDuplicate = await _context.ZoneInfos
                            .AnyAsync(d => d.Id != checkZoneExist.Id &&
                                          (d.EnglishName == request.EnglishName || d.BanglaName == request.BanglaName));

                        if (isDuplicate)
                        {
                            result.HasError = true;
                            result.Messages.Add("This Zone Name is already existed!!!");
                            return result;
                        }


                        checkZoneExist.EnglishName = request.EnglishName;
                        checkZoneExist.BanglaName = request.BanglaName;
                        checkZoneExist.DistrictId = request.DistrictId;

                        _context.ZoneInfos.Update(checkZoneExist);

                        if (await _context.SaveChangesAsync(cancellationToken) > 0)
                        {

                            result.HasError = false;
                            result.Messages.Add("Zone Info has been updated successfully!!");

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
                        result.Messages.Add("Create Zone Permission Denied");
                        return result;
                    }

                    var checkZoneExist = await _context.ZoneInfos
                       .FirstOrDefaultAsync(q => q.EnglishName == request.EnglishName || q.BanglaName == request.BanglaName);

                    if (checkZoneExist != null)
                    {
                        result.HasError = true;
                        result.Messages.Add("This Zone Name is already existed!!!");
                        return result;
                    }



                    var zone = new ZoneInfo
                    {
                        EnglishName = request.EnglishName,
                        BanglaName = request.BanglaName,
                        DistrictId = request.DistrictId,
                        IsActive = true
                    };

                    _context.ZoneInfos.Add(zone);
                    if (await _context.SaveChangesAsync(cancellationToken) > 0)
                    {

                        result.HasError = false;
                        result.Messages.Add("New Zone Info has been created successfully!!");

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
