using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SecretsSharingTool.Core.Status;

namespace SecretsSharingTool.Api.Controllers
{
    public sealed class StatusController : BaseController
    {
        public StatusController(IMediator mediator) : base(mediator)
        {
            
        }
        
        // GET
        public async Task<IActionResult> Index()
        {
            var status = await Mediator.Send(new GetStatusQuery());

            return status ? Ok() : StatusCode(StatusCodes.Status503ServiceUnavailable);
        }
    }
}