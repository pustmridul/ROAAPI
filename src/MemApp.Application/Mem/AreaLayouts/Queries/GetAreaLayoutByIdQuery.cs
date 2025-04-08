using MediatR;
using MemApp.Application.Com.Models;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.AreaLayouts.Models;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Mem.AreaLayouts.Queries
{
    public class GetAreaLayoutByIdQuery : IRequest<AreaLayoutVm>
    {
        public int Id { get; set; }
    }


    public class GetAreaLayoutByIdQueryHandler : IRequestHandler<GetAreaLayoutByIdQuery, AreaLayoutVm>
    {
        private readonly IMemDbContext _context;
        public GetAreaLayoutByIdQueryHandler(IMemDbContext context)
        {
            _context = context;
        }

        public async Task<AreaLayoutVm> Handle(GetAreaLayoutByIdQuery request, CancellationToken cancellationToken)
        {
            var result = new AreaLayoutVm();
            var data = await _context.AreaLayouts
                .Include(i=>i.AreaLayoutDetails.Where(q=>q.IsActive)).ThenInclude(i=>i.TableSetup)
                .SingleOrDefaultAsync(q=>q.Id==request.Id && q.IsActive, cancellationToken);

            if (data==null)
            {
                result.HasError = true;
                result.Messages.Add("Data Not Found");
            }
            else
            {
                result.HasError = false;

                result.Data = new AreaLayoutReq
                {
                    Id = data.Id,
                    Title = data.Title,
                    DisplayName= data.DisplayName,
                    AreaLayoutDetails= data.AreaLayoutDetails.Select(s=>new AreaLayoutDetailReq()
                    { 
                        TableName = s.TableSetup.Title,
                        NumberOfChair=s.NumberOfChair,
                        AreaLayoutId=s.AreaLayoutId,
                        IsActive=s.IsActive,
                        Id=s.Id,
                        TableId=s.TableId,
                    }).ToList(),
                };
            }

            return result;
        }
    }
}
