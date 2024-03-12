using Guide_Me.DTO;
using Guide_Me.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Guide_Me.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
   
    public class TouristController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IConfiguration config;

        public TouristController(UserManager<ApplicationUser> userManager,IConfiguration config)
        {
            this.userManager = userManager;
            this.config = config;
        }

        public UserManager<ApplicationUser> UserManager { get; }

        [HttpPost("signup")]
        public async Task<IActionResult> SignUp(TouristRegisterDto tourist)
        {
            if(ModelState.IsValid)
            {
                ApplicationUser applicationUser = new ApplicationUser();
                applicationUser.UserName = tourist.userName;
                applicationUser.Email = tourist.email;
                IdentityResult result =await userManager.CreateAsync(applicationUser, tourist.password);
                if(result.Succeeded)
                {
                    return Ok("Account Added successfuly");
                }
                else
                {
                    return BadRequest("Account Already Exist");//result.Errors.FirstOrDefault()
                }
            }
            else
            {
                return BadRequest(ModelState); // another option to make custome class contains model state directly
            }
        }

        [HttpPost("signin")]
        public async Task<IActionResult> SignIn(TouristLoginDto touristLogin)
        {
            if(ModelState.IsValid)
            {
                ApplicationUser tourist = await userManager.FindByNameAsync(touristLogin.userName);
                if(tourist != null)
                {
                    bool found = await userManager.CheckPasswordAsync(tourist, touristLogin.password);
                    if (found)
                    {
                        //claims 
                        List<Claim> claims = new List<Claim>();
                        claims.Add(new Claim(ClaimTypes.Name, tourist.UserName));
                        claims.Add(new Claim(ClaimTypes.NameIdentifier, tourist.Id));
                        claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));

                        //security key
                        SecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWT:Secret"]));

                        //signing Credentials
                        SigningCredentials signinCred = new SigningCredentials( securityKey,SecurityAlgorithms.HmacSha256 );

                        //roles
                        var roles = await userManager.GetRolesAsync(tourist);
                        foreach (var role in roles)
                        {
                            claims.Add(new Claim(ClaimTypes.Role, role));

                        }

                        JwtSecurityToken jwttoken = new JwtSecurityToken(

                             issuer: config["JWT:Issuer"],
                             audience: config["JWT:Audience"],
                             claims: claims,
                             expires: DateTime.Now.AddHours(1),
                             signingCredentials: signinCred
                            );


                        return Ok(
                            new
                            {
                                token = new JwtSecurityTokenHandler().WriteToken(jwttoken),
                                expiration = jwttoken.ValidTo
                            });
                    }
                    else
                        return Unauthorized();
                }
                else
                    return Unauthorized("User doesnot Exist");
            }
            else
                return Unauthorized();
        }

    }
}