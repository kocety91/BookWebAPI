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
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult> Register([FromBody]RegisterRequestModel model)
        {
            var response = await service.RegisterAsync(model);
            return Ok(response);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> Login([FromBody]LoginRequestModel model)
        {
            var response = await service.LoginAsync(model);
            return Ok(response);
        }
    }
}
