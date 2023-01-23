using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KWB.Web.Models.Response
{
    public class ResponseContactMessage
    {
        public int ContactMessageID { get; set; }
        public string Name { get; set; }
        public string? Email { get; set; }
        public string? Message { get; set; }
        public List<ContactMessage>? ContactMessageDB { get; set; }

    }
}
