﻿using System;
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

namespace ViewModels
{
    public class NetworkVM
    {
        readonly private NetworkDAO _dao;
        readonly private NetworkService _networkService; 
        public int? Id { get; set; }
        public int? FollowerId { get; set; }    
        public int? FollowingId { get; set; }
        public string? Status { get; set; }
        public string? Username {  get; set; }
        public  byte[]? Picture { get; set; }
        public string? Timer { get; set; }
        public List<NetworkVM> pendingRequests { get; set; } = new();

        public NetworkVM() 
        {
            _dao = new NetworkDAO();
            _networkService = new NetworkService();
        }

        public async Task<List<NetworkVM>> GetAll() 
        {
            List<NetworkVM> allUsername = new();

            try
            {
                List<UserInfo> allUsers = await _networkService.GetAll(); 

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
        
        public async Task GetIdByUsername()
        {
            try
            {
                Id = await _networkService.GetIdByUsername(Username!);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw;
            }
        }

        public async Task GetUsernameById()
        {
            try
            {
                UserInfo? user = await _networkService.GetUsernameById(Id!);
                Id = user?.Id;
                Username = user?.UserName;
                Picture = user?.Picture;
            }
            catch (NullReferenceException nex)
            {
                Debug.WriteLine(nex.Message);
                Username = "Username Not Found!";
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw;
            }
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
                Id = await _networkService.Add(user); 
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

                List<Follower> followers = await _networkService.GetStatusByUserId(Id!);

                if (followers.Any())
                {
                    pendingRequests = followers.Select(f => new NetworkVM
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

        public async Task<int> Update()
        {
            int updateStatus;
            try
            {
                Follower user = new()
                {
                    FollowerId = FollowerId,
                    FollowingId = FollowingId,
                    Status = Status,
                    Timer = Timer != null ? Convert.FromBase64String(Timer) : null
                };
                Debug.WriteLine($"Updating user {user.FollowerId}");
                updateStatus = Convert.ToInt16(await _networkService.Update(user));
                Debug.WriteLine($"Update return value:  { updateStatus}"); 
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw;
            }
            return updateStatus; 
        }

        public async Task<(List<NetworkVM>, int )> GetPendingRequestWithCount(int id)
        {
            try
            {
                var (requests, count) = await _networkService.GetPendingRequestWithCount(id);
                List<NetworkVM> pendingRequest = requests.Select(f => new NetworkVM { 
                    FollowerId = f.FollowerId,
                    FollowingId = f.FollowingId,
                    Status = Status
                }).ToList();
                return (pendingRequest, count); 
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
