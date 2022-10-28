using BookWebAPI.Dtos.Books;
using BookWebAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookWebAPI.Controllers
{
    [ApiController, Route("[controller]/[action]/")]
    public class BooksController : ControllerBase
    {
        private readonly IBookService service;

        public BooksController(IBookService service)
        {
            this.service = service;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Create([FromBody] InputBookDto model)
        {
            var response = await service.CreateAsync(model);
            return this.RedirectToAction(nameof(GetById),new { id = response.Id});
        }


        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> GetAll()
        {
            var response = await service.GetAllAsync();
            return Ok(response);
        }


        [HttpGet, Route("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetById([FromRoute] string id)
        {
            var response = await service.GetByIdAsync(id);
            return Ok(response);
        }

        [HttpPut, Route("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Update([FromRoute] string id, [FromBody] InputBookDto model)
        {
            await service.UpdateAsync(id, model);
            return NoContent();
        }

        [HttpDelete, Route("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Delete([FromRoute] string id)
        {
            await service.DeleteAsync(id);
            return NoContent();
        }
    }
}
