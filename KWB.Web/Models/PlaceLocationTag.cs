using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace KWB.Web.Models
{
    public class PlaceLocationTag
    {
        [Key]
        public int PlaceLocationTagID { get; set; }
        public int PlaceID { get; set; }
        public int LocationTagID { get; set; }
    }
}
