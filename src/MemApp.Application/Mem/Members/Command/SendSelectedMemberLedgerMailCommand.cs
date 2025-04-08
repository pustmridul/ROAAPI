using Dapper;
using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces;
using MemApp.Application.Mem.Reports.MemberReport.Model;
using MemApp.Application.Models.DTOs;
using MemApp.Application.Services;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Application.Mem.Members.Command
{
    public class SendSelectedMemberLedgerMailCommand : IRequest<Result>
    {
        public MailMemberLedgerDto Model { get; set; }
        public string TemplatePath { get; set; } = string.Empty;
    }

    public class SendSelectedMemberLedgerMailCommandHandler : IRequestHandler<SendSelectedMemberLedgerMailCommand, Result>
    {
        private readonly IDapperContext _context;
        private readonly IUserLogService _userLogService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IPermissionHandler _permissionHandler;
        private readonly IBroadcastHandler _broadcastHandler;
        private readonly ILogger<SendSelectedMemberLedgerMailCommandHandler> _logger;

        public SendSelectedMemberLedgerMailCommandHandler(ILogger<SendSelectedMemberLedgerMailCommandHandler> logger, IBroadcastHandler broadcastHandler, IDapperContext context, ICurrentUserService currentUserService, IUserLogService userLogService, IPermissionHandler permissionHandler)
        {
            _context = context;
            _currentUserService = currentUserService;
            _userLogService = userLogService;
            _permissionHandler = permissionHandler;
            _broadcastHandler = broadcastHandler;
            _logger = logger;
        }

        public async Task<Result> Handle(SendSelectedMemberLedgerMailCommand request, CancellationToken cancellationToken)
        {
            using (var connection = _context.CreateConnection())
            {
                var result = new Result();
                foreach (var MembershipNo in request.Model.MembershipNoList)
                {


                    //StringBuilder sb = new StringBuilder();
                    //sb.AppendLine($"select c.Email,c.MembershipNo,c.CardNo,c.Phone,c.FullName MemberName,cl.DatesTime Dates,cl.Description,cl.PayType,cl.BankCreditCardName,cl.Amount from CustomerLedger cl left join Customer c on cl.PrvCusID = c.PrvCusID where c.IsActive=1 and c.Id={id}");

                    //var dataQuery = await connection
                    //    .QueryAsync<MemberLedgerDetailReportVM>(sb.ToString());


                    var parameters = new DynamicParameters();
                    parameters.Add("@MembershipNo", MembershipNo == "null" ? null : MembershipNo, DbType.String, size: 10);
                    parameters.Add("@FromDate", request.Model.FromDate?.Date, DbType.Date);
                    parameters.Add("@ToDate", request.Model.ToDate?.Date, DbType.Date);

                    var dataList = await connection.QueryAsync<MemberLedgerDetailReportVM>(
                        "SP_MemberLedgerDetail",
                        parameters,
                        commandType: CommandType.StoredProcedure
                    );


                    if (dataList.Count() > 0)
                    {
                        var template = RenderTemplate(request.TemplatePath, dataList.ToList(), request.Model);


                        string message = template;



                        string subject = request.Model.Subject;
                        var mailSendStatus = await _broadcastHandler.SendEmail(dataList.FirstOrDefault().Email, subject, message);
                        if (mailSendStatus == false)
                        {
                            var logMessage = $"Failed to send email for member Id= {MembershipNo}";

                            // Log the error in database if necessary
                            var sql = "INSERT INTO mem_MemberLedgerMailLog (Message, Level, TimeStamp) VALUES (@Message, @Level, @TimeStamp)";
                            await connection.ExecuteAsync(sql, new { Message = logMessage, Level = "Error", TimeStamp = DateTime.Now });

                        }
                    }



                }
                result.HasError = false;
                return result;


            }

        }


        private string RenderTemplate(string templatePath, List<MemberLedgerDetailReportVM> model, MailMemberLedgerDto requestModel)
        {
            var templateContent = File.ReadAllText(templatePath);

            // Replace placeholders with member information
            templateContent = templateContent.Replace("######MemberName######", model.FirstOrDefault().MemberName.ToString());
            templateContent = templateContent.Replace("######Phone######", model.FirstOrDefault().Phone.ToString());
            templateContent = templateContent.Replace("######CardNo######", model.FirstOrDefault().Phone.ToString());
            templateContent = templateContent.Replace("######MembershipNo######", model.FirstOrDefault().MembershipNo.ToString());
            templateContent = templateContent.Replace("######FromDate######", requestModel.FromDate?.Date.ToString("dd-MMMM-yyyy"));
            templateContent = templateContent.Replace("######ToDate######", requestModel.ToDate?.Date.ToString("dd-MMMM-yyyy"));

            // Build the HTML for the ledger report table
            decimal cumulativeAmount = model.FirstOrDefault().OpeningBalance;
            var ledgerTableHtml = new StringBuilder();

            ledgerTableHtml.AppendLine("<tr>");
            ledgerTableHtml.AppendLine($"<td colspan=6 >Opening Balance</td>");
            ledgerTableHtml.AppendLine($"<td>{model.FirstOrDefault().OpeningBalance}</td>");

            ledgerTableHtml.AppendLine("</tr>");

           
            foreach (var item in model)
            {

                cumulativeAmount += item.Amount;

                ledgerTableHtml.AppendLine("<tr>");
                ledgerTableHtml.AppendLine($"<td>{item.Dates}</td>");
                ledgerTableHtml.AppendLine($"<td>{item.Description}</td>");
                ledgerTableHtml.AppendLine($"<td>{item.LedgerType}</td>");
                ledgerTableHtml.AppendLine($"<td>{item.PayType}</td>");
                ledgerTableHtml.AppendLine($"<td>{item.BankCreditCardName}</td>");
                ledgerTableHtml.AppendLine($"<td>{item.Amount}</td>");
                ledgerTableHtml.AppendLine($"<td>{cumulativeAmount}</td>");
                ledgerTableHtml.AppendLine("</tr>");
            }
            ledgerTableHtml.AppendLine("<tr>");
            ledgerTableHtml.AppendLine($"<td colspan=5 class='color-red'>Closing Balance</td>");
            ledgerTableHtml.AppendLine($"<td class='color-red'>{cumulativeAmount}</td>");

            ledgerTableHtml.AppendLine("</tr>");
            // Replace the placeholder in the template with the ledger report table HTML
            templateContent = templateContent.Replace("######LedgerReport######", ledgerTableHtml.ToString());

            return templateContent;
        }
    }
}
