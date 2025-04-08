using MediatR;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Reports.Common;
using MemApp.Application.Mem.Reports.RefundReport.ServiceRefund.Model;
using MemApp.Application.Mem.Reports.RefundReport.ServiceRefund.Query;
using MemApp.Application.Mem.Reports.RefundReport.VenueBookingRefund.Model;
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
using MemApp.Application.Mem.Reports.RefundReport.EventRefund.Model;

namespace MemApp.Application.Mem.Reports.RefundReport.VenueBookingRefund.Query
{
    public class VenueBookingRefundSummaryReportQuery: IRequest<List<VenueBookingRefundSummaryReportVM>>
    {
        public CommonCriteria Model { get; set; } = new CommonCriteria();
    }
    public class VenueBookingRefundSummaryReportQueryHandler : IRequestHandler<VenueBookingRefundSummaryReportQuery, List<VenueBookingRefundSummaryReportVM>>
    {
        private readonly IDapperContext _context;
        public VenueBookingRefundSummaryReportQueryHandler(IDapperContext context)
        {
            _context = context;
        }

        public async Task<List<VenueBookingRefundSummaryReportVM>> Handle(VenueBookingRefundSummaryReportQuery request, CancellationToken cancellationToken)
        {
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@MembershipNo", request.Model.MembershipNo == "null" ? null : request.Model.MembershipNo, DbType.String, size: 10);
                    parameters.Add("@FromDate", request.Model.FromDate?.Date, DbType.Date);
                    parameters.Add("@ToDate", request.Model.FromDate?.Date, DbType.Date);
                  

                    var dataList = await connection.QueryAsync<VenueBookingRefundSummaryReportVM>(
                        "SP_VenueRefundSummaryReport",
                        parameters,
                        commandType: CommandType.StoredProcedure
                    );
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
