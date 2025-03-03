using Movies.Services.DTOs.Roles;

namespace Movies.API.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RolesController(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }


        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var roles = await _roleManager.Roles.ToListAsync();

            var roleDetailsTasks = roles.Select(async role => new RoleDetails
            {
                Id = role.Id,
                Name = role.Name!,
                TotalUsers = (await _userManager.GetUsersInRoleAsync(role.Name!)).Count
            });

            return Ok(roleDetailsTasks);
        }

        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(string id)
        {
            var role = await _roleManager
                .FindByIdAsync(id);

            if (role is null)
                return NotFound($"Role with Id = {id} is not exists !!");
            var dto = new RoleDetails
            {
                Id = role.Id,
                Name = role.Name!,
                TotalUsers = (await _userManager.GetUsersInRoleAsync(role.Name!)).Count
            };

            return Ok(dto);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create(CreateRoleDTO Dto)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var roleFound = await _roleManager.RoleExistsAsync(Dto.RoleName);

            if (roleFound)
                return BadRequest("Role is already exists");

            var roleResult = await _roleManager.CreateAsync(new IdentityRole(Dto.RoleName));
            if (roleResult.Succeeded)
                return Ok("Role Created Successfully !!");

            return BadRequest("Role Creation failed !!");
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);

            if (role is null)
                return NotFound("Role is not Exists");

            var res = await _roleManager.DeleteAsync(role);

            if (res.Succeeded)
                return Ok("Role is Delete Successfully !!");

            return BadRequest("Can't delete this role");
        }

        [HttpPost("Assign")]
        public async Task<IActionResult> Assign(RoleAssignDto dto)
        {
            var user = await _userManager.FindByIdAsync(dto.UserId);

            if (user is null)
                return NotFound("User is not found !!");

            var role = await _roleManager.FindByIdAsync(dto.RoleId);

            if (role is null)
                return NotFound("Role is not found !!");

            var result = await _userManager.AddToRoleAsync(user, role.Name!);

            if (result.Succeeded)
                return Ok("Role assigned Successfully !!");

            return BadRequest("Can't assign this role !!");
        }
    }
}
