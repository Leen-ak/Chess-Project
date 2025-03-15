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
    }
}
