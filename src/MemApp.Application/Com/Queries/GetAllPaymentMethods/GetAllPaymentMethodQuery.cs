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

namespace MemApp.Application.com.Queries.GetAllPaymentMethods
{
    public class GetAllPaymentMethodQuery : IRequest<PaymentMethodListVm>
    {
    }

    public class GetAllPaymentMethodQueryHandler : IRequestHandler<GetAllPaymentMethodQuery, PaymentMethodListVm>
    {
        private readonly IMemDbContext _context;
        public GetAllPaymentMethodQueryHandler(IMemDbContext context)
        {
            _context = context;
        }

        public async Task<PaymentMethodListVm> Handle(GetAllPaymentMethodQuery request, CancellationToken cancellationToken)
        {
            var result = new PaymentMethodListVm();
            var data = await _context.PaymentMethods.ToListAsync(cancellationToken);

            if (data.Count==0)
            {
                result.HasError = true;
                result.Messages.Add("Data Not Found");
            }
            else
            {
                result.HasError = false;
                result.DataCount= data.Count;
                result.DataList =data.Select(s=> new PaymentMethodRes
                {
                    Id = s.PaymentTypeID,
                    Title = s.Name,
                }).ToList();
            }

            return result;
        }
    }
}
