using Dapper;
using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Attendances.Model;
using MemApp.Application.Mem.Dashboards.Models;
using MemApp.Application.Mem.Reports.CommonReport.Model;
using MemApp.Application.Mem.Reports.RefundReport.EventRefund.Model;
using MemApp.Application.Mem.Transactions.Queries;
using MemApp.Application.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Application.Mem.Reports.CommonReport.Query
{
    public class GetAllSalesQuery : IRequest<List<AllSalesReportVm>>
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string MembershipNo { get; set; }
    }

    public class GetAllSalesQueryHandler : IRequestHandler<GetAllSalesQuery, List<AllSalesReportVm>>
    {
        private readonly IMemDbContext _context;
        private readonly IDapperContext _dapperContext;
        public GetAllSalesQueryHandler(IMemDbContext context, IDapperContext dapperContext)
        {
            _context = context;
            _dapperContext = dapperContext;
        }

        public async Task<List<AllSalesReportVm>> Handle(GetAllSalesQuery request, CancellationToken cancellationToken)
        {
            using (var connection = _dapperContext.CreateConnection())
            {
                var result = new List<AllSalesReportVm>();
                try
                {

                    var parameters = new DynamicParameters();

                    parameters.Add("@StartDate", request.StartDate.Date, DbType.Date);
                    parameters.Add("@EndDate", request.EndDate.Date, DbType.Date);
                    parameters.Add("@MembershipNo", request.MembershipNo );



                    var dataList = await connection.QueryAsync<AllSalesReportVm>(
                        "SP_GetFilteredSales",
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
