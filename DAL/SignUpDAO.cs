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

        public async Task<int?> Add(UserInfo? NewUser)
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
            return NewUser!.Id;
        }

        public async Task<UpdateStatus> Update(UserInfo updateUser)
        {
            UpdateStatus status;
            try
            {

                var existingUser = await _repo.GetOne(u => u.Id == updateUser.Id);
                if (existingUser == null)
                {
                    Debug.WriteLine("User not found in database!");
                    return UpdateStatus.Failed;
                }

                status = await _repo.Update(updateUser);
                return status;

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

        public async Task<UserInfo?> GetPassword(string username)
        {
            try
            {
                return await _repo.GetOne(user => user.UserName == username);
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
