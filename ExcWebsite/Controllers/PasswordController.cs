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
            if (string.IsNullOrEmpty(_vm.Email))
                return BadRequest(new { msg = "Email is required" });

            await _vm.GetByEmail();

            if (_vm.Id == null)
                return NotFound(new { msg = "User not found" });
            return Ok(new { _vm.Id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msg = "Internal server error", details = ex.Message });
            }
        }
    }
}
