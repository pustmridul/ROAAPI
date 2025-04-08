using MediatR;
using MemApp.Application.Mem.Attendances.Model;
using MemApp.Application.Mem.Attendances.Queries;
using MemApp.Application.Mem.Reports.Common;
using MemApp.Application.Mem.Reports.MemberReport.Query;
using MemApp.WebApi.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MemApp.Controllers.v1.com
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AttendancensController : ApiControllerBase
    {
        public AttendancensController(ISender mediator) : base(mediator) { }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> YearlyAttendance(string memberShipNo)
        {
            var result= await Mediator.Send(new GetYearlyAttendanceMemberIdQuery() { MemberShipNo= memberShipNo });
            return Ok(result);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> DailyAttendance([FromQuery] CommonCriteria model)
        {
            var result = await Mediator.Send(new MemberAttendanceReportQuery() {Model=model});
            return Ok(result);
        }
    }
}
