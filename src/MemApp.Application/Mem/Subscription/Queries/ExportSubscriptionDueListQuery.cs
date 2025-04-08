using Dapper;
using MediatR;
using MemApp.Application.App.Models;
using MemApp.Application.Mem.Subscription.Model;
using MemApp.Application.Services;
using MemApp.Domain.Entities.Payment;
using System.Text;

namespace MemApp.Application.Mem.Subscription.Queries
{

    public class ExportSubscriptionDueListQuery : IRequest<List<ExportSubscriptionDue>>
    {
        public string reportType { get; set; }
        public string reportName { get; set; }
    }

    public class ExportSubscriptionDueListQueryHandler : IRequestHandler<ExportSubscriptionDueListQuery, List<ExportSubscriptionDue>>
    {
        private readonly IDapperContext _context;
        public ExportSubscriptionDueListQueryHandler(IDapperContext context)
        {
            _context = context;
        }

        public async Task<List<ExportSubscriptionDue>> Handle(ExportSubscriptionDueListQuery request, CancellationToken cancellationToken)
        {
            var result = new List<ExportSubscriptionDue>();

            using (var connection = _context.CreateConnection())
            {
                StringBuilder sb = new StringBuilder();
                //sb.AppendLine(" With SubscriptionPayment as ( SELECT MemberShipNo,SUM(PaymentAmount + LateAmount) as DueAmount,STRING_AGG(SubscriptionName, ', ') AS Quaters,");
                //sb.AppendLine(" Concat(SubscriptionYear, '(', STRING_AGG(SubscriptionName, ', '), ')') SubscriptionYearQuarter ");
                //sb.AppendLine(" FROM [dbo].[mem_SubscriptionPaymentTemp]");
                //sb.AppendLine("where SubsStatus = 'Due'");
                //sb.AppendLine("group by MemberShipNo, SubscriptionYear )");
                //sb.AppendLine("select MemberShipNo AS MemberShipNo,");
                //sb.AppendLine("Sum(DueAmount) DueAmount,STRING_AGG(SubscriptionYearQuarter, ', ') SubscriptionYearQuarter from SubscriptionPayment group by MemberShipNo");

                sb.AppendLine("select c.Id AS MemberId,c.MembershipNo AS MemberShipNo,c.FullName AS MemberName,(select SUM(sf.SubscriptionFee)from mem_SubscriptionFees sf where  sf.StartDate< GETDATE()" +
                    ") -ISNULL((select SUM(PaymentAmount) from mem_SubscriptionPaymentTemp WHERE RegisterMemberId=c.Id group by RegisterMemberId),0)" +
                    "AS DueAmount from Customer c JOIN mem_MemberType mt on mt.Id=c.MemberTypeId where mt.IsSubscribed=1 AND c.IsActive=1  AND LEFT(c.MembershipNo, 1)<>'T'" +
                    " AND c.IsMasterMember=1 ORDER bY c.MembershipNo");

                var data = await connection.QueryAsync<ExportSubscriptionDue>(sb.ToString());

                result= data.ToList();

            }

            return result;
        }

    }
}
