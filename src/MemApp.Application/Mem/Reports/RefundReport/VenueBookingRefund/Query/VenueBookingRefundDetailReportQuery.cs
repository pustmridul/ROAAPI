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
using MemApp.Application.Mem.Reports.MemberReport.Model;

namespace MemApp.Application.Mem.Reports.RefundReport.VenueBookingRefund.Query
{
    public class VenueBookingRefundDetailReportQuery: IRequest<List<VenueBookingRefundDetailReportVM>>  
    {
        public CommonCriteria Model { get; set; } = new CommonCriteria();
    }

    public class VenueBookingRefundDetailReportQueryHandler : IRequestHandler<VenueBookingRefundDetailReportQuery, List<VenueBookingRefundDetailReportVM>>
    {
        private readonly IDapperContext _context;
        public VenueBookingRefundDetailReportQueryHandler(IDapperContext context)
        {
            _context = context;
        }


        public async Task<List<VenueBookingRefundDetailReportVM>> Handle(VenueBookingRefundDetailReportQuery request, CancellationToken cancellationToken)
        {

            using (var connection = _context.CreateConnection())
            {
                try
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@MembershipNo", request.Model.MembershipNo == "null" ? null : request.Model.MembershipNo, DbType.String, size: 10);
                    parameters.Add("@FromDate", request.Model.FromDate?.Date, DbType.Date);
                    parameters.Add("@ToDate", request.Model.FromDate?.Date, DbType.Date);
                    parameters.Add("@VenueId", request.Model.VenueId, DbType.Int32);

                    var dataList = await connection.QueryAsync<VenueBookingRefundDetailReportVM>(
                        "SP_VenueRefundDetailReport",
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
