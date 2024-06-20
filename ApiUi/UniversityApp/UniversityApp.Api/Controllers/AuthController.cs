using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UniversityApp.Service.Dtos.UserDtos;
using UniversityApp.Service.Interfaces;
using System.Threading.Tasks;
using UniversityApp.Core.Entites;

namespace UniversityApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;

        public AuthController(IAuthService authService, RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager)
        {
            _authService = authService;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        [HttpGet("create-user")]
        public async Task<IActionResult> CreateFidanhUser()
        {
            var user = new AppUser
            {
                FullName = "Fidanh",
                UserName = "Fidanh",
            };

            var result = await _userManager.CreateAsync(user, "Fidan123");

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            await _userManager.AddToRoleAsync(user, "Member");

            return Ok(user.Id);
        }

        [HttpPost("login")]
        public ActionResult Login(UserLoginDto loginDto)
        {
            var token = _authService.Login(loginDto);
            return Ok(new { token });
        }

        [Authorize]
        [HttpGet("profile")]
        public ActionResult Profile()
        {
            return Ok(User.Identity.Name);
        }
    }
}
