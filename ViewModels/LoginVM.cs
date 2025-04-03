using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic;
using DAL;

namespace ViewModels
{
    public class LoginVM
    {
        readonly private Login_signup_business _service; 
        public int userId { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }

        public LoginVM()
        {
            _service = new Login_signup_business();
        }

        public async Task<(string, string)> GetPassword()
        {
            try
            {
                UserInfo? user = await _service.GetPassword(UserName!);
                UserName = user!.UserName;
                Password = user.Password;
            }
            catch (NullReferenceException nex)
            { 
                Debug.WriteLine(nex.Message);
                UserName = "Username Not Found!";
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw;
            }
            return (UserName, Password)!;
        }

        public async Task<bool> ValidateLogin(string enteredPassword)
        {
            try
            {
                UserInfo? user = await _service.GetPassword(UserName!);
                if (user == null)
                    return false;
                return _service.VerifyPassword(enteredPassword, user.Password);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in ValidateLogin: " + ex.Message);
                throw;
            }
        }
    }
}
