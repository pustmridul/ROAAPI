using MediatR;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Events.Models;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Mem.Events.Queries
{
    public class GetMemberEventTicketCountQuery : IRequest<EventTicketBuyInfo>
    {
        public int MemberId { get; set; }
        public int EventId { get; set; }
    }


    public class GetMemberEventTicketCountQueryHandler : IRequestHandler<GetMemberEventTicketCountQuery, EventTicketBuyInfo>
    {
        private readonly IMemDbContext _context;
        public GetMemberEventTicketCountQueryHandler(IMemDbContext context)
        {
            _context = context;
        }

        public async Task<EventTicketBuyInfo> Handle(GetMemberEventTicketCountQuery request, CancellationToken cancellationToken)
        {
            var result = new EventTicketBuyInfo();
            var data = await _context.SaleEventTickets
                .Include(i => i.SaleEventTicketDetails.Where(q => q.IsActive && q.EventId== request.EventId && q.SaleStatus !="Cancel"))
                .Where(q => q.IsActive).ToListAsync(cancellationToken);

            var serviceTicket = await _context.ServiceTickets
                .Include(i=>i.ServiceTicketDetails)
                .SingleOrDefaultAsync(q => q.Id == request.EventId, cancellationToken);

            
            if (data == null)
            {
                result.HasError = true;
                result.Messages.Add("Data Not Found");
            }
            else
            {
                result.HasError = false;
                if(serviceTicket != null)
                { 
                    List<MemberTicketCriteriaBuyInfo > resultList = new List<MemberTicketCriteriaBuyInfo>();
                    
                    foreach (var item in serviceTicket.ServiceTicketDetails)
                    {
                        var buyCount = data.Where(q=>q.MemberId== request.MemberId).Sum(s => s.SaleEventTicketDetails.Where(q => q.TicketCriteriaId == item.ServiceTicketTypeId).Count());
                        var objdetail = new MemberTicketCriteriaBuyInfo()
                        {
                            TicketCriteriaId = item.ServiceTicketTypeId ?? 0,
                            TicketCriteriaCount = buyCount,
                        };
                        // result.BuyTicketCount += item.SaleEventTicketDetails.Count();
                        resultList.Add( objdetail );
                    }
                    result.TotalSaleCount = data.Sum(s=>s.SaleEventTicketDetails.Count());
                    
                    result.MemberTicketCriteriaBuyInfos= resultList;
                }    
               
            }
            
            return result;
        }
    }
}
