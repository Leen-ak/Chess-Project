﻿using BusinessLogic;
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
using Microsoft.VisualBasic;


namespace ExcWebsite.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class MainPageController : ControllerBase
    {
        //Interface in ASP.NET used to access configuration setting from appsettings.json
        private readonly IConfiguration _config;
        private readonly Login_signup_business _loginBusiness;

        public MainPageController(IConfiguration config)
        {
            _config = config;
            _loginBusiness = new Login_signup_business();
        }

        [HttpPost("signup")]
        public async Task<IActionResult> Post(UserVM viewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        msg = "Invalid User Information",
                        errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                    });
                }

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

        private string GenerateJwtToken(int? userId, string username)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
               new Claim(JwtRegisteredClaimNames.Sub, username),
               new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
               new Claim(ClaimTypes.Name, username),
               new Claim("user_id", userId.ToString())
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

        [HttpGet("Users")]
        [Authorize]
        public IActionResult GetCurrentUser()
        {
            var username = User.Identity?.Name;
            var userIdClaim = User.FindFirst("user_id")?.Value;
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(userIdClaim))
                return Unauthorized();
            return Ok(new 
            { 
                username = username,
                userId = userIdClaim
            });
        }

        //we do not use GET for security reasons like the username and password will be in the request body if it was GET
        // or maybe in the URL, server logs, so instead we do post it keeps the credentials hidden from the URL 
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginVM? vm)
        {
            try
            {
                bool isValid = await vm!.ValidateLogin(vm.Password!);
                if (!isValid)
                    return Unauthorized(new { msg = "Invalid credentials" });

                var token = GenerateJwtToken(vm.userId ,vm.UserName!);
                Debug.WriteLine("THE USERID IS, " , vm.userId);
                Response.Cookies.Append("AuthToken", token, new CookieOptions
                {
                    HttpOnly = true,   // Prevent JavaScript access
                    Secure = true,     // Requires HTTPS (set to false for localhost)
                    SameSite = SameSiteMode.Strict, // Prevent CSRF
                    Expires = DateTime.UtcNow.AddHours(1)
                });

                //Return token for debugging (REMOVE in production)
                return Ok(new { msg = "Login successful!", token = token });
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
    }
}