using MediatR;
using MemApp.Application.Com.Models;
using MemApp.Application.Interfaces.Contexts;
using Microsoft.EntityFrameworkCore;
using MemApp.Domain.Entities.com;
using MemApp.Application.Extensions;

namespace MemApp.Application.Com.Commands.UserConferences
{
    public class CreateEmergencyInfoCommand : IRequest<Result>
    {
        public EmergencyInfoReq Model { get; set; } = new EmergencyInfoReq();
    }

    public class CreateEmergencyInfoCommandHandler : IRequestHandler<CreateEmergencyInfoCommand, Result>
    {
        private readonly IMemDbContext _context;
        public CreateEmergencyInfoCommandHandler(IMemDbContext context)
        {
            _context = context;        
        }
        public async Task<Result> Handle(CreateEmergencyInfoCommand request, CancellationToken cancellation)
        {
            var result = new Result();
            try
            {

                var obj = await _context.EmergencyInfos
                            .SingleOrDefaultAsync(q => q.Id == request.Model.Id, cancellation);

                if (obj == null)
                {
                    obj = new EmergencyInfo()
                    {
                       IsActive = true,
                    };
                    _context.EmergencyInfos.Add(obj);
                    result.Succeeded = true;
                    result.Messages.Add("Created success");
                }

                obj.Name = request.Model.Name;
                obj.ContactNo = request.Model.ContactNo;
                obj.Address = request.Model.Address;

                await _context.SaveChangesAsync(cancellation);
              
            }
            catch (Exception ex) { 
                result.Succeeded = false;
                result.Messages.Add( ex.Message);
            }
            return result;
        }
    }
}
