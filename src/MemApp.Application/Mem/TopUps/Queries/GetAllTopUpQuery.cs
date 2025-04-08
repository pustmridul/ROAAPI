using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Colleges.Models;
using System.Text;
using MemApp.Application.Services;
using Dapper;
using MemApp.Application.Mem.TopUps.Models;
using Microsoft.EntityFrameworkCore;
using MemApp.Application.Interfaces;

namespace MemApp.Application.Mem.TopUps.Queries
{
    public class GetAllTopUpQuery : IRequest<TopUpListVm>
    {
        public string? MemberShipNo { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
    public class GetAllTopUpQueryHandler : IRequestHandler<GetAllTopUpQuery, TopUpListVm>
    {
        private readonly IMemDbContext _context;
        private readonly IPermissionHandler _permissionHandler;
        public GetAllTopUpQueryHandler(IMemDbContext context, IPermissionHandler permissionHandler)
        {
            _context = context;
            _permissionHandler = permissionHandler;
        }

        public async Task<TopUpListVm> Handle(GetAllTopUpQuery request, CancellationToken cancellationToken)
        {
            var result = new TopUpListVm();        
            try
            {
                var st = request.StartDate.AddDays(-1);
                var ed = request.EndDate.AddDays(1);
                var data = await _context.TopUps
                    .Include(i=>i.TopUpDetails.Where(q=>q.IsActive))
                    .Where(q => q.IsActive 
                    && (!string.IsNullOrEmpty(request.MemberShipNo) ? q.MemberShipNo ==request.MemberShipNo: true)
                    && q.TopUpDate >=st && q.TopUpDate <= ed
                    ).Select(s => new TopUpReq
                    {

                        Id = s.Id,
                        MemberShipNo = s.MemberShipNo,
                        RegisterMemberId = s.MemberId,
                        TopUpDate = s.TopUpDate,
                        PaymentMode = s.PaymentMode,
                        Note = s.Note,
                        CardNo = s.CardNo,
                        Status = s.Status,
                        TotalAmount = s.TotalAmount,
                        CreateByName=s.CreatedByName,
                        TopUpDetails = s.TopUpDetails.Select(s => new TopUpDetailReq()
                        {
                            TrxCardNo = s.TrxCardNo,
                            Amount = s.Amount,
                            TopUpId = s.TopUpId,
                            PaymentMethodId = s.PaymentMethodId,
                            PaymentMethodText = s.PaymentMethodText,
                            MachineNo = s.MachineNo,
                            Id = s.Id

                        }).ToList(),
                    }).OrderByDescending(o=> o.Id)
                    .ToPaginatedListAsync(request.PageNumber, request.PageSize, cancellationToken);

                result.DataList = data.Data;

                result.DataCount = data.TotalCount;        
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
