using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DAL
{
    public class NetworkDAO
    {
        readonly IRepository<UserInfo> _repo;

        public NetworkDAO() 
        { 
            _repo = new RepositoryImplementation<UserInfo>();
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
    }
}
