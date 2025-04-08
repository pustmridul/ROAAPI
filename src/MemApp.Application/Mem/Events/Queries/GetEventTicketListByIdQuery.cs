using MediatR;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Events.Models;
using Microsoft.EntityFrameworkCore;


namespace MemApp.Application.Mem.Booking.Queries
{
    public class GetEventTicketListByIdQuery : IRequest<EventTicketListVm>
    {
        public int Id { get; set; }
    }


    public class GetEventTicketListByIdQueryHandler : IRequestHandler<GetEventTicketListByIdQuery, EventTicketListVm>
    {
        private readonly IMemDbContext _context;
        public GetEventTicketListByIdQueryHandler(IMemDbContext context)
        {
            _context = context;
        }

        public async Task<EventTicketListVm> Handle(GetEventTicketListByIdQuery request, CancellationToken cancellationToken)
        {
            var result = new EventTicketListVm();
            var data = await _context.SaleEventTickets               
                .Include(i=>i.SaleEventTicketDetails.Where(q=>q.IsActive))
                .SingleOrDefaultAsync(q=>q.Id==request.Id && q.IsActive,cancellationToken);
             
            var serviceTickets= await _context.ServiceTickets.ToListAsync(cancellationToken);
          
            if (data==null)
            {
                result.HasError = true;
                result.Messages.Add("Data Not Found");
            }
            else
            {
                var memberObj = await _context.RegisterMembers.Select(s => new { s.Id, s.MembershipNo, s.CardNo, s.FullName })
                      .FirstOrDefaultAsync(q => q.Id == data.MemberId, cancellationToken);
                result.HasError = false;
               
                foreach (var item in data.SaleEventTicketDetails)
                {
                    var sTicket = serviceTickets.FirstOrDefault(s => s.Id == item.EventId);

                    var evt = new EventTictetReq()
                    {          
                        Location= sTicket!= null ? sTicket.Location : "",
                        EventTitle = item.EventTitle ?? "",
                        MemberShipNo = data.MemberShipNo ?? "",
                        MemberName= memberObj?.FullName?? "",
                        EventDate = sTicket!= null ? sTicket.EventDate.Value.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") : "",
                        InvoiceDate= data.InvoiceDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                        InvoiceNo= data.InvoiceNo,
                        EventTokens = string.IsNullOrEmpty(item.EventTokens) ? new List<string>() : item.EventTokens.Split(',').ToList(),
                        TicketCodeNo =item.TicketCodeNo ?? "",
                        TicketPrice= item.TicketPrice?? 0,
                        TicketCriteria= item.TicketCriteria,
                        TableTitle= item.TableTitle ?? "",
                        TicketText= item.TicketText ??"",
                        AreaLayoutTitle= item.AreaLayoutTitle ?? "",
                        ChairNo= item.NoofChair.ToString() ?? "0",
                        
                     };
                    result.DataList.Add(evt);
                }
               
               
            }

            return result;
        }
    }
}
