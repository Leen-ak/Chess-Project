using System.Diagnostics;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ViewModels;
using BusinessLogic;
using DAL;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using Microsoft.Extensions.Configuration.UserSecrets;
using Microsoft.AspNetCore.Authorization;


namespace ExcWebsite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NetworkController : ControllerBase
    {
        private readonly TokenService _tokenService;
        NetworkVM vm = new();

        public NetworkController(IConfiguration configuration)
        {
            _tokenService = new TokenService(
                configuration["Jwt:Secret"],
                configuration["Jwt:Issuer"],
                configuration["Jwt:Audience"],
                int.Parse(configuration["Jwt:ExpirationMinutes"])
            );
        }

        [HttpGet("GetUsers{userId}")]
        public async Task<IActionResult> GetAllUsername(int? userId)
        {
            try
            { 
                var allUsers = await vm.SuggestedUsers(userId!);
                return Ok(allUsers); 
            }
            catch(Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("GetUserById/{userId}")]
        public async Task<IActionResult> GetUser(int? userId)
        {
            try
            {
                var user = await vm.GetUserById(userId!);
                if (user == null)
                    return NotFound(new { msg = "User not found" });
                return Ok(user); 
            }
            catch(Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("FollowRequest")]
        [Authorize]
        public async Task<IActionResult> FollowRequest([FromBody] NetworkVM request)
        {
            try
            {
                var followerId = int.Parse(User.FindFirst("user_id")?.Value ?? "0");
                if (followerId == 0 || request.FollowingId == 0)
                    return BadRequest(new { msg = "Invalid user IDs" });

                NetworkVM vm = new NetworkVM
                {
                    FollowerId = followerId,
                    FollowingId = request.FollowingId,
                    Status = "Pending"
                };
                await vm.AddUser();

                if (vm.Id > 0)
                    return Ok(new { msg = $"Follow request sent from {followerId} to {request.FollowingId}" });
                return BadRequest(new { msg = "Could not send follow request" });
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpDelete("UnfollowUser")]
        [Authorize]
        public async Task<IActionResult> DeleteUser([FromBody] NetworkVM user)
        {
            var followerId = int.Parse(User.FindFirst("user_id")?.Value ?? "0");
            if (followerId == 0 || user.FollowingId == 0)
                return BadRequest(new { msg = "Invalid user IDs" });

            NetworkVM vm = new NetworkVM
            {
                FollowerId = user.FollowerId,
                FollowingId = user.FollowingId
            };

            await vm.DeleteUser();
            return Ok(new { msg =$"User {vm.Id} unfollowed successfully"});

        }

        [HttpGet("Status")]
        [Authorize]
        public async Task<IActionResult> GetStatusByUserId()
        {
            try
            {
                var userId = int.Parse(User.FindFirst("user_id")!.Value ?? "0");
                if (userId == null)
                    return BadRequest(new { msg = "Invalid user ID" });
                NetworkVM vm = new NetworkVM
                {
                    Id = userId
                };

                await vm.GetStatusByUserId();
                return Ok(vm);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPut("UpdateStatus")]
        [Authorize]
        public async Task<IActionResult> UpdateStatus([FromBody] NetworkVM updateUser)
        {
            try
            {
                NetworkVM vm = new()
                {
                    FollowerId = updateUser.FollowerId,
                    FollowingId = updateUser.FollowingId,
                    Status = updateUser.Status
                };
                await vm.UpdateFollowStatus();
                return Ok(vm);
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


//Make the code better 
//I need just 3 API 
//1. GET: GetUserNetworkData -> returns all the user data, following everything
//2. POST: AddingFriends -> sends follow request 
//3. PUT: UpdateRequest -> Accepts/Rejects follow request 