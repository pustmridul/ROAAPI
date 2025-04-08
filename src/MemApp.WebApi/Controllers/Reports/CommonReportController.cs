using MediatR;
using MemApp.Application;
using MemApp.Application.Mem.Reports.CommonReport.Query;
using MemApp.Reporting;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace MemApp.WebApi.Controllers.Reports
{
    [Route("api/[controller]/[action]")]
    public class CommonReportController : ApiControllerBase
    {


        private readonly IWebHostEnvironment _webHostEnvironment;

        public CommonReportController(IWebHostEnvironment webHostEnvironment, ISender sender) : base(sender)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllSalesReportData([FromQuery] GetAllSalesQuery query)
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
                masterRow["FormDate"] = query.StartDate.ToString("dd-MMM-yyyy");
                masterRow["ToDate"] = query.EndDate.ToString("dd-MMM-yyyy");



                masterDataTable.Rows.Add(masterRow);
                var dataset = await Mediator.Send(query);

                var dataTable = new DataTable();

                dataTable.Columns.Add("MemberShipNo");
                dataTable.Columns.Add("MemberName");

                dataTable.Columns.Add("TransactionType");
                dataTable.Columns.Add("Amount");

                DataRow row;

                foreach (var item in dataset)
                {
                    row = dataTable.NewRow();
                    row["MemberName"] = item.MemberName;
                    row["MemberShipNo"] = item.MembershipNo;

                    row["TransactionType"] = item.TransactionType;
                    row["Amount"] = item.Amount;

                    dataTable.Rows.Add(row);
                }



                data.Add("master", masterDataTable);
                data.Add("ds", dataTable);


                var path = $"{this._webHostEnvironment.WebRootPath}\\RDLC\\" + "AllSalesReport" + ".rdlc";
                ReportDomain reportDomain = new("PDF", data, path, null);
                return File(new ReportApplication().Load(reportDomain), reportDomain.mimeType, System.Guid.NewGuid().ToString() + "." + "PDF");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
    }
}
