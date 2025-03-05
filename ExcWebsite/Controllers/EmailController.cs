using BusinessLogic;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ExcWebsite.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailController : ControllerBase
    {
        private readonly EmailService _emailService;

        public EmailController(EmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost("send-test-email")]
        public async Task<IActionResult> SendTestEmail()
        {
            bool isSent = await _emailService.SendEmailAsync(
                "leen_8_2001@outlook.com", 
                "Test Email",
                "Hello, this is a test email from Leen's app!"
            );

            return isSent
                ? Ok(new { msg = "Email sent successfully!" })
                : StatusCode(500, "Failed to send email.");
        }
    }
}
