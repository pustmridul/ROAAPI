using MediatR;
using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.TopUps.Models;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Mem.TopUps.Queries
{
    public class GetCurrentBalabnceByMemberIdQuery : IRequest<CurrentBalabnceReq>
    {
        public int MemberId { get; set; }
    }
    public class GetCurrentBalabnceByMemberIdQueryHandler : IRequestHandler<GetCurrentBalabnceByMemberIdQuery, CurrentBalabnceReq>
    {
        private readonly IMemLedgerService _memLedgerService;

        public GetCurrentBalabnceByMemberIdQueryHandler(IMemLedgerService memLedgerService)
        {
            _memLedgerService = memLedgerService;
        }

        public async Task<CurrentBalabnceReq> Handle(GetCurrentBalabnceByMemberIdQuery request, CancellationToken cancellationToken)
        {
            var result = new CurrentBalabnceReq();
            result.CurrentBalabnce= await _memLedgerService.GetCurrentBalance(request.MemberId.ToString());


            return result;
        }
    }
}
