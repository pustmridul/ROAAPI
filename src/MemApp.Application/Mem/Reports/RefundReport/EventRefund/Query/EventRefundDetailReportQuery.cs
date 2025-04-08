using MediatR;
using MemApp.Application.Mem.Reports.Common;
using MemApp.Application.Mem.Reports.RefundReport.EventRefund.Model;
using MemApp.Application.Services;

using System.Data;


using MemApp.Application.Mem.Reports.MemberReport.Model;
using Dapper;

namespace MemApp.Application.Mem.Reports.RefundReport.EventRefund.Query
{
    public class EventRefundDetailReportQuery: IRequest<List<EventRefundDetailReportVM>>
    {
        public CommonCriteria Model { get; set; } = new CommonCriteria();
    }

    public class EventRefundDetailReportQueryHandler : IRequestHandler<EventRefundDetailReportQuery, List<EventRefundDetailReportVM>>
    {
        private readonly IDapperContext _context;
        public EventRefundDetailReportQueryHandler(IDapperContext context)
        {
            _context = context;
        }


        public async Task<List<EventRefundDetailReportVM>> Handle(EventRefundDetailReportQuery request, CancellationToken cancellationToken)
        {
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    var parameters = new DynamicParameters();
                   
                    parameters.Add("@FromDate", request.Model.FromDate?.Date, DbType.Date);
                    parameters.Add("@ToDate", request.Model.ToDate?.Date, DbType.Date);
                    parameters.Add("@MembershipNo", request.Model.MembershipNo == "null" ? null : request.Model.MembershipNo, DbType.String, size: 10);
                    parameters.Add("@EventId", request.Model.EventId, DbType.Int32);


                    var dataList = await connection.QueryAsync<EventRefundDetailReportVM>(
                        "SP_EventRefundDetailReport",
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
