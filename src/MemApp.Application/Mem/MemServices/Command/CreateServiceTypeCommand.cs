using MediatR;
using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Service.Model;
using MemApp.Domain.Entities.services;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Mem.MemServices.Command
{
    public class CreateServiceTypeCommand : IRequest<ServiceTypeVm>
    {
        public ServiceTypeReq Model { get; set; } = new ServiceTypeReq();
    }

    public class CreateServiceTypeCommandHandler : IRequestHandler<CreateServiceTypeCommand, ServiceTypeVm>
    {
        private readonly IMemDbContext _context;
        private readonly ICurrentUserService _currentUserService;
       
        public CreateServiceTypeCommandHandler(IMemDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }
        public async Task<ServiceTypeVm> Handle(CreateServiceTypeCommand request, CancellationToken cancellation)
        {
            if (_currentUserService.UserId == null || _currentUserService.UserId == 0)
            {
                throw new UnauthorizedAccessException();
            }
            var result = new ServiceTypeVm();
            var obj = await _context.ServiceTypes.SingleOrDefaultAsync
                (q => q.Id == request.Model.Id);
            if (obj == null)
            {
                obj = new ServiceType();
                obj.IsActive = true;
                _context.ServiceTypes.Add(obj);
                result.HasError = false;
                result.Messages.Add("New Service Type created");
            }
            else
            {
                result.HasError = false;
                result.Messages.Add("Service Type Updated");
            }

            obj.Title = request.Model.Title;
            obj.DisplayName = request.Model.DisplayName;

            if (await _context.SaveChangesAsync(cancellation) > 0)
            {

                result.Data.Title = obj.Title;
                result.Data.Id = obj.Id;
                result.Data.DisplayName = obj.DisplayName;
            }
            else
            {
                result.HasError = true;
                result.Messages.Add("something wrong");
            }
            return result;
        }

    }

}
