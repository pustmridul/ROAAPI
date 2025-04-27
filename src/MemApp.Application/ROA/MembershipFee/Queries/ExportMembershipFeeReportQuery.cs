using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using Microsoft.EntityFrameworkCore;
using ResApp.Application.Com.Commands.ROAPayment.Models;
using ResApp.Application.ROA.MembershipFee.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResApp.Application.ROA.MembershipFee.Queries
{

    public class ExportMembershipFeeReportQuery : IRequest<Result<MembershipFeePymentRes>>
    {
        public string? PaymentNo { get; set; }
    }

    public class ExportMembershipFeeReportQueryHandler : IRequestHandler<ExportMembershipFeeReportQuery, Result<MembershipFeePymentRes>>
    {
        private readonly IMemDbContext _context;
        public ExportMembershipFeeReportQueryHandler(IMemDbContext context)
        {
            _context = context;
        }

        public async Task<Result<MembershipFeePymentRes>> Handle(ExportMembershipFeeReportQuery request, CancellationToken cancellationToken)
        {

            try
            {
                var result = new Result<MembershipFeePymentRes>
                {
                    Data= new MembershipFeePymentRes()
                };

              
             

                    var data = await _context.ROAMembershipFeePayments.Include(x=>x.Member)               
                         .AsNoTracking()
                         .FirstOrDefaultAsync(q => q.PaymentNo == request.PaymentNo,cancellationToken);


                    if (data ==null)
                    {
                        result.HasError = true;
                        result.Messages.Add("Data Not Found");                      
                    }
                    else
                    {
                        result.HasError = false;
                       
                        result.Data =  new MembershipFeePymentRes
                        {
                            MemberId = data.MemberId,
                            MembershipNo=data.Member.MemberShipNo,
                            Amount = data.Amount,
                            Id = data.Id,
                            PaymentNo = data.PaymentNo,
                            MemberName = data.Member.Name,
                            PaymentDate = data.PaymentDate,                         

                        };

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
