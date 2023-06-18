using System.Net;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SecretsSharingTool.Core.Handlers.Secret.Create;
using SecretsSharingTool.Core.Handlers.Secret.Retrieve;
using BadRequestResult = SecretsSharingTool.Web.Filters.BadRequestResult;

namespace SecretsSharingTool.Web.Controllers;

public class SecretsController: BaseController
{
    public SecretsController(IMediator mediator) : base(mediator)
    {
    }
    
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(RetrieveSecretCommandResponse), (int) HttpStatusCode.OK)]
    [ProducesResponseType(typeof(NotFoundObjectResult), (int) HttpStatusCode.NotFound)]
    public async Task<ActionResult> Get([FromRoute] Guid id, [FromQuery] string key)
    {
        var command = new RetrieveSecretCommand()
        {
            SecretId = id,
            Key = key.Replace(" ", "+")
        };
        
        var response = await Mediator.Send(command);

        if (response is null)
        {
            return new NotFoundObjectResult(new { Message = "The ID provided could either not be found or has expired." });
        }

        return Ok(response);
    }
    
    [HttpPost]
    [ProducesResponseType(typeof(CreateSecretCommandResponse), (int) HttpStatusCode.Created)]
    [ProducesResponseType(typeof(BadRequestResult), (int) HttpStatusCode.BadRequest)]
    public async Task<ActionResult> Create([FromBody] CreateSecretCommand command)
    {
        var response = await Mediator.Send(command);
        return Created($"/api/{response.SecretId}?key={response.Key}", response);
    }
}
