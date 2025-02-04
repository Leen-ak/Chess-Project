using System.Diagnostics;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ViewModels;
using DAL;
using System.Linq;

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
    }
}
