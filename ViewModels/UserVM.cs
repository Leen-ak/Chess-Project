using System;
using DAL;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Reflection;
using BusinessLogic;
using System.ComponentModel.DataAnnotations;

namespace ViewModels
{
    public class UserVM
    {
        readonly private Login_signup_business _service; 
        public int? Id { get; set; }

        [Required(ErrorMessage = "First name is required")]
        [MaxLength(30, ErrorMessage = "First name cannot be more than 30 characters")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        [MaxLength(30, ErrorMessage = "Last name cannot be more than 30 characters")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [MaxLength(64, ErrorMessage = "Email cannot be more than 64 characters")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Username is required")]
        [MaxLength(30, ErrorMessage = "Username cannot be more than 30 characters")]
        public string? UserName { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
        public string? Password { get; set; }

        [Required(ErrorMessage ="Confirm Password is required.")]
        [Compare("Password", ErrorMessage ="Password do not match.")]
        public string? PasswordConfiguration { get; set; }
        public byte[]? Picture { get; set; }
        public string? Timer { get; set; }

        public UserVM() 
        { 
            _service = new Login_signup_business();
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
                    Picture = Picture,
                    Timer = Timer != null ? Convert.FromBase64String(Timer) : null
                }; 
                Id = await _service.Add(user);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " "
                    + MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw; 
            }
        }

        //picture 
        public async Task<int> Update()
        {
            int updateStatus;
            try
            {
                UserInfo user = new()
                {
                    Id = (int)Id!,
                    Picture = Picture,
                    Timer = Timer != null ? Convert.FromBase64String(Timer) : null
                };

                Debug.WriteLine($"Updating User {user.Id} - New Picture Length: {user.Picture?.Length}");
                updateStatus = Convert.ToInt16(await _service.Update(user));
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
    }
}
