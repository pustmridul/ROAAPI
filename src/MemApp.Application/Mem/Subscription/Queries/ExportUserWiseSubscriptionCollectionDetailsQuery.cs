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
    public class ExportUserWiseSubscriptionCollectionDetailsQuery: IRequest<List<ExportUserWiseSubscriptionCollectionModel>>
    {
        public SubscriptionPaymentReportReq Model { get; set; } = new SubscriptionPaymentReportReq();
    }
    public class ExportUserWiseSubscriptionCollectionDetailsQueryHandler : IRequestHandler<ExportUserWiseSubscriptionCollectionDetailsQuery, List<ExportUserWiseSubscriptionCollectionModel>>
    {
        private readonly IDapperContext _context;
        public ExportUserWiseSubscriptionCollectionDetailsQueryHandler(IDapperContext context)
        {
            _context = context;
        }

        public async Task<List<ExportUserWiseSubscriptionCollectionModel>> Handle(ExportUserWiseSubscriptionCollectionDetailsQuery request, CancellationToken cancellationToken)
        {
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    var parameters = new
                    {
                        FromDate = request.Model.FromDate?.Date, // Use parameter names without @
                        ToDate = request.Model.ToDate?.Date,
                    };

                    var dataList = await connection.QueryAsync<ExportUserWiseSubscriptionCollectionModel>(
                        "SP_UserWiseSubscriptionCollectionDetails",
                        parameters,
                        commandType: CommandType.StoredProcedure // Specify that it's a stored procedure
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
