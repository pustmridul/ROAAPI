using Dapper;
using MediatR;
using MemApp.Application.Mem.Reports.Common;
using MemApp.Application.Mem.Reports.MemberReport.Model;
using MemApp.Application.Mem.Reports.RefundReport.EventRefund.Model;
using MemApp.Application.Services;
using System.Data;


namespace MemApp.Application.Mem.Reports.RefundReport.EventRefund.Query
{
    public class EventRefundSummaryReportQuery: IRequest<List<EventRefundSummaryReportVM>>
    {
        public CommonCriteria Model { get; set; } = new CommonCriteria();
    }

    public class EventRefundSummaryReportQueryHandler : IRequestHandler<EventRefundSummaryReportQuery, List<EventRefundSummaryReportVM>>
    {
        private readonly IDapperContext _context;
        public EventRefundSummaryReportQueryHandler(IDapperContext context)
        {
            _context = context;
        }


        public async Task<List<EventRefundSummaryReportVM>> Handle(EventRefundSummaryReportQuery request, CancellationToken cancellationToken)
        {
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@MembershipNo", request.Model.MembershipNo == "null" ? null : request.Model.MembershipNo, DbType.String, size: 10);
                    parameters.Add("@FromDate", request.Model.FromDate?.Date, DbType.Date);
                    parameters.Add("@ToDate", request.Model.ToDate?.Date, DbType.Date);

                    var dataList = await connection.QueryAsync<EventRefundSummaryReportVM>(
                        "SP_EventRefundSummaryReport",
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
