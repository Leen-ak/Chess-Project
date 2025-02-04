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
    public class HomeDAO
    {
        readonly IRepository<UserInfo> _repo;
        public HomeDAO()
        {
            _repo = new RepositoryImplementation<UserInfo>();
        }

        public async Task<byte[]?> GetPictureByUsername(string username)
        {
            try
            {
                UserInfo? user = await _repo.GetOne(user => user.UserName == username);
                return user?.Picture;
            }
            catch(Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " "
                    + MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw; 
            }
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

                existingUser.Picture = updateUser.Picture;
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
    }
}
