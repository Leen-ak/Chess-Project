using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class EmailRequest
    {
        public string ToEmail { get; set; }  // Recipient Email
        public string Subject { get; set; }  // Email Subject
        public string Message { get; set; }  // Email Body
    }

}
