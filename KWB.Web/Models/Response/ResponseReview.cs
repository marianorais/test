using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KWB.Web.Models.Response
{
    public class ResponseReview
    {
        public int ReviewID { get; set; }
        public string? AuthorName { get; set; }
        public string? AuthorPhoto { get; set; }
        public int? Rating { get; set; }
        public string? Age { get; set; }
        public int? AgeInDays { get; set; }
        public string? Text { get; set; }
        public int PlaceID { get; set; }
        public List<Review> ReviewsDB { get; set; }
    }
}
