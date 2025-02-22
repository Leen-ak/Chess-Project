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
        
        public async Task<bool> UpdateProfilePicture(string username, byte[] imageBytes)
        {
            var user = await _repo.GetOne(u => u.UserName == username);
            if (user == null) return false;

            user.Picture = imageBytes;
            await _repo.Update(user);
            return true; 
        }
    }
}
