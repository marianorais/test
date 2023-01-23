using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace KWB.Web.Models
{
    public class Suggestion
    {
        [Key]
        public int SuggestID { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
        public string? Categories { get; set; }
        public string? Website { get; set; }
        public string? Phone { get; set; }
        public DateTime? Date { get; set; }
        public bool? WasSeen { get; set; }
    }
}
