using System.Net;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SecretsSharingTool.Core.Handlers.Health;
using SecretsSharingtool.Data.Database;

namespace SecretsSharingTool.Web.Controllers;

public class HealthController : BaseController
{
    public HealthController(IMediator mediator) : base(mediator)
    {
    }
    
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var response = await Mediator.Send(new HealthCheckQuery());
        return response.IsSuccess
            ? Ok()
            : new StatusCodeResult((int)HttpStatusCode.ServiceUnavailable);
    }
}