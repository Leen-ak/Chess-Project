using DAL;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace ExcWebsite.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController :ControllerBase
    {
        private readonly ChessContext _dao;

        public UserController(ChessContext dao)
        {
            _dao = dao;
        }

        [HttpGet("profile/{username}")]
        public IActionResult GetProfile(string username)
        {
            var user = _dao.UserInfos.FirstOrDefault(u => u.UserName == username);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            //if there is not profile picture return a default picture
            string profilePicture = string.IsNullOrEmpty(user.Picture) ? "/images/user.png" : user.Picture;
            return Ok(profilePicture);
        }
    }
}
