using MediatR;
using MemApp.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MemApp.Application.Mem.Transactions.Queries;
using MemApp.WebApi.Controllers;

namespace Res.WebApi.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DashboardController : ApiControllerBase
    {
        private readonly IDashboardService _dashboardService;
        public DashboardController(IDashboardService dashboardService, ISender sender) : base(sender)
        {
            _dashboardService = dashboardService;
        }
        [Authorize]
        [HttpGet]
        public async Task<ActionResult> GetUserConferenceData(int PageNo, int PageSize)
        {
            var result = await _dashboardService.GetUserConferenceData(PageNo, PageSize);
            return Ok(result);
        }
        [Authorize]
        [HttpGet]
        public async Task<ActionResult> GetDashboardAllData()
        {
            var result = await _dashboardService.GetDashboardData();
            return Ok(result);
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetEventTicketSaleInfo()
        {
            var result = await _dashboardService.GetEventTicketSaleInfo();
            return Ok(result);
        }
        // [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetYearlyIncomeData()
        {
            var result = await _dashboardService.GetYearlyIncomeData();
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTransactionData([FromQuery] GetAllTransactionQuery query)
        {
            return Ok(await Mediator.Send(query));
        }
    }
}
