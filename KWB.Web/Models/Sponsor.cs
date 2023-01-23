using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KWB.Web.Models
{
    public class Sponsor
    {
        [Key]
        public int SponsorID { get; set; }
        public string? Name { get; set; }
        public string? Text { get; set; }
        public string? Url { get; set; }
        public bool? Main { get; set; }
        public bool? Enable { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateExpiry { get; set; }
        [NotMapped]
        public IFormFile? ImageFile { get; set; }
    }
}
