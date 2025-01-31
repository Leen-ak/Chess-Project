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
        public string? Picture { get; set; }
        public string? Timer { get; set; }

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
                    Firstname = FirstName!,
                    Lastname = LastName!,
                    UserName = UserName!,
                    Email = Email!,
                    Password = Password!,
                    Picture = Picture
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

        public async Task<int> Update()
        {
            int updateStatus;
            try
            {
                UserInfo user = new()
                {
                    Id = (int)Id!,
                    Firstname = FirstName!,
                    Lastname = LastName!,
                    UserName = UserName!,
                    Email = Email!,
                    Password = Password!,
                    Picture = Picture,
                    Timer = Timer != null ? Convert.FromBase64String(Timer) : null
                };

                Debug.WriteLine($"Updating User {user.Id} - New Picture Length: {user.Picture?.Length}");
                updateStatus = Convert.ToInt16(await _dao.Update(user));
                Debug.WriteLine($"Update return value: {updateStatus}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " +
                MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw;
            }
            return updateStatus;
        }

        public async Task GetByUsername()
        {
            try
            {
                UserInfo? user = await _dao.GetByUsername(UserName!);
                Id = user!.Id;
                FirstName = user.Firstname;
                LastName = user.Lastname;
                UserName = user.UserName;
                Email = user.Email;
                Password = user.Password;
                Picture = user.Picture;
                Timer = Convert.ToBase64String(user.Timer!); 
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
        }

        public async Task GetByEmail()
        {
            try
            {
                UserInfo? user = await _dao.GetByEmail(Email!);
                Id = user!.Id;
                FirstName = user.Firstname;
                LastName = user.Lastname;
                UserName = user.UserName;
                Email = user.Email;
                Password = user.Password;
                Picture = user.Picture;
                Timer = Convert.ToBase64String(user.Timer!);
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
    }
}
