using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace KWB.Web.Models
{
    public class ChangesHistory
    {
        [Key]
        public int ChangesHistoryID { get; set; }
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public int? PlaceID { get; set; }
        public string? GooglePlaceID { get; set; }
        public DateTime? Date { get; set; }
        public string? Name { get; set; }
        [NotMapped]
        public string? Establishment { get; set; }

    }
}
