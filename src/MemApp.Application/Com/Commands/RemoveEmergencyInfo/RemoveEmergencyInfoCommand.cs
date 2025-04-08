using MediatR;
using MemApp.Application.Interfaces.Contexts;
using Microsoft.EntityFrameworkCore;
using MemApp.Application.Extensions;

namespace MemApp.Application.Com.Commands.UserConferences
{
    public class RemoveEmergencyInfoCommand : IRequest<Result>
    {
        public int Id { get; set; } 
    }

    public class RemoveEmergencyInfoCommandHandler : IRequestHandler<RemoveEmergencyInfoCommand, Result>
    {
        private readonly IMemDbContext _context;
        public RemoveEmergencyInfoCommandHandler(IMemDbContext context)
        {
            _context = context;        
        }
        public async Task<Result> Handle(RemoveEmergencyInfoCommand request, CancellationToken cancellation)
        {
            var result = new Result();
            try
            {

                var obj = await _context.EmergencyInfos
                            .SingleOrDefaultAsync(q => q.Id == request.Id, cancellation);
                if (obj == null)
                {
                    result.Succeeded = false;
                    result.Messages.Add("Remove Fail");
                }
                else {
                    obj.IsActive = false;
                    await _context.SaveChangesAsync(cancellation);
                    result.Succeeded = true;
                    result.Messages.Add("Remove success");
                }
            }
            catch (Exception ex) { 
                result.Succeeded = false;
                result.Messages.Add(ex.Message);
            }
            return result;
        }
    }
}
