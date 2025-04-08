using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MemApp.WebApi.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]/[action]")]
    public abstract class ApiControllerBase : ControllerBase
    {

        private readonly ISender _mediator;

        protected ApiControllerBase(ISender mediator)
        {
            _mediator = mediator;
        }

        protected ISender Mediator => _mediator;
    }


}
