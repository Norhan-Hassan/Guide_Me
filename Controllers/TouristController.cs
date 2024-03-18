//using Guide_Me.DTO;
//using Guide_Me.Models;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.IdentityModel.Tokens;
//using System.IdentityModel.Tokens.Jwt;
//using System.Security.Claims;
//using System.Text;

//namespace Guide_Me.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class TouristController : ControllerBase
//    {
//        private readonly UserManager<Tourist> _userManager;
//        private readonly IConfiguration config;

//        public TouristController(UserManager<Tourist> userManager, IConfiguration _config)
//        {
//            _userManager = userManager;
//            config = _config;

//        }

//        [HttpPost("signup")]
//        public async Task<IActionResult> SignUp(TouristRegisterDto touristDto)
//        {
//            if (ModelState.IsValid)
//            {
//                // Check if a tourist with the provided username already exists
//                var existingTouristByUsername = await _userManager.FindByNameAsync(touristDto.userName);
//                if (existingTouristByUsername != null)
//                {
//                    return BadRequest("Username is already taken.");
//                }

//                // Check if a tourist with the provided email already exists
//                var existingTouristByEmail = await _userManager.FindByEmailAsync(touristDto.email);
//                if (existingTouristByEmail != null)
//                {
//                    return BadRequest("A tourist with this email already exists.");
//                }

//                var tourist = new Tourist
//                {
//                    UserName = touristDto.userName,
//                    Email = touristDto.email,
//                    Language = touristDto.language,
//                };

//                var result = await _userManager.CreateAsync(tourist, touristDto.password);

//                if (result.Succeeded)
//                {
//                    return Ok("Account added successfully");
//                }
//                else
//                {
//                    return BadRequest(result.Errors.FirstOrDefault()?.Description ?? "Failed to create account");
//                }
//            }
//            else
//            {
//                return BadRequest(ModelState);
//            }
//        }


//        [HttpPost("signin")]
//        public async Task<IActionResult> SignIn(TouristLoginDto touristLogin)
//        {
//            if (ModelState.IsValid)
//            {
//                Tourist tourist = await _userManager.FindByNameAsync(touristLogin.userName);
//                if (tourist != null)
//                {
//                    bool found = await _userManager.CheckPasswordAsync(tourist, touristLogin.password);
//                    if (found)
//                    {
//                        //claims 
//                        List<Claim> claims = new List<Claim>();
//                        claims.Add(new Claim(ClaimTypes.Name, tourist.UserName));
//                        claims.Add(new Claim(ClaimTypes.NameIdentifier, tourist.Id));
//                        claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));

//                        //security key
//                        SecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWT:Secret"]));

//                        //signing Credentials
//                        SigningCredentials signinCred = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

//                        //roles
//                        var roles = await _userManager.GetRolesAsync(tourist);
//                        foreach (var role in roles)
//                        {
//                            claims.Add(new Claim(ClaimTypes.Role, role));

//                        }

//                        JwtSecurityToken jwttoken = new JwtSecurityToken(

//                             issuer: config["JWT:Issuer"],
//                             audience: config["JWT:Audience"],
//                             claims: claims,
//                             expires: DateTime.Now.AddHours(1),
//                             signingCredentials: signinCred
//                            );


//                        return Ok(
//                            new
//                            {
//                                token = new JwtSecurityTokenHandler().WriteToken(jwttoken),
//                                expiration = jwttoken.ValidTo
//                            });
//                    }
//                    else
//                        return Unauthorized();
//                }
//                else
//                    return Unauthorized("User doesnot Exist");
//            }
//            else
//                return Unauthorized();
//        }
//    }
//}
using Guide_Me.DTO;
using Guide_Me.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Guide_Me.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TouristController : ControllerBase
    {
        private readonly UserManager<Tourist> _userManager;
        private readonly SignInManager<Tourist> _signInManager;
        private readonly IConfiguration _config;

        public TouristController(UserManager<Tourist> userManager, SignInManager<Tourist> signInManager, IConfiguration config)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _config = config;
        }
        [HttpPost("signup")]
        public async Task<IActionResult> SignUp(TouristRegisterDto touristDto)
        {
            if (ModelState.IsValid)
            {
                var existingTouristByUsername = await _userManager.FindByNameAsync(touristDto.userName);
                if (existingTouristByUsername != null)
                {
                    return BadRequest("Username is already taken.");
                }

                var existingTouristByEmail = await _userManager.FindByEmailAsync(touristDto.email);
                if (existingTouristByEmail != null)
                {
                    return BadRequest("Email is already taken.");
                }

                var tourist = new Tourist
                {
                    UserName = touristDto.userName,
                    Email = touristDto.email,
                    Language = touristDto.language,
                };

                var result = await _userManager.CreateAsync(tourist, touristDto.password);

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(tourist, isPersistent: false);
                    var signUpToken = GenerateJwtToken(tourist, TimeSpan.FromMinutes(4)); 
                    return Ok(new { token = signUpToken });
                }
                else
                {
                    return BadRequest(result.Errors.FirstOrDefault()?.Description ?? "Failed to create account");
                }
            }
            else
            {
                return BadRequest(ModelState);
            }
        }


        [HttpPost("signin")]
        public async Task<IActionResult> SignIn(TouristLoginDto touristLogin)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(touristLogin.userName);

                if (user != null)
                {
                    var result = await _signInManager.PasswordSignInAsync(user.UserName, touristLogin.password, isPersistent: false, lockoutOnFailure: false);

                    if (result.Succeeded)
                    {
                        var signInToken = GenerateJwtToken(user, TimeSpan.FromDays(3)); 
                        return Ok(new { token = signInToken }); 
                    }
                }

                return Unauthorized("Invalid username or password.");
            }

            return BadRequest(ModelState);
        }

        private string GenerateJwtToken(Tourist user, TimeSpan expiration)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Secret"]));
            var signinCred = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var tokenOptions = new JwtSecurityToken(
                issuer: _config["JWT:Issuer"],
                audience: _config["JWT:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.Add(expiration), 
                signingCredentials: signinCred
            );

            return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        }

    }
}
