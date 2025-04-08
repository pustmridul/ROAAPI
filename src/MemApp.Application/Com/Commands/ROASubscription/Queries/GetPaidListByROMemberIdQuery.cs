using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Subscription.Model;
using MemApp.Domain.Entities.subscription;
using Microsoft.EntityFrameworkCore;
using ResApp.Application.Com.Commands.ROAPayment.Models;
using ResApp.Application.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ResApp.Application.Com.Commands.ROASubscription.Queries
{
    
    public class GetPaidListByROMemberIdQuery : IRequest<ListResult<MemberSubPaymentRes>>
    {
        public int MemberId { get; set; }
    }

    public class GetPaidListByMemberIdQueryHandler : IRequestHandler<GetPaidListByROMemberIdQuery, ListResult<MemberSubPaymentRes>>
    {
        private readonly IMemDbContext _context;
        public GetPaidListByMemberIdQueryHandler(IMemDbContext context)
        {
            _context = context;
        }

        public async Task<ListResult<MemberSubPaymentRes>> Handle(GetPaidListByROMemberIdQuery request, CancellationToken cancellationToken)
        {

            try
            {
                var result = new ListResult<MemberSubPaymentRes>();

                var memberObj = await _context.MemberRegistrationInfos
                   .AsNoTracking()
                   .SingleOrDefaultAsync(s => s.Id == request.MemberId, cancellationToken);
              //  .Select(s=> new {s.Id, s.PaidTill, s.Phone, s.Email, s.MembershipNo, s.MemberTypeId});


            
                if (memberObj != null)
                {
                    
                                        

                      var  data = await _context.ROASubscriptionPaymentDetail.AsNoTracking()                        
                           .Where(q => q.MemberId== request.MemberId && q.IsPaid == true)
                           .OrderByDescending(c => c.SubscriptionMonth)
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
                        result.Data = data.Select(s => new MemberSubPaymentRes
                        {
                          MemberId = s.MemberId.GetValueOrDefault(),
                          PaymentAmount=s.PaymentFees,
                         // ActualPaymentDate=s.ActualPaymentDate,
                          LateAmount=s.LateFees,
                          Id = s.Id,
                          SubscriptionMonth=s.SubscriptionMonth,
                          SubscriptionName=s.SubscriptionName,
                          SubscriptionYear=s.SubscriptionYear,
                          MemberName=memberObj.Name,
                          ActualPaymentDate=s.ActualPaymentDate,
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
