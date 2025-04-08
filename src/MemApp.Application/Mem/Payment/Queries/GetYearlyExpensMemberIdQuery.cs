using MediatR;
using MemApp.Application.Interfaces.Contexts;
using System.Text;
using MemApp.Application.Services;
using Dapper;
using MemApp.Application.Mem.Payment.Model;

namespace MemApp.Application.Mem.Payment.Queries
{
    public class GetYearlyExpenseMemberIdQuery : IRequest<YearlyExpenseList>
    {
        public string MemberShipNo { get; set; }=string.Empty;
    }


    public class GetYearlyExpenseMemberIdQueryHandler : IRequestHandler<GetYearlyExpenseMemberIdQuery, YearlyExpenseList>
    {
        private readonly IMemDbContext _context;
        private readonly IDapperContext _dapperContext;
        public GetYearlyExpenseMemberIdQueryHandler(IMemDbContext context, IDapperContext dapperContext)
        {
            _context = context;
            _dapperContext = dapperContext;
        }

        public async Task<YearlyExpenseList> Handle(GetYearlyExpenseMemberIdQuery request, CancellationToken cancellationToken)
        {
            var result = new YearlyExpenseList();
            try
            {
                using (var connection = _dapperContext.CreateConnection())
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine($"select MONTH(Dates) MonthNo,SUM(Amount) Amount from CustomerLedger cl  JOIN Customer c on c.PrvCusID = cl.PrvCusID where cl.PrvCusID='{request.MemberShipNo}' AND Amount<0 and YEAR(Dates)=YEAR(GETDATE()) group by YEAR(Dates), MONTH(Dates)\r\n");
                   
                    var companies = await connection.QueryAsync<dynamic>(sb.ToString());

                    for (int i = 0; i < 12; i++)
                    {
                        decimal a = companies.Where(q => q.MonthNo == i + 1).Sum(s => (decimal)s.Amount);
                        result.YearlyExpenses.Add(-1*a);
                    }
                }
            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.Messages?.Add(ex.ToString());
            }
            
            return result;
        }
    }
}
