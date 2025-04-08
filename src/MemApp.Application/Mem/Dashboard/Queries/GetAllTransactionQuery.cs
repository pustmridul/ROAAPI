using Dapper;
using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Dashboards.Models;
using MemApp.Application.Services;
using System.Text;

namespace MemApp.Application.Mem.Transactions.Queries
{
    public class GetAllTransactionQuery : IRequest<ListResult<TransactionTypeVm>>
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }


    public class GetAllTransactionQueryHandler : IRequestHandler<GetAllTransactionQuery, ListResult<TransactionTypeVm>>
    {
        private readonly IMemDbContext _context;
        private readonly IDapperContext _dapperContext;
        public GetAllTransactionQueryHandler(IMemDbContext context, IDapperContext dapperContext)
        {
            _context = context;
            _dapperContext = dapperContext;
        }

        public async Task<ListResult<TransactionTypeVm>> Handle(GetAllTransactionQuery request, CancellationToken cancellationToken)
        {
            var result = new ListResult<TransactionTypeVm>();
            try
            {
                using (var connection = _dapperContext.CreateConnection())
                {
                    StringBuilder sb = new StringBuilder();
                  
                    sb.AppendLine(" SELECT CASE WHEN sum(Amount)>0 THEN SUM(Amount) ELSE -1*SUM(Amount) END Amount,TransactionType  FROM [dbo].[CustomerLedger]");
                    sb.AppendLine("where  Dates between '"+request.StartDate+"' AND '"+request.EndDate+"'");
                    sb.AppendLine("group by TransactionType");


                    var data = await connection.QueryAsync<TransactionTypeVm>(sb.ToString());
                    result.Data = data.ToList();
                    result.HasError = false;
                    return result;
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
    }
}
