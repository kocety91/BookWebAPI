using BookWebAPI.Dtos.Identity;
using BookWebAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> RefreshToken([FromBody]TokenRequestModel model)
        {
            var response = await service.VerifyTokenAsync(model);
            return Ok(response);
        }


        [Authorize]
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> Logout()
        {
            var userId = this.HttpContext.User.FindFirstValue("Id");
            await service.LogoutAsync(userId);

            return NoContent();
        }
    }
}
