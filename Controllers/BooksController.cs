using BookWebAPI.Dtos.Books;
using BookWebAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookWebAPI.Controllers
{
    [ApiController, Route("[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly IBookService service;

        public BooksController(IBookService service)
        {
            this.service = service;
        }

        [HttpPost, Route("Create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Create([FromBody] InputBookDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var output = await service.CreateAsync(model);
            return Ok(output);
        }


        [HttpGet, Route("GetAll")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> GetAll()
        {
            var output = await service.GetAllAsync();
            return Ok(output);
        }


        [HttpGet, Route("GetById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetById([FromQuery] string id)
        {
            try
            {
                var output = await service.GetByIdAsync(id);
                return Ok(output);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }

        }
    }
}
