using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using Microsoft.EntityFrameworkCore;
using ResApp.Application.ROA.MembershipFee.Models;
using ResApp.Application.ROA.SubscriptionPayment.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResApp.Application.ROA.MembershipFee.Queries
{
   

    public class GetAllMembershipFeePaymentQuery : IRequest<ListResult<MembershipFeePymentRes>>
    {
        public string? MemberShipNo { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }
    }

    public class GetAllMembershipFeePaymentQueryHandler : IRequestHandler<GetAllMembershipFeePaymentQuery, ListResult<MembershipFeePymentRes>>
    {
        private readonly IMemDbContext _context;
        public GetAllMembershipFeePaymentQueryHandler(IMemDbContext context)
        {
            _context = context;
        }

        public async Task<ListResult<MembershipFeePymentRes>> Handle(GetAllMembershipFeePaymentQuery request, CancellationToken cancellationToken)
        {
            try
            {
                DateTime st = request.StartDate.AddDays(-1);
                DateTime ed = request.EndDate.AddDays(1);
                var result = new ListResult<MembershipFeePymentRes>();


                var data = await _context.ROAMembershipFeePayments
                    .Include(i => i.Member)
                    .Where(q => q.PaymentDate >= st && q.PaymentDate <= ed)                  
                    .Select(s => new MembershipFeePymentRes
                    {
                      Id = s.Id,
                      MemberId = s.MemberId,
                      Amount = s.Amount,
                      MemberName=s.Member.Name,
                      MembershipNo=s.Member.MemberShipNo,
                      PaymentDate = s.PaymentDate,
                      PaymentNo=s.PaymentNo,
                    })
                .OrderByDescending(o => o.PaymentDate)
                .AsNoTracking()
                .ToPaginatedListAsync(request.PageNo, request.PageSize, cancellationToken);

               
                result.Count = Convert.ToInt32(data.TotalCount);
                result.HasError = false;
                result.Data = data.Data;
                return result;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
    }
}
