using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DAL;
using Microsoft.Identity.Client;

namespace ViewModels
{
    public class NetworkVM
    {
        readonly private NetworkDAO _dao; 
        public string? Username {  get; set; }
        public NetworkVM() 
        {
            _dao = new NetworkDAO();
        }

        public async Task<List<NetworkVM>> GetAll() 
        {
            List<NetworkVM> allUsername = new();

            try
            {
                List <UserInfo> allUsers = await _dao.GetAll();

                foreach (UserInfo user in allUsers) 
                {
                    NetworkVM netVM = new()
                    {
                        Username = user.UserName
                    };
                    allUsername.Add(netVM);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw;
            }
            return allUsername; 
        }
    }
}
