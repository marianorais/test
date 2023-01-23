using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace KWB.Web.Models
{
    public class Review
    {
        [Key]
        public int ReviewID { get; set; }
        public string? AuthorName { get; set; }
        public string? AuthorPhoto { get; set; }
        public int? Rating { get; set; }
        public string? Age { get; set; }
        public int? AgeInDays { get; set; }
        public string? Text { get; set; }
        public int PlaceID { get; set; }

        [NotMapped]
        public string? Place { get; set; }
    }
}
