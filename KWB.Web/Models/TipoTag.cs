using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace KWB.Web.Models
{
    public class TipoTag
    {
        [Key]
        public int TipoTagID { get; set; }
        public int? PlaceID { get; set; }
        public int? CityID { get; set; }
        public int? TipoID { get; set; }
        public int? TagID { get; set; }
        public bool? Main { get; set; }
    }
}
