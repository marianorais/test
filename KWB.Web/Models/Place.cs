using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace KWB.Web.Models
{
    public class Place
    {
        [Key]
        public int PlaceID { get; set; }
        public string? Name { get; set; }
        public string? Lat { get; set; }
        public string? Lng { get; set; }
        public string? Address { get; set; }
        public string? Photo { get; set; }
        public string? About { get; set; }
        public string? Phone { get; set; }
        public string? Website { get; set; }
        public bool? IsFake { get; set; }
        public bool? IsComplete { get; set; }
        public string? ZipCode { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? LastUpdated { get; set; }
        public int? TipoID { get; set; }
        public int? CityID { get; set; }
        public int? CertificationID { get; set; }
        public bool? Permanently_closed { get; set; }
        public string? GooglePlaceID { get; set; }
        public int? ReviewsCount { get; set; }
        public bool? Enable { get; set; }
        public int? TagMarkerID { get; set; }
        public bool? Deleted { get; set; }

        [NotMapped]
        public string? Type { get; set; }
        [NotMapped]
        public string? PriceLevel { get; set; }
        [NotMapped]
        public string? Rating { get; set; }
        [NotMapped]
        public string? CertificationName { get; set; }
        [NotMapped]
        public List<string>? LocationsTag { get; set; }
        [NotMapped]
        public List<string>? Tags { get; set; }
        [NotMapped]
        public string? TagMain { get; set; }
        [NotMapped]
        public int? NumberOfTags { get; set; }
        [NotMapped]
        public int? DiffDaysToday { get; set; }
        [NotMapped]
        public List<Review>? listReviews { get; set; }
        [NotMapped]
        public List<List<string>>? listHours { get; set; }
        [NotMapped]
        public List<string>? SecundaryImages { get; set; }
        [NotMapped]
        public List<string>? ExtraImages { get; set; }
    }
}
