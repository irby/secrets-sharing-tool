using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecretsSharingTool.Api.Filters;

namespace SecretsSharingTool.Api.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ExceptionFilter]
    public abstract class BaseController : ControllerBase
    {
        protected readonly IMediator Mediator;
        
        public BaseController(IMediator mediator)
        {
            Mediator = mediator;
        }
    }
}