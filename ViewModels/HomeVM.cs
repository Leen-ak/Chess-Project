using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DAL;

namespace ViewModels
{
    public class HomeVM
    {
        readonly private HomeDAO _dao;
        public int? Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? PasswordConfiguration { get; set; }
        public byte[]? Picture { get; set; }
        public string? Timer { get; set; }

        public HomeVM()
        {
            _dao = new HomeDAO();
        }

        public async Task GetPictureByUsername()
        {
            try
            {
                Picture = await _dao.GetPictureByUsername(UserName!); 
            }
            catch (NullReferenceException nex)
            {
                Debug.WriteLine(nex.Message);
                UserName = "Username Not Found!";
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                    MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw;
            }
        }
    }
}
