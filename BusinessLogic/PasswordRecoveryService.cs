using System;
using System.Collections.Generic;
using System.Linq;
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
            int? userId = await _passwordDAO.GetIdByEmail(email);
            return userId ?? throw new Exception("Email not found!"); 
        }
    }
}
 