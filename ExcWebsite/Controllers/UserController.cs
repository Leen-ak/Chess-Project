using BusinessLogic;
using DAL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using ViewModels;

namespace ExcWebsite.Controllers
{
    [ApiController]
    [Route("update-picture")]
    public class UserController : ControllerBase
    {
        private readonly Home_Business _userBusiness;

        public UserController()
        {
            _userBusiness = new Home_Business();
        }

        [HttpGet("profile-picture/{username}")]
        public async Task<IActionResult> GetProfilePicture(string username)
        {
            var imageBytes = await _userBusiness.GetProfilePicture(username);
            if (imageBytes == null)
                return NotFound(new { msg = "No profile picture found." });

            string base64Image = Convert.ToBase64String(imageBytes);
            return Ok(new HomeVM(username, base64Image));
        }

        [HttpPut("update-picture")]
        [Consumes("application/json")] 
        public async Task<IActionResult> UpdateProfilePicture([FromBody] HomeVM model)
        {
            if (string.IsNullOrEmpty(model.UserName) || string.IsNullOrEmpty(model.PictureBase64))
                return BadRequest(new { msg = "Username and picture are required" });

            try
            {
                Console.WriteLine($"✅ Received Username: {model.UserName}");
                Console.WriteLine($"✅ Received Image (Base64 Length): {model.PictureBase64.Length}");

                // Convert Base64 to byte array
                byte[] imageBytes = Convert.FromBase64String(model.PictureBase64);
                bool success = await _userBusiness.UpdateProfilePicture(model.UserName, imageBytes);

                return success ? Ok(new { msg = "Profile picture updated successfully" })
                               : NotFound(new { msg = "User not found" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error in {MethodBase.GetCurrentMethod()?.Name}: {ex}");
                return StatusCode(StatusCodes.Status500InternalServerError, new { msg = "Internal Server Error" });
            }
        }

        [HttpGet("profile")]
        [Authorize]
        public IActionResult GetUserProfile()
        {
            try
            {
                var usernameClaim = User.FindFirst(ClaimTypes.Name)?.Value;

                if (string.IsNullOrEmpty(usernameClaim))
                    return Unauthorized(new { msg = "User not found" });

                return Ok(new { username = usernameClaim });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in {MethodBase.GetCurrentMethod()?.Name}: {ex}");
                return StatusCode(StatusCodes.Status500InternalServerError, new { msg = "Internal Server Error" });
            }
        }

    }
}