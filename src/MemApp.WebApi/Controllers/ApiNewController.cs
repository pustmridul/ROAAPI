using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Res.WebApi.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]/[action]")]
    public class ApiNewControllerBase : ControllerBase
    {
        private readonly ISender _mediator;

        protected ApiNewControllerBase(ISender mediator)
        {
            _mediator = mediator;
        }

        protected ISender Mediator => _mediator;
    }
}
