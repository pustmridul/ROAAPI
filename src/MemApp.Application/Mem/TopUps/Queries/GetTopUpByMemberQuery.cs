using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.TopUps.Models;

namespace MemApp.Application.Mem.TopUps.Queries
{
    public class GetTopUpByMemberQuery : IRequest<TopUpListVm>
    {
        public string? MemberShipNo { get; set; }
        public string? CardNo { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }
    }


    public class GetTopUpMemberQueryHandler : IRequestHandler<GetTopUpByMemberQuery, TopUpListVm>
    {
        private readonly IMemDbContext _context;

        public GetTopUpMemberQueryHandler(IMemDbContext context)
        {
            _context = context;
        }

        public async Task<TopUpListVm> Handle(GetTopUpByMemberQuery request, CancellationToken cancellationToken)
        {
            var result = new TopUpListVm();
            try
            {
                var data = await _context.TopUps.Where(q => 
                (!string.IsNullOrEmpty(request.MemberShipNo)? q.MemberShipNo == request.MemberShipNo: true)
                &&
                (!string.IsNullOrEmpty(request.CardNo) ? q.CardNo == request.CardNo : true)
                && q.IsActive)
                    .ToPaginatedListAsync(request.PageNo, request.PageSize, cancellationToken); ;


                if (data.TotalCount == 0)
                {
                    result.HasError = true;
                    result.Messages.Add("Data Not Found");
                }
                else
                {
                    result.HasError = false;
                    result.DataCount = data.TotalCount;
                    result.DataList = data.Data.Select(s => new TopUpReq
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
                    }).ToList();
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
