using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Azure.Core;
using Microsoft.EntityFrameworkCore;

namespace DAL
{
    public class NetworkDAO
    {
        readonly IRepository<UserInfo> _repo;
        readonly IRepository<Follower> _followRepo;

        public NetworkDAO() 
        { 
            _repo = new RepositoryImplementation<UserInfo>();
            _followRepo = new RepositoryImplementation<Follower>();
        }

        public async Task<List<UserInfo>> GetAll()
        {
            List<UserInfo> allUsername;
            try
            {
                allUsername = await _repo.GetAll();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw;
            }
            return allUsername;
        }

        public async Task<int?> GetIdByUsername(string username)
        {
            try
            {
                UserInfo? user = await _repo.GetOne(user => user.UserName == username);
                return user?.Id;
            }
              catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw;
            }
        }

        //add followers 
        public async Task<int> Add(Follower user)
        {
            try
            {
                var existingFollowers = await _followRepo.GetAll();
                bool exists = existingFollowers.Any(f => f.FollowerId == user.FollowerId && f.FollowingId == user.FollowingId);
                
                if(exists)
                {
                    throw new InvalidOperationException("The user is already following the selected user");
                }

                await _followRepo.Add(user);
            }
            catch(Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                    MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw;
            }
            return user.Id; 
        }

        //get status by userId 
        public async Task<List<Follower>> GetStatusByUserId(int? userId)
        {
            try
            {
                List<Follower> userStatus = await _followRepo.GetAll();
                List<Follower> pendingRequest = userStatus
                    .Where(request => request.FollowingId == userId && request.Status == "Pending").ToList();
                return pendingRequest;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                    MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw;
            }
        }

        public async Task<UserInfo?> GetUsernameById(int? id)
        {
            try
            {
                return await _repo.GetOne(user => user.Id == id);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                    MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw;
            }
        }

        //get followers

        //delete followers 


    }
}
