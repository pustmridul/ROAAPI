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
using MemApp.Application.Mem.Reports.MemberReport.Model;

namespace MemApp.Application.Mem.Reports.RefundReport.ServiceRefund.Query
{
    public class ServiceRefundDetailReportQuery: IRequest<List<ServiceRefundDetailReportVM>>
    {
        public CommonCriteria Model { get; set; } = new CommonCriteria();
    }

    public class ServiceRefundDetailReportQueryHandler : IRequestHandler<ServiceRefundDetailReportQuery, List<ServiceRefundDetailReportVM>>
    {
        private readonly IDapperContext _context;
        public ServiceRefundDetailReportQueryHandler(IDapperContext context)
        {
            _context = context;
        }


        public async Task<List<ServiceRefundDetailReportVM>> Handle(ServiceRefundDetailReportQuery request, CancellationToken cancellationToken)
        {
            using (var connection = _context.CreateConnection())
            {

                try
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@MembershipNo", request.Model.MembershipNo == "null" ? null : request.Model.MembershipNo, DbType.String, size: 10);
                    parameters.Add("@FromDate", request.Model.FromDate?.Date, DbType.Date);
                    parameters.Add("@ToDate", request.Model.ToDate?.Date, DbType.Date);
                    parameters.Add("@ServiceId", request.Model.ServiceId, DbType.Int32);


                    var dataList = await connection.QueryAsync<ServiceRefundDetailReportVM>(
                        "SP_ServiceRefundDetailReport",
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
