using KWB.Web.Models.Response;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace KWB.Web.Models
{
    public class Eruv
    {
        [Key]
        public int EruvID { get; set; }
        public string Point { get; set; }
        public string? Name { get; set; }
        public string? Website { get; set; }
        public string? Email { get; set; }
        public string? Comments { get; set; }
        public string? Phone { get; set; }
        public string? Posek { get; set; }
        public string? Synagogues { get; set; }
        public string? Rabbinate { get; set; }
        public string? HotlinePhone { get; set; }
        public string? City { get; set; }
        public bool? Enable { get; set; }
        [NotMapped]
        public List<string>? Points { get; set; }
        [NotMapped]
        public List<string>? ListSynagogues { get; set; }
        [NotMapped]
        public List<EruvCoordinates>? EruvCoordinates { get;set;}
}
}
