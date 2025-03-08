using System.Diagnostics;
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
        private readonly PasswordVM _vm;
        private readonly PasswordRecoveryService _passwordService;

        public PasswordController()
        {
            _vm = new PasswordVM();
            _passwordService = new PasswordRecoveryService();
        }

        [HttpPost("VerifyEmail")]
        public async Task<IActionResult> VerifyEmail([FromBody] PasswordVM vm)
        {
            try
            {
                await vm.GetIdtByEmail();
                return Ok(new { msg = "Email found!", userId = vm.Id });
            }
            catch (Exception ex)
            {
                return BadRequest(new { msg = ex.Message });
            }
        }

        [HttpPost("Send-Email")]
        public async Task<IActionResult> GetEmail([FromBody] PasswordVM vm)
        {
            string emailBody = $@"
                <html>
                    <body>
                        <p>Hello,</p>
                        <p>We received a request to reset your password. Click the link below:</p>
                        <p><a href='https://localhost:7223/HTML/ResetPassword.html'>Reset Password</a></p>
                        <p>If you did not request this, please ignore this email.</p>
                        <p>Best,</p>
                        <p>The Chess Gambit Team</p>
                    </body>
                </html>";
            await vm.GetEmail();
            bool isSent = await _passwordService.SendEmailAsync(
                vm.Email!,
                "Password Reset",
                emailBody
            );

            Debug.WriteLine($"Send the email to the user: {vm.Email}");
            return isSent
                ? Ok(new { msg = "Email sent successfully!" })
                : StatusCode(500, "Failed to send email.");
        }
    }
}
