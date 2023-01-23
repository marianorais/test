using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace KWB.Web.Models
{
    public class Tokens
    {
        [Key]
        public int? TokenID { get; set; }
        public string? Token { get; set; }
        public DateTime? Date { get; set; }
        public DateTime? LastDateLog { get; set; }
        public int? UserID { get; set; }
    }
}
