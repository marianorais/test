using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace KWB.Web.Models
{
    public class ContactMessage
    {
        [Key]
        public int ContactMessageID { get; set; }
        public string Name { get; set; }
        public string? Email { get; set; }
        public string? Message { get; set; }
        public DateTime? Date { get; set; }
        public bool? WasSeen { get; set; }
    }
}
