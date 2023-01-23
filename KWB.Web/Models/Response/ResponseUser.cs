using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KWB.Web.Models.Response
{
    public class ResponseUser
    {
        public int? UserId { get; set; }
        public string? IdApple { get; set; }
        public string? IdGoogle { get; set; }
        public string? CompleteName { get; set; }
        public string? Name { get; set; }
        public string? LastName { get; set; }
        public DateTime? Birthday { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Picture { get; set; }
        public string? Locale { get; set; }
        public string? Country { get; set; }
        public bool? IsEnable { get; set; }
        public List<User>? UsersDB { get; set; }
    }
}
