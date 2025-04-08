using AutoMapper.Execution;
using MediatR;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.TopUps.Models;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Mem.TopUps.Queries
{
    public class GetTopUpByIdQuery : IRequest<TopUpVm>
    {
        public int Id { get; set; }
    }


    public class GetTopUpByIdQueryHandler : IRequestHandler<GetTopUpByIdQuery, TopUpVm>
    {
        private readonly IMemDbContext _context;

        public GetTopUpByIdQueryHandler(IMemDbContext context)
        {
            _context = context;
        }

        public async Task<TopUpVm> Handle(GetTopUpByIdQuery request, CancellationToken cancellationToken)
        {
            var result = new TopUpVm();
            try
            {
                var data = await _context.TopUps
                    .Include(i=>i.MemberRegistrationInfo)
                   // .Include(i=>i.RegisterMember)
                    .Include(i=>i.TopUpDetails.Where(q=>q.IsActive))
                    .SingleOrDefaultAsync(q => q.Id == request.Id && q.IsActive, cancellationToken);
                   
                if (data == null)
                {
                    result.HasError = true;
                    result.Messages?.Add("Data Not Found");
                }
                else
                {
                    var topupIds = await _context.MemLedgers
                        .Select(s=> new
                        {
                            s.TopUpDetailId,
                            s.TOPUPID
                        })
                        .Where(q => data.TopUpDetails.Select(s => s.Id).ToList().Contains(q.TopUpDetailId ?? 0))
                        .AsNoTracking()
                        .ToListAsync(cancellationToken);
                  
                 //   var currentBalance = await _context.MemLedgers.Where(q => q.PrvCusID == data.RegisterMember.PrvCusID).SumAsync(s => s.Amount, cancellationToken) ?? 0;
                    var currentBalance = await _context.MemLedgers.Where(q => q.PrvCusID == data.MemberRegistrationInfo.Id.ToString()).SumAsync(s => s.Amount, cancellationToken) ?? 0;
                 
                    result.HasError = false;
                    result.Data = new TopUpReq
                    {
                        Id = data.Id,
                        //MemberName= data.RegisterMember?.FullName,
                        //MemberCardNo= data.RegisterMember?.CardNo,
                        MemberShipNo = data.MemberShipNo,
                       // RegisterMemberId = data.RegisterMemberId,
                        RegisterMemberId = data.MemberId,
                        TopUpDate = data.TopUpDate,
                        PaymentMode = data.PaymentMode,
                        CurrentBalance= currentBalance,
                        CreateByName= data.CreatedByName,
                        Note=data.Note,
                        CardNo = data.CardNo,
                        Status = data.Status,
                        TotalAmount = data.TotalAmount,

                        TopUpDetails= data.TopUpDetails.Select(s=> new TopUpDetailReq()
                        {
                            TOPUPNO= topupIds.FirstOrDefault(q=>q.TopUpDetailId==s.Id)?.TOPUPID,
                            TrxCardNo = s.TrxCardNo,
                            Amount = s.Amount,
                            TopUpId = s.TopUpId,
                            PaymentMethodId = s.PaymentMethodId,
                            PaymentMethodText=s.PaymentMethodText,
                            MachineNo = s.MachineNo,
                            Id= s.Id

                        }).ToList(),
                        
                    };
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
