using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using MemApp.Application.Mem.Events.Models;
using MemApp.Application.Mem.MemServices.Models;

namespace MemApp.Application.Mem.Booking.Queries
{
    public class GetAllSaleEventTicketQuery : IRequest<SaleEventTicketListVm>
    {
       public SaleEventTicketSearchReq Model { get; set; }= new SaleEventTicketSearchReq();
    }


    public class GetAllSaleEventTicketQueryHandler : IRequestHandler<GetAllSaleEventTicketQuery, SaleEventTicketListVm>
    {
        private readonly IMemDbContext _context;
        private readonly IPermissionHandler _permissionHandler;
        
        public GetAllSaleEventTicketQueryHandler(IMemDbContext context,IPermissionHandler permissionHandler)
        {
            _context = context;
            _permissionHandler = permissionHandler;

        }

        public async Task<SaleEventTicketListVm> Handle(GetAllSaleEventTicketQuery request, CancellationToken cancellationToken)
        {
            var result = new SaleEventTicketListVm();
            if (!await _permissionHandler.HasRolePermissionAsync(3803))
            {
                result.HasError = true;
                result.Messages?.Add("You have no permission to view");
                return result;
            }

            try
            {

                var data = await _context.SaleEventTickets
                    .Include(i => i.SaleEventTicketDetails.Where(q => q.IsActive && q.SaleStatus!="Cancel"))
                    .ThenInclude(c=>c.Event)
                    .Where(q => q.IsActive
                    && q.SaleStatus != "Cancel"
                    && (!(request.Model.StartDate == null ) ? q.InvoiceDate >= request.Model.StartDate : true)
                    && (!(request.Model.EndDate == null ) ? q.InvoiceDate <= request.Model.EndDate : true)
                    && ( q.SaleStatus!= "Cancel")
                    && (!string.IsNullOrEmpty(request.Model.SearchText) ? q.MemberShipNo.Contains(request.Model.SearchText) : true)
                    ).OrderByDescending(o=>o.Id)
                    .AsNoTracking()
                    .ToPaginatedListAsync(request.Model.PageNo, request.Model.PageSize, cancellationToken);

                if (data.TotalCount == 0)
                {
                    result.HasError = false;
                    result.Messages?.Add("Data Not Found");
                }
                else
                {
                    
                    var availabilitydata = await _context.AvailabilityDetails.Where(q => q.IsActive).ToListAsync(cancellationToken);
                   
                    result.HasError = false;
                    result.DataCount = data.TotalCount;
                    result.DataList = data.Data.Select(s => new SaleEventTicketReq
                    {
                        Id = s.Id,
                        InvoiceNo = s.InvoiceNo,
                        InvoiceDate = s.InvoiceDate.AddHours(-6),
                        SaleStatus = s.SaleStatus,
                        MemberShipNo = s.MemberShipNo,
                        MemberId = s.MemberId,
                        Amount = s.Amount,
                        TotalAmount=s.TotalAmount,
                        VatAmount=s.VatAmount,
                        ServiceAmount=s.ServiceAmount,
                        PaymentAmount=s.PaymentAmount,
                        PaymentDate=s.PaymentDate,
                       
                        OrderFrom = s.OrderFrom,
                        SaleEventTicketDetailReqs = s.SaleEventTicketDetails.Select(s1=> new SaleEventTicketDetailReq
                        {
                            Id=s1.Id,
                            EventId = s1.EventId,
                            EventTitle= s1.EventTitle,                           
                            EventTokens= s1.EventTokens,
                            AreaLayoutId= s1.AreaLayoutId,
                            AreaLayoutTitle= s1.AreaLayoutTitle,
                            TableId=s1.TableId,
                            TableTitle=s1.TableTitle,
                            NoofChair=s1.NoofChair,
                            TicketCriteriaId = s1.TicketCriteriaId,
                            TicketCriteria = s1.TicketCriteria,
                            TicketPrice = s1.TicketPrice,
                            TicketText = s1.TicketText,
                            VatAmount = s1.VatAmount,
                            ServiceChargeAmount = s1.ServiceChargeAmount,
                            Event = new ServiceTicketReq
                            {
                                StartDate = s1.Event.StartDate,
                                EndDate = s1.Event.EndDate,
                                EventDate = s1.Event.EventDate

                            }

                        }).ToList()

                    }).ToList();
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
