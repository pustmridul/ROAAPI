using Dapper;
using MediatR;
using MemApp.Application.Mem.Reports.AppDownloadReport.Model;
using MemApp.Application.Mem.Reports.Common;

using MemApp.Application.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Application.Mem.Reports.AppDownloadReport.Query
{
    public class AppDownloadReportQuery: IRequest<List<AppDownloadReportVM>>
    {
        public bool? IsActive { get; set; } 
    }

    public class AppDownloadReportQueryHandler : IRequestHandler<AppDownloadReportQuery, List<AppDownloadReportVM>>
    {
        private readonly IDapperContext _context;

        public AppDownloadReportQueryHandler(IDapperContext context)
        {
            _context = context;


        }


        public async Task<List<AppDownloadReportVM>> Handle(AppDownloadReportQuery request, CancellationToken cancellationToken)
        {


            using (var connection = _context.CreateConnection())
            {

                try
                {
                    StringBuilder sb = new StringBuilder();                
                    
                    sb.AppendLine("select c.IsMasterMember, u.MemberId, c.MembershipNo, c.FullName MembershipName, c.Phone, c.Email,case when uc.AppId='MOBILEAPP' then 1 else 0 end IsActiveAppUser\r\n");
                    sb.AppendLine("from mem_User u left join Customer c on u.MemberId=c.Id left join mem_UserConference uc on u.Id=uc.UserId");
                    sb.AppendLine("where c.IsActive=1 and u.IsActive=1 and c.MembershipNo NOT LIKE 'T%'");

                    var dataQuery = await connection
                        .QueryAsync<AppDownloadReportVM>(sb.ToString());

                    if (request.IsActive!=null)
                    {
                        dataQuery = dataQuery.Where(x => x.IsActiveAppUser == request.IsActive);
                    }

                    var data = dataQuery.ToList();


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
