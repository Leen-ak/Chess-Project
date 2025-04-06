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

        public async Task<List<UserInfo>> GetSuggestedUsers(int? userId)
        {
            List<UserInfo> allUsername;
            List<Follower> following;
            try
            {
                //i got the user info
                UserInfo? user = await _repo.GetOne(u => u.Id == userId);

                //get all the username 
                allUsername = await _repo.GetAll();
                following = await _followRepo.GetAll();

                //exclude all the users are following or follower and the same id
                //check if the userId == Id we do not want to suggest that
                var filterUser = allUsername.Where(u => u.Id != userId
                    && !following.Any(f => (f.FollowerId == userId && f.FollowingId == u.Id) //user is following this person
                    || (f.FollowingId == userId && f.FollowingId == u.Id) 
                    && (f.Status == "Pending" || f.Status == "Accepted")//user is followed by this person
                    )).ToList();
                return filterUser;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw;
            }
        }

        public async Task<int?> AddUser(Follower? user)
        {
            try
            {
                var existingFollower = await _followRepo.GetAll();
                bool exists = existingFollower.Any(f => f.FollowerId == user!.FollowerId && f.FollowerId == user.FollowingId);

                if (exists)
                    throw new InvalidOperationException("The user is already following the selected user");
                await _followRepo.Add(user!);
                user!.Status = "Pending"; 
            }
            catch(Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                 MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw;
            }
            return user.Id!;    
        }

        public async Task<UpdateStatus> UpdateFollowStatus(Follower? user)
        {                 
            UpdateStatus status;
            try 
            {
                var existingUser = await _followRepo.GetOne(u => u.Id == user.Id);

                if (existingUser == null)
                    return UpdateStatus.Failed;

                if (user!.Status == "Accepted" || user!.Status == "Rejected")
                {
                    existingUser.Status = user.Status;
                    status = await _followRepo.Update(existingUser);
                }
                else
                    return UpdateStatus.Failed;
            }
            catch (DbUpdateConcurrencyException)
            {
                status = UpdateStatus.Stale;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw;
            }
            return status;
        }


        //public async Task<List<UserInfo>> GetAll()
        //{
        //    List<UserInfo> allUsername;

        //    try
        //    {
        //        allUsername = await _repo.GetAll();
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine("Problem in " + GetType().Name + " " +
        //        MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
        //        throw;
        //    }
        //    return allUsername;
        //}

        //public async Task<int?> GetIdByUsername(string username)
        //{
        //    try
        //    {
        //        UserInfo? user = await _repo.GetOne(user => user.UserName == username);
        //        return user?.Id;
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine("Problem in " + GetType().Name + " " +
        //        MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
        //        throw;
        //    }
        //}

        //public async Task<UserInfo?> GetUsernameById(int? id)
        //{
        //    try
        //    {
        //        return await _repo.GetOne(user => user.Id == id);
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine("Problem in " + GetType().Name + " " +
        //            MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
        //        throw;
        //    }
        //}

        ////add followers 
        //public async Task<int> Add(Follower user)
        //{
        //    try
        //    {
        //        var existingFollowers = await _followRepo.GetAll();
        //        bool exists = existingFollowers.Any(f => f.FollowerId == user.FollowerId && f.FollowingId == user.FollowingId);

        //        if (exists)
        //        {
        //            throw new InvalidOperationException("The user is already following the selected user");
        //        }

        //        await _followRepo.Add(user);
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine("Problem in " + GetType().Name + " " +
        //            MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
        //        throw;
        //    }
        //    return user.Id;
        //}

        ////get status by userId 
        //public async Task<List<Follower>> GetStatusByUserId(int? userId)
        //{
        //    try
        //    {
        //        List<Follower> userStatus = await _followRepo.GetAll();
        //        List<Follower> pendingRequest = userStatus
        //            .Where(request => request.FollowingId == userId && request.Status == "Pending").ToList();
        //        return pendingRequest;
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine("Problem in " + GetType().Name + " " +
        //            MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
        //        throw;
        //    }
        //}

        //public async Task<UpdateStatus> Update(Follower user)
        //{
        //    UpdateStatus status;
        //    try
        //    {

        //        var existingUser = await _followRepo.GetOne(u => u.FollowerId == user.FollowerId);
        //        if (existingUser == null)
        //        {
        //            Debug.WriteLine($"User {user.FollowerId} not found in database!");
        //            return UpdateStatus.Failed;
        //        }

        //        existingUser.Status = user.Status;
        //        status = await _followRepo.Update(existingUser); 
        //        return status;

        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        status = UpdateStatus.Stale;
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine("Problem in " + GetType().Name + " " +
        //        MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
        //        throw;
        //    }
        //    return status;
        //}

        //public async Task<(List<Follower> Request, int count)> GetPendingRequestWithCount(int? userId)
        //{
        //    try
        //    {
        //        List<Follower> userStatus = await _followRepo.GetAll() ;
        //        List<Follower> pendingStatus = userStatus.Where(
        //            request => request.FollowingId == userId && request.Status == "Pending").ToList();
        //        int pendingCount = pendingStatus.Count;
        //        return (pendingStatus, pendingCount); 
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine("Problem in " + GetType().Name + " " +
        //        MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
        //        throw;
        //    }
        //}
    }
}
