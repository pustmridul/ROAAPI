using MediatR;
using MemApp.Application.Exceptions;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Payment.Model;
using MemApp.Application.Models;
using MemApp.Application.PaymentGateway.SslCommerz.Model;
using MemApp.Application.Services;
using MemApp.Domain.Entities.com;
using MemApp.Domain.Entities.mem;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace MemApp.Application.Mem.TopUps.Command
{
    public class SendTopUpSmsEmailCommand : IRequest<Result>
    {
        public string TrxNo { get; set; }

    }
    public class SendTopUpSmsEmailCommandHandler : IRequestHandler<SendTopUpSmsEmailCommand, Result>
    {
        private readonly IMemDbContext _context;
        private readonly IBroadcastHandler _broadcastHandler;
        private readonly IMemLedgerService _memLedgerService;
        public SendTopUpSmsEmailCommandHandler(
            IMemDbContext context,
            IBroadcastHandler broadcastHandler, 
            IMemLedgerService memLedgerService)
        {
            _context = context;
            _broadcastHandler = broadcastHandler;
            _memLedgerService = memLedgerService;
        }

        public async Task<Result> Handle(SendTopUpSmsEmailCommand request, CancellationToken cancellationToken)
        {
          
            try
            {
                var result = new Result();
                var tobj = await _context.TopUps
                    .Include(i => i.TopUpDetails)
                    .SingleOrDefaultAsync(q => q.Note == request.TrxNo, cancellationToken);
                if (tobj == null)
                {
                    throw new NotFoundException();
                }
                else
                {
                    var memberObj = await _context.RegisterMembers
                                  .Select(s => new { s.Phone, s.Email, s.Id, s.MembershipNo, s.PrvCusID })
                                  .SingleOrDefaultAsync(q => q.Id == tobj.MemberId, cancellationToken);

                    if (memberObj != null)
                    {
                        var curentBalance = await _memLedgerService.GetCurrentBalance(memberObj.Id.ToString());
                        string message = "";
                        string subject = "";
                        if (memberObj.Phone != null)
                        {
                            message = "Dear Member," + memberObj.MembershipNo + ", Your wallet amount : " + tobj.TotalAmount + " is added, your current Balabnce : " + curentBalance;
                            await _broadcastHandler.SendSms(memberObj.Phone, message, "English");
                            message = "";
                        }

                        if (memberObj.Email != null)
                        {

                            message = "Dear Member," + memberObj.MembershipNo + ", Your wallet amount : " + tobj.TotalAmount + " is added, your current Balabnce : " + curentBalance;
                            subject = "Wallet amount added (Cadet College Club Ltd) ";
                            await _broadcastHandler.SendEmail(memberObj.Email, subject, message);
                            message = "";
                            subject = "";
                        }
                    }

                  
                }
                result.HasError = false;
                result.Messages.Add("Save Success");
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
