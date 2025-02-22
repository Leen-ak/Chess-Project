using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;

namespace BusinessLogic
{
    public class Home_Business
    {
        private readonly HomeDAO _dao;

        public Home_Business()
        {
            _dao = new HomeDAO();
        }
        public async Task<byte[]?> GetProfilePicture(string username)
        {
            return await _dao.GetPictureByUsername(username);
        }

        public async Task<bool> UpdateProfilePicture(string username, byte[] imageBytes)
        {
            return await _dao.UpdateProfilePicture(username, imageBytes);
        }
    }
}
