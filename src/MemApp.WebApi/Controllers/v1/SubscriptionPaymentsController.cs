using AspNetCore.Reporting;
using MediatR;
using MemApp.Application;
using MemApp.Application.Extensions;
using MemApp.Application.Mem.Payment.Command;
using MemApp.Application.Mem.Payment.Model;
using MemApp.Application.Mem.Subscription.Command;
using MemApp.Application.Mem.Subscription.Model;
using MemApp.Application.Mem.Subscription.Queries;
using MemApp.Reporting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace MemApp.WebApi.Controllers.v1
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SubscriptionPaymentsController : ApiControllerBase
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        public SubscriptionPaymentsController(IWebHostEnvironment webHostEnvironment, ISender sender) : base(sender)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        [HttpGet]
        public async Task<IActionResult> SubscriptionPaidUpTo(int id)
        {
            var result = await Mediator.Send(new GetSubscriptionUpToQuery() { Id = id });
            return Ok(result);
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> DueList(DateTime syncDate)
        {
            var result = await Mediator.Send(new GetDueListQuery() { SyncDate = syncDate });
            return Ok(result);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> DueGenerate(DateTime syncDate)
        {
            var result = await Mediator.Send(new SubscriptionDueGenerateCommand() { SyncDate = syncDate });

            return Ok(result);
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> SavePayment(SubscriptionPaymentReq model)
        {
            if (model == null)
            {
                return BadRequest();
            }
            var result = await Mediator.Send(new CreateSubscriptionPaymentCommand { Model = model });
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> AutoSubscription(string subscriptionYear, string quaterNo)
        {
            var result = await Mediator.Send(new AutoSubscriptionPaymentCommand()
            {
                SubscriptionYear = subscriptionYear,
                QuaterNo = quaterNo
            });
            return Ok(result);
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> DueListByMemberId(int id)
        {
            var result = await Mediator.Send(new GetDueListByMemberIdQuery() { Id = id });
            return Ok(result);
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> PaidListByMemberId(int id)
        {
            var result = await Mediator.Send(new GetPaidListByMemberIdQuery() { Id = id });
            return Ok(result);
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> AdvancedListByMemberId(int id)
        {
            var result = await Mediator.Send(new GetAdvancedListQuery() { Id = id });
            return Ok(result);
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> SaveDuePayment(SubscriptionFeeModel model)
        {
            if (model == null)
            {
                return BadRequest();
            }
            var result = await Mediator.Send(new SaveSubDuePaymentCommand() { Model = model });
            return Ok(result);
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> SaveAdvancedPayment(SubscriptionFeeModel model)
        {
            if (model == null)
            {
                return BadRequest();
            }
            var result = await Mediator.Send(new SaveSubAdvancedPaymentCommand() { Model = model });
            return Ok(result);
        }

        [HttpGet]
        public async Task<FileResult> ExportSubscriptionDueList(string reportType = "PDF", string reportName = "SubscriptionDue")
        {
            IActionResult response = Unauthorized();
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
            masterRow["ReportTitle"] = "Subscription Due List";
            masterRow["FormDate"] ="";
            masterRow["ToDate"] = "";


            masterDataTable.Rows.Add(masterRow);

            var result = await Mediator.Send(new ExportSubscriptionDueListQuery()
            {
                reportName = reportName,
                reportType = reportType,
            });

            if (result != null)
            {
                DataTable dt = CustomExtensions.ConvertListToDataTable(result);
                data.Add("ds", dt);
                data.Add("master", masterDataTable);
            }

            var path = $"{this._webHostEnvironment.WebRootPath}\\RDLC\\" + reportName + ".rdlc";
            ReportDomain reportDomain = new(reportType, data, path, null);

            return File(new ReportApplication().Load(reportDomain), reportDomain.mimeType, System.Guid.NewGuid().ToString() + "." + reportType);

        }

        [HttpGet]
        public async Task<IActionResult> ExportSubscriptionPaymentDetailReport([FromQuery] SubscriptionPaymentReportReq model)
        {
            try
            {
                var result = await Mediator.Send(new ExportSubscriptionPaymentListQuery() { Model = model });
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
                masterRow["ReportTitle"] = "Subscription Payment (Detail)";
                masterRow["FormDate"] = model.FromDate?.ToString("dd-MMM-yyyy");
                masterRow["ToDate"] = model.ToDate?.ToString("dd-MMM-yyyy");


                masterDataTable.Rows.Add(masterRow);
                if (result != null)
                {
                    DataTable dt = CustomExtensions.ConvertListToDataTable(result);
                    data.Add("ds", dt);
                    
                }
                data.Add("master", masterDataTable);
                var path = $"{this._webHostEnvironment.WebRootPath}\\RDLC\\" + "SubscriptionPayment" + ".rdlc";

                ReportDomain reportDomain = new("PDF", data, path, null);
                return File(new ReportApplication().Load(reportDomain), reportDomain.mimeType, System.Guid.NewGuid().ToString() + "." + "PDF");

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        [HttpGet]
        public async Task<IActionResult> ExportSubscriptionPaymentSummaryReport([FromQuery] SubscriptionPaymentReportReq model)
        {
            try
            {
                var result = await Mediator.Send(new SubscriptionPymentReportQuery() { Model = model });
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
                masterRow["ReportTitle"] = "Subscription Payment (Summary)";
                masterRow["FormDate"] = model.FromDate?.ToString("dd-MMM-yyyy");
                masterRow["ToDate"] = model.ToDate?.ToString("dd-MMM-yyyy");


                masterDataTable.Rows.Add(masterRow);
                if (result != null)
                {
                    DataTable dt = CustomExtensions.ConvertListToDataTable(result);
                    data.Add("ds", dt);
                   
                }
                data.Add("master", masterDataTable);
                var path = $"{this._webHostEnvironment.WebRootPath}\\RDLC\\" + "SubscriptionPaymentSummary" + ".rdlc";

                ReportDomain reportDomain = new("PDF", data, path, null);
                return File(new ReportApplication().Load(reportDomain), reportDomain.mimeType, System.Guid.NewGuid().ToString() + "." + "PDF");

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        [HttpGet]
        public async Task<IActionResult> ExportSubscriptionDueDetailReport([FromQuery] SubscriptionPaymentReportReq model)
        {
            try
            {
                var result = await Mediator.Send(new SubscriptionDueDetailReportQuery() { Model = model });
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
                masterRow["ReportTitle"] = "Subscription Due (Detail)";
                masterRow["FormDate"] = model.FromDate?.ToString("dd-MMM-yyyy");
                masterRow["ToDate"] = model.ToDate?.ToString("dd-MMM-yyyy");


                masterDataTable.Rows.Add(masterRow);


                if (result != null)
                {
                    DataTable dt = CustomExtensions.ConvertListToDataTable(result);
                    data.Add("ds", dt);
                }
                data.Add("master", masterDataTable);
                var path = $"{this._webHostEnvironment.WebRootPath}\\RDLC\\" + "SubscriptionDueDetail" + ".rdlc";

                ReportDomain reportDomain = new("PDF", data, path, null);
                return File(new ReportApplication().Load(reportDomain), reportDomain.mimeType, System.Guid.NewGuid().ToString() + "." + "PDF");

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        [HttpGet]
        public async Task<IActionResult> ExportSubscriptionDueSummaryReport([FromQuery] SubscriptionPaymentReportReq model)
        {
            try
            {
                var result = await Mediator.Send(new SubscriptionDueSummaryReportQuery() { Model = model });
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
                masterRow["ReportTitle"] = "Subscription Due (Summary)";
                masterRow["FormDate"] = model.FromDate?.ToString("dd-MMM-yyyy");
                masterRow["ToDate"] = model.ToDate?.ToString("dd-MMM-yyyy");


                masterDataTable.Rows.Add(masterRow);
                if (result != null)
                {
                    DataTable dt = CustomExtensions.ConvertListToDataTable(result);
                    data.Add("ds", dt);
                }
                data.Add("master", masterDataTable);
                var path = $"{this._webHostEnvironment.WebRootPath}\\RDLC\\" + "SubscriptionDueSummary" + ".rdlc";

                ReportDomain reportDomain = new("PDF", data, path, null);
                return File(new ReportApplication().Load(reportDomain), reportDomain.mimeType, System.Guid.NewGuid().ToString() + "." + "PDF");

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        [HttpGet]
        public async Task<IActionResult> GetAllSubscriptionPayment(DateTime startDate, DateTime endDate, string? memberShipNo, int pageNo, int pageSize)
        {
            var result = await Mediator.Send(new GetSubscriptionPaymentListQuery()
            {
                StartDate = startDate,
                EndDate = endDate,
                MemberShipNo = memberShipNo == "null" ? null : memberShipNo,
                PageNo = pageNo,
                PageSize = pageSize
            });
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetSubscriptionPayment(string payemntNo)
        {
            var result = await Mediator.Send(new GetSubscriptionPaymentQuery() { PaymentNo = payemntNo });

            return Ok(result);
        }
        [HttpGet]
        public async Task<IActionResult> GetSubscriptionPaymentReport(string payemntNo)
        {
            try
            {
                var result = await Mediator.Send(new GetSubscriptionPaymentQuery() { PaymentNo = payemntNo });

                string mimeType = "application/pdf";
                string extension = "pdf";

                var dataTable = new DataTable();

                dataTable.Columns.Add("ActualPaymentDate");
                dataTable.Columns.Add("TotalPaymentAmount");
                dataTable.Columns.Add("PaymentDate");
                dataTable.Columns.Add("PaymentYear");
                dataTable.Columns.Add("SubscriptionName");
                dataTable.Columns.Add("MemberShipNo");
                dataTable.Columns.Add("MemberName");
                dataTable.Columns.Add("AbroadFeeAmount");
                dataTable.Columns.Add("PaymentAmount");
                dataTable.Columns.Add("LateAmount");

                DataRow row;


                foreach (var item in result.Data.ToList())
                {
                    row = dataTable.NewRow();
                    row["ActualPaymentDate"] = item.ActualPaymentDate?.ToString("yyyy-MM-dd");
                    row["TotalPaymentAmount"] = Math.Round(item.TotalPaymentAmount, 2);
                    row["PaymentDate"] = item.PaymentDate.ToString("yyyy-MM-dd");
                    row["PaymentYear"] = item.PaymentYear;
                    row["SubscriptionName"] = item.SubscriptionName;
                    row["MemberShipNo"] = item.MemberShipNo;
                    row["MemberName"] = item.MemberName;
                    row["AbroadFeeAmount"] = item.AbroadFeeAmount;
                    row["PaymentAmount"] = Math.Round(item.PaymentAmount, 2);
                    row["LateAmount"] = Math.Round(item.LateAmount, 2);


                    dataTable.Rows.Add(row);
                }
               
                Dictionary<string, DataTable> data = new();

                data.Add("ds", dataTable);
               // data.Add("master", masterDataTable);
                var path = $"{this._webHostEnvironment.WebRootPath}\\RDLC\\" + "SubscriptionPaymentReport" + ".rdlc";

                ReportDomain reportDomain = new("PDF", data, path, null);
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


        [HttpGet]
        public async Task<IActionResult> ExportUserWiseSubscriptionCollectionReport([FromQuery] SubscriptionPaymentReportReq model)
        {
            try
            {
                var result = await Mediator.Send(new ExportUserWiseSubscriptionCollectionQuery() { Model = model });
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
                masterRow["ReportTitle"] = "User Wise Subscription Collection";
                masterRow["FormDate"] = model.FromDate?.ToString("dd-MMM-yyyy");
                masterRow["ToDate"] = model.ToDate?.ToString("dd-MMM-yyyy");


                masterDataTable.Rows.Add(masterRow);
                if (result != null)
                {
                    DataTable dt = CustomExtensions.ConvertListToDataTable(result);
                    data.Add("ds", dt);
                }
                data.Add("master", masterDataTable);
                var path = $"{this._webHostEnvironment.WebRootPath}\\RDLC\\" + "UserWiseSubscriptionCollectionReport" + ".rdlc";

                ReportDomain reportDomain = new("PDF", data, path, null);
                return File(new ReportApplication().Load(reportDomain), reportDomain.mimeType, System.Guid.NewGuid().ToString() + "." + "PDF");

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        [HttpGet]
        public async Task<IActionResult> ExportUserWiseSubscriptionCollectionReportdetails([FromQuery] SubscriptionPaymentReportReq model)
        {
            try
            {
                var result = await Mediator.Send(new ExportUserWiseSubscriptionCollectionDetailsQuery() { Model = model });
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
                masterRow["ReportTitle"] = "User Wise Subscription Collection (Detail)";
                masterRow["FormDate"] = model.FromDate?.ToString("dd-MMM-yyyy");
                masterRow["ToDate"] = model.ToDate?.ToString("dd-MMM-yyyy");


                masterDataTable.Rows.Add(masterRow);
                if (result != null)
                {
                    DataTable dt = CustomExtensions.ConvertListToDataTable(result);
                    data.Add("ds", dt);
                }
                data.Add("master", masterDataTable);
                var path = $"{this._webHostEnvironment.WebRootPath}\\RDLC\\" + "UserWiseSubscriptionCollectionReportDetails" + ".rdlc";

                ReportDomain reportDomain = new("PDF", data, path, null);
                return File(new ReportApplication().Load(reportDomain), reportDomain.mimeType, System.Guid.NewGuid().ToString() + "." + "PDF");

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }


    }
}
