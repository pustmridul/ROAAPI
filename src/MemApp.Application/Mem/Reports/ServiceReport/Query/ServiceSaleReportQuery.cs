using Dapper;
using MediatR;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Members.Models;
using MemApp.Application.Mem.Reports.ServiceReport.Model;
using MemApp.Application.Services;

using System.Text;


namespace MemApp.Application.Mem.Reports.ServiceReport.Query
{
    public class ServiceSaleReportQuery: IRequest<List<ServiceSaleReportVM>>
    {
        public ServiceSaleReportCriteria Model { get; set; } = new ServiceSaleReportCriteria();
    }

    public class ServiceSaleReportQueryyHandler : IRequestHandler<ServiceSaleReportQuery, List<ServiceSaleReportVM>>
    {
        private readonly IDapperContext _context;
        private readonly IMemDbContext _memdbcontext;
        public ServiceSaleReportQueryyHandler(IDapperContext context, IMemDbContext memdbcontext)
        {
            _context = context;
            _memdbcontext = memdbcontext;

        }
    

        public async Task<List<ServiceSaleReportVM>> Handle(ServiceSaleReportQuery request, CancellationToken cancellationToken)
        {
            var result = new List<ViewMemberDto>();

            using (var connection = _context.CreateConnection())
            {
               

                try
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("select * from VW_SaleService");                                    

                    var dataQuery = await connection
                        .QueryAsync<ServiceSaleReportVM>(sb.ToString());


                    if (request.Model.ServiceId!=null)
                    {
                        
                        dataQuery = dataQuery.Where(c => c.ServiceTicketId== request.Model.ServiceId);
                    }

                    if (request.Model.FromDate != null)
                    {
                        dataQuery = dataQuery.Where(c => c.InvoiceDate.Date >= request.Model.FromDate?.Date);
                    }
                    if (request.Model.ToDate != null)
                    {
                        dataQuery = dataQuery.Where(c => c.InvoiceDate.Date <= request.Model.ToDate?.Date);
                    }
                    if (request.Model.MembershipNo != null && request.Model.MembershipNo!="null")
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

