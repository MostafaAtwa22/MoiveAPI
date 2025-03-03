using Movies.Services.DTOs.Accounts;
using RestSharp;

namespace Movies.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _RoleManager;
        private readonly IConfiguration _configuration;

        public AccountsController(UserManager<ApplicationUser> userManager,
            IConfiguration configuration,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _configuration = configuration;
            _RoleManager = roleManager;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterUserDTO registerUser)
        {
            if (ModelState.IsValid)
            {
                // save
                ApplicationUser user = new ApplicationUser();
                user.UserName = registerUser.UserName;
                user.Email = registerUser.Email;
                IdentityResult res = await _userManager.CreateAsync(user, registerUser.Password);
                if (res.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, registerUser.Role is null ? "User" : registerUser.Role);

                    return Ok("Account Added Successfully");
                }
                foreach (var item in res.Errors)
                {
                    return BadRequest(item);
                }
            }
            return BadRequest(ModelState);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginUserDTO loginUserDTO)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(loginUserDTO.UserName);
                if (user is not null)
                {
                    bool foundPassword = await _userManager.CheckPasswordAsync(user, loginUserDTO.Password);
                    if (foundPassword)
                    {
                        var claims = new List<Claim>();
                        claims.Add(new Claim(ClaimTypes.Name, loginUserDTO.UserName));
                        claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
                        claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
                        var roles = await _userManager.GetRolesAsync(user);
                        foreach (var role in roles)
                        {
                            claims.Add(new Claim(ClaimTypes.Role, role));
                        }

                        SecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:key"]!));
                        SigningCredentials signingCredentials = new SigningCredentials(
                                key,
                                SecurityAlgorithms.HmacSha256
                            );

                        JwtSecurityToken token = new JwtSecurityToken(
                                issuer: _configuration["JWT:ValidIssuer"],
                                audience: _configuration["JWT:ValidAudiance"],
                                claims: claims,
                                expires: DateTime.Now.AddHours(1),
                                signingCredentials: signingCredentials
                            );

                        // refresh token
                        var refreshToke = GenerateRefreshToke();
                        _ = int.TryParse(_configuration.GetSection("JWT")
                            .GetSection("RefreshTokenValidation").Value,
                            out int RefreshTokenValidation);

                        user.RefreshToken = refreshToke;
                        user.RefreshTokenExpirationTime = DateTime.UtcNow.AddMinutes(RefreshTokenValidation);

                        await _userManager.UpdateAsync(user);

                        return Ok(new
                        {
                            token = new JwtSecurityTokenHandler().WriteToken(token),
                            expiration = token.ValidTo
                        });
                    }
                }
                return Unauthorized();
            }
            return Unauthorized();
        }

        [AllowAnonymous]
        [HttpPost("GoogleLogin")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginDto googleLoginDto)
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(googleLoginDto.IdToken) as JwtSecurityToken;

            if (jsonToken is null)
                return BadRequest("Invalid Google token.");

            var email = jsonToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Email)?.Value;
            var name = jsonToken.Claims.FirstOrDefault(c => c.Type == "name")?.Value;
            var googleId = jsonToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;

            if (email is null || googleId is null)
                return BadRequest("Google login failed.");

            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email
                };

                var result = await _userManager.CreateAsync(user);

                if (!result.Succeeded)
                    return BadRequest("Failed to create account.");
            }

            var token = await GenerateJwtToken(user);

            return Ok(new
            {
                token,
                expiration = DateTime.UtcNow.AddHours(1),
                email,
                name
            });
        }

        private async Task<string> GenerateJwtToken(ApplicationUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudiance"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [AllowAnonymous]
        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken(TokenDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var principle = GetPrincipleFromExpiredToken(dto.Token);
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (principle is null || user is null
                || user.RefreshToken != dto.RefreshToke
                || user.RefreshTokenExpirationTime <= DateTime.UtcNow)
                return BadRequest("Can't Refresh Token !!");

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["JWT:key"]);
            var roles = _userManager.GetRolesAsync(user).Result;
            List<Claim> claims =
            [
                new (JwtRegisteredClaimNames.Email, user.Email ?? ""),
                new (JwtRegisteredClaimNames.Name, user.UserName ?? ""),
                new (JwtRegisteredClaimNames.NameId, user.Id ?? ""),
                new (JwtRegisteredClaimNames.Aud, _configuration["JWT:ValidAudiance"] ?? ""),
                new (JwtRegisteredClaimNames.Iss, _configuration["JWT:ValidIssuer"] ?? ""),
            ];

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256
                )
            };
            var newJwtToken = tokenHandler.CreateToken(tokenDescription);

            var newRefreshToken = GenerateRefreshToke();
            _ = int.TryParse(_configuration.GetSection("JWT")
                           .GetSection("RefreshTokenValidation").Value,
                           out int RefreshTokenValidation);

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpirationTime = DateTime.UtcNow.AddMinutes(RefreshTokenValidation);

            await _userManager.UpdateAsync(user);

            return Ok(dto);
        }

        private ClaimsPrincipal? GetPrincipleFromExpiredToken(string token)
        {
            var tokenParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]!)),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenParameters,
                out SecurityToken securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid Token !!");

            return principal;
        }

        private string GenerateRefreshToke()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        [Authorize]
        [HttpGet("Details")]
        public async Task<IActionResult> GetUserDetails()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId!);

            if (user is null)
                return NotFound();

            return Ok(new UserDetailsDTO
            {
                Id = user.Id,
                UserName = user.UserName!,
                Email = user.Email!,
                Roles = [.. await _userManager.GetRolesAsync(user)]
            });
        }

        [Authorize]
        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            // Retrieve the users without fetching roles
            var users = await _userManager.Users
                .Select(u => new UserDetailsDTO
                {
                    Id = u.Id,
                    Email = u.Email!,
                    UserName = u.UserName!,
                    Roles = Array.Empty<string>()
                })
                .ToListAsync();

            foreach (var user in users)
            {
                user.Roles = (await _userManager
                    .GetRolesAsync(new ApplicationUser { Id = user.Id }))
                    .ToArray();
            }

            return Ok(users);
        }

        [HttpPost("ForgetPassword")]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user is null)
                return BadRequest("Tis Email is not exists !!");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var resetLink = Url.Action("ResetPassword",
                        "Account",
                        new { email = user.Email, token = token },
                        "iisExpress",
                        "localhost:10287");

            var client = new RestClient("https://send.api.mailtrap.io/api/send");

            var request = new RestRequest
            {
                Method = Method.Post,
                RequestFormat = DataFormat.Json
            };

            request.AddHeader("Authorization", "Bearer 32c971e584b2f2b17d7a291466cdc27f");
            request.AddJsonBody(new
            {
                from = new { email = "mailtrap@demomailtrap.com" },
                to = new[] { new { email = user.Email } },
                template_uuid = "8a86e12a-1b82-4c8c-8b1a-21a050d5cdc3",
                template_variables = new { user_email = user.Email, PasswordReset = resetLink }
            });

            var response = client.Execute(request);

            if (response.IsSuccessful)
                return Ok("Email Sent With Password rest link. check your inbox");

            return BadRequest("Failed to send email !!");
        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            dto.Token = WebUtility.UrlDecode(dto.Token);

            if (user is null)
                return BadRequest("This email is not exists !!");

            var res = await _userManager.ResetPasswordAsync(user,
                dto.Token, dto.NewPassword);

            if (res.Succeeded)
                return Ok("Password Reset Successfully !!");

            return BadRequest("Can't reset this Password !!");
        }

        [HttpPost("ChangePassword")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user is null)
                return BadRequest("This Email is not Exists !!");

            var result = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);

            if (result.Succeeded)
                return Ok("The Password is changed successfully !!");

            return BadRequest(result.Errors.FirstOrDefault()!.Description);
        }
    }
}
