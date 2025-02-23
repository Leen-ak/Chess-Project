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

        public LoginPageController(IConfiguration config)
        {
            _config = config;
            _loginBusiness = new Login_signup_business();
        }

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
                    return Unauthorized(new { msg = "Invalid credentials" });

                var token = GenerateJwtToken(vm.UserName!);

                // ✅ FIX: Ensure cookie is set correctly
                Response.Cookies.Append("AuthToken", token, new CookieOptions
                {
                    HttpOnly = true,   // Prevent JavaScript access
                    Secure = true,     // Requires HTTPS (set to false for localhost)
                    SameSite = SameSiteMode.Strict, // Prevent CSRF
                    Expires = DateTime.UtcNow.AddHours(1)
                });

                return Ok(new { msg = "Login successful!" });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("AuthToken");
            return Ok(new { msg = "Logged out successfully" });
        }

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