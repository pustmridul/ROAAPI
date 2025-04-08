using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Mem.Reports.AppDownloadReport.Query;
using MemApp.Reporting;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace MemApp.WebApi.Controllers.Reports
{
    public class AppDownloadController : ApiControllerBase
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AppDownloadController(IWebHostEnvironment webHostEnvironment, ISender sender) : base(sender)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        [HttpGet]
        public async Task<IActionResult> GetAppDownloadReport(bool? IsActive)
        {
            try
            {

                var result = await Mediator.Send(new AppDownloadReportQuery() { IsActive = IsActive });
                Dictionary<string, DataTable> data = new();

                if (result != null)
                {
                    DataTable dt = CustomExtensions.ConvertListToDataTable(result);
                    data.Add("ds", dt);
                }

                var path = $"{this._webHostEnvironment.WebRootPath}\\RDLC\\" + "AppDownloadReport" + ".rdlc";

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
