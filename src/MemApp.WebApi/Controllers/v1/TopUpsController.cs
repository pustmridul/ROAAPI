using MediatR;
using MemApp.Application;
using MemApp.Application.Com.Commands.Transactions;
using MemApp.Application.Extensions;
using MemApp.Application.Mem.Reports.Common;
using MemApp.Application.Mem.TopUps.Command;
using MemApp.Application.Mem.TopUps.Models;
using MemApp.Application.Mem.TopUps.Queries;
using MemApp.Application.PaymentGateway.SslCommerz.Command;
using MemApp.Application.PaymentGateway.SslCommerz.Model;
using MemApp.Reporting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Res.WebApi.Controllers;
using ResApp.Application.Com.Commands.RoMemberLedgers;
using Serilog;
using System.Data;
using System.Globalization;

namespace MemApp.WebApi.Controllers.v1
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TopUpsController : ApiNewControllerBase
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        public TopUpsController(IWebHostEnvironment webHostEnvironment, ISender sender) : base(sender)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> SendTopUpSmsEmail(string trxNo)
        {
            var result = await Mediator.Send(new SendTopUpSmsEmailCommand() { TrxNo = trxNo });
            return Ok(result);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost]
        public async Task<IActionResult> SavePayment()
        {
            SSLCommerzValidatorResponseReq model = new SSLCommerzValidatorResponseReq();

            model.APIConnect = Request.Form["APIConnect"];
            model.status = Request.Form["status"];
            model.sessionkey = Request.Form["sessionkey"];
            model.tran_date = DateTime.Parse(Request.Form["tran_date"]);
            model.tran_id = Request.Form["tran_id"];
            model.val_id = Request.Form["val_id"];
            model.amount = Convert.ToDecimal(Request.Form["amount"]);
            model.store_amount = Convert.ToDecimal(Request.Form["store_amount"]);
            model.card_type = Request.Form["card_type"];
            model.card_no = Request.Form["card_no"];
            model.currency = Request.Form["currency"];
            model.bank_tran_id = Request.Form["bank_tran_id"];
            model.card_issuer = Request.Form["card_issuer"];
            model.card_brand = Request.Form["card_brand"];
            model.card_issuer_country = Request.Form["card_issuer_country"];
            model.card_issuer_country_code = Request.Form["card_issuer_country_code"];
            model.currency_type = Request.Form["currency_type"];
            model.currency_amount = Convert.ToDecimal(Request.Form["currency_amount"]);
            model.emi_instalment = Convert.ToInt32(Request.Form["emi_instalment"]);
            model.emi_amount = Convert.ToDecimal(Request.Form["emi_amount"]);
            model.discount_percentage = Convert.ToDecimal(Request.Form["discount_percentage"]);
            model.discount_remarks = Request.Form["discount_remarks"];
            model.value_a = Request.Form["value_a"];
            model.value_b = Request.Form["value_b"];
            model.value_c = Request.Form["value_c"];
            model.value_d = Request.Form["value_d"];
            model.risk_level = Convert.ToInt32(Request.Form["risk_level"]);
            model.risk_title = Request.Form["risk_title"];

            Log.Information("TopUp :" + JsonConvert.SerializeObject(model));
            var result = await Mediator.Send(new SSLCommerzValidatorCommand() { Model = model });

            if (result.HasError)
            {
                return BadRequest();
            }
             var paymentNo= result.Messages[1].ToString();
             var paymentFor= result.Messages[2].ToString();
            //  var redirectUrl = $"http://localhost:4200/#/subscription/payment-success/" + paymentNo;
            var redirectUrl = $"http://36.255.71.72:177/member_topup?paymentNo={paymentNo}&paymentFor={paymentFor}";
            // $"?paymentNo={paymentNo}";
            Response.Headers["Location"] = redirectUrl;
            return StatusCode(302);  // This forces a direct redirect
          //  return Redirect(redirectUrl);

           // return Ok(result);
        }


        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost]
        public async Task<IActionResult> IpnPayment()
        {
            SSLCommerzValidatorResponseReq model = new SSLCommerzValidatorResponseReq();

            model.APIConnect = Request.Form["APIConnect"];
            model.status = Request.Form["status"];
            model.sessionkey = Request.Form["sessionkey"];
            model.tran_date = DateTime.Parse(Request.Form["tran_date"]);
            model.tran_id = Request.Form["tran_id"];
            model.val_id = Request.Form["val_id"];
            model.amount = Convert.ToDecimal(Request.Form["amount"]);
            model.store_amount = Convert.ToDecimal(Request.Form["store_amount"]);
            model.card_type = Request.Form["card_type"];
            model.card_no = Request.Form["card_no"];
            model.currency = Request.Form["currency"];
            model.bank_tran_id = Request.Form["bank_tran_id"];
            model.card_issuer = Request.Form["card_issuer"];
            model.card_brand = Request.Form["card_brand"];
            model.card_issuer_country = Request.Form["card_issuer_country"];
            model.card_issuer_country_code = Request.Form["card_issuer_country_code"];
            model.currency_type = Request.Form["currency_type"];
            model.currency_amount = Convert.ToDecimal(Request.Form["currency_amount"]);
            model.emi_instalment = Convert.ToInt32(Request.Form["emi_instalment"]);
            model.emi_amount = Convert.ToDecimal(Request.Form["emi_amount"]);
            model.discount_percentage = Convert.ToDecimal(Request.Form["discount_percentage"]);
            model.discount_remarks = Request.Form["discount_remarks"];
            model.value_a = Request.Form["value_a"];
            model.value_b = Request.Form["value_b"];
            model.value_c = Request.Form["value_c"];
            model.value_d = Request.Form["value_d"];
            model.risk_level = Convert.ToInt32(Request.Form["risk_level"]);
            model.risk_title = Request.Form["risk_title"];
            Log.Information("TopUp IPN :" + model);
            var result = await Mediator.Send(new SSLCommerzIpnCommand() { Model = model });
            if (result.HasError)
            {
                return BadRequest();
            }

              var paymentNo = result.Messages[1].ToString();
              var paymentFor = result.Messages[2].ToString();
            // var redirectUrl = $"http://localhost:4200/#/subscription/payment-success/" + paymentNo;
            var redirectUrl = $"http://36.255.71.72:177/member_topup?paymentNo={paymentNo}&paymentFor={paymentFor}";

            // $"?paymentNo={paymentNo}";

            return Redirect(redirectUrl);
          //  return Ok(result);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost]
        public async Task<IActionResult> FailPayment()
        {
            SSLCommerzValidatorResponseReq model = new SSLCommerzValidatorResponseReq();

            model.APIConnect = Request.Form["APIConnect"];
            model.status = Request.Form["status"];
            model.sessionkey = Request.Form["sessionkey"];
            model.tran_date = DateTime.Parse(Request.Form["tran_date"]);
            model.tran_id = Request.Form["tran_id"];
            model.val_id = Request.Form["val_id"];
            model.amount = Convert.ToDecimal(Request.Form["amount"]);
            model.store_amount = Convert.ToDecimal(Request.Form["store_amount"]);
            model.card_type = Request.Form["card_type"];
            model.card_no = Request.Form["card_no"];
            model.currency = Request.Form["currency"];
            model.bank_tran_id = Request.Form["bank_tran_id"];
            model.card_issuer = Request.Form["card_issuer"];
            model.card_brand = Request.Form["card_brand"];
            model.card_issuer_country = Request.Form["card_issuer_country"];
            model.card_issuer_country_code = Request.Form["card_issuer_country_code"];
            model.currency_type = Request.Form["currency_type"];
            model.currency_amount = Convert.ToDecimal(Request.Form["currency_amount"]);
            model.emi_instalment = Convert.ToInt32(Request.Form["emi_instalment"]);
            model.emi_amount = Convert.ToDecimal(Request.Form["emi_amount"]);
            model.discount_percentage = Convert.ToDecimal(Request.Form["discount_percentage"]);
            model.discount_remarks = Request.Form["discount_remarks"];
            model.value_a = Request.Form["value_a"];
            model.value_b = Request.Form["value_b"];
            model.value_c = Request.Form["value_c"];
            model.value_d = Request.Form["value_d"];
            model.risk_level = Convert.ToInt32(Request.Form["risk_level"]);
            model.risk_title = Request.Form["risk_title"];
            Log.Information("TopUp Fail :" + JsonConvert.SerializeObject(model));
            var result = await Mediator.Send(new CreateOrderFailCommand() { Model = model });
            return Ok(result);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost]
        public async Task<IActionResult> CancelPayment()
        {
            SSLCommerzValidatorResponseReq model = new SSLCommerzValidatorResponseReq();

            model.APIConnect = Request.Form["APIConnect"];
            model.status = Request.Form["status"];
            model.sessionkey = Request.Form["sessionkey"];
            model.tran_date = DateTime.Parse(Request.Form["tran_date"]);
            model.tran_id = Request.Form["tran_id"];
            model.val_id = Request.Form["val_id"];
            model.amount = Convert.ToDecimal(Request.Form["amount"]);
            model.store_amount = Convert.ToDecimal(Request.Form["store_amount"]);
            model.card_type = Request.Form["card_type"];
            model.card_no = Request.Form["card_no"];
            model.currency = Request.Form["currency"];
            model.bank_tran_id = Request.Form["bank_tran_id"];
            model.card_issuer = Request.Form["card_issuer"];
            model.card_brand = Request.Form["card_brand"];
            model.card_issuer_country = Request.Form["card_issuer_country"];
            model.card_issuer_country_code = Request.Form["card_issuer_country_code"];
            model.currency_type = Request.Form["currency_type"];
            model.currency_amount = Convert.ToDecimal(Request.Form["currency_amount"]);
            model.emi_instalment = Convert.ToInt32(Request.Form["emi_instalment"]);
            model.emi_amount = Convert.ToDecimal(Request.Form["emi_amount"]);
            model.discount_percentage = Convert.ToDecimal(Request.Form["discount_percentage"]);
            model.discount_remarks = Request.Form["discount_remarks"];
            model.value_a = Request.Form["value_a"];
            model.value_b = Request.Form["value_b"];
            model.value_c = Request.Form["value_c"];
            model.value_d = Request.Form["value_d"];
            model.risk_level = Convert.ToInt32(Request.Form["risk_level"]);
            model.risk_title = Request.Form["risk_title"];

            Log.Information("TopUp Cancel : " + JsonConvert.SerializeObject(model));
            var result = await Mediator.Send(new CreateOrderCancelCommand() { Model = model });
            return Ok(result);
        }

       
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> PaymentInitiate(SSLCommerzInitRequest model)
        {
            Log.Information("TopUp Init :" + JsonConvert.SerializeObject(model));
            var result = await Mediator.Send(new PaymentInitiateCommand() { Model = model });
            return Ok(result);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateTopUp(TopUpReq model)
        {
            if (model == null)
            {
                return BadRequest();
            }
            var result = await Mediator.Send(new CreateTopUpCommand() { Model = model });
            return Ok(result);
        }
        [HttpGet]
        public async Task<IActionResult> HasTrxNo(string trxNo)
        {
            var result = await Mediator.Send(new HasTrxNoQuery()
            {
                TrxNo = trxNo
            });
            return Ok(result);
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll(int pageNo, int pageSize, string? memberShipNo, DateTime startDate, DateTime endDate)
        {
            var result = await Mediator.Send(new GetAllTopUpQuery()
            {
                PageNumber = pageNo,
                PageSize = pageSize,
                MemberShipNo = memberShipNo == "null" ? null : memberShipNo,
                StartDate = startDate,
                EndDate = endDate
            });
            return Ok(result);
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetByMemberShipNo(string? memberShipNo, string cardNo, int pageNo = 1, int pageSize = 1000)
        {
            var result = await Mediator.Send(new GetTopUpByMemberQuery()
            {
                MemberShipNo = memberShipNo,
                CardNo = cardNo,
                PageNo = pageNo,
                PageSize = pageSize
            });
            return Ok(result);
        }
        [HttpGet]
        public async Task<IActionResult> VerifyTopUp(int id)
        {
            var result = await Mediator.Send(new VerifyTopUpCommand() { MemberId = id });
            return Ok(result);
        }
        [HttpGet]
        public async Task<IActionResult> GetAlleById(int id)
        {
            var result = await Mediator.Send(new GetTopUpByIdQuery() { Id = id });
            return Ok(result);
        }
        //  [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetCurrentBalanceByMemberId(int memberId)
        {
            var result = await Mediator.Send(new GetCurrentBalabnceByMemberIdQuery() { MemberId = memberId });
            return Ok(result);
        }
        [HttpDelete]
        public async Task<IActionResult> Remove(int id)
        {
            var result = await Mediator.Send(new DeleteTopUpCommand() { Id = id });
            return Ok(result);
        }


        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetMemberLedger(string id, int pageNo, int pageSize)
        {
            var result = await Mediator.Send(new GetMemberLedgerCommand() { MemberId = id, PageNo = pageNo, PageSize = pageSize });
            return Ok(result);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetRoMemberLedger(string id, int pageNo, int pageSize)
        {
            var result = await Mediator.Send(new GetRoMemberLedgerCommand() { MemberId = id, PageNo = pageNo, PageSize = pageSize });
            return Ok(result);
        }

        [HttpGet]
        public async Task<FileResult> ExportTopUpReport([FromQuery] CommonCriteria model)
        {

            var dataTable = new DataTable();
            Dictionary<string, DataTable> data = new();
            var masterDataTable = new DataTable();

            masterDataTable.Columns.Add("SiteLogo");
            masterDataTable.Columns.Add("SiteTitle");
            masterDataTable.Columns.Add("ReportTitle");
            masterDataTable.Columns.Add("FormDate");
            masterDataTable.Columns.Add("ToDate");
            DataRow masterRow;
            masterRow = masterDataTable.NewRow();

            masterRow["SiteLogo"] = StaticData.ImageConvertToBase64(this._webHostEnvironment.WebRootPath + "\\Image\\sitelogo.png");


            masterRow["SiteTitle"] = StaticData.SiteTitle;
            masterRow["ReportTitle"] = "Top Up Details";
            masterRow["FormDate"] = model.FromDate?.ToString("dd-MMM-yyyy");
            masterRow["ToDate"] = model.ToDate?.ToString("dd-MMM-yyyy");


            masterDataTable.Rows.Add(masterRow);

            string extension = "pdf";
            var result = await Mediator.Send(new ExportTopUpQuery()
            {
                StartDate = model.FromDate?.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                EndDate = model.ToDate?.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                MemberShipNo = model.MembershipNo
            });

            DataTable dt = CustomExtensions.ConvertListToDataTable(result);

            data.Add("ds", dt);
            data.Add("master", masterDataTable);


            var path = $"{this._webHostEnvironment.WebRootPath}\\RDLC\\" + "TopUpDetail" + ".rdlc";
            ReportDomain reportDomain = new("PDF", data, path, null);
            return File(new ReportApplication().Load(reportDomain), reportDomain.mimeType, System.Guid.NewGuid().ToString() + "." + extension);
        }
        [HttpGet]
        public async Task<FileResult> ExportTopUpSummaryReport([FromQuery] CommonCriteria model)
        {
            
            Dictionary<string, DataTable> data = new();
            var masterDataTable = new DataTable();

            masterDataTable.Columns.Add("SiteLogo");
            masterDataTable.Columns.Add("SiteTitle");
            masterDataTable.Columns.Add("ReportTitle");
            masterDataTable.Columns.Add("FormDate");
            masterDataTable.Columns.Add("ToDate");
            DataRow masterRow;
            masterRow = masterDataTable.NewRow();

            masterRow["SiteLogo"] = StaticData.ImageConvertToBase64(this._webHostEnvironment.WebRootPath + "\\Image\\sitelogo.png");


            masterRow["SiteTitle"] = StaticData.SiteTitle;
            masterRow["ReportTitle"] = "Top Up Summary";
            masterRow["FormDate"] = model.FromDate?.ToString("dd-MMM-yyyy");
            masterRow["ToDate"] = model.ToDate?.ToString("dd-MMM-yyyy");


            masterDataTable.Rows.Add(masterRow);
            string extension = "pdf";
            var result = await Mediator.Send(new ExportTopUpQuery()
            {
                StartDate = model.FromDate?.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                EndDate = model.ToDate?.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                MemberShipNo = model.MembershipNo,
                UserName = model.UserName
            });

            DataTable dt = CustomExtensions.ConvertListToDataTable(result);

            data.Add("ds", dt);
            data.Add("master", masterDataTable);
            var path = $"{this._webHostEnvironment.WebRootPath}\\RDLC\\" + "TopUpSummary" + ".rdlc";
            //var path = $"{this._webHostEnvironment.WebRootPath}\\RDLC\\" + "TopUpDetail" + ".rdlc";
            ReportDomain reportDomain = new("PDF", data, path, null);
            return File(new ReportApplication().Load(reportDomain), reportDomain.mimeType, System.Guid.NewGuid().ToString() + "." + extension);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> TopUpsReport(int id)
        {
            var result = await Mediator.Send(new GetTopUpByIdQuery()
            {
                Id = id
            });
            var topUp = result.Data;

            string mimeType = "application/pdf";
            string extension = "pdf";

            var dataTable = new DataTable();

            dataTable.Columns.Add("MemberShipNo");
            dataTable.Columns.Add("TOPUPID");
            dataTable.Columns.Add("TopUpDate");
            dataTable.Columns.Add("TotalAmount");
            dataTable.Columns.Add("Amount");
            dataTable.Columns.Add("MemberName");
            dataTable.Columns.Add("MemberCardNo");
            dataTable.Columns.Add("CurrentBalance");
            dataTable.Columns.Add("CreateByName");
            dataTable.Columns.Add("PaymentMethodText");


            DataRow row;

            foreach (var item in topUp.TopUpDetails)
            {
                row = dataTable.NewRow();
                row["MemberShipNo"] = topUp.MemberShipNo;
                row["MemberName"] = topUp.MemberName;
                row["MemberCardNo"] = topUp.MemberCardNo;
                row["CurrentBalance"] = topUp.CurrentBalance;
                row["CreateByName"] = topUp.CreateByName;
                row["TOPUPID"] = item.TOPUPNO;
                row["TopUpDate"] = topUp.TopUpDate;
                row["TotalAmount"] = topUp.TotalAmount;
                row["Amount"] = item.Amount;
                row["PaymentMethodText"] = item.PaymentMethodText;


                dataTable.Rows.Add(row);
            }

            Dictionary<string, DataTable> data = new();

            data.Add("ds", dataTable);
            var path = $"{this._webHostEnvironment.WebRootPath}\\RDLC\\" + "TopUpReport" + ".rdlc";

            ReportDomain reportDomain = new("PDF", data, path, null);
            return File(new ReportApplication().Load(reportDomain), reportDomain.mimeType, System.Guid.NewGuid().ToString() + "." + "PDF");


        }
    }
}
