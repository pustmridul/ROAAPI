using MediatR;
using MemApp.Application.Com.Models;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.AreaLayouts.Models;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Mem.AreaLayouts.Queries
{
    public class GetAreaLayoutTableDetailsByIdQuery : IRequest<AreaLayoutTableDetailVm>
    {
        public int Id { get; set; }
    }


    public class GetAreaLayoutTableDetailsByIdQueryHandler : IRequestHandler<GetAreaLayoutTableDetailsByIdQuery, AreaLayoutTableDetailVm>
    {
        private readonly IMemDbContext _context;
        public GetAreaLayoutTableDetailsByIdQueryHandler(IMemDbContext context)
        {
            _context = context;
        }

        public async Task<AreaLayoutTableDetailVm> Handle(GetAreaLayoutTableDetailsByIdQuery request, CancellationToken cancellationToken)
        {
            var result = new AreaLayoutTableDetailVm();

            var data = await _context.AreaTableMatrixs
                .Include(i => i.AreaLayout)
                .Include(i => i.TableSetup)
                .Where(q => q.IsActive && q.AreaLayoutId == request.Id).ToListAsync(cancellationToken);



            if (data==null)
            {
                result.HasError = true;
                result.Messages.Add("Data Not Found");
            }
            else
            {
                result.HasError = false;
                var abc = new AreaLayoutTableDetail();
                abc.Title = data.FirstOrDefault()!.AreaLayout.Title;
                abc.DisplayName = data.FirstOrDefault()!.AreaLayout.DisplayName!;

                var tableList= data.Select(i=>i.TableSetup).Distinct().ToList();
                foreach(var d in tableList)
                {
                    var t = new TableDetailVm();
                    t.Id = d.Id;
                    t.TableName = d.Title;

                    var chairList= data.Where(q=>q.TableId== d.Id).ToList();

                    foreach(var c in chairList)
                    {
                        var cv = new ChairVm();
                        cv.ChairNo= c.ChairNo;
                        t.ChairList.Add(cv);
                    }
                    abc.TableListVm.Add(t);
                }
                result.Data = abc;
            }

            return result;
        }
    }
}
