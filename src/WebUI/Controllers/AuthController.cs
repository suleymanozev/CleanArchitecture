using CleanArchitecture.Application.Auth.Commands.Login;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.WebUI.Controllers;

public class AuthController : ApiControllerBase
{
    [HttpPost("[action]")]
    public async Task<ActionResult<TokenVm>> Login([FromBody] LoginCommand command)
    {
        return await Mediator.Send(command);
    }
}