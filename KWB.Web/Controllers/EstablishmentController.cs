using KWB.Web.Models;
using KWB.Web.Models.NewFolder;
using KWB.Web.Models.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Runtime.ConstrainedExecution;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace KWB.Web.Controllers
{
    public class EstablishmentController : Controller
    {
        private readonly AplicationDbContext context;
        public EstablishmentController(AplicationDbContext context)
        {
            this.context = context;
        }
      
        #region Add views 
        public IActionResult AddPlace()
        {
            string status = HttpContext.Session.GetString("authstatus");
            string userID = HttpContext.Session.GetString("userID");
            if(status != "true")
            {
                return RedirectToAction("Login", "User");
            }
            UserChange newUserChange = new CommonFunctions().CreateUserChange(Int32.Parse(userID), "page add place", null);
            context.UserChange.Add(newUserChange);
            context.SaveChanges();

            List<Certification> certificados = context.Certification.OrderBy(a => a.Name).ToList();
            List<Tag> tags = context.Tag.OrderBy(a => a.TagName).ToList();
            List<string> locations = context.LocationTag.OrderBy(a => a.Name).Select(a => a.Name).ToList();
            List<Tag> markers = tags.Where(a => a.Marker != null && a.Marker.Length>0).ToList();

            ResponsePlace response = new();
            response.Markers = markers;
            response.Certifications = certificados;
            response.TagsDB = tags;
            response.LocationsDB = locations;
            response.TagsNotMainDB = tags.Where(a => a.Main == false && a.TagName != "minyan").Select(a => a.TagName).ToList();
            response.TagsMainDB = tags.Where(a => a.Main == true && a.TagName != "must_see!" && a.TagName != "Minyan").Select(a => a.TagName).ToList();
            return View(response);
        }
       
        [HttpPost]
        public async Task<IActionResult> AddPlace(ResponsePlace place)
        {
            if (place.Name == null) { return Json(new { status = "failed", message = "The place name cant be null" }); }
            if ((bool)place.Enable)
            {
                if (place.Latitude == null || place.Longitude == null) { return Json(new { status = "failed", message = "The place latitude and the lan cant be null" }); }
                if (place.PrincipalTag == null || place.PrincipalTag == "Select One") { return Json(new { status = "failed", message = "The place must have a principal Tag" }); }
                if (place.ImageFile == null && (place.GooglePictureMain == null || place.GooglePictureMain == "Choose file")) { return Json(new { status = "failed", message = "The place must have an image" }); }
            }
            
            var CurrentDirectory = Directory.GetCurrentDirectory();
            string filePath = CurrentDirectory + @"\wwwroot\img\upload\";
            try
            {
                string placeAndLocation = place.Name + " " + place.Location;
                placeAndLocation = placeAndLocation.Length > 100 ? placeAndLocation.Substring(0,100) : placeAndLocation;
                
                string userID = HttpContext.Session.GetString("userID");
                UserChange newUserChange = new CommonFunctions().CreateUserChange(Int32.Parse(userID), "page add place saving", placeAndLocation);
                context.UserChange.Add(newUserChange);
                context.SaveChanges();

                var places = context.Place.ToList();
                var certification = context.Certification.Where(a => a.Name == place.CertificationName).FirstOrDefault();
                var tagsDB = context.Tag.ToList();
                var caracteristicas = context.Caracteristicas.ToList();
                var citys = context.LocationTag.ToList();

                var locationsID = new List<int>();

                var existPlace = places.Where(a => a.Name == place.Name && a.Lat == place.Latitude && a.Lng == place.Longitude).FirstOrDefault();
                if (existPlace != null)
                {
                    return Json(new { status = "failed", message = "this place already exists" });
                }

                string[] RevName = place.AuthorName.Split("##").Where(a => !string.IsNullOrEmpty(a)).ToArray();
                string[] RevUrl = place.AuthorUrl.Split("##").Where(a => !string.IsNullOrEmpty(a)).ToArray();
                string[] RevRating = place.AuthorRating.Split("##").Where(a => !string.IsNullOrEmpty(a)).ToArray();
                string[] RevRelativeTime = place.AuthorRating.Split("##").Where(a => !string.IsNullOrEmpty(a)).ToArray();
                string[] RevText = place.Text.Split("##").Where(a => !string.IsNullOrEmpty(a)).ToArray();
                string[] RevTime = place.Time.Split("##").Where(a => !string.IsNullOrEmpty(a)).ToArray();

                string[] listCloseTime = place.ClosingTime != null ? place.ClosingTime.Split("##").Where(a => !string.IsNullOrEmpty(a)).ToArray() : new List<string>().ToArray();
                string[] listOpenTime = place.OpeningTime != null ? place.OpeningTime.Split("##").Where(a => !string.IsNullOrEmpty(a)).ToArray() : new List<string>().ToArray();
                
                var tags = place.Tags[0] == null ? new List<string>().ToList() : place.Tags[0].Split(",").Where(a => !string.IsNullOrEmpty(a)).ToList();
                var tagsNotMain = place.TagsNotMainDB[0] == null ? new List<string>().ToList() : place.TagsNotMainDB[0].Split(",").Where(a => !string.IsNullOrEmpty(a)).ToList();
                string[] locations = place.LocationsDB[0] == null ? new List<string>().ToArray() : place.LocationsDB[0].Split(",").Where((a => !string.IsNullOrEmpty(a))).ToArray();

                foreach (var location in locations)
                {
                    if (location != null)
                    {
                        locationsID.Add(citys.Where(a => a.Name == location).Select(a => a.CityID).FirstOrDefault());
                    }
                }
                
                if (place.Permanently_closed == true) { filePath = "http://ec2-44-202-212-224.compute-1.amazonaws.com/KWBfront/img/upload/b5f0da8d-ed39-44b6-b0dc-57273082a23f.jpg"; }
                else if (place.NotKosher == true) { filePath = "http://ec2-44-202-212-224.compute-1.amazonaws.com/KWBfront/img/upload/image_1670283072_mano%201%20Large.jpg"; }
                else if (place.Questionable == true) { filePath = "http://ec2-44-202-212-224.compute-1.amazonaws.com/KWBfront/img/upload/interrogacion-Large.jpg"; }
                else if (place.ImageFile != null && place.ImageFile.Length > 0)
                {
                    long unixTime = ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds();
                    filePath = filePath + "image_" + unixTime + "_";
                    filePath = string.Format("{0}{1}", filePath, place.ImageFile.FileName);
                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await place.ImageFile.CopyToAsync(stream);
                    }
                    filePath = "http://ec2-44-202-212-224.compute-1.amazonaws.com/KWBfront/img/upload/" + "image_" + unixTime + "_";
                    filePath = string.Format("{0}{1}", filePath, place.ImageFile.FileName).Replace(" ", "%20");
                }
                else { filePath = null; }

                if (place.GooglePictureMain != null && place.GooglePictureMain.Contains("maps.googleapis.com/maps/api/place/js/PhotoService.GetPhoto"))
                {
                    filePath = new CommonFunctions().FixPhotos(place.GooglePictureMain);
                    var newApiRegister = new CommonFunctions().CreateApiCallRegister("photos");
                    context.ApiCallsRegister.Add(newApiRegister);
                }
                
                List<int> cantReviews = new List<int> { RevName.Length, RevUrl.Length, RevRating.Length, RevRelativeTime.Length, RevRelativeTime.Length, RevText.Length, RevTime.Length };

                if (existPlace == null)
                {
                    var newEstablishment = new CommonFunctions().CreateEstablishment(place, filePath, certification);

                    context.Place.Add(newEstablishment);
                    context.SaveChanges();
                    existPlace = newEstablishment;
                }

                
                var newTipoTagMain = new CommonFunctions().CreateTipoTag(existPlace.PlaceID,null, tagsDB.Where(a => a.TagName == place.PrincipalTag).Select(a => a.TagID).FirstOrDefault(), true);
                context.TipoTag.Add(newTipoTagMain);
                context.SaveChanges();

                if (place.CantMiss == true)
                {
                    var newTipoTagCantMiss = new CommonFunctions().CreateTipoTag(existPlace.PlaceID,null, tagsDB.Where(a => a.TagName == "must_see!").Select(a => a.TagID).FirstOrDefault(), false);
                    context.TipoTag.Add(newTipoTagCantMiss);
                    context.SaveChanges();
                }
                if (place.NotKosher == true)
                {
                    var newTipoTagNotKosher = new CommonFunctions().CreateTipoTag(existPlace.PlaceID, null, tagsDB.Where(a => a.TagName == "NOT KOSHER").Select(a => a.TagID).FirstOrDefault(), false);
                    context.TipoTag.Add(newTipoTagNotKosher);
                    context.SaveChanges();
                }
                if (place.Questionable == true)
                {
                    var newTipoTagQuestionable = new CommonFunctions().CreateTipoTag(existPlace.PlaceID, null, tagsDB.Where(a => a.TagName == "Questionable").Select(a => a.TagID).FirstOrDefault(), false);
                    context.TipoTag.Add(newTipoTagQuestionable);
                    context.SaveChanges();
                }

                var allTags = tags.Concat(tagsNotMain).ToList();
                for (var i = 0; i < allTags.Count(); i++)
                {
                    var tag = tagsDB.Where(a => a.TagName == allTags[i]).Select(a => a.TagID).FirstOrDefault();
                    if (tag != 0)
                    {
                        var newTipoTag = new CommonFunctions().CreateTipoTag(existPlace.PlaceID,null, tag, false);
                        context.TipoTag.Add(newTipoTag);
                        context.SaveChanges();
                    }
                }
                //Adding PlaceLocationTag
                for (var i = 0; i < locationsID.Count(); i++)
                {
                    var newPlaceLocationTag = new CommonFunctions().CreatePlaceLocationTag(existPlace.PlaceID, locationsID[i]);
                    context.PlaceLocationTag.Add(newPlaceLocationTag);
                    context.SaveChanges();
                }

                //Adding Reviews
                for (var i = 0; i < cantReviews.Max(); i++)
                {
                    if (i < 5)
                    {
                        Review newReview = new Review
                        {
                            PlaceID = existPlace.PlaceID,
                            AuthorName = RevName[i] != null ? RevName[i] : "",
                            AuthorPhoto = RevUrl[i] != null ? RevUrl[i] : "",
                            Rating = Int32.Parse(RevRating?[i]),
                            Text = RevText.Count() > 0 ? RevText[i] : "",
                            Age = RevRelativeTime[i] != null ? RevRelativeTime[i] : "",
                            AgeInDays = Int32.Parse(RevTime?[i])
                        };
                        context.Review.Add(newReview);
                        context.SaveChanges();
                    }
                }

                //Rating
                if (!caracteristicas.Any(a => a.PlaceID == existPlace.PlaceID && a.Caracteristica == "rating") && place.Rating!= null && place.Rating!="")
                {
                    var newCaract = new CommonFunctions().CreateCaracteristica(existPlace.PlaceID, "rating", place.Rating);
                    context.Caracteristicas.Add(newCaract);
                    context.SaveChanges();
                }
                //PriceLevel
                if (!caracteristicas.Any(a => a.PlaceID == existPlace.PlaceID && a.Caracteristica == "price_level") && place.PriceLevel.ToString().Length > 0)
                {
                    var newCaract = new CommonFunctions().CreateCaracteristica(existPlace.PlaceID, "price_level", place.PriceLevel.ToString());
                    context.Caracteristicas.Add(newCaract);
                    context.SaveChanges();
                }
                //Opening Hours
                if (!caracteristicas.Any(a => a.PlaceID == existPlace.PlaceID && a.Caracteristica == "opening_hours"))
                {
                    listOpenTime = new CommonFunctions().CompleteOpeningHours(listOpenTime);
                    
                    var value = "" + listOpenTime[0] + "##" + listOpenTime[1] + "##" + listOpenTime[2] + "##" + listOpenTime[3] + "##" + listOpenTime[4] + "##" + listOpenTime[5] + "##" + listOpenTime[6];
                    var newCaract = new CommonFunctions().CreateCaracteristica(existPlace.PlaceID, "opening_hours", value);
                    context.Caracteristicas.Add(newCaract);
                    context.SaveChanges();
                }
                //Closing Hours
                if (!caracteristicas.Any(a => a.PlaceID == existPlace.PlaceID && a.Caracteristica == "closing_hours"))
                {
                    listCloseTime = new CommonFunctions().CompleteOpeningHours(listCloseTime);
                    
                    var value = "" + listCloseTime[0] + "##" + listCloseTime[1] + "##" + listCloseTime[2] + "##" + listCloseTime[3] + "##" + listCloseTime[4] + "##" + listCloseTime[5] + "##" + listCloseTime[6];
                    var newCaract = new CommonFunctions().CreateCaracteristica(existPlace.PlaceID, "opening_hours", value);
                    context.Caracteristicas.Add(newCaract);
                    context.SaveChanges();
                }

                //PrayerTime
                if(place.PrayersTime[0] != null)
                {
                    var prayerTimeDays = new CommonFunctions().PrayerTime(place.PrayersTime[0]);
                    //var prayerTime = new CommonFunctions().PrayersTime2(place.PrayersTime[0]);
                    if (!prayerTimeDays.Contains("NaN"))
                    {
                        var newCaract = new CommonFunctions().CreateCaracteristica(existPlace.PlaceID, "prayer_days", prayerTimeDays);
                        context.Caracteristicas.Add(newCaract);
                        context.SaveChanges();
                    }
                }

                //Photos
                var secundaryPhotosValue = "";

                if (place.Permanently_closed == true) { 
                    filePath = "http://ec2-44-202-212-224.compute-1.amazonaws.com/KWBfront/img/upload/b5f0da8d-ed39-44b6-b0dc-57273082a23f.jpg";
                    secundaryPhotosValue = secundaryPhotosValue + filePath + "##";
                }
                else if (place.GoogleSecundaryPicture != null && place.GoogleSecundaryPicture.Contains("maps.googleapis.com/maps/api/place/js/PhotoService.GetPhoto"))
                {
                    filePath = new CommonFunctions().FixPhotos(place.GoogleSecundaryPicture);
                    secundaryPhotosValue = secundaryPhotosValue + filePath + "##";
                    var newApiRegister = new CommonFunctions().CreateApiCallRegister("photos");
                    context.ApiCallsRegister.Add(newApiRegister);

                }
                else if (place.SecundaryPicture != null && place.SecundaryPicture.Length > 0)
                {
                    long unixtime = ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds();
                    filePath = CurrentDirectory + @"\wwwroot\img\upload\";
                    filePath = filePath + "image_" + unixtime + "_";
                    filePath = string.Format("{0}{1}", filePath, place.SecundaryPicture.FileName);
                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await place.SecundaryPicture.CopyToAsync(stream);
                    }
                    //filePath = "https://" + @"adminkwb.jojma.com.ar/img/upload/";
                    filePath = "http://ec2-44-202-212-224.compute-1.amazonaws.com/KWBfront/img/upload/" + "image_" + unixtime + "_";
                    filePath = string.Format("{0}{1}", filePath, place.SecundaryPicture.FileName).Replace(" ", "%20");
                    secundaryPhotosValue = secundaryPhotosValue + filePath + "##";

                }
               
                if (place.GoogleSecundaryPicture2 != null && place.GoogleSecundaryPicture2.Contains("maps.googleapis.com/maps/api/place/js/PhotoService.GetPhoto"))
                {
                    filePath = new CommonFunctions().FixPhotos(place.GoogleSecundaryPicture2);
                    secundaryPhotosValue = secundaryPhotosValue + filePath + "##";
                    var newApiRegister = new CommonFunctions().CreateApiCallRegister("photos");
                    context.ApiCallsRegister.Add(newApiRegister);
                }
                else if (place.SecundaryPicture2 != null && place.SecundaryPicture2.Length > 0)
                {
                    long unixtime = ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds();
                    filePath = CurrentDirectory + @"\wwwroot\img\upload\";
                    filePath = filePath + "image_" + unixtime + "_";
                    filePath = string.Format("{0}{1}", filePath, place.SecundaryPicture2.FileName);

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await place.SecundaryPicture2.CopyToAsync(stream);
                    }
                    //filePath = "https://" + @"adminkwb.jojma.com.ar/img/upload/";
                    filePath = "http://ec2-44-202-212-224.compute-1.amazonaws.com/KWBfront/img/upload/" + "image_" + unixtime + "_";

                    filePath = string.Format("{0}{1}", filePath, place.SecundaryPicture2.FileName).Replace(" ", "%20");
                    secundaryPhotosValue = secundaryPhotosValue + filePath + "##";
                }

                if (place.GoogleSecundaryPicture3 != null && place.GoogleSecundaryPicture3.Contains("maps.googleapis.com/maps/api/place/js/PhotoService.GetPhoto"))
                {
                    filePath = new CommonFunctions().FixPhotos(place.GoogleSecundaryPicture3);
                    secundaryPhotosValue = secundaryPhotosValue + filePath + "##";
                    var newApiRegister = new CommonFunctions().CreateApiCallRegister("photos");
                    context.ApiCallsRegister.Add(newApiRegister);
                }
                else if (place.SecundaryPicture3 != null && place.SecundaryPicture3.Length > 0)
                {
                    long unixtime = ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds();
                    filePath = CurrentDirectory + @"\wwwroot\img\upload\";
                    filePath = filePath + "image_" + unixtime + "_";
                    filePath = string.Format("{0}{1}", filePath, place.SecundaryPicture3.FileName);
                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await place.SecundaryPicture3.CopyToAsync(stream);
                    }
                    //filePath = "https://" + @"adminkwb.jojma.com.ar/img/upload/";
                    filePath = "http://ec2-44-202-212-224.compute-1.amazonaws.com/KWBfront/img/upload/" + "image_" + unixtime + "_";
                    filePath = string.Format("{0}{1}", filePath, place.SecundaryPicture3.FileName).Replace(" ", "%20");
                    secundaryPhotosValue = secundaryPhotosValue + filePath + "##";
                }

                if (filePath != "")
                {
                    var newCaract = new Caracteristicas()
                    {
                        PlaceID = existPlace.PlaceID,
                        Caracteristica = "photos",
                        Valor = secundaryPhotosValue
                    };
                    context.Caracteristicas.Add(newCaract);
                    context.SaveChanges();
                }

                if (place.ExtraPicture != null && place.ExtraPicture.Length > 0)
                {
                    var extraImages = place.ExtraPicture.Split("##").ToList();
                    var categories = place.Categories.Split("##").ToList();
                    for (var i = 0; i < extraImages.Count(); i++)
                    {
                        if (extraImages[i].Length > 0 && extraImages[i] != "Choose file" && extraImages[i].Contains("maps.googleapis.com/maps/api/place/js/PhotoService.GetPhoto"))
                        {
                            var newPhoto = new Photo()
                            {
                                PlaceID = existPlace.PlaceID,
                                Category = categories[i],
                                Url = new CommonFunctions().FixPhotos(extraImages[i])
                            };
                            context.Photo.Add(newPhoto);
                            context.SaveChanges();
                            var newApiRegister = new CommonFunctions().CreateApiCallRegister("photos");
                            context.ApiCallsRegister.Add(newApiRegister);
                        }
                    }
                }
                
                context.SaveChanges();
                return Json(new { status = "success" });
            }
            catch (Exception exc)
            {
                return Json(new { status = "error", message = exc.ToString() });
            }
        }
        public IActionResult Certifications()
        {
            string status = HttpContext.Session.GetString("authstatus");
            string userID = HttpContext.Session.GetString("userID");
            if(status != "true")
            {
                return RedirectToAction("Login", "User");
            }
            UserChange newUserChange = new CommonFunctions().CreateUserChange(Int32.Parse(userID), "page add certification", null);
            context.UserChange.Add(newUserChange);
            context.SaveChanges();

            var certificadosNames = context.Certification.Select(a => a.Name).ToList();
            ResponseCertifications response = new();
            response.CertNames = certificadosNames;
            return View(response);
        }
       
        [HttpPost]
        public async Task<IActionResult> Certifications(ResponseCertifications certification)
        {
            if (certification.Name == null)
            {
                return Json(new { status = "error", message = "The name cant be empty" });
            }
            bool existNameCertification = context.Certification.Any(a => a.Name == certification.Name);

            if (!existNameCertification)
            {
                var CurrentDirectory = Directory.GetCurrentDirectory();
                string filePath = CurrentDirectory + @"\wwwroot\img\upload\";
                var formFile = certification.Icono;

                var userID = HttpContext.Session.GetString("userID");
                UserChange newUserChange = new CommonFunctions().CreateUserChange(Int32.Parse(userID), "page add certification saving", certification.Name);
                context.UserChange.Add(newUserChange);
                context.SaveChanges();

                try
                {
                    if (formFile != null && formFile.Length > 0)
                    {
                        long unixTime = ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds();
                        filePath = filePath + "image_" + unixTime + "_";
                        filePath = string.Format("{0}{1}", filePath, formFile.FileName);
                        using (var stream = System.IO.File.Create(filePath))
                        {
                            await formFile.CopyToAsync(stream);
                        }
                        //filePath = "https://" + @"adminkwb.jojma.com.ar/img/upload/";
                        filePath = "http://ec2-44-202-212-224.compute-1.amazonaws.com/KWBfront/img/upload/" + "image_" + unixTime + "_";
                        filePath = string.Format("{0}{1}", filePath, formFile.FileName).Replace(" ","%20");
                    }
                    else { filePath = ""; }
                }
                catch (Exception exc)
                {
                    return Json(new { status = "error", message = exc.ToString() });
                }
                var newCert = new CommonFunctions().CreateCertification(certification.Name, certification.Website, filePath);

                context.Certification.Add(newCert);
                context.SaveChanges();
                return Json(new { status = "success" });
            }
            else
            {
                return Json(new { status = "error", message = "The CertName is already exist" });
            }
        }
        public IActionResult Eruv()
        {
            string status = HttpContext.Session.GetString("authstatus");
            string userID = HttpContext.Session.GetString("userID"); //861 user Eruv
            string username = HttpContext.Session.GetString("userName");
            if (status != "true")
            {
                return RedirectToAction("Login", "User");
            }
            if (username.ToLower() != "eruv")
            {
                return RedirectToAction("Index", "Home");
            }
            UserChange newUserChange = new CommonFunctions().CreateUserChange(Int32.Parse(userID), "page add eruv", null);
            context.UserChange.Add(newUserChange);
            context.SaveChanges();

            var listCoordinates = new List<List<string>>();
            return View(listCoordinates);
        }
        
        [HttpPost]
        public IActionResult Eruv(string Points,string? Name, string? Email, string? Phone, string? Comments, string? Website,string? Rabbinate, string? Posek,
            List<string>? Synagogues, string? HotlinePhone, string? City, string? Enable)
        {
            if (Points == null)
            {
                return Json(new { status = "error", message = "complete the points in the map" });
            }
            if(Enable == "true" && (Posek == null ||Name == null ))
            {
                return Json(new { status = "error", message = "complete all fields" });
            }
            try {
                string userID = HttpContext.Session.GetString("userID");
                UserChange newUserChange = new CommonFunctions().CreateUserChange(Int32.Parse(userID), "page add eruv saving", Name);
                context.UserChange.Add(newUserChange);
                context.SaveChanges();

                Points = Points.Replace("),(", "##").Replace("(", "").Replace(")", "").ToString();
                var eruvDB = context.Eruv.ToList();
                var existEruv = eruvDB.Where(a => a.Point == Points).FirstOrDefault();
                if (existEruv != null)
                {
                    return Json(new { status = "error", message = "this eruv already exists" });
                }
                else
                {
                    var synagoguesSelected = Synagogues[0] != null ? Synagogues[0].Split(",").ToList() : null;
                    //string.Join(",", strings);
                    var synagogues = synagoguesSelected != null ? string.Join("##",synagoguesSelected) : null;
                    var newEruv = new Eruv()
                    {
                        Comments = Comments,
                        Email = Email,
                        Rabbinate = Rabbinate,
                        Name = Name,
                        Phone = Phone,
                        Point = Points,
                        HotlinePhone=HotlinePhone,
                        City = City,
                        Posek=Posek,
                        Synagogues= synagogues,
                        Website=Website
                    };
                    context.Eruv.Add(newEruv);
                    context.SaveChanges();
                    return Json(new { status = "success" });
                }

                return Json(new { status = "error", message = "this eruv could not be saved " });
            }
            catch(Exception exc)
            {
                return Json(new { status = "error", message = exc.InnerException.ToString() });
            }
        }
        #endregion
       
        #region Show views
        public IActionResult ShowEruv()
        {
            var status = HttpContext.Session.GetString("authstatus");
            if(status != "true")
            {
                return RedirectToAction("Login", "User");
            }
            string userID = HttpContext.Session.GetString("userID");
            string username = HttpContext.Session.GetString("userName");
            if (username.ToLower() != "eruv")
            {
                return RedirectToAction("Index", "Home");
            }
            UserChange newUserChange = new CommonFunctions().CreateUserChange(Int32.Parse(userID), "page show eruv", null);
            context.UserChange.Add(newUserChange);
            context.SaveChanges();

            ResponseEruv response = new();
            var eruvs = context.Eruv.ToList();
            var synagogues = new List<string>();
            foreach(var eruv in eruvs)
            {
                eruv.Points = eruv.Point.Split("##").ToList();
                eruv.ListSynagogues = eruv.Synagogues == null ? new List<string>() : eruv.Synagogues.Split("##").ToList();
                if(eruv.Synagogues != null)
                {
                    var list = eruv.Synagogues.Split("##").ToList();
                    foreach(var synagogue in list)
                    {
                        synagogues.Add(synagogue);
                    }
                }

            }
            response.Eruvs = eruvs;
            response.AllSynagogues = synagogues;

            return View(response);
        }
        public IActionResult ShowReview()
        {
            string status = HttpContext.Session.GetString("authstatus");
            if(status != "true")
            {
                return RedirectToAction("Login", "User");
            }
            string userID = HttpContext.Session.GetString("userID");
            UserChange newUserChange = new CommonFunctions().CreateUserChange(Int32.Parse(userID), "page show review", null);
            context.UserChange.Add(newUserChange);
            context.SaveChanges();

            var reviews = context.Review.ToList();
            ResponseReview response = new();
            response.ReviewsDB = reviews;
            var places = context.Place.ToList();
            for (var i = 0; i < reviews.Count(); i++)
            {
                response.ReviewsDB[i].Place = places.Where(a => a.PlaceID == response.ReviewsDB[i].PlaceID).Select(a => a.Name).FirstOrDefault();
            }
            return View(response);
        }
        public IActionResult ShowPhotos()
        {
            string status = HttpContext.Session.GetString("authstatus");
            if(status != "true")
            {
                return RedirectToAction("Login", "User");
            }
            string userID = HttpContext.Session.GetString("userID");
            UserChange newUserChange = new CommonFunctions().CreateUserChange(Int32.Parse(userID), "page show photos", null);
            context.UserChange.Add(newUserChange);
            context.SaveChanges();

            var photos = context.Photo.ToList();
            var places = context.Place.ToList();
            foreach (var photo in photos)
            {
                var place = places.Where(a => a.PlaceID == photo.PlaceID).FirstOrDefault();
                photo.PlaceName = place == null ? null : place.Name;
            }
            return View(photos);
        }
        public IActionResult ShowCertifications()
        {
            string status = HttpContext.Session.GetString("authstatus");
            if(status != "true")
            {
                return RedirectToAction("Login", "User");
            }
            string userID = HttpContext.Session.GetString("userID");
            UserChange newUserChange = new CommonFunctions().CreateUserChange(Int32.Parse(userID), "page show certification", null);
            context.UserChange.Add(newUserChange);
            context.SaveChanges();

            var certifications = context.Certification.ToList();
            ResponseCertifications response = new();
            response.CertNames = certifications.Select(a => a.Name).ToList();
            response.CertificationsDB = certifications;
            return View(response);
        }
        public IActionResult ShowPlaces()
        {
            string status = HttpContext.Session.GetString("authstatus");
            if(status != "true")
            {
                return RedirectToAction("Login", "User");
            }
            string userID = HttpContext.Session.GetString("userID");
            UserChange newUserChange = new CommonFunctions().CreateUserChange(Int32.Parse(userID), "page show places", null);
            context.UserChange.Add(newUserChange);
            context.SaveChanges();

            var certificados = context.Certification.OrderBy(a => a.Name).ToList();
            var tags = context.Tag.OrderBy(a => a.TagName).ToList();
            var tipoTags = context.TipoTag.ToList();
            var caracteristicas = context.Caracteristicas.ToList();
            var locations = context.LocationTag.OrderBy(a => a.Name).Select(a => a.Name).ToList();

            ResponsePlace response = new();
            response.Certifications = certificados;
            response.TagsDB = tags;
            response.Markers = tags.Where(a=>a.Marker!=null && a.Marker.Length>0).ToList();
            response.TagsMainDB = tags.Where(a => a.Main == true).Select(a=>a.TagName).ToList();
            response.LocationsDB = locations;
            response.PlacesDB = context.Place.ToList();

            for (var i = 0; i < response.PlacesDB.Count(); i++)
            {
                var tagsID = tipoTags.Where(a => a.PlaceID == response.PlacesDB[i].PlaceID).Select(a => a.TagID).ToList();
                response.PlacesDB[i].Tags = new CommonFunctions().ReturnTag(tagsID,tags).Select(a=>a.TagName).ToList();
                response.PlacesDB[i].NumberOfTags = response.PlacesDB[i].Tags == null ? 0 : response.PlacesDB[i].Tags.Count();
                response.PlacesDB[i].CertificationName = response.PlacesDB[i].CertificationID != null ?
                    certificados.Where(a => a.CertificationID == response.PlacesDB[i].CertificationID).Select(a => a.Name).FirstOrDefault() : null;
                response.PlacesDB[i].SecundaryImages = caracteristicas.Where(a => a.PlaceID == response.PlacesDB[i].PlaceID && a.Caracteristica == "photos")
                    .Select(a => a.Valor).ToList();
            }
            return View(response);
        }
        
        #endregion

        #region Update functions
        [HttpPost]
        public async Task<IActionResult> UpdateEstablishment(ResponsePlace response)
        {
            try
            {
                if (response.Name == null) { return Json(new { status = "failed", message = "The place name cant be null" }); }
                if ((bool)response.Enable)
                {
                    if (response.Latitude == null || response.Longitude == null) { return Json(new { status = "failed", message = "The place latitude and the lan cant be null" }); }
                    if (response.ImageFile == null && response.GoogleMainPhoto == null) { return Json(new { status = "failed", message = "The place must have an image" }); }
                    if (response.TagsMainDB[0] == null) { return Json(new { status = "failed", message = "The place must have a main tag" }); }
                }

                string userID = HttpContext.Session.GetString("userID");
                UserChange newUserChange = new CommonFunctions().CreateUserChange(Int32.Parse(userID), "page show places saving", response.Name);
                context.UserChange.Add(newUserChange);
                context.SaveChanges();

                var placeID = Int32.Parse(response.ID);
                var tagsID = new List<int>();
                var locationsTagID = new List<int>();
                var deleteTipoTags = new List<TipoTag>();
                var establishment = context.Place.Where(a => a.PlaceID == placeID).FirstOrDefault();
                var caracteristicas = context.Caracteristicas.ToList();
                var locationsTagDB = context.LocationTag.ToList();
                var TagsDB = context.Tag.ToList();
                var placeLocationTag = context.PlaceLocationTag.ToList();
                var tipoTagDB = context.TipoTag.ToList();
                var oldTipoTags = tipoTagDB.Where(a => a.PlaceID == establishment.PlaceID).ToList();

                foreach (var oldTag in oldTipoTags)
                {
                    context.TipoTag.Remove(oldTag);
                    context.SaveChanges();
                }
                
                if (response.CantMiss == true)
                {
                    var newTipoTagCantMiss = new CommonFunctions().CreateTipoTag(establishment.PlaceID, null, TagsDB.Where(a => a.TagName == "must_see!").Select(a => a.TagID).FirstOrDefault(), false);
                    context.TipoTag.Add(newTipoTagCantMiss);
                    context.SaveChanges();
                }
                if (response.Questionable == true)
                {
                    var newTipoTagCantMiss = new CommonFunctions().CreateTipoTag(establishment.PlaceID, null, TagsDB.Where(a => a.TagName == "Questionable").Select(a => a.TagID).FirstOrDefault(), false);
                    context.TipoTag.Add(newTipoTagCantMiss);
                    context.SaveChanges();
                }
                if (response.NotKosher == true)
                {
                    var newTipoTagCantMiss = new CommonFunctions().CreateTipoTag(establishment.PlaceID, null, TagsDB.Where(a => a.TagName == "NOT KOSHER").Select(a => a.TagID).FirstOrDefault(), false);
                    context.TipoTag.Add(newTipoTagCantMiss);
                    context.SaveChanges();
                }
                if (response.Tags[0] != null)
                {
                    var tagsSelected = response.Tags[0].Split(",").ToList();
                    //Agrego nuevos tags
                    for (var i = 0; i < tagsSelected.Count(); i++)
                    {
                        if (!(tagsSelected[i] != null && response.TagsMainDB[0] != null && tagsSelected[i] == response.TagsMainDB[0]))
                        {
                            var tagID = TagsDB.Where(a => a.TagName == tagsSelected[i]).Select(a => a.TagID).FirstOrDefault();
                            if (tagID != 0) 
                            {
                                //If exist in DB add
                                TipoTag newTipoTag = new()
                                    {
                                        TagID = tagID,
                                        PlaceID = placeID,
                                        TipoID = 1,
                                        Main = false
                                    };
                                    context.TipoTag.Add(newTipoTag);
                                    context.SaveChanges();
                                }
                            }
                        }
                }
                if (response.TagsMainDB[0] != null)
                {
                    var tagMain = TagsDB.Where(a => a.TagName == response.TagsMainDB[0]).Select(a => a.TagID).FirstOrDefault();
                    if (tagMain!=0)
                    {
                        var newTipoTag = new TipoTag()
                        {
                            Main = true,
                            PlaceID = establishment.PlaceID,
                            TagID = tagMain
                        };
                        context.TipoTag.Add(newTipoTag);
                        context.SaveChanges();
                    }
                }

                if (response.CertificationName != null)
                {
                    var cert = context.Certification.Where(a => a.Name == response.CertificationName).FirstOrDefault();
                    if (cert != null)
                    {
                        establishment.CertificationID = cert.CertificationID;
                    }
                }
                if (response.PriceLevel != null)
                {
                    var priceLevel = context.Caracteristicas.Where(a => a.PlaceID == placeID && a.Caracteristica == "price_level").FirstOrDefault();
                    if (priceLevel != null)
                    {
                        priceLevel.Valor = response.PriceLevel.ToString();
                        context.Caracteristicas.Update(priceLevel);
                        context.SaveChanges();
                    }
                    else
                    {
                        Caracteristicas newCaract = new()
                        {
                            Caracteristica = "price_level",
                            Valor = response.PriceLevel.ToString(),
                            PlaceID = establishment.PlaceID,
                        };
                        context.Caracteristicas.Add(newCaract);
                        context.SaveChanges();
                    }
                }
                if (response.LocationsTag[0] != null)
                {
                    var locationsTag = response.LocationsTag[0].Split(",").ToList();
                    for (var i = 0; i < locationsTag.Count(); i++)
                    {
                        if (locationsTag[i] != null)
                        {
                            var locationTagID = locationsTagDB.Where(a => a.Name == locationsTag[i]).Select(a => a.CityID).FirstOrDefault();
                            if (locationTagID != 0)
                            {
                                locationsTagID.Add(locationTagID);
                                if (placeLocationTag.Where(a => a.LocationTagID == locationTagID && a.PlaceID == establishment.PlaceID).FirstOrDefault() == null)
                                {
                                    PlaceLocationTag newPlaceLocationTag = new()
                                    {
                                        LocationTagID = locationTagID,
                                        PlaceID = placeID
                                    };
                                    context.PlaceLocationTag.Add(newPlaceLocationTag);
                                    context.SaveChanges();
                                }
                            }
                        }
                    }
                    var oldLocationsTag = placeLocationTag.Where(a => a.PlaceID == placeID).ToList();
                    foreach (var oldLocationTag in oldLocationsTag)
                    {
                        if (oldLocationTag != null)
                        {
                            if (locationsTagID.IndexOf((int)oldLocationTag.LocationTagID) == -1)
                            {
                                context.PlaceLocationTag.Remove(oldLocationTag);
                                context.SaveChanges();
                            }
                        }
                    }
                }

                if (response.Permanently_closed == true) { 
                    string filePath = "http://ec2-44-202-212-224.compute-1.amazonaws.com/KWBfront/img/upload/b5f0da8d-ed39-44b6-b0dc-57273082a23f.jpg";
                    establishment.Photo = filePath;
                }
                else if (response.Questionable == true)
                {
                    string filePath = "http://ec2-44-202-212-224.compute-1.amazonaws.com/KWBfront/img/upload/interrogacion-Large.jpg";
                    establishment.Photo = filePath;
                }
                else if (response.NotKosher == true)
                {
                    string filePath = "http://ec2-44-202-212-224.compute-1.amazonaws.com/KWBfront/img/upload/image_1670283072_mano%201%20Large.jpg";
                    establishment.Photo = filePath;
                }
                else if (response.ImageFile != null && response.ImageFile.Length > 0)
                {
                    try
                    {
                        long unixTime = ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds();
                        var CurrentDirectory = Directory.GetCurrentDirectory();
                        string filePath = CurrentDirectory + @"\wwwroot\img\upload\" + "image_" + unixTime + "_";
                        filePath = string.Format("{0}{1}", filePath, response.ImageFile.FileName);
                        using (var stream = System.IO.File.Create(filePath))
                        {
                            await response.ImageFile.CopyToAsync(stream);
                        }
                        //filePath = "https://" + @"adminkwb.jojma.com.ar/img/upload/";
                        filePath = "http://ec2-44-202-212-224.compute-1.amazonaws.com/KWBfront/img/upload/" + "image_" + unixTime + "_";
                        filePath = string.Format("{0}{1}", filePath, response.ImageFile.FileName).Replace(" ", "%20");
                        establishment.Photo = filePath;
                    }
                    catch (Exception exc)
                    {
                        return Json(new { status = "failed", message = "The image selected could not save" });
                    }
                }

                var prayer = caracteristicas.Where(a => a.PlaceID == establishment.PlaceID && a.Caracteristica == "prayer_days").FirstOrDefault();
                if (prayer != null && response.PrayersTime[0] == null)
                {
                    context.Caracteristicas.Remove(prayer);
                    context.SaveChanges();
                }
                else if (prayer != null && response.PrayersTime[0] != null)
                {
                    var newPrayerTime = new CommonFunctions().PrayerTime(response.PrayersTime[0]);

                    if (!newPrayerTime.Contains("NaN"))
                    {
                        prayer.Valor = newPrayerTime;
                        context.Caracteristicas.Update(prayer);
                        context.SaveChanges();
                    }
                }
                else if(response.PrayersTime[0] != null)
                {
                    var newPrayerTime = new CommonFunctions().PrayerTime(response.PrayersTime[0]);
                    if (!newPrayerTime.Contains("NaN"))
                    {
                        var newCaract = new CommonFunctions().CreateCaracteristica(placeID, "prayer_days", newPrayerTime);
                        context.Caracteristicas.Add(newCaract);
                        context.SaveChanges();
                    }
                    
                }

                var secundaryPhotosValue = "";

                if (response.Permanently_closed == true)
                {
                    string filePath = "http://ec2-44-202-212-224.compute-1.amazonaws.com/KWBfront/img/upload/b5f0da8d-ed39-44b6-b0dc-57273082a23f.jpg";
                    secundaryPhotosValue = secundaryPhotosValue + filePath + "##";
                }
                else if (response.SecundaryPicture != null && response.SecundaryPicture.Length > 0)
                {
                    try
                    {
                        long unixTime = ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds();
                        var CurrentDirectory = Directory.GetCurrentDirectory();
                        string filePath = CurrentDirectory + @"\wwwroot\img\upload\" + "image_" + unixTime + "_";
                        filePath = string.Format("{0}{1}", filePath, response.SecundaryPicture.FileName);

                        using (var stream = System.IO.File.Create(filePath))
                        {
                            await response.SecundaryPicture.CopyToAsync(stream);
                        }
                        //filePath = "https://" + @"adminkwb.jojma.com.ar/img/upload/";
                        filePath = "http://ec2-44-202-212-224.compute-1.amazonaws.com/KWBfront/img/upload/" + "image_" + unixTime + "_";
                        filePath = string.Format("{0}{1}", filePath, response.SecundaryPicture.FileName).Replace(" ", "%20");
                        secundaryPhotosValue = secundaryPhotosValue + filePath + "##";
                    }
                    catch (Exception exc)
                    {

                    }
                }
                else if(response.SecundaryPictureUrl != null && response.SecundaryPictureUrl != "Choose file")
                {
                    secundaryPhotosValue = secundaryPhotosValue + response.SecundaryPictureUrl + "##";
                }
                
                if (response.SecundaryPicture2 != null && response.SecundaryPicture2.Length > 0)
                {
                    try
                    {
                        long unixTime = ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds();
                        var CurrentDirectory = Directory.GetCurrentDirectory();
                        string filePath = CurrentDirectory + @"\wwwroot\img\upload\" + "image_" + unixTime + "_";
                        filePath = string.Format("{0}{1}", filePath, response.SecundaryPicture2.FileName);
                        using (var stream = System.IO.File.Create(filePath))
                        {
                            await response.SecundaryPicture2.CopyToAsync(stream);
                        }
                        //filePath = "https://" + @"adminkwb.jojma.com.ar/img/upload/";
                        filePath = "http://ec2-44-202-212-224.compute-1.amazonaws.com/KWBfront/img/upload/" + "image_" + unixTime + "_";
                        filePath = string.Format("{0}{1}", filePath, response.SecundaryPicture2.FileName).Replace(" ", "%20");
                        secundaryPhotosValue = secundaryPhotosValue + filePath + "##";
                    }
                    catch (Exception exc)
                    {

                    }
                }
                else if (response.SecundaryPicture2Url != null && response.SecundaryPicture2Url != "Choose file")
                {
                    secundaryPhotosValue = secundaryPhotosValue + response.SecundaryPicture2Url + "##";
                }
                
                if (response.SecundaryPicture3 != null && response.SecundaryPicture3.Length > 0)
                {
                    try
                    {
                        long unixTime = ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds();
                        var CurrentDirectory = Directory.GetCurrentDirectory();
                        string filePath = CurrentDirectory + @"\wwwroot\img\upload\" + "image_" + unixTime + "_";
                        filePath = string.Format("{0}{1}", filePath, response.SecundaryPicture3.FileName);
                        using (var stream = System.IO.File.Create(filePath))
                        {
                            await response.SecundaryPicture3.CopyToAsync(stream);
                        }
                        //filePath = "https://" + @"adminkwb.jojma.com.ar/img/upload/";
                        filePath = "http://ec2-44-202-212-224.compute-1.amazonaws.com/KWBfront/img/upload/" + "image_" + unixTime + "_";
                        filePath = string.Format("{0}{1}", filePath, response.SecundaryPicture3.FileName).Replace(" ", "%20");
                        secundaryPhotosValue = secundaryPhotosValue + filePath;
                    }
                    catch (Exception exc)
                    {

                    }
                }
                else if (response.SecundaryPicture3Url != null && response.SecundaryPicture3Url != "Choose file")
                {
                    secundaryPhotosValue = secundaryPhotosValue + response.SecundaryPicture3Url;
                }

                if(secundaryPhotosValue != "")
                {
                    var existCaract = caracteristicas.Where(a=>a.PlaceID == Int32.Parse(response.ID) && a.Caracteristica == "photos").FirstOrDefault();
                    if (existCaract != null)
                    {
                        existCaract.Valor = secundaryPhotosValue;
                        context.Caracteristicas.Update(existCaract);
                        context.SaveChanges();
                    }
                    else
                    {
                        var newCaract = new Caracteristicas()
                        {
                            PlaceID = placeID,
                            Caracteristica = "photos",
                            Valor = secundaryPhotosValue
                        };
                        context.Caracteristicas.Add(newCaract);
                        context.SaveChanges();
                    }
                }

                establishment.Name = response.Name;
                establishment.Lat = response.Latitude;
                establishment.Lng = response.Longitude;
                establishment.Address = response.Location;
                establishment.TagMarkerID = (response.MarkerID == "0" || response.MarkerID == null ) ? null : Int32.Parse(response.MarkerID);
                establishment.Phone = response.Phone == null ? "" : response.Phone;
                establishment.Website = response.Website == null ? "" : response.Website;
                establishment.About = response.About == null ? "" : response.About;
                establishment.ZipCode = response.ZipCode == null ? "" : response.ZipCode;
                establishment.Permanently_closed = response.Permanently_closed; 
                establishment.Enable = response.Enable;
                context.Place.Update(establishment);
                context.SaveChanges();

                return Json(new { status = "success" });
            }
            catch (Exception exc)
            {
                return Json(new { status = "failed", message = "The establishment could not update" });
            }
        }
        [HttpPost]
        public IActionResult UpdateEruv(Eruv eruv)
        {
            try
            {
                var eruvsP = eruv.Point.Replace("),(", "##").Replace("(","").Replace(")","").ToString();
                var existEruv = context.Eruv.Where(a => a.EruvID == eruv.EruvID).FirstOrDefault();
                if (existEruv != null)
                {
                    if ((bool)eruv.Enable && (eruv.Posek == null || eruv.Name == null))
                    {
                        return Json(new { status = "failed", message = "complete all fields" });
                    }

                    string userID = HttpContext.Session.GetString("userID");
                    UserChange newUserChange = new CommonFunctions().CreateUserChange(Int32.Parse(userID), "page show eruv saving", eruv.Name);
                    context.UserChange.Add(newUserChange);
                    context.SaveChanges();

                    existEruv.Point = eruvsP;
                    existEruv.Name = eruv.Name;
                    existEruv.Website = eruv.Website;
                    existEruv.Rabbinate = eruv.Rabbinate;
                    existEruv.Phone = eruv.Phone;
                    existEruv.HotlinePhone = eruv.HotlinePhone;
                    existEruv.Email = eruv.Email;
                    existEruv.Posek = eruv.Posek;
                    existEruv.Comments = eruv.Comments;
                    existEruv.City = eruv.City;
                    existEruv.Enable = eruv.Enable;
                    if (eruv.ListSynagogues[0] != null)
                    {
                        var listSynagogues = eruv.ListSynagogues[0].Split(",").ToList();
                        existEruv.Synagogues = string.Join("##", listSynagogues);
                    }
                    context.Eruv.Update(existEruv);
                    context.SaveChanges();
                    return Json(new { status = "success" });
                }
                else { return Json(new { status = "failed", message = "the eruv was not found" }); }
            }
            catch (Exception exc)
            {
                return Json(new { status = "failed", message = exc.InnerException.ToString() });
            }
        }
        [HttpPost]
        public IActionResult UpdatePhoto(int id, string category)
        {
            try
            {
                var existPhoto = context.Photo.Where(a => a.PhotoID == id).FirstOrDefault();
                if (existPhoto != null && category != null)
                {
                    existPhoto.Category = category;
                    context.Photo.Update(existPhoto);
                    context.SaveChanges();

                    var userID = HttpContext.Session.GetString("userID");
                    UserChange newUserChange = new CommonFunctions().CreateUserChange(Int32.Parse(userID), "page show photos saving", existPhoto.PlaceID + " " + existPhoto.Category);
                    context.UserChange.Add(newUserChange);
                    context.SaveChanges();

                    return Json(new { status = "success" });
                }
                else { return Json(new { status = "failed", message = "the photo must have category" }); }
            }
            catch (Exception exc)
            {
                return Json(new { status = "failed", message = exc.InnerException.ToString() });
            }
        }
        [HttpPost]
        public async Task<IActionResult> UpdateCert(ResponseCertifications response)
        {
            try
            {
                if (response.Name == null || response.Website == null)
                {
                    return Json(new { status = "failed", message = "Complete all fields" });
                }

                var userID = HttpContext.Session.GetString("userID");
                UserChange newUserChange = new CommonFunctions().CreateUserChange(Int32.Parse(userID), "page show certification saving", response.Name);
                context.UserChange.Add(newUserChange);
                context.SaveChanges();

                var cert = context.Certification.Where(a => a.CertificationID == response.CertificationID).FirstOrDefault();
                if (response.Icono != null && response.Icono.Length > 0)
                {
                    long unixTime = ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds();
                    var CurrentDirectory = Directory.GetCurrentDirectory();
                    string filePath = CurrentDirectory + @"\wwwroot\img\upload\" + "image_" + unixTime + "_";
                    filePath = string.Format("{0}{1}", filePath, response.Icono.FileName);
                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await response.Icono.CopyToAsync(stream);
                    }
                    //filePath = "https://" + @"adminkwb.jojma.com.ar/img/upload/";
                    filePath = "http://ec2-44-202-212-224.compute-1.amazonaws.com/KWBfront/img/upload/" + "image_" + unixTime + "_";
                    filePath = string.Format("{0}{1}", filePath, response.Icono.FileName).Replace(" ","%20");
                    cert.Icono = filePath;
                }
                cert.Name = response.Name;
                cert.Website = response.Website;
                context.Certification.Update(cert);
                context.SaveChanges();
                return Json(new { status = "success" });
            }
            catch (Exception exc)
            {
                return Json(new { status = "failed", message = "The cert could not save" });
            }
        }
        #endregion
          
        #region Return functions
        
        [HttpPost]
        public IActionResult ReturnEstablishment(int id)
        {
            try
            {
                var place = context.Place.Where(a => a.PlaceID == id).FirstOrDefault();

                var userID = HttpContext.Session.GetString("userID");
                UserChange newUserChange = new CommonFunctions().CreateUserChange(Int32.Parse(userID), "page show place editing", place.Name);
                context.UserChange.Add(newUserChange);
                context.SaveChanges();

                var certificados = context.Certification.OrderBy(a => a.Name).ToList();
                var tags = context.Tag.OrderBy(a => a.TagName).ToList();
                var locations = context.LocationTag.OrderBy(a => a.Name).ToList();
                var caracteristicas = context.Caracteristicas.ToList();
                var tipoTags = context.TipoTag.Where(a => a.PlaceID == place.PlaceID).ToList();
                var locationTagID = context.PlaceLocationTag.Where(a => a.PlaceID == place.PlaceID).Select(a => a.LocationTagID).Distinct().ToList();

                var tipoTagMain = tipoTags.Where(a => a.Main == true).FirstOrDefault();
                var tagsID = tipoTags.Where(a => a.Main == false).Select(a => a.TagID).Distinct().ToList();

                var tagResponse = new List<Tag>();
                var tagsMain = new List<string>();
                var locationTagResponse = new List<string>();

                for (var i = 0; i < tagsID.Count(); i++)
                {
                    var tag = tags.Where(a => a.TagID == tagsID[i]).FirstOrDefault();
                    if (tag != null)
                    {
                        tagResponse.Add(tag);
                    }
                }
                for (var i = 0; i < locationTagID.Count(); i++)
                {
                    locationTagResponse.Add(locations.Where(a => a.CityID == locationTagID[i]).Select(a => a.Name).FirstOrDefault());
                }
                
                place.PriceLevel = caracteristicas.Where(a => a.PlaceID == id && a.Caracteristica == "price_level").Select(a => a.Valor).FirstOrDefault();
                place.Type = "food";
                place.CertificationName = certificados.Where(a => a.CertificationID == place.CertificationID).Select(a => a.Name).FirstOrDefault();
                place.LocationsTag = locationTagResponse;
                place.Tags = (tagResponse != null && tagResponse.Count()>0 && tagResponse[0] != null ) ? tagResponse.Where(a=>a.TagName != "must_see!" && a.TagName != "Questionable" && a.TagName != "NOT KOSHER").Select(a => a.TagName).ToList() : null;
                place.TagMain = tipoTagMain == null ? null : tags.Where(a=>a.TagID == tipoTagMain.TagID).Select(a=>a.TagName).FirstOrDefault();
                var secundaryImages = caracteristicas.Where(a => a.PlaceID == place.PlaceID && a.Caracteristica == "photos").FirstOrDefault();
                bool existTagCantMiss = tagResponse.Where(a => a.TagName == "must_see!").FirstOrDefault() != null;
                bool existTagQuestionable = tagResponse.Where(a => a.TagName == "Questionable").FirstOrDefault() != null;
                bool existTagNOTKOSHER = tagResponse.Where(a => a.TagName == "NOT KOSHER").FirstOrDefault() != null;


                ResponsePlace response = new();
                //var prayersTime = caracteristicas.Where(a => a.PlaceID == id && a.Caracteristica == "prayer_time").FirstOrDefault();
                var prayersTimeDays = caracteristicas.Where(a => a.PlaceID == id && a.Caracteristica == "prayer_days").FirstOrDefault();
                //var prayerTime = new CommonFunctions().ReturnPrayersTimeDays(prayersTimeDays.Valor);
                //response.PrayersTime2 = prayersTime == null ? new List<List<string>>() : new CommonFunctions().ReturnPrayersTime2(prayersTime.Valor);
                response.PrayersTime2 = prayersTimeDays == null ? new List<List<string>>() : new CommonFunctions().ReturnPrayersTimeDays(prayersTimeDays.Valor);
                response.PlaceDB = place;
                response.Certifications = certificados;
                response.TagsDB = tags.OrderBy(a => a.TagName).ToList();
                response.CantMiss = (tagResponse != null && tagResponse.Count() > 0 && tagResponse[0] != null && existTagCantMiss) ? true : false;
                response.Questionable = (tagResponse != null && tagResponse.Count() > 0 && tagResponse[0] != null && existTagQuestionable) ? true : false;
                response.NotKosher = (tagResponse != null && tagResponse.Count() > 0 && tagResponse[0] != null && existTagNOTKOSHER) ? true : false;
                response.TagsMainDB = tags.Where(a=>a.Main==true).OrderBy(a => a.TagName).Select(a => a.TagName).ToList();
                response.LocationsDB = locations.Select(a => a.Name).ToList();
                if (secundaryImages != null) {
                    var photos = secundaryImages.Valor.Split("##").ToList().Where(a => a != "").ToList();
                    response.SecundaryPictureUrl = photos.Count() > 0 ? photos[0] : null;
                    response.SecundaryPicture2Url = photos.Count() > 1 ? photos[1] : null;
                    response.SecundaryPicture3Url = photos.Count() > 2 ? photos[2] : null;
                    response.SecundaryPictureID = secundaryImages.PlaceID;
                }
                //response.PlaceDB.Photo = new CommonFunctions().CheckUrl(response.PlaceDB.Photo) ? response.PlaceDB.Photo : null;
                return Json(new { status = "success", data = response });
            }
            catch (Exception exc)
            {
                return Json(new { status = "failed", message = exc.Message.ToString() });
            }
        }
        [HttpPost]
        public IActionResult ReturnEruv(int id)
        {
            try
            {
                var eruv = context.Eruv.Where(a => a.EruvID == id).FirstOrDefault();

                var userID = HttpContext.Session.GetString("userID");
                UserChange newUserChange = new CommonFunctions().CreateUserChange(Int32.Parse(userID), "page show eruv editing", eruv.Name);
                context.UserChange.Add(newUserChange);
                context.SaveChanges();

                eruv.ListSynagogues = new List<string>();
                eruv.ListSynagogues = eruv.Synagogues == null ? new List<string>() : eruv.Synagogues.Split("##").ToList();

                List<Eruvs> eruvMapResponse = new List<Eruvs>();
                var newEruv = new Eruvs();
                newEruv.EruvCoordinates = new List<EruvCoordinates>();
                if (eruv.Point != null)
                {
                    var coordinates = eruv.Point.Split("##").ToList();
                    foreach (var point in coordinates)
                    {
                        var points = point.Split(",").ToList();
                        var eruvCoordinates = new EruvCoordinates()
                        {
                            
                            Lat = decimal.Parse(points[0].Replace("\r\n", "").Replace(@"\s", "").Replace(",", "."), CultureInfo.InvariantCulture),
                            Lng = decimal.Parse(points[1].Replace("\r\n", "").Replace(@"\s", "").Replace(",", "."), CultureInfo.InvariantCulture)
                        };
                        newEruv.EruvCoordinates.Add(eruvCoordinates);
                    }
                }
                eruv.EruvCoordinates = newEruv.EruvCoordinates;
                return Json(new { status = "success", data = eruv });
            }
            catch (Exception exc)
            {
                return Json(new { status = "failed", message = exc.Message.ToString() });
            }
        }
        [HttpPost]
        public IActionResult ReturnPhoto(int id)
        {
            try
            {
                var photo = context.Photo.Where(a => a.PhotoID == id).FirstOrDefault();
                var placeName = context.Place.Where(a => a.PlaceID == photo.PlaceID).Select(a => a.Name).FirstOrDefault();
                photo.PlaceName = placeName;

                var userID = HttpContext.Session.GetString("userID");
                UserChange newUserChange = new CommonFunctions().CreateUserChange(Int32.Parse(userID), "page show photos editing", placeName);
                context.UserChange.Add(newUserChange);
                context.SaveChanges();

                return Json(new { status = "success", data = photo });
            }
            catch (Exception exc)
            {
                return Json(new { status = "failed", message = exc.InnerException.ToString() });
            }
        }
        [HttpPost]
        public IActionResult ReturnCert(int id)
        {
            try
            {
                var cert = context.Certification.Where(a => a.CertificationID == id).FirstOrDefault();

                var userID = HttpContext.Session.GetString("userID");
                UserChange newUserChange = new CommonFunctions().CreateUserChange(Int32.Parse(userID), "page show certification editing", cert.Name);
                context.UserChange.Add(newUserChange);
                context.SaveChanges();

                return Json(new { status = "success", data = cert });
            }
            catch (Exception exc)
            {
                return Json(new { status = "failed", message = exc.Message.ToString() });
            }
        }
        [HttpPost]
        public IActionResult ReturnEruvCoordinates()
        {
            try
            {
                var eruvs = context.Eruv.ToList();
                List<Eruvs> eruvsResponse = new List<Eruvs>();
                
                foreach (var eruv in eruvs)
                {
                    List<Eruvs> eruvMapResponse = new List<Eruvs>();
                    var coordinates = eruv.Point.Split("##").ToList();
                    var eruvData = new Eruv()
                    {
                        Name = eruv.Name,
                        Email = eruv.Email,
                        Phone = eruv.Phone,
                        Website = eruv.Website,
                        Rabbinate = eruv.Rabbinate,
                        Comments = eruv.Comments,
                        City = eruv.City,
                        HotlinePhone = eruv.HotlinePhone,
                        Posek = eruv.Posek,
                        Enable = eruv.Enable
                    };
                    var newEruv = new Eruvs();
                    newEruv.Eruv = eruvData;
                    newEruv.EruvCoordinates = new List<EruvCoordinates>();
                    foreach (var point in coordinates)
                    {
                        var points = point.Split(",").ToList();
                        var eruvCoordinates = new EruvCoordinates()
                        {
                            Lat = decimal.Parse(points[0].Replace("\r\n", "").Replace(@"\s", "").Replace(",", "."), CultureInfo.InvariantCulture),
                            Lng = decimal.Parse(points[1].Replace("\r\n", "").Replace(@"\s", "").Replace(",", "."), CultureInfo.InvariantCulture) 
                        };
                        newEruv.EruvCoordinates.Add(eruvCoordinates);
                    }
                    eruvsResponse.Add(newEruv);
                }
                
                return Json(new { status = "success", data = eruvsResponse });
            }
            catch (Exception exc)
            {
                return Json(new { status = "failed", message = "The value is too long" });
            }
        }
        public void Ret()
        {
            try
            {
                var eruvs = context.Eruv.ToList();
                List<Eruvs> eruvsResponse = new List<Eruvs>();

                foreach (var eruv in eruvs)
                {
                    List<Eruvs> eruvMapResponse = new List<Eruvs>();
                    var coordinates = eruv.Point.Split("##").ToList();
                    var eruvData = new Eruv()
                    {
                        Name = eruv.Name,
                        Email = eruv.Email,
                        Phone = eruv.Phone,
                        Website = eruv.Website,
                        Rabbinate = eruv.Rabbinate,
                        Comments = eruv.Comments,
                        City = eruv.City,
                        HotlinePhone = eruv.HotlinePhone,
                        Posek = eruv.Posek,
                        Enable = eruv.Enable
                    };
                    var newEruv = new Eruvs();
                    newEruv.Eruv = eruvData;
                    newEruv.EruvCoordinates = new List<EruvCoordinates>();
                    foreach (var point in coordinates)
                    {
                        var points = point.Split(",").ToList();

                        var eruvCoordinates = new EruvCoordinates()
                        {
                            Lat = decimal.Parse(points[0].Replace("\r\n", "").Replace(@"\s", "").Replace(",", "."), CultureInfo.InvariantCulture),
                            Lng = decimal.Parse(points[1].Replace("\r\n", "").Replace(@"\s", "").Replace(",", "."), CultureInfo.InvariantCulture)
                        };
                        newEruv.EruvCoordinates.Add(eruvCoordinates);
                    }
                    eruvsResponse.Add(newEruv);
                }
            }
            catch (Exception exc)
            {
            }
        }
        #endregion
       
        #region Delete functions

        [HttpPost]
        public IActionResult DeletePlace(int id)
        {
            try
            {
                var place = context.Place.Where(a => a.PlaceID == id).FirstOrDefault();
                //place.Deleted = true;
                //context.Place.Update(place);
                //context.SaveChanges();
                var newCaract = new Caracteristicas
                {
                    Caracteristica = "delete",
                    PlaceID = place.PlaceID,
                    Valor = null
                };
                context.Caracteristicas.Add(newCaract);
                context.SaveChanges();

                var userID = HttpContext.Session.GetString("userID");
                UserChange newUserChange = new CommonFunctions().CreateUserChange(Int32.Parse(userID), "page show places removing", place.Name);
                context.UserChange.Add(newUserChange);
                context.SaveChanges();

                var tipoTags = context.TipoTag.Where(a => a.PlaceID == id).ToList();
                var caracts = context.Caracteristicas.Where(a => a.PlaceID == id).ToList();
                var favs = context.Favorite.Where(a => a.PlaceID == id).ToList();
                var photos = context.Photo.Where(a => a.PlaceID == id).ToList();
                var placeLocationTags = context.PlaceLocationTag.Where(a => a.PlaceID == id).ToList();
                var prayersTime = context.PrayerTime.Where(a => a.PlaceID == id).ToList();
                var reviews = context.Review.Where(a => a.PlaceID == id).ToList();
                var userReports = context.UserReport.Where(a => a.PlaceID == id).ToList();//101225
                var changesHistory = context.ChangesHistory.Where(a => a.PlaceID == id).ToList();

                foreach (var userReport in userReports)
                {
                    context.UserReport.Remove(userReport);
                    context.SaveChanges();
                }
                foreach (var tipoTag in tipoTags)
                {
                    context.TipoTag.Remove(tipoTag);
                    context.SaveChanges();
                }
                foreach (var caract in caracts)
                {
                    context.Caracteristicas.Remove(caract);
                    context.SaveChanges();
                }
                foreach (var fav in favs)
                {
                    context.Favorite.Remove(fav);
                    context.SaveChanges();
                }
                foreach (var photo in photos)
                {
                    context.Photo.Remove(photo);
                    context.SaveChanges();
                }
                foreach (var placeLocationTag in placeLocationTags)
                {
                    context.PlaceLocationTag.Remove(placeLocationTag);
                    context.SaveChanges();
                }
                foreach (var prayerTime in prayersTime)
                {
                    context.PrayerTime.Remove(prayerTime);
                    context.SaveChanges();
                }
                foreach (var review in reviews)
                {
                    context.Review.Remove(review);
                    context.SaveChanges();
                }
                foreach (var userReport in userReports)
                {
                    context.UserReport.Remove(userReport);
                    context.SaveChanges();
                }
                foreach (var changeHistory in changesHistory)
                {
                    context.ChangesHistory.Remove(changeHistory);
                    context.SaveChanges();
                }

                context.Place.Remove(place);
                context.SaveChanges();
                return Json(new { status = "success" });
            }
            catch (Exception exc)
            {
                return Json(new { status = "failed", message = "The place could not remove" });
            }
        }
        [HttpPost]
        public IActionResult DeleteEruv(int id)
        {
            try
            {
                var eruv = context.Eruv.Where(a => a.EruvID == id).FirstOrDefault();
                
                context.Eruv.Remove(eruv);
                context.SaveChanges();

                var userID = HttpContext.Session.GetString("userID");
                UserChange newUserChange = new CommonFunctions().CreateUserChange(Int32.Parse(userID), "page show eruv removing", eruv.Name);
                context.UserChange.Add(newUserChange);
                context.SaveChanges();

                return Json(new { status = "success" });
            }
            catch (Exception exc)
            {
                return Json(new { status = "failed", message = "The eruv could not remove" });
            }
        }
        [HttpPost]
        public IActionResult DeleteReview(int id)
        {
            try
            {
                var review = context.Review.Where(a => a.ReviewID == id).FirstOrDefault();
                context.Review.Remove(review);
                context.SaveChanges();
                Place place = context.Place.Where(a => a.PlaceID == review.PlaceID).FirstOrDefault();

                string userID = HttpContext.Session.GetString("userID");
                UserChange newUserChange = new CommonFunctions().CreateUserChange(Int32.Parse(userID), "page show review removing", place.Name + " " + review.AuthorName);
                context.UserChange.Add(newUserChange);
                context.SaveChanges();

                return Json(new { status = "success" });
            }
            catch (Exception exc)
            {
                return Json(new { status = "failed", message = "The review could not remove" });
            }
        }
        [HttpPost]
        public IActionResult DeletePhoto(int id)
        {
            var photos = context.Photo.Where(a => a.PhotoID == id).FirstOrDefault();
            if (photos != null)
            {
                context.Photo.Remove(photos);
                context.SaveChanges();
                Place place = context.Place.Where(a => a.PlaceID == photos.PlaceID).FirstOrDefault();

                string userID = HttpContext.Session.GetString("userID");
                UserChange newUserChange = new CommonFunctions().CreateUserChange(Int32.Parse(userID), "page show photos removing", place.Name + " " + photos.Category);
                context.UserChange.Add(newUserChange);
                context.SaveChanges();

                return Json(new { status = "success" });
            }
            else
            {
                return Json(new { status = "failed", message = "The photo was not found, refresh the page" });
            }
        }
        [HttpPost]
        public IActionResult DeleteCert(int id)
        {
            try
            {
                var cert = context.Certification.Where(a => a.CertificationID == id).FirstOrDefault();
                context.Certification.Remove(cert);
                context.SaveChanges();

                string userID = HttpContext.Session.GetString("userID");
                UserChange newUserChange = new CommonFunctions().CreateUserChange(Int32.Parse(userID), "page show certification removing", cert.Name);
                context.UserChange.Add(newUserChange);
                context.SaveChanges();

                return Json(new { status = "success" });
            }
            catch (Exception exc)
            {
                return Json(new { status = "failed", message = "The cert could not remove" });
            }
        }
        #endregion
        
        #region Test functions
        public void PlacesTest()
        {
            var photos = context.Caracteristicas.Where(a => a.Caracteristica == "photos").ToList();
            foreach (var photo in photos)
            {
                if (photo != null && photo.Valor != null && photo.Valor.Contains(" "))
                {
                    var photoString = "";
                    var photosList = photo.Valor.Split("##").Where(a => a.Length > 0 && a != null).ToList();
                    foreach (var phot in photosList)
                    {
                        photoString = photoString + phot.Replace(" ", "%20") + "##";
                    }
                    photo.Valor = photoString;
                    context.Caracteristicas.Update(photo);
                    context.SaveChanges();
                }
            }
        }

        public void WriteFile()
        {
            var place = context.Place.ToList();
            var json = JsonConvert.SerializeObject(place);
            System.IO.File.WriteAllText(@"C:\inetpub\wwwroot\path.txt", json);
            var a = "";
        }


        #endregion 
    }
}