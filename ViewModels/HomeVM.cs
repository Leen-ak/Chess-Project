using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DAL;
using Microsoft.VisualBasic;

namespace ViewModels
{
    public class HomeVM
    {
        public int? Id { get; set; }
        public string? Email { get; set; }
        public string? UserName { get; set; }
        public string? Picture { get; set; }
        public string? Timer { get; set; }

        public HomeVM(string username, string pictureBase64)
        {
            UserName = username;
            Picture = pictureBase64;            
        }
    }
}
