using MediatR;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Subscription.Model;
using MemApp.Application.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using MemApp.Application.Mem.Reports.RefundReport.ServiceRefund.Model;
using MemApp.Application.Mem.Reports.MemberReport.Model;

namespace MemApp.Application.Mem.Subscription.Queries
{
    public class SubscriptionDueDetailReportQuery : IRequest<List<ExportSubscriptionDue>>
    {
        public SubscriptionPaymentReportReq Model { get; set; } = new SubscriptionPaymentReportReq();
    }

    public class SubscriptionDueDetailReportQueryHandler : IRequestHandler<SubscriptionDueDetailReportQuery, List<ExportSubscriptionDue>>
    {
        private readonly IDapperContext _context;
        public SubscriptionDueDetailReportQueryHandler(IDapperContext context)
        {
            _context = context;
        }

        public async Task<List<ExportSubscriptionDue>> Handle(SubscriptionDueDetailReportQuery request, CancellationToken cancellationToken)
        {
            using (var connection = _context.CreateConnection())
            {
                try
                {
                  

                    if (request.Model.MembershipNo == null)
                    {
                        request.Model.MembershipNo = "null";
                    }
                    var parameters = new DynamicParameters();
                    parameters.Add("@MembershipNo", request.Model.MembershipNo == "null" ? null : request.Model.MembershipNo, DbType.String, size: 10);
                    parameters.Add("@Year", request.Model.Year == "null" ? null : request.Model.Year);
                    parameters.Add("@Quarter", request.Model.Quarter == "null" ? null : request.Model.Quarter);

                    var dataList = await connection.QueryAsync<ExportSubscriptionDue>(
                        "SP_SubscriptionDueDetailReport",
                       parameters,
                            commandType: CommandType.StoredProcedure);
                    if (dataList.Any())
                    {
                        dataList = dataList.Where(c => c.IsDue == true)
                        .ToList();
                    }
                    return dataList.ToList();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }         
        }
    }
}
