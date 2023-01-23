using System.Collections.Generic;

namespace KWB.Web.Models.Response
{
    public class ResponseEruv
    {
        public List<Eruv>? Eruvs { get; set; }
        public List<string>? AllSynagogues { get; set; }
        public List<EruvData>? Eruv { get; set; }
        public List<List<string>>? ListCoordinates { get; set; }
        public EruvCoordinates EruvMapResponse { get; set; }
    }
    public class EruvData
    {
        public string? Name { get; set; }
        public string? Desc { get; set; }
        public string? Rabinato { get; set; }
        public string? Web { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
    }
    public class Eruvs
    {
        public List<EruvCoordinates> EruvCoordinates { get;set; }
        public Eruv Eruv { get; set; }
    }
    public class EruvCoordinates
    {
        public decimal? Lat { get; set; }
        public decimal? Lng { get; set; }
    }
}
