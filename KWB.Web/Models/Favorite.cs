using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace KWB.Web.Models
{
    public class Favorite
    {
        [Key]
        public int FavoriteID { get; set; }
        public int? PlaceID { get; set; }
        public DateTime? Fecha { get; set; }
        public int? UserID { get; set; }

        [NotMapped]
        public string? Place { get; set; }
        [NotMapped]
        public string? User { get; set; }
    }
}
