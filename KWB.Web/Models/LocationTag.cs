using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace KWB.Web.Models
{
    public class LocationTag
    {
        [Key]
        public int CityID { get; set; }
        public string Name { get; set; }
        public string? Lat { get; set; }
        public string? Lng { get; set; }
        public string? RectangleImage { get; set; }
        public string? SquareImage { get; set; }
        public string? Description { get; set; }
        public string? Highlights { get; set; }
        public int? ThingsToDoID { get; set; }
        public bool? Enable { get; set; }
        public DateTime? DateCreated { get; set; }
        public bool? Deleted { get; set; }

        [NotMapped]
        public List<string>? ThingsToDo { get; set; }
        [NotMapped]
        public List<string>? ShowThingsToDo { get; set; }
        [NotMapped]
        public List<string>? TagsMain { get; set; }
        [NotMapped]
        public List<string>? TagsNotMain { get; set; }
    }
}
