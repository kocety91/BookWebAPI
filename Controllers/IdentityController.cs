using BookWebAPI.Dtos.Identity;
using BookWebAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookWebAPI.Controllers
{
    [ApiController,Route("[controller]/[action]/")]
    public class IdentityController : ControllerBase
    {
        private readonly IIdentityService service;

        public IdentityController(IIdentityService service)
        {
            this.service = service;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> Register(RegisterRequestModel model)
        {
            var response = await service.RegisterAsync(model);
            return Ok(response);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> Login(LoginRequestModel model)
        {
            var response = await service.LoginAsync(model);
            return Ok(response);
        }
    }
}
