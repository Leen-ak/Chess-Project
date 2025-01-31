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
    public class UserController :ControllerBase
    {
        [HttpPut]
        public async Task<IActionResult> UpdatePicture(string username, IFormFile file)
        {
            if (file == null)
                return BadRequest(new { msg = "No file uploaded." });

            try
            {
                using var memoryStream = new MemoryStream();
                await file.CopyToAsync(memoryStream);

                // ✅ Convert image to byte[] instead of string
                byte[] imageBytes = memoryStream.ToArray();

                UserVM user = new();
                user.UserName = username;
                await user.GetByUsername();

                Debug.WriteLine($"After Update - Picture: {user.Picture}");
                Debug.WriteLine($"User found: {user.Id}, Picture before update: {user.Picture}");
                Debug.WriteLine($"Image Byte Length: {imageBytes.Length}"); // ✅ Debug byte length

                if (user.Id == null)
                    return NotFound(new { msg = "User not found." });

                // ✅ Assign the correct byte[] format to Picture
                user.Picture = imageBytes;

                if (user.Picture == null || user.Picture.Length == 0)
                    Debug.WriteLine("The picture is empty");

                int retVal = await user.Update();
                Debug.WriteLine(retVal);

                return retVal == 1
                    ? Ok(new { msg = "Profile picture updated successfully!" })
                    : BadRequest(new { msg = "Failed to update profile picture!", debug = retVal });
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

    }
}
