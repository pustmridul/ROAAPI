using MediatR;
using MemApp.Application.Com.Commands.SaveUserPermission;
using MemApp.Application.Com.Models;
using MemApp.Application.Com.Queries.GetUserPermissions;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Application.Com.Commands.Transactions
{
    public class GetMemberLedgerCommand : IRequest<MemberLedgerListVm>
    {
        public string MemberId { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }

    }
    public class GetMemberLedgerCommandHandler : IRequestHandler<GetMemberLedgerCommand, MemberLedgerListVm>
    {
        private readonly IMemDbContext _context;
        private readonly IMediator _mediator;
        public GetMemberLedgerCommandHandler(IMemDbContext context, IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }
        public async Task<MemberLedgerListVm> Handle(GetMemberLedgerCommand request, CancellationToken cancellation)
        {
            var result = new MemberLedgerListVm();
            var dataList = await _context.MemLedgers.Where(i => i.PrvCusID == request.MemberId).Select(s => new MemberLedgerReq
            {
                Amount = s.Amount??0,
                BankCreditCardName = s.BankCreditCardName,
                ChequeCardNo = s.ChequeCardNo,
                CustomerLedgerID = s.CustomerLedgerID,
                Dates = s.Dates?? new DateTime(),
                DatesTime = s.Dates ?? new DateTime(),
                Description = s.Description,
                Notes = s.Notes,
                PayType = s.PayType,
                PrvCusID = s.PrvCusID,
                RefundId = s.RefundId,
                ServiceChargeAmount = s.ServiceChargeAmount??0,
                TOPUPID = s.TOPUPID,
                UpdateBy = s.UpdateBy,
                UpdateDate = s.UpdateDate ?? new DateTime()
            }).OrderByDescending(o=>o.CustomerLedgerID)
            .ToPaginatedListAsync(request.PageNo, request.PageSize,cancellation);

            result.DataCount= await _context.MemLedgers.Where(i => i.PrvCusID == request.MemberId).CountAsync(cancellation);

            result.DataList = dataList.Data;


            return  result;
        }
    }
}
