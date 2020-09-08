using System.Text;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using backend.Helpers;
using backend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace backend.Controllers
{   
    [Route("/api/[controller]")]
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly AppSettings _appSettings;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IOptions<AppSettings> appSettings)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _appSettings = appSettings.Value;
        }
        
        [HttpPost("[action]")]
        public async Task<IActionResult>Register([FromBody] RegisterView registerView)
        {
            //Error list relating to User Registration
            List<string>errorList = new List<string>();
            
            //creating a new user
            var user = new IdentityUser
            {
                Email = registerView.Email,
                UserName = registerView.Username,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var results = await _userManager.CreateAsync(user, registerView.Password);
            if(results.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Customer");
                //Send a confirmation email
                return Ok(new {username=user.UserName, email=user.Email, status=1, message="Registration is Successful"}); 
            }
            else
            {
                foreach(var error in results.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                    errorList.Add(error.Description);
                }
            }
            
            return BadRequest(new JsonResult(errorList));

        }

        //Login Method;
        [HttpPost("[action]")]
        public async Task<IActionResult>Login([FromBody] LoginView loginView)
        {
            //Get user from the database
            var user = await _userManager.FindByNameAsync(loginView.Username);
            var roles = await _userManager.GetRolesAsync(user);
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_appSettings.Secret));
            double tokenExpireTime = Convert.ToDouble(_appSettings.ExpireTime);
            if(user != null && await _userManager.CheckPasswordAsync(user, loginView.Password))
            {  
                //Email Confirmation

                //code here
                //Token generation
                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]{
                        new Claim(JwtRegisteredClaimNames.Sub, loginView.Username),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(ClaimTypes.NameIdentifier, user.Id),

                        //role claim
                        new Claim(ClaimTypes.Role, roles.FirstOrDefault()),
                        new Claim("LoggedIn", DateTime.Now.ToString()),
                    }),

                    SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature),
                    Issuer =_appSettings.Site,
                    Audience = _appSettings.Audience,
                    Expires = DateTime.UtcNow.AddMinutes(tokenExpireTime)

                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                return Ok(new {token=tokenHandler.WriteToken(token), expiration=token.ValidTo, username=user.UserName,IdentityUserRole=roles.FirstOrDefault() });
            }

            //Error Messaage;
            ModelState.AddModelError("", "Username or Password was not found");
            return Unauthorized(new {LoginError="Pleasce check  your username credentials, Username and Password"});
        }
    }
}