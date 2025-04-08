using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Colleges.Models;
using System.Text;
using MemApp.Application.Services;
using Dapper;
using MemApp.Application.Mem.TopUps.Models;
using Microsoft.EntityFrameworkCore;
using MemApp.Application.Interfaces;
using MemApp.Application.Models;
using MemApp.Application.Mem.Reports.RefundReport.EventRefund.Model;
using System.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Configuration;
using MemApp.Application.Mem.Reports.AppDownloadReport.Model;

namespace MemApp.Application.Mem.TopUps.Queries
{
    public class ExportTopUpQuery : IRequest<List<TopUpExportReq>>
    {
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string? MemberShipNo { get; set;}
        public string? UserName { get; set; }
    }


    public class ExportTopUpQueryHandler : IRequestHandler<ExportTopUpQuery, List<TopUpExportReq>>
    {
        private readonly IDapperContext _context;

        public ExportTopUpQueryHandler(IDapperContext context)
        {
            _context = context;
        }

        public async Task<List<TopUpExportReq>> Handle(ExportTopUpQuery request, CancellationToken cancellationToken)
        {
            var result = new List<TopUpExportReq>();
            using (var connection = _context.CreateConnection())
            {

                try
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@FromDate", request.StartDate, DbType.DateTime);
                    parameters.Add("@ToDate", request.EndDate, DbType.DateTime);
                    parameters.Add("@MembershipNo", request.MemberShipNo, DbType.String);
                    parameters.Add("@UserName", request.UserName, DbType.String);


                    var dataList = await connection.QueryAsync<TopUpExportReq>(
                        "SP_TopUpDetailReport",
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
