using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.MiscSales.Models;
using MemApp.Application.Services;
using MemApp.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Mem.MiscSales.Queries
{
    public class GetAllMiscSaleQuery : IRequest<MiscSaleListVm>
    {
       public  MiscSaleSearchModel SearchModel { get; set; }= new MiscSaleSearchModel();
    }

    public class GetAllMiscSaleQueryHandler : IRequestHandler<GetAllMiscSaleQuery, MiscSaleListVm>
    {
        private readonly IMemDbContext _context;
        private readonly IDapperContext _dapperContext;
        private readonly IPermissionHandler _permissionHandler;
        
        public GetAllMiscSaleQueryHandler(IMemDbContext context, IDapperContext dapperContext, IPermissionHandler permissionHandler)
        {
            _context = context;
            _dapperContext = dapperContext;
            _permissionHandler = permissionHandler;

        }

        public async Task<MiscSaleListVm> Handle(GetAllMiscSaleQuery request, CancellationToken cancellationToken)
        {
            var result = new MiscSaleListVm();
            try
            {
               

                var data = await _context.MiscSales
                    .Include(i => i.RegisterMember)
                    .Where(q => (request.SearchModel.StartDate != null ? q.InvoiceDate >= request.SearchModel.StartDate : true)
                    && (request.SearchModel.EndDate != null ? q.InvoiceDate <= request.SearchModel.EndDate : true)
                && (!string.IsNullOrEmpty(request.SearchModel.SearchText) ? q.InvoiceNo.ToLower().Contains(request.SearchModel.SearchText.ToLower()) : true))
                    .AsNoTracking()
                    .ToPaginatedListAsync(request.SearchModel.PageNo, request.SearchModel.PageSize, cancellationToken);



                if (data.TotalCount == 0)
                {
                    result.HasError = false;
                    result.Messages.Add("Data Not Found");
                }
                else
                {
                    result.HasError = false;
                    result.Count = Convert.ToInt32(data.TotalCount);
                    result.Data = data.Data.Select(s => new MiscSaleReq
                    {
                        Id = s.Id,
                        InvoiceNo = s.InvoiceNo,
                        InvoiceDate = s.InvoiceDate,
                        MemberId = s.MemberId,
                        MemberText = s.RegisterMember?.FullName,
                        MemberShipNo=s.RegisterMember?.MembershipNo,
                        Note = s.Note,
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
