using Movies.Services.DTOs.Movies;
using Movies.Services.Services.MoviesServices;

namespace Movies.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieServices _movieServices;

        public MoviesController(IMovieServices movieServices)
        {
            _movieServices = movieServices;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var movies = await _movieServices.GetAll();

            if (movies is null)
                return NotFound();

            return Ok(movies);
        }

        [HttpGet("GetById/{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var movie = await _movieServices.GetById(id);

            if (movie is null)
                return NotFound($"Movie With Id = {id} is not found");

            return Ok(movie);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromForm] CreateMovieDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var movie = await _movieServices.Create(dto);

            if (movie is null)
                return BadRequest($"Invalid Genere Id = {dto.GeneraId}");

            return Ok(movie);
        }

        [HttpPut("Update/{id:int}")]
        public async Task<IActionResult> Update(int id, [FromForm] UpdateMovieDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var movie = await _movieServices.Update(id, dto);

            if (movie is null)
                return BadRequest($"No Movie with ID = {id} || Invalid Genere Id = {dto.GeneraId}");

            return Ok(movie);
        }

        [HttpDelete("Delete/{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var movie = await _movieServices.Delete(id);

            if (movie is null)
                return BadRequest($"Movie With ID {id} is not exists");

            return Ok(movie);
        }
    }
}
