using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Reflection;

namespace DAL
{
    public class SignUpDAO
    {
        readonly IRepository<UserInfo> _repo;

        public SignUpDAO() 
        {
            _repo = new RepositoryImplementation<UserInfo>();
        }

        public async Task<int> Add(UserInfo NewUser)
        {
            try
            {
                await _repo.Add(NewUser);
            }
            catch(Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " + 
                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw; 
            }
            return NewUser.Id;
        }
    }
}
