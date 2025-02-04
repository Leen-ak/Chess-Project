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
        [HttpPut]
        public async Task<IActionResult> UpdatePicture(IFormFile file, string username)
        {
            if (file == null)
                return BadRequest(new { msg = "No file uploaded." });

            try
            {
                using var memoryStream = new MemoryStream();
                await file.CopyToAsync(memoryStream);

                byte[] imageBytes = memoryStream.ToArray();

                UserVM user = new();
                user.UserName = username;
                await user.GetByUsername();

                Debug.WriteLine($"After Update - Picture: {user.Picture}");
                Debug.WriteLine($"User found: {user.Id}, Picture before update: {user.Picture}");
                Debug.WriteLine($"Image Byte Length: {imageBytes.Length}"); 

                if (user.Id == null)
                    return NotFound(new { msg = "User not found." });

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


        [HttpGet("username/{username}")]
        public async Task<IActionResult> GetUserPicture(string username)
        {
            try
            {
                HomeVM viewModel = new() { UserName = username };
                await viewModel.GetPictureByUsername();

                if (viewModel.Picture == null || viewModel.Picture.Length == 0)
                    return NotFound(new { msg = "No profile picture found." });

                string base46Image = Convert.ToBase64String(viewModel.Picture);
                return Ok(new { picture = base46Image });
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