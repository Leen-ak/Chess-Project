using BusinessLogic;
using DAL;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using ViewModels;

namespace ExcWebsite.Controllers
{
    [ApiController]
    [Route("update-picture")]
    [Consumes("multipart/form-data")]
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
        public async Task<IActionResult> UpdateProfilePicture(IFormFile file, string username)
        {
            if (file == null || string.IsNullOrEmpty(username))
                return BadRequest(new { msg = "Username and file are required." });

            try
            {
                using var memoryStream = new MemoryStream();
                await file.CopyToAsync(memoryStream);
                byte[] imageBytes = memoryStream.ToArray();

                bool success = await _userBusiness.UpdateProfilePicture(username, imageBytes);
                return success ? Ok(new { msg = "Profile picture updated successfully!" })
                               : NotFound(new { msg = "User not found." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating profile picture: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new { msg = "Internal Server Error" });
            }
        }
    }
}