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

        [HttpGet("GetIdByEmail/{email}")]
        public async Task<IActionResult> GetEmail(string email)
        {
            try
            {
                PasswordVM vm = new PasswordVM { Email = email };
                await vm.GetByEmail();
                return Ok(new { msg = $"The user ID of {vm.Email} is {vm.Id} " }); 
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msg = "Internal server error", details = ex.Message });
            }
        }
    }
}
