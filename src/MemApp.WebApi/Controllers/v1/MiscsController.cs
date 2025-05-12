using MediatR;
using MemApp.Application.com.Queries.GetAllEmergencyInfo;
using MemApp.Application.com.Queries.GetAllMessageTemplate;
using MemApp.Application.Com.Commands.UserConferences;
using MemApp.Application.Com.Models;
using MemApp.Application.Interfaces;
using MemApp.Application.Mem.MiscItems.Command;
using MemApp.Application.Mem.MiscItems.Models;
using MemApp.Application.Mem.MiscItems.Queries;
using MemApp.Application.Mem.MiscSales.Command;
using MemApp.Application.Mem.MiscSales.Models;
using MemApp.Application.Mem.MiscSales.Queries;
using MemApp.Reporting;
using MemApp.WebApi.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Globalization;

namespace MemApp.Controllers.v1.com
{

    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MiscsController : ApiControllerBase
    {
        private INotificationService _notificationService;

        private readonly IWebHostEnvironment _webHostEnvironment;
        public MiscsController(IWebHostEnvironment webHostEnvironment, INotificationService notificationService, ISender sender) : base(sender)
        {
            _notificationService = notificationService;
            _webHostEnvironment = webHostEnvironment;
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> SaveMiscItem(MiscItemReq model)
        {
            if (model == null)
            {
                return BadRequest();
            }
            var result = await Mediator.Send(new CreateMiscItemCommand { Model = model });
            return Ok(result);
        }
        [HttpGet]
        public async Task<IActionResult> GetAllMiscItemById(int id)
        {
            var result = await Mediator.Send(new GetMiscItemByIdQuery() { Id = id });
            return Ok(result);
        }
        [HttpGet]
        public async Task<IActionResult> GetAllMiscItem(int pageNo = 1, int pageSize = 1000)
        {
            var result = await Mediator.Send(new GetAllMiscItemQuery()
            {
                PageNumber = pageNo,
                PageSize = pageSize
            });
            return Ok(result);
        }
        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> RemoveMiscItem(int id)
        {
            var result = await Mediator.Send(new DeleteMiscItemCommand() { Id = id });
            return Ok(result);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> SaveMiscSale(MiscSaleReq model)
        {
            if (model == null)
            {
                return BadRequest();
            }
            var result = await Mediator.Send(new CreateMiscSaleCommand { Model = model });
            return Ok(result);
        }
        [HttpGet]
        public async Task<IActionResult> GetAllMiscSaleById(int id)
        {
            var result = await Mediator.Send(new GetMiscSaleByIdQuery() { Id = id });
            return Ok(result);
        }
        [HttpGet]
        public async Task<IActionResult> GetAllMiscSale([FromQuery] MiscSaleSearchModel model)
        {
            var result = await Mediator.Send(new GetAllMiscSaleQuery()
            {
                SearchModel = model
            });
            return Ok(result);
        }
        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> RemoveMiscSale(int id)
        {
            var result = await Mediator.Send(new DeleteMiscSaleCommand() { Id = id });
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetMiscSaleReportById(int id)
        {
            try
            {
                var result = await Mediator.Send(new GetMiscSaleByIdQuery() { Id = id });


                //string mimeType = "application/pdf";
                //string extension = "pdf";

                var masterDataTable = new DataTable();
                masterDataTable.Columns.Add("InvoiceNo");
                masterDataTable.Columns.Add("InvoiceDate");
                masterDataTable.Columns.Add("MemberText");
                masterDataTable.Columns.Add("MemberShipNo");

                masterDataTable.Columns.Add("Note");


                DataRow masterRow;
                masterRow = masterDataTable.NewRow();
                masterRow["InvoiceNo"] = result.Data.InvoiceNo;
                masterRow["InvoiceDate"] = result.Data.InvoiceDate.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture);
                masterRow["MemberText"] = result.Data.MemberText;
                masterRow["MemberShipNo"] = result.Data.MemberShipNo;

                masterRow["Note"] = result.Data.Note;

                masterDataTable.Rows.Add(masterRow);

                var detailDataTable = new DataTable();
                detailDataTable.Columns.Add("ItemText");
                detailDataTable.Columns.Add("Quantity");
                detailDataTable.Columns.Add("UnitPrice");
                detailDataTable.Columns.Add("LineTotal");

                DataRow detailRow;

                foreach (var item in result.Data.MiscSaleDetailReqs)
                {
                    detailRow = detailDataTable.NewRow();

                    detailRow["ItemText"] = item.ItemText;
                    detailRow["Quantity"] = item.Quantity;
                    detailRow["UnitPrice"] = item.UnitPrice.ToString("F2");
                    detailRow["LineTotal"] = (item.Quantity * item.UnitPrice).ToString("F2");
                    detailDataTable.Rows.Add(detailRow);
                }
                Dictionary<string, DataTable> data = new();

                data.Add("masterDataSet", masterDataTable);
                data.Add("detailDataSet", detailDataTable);

                var path = $"{this._webHostEnvironment.WebRootPath}\\RDLC\\" + "MiscSaleReport" + ".rdlc";

                ReportDomain reportDomain = new("PDF", data, path, null);
                return File(new ReportApplication().Load(reportDomain), reportDomain.mimeType, System.Guid.NewGuid().ToString() + "." + "PDF");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }


            
        }

        #region Emergency info
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> SaveEmergencyInfo(EmergencyInfoReq model)
        {
            if (model == null)
            {
                return BadRequest();
            }
            var result = await Mediator.Send(new CreateEmergencyInfoCommand { Model = model });
            return Ok(result);
        }
        [HttpGet]
        public async Task<IActionResult> GetAllEmergencyInfo()
        {
            var result = await Mediator.Send(new GetAllEmergencyInfoQuery() { });
            return Ok(result);
        }

        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> RemoveEmergencyInfo(int id)
        {
            var result = await Mediator.Send(new RemoveEmergencyInfoCommand() { Id = id });
            return Ok(result);
        }

        #endregion

        #region MessageTemplate
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> SaveMessageTemplate(MessageTemplateReq model)
        {
            if (model == null)
            {
                return BadRequest();
            }
            var result = await Mediator.Send(new CreateMessageTemplateCommand { Model = model });
            return Ok(result);
        }
        [HttpGet]
        public async Task<IActionResult> GetAllMessageTemplate([FromQuery] MiscTemplateSearchModel model)
        {
            var result = await Mediator.Send(new GetAllMessageTemplateQuery() { Model = model });
            return Ok(result);
        }

        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> RemoveMessageTemplate(int id)
        {
            var result = await Mediator.Send(new RemoveMessageTemplateCommand() { Id = id });
            return Ok(result);
        }

        #endregion
    }
}
