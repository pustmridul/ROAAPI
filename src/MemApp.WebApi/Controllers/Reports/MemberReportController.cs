using MediatR;
using MemApp.Application;
using MemApp.Application.Extensions;
using MemApp.Application.Mem.Reports.Common;
using MemApp.Application.Mem.Reports.MemberReport.Query;
using MemApp.Reporting;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Globalization;

namespace MemApp.WebApi.Controllers.Reports
{
    public class MemberReportController : ApiControllerBase
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public MemberReportController(IWebHostEnvironment webHostEnvironment, ISender sender) : base(sender)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        [HttpGet]
        public async Task<IActionResult> GetMemberLedgerDetail([FromQuery] CommonCriteria model)
        {
            try
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
                masterRow["ReportTitle"] = "Member Ledger Report (Detail)";
                masterRow["FormDate"] = model.FromDate?.ToString("dd-MMM-yyyy");
                masterRow["ToDate"] = model.ToDate?.ToString("dd-MMM-yyyy");



                masterDataTable.Rows.Add(masterRow);

                var result = await Mediator.Send(new MemberLedgerDetailReportQuery() { Model = model });

                string mimeType = "application/pdf";
                string extension = "pdf";

                var dataTable = new DataTable();

                dataTable.Columns.Add("MemberName");
                dataTable.Columns.Add("MemberShipNo");
                dataTable.Columns.Add("Phone");
                dataTable.Columns.Add("CardNo");
                dataTable.Columns.Add("Dates");
                dataTable.Columns.Add("Description");
                dataTable.Columns.Add("LedgerType");
                dataTable.Columns.Add("PayType");
                dataTable.Columns.Add("BankCreditCardName");
                dataTable.Columns.Add("Amount");
                dataTable.Columns.Add("OpeningBalance");


                DataRow row;
                foreach (var item in result)
                {
                    row = dataTable.NewRow();
                    row["MemberName"] = item.MemberName;
                    row["MemberShipNo"] = item.MembershipNo;
                    row["Phone"] = item.Phone;
                    row["CardNo"] = item.CardNo;
                    row["Dates"] = item.Dates?.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture);
                    row["Description"] = item.Description;
                    row["LedgerType"] = item.LedgerType;

                    row["PayType"] = item.PayType;
                    row["BankCreditCardName"] = item.BankCreditCardName;
                    row["Amount"] = item.Amount;
                    row["OpeningBalance"] = item.OpeningBalance;


                    dataTable.Rows.Add(row);
                }

                data.Add("master", masterDataTable);
                data.Add("MemberLedgerDataSet", dataTable);

                var path = $"{this._webHostEnvironment.WebRootPath}\\RDLC\\" + "MemberLedgerDetailReport" + ".rdlc";

                ReportDomain reportDomain = new("PDF", data, path, null);
                return File(new ReportApplication().Load(reportDomain), reportDomain.mimeType, System.Guid.NewGuid().ToString() + "." + "PDF");


            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        [HttpGet]
        public async Task<IActionResult> GetMemberLedgerSummary([FromQuery] CommonCriteria model)
        {
            try
            {
                Dictionary<string, DataTable> data = new();
                var result = await Mediator.Send(new MemberLedgerSummaryReportQuery() { Model = model });

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
                masterRow["ReportTitle"] = "Member Ledger Report (Summary)";
                masterRow["FormDate"] = model.FromDate?.ToString("dd-MMM-yyyy");
                masterRow["ToDate"] = model.ToDate?.ToString("dd-MMM-yyyy");


                masterDataTable.Rows.Add(masterRow);

                if (result != null)
                {
                    DataTable dt = CustomExtensions.ConvertListToDataTable(result);
                    data.Add("ds", dt);
                }
                data.Add("master", masterDataTable);

                var path = $"{this._webHostEnvironment.WebRootPath}\\RDLC\\" + "MemberLedgerSummaryReport" + ".rdlc";

                ReportDomain reportDomain = new("PDF", data, path, null);
                return File(new ReportApplication().Load(reportDomain), reportDomain.mimeType, System.Guid.NewGuid().ToString() + "." + "PDF");

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }



        [HttpGet]
        public async Task<IActionResult> GetMemberAttendance([FromQuery] CommonCriteria model)
        {
            try
            {

                string mimeType = "application/pdf";
                string extension = "pdf";

                var dataTable = new DataTable();
                Dictionary<string, DataTable> data = new();
                var result = await Mediator.Send(new MemberAttendanceReportQuery() { Model = model });

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
                masterRow["ReportTitle"] = "Member Attendance Report";
                masterRow["FormDate"] = model.FromDate?.ToString("dd-MMM-yyyy");
                masterRow["ToDate"] = model.ToDate?.ToString("dd-MMM-yyyy");


                masterDataTable.Rows.Add(masterRow);



                dataTable.Columns.Add("MemberName");
                dataTable.Columns.Add("MemberShipNo");
                dataTable.Columns.Add("InTime");



                DataRow row;
                foreach (var item in result)
                {
                    row = dataTable.NewRow();
                    row["MemberName"] = item.Name;
                    row["MemberShipNo"] = item.MembershipNo;
                    row["InTime"] = item.InTime;


                    dataTable.Rows.Add(row);
                }



                data.Add("ds", dataTable);
                data.Add("master", masterDataTable);

                var path = $"{this._webHostEnvironment.WebRootPath}\\RDLC\\" + "MemberAttendaceReport" + ".rdlc";

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
