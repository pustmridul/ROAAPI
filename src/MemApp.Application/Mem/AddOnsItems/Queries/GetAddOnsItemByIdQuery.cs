using MediatR;
using MemApp.Application.Com.Models;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.AddOnsItems.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Application.Mem.AddOnsItems.Queries
{
    public class GetAddOnsItemByIdQuery : IRequest<AddOnsItemVm>
    {
        public int Id { get; set; }
    }


    public class GetAddOnsItemByIdQueryHandler : IRequestHandler<GetAddOnsItemByIdQuery, AddOnsItemVm>
    {
        private readonly IMemDbContext _context;
        public GetAddOnsItemByIdQueryHandler(IMemDbContext context)
        {
            _context = context;
        }

        public async Task<AddOnsItemVm> Handle(GetAddOnsItemByIdQuery request, CancellationToken cancellationToken)
        {
            var result = new AddOnsItemVm();
            var data = await _context.AddOnsItems
                .SingleOrDefaultAsync(q=>q.Id==request.Id, cancellationToken);
            if (data==null)
            {
                result.HasError = true;
                result.Messages.Add("Data Not Found");
            }
            else
            {
                result.HasError = false;

                result.Data = new AddOnsItemReq
                {
                    Id = data.Id,
                    Title = data.Title,
                    Description= data.Description,
                    Price= data.Price,
                    PriceDate =  data.PriceDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
                };
            }

            return result;
        }
    }
}
