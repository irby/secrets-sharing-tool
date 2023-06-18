using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecretsSharingTool.Web.Filters;

namespace SecretsSharingTool.Web.Controllers;

[ApiController]
[AllowAnonymous]
[ExceptionFilter]
[Route("/api/[controller]")]
public abstract class BaseController : Controller
{
    protected readonly IMediator Mediator;
    
    protected BaseController(IMediator mediator)
    {
        Mediator = mediator;
    }
}