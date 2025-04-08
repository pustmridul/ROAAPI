using MediatR;
using MemApp.Application.Com.Models;
using MemApp.Application.Interfaces.Contexts;
using Microsoft.EntityFrameworkCore;


namespace MemApp.Application.Mem.MemberTypes.Queries
{
    public class GetReleaseVersionQuery : IRequest<ReleaseVersionVm>
    {
        public string AppId { get; set; }
        public string ReleaseType { get; set; }

    }

    public class GetReleaseVersionQueryHandler : IRequestHandler<GetReleaseVersionQuery, ReleaseVersionVm>
    {
        private readonly IMemDbContext _context;
        public GetReleaseVersionQueryHandler(IMemDbContext context)
        {
            _context = context;
        }

        public async Task<ReleaseVersionVm> Handle(GetReleaseVersionQuery request, CancellationToken cancellationToken)
        {
            var result = new ReleaseVersionVm();
            var data = await _context.ReleaseVersions
                .OrderByDescending(o => o.ReleaseDate)
                .FirstOrDefaultAsync(q => q.AppId == request.AppId && q.ReleaseType== request.ReleaseType, cancellationToken);
                
                

            if (data==null)
            {
                result.HasError = true;
                result.Messages.Add("Data Not Found");
            }
            else
            {
                result.HasError = false;
                result.Data.ReleaseTitle= data.ReleaseTitle;
                result.Data.ReleaseType= data.ReleaseType;
                result.Data.ReleaseDate = data.ReleaseDate;
                result.Data.IsRequired = data.IsRequired;
                result.Data.AppId = data.AppId;
                result.Data.Id = data.Id;
            }

            return result;
        }
    }
}
