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
        private readonly ResetPasswordVM _resetPassword; 

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
                string emailBody = $@"
                 <html>
                     <body>
                         <p>Hello,</p>
                         <p>We received a request to reset your password. Click the link below:</p>
                         <p><a href='https://localhost:7223/html/Password.html'>Reset Password</a></p>
                         <p>If you did not request this, please ignore this email.</p>
                         <p>Best,</p>
                         <p>The Chess Gambit Team</p>
                     </body>
                 </html>";

                bool isSent = await _passwordService.SendEmailAsync(vm.Email!, "Password Reset", emailBody);
                return isSent ? Ok(new { msg = "Password reset email sent!" }) : StatusCode(500, new { msg = "Failed to send email." });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, new { msg = $"Server error: {ex.Message}" });
            }
        }

        [HttpPost("ValidateToken")]
        public async Task<IActionResult> ValidateResetToken([FromBody] string token)
        {
            try
            {
                bool isValid = await _passwordService.isRestTokenValid(token);
                return isValid ? Ok(new { msg = "Token is valid!" })
                               : BadRequest(new { msg = "Token is invalid or expired" }); 
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in ValidateToken API: {ex.Message}");
                return StatusCode(500, new { msg = "Server error: " + ex.Message });
            }
        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordVM VM)
        {
            try
            {
                bool isValid = await _passwordService.isRestTokenValid(VM.Token!);
                if (!isValid)
                    return BadRequest(new { msg = "Invalid or expired token" });

                if (string.IsNullOrEmpty(VM.NewPassword) || string.IsNullOrEmpty(VM.Token))
                    return BadRequest(new { msg = "New password and reset token are required" });
                bool isSuccessful = await VM.ResetPassword();
                return isSuccessful ? Ok(new { msg = "Password has been sent successfully" })
                                    : BadRequest(new { msg = "Failed to reset password. Toke may be invalid or expired" });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in ValidateToken API: {ex.Message}");
                return StatusCode(500, new { msg = "Server error: " + ex.Message });
            }
        }
    }
}
