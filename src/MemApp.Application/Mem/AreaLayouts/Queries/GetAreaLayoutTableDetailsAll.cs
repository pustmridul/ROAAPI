using MediatR;
using MemApp.Application.Com.Models;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.AreaLayouts.Models;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Mem.AreaLayouts.Queries
{
    public class GetAreaLayoutTableDetailsAll : IRequest<AreaLayoutListVm>
    {
        public int Id { get; set; }
    }


    public class GetAreaLayoutTableDetailsAllHandler : IRequestHandler<GetAreaLayoutTableDetailsAll, AreaLayoutListVm>
    {
        private readonly IMemDbContext _context;
        public GetAreaLayoutTableDetailsAllHandler(IMemDbContext context)
        {
            _context = context;
        }

        public async Task<AreaLayoutListVm> Handle(GetAreaLayoutTableDetailsAll request, CancellationToken cancellationToken)
        {
            var result = new AreaLayoutListVm();

            var data = await _context.AreaLayouts
                .Include(i=>i.AreaLayoutDetails.Where(q=>q.IsActive)).ThenInclude(i=>i.TableSetup)
                .Where(q => q.IsActive).ToListAsync(cancellationToken);



            if (data.Count==0)
            {
                result.HasError = true;
                result.Messages.Add("Data Not Found");
            }
            else
            {
                result.HasError = false;
                result.DataList = data.Select(s => new AreaLayoutReq
                {
                    Id = s.Id,
                    Title = s.Title,
                    DisplayName = s.DisplayName,
                    Status = s.Status,
                    IsActive = s.IsActive,
                    AreaLayoutDetails = s.AreaLayoutDetails.Select(s => new AreaLayoutDetailReq
                    {
                        AreaLayoutId = s.AreaLayoutId,
                        TableId = s.TableId,
                        TableName = s.TableSetup.Title,                      
                        NumberOfChair = s.NumberOfChair,
                    }).ToList()
                }).ToList();
            }

            return result;
        }
    }
}
