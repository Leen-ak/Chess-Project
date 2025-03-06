using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ViewModels;

namespace ExcWebsite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PasswordController : ControllerBase
    {
        private readonly PasswordVM _vm;

        public PasswordController()
        {
            _vm = new PasswordVM(); 
        }

        [HttpPost("VerifyEmail")]
        public async Task<IActionResult> VerifyEmail([FromBody] PasswordVM vm)
        {
            try
            {
                await vm.GetByEmail();
                return Ok(new { msg = "Email found! Proceeding to reset.", userId = vm.Id });
            }
            catch (Exception ex)
            {
                return BadRequest(new { msg = ex.Message });
            }
        }

    }
}
