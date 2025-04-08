using MediatR;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Reports.Common;
using MemApp.Application.Mem.Reports.RefundReport.EventRefund.Model;
using MemApp.Application.Mem.Reports.RefundReport.EventRefund.Query;
using MemApp.Application.Mem.Reports.RefundReport.ServiceRefund.Model;
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
using MemApp.Application.Mem.Reports.RefundReport.VenueBookingRefund.Model;
using MemApp.Application.Mem.Reports.MemberReport.Model;

namespace MemApp.Application.Mem.Reports.RefundReport.ServiceRefund.Query
{
    public class ServiceRefundSummaryReportQuery: IRequest<List<ServiceRefundSummaryReportVM>>
    {
        public CommonCriteria Model { get; set; } = new CommonCriteria();
    }

    public class ServiceRefundSummaryReportQueryHandler : IRequestHandler<ServiceRefundSummaryReportQuery, List<ServiceRefundSummaryReportVM>>
    {
        private readonly IDapperContext _context;
        public ServiceRefundSummaryReportQueryHandler(IDapperContext context)
        {
            _context = context;          
        }


        public async Task<List<ServiceRefundSummaryReportVM>> Handle(ServiceRefundSummaryReportQuery request, CancellationToken cancellationToken)
        {

            using (var connection = _context.CreateConnection())
            {
                try
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@MembershipNo", request.Model.MembershipNo == "null" ? null : request.Model.MembershipNo, DbType.String, size: 10);
                    parameters.Add("@FromDate", request.Model.FromDate?.Date, DbType.Date);
                    parameters.Add("@ToDate", request.Model.ToDate?.Date, DbType.Date);

                    var dataList = await connection.QueryAsync<ServiceRefundSummaryReportVM>(
                        "SP_ServiceRefundSummaryReport",
                        parameters,
                        commandType: CommandType.StoredProcedure
                    );
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
