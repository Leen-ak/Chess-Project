using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;


namespace DAL
{
    public class PasswordDAO
    {
        readonly IRepository<UserInfo> _repo;
        readonly IRepository<PasswordResetToken> _passRepo; 
        public PasswordDAO()
        {
            _repo = new RepositoryImplementation<UserInfo>();
            _passRepo = new RepositoryImplementation<PasswordResetToken>();
        }

        public async Task<int?> GetIdByEmail(string email)
        {
            try
            {
                var user = await _repo.GetOne(user => user.Email == email);
                if (user?.Id == null)
                    throw new NullReferenceException("Id not found!");
                if(user.Email != email)
                    Debug.Write("The email does not match the entered email");
                return user!.Id; 
            }
            catch(Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                 MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw;
            }
        }

        public async Task<string?> GetEmail(int? id)
        {
            try 
            { 
                var user = await _repo.GetOne(user => user.Id == id);
                if (user?.Id == null)
                    throw new NullReferenceException("The user is not found");
                return user!.Email;
            }
            catch(Exception ex) 
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                    MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw;
            }
        }

        public async Task SavePasswordResetToken(int userId, string token, DateTime expiry)
        {
            try
            {
                var existingToken = await _passRepo.GetOne(u => u.UserId == userId);
                if (existingToken != null)
                {
                    existingToken.ResetToken = token;
                    existingToken.RestTokenExpiry = expiry;
                    await _passRepo.Update(existingToken);
                }
                else
                {
                    var passwordToken = new PasswordResetToken
                    {
                        UserId = userId,
                        ResetToken = token,
                        RestTokenExpiry = expiry
                    };
                    _passRepo.Add(passwordToken);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw;
            }

        }
        public async Task<string?> GetLatestResetToken(string? email)
        {
            try
            {
                var userId = await GetIdByEmail(email!);
                if (userId == null)
                    return null;

                var tokenRecord = await _passRepo.GetOne(t => t.UserId == userId.Value);
                return tokenRecord?.ResetToken;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error in GetLatestResetToken: " + ex.Message);
                throw;
            }
        }

        public async Task<bool> isResetTokenValid(string token)
        {
            try
            {
                var tokenRecord = await _passRepo.GetOne(t => t.ResetToken == token);
                if (tokenRecord == null)
                    return false;
                if (tokenRecord.RestTokenExpiry < DateTime.UtcNow)
                    return false; //token is expired
                return true;
            }
            catch(Exception ex)
            {
                Debug.WriteLine("Error in IsResetTokenValid: " + ex.Message);
                throw; 
            }
        }

        private string HashPassword(string password)
        {
            Debug.WriteLine("The password is", password);
            var hasher = new PasswordHasher<UserInfo>();
            return hasher.HashPassword(null, password);
        }

        public async Task ResetPassword(string? newPassword, string token)
        {
            try
            {
                var record = await _passRepo.GetOne(t => t.ResetToken == token);
                if (record == null)
                    Debug.WriteLine("Token is not found");
                if (record!.RestTokenExpiry < DateTime.UtcNow)
                    Debug.WriteLine("Toke is invalid or expired");

                if (record.ResetToken != null && record.RestTokenExpiry > DateTime.UtcNow)
                    Debug.WriteLine("Token is not valid");

                UserInfo? user = await _repo.GetOne(u => u.Id == record.UserId);
                if (user == null)
                    Debug.WriteLine("UserId is not found");
                user!.Password = HashPassword(newPassword!);
                await _repo.Update(user);
                await _passRepo.Delete(record!.Id);
                Debug.WriteLine("Password has been reset successfully"); 
            }
            catch(Exception ex)
            {
                Debug.WriteLine("Error in IsResetTokenValid: " + ex.Message);
                throw;
            }
        }
    }
}
