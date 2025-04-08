using Dapper;
using MediatR;
using MemApp.Application.Mem.Members.Models;
using MemApp.Application.Mem.Reports.TicketReport.Model;
using MemApp.Application.Services;
using System.Text;

namespace MemApp.Application.Mem.Reports.TicketReport.Queries
{
    public class EventTicketReportQuery : IRequest<List<EventTicketReportVM>>
    {
        public EventTicketReportCriteria Model { get; set; } = new EventTicketReportCriteria();
    }

    public class EventTicketReportQueryHandler : IRequestHandler<EventTicketReportQuery,List<EventTicketReportVM>>
    {
        private readonly IDapperContext _context;

        public EventTicketReportQueryHandler(IDapperContext context)
        {
            _context = context;
  
        }

        public async Task<List<EventTicketReportVM>> Handle(EventTicketReportQuery request, CancellationToken cancellationToken)
        {
            var result = new List<ViewMemberDto>();

            using (var connection = _context.CreateConnection())
            {


                try
                {
                     StringBuilder sb = new StringBuilder();
                    sb.AppendLine("select Count(SaleEventId) TicketCount, setd.EventId,setd.EventTitle, st.EventDate,setd.TicketCriteria,CAST(se.InvoiceDate AS DATE) InvoiceDate," +
                        "MIN(setd.TicketPrice) TicketPrice, (MIN(setd.TicketPrice) * (Count(SaleEventId))) TotalSaleAmount");
                    sb.AppendLine("from mem_SaleEventTicketDetail setd");
                    sb.AppendLine("join mem_SaleEventTicket se on se.Id = setd.SaleEventId");
                    sb.AppendLine("join mem_ServiceTicket st on setd.EventId = st.Id");
                    sb.AppendLine("where setd.IsActive = 1 and (setd.SaleStatus <> 'Cancel' OR setd.SaleStatus is null)");
                    sb.AppendLine("group by setd.EventId, setd.EventTitle, st.EventDate, setd.TicketCriteria, CAST(se.InvoiceDate AS DATE) ");
                    sb.AppendLine("order by CAST(se.InvoiceDate AS DATE)");
                    

                    var dataQuery = await connection
                        .QueryAsync<EventTicketReportVM>(sb.ToString());

                    if (!string.IsNullOrEmpty(request.Model.EventTicketIds))
                    {
                        int[] EventTicketIds = request.Model.EventTicketIds.Split(',')
                        .Select(id => int.TryParse(id, out int eventId) ? eventId : -1) // Handle parsing failures with -1 (or other default value)
                        .Where(id => id != -1)
                        .ToArray();
                        dataQuery = dataQuery.Where(c => EventTicketIds.Contains(c.EventId));
                    }
                    if (request.Model.FromDate != null)
                    {
                        dataQuery = dataQuery.Where(c => c.InvoiceDate.Date >= request.Model.FromDate?.Date);
                    }
                    if (request.Model.ToDate != null)
                    {
                        dataQuery = dataQuery.Where(c => c.InvoiceDate.Date <= request.Model.ToDate?.Date);
                    }

                    var data = dataQuery.ToList();


                    return data;
                }
                catch(Exception ex){
                    throw new Exception(ex.Message);
                }

            }
        }

    }
}
