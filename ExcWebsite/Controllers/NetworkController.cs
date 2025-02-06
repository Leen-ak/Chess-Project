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
        [HttpGet]
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

        [HttpPost]
        public async Task<IActionResult> Post(NetworkVM userVM) 
        {
            try
            {
                await userVM.Add();
                return userVM.Id > 0 ? Ok(new { msg = $"User {userVM.Id} added {userVM.FollowingId} as a friend!" }) 
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
                return Ok(new { msg = $"The user status is {userVM.Status}" }); 
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
