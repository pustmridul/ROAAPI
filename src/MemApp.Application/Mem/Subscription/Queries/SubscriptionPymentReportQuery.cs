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
    public class SubscriptionPymentReportQuery: IRequest<List<ExportSubscriptionPayment>>
    {
        public SubscriptionPaymentReportReq Model { get; set; } = new SubscriptionPaymentReportReq();
    }

    public class SubscriptionPymentReportQueryHandler : IRequestHandler<SubscriptionPymentReportQuery, List<ExportSubscriptionPayment>>
    {
        private readonly IDapperContext _context;
        public SubscriptionPymentReportQueryHandler(IDapperContext context)
        {
            _context = context;
        }


        public async Task<List<ExportSubscriptionPayment>> Handle(SubscriptionPymentReportQuery request, CancellationToken cancellationToken)
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
                    parameters.Add("@FromDate", request.Model.FromDate?.Date, DbType.Date);
                    parameters.Add("@ToDate", request.Model.ToDate?.Date, DbType.Date);
                    parameters.Add("@Year", request.Model.Year == "null" ? null : request.Model.Year);


                    var dataList = await connection.QueryAsync<ExportSubscriptionPayment>(
                        "SP_SubscriptionPaymentSummaryReport",
                        parameters,
                        commandType: CommandType.StoredProcedure);
                    var data = dataList.ToList();
                    return data;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }         
        }

    }
}
