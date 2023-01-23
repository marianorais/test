using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace KWB.Web.Models
{
    public class Tag
    {
        [Key]
        public int TagID { get; set; }
        public string? TagName { get; set; }
        public string? Icon { get; set; }
        public int? TipoID { get; set; }
        public bool? Main { get; set; }
        public string? Photo { get; set; }
        public int? Sort { get; set; }
        public string? Marker { get; set; }
        public string? MarkerSelected { get; set; }
        public int? FatherID { get; set; }

        [NotMapped]
        public string? Type { get; set; }
        [NotMapped]
        public string? FatherTag { get; set; }
        [NotMapped]
        public string? MaterialIcon { get; set; }
    }
}
