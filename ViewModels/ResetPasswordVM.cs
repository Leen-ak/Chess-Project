using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic;
using DAL;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ViewModels
{
    public class ResetPasswordVM
    {
        private readonly PasswordRecoveryService _password;

        public ResetPasswordVM()
        {
            _password = new PasswordRecoveryService();
        }

        public int? Id { get; set; }
        public string? NewPassword { get; set; }
        public string? ConfirmPassword { get; set; }
        public string? Token { get; set; }
        public string? Timer { get; set; }

        public async Task<bool> ResetPassword()
        {
            try
            {
                if (string.IsNullOrEmpty(NewPassword))
                {
                    Debug.WriteLine("New password is required");
                    return false; 
                }

                if (string.IsNullOrEmpty(Token))
                {
                    Debug.WriteLine("Reset token is required");
                    return false;
                }

                if(NewPassword != ConfirmPassword)
                {
                    Debug.WriteLine("New password does not equal confirm password");
                    return false;
                }

                await _password.ResetPassword(NewPassword, Token);
                Debug.WriteLine("Password reset successfully");
                return true; 
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw;
            }
        }
        //public async Task<int> UpdatePassword()
        //{
        //    int updateStatus;
        //    try
        //    {
        //        UserInfo user = new()
        //        {
        //            Id = (int)Id!,
        //            Password = newPassword!,
        //            Timer = Timer != null ? Convert.FromBase64String(Timer) : null
        //        };

        //        if (newPassword != confirmPassword)
        //            return updateStatus = -1; 
        //        Debug.WriteLine($"Updating user password {user.Id} - New Password Updated");
        //        updateStatus = Convert.ToInt16(await _password.ResetPassword(newPassword))
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine("Problem in " + GetType().Name + " " +
        //        MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
        //        throw;
        //    }
        //}
    }
}
