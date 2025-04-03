using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;
using System.Diagnostics;

namespace BusinessLogic
{
    public class Login_signup_business
    {
        private readonly SignUpDAO _signUpDao;

        public Login_signup_business()
        {
            _signUpDao = new SignUpDAO();
        }

        private string HashPassword(string password)
        {
            Debug.WriteLine("The password is", password);
            var hasher = new PasswordHasher<UserInfo>();
            return hasher.HashPassword(null, password);
        }

        public async Task<int> Add(UserInfo newUser)
        {
            newUser.Password = HashPassword(newUser.Password);
            return await _signUpDao.Add(newUser);
        }
        public async Task<UpdateStatus> Update(UserInfo updateUser)
        {
            return await _signUpDao.Update(updateUser); 
        }

        public async Task<UserInfo?> GetPassword(string username)
        {
            return await _signUpDao.GetPassword(username);
        }

        public bool VerifyPassword(string enteredPassword, string storeHash)
        {
            var hasher = new PasswordHasher<UserInfo>();
            var result = hasher.VerifyHashedPassword(null, storeHash, enteredPassword);
            return result == PasswordVerificationResult.Success; 
        }

    }
}
