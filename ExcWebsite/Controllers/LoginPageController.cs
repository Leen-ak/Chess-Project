using DAL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Reflection;
using ViewModels;


namespace ExcWebsite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginPageController : ControllerBase
    {
        //we need adding info when we are doing signup 
        [HttpPost]
        public async Task<ActionResult> Post(UserVM viewModel)
        {
            try
            {
                await viewModel.Add();
                return viewModel.Id > 0 ? Ok(new { msg = "User " + viewModel.FirstName + " added!" })
                    : Ok(new { msg = "User " + viewModel.FirstName + " not added!" }); 
            }
            catch (Exception ex) 
            {
                Debug.WriteLine("Problem in " + GetType().Name + " "
                    + MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError); 
            }
        }

        //we need updating info if the user forget the password 

        //we need delete if the user wants to delete the account 

        //Get by UserName 
        [HttpGet("username/{username}")]
        public async Task<IActionResult> GetByUsername(string username)
        {
            try
            {
                UserVM viewModel = new() { UserName = username };
                await viewModel.GetByUsername();
                return Ok(viewModel);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        //Get by Email
        [HttpGet("email/{email}")]
        public async Task<IActionResult> GetByEmail(string email)
        {
            try
            {
                UserVM viewModel = new() { Email = email };
                await viewModel.GetByEmail();
                return Ok(viewModel);
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