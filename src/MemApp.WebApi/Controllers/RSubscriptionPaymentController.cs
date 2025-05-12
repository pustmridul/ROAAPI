using MediatR;
using MemApp.Reporting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResApp.Application.ROA.RoaSubcription.Queries;
using ResApp.Application.Com.Queries.ROSubcriptionPaymentReport;
using System.Data;
using ResApp.Application.ROA.RoaSubcription.Command;
using ResApp.Application.ROA.SubscriptionPayment.Models;
using ResApp.Application.ROA.SubscriptionPayment.Queries;
using ResApp.Application.ROA.SubscriptionPayment.Command;

namespace Res.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class RSubscriptionPaymentController : ApiNewControllerBase
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        public RSubscriptionPaymentController(IWebHostEnvironment webHostEnvironment, ISender sender) : base(sender)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> DueGenerate(DateTime syncDate)
        {
            var result = await Mediator.Send(new RSubscriptionMonthDueGenerateCommand() { SyncDate = syncDate });

            return Ok(result);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> SaveDuePayment(SubscriptionSubDuePayReq model)
        {
            if (model == null)
            {
                return BadRequest();
            }
            var result = await Mediator.Send(new ROSaveSubDuePaymentCommand() { Model = model });
            return Ok(result);
        }



        [Authorize]
        [HttpPost]
        public async Task<IActionResult> SaveAdvancePayment(SubscriptionSubDuePayReq model )
        {
            if (model == null)
            {
                return BadRequest();
            }
            var result = await Mediator.Send(new ROSaveSubAdvancePaymentCommand() { Model = model });
            return Ok(result);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetDueListByMemberId(int MemberId)
        {
            
            var result = await Mediator.Send(new GetDueListByROMemberIdQuery() { MemberId = MemberId });
            return Ok(result);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAdvancePayListByMemberId(int MemberId)
        {

            var result = await Mediator.Send(new GetAdvanceListByROMemberIdQuery() { MemberId = MemberId });
            return Ok(result);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetPaidListByMemberId(int MemberId)
        {

            var result = await Mediator.Send(new GetPaidListByROMemberIdQuery() { MemberId = MemberId });
            return Ok(result);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetSubscriptionPaymentReport(string payemntNo)
        {
            try
            {
                var result = await Mediator.Send(new GetRoSubscriptionPaymentQuery() { PaymentNo = payemntNo });

                //string mimeType = "application/pdf";
                //string extension = "pdf";

                var masterDataTable = new DataTable();
                masterDataTable.Columns.Add("MemberName");
                masterDataTable.Columns.Add("PaymentDate");
                masterDataTable.Columns.Add("MemberShipNo");
                masterDataTable.Columns.Add("TotalPaymentAmount");
                masterDataTable.Columns.Add("SubscriptionDetails");
               
                DataRow masterRow;
                masterRow = masterDataTable.NewRow();

               // masterRow["SiteLogo"] = StaticData.ImageConvertToBase64(this._webHostEnvironment.WebRootPath + "\\Image\\sitelogo.png");


                masterRow["MemberName"] = result.Data.MemberName;
                masterRow["PaymentDate"] = result.Data.PaymentDate.ToString("MMMM dd, yyyy");
                masterRow["MemberShipNo"] = result.Data.MemberShipNo;
                masterRow["TotalPaymentAmount"] = result.Data.TotalPaymentAmount;
                masterRow["SubscriptionDetails"] = result.Data.SubscriptionDetails;
               


                masterDataTable.Rows.Add(masterRow);

                var dataTable = new DataTable();

                dataTable.Columns.Add("SubscriptionYear");
                dataTable.Columns.Add("SubscriptionMonth");
                dataTable.Columns.Add("SubscriptionName");                 
                dataTable.Columns.Add("PaymentAmount");
                dataTable.Columns.Add("LateAmount");
               

                DataRow row;


                foreach (var item in result.Data.PaymentDetails.ToList())
                {
                    row = dataTable.NewRow();
                    row["SubscriptionYear"] = item.SubscriptionYear.ToString();
                    row["SubscriptionMonth"] = item.SubscriptionMonth.ToString("MMMM");
                    row["SubscriptionName"] = item.SubscriptionName;                 
                                 
                  
                    row["PaymentAmount"] = Math.Round(item.PaymentAmount, 2);
                    row["LateAmount"] = Math.Round(item.LateAmount, 2);
                   
                    dataTable.Rows.Add(row);
                }

                Dictionary<string, DataTable> data = new()
                {
                    { "ds", dataTable },
                    { "master", masterDataTable }
                };
                var path = $"{this._webHostEnvironment.WebRootPath}\\RDLC\\" + "RoSubscriptionPaymentReport" + ".rdlc";

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

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllSubscriptionPayment(DateTime startDate, DateTime endDate, string? memberShipNo, int pageNo, int pageSize)
        {
            var result = await Mediator.Send(new GetRoSubscriptionPaymentListQuery()
            {
                StartDate = startDate,
                EndDate = endDate,
                MemberShipNo = memberShipNo == "null" ? null : memberShipNo,
                PageNo = pageNo,
                PageSize = pageSize
            });
            return Ok(result);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetSubscriptionPayment(string payemntNo)
        {
            var result = await Mediator.Send(new GetRoSubscriptionPaymentDetailsQuery() { PaymentNo = payemntNo });

            return Ok(result);
        }
    }
}
