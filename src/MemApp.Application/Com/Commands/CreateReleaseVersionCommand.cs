using MediatR;
using MemApp.Application.Com.Models;
using MemApp.Application.Interfaces.Contexts;
using Microsoft.EntityFrameworkCore;
using MemApp.Domain.Entities.com;

namespace MemApp.Application.Com.Commands
{
    public class CreateReleaseVersionCommand : IRequest<bool>
    {
        public ReleaseVersionReq Model { get; set; } = new ReleaseVersionReq();
    }

    public class CreateReleaseVersionCommandHandler : IRequestHandler<CreateReleaseVersionCommand, bool>
    {
        private readonly IMemDbContext _context;
        public CreateReleaseVersionCommandHandler(IMemDbContext context)
        {
            _context = context;        
        }
        public async Task<bool> Handle(CreateReleaseVersionCommand request, CancellationToken cancellation)
        {
            var obj = await _context.ReleaseVersions
                .SingleOrDefaultAsync(q => q.Id == request.Model.Id, cancellation);

            if (obj == null)
            {
                obj = new ReleaseVersion()
                {
                    ReleaseTitle = request.Model.ReleaseTitle,
                    ReleaseDate = request.Model.ReleaseDate,
                    AppId = request.Model.AppId,
                    IsRequired=request.Model.IsRequired,
                    ReleaseType= request.Model.ReleaseType
                };
                _context.ReleaseVersions.Add(obj);
            }
          
            return await _context.SaveChangesAsync(cancellation) > 0 ? true : false;
        }
    }
}
