using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace KWB.Web.Models
{
    public class Photo
    {
        [Key]
        public int? PhotoID { get; set; }
        public int? PlaceID { get; set; }
        public string? Url { get; set; }
        public string? Category { get; set; }
        [NotMapped]
        public string? PlaceName { get; set; }
    }
}
