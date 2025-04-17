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
        public List<NetworkVM> PendingSent { get; set; } = new();
        public List<NetworkVM> PendingReceived { get; set; } = new();
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

        public async Task<UserInfo?> GetUserById(int? userId)
        {
            try
            {
                var user = await _bus.GetUserById(userId!);
                if (user == null)
                    return null;
                return new UserInfo
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Picture = user.Picture
                };
            }
            catch (Exception ex)
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

        public async Task DeleteUser()
        {
            try
            {
                Follower userToDelete = new()
                {
                    Id = this.Id,
                    FollowerId = this.FollowerId,
                    FollowingId = this.FollowingId
                };

                 await _bus.DeleteUser(userToDelete!); 
            }
            catch (Exception ex)
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

                var (pendingSent, pendingReceived, acceptedRequest) = await _bus.GetStatusByUserId(Id!);

                if (pendingSent.Any() || pendingReceived.Any() || acceptedRequest.Any())
                {
                    PendingSent = pendingSent.Select(f => new NetworkVM
                    {
                        FollowerId = f.FollowerId,
                        FollowingId = f.FollowingId,
                        Status = f.Status
                    }).ToList();

                    PendingReceived = pendingReceived.Select(f => new NetworkVM
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
        public async Task UpdateFollowStatus()
        {
            try
            {
                Follower updateUser = new()
                {
                  Id = this.Id,
                  FollowerId = this.FollowerId,
                  FollowingId = this.FollowingId,
                  Status = this.Status
                };
                await _bus.UpdateFollowStatus(updateUser); 
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw;
            }
        }

        public async Task<(List<Follower> Request, int count)> GetAcceptedRequestWithCount()
        {
            try{
                Follower statusCount = new()
                {
                    Id = this.Id,
                    FollowerId = this.FollowerId,
                    FollowingId = this.FollowingId,
                    Status = this.Status
                };
                return await _bus.GetAcceptedRequestWithCount(statusCount.FollowingId!);
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
