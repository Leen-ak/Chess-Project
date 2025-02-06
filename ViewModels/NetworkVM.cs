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
        public int? Id { get; set; }
        public int? FollowerId { get; set; }    
        public int? FollowingId { get; set; }
        public string? Status { get; set; }
        public string? Username {  get; set; }
        public  byte[]? Picture { get; set; }
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
                        Id = user.Id, 
                        Username = user.UserName,
                        Picture = user.Picture
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
        
        public async Task Add()
        {
            try
            {
                Follower user = new()
                {
                    FollowerId = FollowerId,
                    FollowingId = FollowingId,
                    Status = Status
                };
                Id = await _dao.Add(user);
            }
            catch(Exception ex) 
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw;
            }
        }

        public async Task GetStatusByUserId()
        {
            try
            {
                if(Id == null)
                {
                    Debug.WriteLine("Error: Id is null in " + GetType().Name + " " + MethodBase.GetCurrentMethod()?.Name);
                    return; 
                }
                Status = await _dao.GetStatusByUserId(Id!);
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
