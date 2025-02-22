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
        public async Task<IActionResult> UpdateProfilePicture(HomeVM model)
        {
            if (string.IsNullOrEmpty(model.UserName) || string.IsNullOrEmpty(model.Picture))
                return BadRequest(new { msg = "Username and picture are required." });

            // ✅ Convert Base64 to byte[] before passing to Business Layer
            byte[] imageBytes = Convert.FromBase64String(model.Picture);
            bool success = await _userBusiness.UpdateProfilePicture(model.UserName, imageBytes);

            return success ? Ok(new { msg = "Profile picture updated successfully!" })
                           : NotFound(new { msg = "User not found." });
        }

    }
}