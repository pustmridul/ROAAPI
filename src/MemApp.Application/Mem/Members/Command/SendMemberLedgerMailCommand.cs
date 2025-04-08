using MediatR;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Interfaces;
using MemApp.Application.Mem.MemberStatuss.Command;
using MemApp.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MemApp.Application.Mem.Reports.MemberReport.Model;
using MemApp.Application.Services;
using Dapper;
using AutoMapper.Execution;
using MemApp.Application.Extensions;

namespace MemApp.Application.Mem.Members.Command
{
    public class SendMemberLedgerMailCommand : IRequest<Result>
    {
        public int Id { get; set; }
        public string TemplatePath { get; set; } = string.Empty;


    }

    public class SendMemberLedgerMailCommandHandler : IRequestHandler<SendMemberLedgerMailCommand, Result>
    {
        private readonly IDapperContext _context;
        private readonly IUserLogService _userLogService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IPermissionHandler _permissionHandler;
        private readonly IBroadcastHandler _broadcastHandler;

        public SendMemberLedgerMailCommandHandler(IBroadcastHandler broadcastHandler,IDapperContext context, ICurrentUserService currentUserService, IUserLogService userLogService, IPermissionHandler permissionHandler)
        {
            _context = context;
            _currentUserService = currentUserService;
            _userLogService = userLogService;
            _permissionHandler = permissionHandler;
            _broadcastHandler = broadcastHandler;
        }

        public async Task<Result> Handle(SendMemberLedgerMailCommand request, CancellationToken cancellationToken)
        {
            using (var connection = _context.CreateConnection())
            {
                var result = new Result();

                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"select c.Email,c.MembershipNo,c.CardNo,c.Phone,c.FullName MemberName,cl.DatesTime Dates,cl.Description,cl.PayType,cl.BankCreditCardName,cl.Amount from CustomerLedger cl left join Customer c on cl.PrvCusID = c.PrvCusID where c.IsActive=1 and c.Id={request.Id}");

                var dataQuery = await connection
                    .QueryAsync<MemberLedgerDetailReportVM>(sb.ToString());

                var template = RenderTemplate(request.TemplatePath, dataQuery.ToList());


                string message = template;
             


                string subject = "Service Buy (Cadet College Club Ltd) ";
                var mailSendStatus = await _broadcastHandler.SendEmail(dataQuery.FirstOrDefault().Email, subject, message);

                if (mailSendStatus==true)
                {
                    result.HasError = false;
                    return result;
                }
                else
                {
                    result.HasError = true;
                    return result;
                }
                


            }

        }


        private string RenderTemplate(string templatePath,List<MemberLedgerDetailReportVM> model)
        {
            var templateContent = File.ReadAllText(templatePath);

            // Replace placeholders with member information
            templateContent = templateContent.Replace("######MemberName######", model.FirstOrDefault().MemberName.ToString());
            templateContent = templateContent.Replace("######Phone######", model.FirstOrDefault().Phone.ToString());
            templateContent = templateContent.Replace("######CardNo######", model.FirstOrDefault().Phone.ToString());
            templateContent = templateContent.Replace("######MembershipNo######", model.FirstOrDefault().MembershipNo.ToString());

            // Build the HTML for the ledger report table
            var ledgerTableHtml = new StringBuilder();

            foreach (var item in model)
            {
                ledgerTableHtml.AppendLine("<tr>");
                ledgerTableHtml.AppendLine($"<td>{item.Dates}</td>");
                ledgerTableHtml.AppendLine($"<td>{item.Description}</td>");
                ledgerTableHtml.AppendLine($"<td>{item.PayType}</td>");
                ledgerTableHtml.AppendLine($"<td>{item.BankCreditCardName}</td>");
                ledgerTableHtml.AppendLine($"<td>{item.Amount}</td>");
                ledgerTableHtml.AppendLine("</tr>");
            }

            // Replace the placeholder in the template with the ledger report table HTML
            templateContent = templateContent.Replace("######LedgerReport######", ledgerTableHtml.ToString());

            return templateContent;
        }
    }
}
