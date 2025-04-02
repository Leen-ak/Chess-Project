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
        private readonly NetworkVM _networkVm;

        //public NetworkController(IConfiguration configuration)
        //{
        //    _tokenService = new TokenService(
        //        configuration["JwtSettings:Secret"],
        //        configuration["JwtSettings:Issuer"],
        //        configuration["JwtSettings:Audience"],
        //        int.Parse(configuration["JwtSettings:ExpirationMinutes"])
        //    );

        //    _networkVm = new NetworkVM(); 
        //}

        [HttpGet("GetUsers{userId}")]
        public async Task<IActionResult> GetAllUsername(int? userId)
        {
            try
            {
                NetworkVM vm = new();
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

        //[HttpGet("GetAllUsernames")]
        //public async Task<IActionResult> GetAll() 
        //{
        //    try
        //    {
        //        NetworkVM vm = new();
        //        List<NetworkVM> allUsers = await vm.GetAll();
        //        return Ok(allUsers);

        //    }
        //    catch (Exception ex) 
        //    {
        //        Debug.WriteLine("Problem in " + GetType().Name + " " +
        //        MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
        //        return StatusCode(StatusCodes.Status500InternalServerError);
        //    }
        //}

        //[HttpGet("getUserId/${username}")]
        //public async Task<IActionResult> GetUserId(string username)
        //{
        //    try
        //    {
        //        NetworkVM vm = new() { Username = username };
        //        await vm.GetIdByUsername();

        //        if (vm.Username == null)
        //            return NotFound(new { msg = $"The {vm.Username} not found" });
        //        return Ok(new { msg = $"The user ID of {vm.Username} is {vm.Id}" }); 
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine("Problem in " + GetType().Name + " " +
        //        MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
        //        throw;
        //    }
        //}

        //[HttpGet("GetUserName/{userId}")]
        //public async Task<IActionResult> GetUsernameById(int userId)
        //{
        //    try
        //    {
        //        NetworkVM vm = new() { Id = userId };
        //        await vm.GetUsernameById();
        //        return Ok(new { vm });
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine("Problem in " + GetType().Name + " " +
        //        MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
        //        return StatusCode(StatusCodes.Status500InternalServerError);
        //    }
        //}

        //[HttpPost("AddFollowing")]
        //public async Task<IActionResult> Post(NetworkVM userVM) 
        //{
        //    try
        //    {
        //        await userVM.Add();
        //        return userVM.Id > 0 ? Ok(new { msg = $"User {userVM.FollowingId} added {userVM.FollowingId} as a friend!" }) 
        //            : Ok(new { msg = "Invalid following or follower ID" });
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine("Problem in " + GetType().Name + " "
        //            + MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
        //        return StatusCode(StatusCodes.Status500InternalServerError);
        //    }
        //}

        //[HttpGet("Status/{userId}")]
        //public async Task<IActionResult> GetStatusByUserId(int userId)
        //{
        //    try
        //    {
        //        NetworkVM userVM = new() { Id = userId };
        //        await userVM.GetStatusByUserId();
        //        List<NetworkVM> result = new(); 
        //        return Ok(new { result = userVM.pendingRequests }); 
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine("Problem in " + GetType().Name + " " +
        //        MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
        //        return StatusCode(StatusCodes.Status500InternalServerError);
        //    }
        //}


        //[Authorize]
        //[HttpPut("UpdateStatus/{requestId}")]
        //public async Task<IActionResult> Update(int requestId ,NetworkVM requestUpdate)
        //{
        //    try
        //    {
        //        foreach (var claim in User.Claims)
        //            Console.WriteLine($"Claim Type:{claim.Type}, value: {claim.Value}");
        //        var userId = int.Parse(User.Claims.First(c => c.Type == "UserId").Value);
        //        NetworkVM vm = new();
        //        await vm.GetStatusByUserId();
        //        var friendshipRequest = vm.pendingRequests.FirstOrDefault(r => r.FollowerId == requestId);
        //        if (friendshipRequest == null)
        //            return NotFound(new { msg = "Friend request not found" });
        //        if (friendshipRequest.FollowingId != userId) //The following Id the only one can accept the request not the followerId
        //            return Unauthorized(new { msg = "Unauthorized to change this status!" });
        //        if (requestUpdate.Status != "Accepted" && requestUpdate.Status != "Rejected")
        //            return BadRequest(new { msg = "Invalid status update!" });
        //        friendshipRequest.Status = requestUpdate.Status;
        //        await vm.Update();
        //        return Ok(new { msg = $"Friend request {requestId} updated to {requestUpdate.Status}" }); 
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine("Problem in " + GetType().Name + " " +
        //        MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
        //        return StatusCode(StatusCodes.Status500InternalServerError);
        //    }
        //}

        //[HttpGet("GetAllStatus/{followingId}")]
        //public async Task<IActionResult> GetPendingStatus(int followingId)
        //{
        //    try
        //    {
        //        NetworkVM vm = new(); 
        //        var (request, count) = await vm.GetPendingRequestWithCount(followingId);
        //        return Ok( new { request, count });
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine("Problem in " + GetType().Name + " " +
        //        MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
        //        return StatusCode(StatusCodes.Status500InternalServerError);
        //    }
        //}
    }
}


//Make the code better 
//I need just 3 API 
//1. GET: GetUserNetworkData -> returns all the user data, following everything
//2. POST: AddingFriends -> sends follow request 
//3. PUT: UpdateRequest -> Accepts/Rejects follow request 