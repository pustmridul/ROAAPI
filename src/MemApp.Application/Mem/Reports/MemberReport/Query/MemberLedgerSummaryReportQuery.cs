using MediatR;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Reports.Common;
using MemApp.Application.Mem.Reports.MemberReport.Model;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using MemApp.Application.Services;

namespace MemApp.Application.Mem.Reports.MemberReport.Query
{

    public class MemberLedgerSummaryReportQuery : IRequest<List<MemberLedgerSummaryReportVM>>
    {
        public CommonCriteria Model { get; set; } = new CommonCriteria();
    }

    public class MemberLedgerSummaryReportQueryHandler : IRequestHandler<MemberLedgerSummaryReportQuery, List<MemberLedgerSummaryReportVM>>
    {
        private readonly IDapperContext _context;

        public MemberLedgerSummaryReportQueryHandler(IDapperContext context)
        {
            _context = context;
        }
        public async Task<List<MemberLedgerSummaryReportVM>> Handle(MemberLedgerSummaryReportQuery request, CancellationToken cancellationToken)
        {
            using (var connection = _context.CreateConnection())
            {

                try
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@MembershipNo", request.Model.MembershipNo == "null" ? null : request.Model.MembershipNo, DbType.String, size: 10);
                    parameters.Add("@FromDate", request.Model.FromDate?.Date, DbType.Date);
                    parameters.Add("@ToDate", request.Model.ToDate?.Date, DbType.Date);

                    var dataList = await connection.QueryAsync<MemberLedgerSummaryReportVM>(
                        "SP_MemberLedgerSummary",
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
