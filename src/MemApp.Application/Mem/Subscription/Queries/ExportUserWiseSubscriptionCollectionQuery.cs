using MediatR;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Subscription.Model;
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
using MemApp.Application.Mem.Reports.RefundReport.ServiceRefund.Model;

namespace MemApp.Application.Mem.Subscription.Queries
{
    public class ExportUserWiseSubscriptionCollectionQuery : IRequest<List<ExportUserWiseSubscriptionCollectionModel>>
    {
        public SubscriptionPaymentReportReq Model { get; set; } = new SubscriptionPaymentReportReq();
    }

    public class ExportUserWiseSubscriptionCollectionQueryHandler : IRequestHandler<ExportUserWiseSubscriptionCollectionQuery, List<ExportUserWiseSubscriptionCollectionModel>>
    {
        private readonly IDapperContext _context;
        public ExportUserWiseSubscriptionCollectionQueryHandler(IDapperContext context)
        {
            _context = context;

        }

        public async Task<List<ExportUserWiseSubscriptionCollectionModel>> Handle(ExportUserWiseSubscriptionCollectionQuery request, CancellationToken cancellationToken)
        {
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@FromDate", request.Model.FromDate?.Date, DbType.Date);
                    parameters.Add("@ToDate", request.Model.ToDate?.Date, DbType.Date);
                    var dataList = await connection.QueryAsync<ExportUserWiseSubscriptionCollectionModel>(
                        "SP_UserWiseSubscriptionCollection",
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
