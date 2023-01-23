using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KWB.Web.Models.Response
{
    public class ResponseTags
    {
        public int? TagID { get; set; }
        public string? TagName { get; set; }
        public int? Sort { get; set; }
        public bool? Main { get; set; }
        public IFormFile? Photo { get; set; }
        public IFormFile? Marker { get; set; }
        public IFormFile? MarkerSelected { get; set; }
        public List<string>? Types { get; set; }
        public string? IconCode { get; set; }
        public string? FatherTag { get; set; }
        public List<Tag>? TagsDB { get; set; }
        public Tag? TagDB { get; set; }
        public string? Tag { get; set; }
    }
}
