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
                if (user?.Id == null)
                    throw new NullReferenceException("Id not found!");
                return user!.Id; 
            }
            catch(Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                 MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw;
            }
        }

        public async Task<string?> GetEmail(int? id, string email)
        {
            try 
            { 
                var user = await _repo.GetOne(user => user.Id == id);
                if (user == null)
                    throw new NullReferenceException("The user is not found");
                if (user.Email != email)
                   Debug.Write("The email does not match the entered email"); 
                return user!.Email;
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
