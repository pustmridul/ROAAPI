using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MemApp.Reporting;
using System.Data;
using ResApp.Application.ROA.MembershipFee.Models;
using ResApp.Application.ROA.MembershipFee.Command;
using ResApp.Application.ROA.MembershipFee.Queries;

namespace Res.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class RoMembershipFeeController : ApiNewControllerBase
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        public RoMembershipFeeController(ISender mediator,  IWebHostEnvironment webHostEnvironment) : base(mediator)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Payment(MembershipFeePayReq model)
        {
            if (model == null)
            {
                return BadRequest();
            }
            var result = await Mediator.Send(new RoMembershipFeePaymentCommand() { Model = model });
            return Ok(result);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetPaidByMemberId(int MemberId)
        {

            var result = await Mediator.Send(new GetPaidByROMemberIdQuery() { MemberId = MemberId });
            return Ok(result);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ExportMembershipPaymentReport(string payemntNo)
        {
            try
            {
                var result = await Mediator.Send(new ExportMembershipFeeReportQuery() 
                { 
                    PaymentNo = payemntNo 
                });

                string mimeType = "application/pdf";
                string extension = "pdf";

                var masterDataTable = new DataTable();
                masterDataTable.Columns.Add("MemberName");
                masterDataTable.Columns.Add("PaymentDate");
                masterDataTable.Columns.Add("MembershipNo");
                masterDataTable.Columns.Add("Amount");           

                DataRow masterRow;
                masterRow = masterDataTable.NewRow();

                // masterRow["SiteLogo"] = StaticData.ImageConvertToBase64(this._webHostEnvironment.WebRootPath + "\\Image\\sitelogo.png");


                masterRow["MemberName"] = result.Data.MemberName;
                masterRow["PaymentDate"] = result.Data.PaymentDate.ToString("MMMM dd, yyyy");
                masterRow["MembershipNo"] = result.Data.MembershipNo;
                masterRow["Amount"] = result.Data.Amount;
               


                masterDataTable.Rows.Add(masterRow);

               


                
                Dictionary<string, DataTable> data = new()
                {              
                    { "master", masterDataTable }
                };
                var path = $"{this._webHostEnvironment.WebRootPath}\\RDLC\\" + "RoMembershipFeePaymentReport" + ".rdlc";

                ReportDomain reportDomain = new("PDF", data, path, null!);
                return File(new ReportApplication().Load(reportDomain), reportDomain.mimeType, System.Guid.NewGuid().ToString() + "." + "PDF");

                //localReport.DataSources.Add(new ReportDataSource("ds", dataTable));
                //localReport.ReportPath = $"{this._webHostEnvironment.WebRootPath}\\RDLC\\" + "SubscriptionPaymentReport" + ".rdlc";

                //var pdf = localReport.Render("PDF");
                //var file = File(pdf, mimeType, "report." + extension);
                //return file;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
    }
}
