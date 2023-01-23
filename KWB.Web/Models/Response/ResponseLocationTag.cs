using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KWB.Web.Models.Response
{
    public class ResponseLocationTag
    {
        public int CityID { get; set; }
        public string Name { get; set; }
        public string? Lat { get; set; }
        public string? Lng { get; set; }
        public bool? Enable { get; set; }
        public IFormFile? RectangleImage { get; set; }
        public IFormFile? SquareImage { get; set; }
        public string? Description { get; set; }
        public string? Highlights { get; set; }
        public string? ThingToDo { get; set; }
        public List<string>? ThingsToDoResponse { get; set; }
        public List<LocationTag>? LocationTagDB { get; set; }
        public List<string>? ThingsToDoDB { get; set; }
        public List<string>? Tags { get; set; }
        public List<string>? TagsMain { get; set; }
        public List<string>? TagsNotMain { get; set; }

    }
}
