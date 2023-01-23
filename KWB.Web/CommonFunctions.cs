using KWB.Web.Models;
using KWB.Web.Models.NewFolder;
using KWB.Web.Models.Response;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace KWB.Web
{
    public class CommonFunctions
    {
        #region Create objects
        public Place CreateEstablishment(ResponsePlace establishment, string filePath, Certification certification)
        {
            Place newEstablishment = new Place
            {
                Name = establishment.Name,
                Lat = establishment.Latitude,
                Lng = establishment.Longitude,
                Address = establishment.Location,
                About = establishment.About,
                Photo = filePath,
                Phone = establishment.Phone,
                IsFake = false,
                Website = establishment.Website,
                TipoID = 6,
                ZipCode = establishment.ZipCode,
                DateCreated = System.DateTime.Now,
                LastUpdated = System.DateTime.Now,
                CityID = null,
                Permanently_closed = establishment.Permanently_closed,
                GooglePlaceID = establishment.GooglePlaceID,
                ReviewsCount = establishment.ReviewsCount,
                Enable = establishment.Enable,
                TagMarkerID = establishment.MarkerID == "0" ? null : Int32.Parse(establishment.MarkerID)
            };
            if (certification != null) { newEstablishment.CertificationID = certification.CertificationID; }
            return newEstablishment;
        }
        public TipoTag CreateTipoTag(int? placeID, int? locationTagID, int? tagID, bool main)
        {
            TipoTag newTipoTag = new TipoTag
            {
                PlaceID = placeID,
                CityID = locationTagID,
                TagID = tagID,
                Main = main
            };
            return newTipoTag;
        }
        public PlaceLocationTag CreatePlaceLocationTag(int placeID, int locationTagID)
        {
            PlaceLocationTag newPlaceLocationTag = new PlaceLocationTag
            {
                PlaceID = placeID,
                LocationTagID = locationTagID
            };
            return newPlaceLocationTag;
        }
        public Caracteristicas CreateCaracteristica(int placeID, string? name, string? value)
        {
            Caracteristicas newCaracteristica = new Caracteristicas
            {
                PlaceID = placeID,
                Caracteristica = name,
                Valor = value
            };
            return newCaracteristica;
        }
        public Certification CreateCertification(string name, string? website, string? icon)
        {
            Certification newCert = new Certification
            {
                Name = name,
                Website = website,
                Icono = icon
            };
            return newCert;
        }
        public ApiCallsRegister CreateApiCallRegister(string name)
        {
            ApiCallsRegister newApiCallRegister = new ApiCallsRegister
            {
                Name = name,
                Count = 1,
                Date = System.DateTime.Now
            };
            return newApiCallRegister;
        }
        public LocationTag CreateLocationTag(ResponseLocationTag locationTag, string filePathRectangle, string filePathSquare)
        {
            LocationTag newLocationTag = new LocationTag
            {
                Name = locationTag.Name,
                Lat = locationTag.Lat,
                Lng = locationTag.Lng,
                Description = locationTag.Description,
                Highlights = locationTag.Highlights,
                RectangleImage = filePathRectangle,
                SquareImage = filePathSquare,
                Enable = locationTag.Enable,
                DateCreated = System.DateTime.Now
            };
            return newLocationTag;
        }
        public UserChange CreateUserChange(int userID, string description, string? InterfaceID)
        {
            UserChange newUserChange = new UserChange
            {
                UserID= userID,
                Description= description,
                Date = System.DateTime.Now,
                InterfaceID= InterfaceID,
            };
            return newUserChange;
        }

        #endregion
        public string ReturnIconFontawesome(string code)
        {
            switch (code)
            {
                case "0xe18b":
                    return "fa-address-book";
                    break;
                case "0xf04be":
                    return "fa-apple";//APPLE
                    break;
                case "0xe0a0":
                    return "fa-arrow-up";
                    break;
                case "0xe122":
                    return "fa-calendar";
                    break;
                case "0xe7ca":
                    return "fa-bar-chart";
                    break;
                case "0xf04c3":
                    return "fa-university";
                    break;
                case "0xe1f6":
                    return "fa-check";
                    break;
                case "0xe4b6":
                    return "fa-camera";
                    break;
                case "0xe1af":
                    return "fa-exclamation-triangle";
                    break;
                case "0xe050":
                    return "fa-plus";
                    break;
                case "0xe0d7":
                    return "fa-bed";
                    break;
                case "0xe13e":
                    return "fa-gift";
                    break;
                case "0xe640":
                    return "fa-tag";
                    break;
                case "0xe3ae":
                    return "fa-unlock";
                    break;
                case "0xe0b2":
                    return "fa-dollar-sign";
                    break;
                case "0xf055c"://Rocket
                    return "fa-rocket";
                    break;
                case "0xe514":
                    return "fa-refresh";
                    break;
                case "0xe22d":
                    return "fa-flag";
                    break;
                case "0xe532":
                    return "fa-cutlery";
                    break;
                case "0xe061":
                    return "fa-cog";
                    break;
                case "0xf04c6"://Balance
                    return "fa-balance-scale";
                    break;
                case "0xe0e1":
                    return "fa-ban";
                    break;
                case "0xe1fe":
                    return "fa-bell";
                    break;
                case "0xe178":
                    return "xeb44";
                    break;
                case "0xf04b7":
                    return "fa-credit-card";
                    break;
                case "0xe51c":
                    return "fa-eye";
                    break;
                case "0xe288":
                    return "fa-fire-extinguisher";//Matafuego
                    break;
                case "0xf0513":
                    return "fa-female";
                    break;
                case "0xf0516":
                    return "fa-heart";
                    break;
                case "0xe318":
                    return "fa-home";
                    break;
                case "0xe3f8":
                    return "fa-money";
                    break;
                case "0xf0541"://NewsPaper
                    return "fa-newspaper-o";
                    break;
                case "0xf1c2":
                    return "fa-book";
                    break;
                case "0xf04ed"://Diamond
                    return "fa-diamond";
                    break;
                case "0xe297":
                    return "fa-plane";
                case "0xe037":
                    return "fa-snowflake-o";//Snow
                case "0xe03a":
                    return "fa-clock-o";//Time
                case "0xf51b":
                    return "fa-male";//Human
                case "0xe03e":
                    return "fa-wheelchair";//Silla de ruedas
                case "0xf520":
                    return "xe850";//Wallet
                case "0xe046":
                    return "fa-bug";//Bug
                case "0xe048":
                    return "fa-camera-retro";//New Photo
                case "0xe04b":
                    return "fa-plus-square";//Rectangle More
                case "0xe054":
                    return "fa-map-marker";//Marker
                case "0xee50":
                    return "fa-television";//TV
                case "0xe172":
                    return "fa-cloud";//Upload
                case "0xee85":
                    return "fa-arrow-left";//Back
                case "0xe0b6":
                    return "fa-music";//Music
                case "0xe0d1":
                    return "fa-battery-full";//Full
                case "0xe0d6":
                    return "fa-umbrella";//Sombrilla
                case "0xe0e4":
                    return "fa-bluetooth-b";//Bluetooth
                case "0xe0fa":
                    return "fa-pencil-square-o";//Draw
                case "0xe113":
                    return "fa-pencil";//Pincel
                case "0xe120":
                    return "fa-birthday-cake";//Cake
                case "0xe6fe":
                    return "fa-search-minus";
                case "0xe6fd":
                    return "fa-search-plus";
                case "0xf34b":
                    return "fa-search";
                case "0xf608":
                    return "fa-phone";
                case "0xf286":
                    return "fa-mobile";
                case "0xe17e":
                    return "fa-commenting";
                case "0xe66b":
                    return "fa-toggle-on";
                case "0xf44e":
                    return "fa-toggle-off";
                case "0xe043":
                    return "fa-user-circle-o";
                case "0xe04f":
                    return "fa-plus-circle";
                case "0xf537":
                    return "fa-plus";
                case "0xe05a":
                    return "fa-cart-plus";
                case "0xe081":
                    return "fa-at";
                case "0xe084":
                    return "fa-anchor";
                case "0xe089":
                    return "fa-building";
                case "0xf05bb":
                    return "fa-area-chart";
                case "0xe095":
                    return "fa-arrow-circle-o-down";
                case "0xf05bc":
                    return "fa-arrow-circle-o-left";
                case "0xf05bd":
                    return "fa-arrow-circle-o-right";
                case "0xe096":
                    return "fa-arrow-circle-o-up";
                case "0xeea1":
                    return "fa-paperclip";
                case "0xf5ac":
                    return "fa-bath";
                case "0xf06d8":
                    return "fa-battery-empty";
                case "0xf078a":
                    return "fa-battery-quarter";
                case "0xf0733":
                    return "fa-battery-half";
                case "0xf078d":
                    return "fa-battery-three-quarters";
                case "0xeecb":
                    return "fa-moon-o";
                case "0xe0ee":
                    return "fa-bolt";
                case "0xe0f1":
                    return "fa-bookmark";
                case "0xf5cf":
                    return "fa-bookmark-o";
                case "0xe105":
                    return "fa-circle";
                case "0xf5e5":
                    return "fa-sun-o";
                default:
                    break;
            }
            return "";
        }
        public List<Tag> ReturnTag(List<int?> tagsID, List<Tag> tags)
        {
            var tagsResponse = new List<Tag>();
            for (var i = 0; i < tagsID.Count(); i++)
            {
                var tag = tags.Where(a => a.TagID == tagsID[i]).FirstOrDefault();
                if (tag != null) { tagsResponse.Add(tag); }
            }
            return tagsResponse;
        }
        public Place GetPlaceDetail(string placeName)
        {
            try
            {
                string requestUri = string.Format("https://maps.googleapis.com/maps/api/geocode/xml?key={1}&address={0}&sensor=false", Uri.EscapeDataString(placeName), "AIzaSyCRJLTDv-YeDn8TIx7uYptHtuB5DwbXYJs");

                WebRequest request = WebRequest.Create(requestUri);
                WebResponse response = request.GetResponse();
                XDocument xdoc = XDocument.Load(response.GetResponseStream());

                XElement result = xdoc.Element("GeocodeResponse").Element("result");
                if (result != null)
                {
                    XElement locationAddress = result.Element("formatted_address");
                    XElement googleID = result.Element("place_id");
                    var placeDetail = ReturnPlaceDetail((string)googleID, placeName);
                    if (placeDetail != null)
                    {
                        return placeDetail;
                    }
                }
                return null;
            }
            catch
            {
                return null;
            }
        }
        public Place ReturnPlaceDetail(string googleID,string placeName) {
            try
            {
                var dict = new Dictionary<string, string>();
                var url = "https://maps.googleapis.com/maps/api/place/details/json?key=AIzaSyCRJLTDv-YeDn8TIx7uYptHtuB5DwbXYJs&language=en&place_id=" + googleID;
                var client = new HttpClient();
                var req = new HttpRequestMessage(HttpMethod.Post, url) { Content = new FormUrlEncodedContent(dict) };
                var res = client.Send(req);
                if (res.StatusCode.ToString() == "OK")
                {
                    var responseAPI = res.Content.ReadAsStringAsync().Result;
                    var jsonResponse = JsonConvert.DeserializeObject<ResponseGoogle>(responseAPI);
                    var zipCode = "";
                    var listReviews= new List<Review>();
                    var listOpeningHours =new List<List<string>>();

                    for (int i = 0; i < jsonResponse.Result.Address_components.Count(); i++) {
                        if (jsonResponse.Result.Address_components[i].Types[0] == "postal_code") {
                            zipCode = jsonResponse.Result.Address_components[i].Short_name;
                        }
                    }
                    var Permanently_closed = jsonResponse.Result.Opening_hours == null ? false : !jsonResponse.Result.Opening_hours.Open_now;

                    Place newPlace = new()
                    {
                        Name = placeName,
                        Lat = jsonResponse.Result.Geometry.Location.Lat,
                        Lng = jsonResponse.Result.Geometry.Location.Lng,
                        Address = jsonResponse.Result.Formatted_address,
                        Photo = GetGooglePhoto(jsonResponse.Result.Photos == null ? "" : jsonResponse.Result.Photos[0].Photo_reference),
                        ZipCode = zipCode,
                        PriceLevel = jsonResponse.Result.Price_level == null ? "": jsonResponse.Result.Price_level,
                        Website = jsonResponse.Result.Website == null ? "" : jsonResponse.Result.Website,
                        Phone = jsonResponse.Result.International_phone_number == null ?"" : jsonResponse.Result.International_phone_number,
                        Rating = jsonResponse.Result.Rating == null ? "" : jsonResponse.Result.Rating,
                        Permanently_closed = Permanently_closed,
                        ReviewsCount = jsonResponse.Result.User_ratings_total,
                        DateCreated=System.DateTime.Now,
                        GooglePlaceID = googleID,
                        LastUpdated = System.DateTime.Now
                    };
                    newPlace.Photo = FixPhotos(newPlace.Photo);
                    newPlace.SecundaryImages = new List<string>();
                    newPlace.ExtraImages = new List<string>();
                    newPlace.SecundaryImages.Add(FixPhotos(GetGooglePhoto(jsonResponse.Result.Photos == null ? "" : jsonResponse.Result.Photos[1].Photo_reference)));
                    newPlace.SecundaryImages.Add(FixPhotos(GetGooglePhoto(jsonResponse.Result.Photos == null ? "" : jsonResponse.Result.Photos[2].Photo_reference)));
                    newPlace.SecundaryImages.Add(FixPhotos(GetGooglePhoto(jsonResponse.Result.Photos == null ? "" : jsonResponse.Result.Photos[3].Photo_reference)));
                    for(int i = 0; i< jsonResponse.Result.Photos?.Count(); i++)
                    {
                        if (i > 3)
                        {
                            newPlace.ExtraImages.Add(FixPhotos(GetGooglePhoto(jsonResponse.Result.Photos == null ? "" : jsonResponse.Result.Photos[i].Photo_reference)));
                        }
                    }
                    if (jsonResponse.Result.Reviews != null) { 
                        for (int i = 0; i < jsonResponse.Result.Reviews.Count(); i++)
                        {
                            Review newReview = new()
                            {
                                AuthorName = jsonResponse.Result.Reviews[i].Author_name == null ? "" : jsonResponse.Result.Reviews[i].Author_name,
                                AuthorPhoto = jsonResponse.Result.Reviews[i].Author_url == null ? "" : jsonResponse.Result.Reviews[i].Author_url,
                                Rating = jsonResponse.Result.Reviews[i].Rating ,
                                Age = jsonResponse.Result.Reviews[i].Relative_time_description == null ? "" : jsonResponse.Result.Reviews[i].Relative_time_description,
                                AgeInDays = jsonResponse.Result.Reviews[i].Time,
                                Text = jsonResponse.Result.Reviews[i].Text == null ? "" : jsonResponse.Result.Reviews[i].Text,
                                PlaceID = newPlace.PlaceID,
                            };
                            listReviews.Add(newReview);
                        }
                    }
                    if (jsonResponse.Result.Opening_hours?.Periods != null)
                    {
                        listOpeningHours.Add(new List<string>());
                        listOpeningHours.Add(new List<string>());
                        for (var i = 0; i < 7; i++)
                        {

                            try
                            {
                                var openTime = jsonResponse.Result.Opening_hours.Periods[i].Open.Time;
                                if (openTime == null || openTime.Length < 1) { listOpeningHours[0].Add("00:00"); }
                                else
                                {
                                    listOpeningHours[0].Add(openTime);
                                }
                            }
                            catch { listOpeningHours[0].Add("00:00"); }
                            try
                            {
                                var closeTime = jsonResponse.Result.Opening_hours.Periods[i].Close == null ? null: jsonResponse.Result.Opening_hours.Periods[i].Close.Time;
                            
                                if (closeTime == null || closeTime.Length < 1) { listOpeningHours[1].Add("00:00"); }
                                else
                                {
                                    listOpeningHours[1].Add(closeTime);
                                }
                            }
                            catch { listOpeningHours[1].Add("00:00"); }
                        }
                    }
                    newPlace.listReviews = listReviews;
                    newPlace.listHours = listOpeningHours;
                    return newPlace;
                }
                return null;
            }
            catch
            {
                return null;
            }
        }
        public string GetGooglePhoto(string photoReference)
        {
            try {
                if(photoReference == null || photoReference == "") { return ""; }
                var dict = new Dictionary<string, string>();
                var url = "https://maps.googleapis.com/maps/api/place/photo?photo_reference=" + photoReference + "&maxheight=500&maxwidth=500&key=AIzaSyCRJLTDv-YeDn8TIx7uYptHtuB5DwbXYJs";
                var client = new HttpClient();
                var req = new HttpRequestMessage(HttpMethod.Post, url) { Content = new FormUrlEncodedContent(dict) };
                var res = client.Send(req);
                if (res.StatusCode.ToString() == "OK")
                {
                    var responseAPI = res.Content.ReadAsStringAsync().Result;
                    var photo = req.RequestUri.ToString();
                    return photo;
                }
            }
            catch { }
            return "";
        }
        public bool CheckUrl(string url)
        {
            return Uri.IsWellFormedUriString(url, UriKind.Absolute);
        }
        public string[] CompleteOpeningHours(string[] timer)
        {
            var newTimer = new string[7];
            for (var i = 0; i < timer.Length; i++)
            {
                if (timer[i].Length > 1 && timer[i] != null)
                {
                    timer[i] = timer[i].Replace(":", "");
                    newTimer[i] = timer[i].Insert(2, ":");
                }
                else
                {
                    newTimer[i] = "00:00";
                }
            }
            for (var i = 0; i < newTimer.Length; i++)
            {
                if (newTimer[i] == null)
                {
                    newTimer[i] = "00:00";
                }
            }
            return newTimer;
        }
        public string FixPhotos(string photo)
        {
            if (photo != null && photo.Contains("maps.googleapis.com"))
            {
                        var newPhoto = photo.Split("1s").ToList();
                        var photoSplit = newPhoto[1].Split("&callback").ToList();
                        var photoReference = photoSplit[0];
                        var photoGoogle = new CommonFunctions().GetGooglePhoto(photoReference);
                        if (photoGoogle != "")
                        {
                            photo = photoGoogle;
                        }
                        return photo;
            }
            else
            {
                return photo;
            }
        }
        public string PrayersTime(string prayerTime)
        {
            var prayersResponse = "";
            var listPrayers = prayerTime.Split(",").ToList();
            foreach(var prayer in listPrayers)
            {
                var prayerResponse = prayer.Split("**").ToList();
                if (prayerResponse[1] != "")
                {
                    prayersResponse = prayersResponse + prayerResponse[0] + "**" + prayerResponse[1] + "##";
                }
            }
            return prayersResponse;
        }
        public string PrayersTime2(string prayerTime)
        {
            prayerTime = prayerTime.Replace("NaN", "").Replace(",,", "").Replace(",##", "##").Replace("**,", "**")
                                   .Replace("Shacharit**##", "").Replace("Mincha**##", "").Replace("Maariv**##", "");
            return prayerTime;
        }
        public List<string> ReturnPrayersTime(string prayerTime)
        {
            var listTime = new List<string>() { "","",""};

            var listPrayers = prayerTime.Split("##").Where(a=>a!="").ToList();
            for(var i=0; i< listPrayers.Count(); i++)
            {
                var prayerResponse = listPrayers[i].Split("**").ToList();
                if (prayerResponse[1] != "")
                {
                    if (prayerResponse[0].Contains("Shacharit"))
                    {
                        listTime[0] = prayerResponse[1];
                    }
                    else if (prayerResponse[0].Contains("Mincha"))
                    {
                        listTime[1] = prayerResponse[1];
                    }
                    else
                    {
                        listTime[2] = prayerResponse[1];
                    }
                }
            }
            return listTime;
        }
        public List<List<string>> ReturnPrayersTime2(string prayerTime)
        {
            List<List<string>> listTime = new List<List<string>>();
            List<string> shacharit = new List<string>() { "","",""};
            List<string> mincha = new List<string>() { "", "", "" };
            List<string> maariv = new List<string>() { "", "", "" };
            listTime.Add(shacharit);
            listTime.Add(mincha);
            listTime.Add(maariv);

            var listPrayers = prayerTime.Split("##").Where(a => a != "").ToList();
            for (var i = 0; i < listPrayers.Count(); i++)
            {
                var prayerResponse = listPrayers[i].Split("**").ToList();
                if (prayerResponse[1] != "")
                {
                    List<string> times = prayerResponse[1].Split(",").ToList();
                    if (prayerResponse[0].Contains("Shacharit"))
                    {
                        for(var contador = 0; contador < times.Count(); contador++)
                        {
                            listTime[0][contador] = times[contador];
                        }
                    }
                    else if (prayerResponse[0].Contains("Mincha"))
                    {
                        for (var contador = 0; contador < times.Count(); contador++)
                        {
                            listTime[1][contador] = times[contador];
                        }
                    }
                    else
                    {
                        for (var contador = 0; contador < times.Count(); contador++)
                        {
                            listTime[2][contador] = times[contador];
                        }
                    }
                }
            }
            return listTime;
        }
        public List<List<string>> ReturnPrayersTimeDays(string prayerTime)
        {
            List<List<string>> listTime = new List<List<string>>();
            List<string> sunday = new List<string>();
            List<string> monday = new List<string>();
            List<string> tuesday = new List<string>();
            List<string> wednesday = new List<string>();
            List<string> thursday = new List<string>();
            List<string> friday = new List<string>();
            List<string> saturday = new List<string>();
            listTime.Add(sunday); listTime.Add(monday);
            listTime.Add(tuesday); listTime.Add(wednesday); 
            listTime.Add(thursday); listTime.Add(friday); listTime.Add(saturday);

            var listPrayers = prayerTime.Split("##").Where(a => a != "").ToList();
            var listPrayers3 = prayerTime.Split("##,").Where(a => a != "").ToList();
            for(var i = 0; i < listPrayers3.Count(); i++)
            {
                var time = listPrayers3[i].Split("$$");
                var sections = time[1].Split("##").ToList();
                foreach(var section in sections)
                {
                    if(section == "")
                    {
                        listTime[i].Add("");
                        listTime[i].Add("");
                        listTime[i].Add("");
                    }
                    else
                    {
                        var hours = section.Split("**").ToList();
                        var allhours = hours[1].Split("-").ToList();
                        listTime[i].Add(allhours.Count >= 1 ? allhours[0] : "");
                        listTime[i].Add(allhours.Count >= 2 ? allhours[1] : "");
                        listTime[i].Add(allhours.Count >= 3 ? allhours[2] : "");
                    }
                }
            }

            return listTime;
        }
        public string PrayerTime(string prayerTime)
        {
            prayerTime = prayerTime.Replace("--", "-").Replace("Shabbos**-##", "##")
                .Replace("Mincha**-##", "##").Replace("Maariv**-##", "##")
                .Replace("Shabbos**-##", "##").Replace("-##", "##").Replace("**-", "**");
            //Sunday$$Shacharit * *08:50 - 09:20####Maariv**16:11-20:20##,
            //Monday$$Shacharit **08:50##Mincha**14:52##Maariv**16:11##,
            //Tuesday$$Shacharit **08:50##Mincha**14:52##Maariv**16:11##,
            //Wendesday$$Shacharit **08:50##Mincha**14:52##Maariv**16:11##,
            //Thursday$$Shacharit **08:50##Mincha**14:52##Maariv**16:11##,
            //Friday$$Shacharit **08:50##Mincha**14:52##Maariv**16:11##,
            //Saturday$$Shacharit **08:50##Mincha**14:52##Maariv**16:11##
            return prayerTime;
        }
    }
}
