using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using API.Services.Interface;
using Data.Context;
using Data.Model;
using Data.ViewModel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace API.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUserServices _userServices;
        public ITokenServices _tokenServices;

        public IConfiguration _configuration;
        private readonly MyContext _context;
        public UsersController(UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager, IUserServices userServices, ITokenServices tokenServices, IConfiguration config, MyContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _userServices = userServices;
            _configuration = config;
            _context = context;
            _tokenServices = tokenServices;
        }
        [HttpPost]
        public ActionResult Post(UserVM userVM)
        {
            return Ok(_userServices.Create(userVM));
        }
        [HttpPut("{Id}")]
        public ActionResult Put(int Id, UserVM userVM)
        {
            return Ok(_userServices.Update(Id, userVM));
        }
        [HttpDelete("{Id}")]
        public ActionResult Delete(int Id)
        {
            return Ok(_userServices.Delete(Id));
        }
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(UserVM loginVM)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    TokenVM token = new TokenVM();
                    token.Username = loginVM.UserName;
                    #region login biasa
                    var user = await _userManager.FindByNameAsync(loginVM.UserName);
                    if (user != null)
                    {
                        var authClaim = new List<Claim>
                    {
                    new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                };
                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                        var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                        var acctoken = new JwtSecurityToken(_configuration["Jwt:Issuer"],
                            _configuration["Jwt:Audience"],
                            authClaim,
                            expires: DateTime.UtcNow.AddMinutes(40),
                            signingCredentials: signIn);
                        var accessToken = new JwtSecurityTokenHandler().WriteToken(acctoken);
                        var expirationToken = DateTime.UtcNow.AddMinutes(40).Ticks;
                        var refreshToken = _tokenServices.GenerateRefreshToken();
                        var expirationRefreshToken = DateTime.UtcNow.AddMinutes(140).Ticks;

                        Token resp = _tokenServices.Get(loginVM.UserName);
                        if (resp == null)
                        {
                            _tokenServices.Create(new TokenVM
                            {
                                Username = loginVM.UserName,
                                AccessToken = accessToken,
                                ExpireToken = DateTime.UtcNow.AddMinutes(40).Ticks,
                                RefreshToken = refreshToken,
                                ExpireRefreshToken = expirationRefreshToken
                            });
                        }
                        else
                        {
                            _tokenServices.Update(new TokenVM
                            {
                                Username = loginVM.UserName,
                                AccessToken = accessToken,
                                ExpireToken = DateTime.UtcNow.AddMinutes(40).Ticks,
                                RefreshToken = refreshToken,
                                ExpireRefreshToken = expirationRefreshToken
                            });
                        }
                        return Ok(new
                        {
                            Username = loginVM.UserName,
                            AccessToken = accessToken,
                            ExpireToken = expirationToken,
                            RefreshToken = refreshToken,
                            ExpireRefreshToken = expirationRefreshToken
                        }
                            );
                    }
                    #endregion
                    return BadRequest(new { message = "Username or password is invalid" });
                }
                else
                {
                    return BadRequest("Failed");
                }
            }
            catch (Exception e) { }
            return BadRequest("Failed");
        }
        [HttpGet("Logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok("Logged out");
        }
        [HttpPost]
        [AllowAnonymous]
        [Route("register")]
        public async Task<IActionResult> Register(UserVM userVM)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = new User { };
                    user.Id = userVM.Email;
                    user.UserName = userVM.UserName;
                    user.Email = userVM.Email;
                    user.Password = userVM.Password;
                    user.CreateDate = DateTime.Now;
                    user.EmailConfirmed = true;
                    if (userVM.Role != null)
                    {
                        if (userVM.Role.ToLower() == "admin")
                        {

                            var role = await _roleManager.RoleExistsAsync(userVM.Role);
                            var check = await _roleManager.FindByNameAsync(userVM.Role);

                            //var result = await _userService.Register(userVM);
                            if (role)
                            {
                                var result = await _userManager.CreateAsync(user, userVM.Password);
                                if (result.Succeeded)
                                {
                                    await _userManager.AddToRoleAsync(user, userVM.Role);
                                }
                                return Ok("Register success");
                            }
                            else
                            {
                                var userrole = new IdentityRole(userVM.Role);
                                var result = await _roleManager.CreateAsync(userrole);
                                if (result.Succeeded)
                                {
                                    await _userManager.CreateAsync(user, userVM.Password);
                                    await _userManager.AddToRoleAsync(user, userVM.Role);
                                }
                                return Ok("Register success");
                            }
                        }
                        return BadRequest("Role Not Found");
                    }
                    else
                    {
                        var role = await _roleManager.RoleExistsAsync("Customer");
                        if (role)
                        {
                            try
                            {
                                var data = await _userManager.CreateAsync(user, userVM.Password);
                                await _userManager.AddToRoleAsync(user, "Customer");
                                return Ok("Register succes");
                            }
                            catch (Exception e)
                            {
                                return BadRequest();
                            }
                        }
                        else
                        {
                            var userrole = new IdentityRole("Customer");
                            var result = await _roleManager.CreateAsync(userrole);
                            if (result.Succeeded)
                            {
                                await _userManager.CreateAsync(user, userVM.Password);
                                await _userManager.AddToRoleAsync(user, "Customer");
                            }
                            return Ok("Register succes");
                        }
                    }
                }
                catch (Exception)
                {

                    throw;
                }
            }
            return BadRequest(ModelState);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        [HttpGet]
        public IActionResult GetSecuredData()
        {
            return Ok("Secured data " + User.FindFirst(ClaimTypes.NameIdentifier).Value);
        }

        private async Task<object> GenerateJwtToken(string username, IdentityUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.Sub, username)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(Convert.ToDouble(_configuration["JwtExpireDays"]));

            var token = new JwtSecurityToken(
                _configuration["JwtIssuer"],
                _configuration["JwtIssuer"],
                claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        [HttpPost]
        [Route("refresh")]
        public IActionResult Refresh(TokenVM tokenViewModel)
        {
            try
            {
                var refToken = _tokenServices.Get(tokenViewModel.Email);
                if (refToken.ExpireRefreshToken < DateTime.UtcNow.Ticks)
                {
                    return Unauthorized();
                }
                if (refToken.RefreshToken == tokenViewModel.RefreshToken)
                {
                    var authClaim = new List<Claim>
                    {
                    new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                };
                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                    var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    var acctoken = new JwtSecurityToken(_configuration["Jwt:Issuer"],
                        _configuration["Jwt:Audience"],
                        authClaim,
                        expires: DateTime.UtcNow.AddMinutes(40),
                        signingCredentials: signIn);
                    var accessToken = new JwtSecurityTokenHandler().WriteToken(acctoken);
                    var expirationToken = DateTime.UtcNow.AddMinutes(40).Ticks;
                    var refreshToken = _tokenServices.GenerateRefreshToken();
                    var expirationRefreshToken = DateTime.UtcNow.AddMinutes(140).Ticks;

                    Token resp = _tokenServices.Get(tokenViewModel.Email);
                    if (resp == null)
                    {
                        _tokenServices.Create(new TokenVM
                        {
                            Email = tokenViewModel.Email,
                            AccessToken = accessToken,
                            ExpireToken = DateTime.UtcNow.AddMinutes(40).Ticks,
                            RefreshToken = refreshToken,
                            ExpireRefreshToken = expirationRefreshToken
                        });
                    }
                    else
                    {
                        _tokenServices.Update(new TokenVM
                        {
                            Email = tokenViewModel.Email,
                            AccessToken = accessToken,
                            ExpireToken = DateTime.UtcNow.AddMinutes(40).Ticks,
                            RefreshToken = refreshToken,
                            ExpireRefreshToken = expirationRefreshToken
                        });
                    }
                    return Ok(new
                    {
                        Email = tokenViewModel.Email,
                        AccessToken = accessToken,
                        ExpireToken = expirationToken,
                        RefreshToken = refreshToken,
                        ExpireRefreshToken = expirationRefreshToken
                    }
                        );
                }
            }
            catch (Exception ex)
            {
                return Unauthorized(ex);
            }
            return Unauthorized();
        }
    }
}