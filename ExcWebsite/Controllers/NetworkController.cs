using System.Diagnostics;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ViewModels;
using DAL;
using System.Linq;
using System.Reflection.Metadata.Ecma335;

namespace ExcWebsite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NetworkController : ControllerBase
    {
        [HttpGet("GetAllUsernames")]
        public async Task<IActionResult> GetAll() 
        {
            try
            {
                NetworkVM vm = new();
                List<NetworkVM> allUsers = await vm.GetAll();
                return Ok(allUsers);

            }
            catch (Exception ex) 
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("getUserId/${username}")]
        public async Task<IActionResult> GetUserId(string username)
        {
            try
            {
                NetworkVM vm = new() { Username = username };
                await vm.GetIdByUsername();

                if (vm.Username == null)
                    return NotFound(new { msg = $"The {vm.Username} not found" });
                return Ok(new { msg = $"The user ID of {vm.Username} is {vm.Id}" }); 
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw;
            }
        }

        [HttpGet("GetUserName/{userId}")]
        public async Task<IActionResult> GetUsernameById(int userId)
        {
            try
            {
                NetworkVM vm = new() { Id = userId };
                await vm.GetUsernameById();
                return Ok(new { vm });
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("AddFollowing")]
        public async Task<IActionResult> Post(NetworkVM userVM) 
        {
            try
            {
                await userVM.Add();
                return userVM.Id > 0 ? Ok(new { msg = $"User {userVM.FollowingId} added {userVM.FollowingId} as a friend!" }) 
                    : Ok(new { msg = "Invalid following or follower ID" });
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " "
                    + MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("Status/{userId}")]
        public async Task<IActionResult> GetStatusByUserId(int userId)
        {
            try
            {
                NetworkVM userVM = new() { Id = userId };
                await userVM.GetStatusByUserId();
                List<NetworkVM> result = new(); 
                return Ok(new { result = userVM.pendingRequests }); 
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPut("UpdateStatus")]
        public async Task<IActionResult> Update(NetworkVM vm)
        {
            try
            {
                int updateValue = await vm.Update();
                return updateValue switch
                {
                    1 => Ok(new { msg = "User " + vm.FollowerId + " with the status " + vm.Status + " updated!" }),
                    -1 => Ok(new { msg = "User " + vm.FollowerId + " with the status " + vm.Status + " not updated!" }),
                    -2 => Ok(new { msg = "Data is stale for " + vm.FollowerId + ", User not updated!" }),
                    _ => Ok(new { msg = "User " + vm.FollowerId + " not updated!" }),
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("GetAllStatus/{followingId}")]
        public async Task<IActionResult> GetPendingStatus(int followingId)
        {
            try
            {
                NetworkVM vm = new(); 
                var (request, count) = await vm.GetPendingRequestWithCount(followingId);
                return Ok( new { request, count });
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
