using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecretsSharingTool.Core.Create;
using SecretsSharingTool.Core.Retrieve;

namespace SecretsSharingTool.Api.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("api/[controller]")]
    public sealed class SecretsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SecretsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SecretCreationCommand command)
        {
            var result = await _mediator.Send(command);

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Retrieve(string id, [Required] string key)
        {
            var query = new SecretRetrieveQuery()
            {
                Id = Guid.Parse(id),
                PrivateKey = Convert.FromBase64String(key.Replace(" ", "+"))
            };

            var result = await _mediator.Send(query);

            if (result == null)
            {
                return NotFound(new { Message = "Resource was either not found or expired, or the key provided may not be correct" });
            }

            return Ok(result);
        }
    }
}