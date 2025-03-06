using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using DAL;
using Microsoft.AspNetCore.Http.HttpResults;

namespace BusinessLogic
{
    public class PasswordRecoveryService
    {
        private readonly PasswordDAO _passwordDAO;

        public PasswordRecoveryService()
        {
            _passwordDAO = new PasswordDAO(); 
        }

        public async Task<int?> GetIdByEmail(string email)
        {
            var user = await _passwordDAO.GetIdByEmail(email);
            Debug.WriteLine($"User id is {user}"); 
            return user == null ? throw new InvalidOperationException("Id not found") : user.Value;
        }
    }
}
 