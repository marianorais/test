using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace KWB.Web.Models
{
    public class PrayerTime
    {
        [Key]
        public int? PrayerTimeID { get; set; }
        public string? Name { get; set; }
        public string? Time { get; set; }
        public int? PlaceID { get; set; }

    }
}
