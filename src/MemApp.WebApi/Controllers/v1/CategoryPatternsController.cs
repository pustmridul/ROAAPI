using MediatR;
using MemApp.Application.Mem.Members.Models;
using MemApp.Application.Mem.Members.Queries;
using Microsoft.AspNetCore.Mvc;

namespace MemApp.WebApi.Controllers.v1
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CategoryPatternsController : ApiControllerBase
    {
        public CategoryPatternsController(ISender mediator) : base(mediator) { }
      
        [HttpGet]
        public async Task<IActionResult> GetCategoryPatterns()
        {
            var result = await Mediator.Send(new GetCategoryPatternsQuery());
            return Ok(result);
        }
    }
}
