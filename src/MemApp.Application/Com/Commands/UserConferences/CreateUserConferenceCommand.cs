using MediatR;
using MemApp.Application.Com.Models;
using MemApp.Application.Interfaces.Contexts;
using Microsoft.EntityFrameworkCore;
using MemApp.Domain.Entities;

namespace MemApp.Application.Com.Commands.UserConferences
{
    public class CreateUserConferenceCommand : IRequest<bool>
    {
        public UserConferenceReq Model { get; set; } = new UserConferenceReq();
    }

    public class CreateUserConferenceCommandHandler : IRequestHandler<CreateUserConferenceCommand, bool>
    {
        private readonly IMemDbContext _context;
        public CreateUserConferenceCommandHandler(IMemDbContext context)
        {
            _context = context;        
        }
        public async Task<bool> Handle(CreateUserConferenceCommand request, CancellationToken cancellation)
        {
            var obj = await _context.UserConferences
                .SingleOrDefaultAsync(q => q.UserId == request.Model.UserId.ToString(), cancellation);

            if (obj == null)
            {
                obj = new UserConference()
                {
                    UserId = request.Model.UserId.ToString(),                                 
                };
                _context.UserConferences.Add(obj);
            }
          
                obj.UserName= request.Model.UserName;
                obj.AppId= request.Model.AppId;
                obj.IpAddress= request.Model.IpAddress;
                obj.UserRefToken   = request.Model.UserRefToken;
                obj.UserToken = request.Model.UserToken;
                obj.LogInDate = DateTime.Now;
                obj.LogOutDate= request.Model?.LogOutDate;
                obj.LogOutStatus = request.Model.LogOutStatus;
          
            return await _context.SaveChangesAsync(cancellation) > 0 ? true : false;
        }
    }
}
