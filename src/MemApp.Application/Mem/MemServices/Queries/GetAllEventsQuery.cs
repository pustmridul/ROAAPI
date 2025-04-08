using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.AreaLayouts.Models;
using MemApp.Application.Mem.AreaLayouts.Queries;
using MemApp.Application.Mem.MemServices.Models;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Mem.MemServices.Queries
{
    public class GetAllEventsQuery : IRequest<EventListVm>
    {
    }
    public class GetAllEventsQueryHandler : IRequestHandler<GetAllEventsQuery, EventListVm>
    {
        private readonly IMemDbContext _context;
        private readonly IMediator _mediator;
        public GetAllEventsQueryHandler(IMemDbContext context, IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }

        public async Task<EventListVm> Handle(GetAllEventsQuery request, CancellationToken cancellationToken)
        {
            var result = new EventListVm();
            try
            {
                var data = await _context.ServiceTickets
                    .Include(i=>i.MemServices)
                    .Include(i=>i.EventTokens.Where(q=>q.IsActive))
                    .Include(i=>i.ServiceTicketDetails).ThenInclude(i=>i.ServiceTicketType)
                    .Include(i=>i.SerTicketAreaLayouts).ThenInclude(i=>i.SerTicketAreaLayoutMatrices)
                    .Where(q => q.IsActive && q.MemServiceTypeId==6 && q.EndDate.Value.Date >=DateTime.Now.Date)
                    .ToPaginatedListAsync(1,10000, cancellationToken);

                if (data.TotalCount == 0)
                {
                    result.HasError = true;
                    result.Messages.Add("Data Not Found");
                }
                else
                {
                    
                    var saleList = await _context.SaleEventTicketDetails
                        .Where(q => q.IsActive && (q.SaleStatus!= "Cancel" || q.SaleStatus == null)).ToListAsync(cancellationToken);

                    var layoutMatrix =await _context.AreaTableMatrixs.ToListAsync(cancellationToken);


                    result.HasError = false;
                    result.DataCount = data.TotalCount;
                    foreach(var e in data.Data)
                    {
                        var s = new EventReq();
                        s.Id = e.Id;
                        s.Title = e.Title??"";
                        s.ServiceName = e.MemServices.Title;
                        s.Location = e.Location;
                        s.Description= e.Description;
                        s.ImgFileUrl = e.ImgFileUrl;
                        s.TicketLimit = e.TicketLimit;
                        s.PromoCode = e.PromoCode;
                        s.VatPercent = e.VatChargePercent;
                        s.ServicePercent = e.ServiceChargePercent;
                        s.EventDate= e.EventDate;
                        s.StartDate = e.StartDate;
                        s.EndDate = e.EndDate;
                        s.VatPercent = e.VatChargePercent;
                        s.ServicePercent = e.ServiceChargePercent;
                        foreach (var i in e.EventTokens)
                        {
                            var et= new EventTokenReq();
                            et.Id = i.Id;
                            et.TokenCode = i.TokenCode;
                            et.TokenTitle = i.TokenTitle;
                            s.EventTokenReqs.Add(et);
                        }
                        foreach(var bc in e.ServiceTicketDetails)
                        {
                            var b = new BookingCriteria();
                            b.Id = bc.ServiceTicketTypeId??0;
                            b.Price = bc.UnitPrice;
                            b.Title =bc.ServiceTicketType !=null? bc.ServiceTicketType.Title : "";
                            b.MaxQuantity= bc.MaxQuantity;
                            b.MinQuantity= bc.MinQuantity;
                            b.SaleQty = saleList.Where(q=>q.TicketCriteriaId==bc.ServiceTicketTypeId && q.EventId==e.Id).Count();
                            s.BookingCriterias.Add(b);
                        }
                        foreach (var bc in e.SerTicketAreaLayouts)
                        {
                            var sa = new AreaLayoutTableDetail();
                            sa.AreaLayoutId = bc.AreaLayoutId;
                            sa.Title = bc.AreaLayoutTitle !=null? bc.AreaLayoutTitle : "";
                            foreach(var st in bc.SerTicketAreaLayoutMatrices)
                            {
                                var td = new TableDetailVm();
                                td.Id = st.TableId;
                                
                                td.TableName = st.TableTitle!=null? st.TableTitle: "";
                                for(int i=1; i<=st.NoofChair; i++)
                                {
                                    var c = new ChairVm();
                                    c.ChairNo = i;
                                    c.ChairKeyNo = layoutMatrix.FirstOrDefault(q=>q.AreaLayoutId==sa.AreaLayoutId && q.TableId==st.TableId && q.ChairNo==i)?.ChairKeyNo ??  Guid.NewGuid().ToString();
                                    c.IsSale = saleList.Where(q=> q.EventId== s.Id && q.AreaLayoutId== sa.AreaLayoutId && q.TableId==st.TableId && q.NoofChair==i).Count()>0? true : false;
                                    td.ChairList.Add(c);
                                }
                                sa.TableListVm.Add(td);

                            }
                            s.layoutTableDetails.Add(sa);
                        }
                        result.DataList.Add(s);

                    }
                  
                }
            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.Messages.Add(ex.ToString());
            }
            
            return result;
        }
    }
}
