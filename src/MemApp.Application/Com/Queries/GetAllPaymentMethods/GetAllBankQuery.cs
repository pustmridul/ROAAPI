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

namespace MemApp.Application.com.Queries.GetAllBanks
{
    public class GetAllBankQuery : IRequest<BankListVm>
    {
    }

    public class GetAllBankQueryHandler : IRequestHandler<GetAllBankQuery, BankListVm>
    {
        private readonly IMemDbContext _context;
        public GetAllBankQueryHandler(IMemDbContext context)
        {
            _context = context;
        }

        public async Task<BankListVm> Handle(GetAllBankQuery request, CancellationToken cancellationToken)
        {
            var result = new BankListVm();
            try
            {
                var data = await _context.Banks.ToListAsync(cancellationToken);

                if (data.Count == 0)
                {
                    result.HasError = true;
                    result.Messages.Add("Data Not Found");
                }
                else
                {
                    result.HasError = false;
                    result.DataCount = data.Count;
                    result.DataList = data.Select(s => new BankReq
                    {
                        slno = Convert.ToInt32(s.slno),
                        BANKNAME = s.BANKNAME,
                    }).ToList();
                }

            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.Messages.Add(ex.Message);

            }
           
            return result;
        }
    }
}
