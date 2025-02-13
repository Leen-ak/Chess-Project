using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL; 

namespace BusinessLogic
{
    public class NetworkService
    {
        private readonly NetworkDAO _networkDao;
       
        
        public NetworkService()
        {
            _networkDao = new NetworkDAO();
        }

        //first get the username in the database to add it to the friend list you migh want to follow
        public async Task<List<UserInfo>> GetAll()
        {
            return await _networkDao.GetAll();  
        }

        //second get the userId, picture, followingId, followerId, status 
        



        public async Task<int> GetPendingRequestCount(int? userId)
        {
            return await _networkDao.GetPendingStatus(userId!); 
        }
    }
}
