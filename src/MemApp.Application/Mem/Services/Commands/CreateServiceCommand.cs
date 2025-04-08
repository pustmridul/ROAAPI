using MediatR;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Interfaces;
using MemApp.Domain.Entities.mem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MemApp.Application.Mem.Services.Models;
using MemApp.Application.Mem.MemServices.Models;
using MemApp.Application.Services;
using MemApp.Domain.Entities.ser;
using MemApp.Domain.Entities.services;

namespace MemApp.Application.Mem.Services.Commands
{

    public class CreateServiceCommand : IRequest<ServiceVm>
    {
        public ServiceReq Model { get; set; } = new ServiceReq();
        public string WebRootPath { get; set; } = string.Empty;
    }

    public class ServiceCommandHandler : IRequestHandler<CreateServiceCommand, ServiceVm>
    {
        private readonly IMemDbContext _context;

        private readonly ICurrentUserService _currentUserService;
        private readonly IPermissionHandler _permissionHandler;


        public ServiceCommandHandler(IMemDbContext context, IMediator mediator, ICurrentUserService currentUserService, IPermissionHandler permissionHandler)
        {
            _context = context;
            _currentUserService = currentUserService;
            _permissionHandler = permissionHandler;
    }

        public async Task<ServiceVm> Handle(CreateServiceCommand request, CancellationToken cancellationToken)
        {
            if (_currentUserService?.UserId == null || _currentUserService.UserId == 0)
            {
                throw new UnauthorizedAccessException();
            }
            var result = new ServiceVm();
            try
            {
                var obj = await _context.ServiceRecords
                .SingleOrDefaultAsync(q => q.Id == request.Model.Id);
                if (obj == null)
                {
                    if (!await _permissionHandler.HasRolePermissionAsync(1101))
                    {
                        result.HasError = true;
                        result.Messages?.Add("MemService create Permission Denied");
                        return result;
                    }
          
                    obj = new ServiceRecord();

                    _context.ServiceRecords.Add(obj);
                    result.HasError = false;
                    result.Messages?.Add("New MemService created");
                }
                else
                {
                    if (!await _permissionHandler.HasRolePermissionAsync(1102))
                    {
                        result.HasError = true;
                        result.Messages?.Add("MemService Update Permission Denied");
                        return result;
                    }

                    result.HasError = false;
                    result.Messages?.Add("MemService Updated");
                }

                obj.Title = request.Model.Title;
                obj.ServiceTypeId = request.Model.ServiceTypeId;
      

                if (await _context.SaveChangesAsync(cancellationToken) > 0)
                {
                    result.Data.Title = obj.Title;
                    result.Data.Id = obj.Id;
                }
                else
                {
                    result.HasError = true;
                    result.Messages?.Add("something wrong");
                }

            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.Messages?.Add("Exception" + ex.Message);
            }


            return result;
        }
    }
}
