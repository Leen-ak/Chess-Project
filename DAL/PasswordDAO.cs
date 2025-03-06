using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class PasswordDAO
    {
        readonly IRepository<UserInfo> _repo; 
        public PasswordDAO()
        {
            _repo = new RepositoryImplementation<UserInfo>(); 
        }

        public async Task<int?> GetIdByEmail(string email)
        {
            try
            {
                var user = await _repo.GetOne(user => user.Email == email);
                return user!.Id; 
            }
            catch(Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                 MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw;
            }
        }
    }
}
