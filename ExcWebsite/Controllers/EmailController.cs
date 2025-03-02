using BusinessLogic;
using Microsoft.AspNetCore.Mvc;

namespace ExcWebsite.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailController : ControllerBase
    {

        private readonly IConfiguration _config;
        public EmailController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost("send-test-email")]
        public async Task<IActionResult> SendTestEmail()
        {
            var emailService = new EmailService(_config);

            bool isSent = await emailService.SendEmailAsync(
                "Leen_8_2001@outlook.com",
                "Test Email",
                "Hello, this is a test email from Leen's app!"
            );

            return isSent ? Ok(new { msg = "Email sent successfully!" }) 
                : StatusCode(500, "Failed to send email.");
        }
    }
}
