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
        public string? UserName { get; set; }
        public string? PictureBase64 { get; set; }
        public string? Timer { get; set; }

        public HomeVM(string username, string pictureBase64)
        {
            UserName = username;
            PictureBase64 = pictureBase64;
        }
    }
}
