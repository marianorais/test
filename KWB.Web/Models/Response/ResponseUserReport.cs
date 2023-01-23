using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KWB.Web.Models.Response
{
    public class ResponseUserReport
    {
        public int UserReportID { get; set; }
        public int? PlaceID { get; set; }
        public string? Message { get; set; }
        public int? TypeUserReportID { get; set; }
        public DateTime? Date { get; set; }
        public List<UserReport>? UserReportDB { get; set; }

    }
}
