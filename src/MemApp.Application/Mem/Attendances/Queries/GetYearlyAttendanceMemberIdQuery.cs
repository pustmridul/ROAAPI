using MediatR;
using MemApp.Application.Interfaces.Contexts;
using System.Text;
using MemApp.Application.Services;
using Dapper;
using MemApp.Application.Mem.Attendances.Model;

namespace MemApp.Application.Mem.Attendances.Queries
{
    public class GetYearlyAttendanceMemberIdQuery : IRequest<YearlyAttendanceList>
    {
        public string MemberShipNo { get; set; } = string.Empty;
    }


    public class GetYearlyAttendanceMemberIdQueryHandler : IRequestHandler<GetYearlyAttendanceMemberIdQuery, YearlyAttendanceList>
    {
        private readonly IMemDbContext _context;
        private readonly IDapperContext _dapperContext;
        public GetYearlyAttendanceMemberIdQueryHandler(IMemDbContext context, IDapperContext dapperContext)
        {
            _context = context;
            _dapperContext = dapperContext;
        }

        public async Task<YearlyAttendanceList> Handle(GetYearlyAttendanceMemberIdQuery request, CancellationToken cancellationToken)
        {
            var result = new YearlyAttendanceList();
            try
            {           
                using (var connection = _dapperContext.CreateConnection())
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("SELECT month(ab.PTime) Month,Count(distinct(day(ab.PTime))) Attendance");
                    sb.AppendLine("FROM Atten_BarcodeTable ab LEFT JOIN Customer c ON ab.Barcode = c.MembershipNo");
                    sb.AppendLine($"WHERE YEAR(ab.PDate) = YEAR(GETDATE()) AND c.IsActive = 1 and c.MembershipNo='{request.MemberShipNo}' group by Month(ab.PTime)");


                    var data = await connection.QueryAsync<MonthlyAttendance>(sb.ToString());
                    int i = 1;
                    for (i = 1; i <= 12; i++)
                    {
                        
                        if(data.ToList().Select(c=>c.Month == i).Count() > 0)
                        {
                            var entry = data.Where(c => c.Month == i).FirstOrDefault();
                            if (entry != null)
                            {
                                result.YearlyAttendances.Add(entry.Attendance);
                            }
                            else
                            {
                                result.YearlyAttendances.Add(0);
                            }
                        }
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
