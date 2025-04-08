using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using Microsoft.EntityFrameworkCore;
using ResApp.Application.Com.Commands.RoaMembershipFee.Models;
using ResApp.Application.Com.Commands.ROAPayment.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResApp.Application.Com.Commands.RoaMembershipFee.Queries
{
    
    public class GetPaidByROMemberIdQuery : IRequest<ListResult<MembershipFeePymentRes>>
    {
        public int MemberId { get; set; }
    }

    public class GetPaidByROMemberIdQueryHandler : IRequestHandler<GetPaidByROMemberIdQuery, ListResult<MembershipFeePymentRes>>
    {
        private readonly IMemDbContext _context;
        public GetPaidByROMemberIdQueryHandler(IMemDbContext context)
        {
            _context = context;
        }

        public async Task<ListResult<MembershipFeePymentRes>> Handle(GetPaidByROMemberIdQuery request, CancellationToken cancellationToken)
        {

            try
            {
                var result = new ListResult<MembershipFeePymentRes>();

                var memberObj = await _context.MemberRegistrationInfos
                   .AsNoTracking()
                   .SingleOrDefaultAsync(s => s.Id == request.MemberId, cancellationToken);
                //  .Select(s=> new {s.Id, s.PaidTill, s.Phone, s.Email, s.MembershipNo, s.MemberTypeId});



                if (memberObj != null)
                {



                    var data = await _context.ROAMembershipFeePayments.AsNoTracking()
                         .Where(q => q.MemberId == request.MemberId)
                        // .OrderByDescending(c => c.SubscriptionMonth)
                         .AsNoTracking()
                         .ToListAsync(cancellationToken);


                    if (data.Count == 0)
                    {
                        result.HasError = true;
                        result.Messages.Add("Data Not Found");
                    }
                    else
                    {
                        result.HasError = false;
                        result.Count = data.Count;
                        result.Data = data.Select(s => new MembershipFeePymentRes
                        {
                            MemberId = s.MemberId,
                            Amount = s.Amount,                          
                            Id = s.Id,
                            PaymentNo = s.PaymentNo,
                            MemberName = memberObj.Name,
                            PaymentDate = s.PaymentDate,
                            // IsChecked=false
                            // PaymentDate=s.



                        }).ToList();

                    }




                }
                else
                {
                    result.HasError = true;
                    result.Messages?.Add("Data Not Found");
                }



                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

    }
}
