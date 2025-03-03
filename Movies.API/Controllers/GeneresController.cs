using Movies.Services.DTOs.Generes;
using Movies.Services.Services.GeneresServices;

namespace Movies.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class GeneresController : ControllerBase
    {
        private readonly IGenereService _genereService;

        public GeneresController(IGenereService genereService)
        {
            _genereService = genereService;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllGeneres()
        {
            var generes = await _genereService.GetAll();
            return Ok(generes);
        }

        [HttpGet("GetById/{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var Genera = await _genereService.GetById(id);

            if (Genera is null)
                return NotFound();

            return Ok(Genera);
        }


        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] GenereDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            Genera genera = new Genera
            {
                Name = dto.Name,
                Description = dto.Description,
            };

            await _genereService.Create(genera);

            return Ok(genera);
        }

        [HttpPut("Update/{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] GenereDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var genera = await _genereService.Update(id, dto);

            if (genera is null)
                return NotFound($"Genere With ID {id} is not found!!");

            return Ok(genera);
        }

        [HttpDelete("Delete/{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var genera = await _genereService.Delete(id);

            if (genera is null)
                return NotFound($"Genere With ID {id} is not found!!");

            return Ok(genera);
        }
    }
}
