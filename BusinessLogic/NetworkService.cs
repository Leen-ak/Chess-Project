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


        //first get the userId, picture, followingId, followerId, status 



        public async Task<int> GetPendingRequestCount(int? userId)
        {
            return await _networkDao.GetPendingStatus(userId!); 
        }
    }
}
