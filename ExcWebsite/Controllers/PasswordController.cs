using System.Diagnostics;
using System.Security.Cryptography;
using BusinessLogic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ViewModels;
using static System.Net.WebRequestMethods;

namespace ExcWebsite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PasswordController : ControllerBase
    {
        private readonly EmailVM _vm;
        private readonly PasswordRecoveryService _passwordService;
        private readonly PasswordVM _passwordvm;

        public PasswordController()
        {
            _vm = new EmailVM();
            _passwordService = new PasswordRecoveryService();
            _passwordvm = new PasswordVM();
        }

        [HttpPost("VerifyEmail")]
        public async Task<IActionResult> VerifyEmail([FromBody] EmailVM vm)
        {
            try
            {
                await vm.GetIdtByEmail();
                if (vm.Id == null)
                    return BadRequest(new { msg = "User Id not found" }); 
                return Ok(new { msg = "Email found!", userId = vm.Id });
            }
            catch (Exception ex)
            {
                return BadRequest(new { msg = ex.Message });
            }
        }

        [HttpPost("Send-Email")]
        public async Task<IActionResult> SendResetEmail([FromBody] PasswordVM vm)
        {
            try
            {
                bool isSuccessful = await vm.RequestPasswordReset();
                if (!isSuccessful)
                    return BadRequest(new { msg = "Failed to generate reset token!" });

                string? token = await _passwordService.GetLatestResetToken(vm.Email!);
                if (string.IsNullOrEmpty(token))
                    return BadRequest(new { msg = "Failed to retrieve reset token!" });

                string resetLink = $"https://localhost:7223/HTML/ResetPassword.html?token={token}";

                string emailBody = $"<html><body><p>Click <a href='{resetLink}'>here</a> to reset your password.</p></body></html>";

                bool isSent = await _passwordService.SendEmailAsync(vm.Email!, "Password Reset", emailBody);
                return isSent ? Ok(new { msg = "Password reset email sent!" }) : StatusCode(500, new { msg = "Failed to send email." });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, new { msg = $"Server error: {ex.Message}" });
            }
        }

        //[HttpPost("RequestReset")]
        //public async Task<IActionResult> RequestReset([FromBody] PasswordVM vm)
        //{
        //    try
        //    {
        //        bool isSuccessful = await vm.RequestPasswordReset();
        //        if (!isSuccessful)
        //            return BadRequest(new { msg = "Failed to send reset email!" });
        //        return Ok(new {msg = "Password reset email sent successfully!"});
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine("Error in RequestReset API: " + ex.Message);
        //        return StatusCode(500, new { msg = "Server error: " + ex.Message });
        //    }
        //}
    }
}
