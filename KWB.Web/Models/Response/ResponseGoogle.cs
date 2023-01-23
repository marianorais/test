using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KWB.Web.Models.Response
{
    public class ResponseGoogle
    {
        public Result Result{get;set;}
        public string Status { get; set; }
    }
    public class Result
    {
        public List<Address_components> Address_components { get; set; }
        public string Adr_address { get; set; }
        public string Business_status { get; set; }
        public string Formatted_address { get; set; }
        public string Formatted_phone_number { get; set; }
        public Geometry Geometry { get; set; }
        public string Icon { get; set; }
        public string Icon_background_color { get; set; }
        public string Icon_mask_base_uri { get; set; }
        public string International_phone_number { get; set; }
        public string Name { get; set; }
        public Opening_hours Opening_hours { get; set; }
        public List<Photos> Photos { get; set; }
        public string Place_id { get; set; }
        public string Rating { get; set; }
        public List<Reviews> Reviews { get; set; }
        public List<string> Types { get; set; }
        public string Url { get; set; }
        public int User_ratings_total { get; set; }
        public string Vicinity { get; set; }
        public string Website { get; set; }
        public string? Price_level { get; set; }
    }
    public class Address_components
    {
        public string Long_name { get;set;}
        public string Short_name { get; set; }
        public List<string> Types { get; set; }
    }
    public class Geometry
    {
        public Location Location { get; set; }
        public Viewport Viewport { get; set; }
    }
    public class Location
    {
        public string Lat { get; set; }
        public string Lng { get; set; }
    }
    public class Viewport
    {
        public Location Northeast { get; set; }
        public Location Southwest { get; set; }
    }
    public class Opening_hours
    {
        public bool Open_now { get; set; }
        public List<Periods> Periods { get; set; }
    }
    public class Periods
    {
        public bool Open_now { get; set; }
        public Open Close { get; set; }
        public Open Open { get; set; }
    }
    public class Open
    {
        public int Day { get; set; }
        public string Time { get; set; }
    }
    public class Photos
    {
        public int Height { get; set; }
        public string Photo_reference { get; set; }
        public int Width { get; set; }
        public List<string> Html_attributions { get; set; }
    }
    public class Reviews
    {
        public string Text { get; set; }
        public string Author_url { get; set; }
        public string Author_name { get; set; }
        public string Language { get; set; }
        public string Profile_photo_url { get; set; }
        public string Relative_time_description { get; set; }
        public int Rating { get; set; }
        public int Time { get; set; }
        public bool Translated { get; set; }
    }
}
