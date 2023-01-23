using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace KWB.Web.Models
{
    public class UserReport
    {
        [Key]
        public int UserReportID { get; set; }
        public int? PlaceID { get; set; }
        public int? UserID { get; set; }
        public string? Message { get; set; }
        public int? TypeUserReportID { get; set; }
        public DateTime? Date { get; set; }
        public bool? WasSeen { get; set; }
        public string? Email { get; set; }


        [NotMapped]
        public string? Place { get; set; }
        [NotMapped]
        public User? User { get; set; }
        [NotMapped]
        public string? UserEmail { get; set; }
    }
}
