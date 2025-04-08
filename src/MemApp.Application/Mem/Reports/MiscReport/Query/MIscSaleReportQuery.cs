using Dapper;
using MediatR;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Members.Models;
using MemApp.Application.Mem.Reports.MiscReport.Model;
using MemApp.Application.Services;
using System.Text;


namespace MemApp.Application.Mem.Reports.MiscReport.Query
{
    public class MiscSaleReportQuery: IRequest<List<MiscSaleReportVM>>
    {
        public MiscSaleReportCriteria Model { get; set; } = new MiscSaleReportCriteria();
    }

    public class MiscSaleReportQueryyHandler : IRequestHandler<MiscSaleReportQuery, List<MiscSaleReportVM>>
    {
        private readonly IDapperContext _context;
        private readonly IMemDbContext _memdbcontext;
        public MiscSaleReportQueryyHandler(IDapperContext context, IMemDbContext memdbcontext)
        {
            _context = context;
            _memdbcontext = memdbcontext;

        }
    

        public async Task<List<MiscSaleReportVM>> Handle(MiscSaleReportQuery request, CancellationToken cancellationToken)
        {
            var result = new List<ViewMemberDto>();

            using (var connection = _context.CreateConnection())
            {              
                try
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("select * from VW_MiscSaleRpt");                                    

                    var dataQuery = await connection
                        .QueryAsync<MiscSaleReportVM>(sb.ToString());


                    if (request.Model.FromDate != null && request.Model.ToDate!=null)
                    {
                        dataQuery = dataQuery.Where(c => c.InvoiceDate.Date >= request.Model.FromDate?.Date && c.InvoiceDate.Date <= request.Model.ToDate?.Date);
                    }
                   
                    if (request.Model.MembershipNo != null&& request.Model.MembershipNo!="null")
                    {

                        dataQuery = dataQuery.Where(c => c.MembershipNo == request.Model.MembershipNo);
                    }

                    var data = dataQuery.ToList();


                    return data;
                }
                catch (Exception ex)
                {
                    throw  new Exception(ex.Message);
                }

            }
        }

    }

}

