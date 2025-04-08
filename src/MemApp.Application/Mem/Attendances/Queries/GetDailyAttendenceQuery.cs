using Dapper;
using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Attendances.Model;
using MemApp.Application.Mem.Booking.Queries;
using MemApp.Application.Mem.Reports.VenueReport.Model;
using MemApp.Application.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MemApp.Application.Mem.Attendances.Queries
{
    public class GetDailyAttendenceQuery :IRequest<DailyAttendanceList>
    {
        public int PageNo { get; set; }
        public int PageSize { get; set; }
        
    }
    public class GetDailyAttendenceQueryHandler : IRequestHandler<GetDailyAttendenceQuery, DailyAttendanceList>
    {
        private readonly IMemDbContext _context;
        private readonly IDapperContext _dapperContext;
        public GetDailyAttendenceQueryHandler(IMemDbContext context, IDapperContext dapperContext)
        {
            _context = context;
            _dapperContext = dapperContext;
        }

        public async Task<DailyAttendanceList> Handle(GetDailyAttendenceQuery request,CancellationToken cancellationToken)
        {
            var result = new DailyAttendanceList();
            try
            {
                using (var connection = _dapperContext.CreateConnection())
                {

                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("select * from VW_DailyAttendance ");

                    var companies = await connection
                        .QueryAsync<DailyAttendance>(sb.ToString());

                    result.DataList = companies.OrderByDescending(c=>c.InTime).Skip((request.PageNo-1)* request.PageSize).Take(request.PageSize).ToList();
                    result.DataCount = Convert.ToInt32(companies.Count());
                }


               

            }
            catch (Exception ex)
            {
                
            }

            return result;
        }
    }

}
