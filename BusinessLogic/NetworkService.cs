using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;
using Org.BouncyCastle.Crypto;

namespace BusinessLogic
{
    public class NetworkService
    {
        private readonly NetworkDAO _networkDao;
        
        public NetworkService()
        {
            _networkDao = new NetworkDAO();
        }
        public async Task<List<UserInfo>> GetSuggestedUsers(int? userId)
        {
            return await _networkDao.GetSuggestedUsers(userId!); 
        }

        public async Task<UserInfo?> GetUserById(int? userId)
        {
            return await _networkDao.GetUserInfoById(userId!); 
        }

        public async Task<int?> AddUser(Follower? user)
        {
            return await _networkDao.AddUser(user!); 
        }

        public async Task<UpdateStatus> UpdateFollowStatus(Follower? user)
        {
            return await _networkDao.UpdateFollowStatus(user!);
        }

        public async Task<(List<Follower> PendingSent, List<Follower> PendingReceived, List<Follower> Accepted)> GetStatusByUserId(int? userId)
        {
            return await _networkDao.GetStatusByUserId(userId);
        }

        public async Task<int?> DeleteUser(Follower? user)
        {
            return await _networkDao.DeleteUser(user!);
        }

        public async Task<(List<Follower> Request, int count)> GetPendingRequestWithCount(int? userId)
        {
            return await _networkDao.GetPendingRequestWithCount(userId!); 
        }

        public async Task<(List<Follower> Request, int count)> GetAcceptedRequestWithCount(int? userId)
        {
            return await _networkDao.GetAcceptedRequestWithCount(userId!); 
        }
    }
}
