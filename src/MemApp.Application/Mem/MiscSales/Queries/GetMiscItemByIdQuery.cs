using MediatR;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.MiscItems.Models;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Mem.MiscItems.Queries
{
    public class GetMiscItemByIdQuery : IRequest<MiscItemVm>
    {
        public int Id { get; set; }
    }


    public class GetMiscItemByIdQueryHandler : IRequestHandler<GetMiscItemByIdQuery, MiscItemVm>
    {
        private readonly IMemDbContext _context;
        public GetMiscItemByIdQueryHandler(IMemDbContext context)
        {
            _context = context;
        }

        public async Task<MiscItemVm> Handle(GetMiscItemByIdQuery request, CancellationToken cancellationToken)
        {
            var result = new MiscItemVm();
            var data = await _context.MiscItems.FirstOrDefaultAsync(q=>q.Id==request.Id, cancellationToken);
            if (data==null)
            {
                result.HasError = true;
                result.Messages.Add("Data Not Found");
            }
            else
            {
                result.HasError = false;

                result.Data = new MiscItemReq
                {
                    Id = data.Id,
                    Name = data.Name,
                    Price = data.Price,                  
                };
            }

            return result;
        }
    }
}
