using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace KWB.Web.Models
{
    public class ThingsToDo
    {
        [Key]
        public int ID { get; set; }
        public string? Value { get; set; }
        public int? PlaceID { get; set; }

        [NotMapped]
        public string? LocationTag { get; set; }
    }
}
