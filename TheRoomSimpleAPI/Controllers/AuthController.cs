using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TheRoomSimpleAPI.Context;
using TheRoomSimpleAPI.Model.Requests;
using TheRoomSimpleAPI.Model.Responses;
using TheRoomSimpleAPI.Models;

namespace TheRoomSimpleAPI.Controllers
{
    [Route("api/v1/[controller]")]
    [AllowAnonymous]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private UserManager<ApplicationUser> _userManager;
        private SignInManager<ApplicationUser> _signInManager;
        private TheRoomContext _context;
        private readonly ApplicationSettings _settings;

        public AuthController(UserManager<ApplicationUser> userManager,
                             SignInManager<ApplicationUser> signInManager,
                             TheRoomContext context,
                             IOptionsSnapshot<ApplicationSettings> settings)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _settings = settings.Value;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterRequests model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.PhoneNumber,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    FullName = $@"{model.FirstName} {model.LastName}",
                    LastName = model.LastName,
                    PhoneNumber = model.PhoneNumber
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    var jwtToken = await LogUser(_settings, user);

                    return Ok(new AuthResponse() {
                        Email = user.Email,
                        FullName = user.FullName,
                        Token = jwtToken
                    });
                }
                else
                {
                    return BadRequest(result.Errors);
                }
            }
            return BadRequest(ModelState);
            
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginRequest model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, lockoutOnFailure: true);
                if (result.Succeeded)
                {
                    var currentUser = _userManager.FindByNameAsync(model.Username).Result;

                    var claim = new ClaimsIdentity(new Claim[]
                            {
                                new Claim(ClaimTypes.Email, currentUser.Email),
                                new Claim(ClaimTypes.MobilePhone, currentUser.PhoneNumber),
                                new Claim(ClaimTypes.GivenName, currentUser.FullName ?? $@"{currentUser.FirstName} {currentUser.LastName}"),
                                new Claim(ClaimTypes.Authentication, currentUser.Id),
                            });
                    var tokenString = await LogUser(_settings, currentUser);


                    return Ok(new AuthResponse
                    {
                        Email = currentUser.Email,
                        FullName = currentUser.FullName,
                        Token = tokenString
                    });
                }
                return BadRequest("Ooops!!! Something went wrong");
            }
            return BadRequest(ModelState);
        }
        private async Task<string> LogUser(ApplicationSettings settings, ApplicationUser user)
        {
            var claim = new ClaimsIdentity(new Claim[]
                            {
                                new Claim(ClaimTypes.Email, user.Email),
                                new Claim(ClaimTypes.MobilePhone, user.PhoneNumber),
                                new Claim(ClaimTypes.GivenName, user.FullName ?? $@"{user.FirstName} {user.LastName}"),
                                new Claim(ClaimTypes.Authentication, user.Id),
                            });
            await _signInManager.SignInAsync(user, isPersistent: false);

            var tokenString = GetToken(settings, claim);
            return tokenString;
        }
        private static string GetToken(ApplicationSettings settings, ClaimsIdentity subject)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(settings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = subject,
                Expires = DateTime.UtcNow.AddDays(settings.AppTokenLifeSpan),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = "TheRoom",
                IssuedAt = DateTime.Now
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return tokenString;
        }
        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                await _signInManager.SignOutAsync();
            }
            return Ok();
        }
    }
}