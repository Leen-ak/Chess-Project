using System;
using DAL;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Reflection;

namespace ViewModels
{
    public class UserVM
    {
        readonly private SignUpDAO _dao;
        public int? Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? PasswordConfiguration { get; set; }

        public UserVM() 
        { 
            _dao = new SignUpDAO();
        }

        public async Task Add() 
        {
            try
            {
                UserInfo user = new()
                {
                    Firstname = FirstName,
                    Lastname = LastName,
                    UserName = UserName,
                    Email = Email,
                    Password = Password
                };
                Id = await _dao.Add(user);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " "
                    + MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw; 
            }
        }
    }
}
