using MediatR;
using MemApp.Application.Com.Models;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Members.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Application.com.Queries.GetAllCreditCards
{
    public class GetAllCreditCardQuery : IRequest<CreditCardListVm>
    {
    }

    public class GetAllCreditCardQueryHandler : IRequestHandler<GetAllCreditCardQuery, CreditCardListVm>
    {
        private readonly IMemDbContext _context;
        public GetAllCreditCardQueryHandler(IMemDbContext context)
        {
            _context = context;
        }

        public async Task<CreditCardListVm> Handle(GetAllCreditCardQuery request, CancellationToken cancellationToken)
        {
            var result = new CreditCardListVm();
            var data = await _context.CreditCards.ToListAsync(cancellationToken);

            if (data.Count==0)
            {
                result.HasError = true;
                result.Messages.Add("Data Not Found");
            }
            else
            {
                result.HasError = false;
                result.DataCount= data.Count;
                result.DataList =data.Select(s=> new CreditCardReq
                {
                    slno =Convert.ToInt32(s.slno),
                    CardName = s.CardName,
                }).ToList();
            }

            return result;
        }
    }
}
