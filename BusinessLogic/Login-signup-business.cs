using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;

namespace BusinessLogic
{
    public class Login_signup_business
    {
        private readonly SignUpDAO _signUpDao;


        private string HashPassword(string password)
        {
            var hasher = new PasswordHasher<UserInfo>();
            return hasher.HashPassword(null, password);
        }

        public Login_signup_business()
        {
            _signUpDao = new SignUpDAO();
        }

        public async Task<int> Add(UserInfo newUser)
        {
            newUser.Password = HashPassword(newUser.Password);
            return await _signUpDao.Add(newUser);
        }
    }
}
