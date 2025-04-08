using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.ServiceSales.Models;
using System.Text;
using MemApp.Application.Services;
using Dapper;
using MemApp.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace MemApp.Application.Mem.ServiceSales.Queries
{
    public class GetAllServiceSaleQuery : IRequest<ServiceSaleListVm>
    {
        public ServiceSaleSearch Model { get; set; }= new ServiceSaleSearch();
    }
    public class GetAllServiceSaleQueryHandler : IRequestHandler<GetAllServiceSaleQuery, ServiceSaleListVm>
    {
        private readonly IMemDbContext _context;
        private readonly IDapperContext _dapperContext;
        private readonly IPermissionHandler _permissionHandler;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetAllServiceSaleQueryHandler(IMemDbContext context, IDapperContext dapperContext, IPermissionHandler permissionHandler, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _dapperContext = dapperContext;
            _permissionHandler = permissionHandler;
            _httpContextAccessor = httpContextAccessor;

    }

        public async Task<ServiceSaleListVm> Handle(GetAllServiceSaleQuery request, CancellationToken cancellationToken)
        {
            var result = new ServiceSaleListVm();
            if (!await _permissionHandler.HasRolePermissionAsync(3703))
            {
                result.HasError = true;
                result.Messages?.Add("You have no permission to view");
                return result;
            }
            string baseUrl = _httpContextAccessor.HttpContext.Request.Scheme + "://" + _httpContextAccessor.HttpContext.Request.Host + _httpContextAccessor.HttpContext.Request.PathBase;

            try
            {
                var data = await _context.ServiceSales
                .Include(i=>i.RegisterMember)
                
                 .Include(i=>i.ServiceSaleDetails.Where(q=>q.IsActive && q.SaleStatus!="Cancel"))

                     //// .Include(i=>i.ServiceSaleAvailability.Where(q=>q.IsActive))
                      .Where(q => q.IsActive
                      && q.SaleStatus != "Cancel"
                     && (!(request.Model.StartDate == null || request.Model.StartDate=="") ? q.InvoiceDate >= DateTime.Parse(request.Model.StartDate) : true)
                         && (!(request.Model.EndDate == null || request.Model.EndDate == "") ? q.InvoiceDate <= DateTime.Parse(request.Model.EndDate) : true)
                     //   && (request.Model.TicketCriteriaId >0 ? q.ServiceCriteriaId == request.Model.TicketCriteriaId : true)
                         && (!string.IsNullOrEmpty(request.Model.SearchText) ? q.MembershipNo.Contains(request.Model.SearchText) : true)
                      ) .OrderByDescending(o => o.Id)
                     .ToPaginatedListAsync(request.Model.PageNo, request.Model.PageSize, cancellationToken);



                if (data.TotalCount == 0)
                {
                    result.HasError = true;
                    result.Messages?.Add("Data Not Found");
                }
                else
                {
                    result.HasError = false;
                    result.DataCount = data.TotalCount;
                    result.DataList = data.Data.Select(s => new ServiceSaleReq
                    {
                        Id = s.Id,
                        InvoiceDate = s.InvoiceDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                        IsActive = s.IsActive,
                        MemberId = s.MemberId,
                        InvoiceNo = s.InvoiceNo,
                        MembershipNo = s.MembershipNo,
                        OrderFrom = s.OrderFrom,
                        TotalAmount = s.TotalAmount,
                        Amount = s.Amount,
                        VatAmount = s.VatAmount,
                        ServiceChargeAmount = s.ServiceChargeAmount,
                        Note = s.Note,

                        ServiceSaleDetailReqs = s.ServiceSaleDetails.Select(s => new ServiceSaleDetailReq
                        {
                            Id = s.Id,
                            IsActive = s.IsActive,
                            Quantity = s.Quantity,
                            UnitName = s.UnitName,
                            UnitPrice = s.UnitPrice,
                            TicketText=s.TicketText,
                            TicketCodeNo = s.TicketCodeNo,
                            SeviceTicketAvailablityId = s.SeviceTicketAvailablityId,
                            EndTime = s.EndTime,
                            ServiceCriteriaId = s.ServiceCriteriaId,
                            StartTime = s.StartTime,
                            DayText=s.DayText,
                            SaleDay = s.RevDay,
                            RevDate=s.RevDate,
                            SaleMonth = s.RevMonth,
                            SaleYear = s.RevYear,
                            ServiceSaleId = s.ServiceSaleId,
                            VatChargeAmount = s.VatChargeAmount,
                            ServiceChargeAmount = s.ServiceChargeAmount,
                            ServiceChargePercent = s.ServiceChargePercent,
                            VatChargePercent = s.VatChargePercent,
                            ServiceTicketId = s.ServiceTicketId
                        }).ToList(),                      
                        ImgFileUrl = baseUrl + s.RegisterMember?.ImgFileUrl,
                        MemberName = s.RegisterMember?.FullName ?? ""
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
