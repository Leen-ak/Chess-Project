using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DAL;
using BusinessLogic;
using Microsoft.Identity.Client;
using Org.BouncyCastle.Crypto;

namespace ViewModels
{
    public class NetworkVM
    {
        readonly private NetworkService _bus; 
        public int? Id { get; set; }
        public int? FollowerId { get; set; }    
        public int? FollowingId { get; set; }
        public string? Status { get; set; }
        public string? Username {  get; set; }
        public  byte[]? Picture { get; set; }
        public string? Timer { get; set; }
        public List<NetworkVM> PendingRequests { get; set; } = new();
        public List<NetworkVM> AcceptedRequest { get; set; } = new(); 

        public NetworkVM() 
        {
            _bus = new NetworkService();
        }

        public async Task<List<NetworkVM>> SuggestedUsers(int? userId)
        {
            List<NetworkVM> userInfo = new();
            try
            {
                List<UserInfo> users = await _bus.GetSuggestedUsers(userId!);
                foreach(UserInfo u in users)
                {
                    NetworkVM vm = new()
                    {
                        Id = u.Id,
                        Username = u.UserName,
                        Picture = u.Picture,
                    };
                    userInfo.Add(vm); 
                }
                return userInfo;
            }
            catch(Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw;
            }
        }

        public async Task AddUser()
        {
            try
            {
                Follower user = new()
                {
                    FollowerId = FollowerId,
                    FollowingId = FollowingId,
                    Status = Status
                };
                Id = await _bus.AddUser(user);
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
                if (Id == null)
                {
                    Debug.WriteLine("Error: Id is null in " + GetType().Name + " " + MethodBase.GetCurrentMethod()?.Name);
                    return;
                }

                var (pendingRequest, acceptedRequest) = await _bus.GetStatusByUserId(Id!);

                if (pendingRequest.Any() || acceptedRequest.Any())
                {
                    PendingRequests = pendingRequest.Select(f => new NetworkVM
                    {
                        FollowerId = f.FollowerId,
                        FollowingId = f.FollowingId,
                        Status = f.Status
                    }).ToList();

                    AcceptedRequest = acceptedRequest.Select(f => new NetworkVM
                    {
                        FollowerId = f.FollowerId,
                        FollowingId = f.FollowingId,
                        Status = f.Status

                    }).ToList();
                }
                else
                    Status = "No Status Found";
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
