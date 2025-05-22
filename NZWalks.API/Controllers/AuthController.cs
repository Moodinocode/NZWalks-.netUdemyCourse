using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;

namespace NZWalks.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly ITokenRepository tokenRepository;

        public AuthController(UserManager<IdentityUser> userManager,ITokenRepository tokenRepository)
        {
            this.userManager = userManager;
            this.tokenRepository = tokenRepository;
        }


        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerRequestDto )
        {
            var identityUser = new IdentityUser
            {
                UserName = registerRequestDto.Username,
                Email = registerRequestDto.Username
            };


            var identityResult = await userManager.CreateAsync(identityUser, registerRequestDto.Password);

            if (identityResult.Succeeded)
            {   if (registerRequestDto.Roles != null && registerRequestDto.Roles.Any()) 
                    {
                   identityResult = await userManager.AddToRolesAsync(identityUser, registerRequestDto.Roles);
                if (identityResult.Succeeded)
                    {
                        return Ok("User registered Successfully");
                    }
                }

            }
            return BadRequest("Something went wrong");
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
        {
            var user = await userManager.FindByEmailAsync(loginRequestDto.Username);
            if (user == null) return BadRequest("Incorrect Username");
            var PasswordCheck = await userManager.CheckPasswordAsync(user, loginRequestDto.Password);
            if (!PasswordCheck) return BadRequest("Incorrect Password");

            var roles = await userManager.GetRolesAsync(user);
            if (roles == null) return BadRequest();

            var token = tokenRepository.CreateJwtToken(user,roles.ToList());

            var response = new LoginResponseDto
            {
                jwtToken = token,
            };
            return Ok(token);

        }
    }
}
