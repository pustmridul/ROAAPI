using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Colleges.Models;
using System.Text;
using MemApp.Application.Services;
using Dapper;
using Microsoft.EntityFrameworkCore;
using MemApp.Application.Interfaces;

namespace MemApp.Application.Mem.Colleges.Queries
{
    public class GetAllCommCatQuery : IRequest<CommCatListVm>
    {
    }

    public class GetAllCommCatQueryHandler : IRequestHandler<GetAllCommCatQuery, CommCatListVm>
    {
        private readonly IMemDbContext _context;
        private readonly IPermissionHandler _permissionHandler;
        public GetAllCommCatQueryHandler(IMemDbContext context, IPermissionHandler permissionHandler)
        {
            _context = context;
            _permissionHandler = permissionHandler;
        }

        public async Task<CommCatListVm> Handle(GetAllCommCatQuery request, CancellationToken cancellationToken)
        {
            var result = new CommCatListVm();
            try
            {
                var data = await _context.CommitteeCategories
                    .Where(q => q.IsActive)
                    .AsNoTracking()
                    .ToListAsync(cancellationToken);

                if (data.Count == 0)
                {
                    result.HasError = true;
                    result.Messages?.Add("Data Not Found");
                }
                else
                {
                    result.HasError = false;
                    result.DataCount = data.Count;
                    result.DataList = data.Select(s => new CommCatReq
                    {
                        Id = s.Id,
                        Title= s.Title
                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.Messages?.Add(ex.ToString());
            }
            
            return result;
        }
    }
}
