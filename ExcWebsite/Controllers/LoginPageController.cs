using BusinessLogic;
using DAL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Reflection;
using ViewModels;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;


namespace ExcWebsite.Controllers
{
   
    [Route("api/[controller]")]
    [ApiController]
    public class LoginPageController : ControllerBase
    {

        private readonly IConfiguration _config;
        private readonly Login_signup_business _loginBusiness;

        private string GenerateJwtToken(string username)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
               new Claim(JwtRegisteredClaimNames.Sub, username),
               new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
               new Claim(ClaimTypes.Name, username)
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public LoginPageController(IConfiguration config)
        {
            _config = config;
            _loginBusiness = new Login_signup_business();
        }

        [HttpPost("signup")]
        public async Task<IActionResult> Post(UserVM viewModel)
        {
            try
            {
                await viewModel.Add();
                return viewModel.Id > 0 ? Ok(new { msg = "User " + viewModel.FirstName + " added!" })
                    : Ok(new { msg = "User " + viewModel.FirstName + " not added!" }); 
            }
            catch (Exception ex) 
            {
                Debug.WriteLine("Problem in " + GetType().Name + " "
                    + MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError); 
            }
        }

        //we do not use GET for security reasons like the username and password will be in the request body if it was GET
        // or maybe in the URL, server logs, so instead we do post it keeps the credentials hidden from the URL 
        [HttpPost("Login")]
        public async Task<IActionResult> Login(UserVM vm)
        {
            try
            {
                bool isValid = await vm.ValidateLogin(vm.Password!);
                if (!isValid)
                    return Unauthorized(new { msg = "Invalid credentials"});
                var token = GenerateJwtToken(vm.UserName!);
                Response.Cookies.Append("AuthToken", token, new CookieOptions { 
                    HttpOnly = true, //JS can't access it 
                    Secure = true, //Sent only over HTTPS 
                    SameSite = SameSiteMode.Strict, //To prevents CSRF attacks, also that it just accept the cookies that generate from my site not another site
                    Expires = DateTime.UtcNow.AddHours(1)
                });
                return Ok(new { msg = "Login successful!", token = token });
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " "
                    + MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        //we need updating info if the user forget the password 

        //we need delete if the user wants to delete the account 

        //Get by UserName 
        [HttpGet("username/{username}")]
        public async Task<IActionResult> GetByUsername(string username)
        {
            try
            {
                UserVM viewModel = new() { UserName = username };
                await viewModel.GetByUsername();
                return Ok(viewModel);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        //Get by Email
        [HttpGet("email/{email}")]
        public async Task<IActionResult> GetByEmail(string email)
        {
            try
            {
                UserVM viewModel = new() { Email = email };
                await viewModel.GetByEmail();
                return Ok(viewModel);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }     
    }
}