using MediatR;
using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Committees.Models;
using MemApp.Domain.Entities.mem;
using Microsoft.EntityFrameworkCore;
using System.Security;

namespace MemApp.Application.Mem.Committees.Queries
{
    public class GetCommitteeListQuery : IRequest<CommitteeDDListVm>
    {
      
    }

    public class GetCommitteeListQueryHandler : IRequestHandler<GetCommitteeListQuery, CommitteeDDListVm>
    {
        private readonly IMemDbContext _context;
        private readonly IPermissionHandler _permissionHandler;
        public GetCommitteeListQueryHandler(IMemDbContext context, IPermissionHandler permissionHandler)
        {
            _context = context;
            _permissionHandler = permissionHandler;
        }

        public async Task<CommitteeDDListVm> Handle(GetCommitteeListQuery request, CancellationToken cancellationToken)
        {
            var result = new CommitteeDDListVm();

            try
            {
                var dataList = await _context.Committees.Where(q=>q.IsActive).ToListAsync(cancellationToken);

                result.DataList = dataList.Select(s => new CommitteeDD
                {
                    CommitteeId = s.Id,
                    CommitteeTitle = s.Title,
                    CommitteeType = s.CommitteeType
                }).ToList();
                result.DataCount = dataList.Count();
            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.Messages.Add(ex.ToString());
            }
            
            return result;
        }
    }
}
