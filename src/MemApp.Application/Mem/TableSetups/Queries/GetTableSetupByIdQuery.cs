using MediatR;
using MemApp.Application.Com.Models;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.TableSetups.Models;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Mem.TableSetups.Queries
{
    public class GetTableSetupByIdQuery : IRequest<TableSetupVm>
    {
        public int Id { get; set; }
    }


    public class GetTableSetupByIdQueryHandler : IRequestHandler<GetTableSetupByIdQuery, TableSetupVm>
    {
        private readonly IMemDbContext _context;
        public GetTableSetupByIdQueryHandler(IMemDbContext context)
        {
            _context = context;
        }

        public async Task<TableSetupVm> Handle(GetTableSetupByIdQuery request, CancellationToken cancellationToken)
        {
            var result = new TableSetupVm();
            var data = await _context.TableSetups
                .SingleOrDefaultAsync(q=>q.Id==request.Id && q.IsActive, cancellationToken);

            if (data==null)
            {
                result.HasError = true;
                result.Messages.Add("Data Not Found");
            }
            else
            {
                result.HasError = false;

                result.Data = new TableSetupReq
                {
                    Id = data.Id,
                    Title = data.Title,
                    DisplayName= data.DisplayName                  
                };
            }

            return result;
        }
    }
}
