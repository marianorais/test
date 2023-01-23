using KWB.Web.Models.Response;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KWB.Web.Models.NewFolder
{
    public class ResponsePlace
    {
        public string? Name { get; set; }
        public string? ID { get; set; }
        public string? Location { get; set; }
        public string? EstablishmentType { get; set; }
        public string? ZipCode { get; set; }
        public string? About { get; set; }
        public string? Country { get; set; }
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
        public string? Photos { get; set; }
        public string? AuthorName { get; set; }
        public string? AuthorUrl { get; set; }
        public string? AuthorRating { get; set; }
        public string? RelativeTime { get; set; }
        public string? Text { get; set; }
        public string? Time { get; set; }
        public string? Rating { get; set; }
        public string? Phone { get; set; }
        public string? OpeningTime { get; set; }
        public string? ClosingTime { get; set; }
        public string? Website { get; set; }
        public int? PriceLevel { get; set; }
        public int? ReviewsCount { get; set; }
        public bool? CantMiss { get; set; }
        public bool? Questionable { get; set; }
        public bool? NotKosher { get; set; }
        public string? GoogleMainPhoto { get; set; }
        public string? GooglePlaceID { get; set; }
        public IFormFile? ImageFile { get; set; }
        public IFormFile? SecundaryPicture { get; set; }
        public IFormFile? SecundaryPicture2 { get; set; }
        public IFormFile? SecundaryPicture3 { get; set; }
        public int? SecundaryPictureID { get; set; }
        public bool? Enable { get; set; }
        public string? SecundaryPictureUrl { get; set; }
        public string? SecundaryPicture2Url { get; set; }
        public string? SecundaryPicture3Url { get; set; }
        public List<string>? Tags { get; set; }
        public List<string>? LocationsTag { get; set; }
        public string? CertificationName { get; set; }
        public bool? Permanently_closed { get; set; }
        public IFormFile? ImageCategorie1 { get; set; }
        public IFormFile? ImageCategorie2 { get; set; }
        public IFormFile? ImageCategorie3 { get; set; }
        public List<Certification>? Certifications { get; set; }
        public List<Tag>? TagsDB { get; set; }
        public List<string>? LocationsDB { get; set; }
        public List<string>? TypesDB { get; set; }
        public List<string>? TagsMainDB { get; set; }
        public List<Tag>? Markers { get; set; }
        public string? MarkerID { get; set; }
        public string? PrincipalTag { get; set; }
        public List<string>? TagsNotMainDB { get; set; }
        public List<Place>? PlacesDB { get; set; }
        public Place? PlaceDB { get; set; }
        public ResponseGoogle? GoogleDataPlace { get; set; }
        public string? GooglePictureMain { get; set; }
        public string? GoogleSecundaryPicture { get; set; }
        public string? GoogleSecundaryPicture2 { get; set; }
        public string? GoogleSecundaryPicture3 { get; set; }
        public string? ExtraPicture { get; set; }
        public string? Categories { get; set; }
        public List<string>? PrayersTime { get; set; }
        public List<List<string>>? PrayersTime2 { get; set; }
    }
    public class PrayerTime
    {
        public string? Prayer { get; set; }
        public string? Time { get; set; }
    }
    public class PrayerTimeResponse
    {
        public string Name { get; set; }
        public string Day { get; set; }
        public string Time { get; set; }
        public string Shacharis { get; set; }
        public string Mincha { get; set; }
        public string Maariv { get; set; }
    }
}
