using Dapper;
using MediatR;
using MemApp.Application.Mem.Members.Models;
using MemApp.Application.Mem.Reports.MemberReport.Model;
using MemApp.Application.Mem.Reports.TicketReport.Model;
using MemApp.Application.Services;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MemApp.Application.Mem.Reports.RefundReport.ServiceRefund.Model;

namespace MemApp.Application.Mem.Reports.TicketReport.Queries
{
    public class EventTicketSaleReportQuery: IRequest<List<EventTicketSaleReportVM>>
    {
        public int? EventId { get; set; }
        public string? MembershipNo { get; set; }
    }

    public class EventTicketSaleReportQueryHandler : IRequestHandler<EventTicketSaleReportQuery, List<EventTicketSaleReportVM>>
    {
        private readonly IDapperContext _context;

        public EventTicketSaleReportQueryHandler(IDapperContext context)
        {
            _context = context;
        }

        public async Task<List<EventTicketSaleReportVM>> Handle(EventTicketSaleReportQuery request, CancellationToken cancellationToken)
        {

            using (var connection = _context.CreateConnection())
            {
                try
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@MembershipNo", request.MembershipNo == "null" ? null : request.MembershipNo, DbType.String, size: 10);
                    parameters.Add("@EventId", request.EventId, DbType.Int32);
                   
                    var dataList = await connection.QueryAsync<EventTicketSaleReportVM>(
                        "SP_EventDetailReport",
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
