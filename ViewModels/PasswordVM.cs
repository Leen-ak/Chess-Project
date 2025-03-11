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
    public class PasswordVM
    {
        readonly private PasswordRecoveryService _password;

        public PasswordVM()
        {
            _password = new PasswordRecoveryService(); 
        }

        public int? Id { get; set; }
        public string? Email { get; set; }

        public async Task GetIdtByEmail()
        {
            try
            {
                Id = await _password.GetIdByEmail(Email!);
            }
            catch (NullReferenceException nex)
            {
                Debug.WriteLine(nex.Message);
                Email = "Email Not Found!";
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw;
            }
        }

        public async Task GetEmail()
        {
            try
            {
               Email = await _password.GetEmail(Id);
            }
            catch(Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw;
            }
        }

    }
}
