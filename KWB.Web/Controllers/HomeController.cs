using HtmlAgilityPack;
using KWB.Web.Models;
using KWB.Web.Models.NewFolder;
using KWB.Web.Models.Response;
using KWB.Web.Models.Table;
using KWB.Web.Models.ViewModels;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic.FileIO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Numerics;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Runtime.ConstrainedExecution;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace KWB.Web.Controllers
{
    public class HomeController : Controller
    {

        private readonly AplicationDbContext context;
        public HomeController(AplicationDbContext context)
        {
            this.context = context;
        }
       
        #region Add functions
        public IActionResult Tags()
        {
            var status = HttpContext.Session.GetString("authstatus");
            var userID = HttpContext.Session.GetString("userID");
            if (status != "true")
            {
                return RedirectToAction("Login", "User");
            }
            else{
                UserChange newUserChange = new CommonFunctions().CreateUserChange(Int32.Parse(userID), "page add tag", null);
                context.UserChange.Add(newUserChange);
                context.SaveChanges();
            }

            var tipos = context.Tipo.Select(a=>a.Name).ToList();
            var tags = context.Tag.OrderBy(a=>a.TagName).ToList();
            ResponseTags response = new();
            response.Types = tipos;
            response.TagsDB = tags;
            return View(response);
        }
        
        [HttpPost]
        public async Task<IActionResult> Tags(ResponseTags tags)
        {
            if (tags.TagName == null) { return Json(new { status = "error", message = "The tag must have TagName" }); }
            var tagsDB = context.Tag.ToList();
            bool existTag = tagsDB.Any(a => a.TagName == tags.TagName);
            if (existTag) { return Json(new { status = "error", message = "The TagName is already exist" }); }

            var userID = HttpContext.Session.GetString("userID");
            UserChange newUserChange = new CommonFunctions().CreateUserChange(Int32.Parse(userID), "page add tag saving", tags.TagName);
            context.UserChange.Add(newUserChange);
            context.SaveChanges();

            string filePathPhoto = "";
            string filePathMarker = "";
            string filePathMarkerSelected = "";
            var fatherTag = 0;

            if ((bool)tags.Main)
            {
                var CurrentDirectory = Directory.GetCurrentDirectory();
                string filePath = CurrentDirectory + @"\wwwroot\img\upload\";
                if (tags.Photo == null) { return Json(new { status = "error", message = "complete all the fields" }); } 
                else {
                        filePathPhoto = string.Format("{0}{1}", filePath, tags.Photo.FileName);
                        using (var stream = System.IO.File.Create(filePathPhoto))
                        {
                            await tags.Photo.CopyToAsync(stream);
                        }
                    //filePathPhoto = "https://" + @"adminkwb.jojma.com.ar/img/upload/";
                    filePathPhoto = "http://ec2-44-202-212-224.compute-1.amazonaws.com/KWBfront/img/upload/";
                    filePathPhoto = string.Format("{0}{1}", filePathPhoto, tags.Photo.FileName);
                }
                if (tags.Marker == null) { return Json(new { status = "error", message = "complete all the fields" }); }
                else
                {
                    filePathMarker = string.Format("{0}{1}", filePath, tags.Marker.FileName);
                    using (var stream = System.IO.File.Create(filePathMarker))
                    {
                        await tags.Photo.CopyToAsync(stream);
                    }
                    //filePathMarker = "https://" + @"adminkwb.jojma.com.ar/img/upload/";
                    filePathMarker = "http://ec2-44-202-212-224.compute-1.amazonaws.com/KWBfront/img/upload/";
                    filePathMarker = string.Format("{0}{1}", filePathMarker, tags.Marker.FileName);
                }
                if (tags.MarkerSelected == null) { return Json(new { status = "error", message = "complete all the fields" }); }
                else
                {
                    filePathMarkerSelected = string.Format("{0}{1}", filePath, tags.MarkerSelected.FileName);
                    using (var stream = System.IO.File.Create(filePathMarkerSelected))
                    {
                        await tags.Photo.CopyToAsync(stream);
                    }
                    //filePathMarkerSelected = "https://" + @"adminkwb.jojma.com.ar/img/upload/";
                    filePathMarkerSelected = "http://ec2-44-202-212-224.compute-1.amazonaws.com/KWBfront/img/upload/";
                    filePathMarkerSelected = string.Format("{0}{1}", filePathMarkerSelected, tags.MarkerSelected.FileName);
                }
            }
            else { 
                if(tags.FatherTag != null)
                {
                    var tag = context.Tag.Where(a => a.TagName == tags.FatherTag).FirstOrDefault();
                    if (tag != null) { fatherTag = tag.TagID; }
                }
                filePathPhoto = null;
                filePathMarker = null;
                filePathMarkerSelected = null;
            }
            Tag newTag = new() { 
                TagName = tags.TagName,
                Icon = tags.IconCode == null ? "" : tags.IconCode,
                Main = tags.Main,
                Photo = filePathPhoto, 
                Marker = filePathMarker,
                MarkerSelected = filePathMarkerSelected,
                FatherID = fatherTag == 0 ? null : fatherTag,
                Sort=0
            };
            context.Tag.Add(newTag);
            context.SaveChanges();
            return Json(new { status = "success" });
        }
        #endregion
     
        #region Show functions
        public IActionResult Index()
        {
            var status = HttpContext.Session.GetString("authstatus");
            var userID = HttpContext.Session.GetString("userName");
            if(status != "true")
            {
                return RedirectToAction("Login", "User");
            }
            HomeViewModel homeViewModel = new HomeViewModel();
            homeViewModel.UserReportWereNotSeen = context.UserReport.Any(a => a.WasSeen == false);
            homeViewModel.SuggestionWereNotSeen = context.Suggestion.Any(a => a.WasSeen == false);
            homeViewModel.ContactMessageWereNotSeen = context.ContactMessage.Any(a => a.WasSeen == false);
            return View(homeViewModel);
        }
        public IActionResult ShowChangesHistory()
        {
            var status = HttpContext.Session.GetString("authstatus");
            var userID = HttpContext.Session.GetString("userID");
            if (status != "true")
            {
                return RedirectToAction("Login", "User");
            }

            UserChange newUserChange = new CommonFunctions().CreateUserChange(Int32.Parse(userID), "page show change history", null);
            context.UserChange.Add(newUserChange);
            context.SaveChanges();

            ResponseChangesHistory response = new();
            var places = context.Place.ToList();
            response.ChangesHistory = context.ChangesHistory.ToList();
            for(var i = 0; i<response.ChangesHistory.Count(); i++)
            {
                if(response.ChangesHistory[i].Name == "Review") { response.ChangesHistory[i].NewValue = "New Review"; }
                if (response.ChangesHistory[i].Name == "Secundary Picture") { response.ChangesHistory[i].NewValue = "New Secundary Images"; }
                if (response.ChangesHistory[i].Name == "Upload Picture") { response.ChangesHistory[i].NewValue = "New Upload Images"; }
                response.ChangesHistory[i].Establishment = places.Where(a => a.PlaceID == response.ChangesHistory[i].PlaceID).Select(a => a.Name).FirstOrDefault();
            }
            return View(response);
        }
        public IActionResult ShowTags()
        {
            var status = HttpContext.Session.GetString("authstatus");
            var userID = HttpContext.Session.GetString("userID");
            if (status != "true")
            {
                return RedirectToAction("Login", "User");
            }

            UserChange newUserChange = new CommonFunctions().CreateUserChange(Int32.Parse(userID), "page show tags", null);
            context.UserChange.Add(newUserChange);
            context.SaveChanges();

            var tipos = context.Tipo.ToList();
            var tags = context.Tag.OrderBy(a => a.TagName).ToList();
            ResponseTags response = new();
            response.Types = tipos.Select(a=>a.Name).ToList();
            response.TagsDB = context.Tag.OrderBy(a => a.TagName).ToList();
            for (var i = 0; i < response.TagsDB.Count(); i++)
            {
                var tag = response.TagsDB[i];
                tag.MaterialIcon = new CommonFunctions().ReturnIconFontawesome(tag.Icon);
                tag.Type = tipos.Where(a => a.TipoID == tag.TipoID).Select(a => a.Name).FirstOrDefault();
                var father = tags.Where(a => a.TagID == tag.FatherID).FirstOrDefault();

                if (father != null) { tag.FatherTag = father.TagName; }
                if(tag.Marker != null && tag.Marker.Length > 0) { tag.Marker = "data:image/png;base64," + tag.Marker; }
                if (tag.MarkerSelected != null && tag.MarkerSelected.Length > 0) 
                { tag.MarkerSelected = "data:image/png;base64," + tag.MarkerSelected; }
                //if(tag.Main != true) {
                //    tag.MarkerSelected = null;
                //    tag.Marker = null; }
            }
            return View(response);
        }
        #endregion
       
        #region Update functions
        
        [HttpPost]
        public IActionResult UpdateChangeHistory(string changeHistoryID)
        {

            if (changeHistoryID == null) { return Json(new { status = "failed", message = "Select one change to do" }); }

            var userID = HttpContext.Session.GetString("userID");
            UserChange newUserChange = new CommonFunctions().CreateUserChange(Int32.Parse(userID), "page show change history saving", changeHistoryID);
            context.UserChange.Add(newUserChange);
            context.SaveChanges();

            var listID = changeHistoryID.Split("##").Where(a => a != "").ToList();
            foreach(var changeID in listID)
            {
                var changeHistory = context.ChangesHistory.Where(a => a.ChangesHistoryID == Int32.Parse(changeID)).FirstOrDefault();
                if (changeHistory != null)
                {
                    var establishment = context.Place.Where(a => a.PlaceID == changeHistory.PlaceID).FirstOrDefault();
                    switch (changeHistory.Name)
                    {
                        case "Website":
                            try {
                                establishment.Website = changeHistory.NewValue;
                                context.Place.Update(establishment);
                                context.SaveChanges();
                            }
                            catch { }
                            break;
                        case "Rating":
                            var caractRating = context.Caracteristicas.Where(a => a.PlaceID == changeHistory.PlaceID && a.Caracteristica == "rating").FirstOrDefault();
                            try { 
                                if (caractRating != null)
                                {
                                    caractRating.Valor = changeHistory.NewValue;
                                    context.Caracteristicas.Update(caractRating);
                                    context.SaveChanges();
                                }
                                else
                                {
                                    Caracteristicas caract = new()
                                    {
                                        PlaceID = changeHistory.PlaceID,
                                        Caracteristica = "rating",
                                        Valor = changeHistory.NewValue
                                    };
                                    context.Caracteristicas.Add(caract);
                                    context.SaveChanges();
                                }
                            }
                            catch { }
                            break;
                        case "PriceLevel":
                            try { 
                                var caractPriceLevel = context.Caracteristicas.Where(a => a.PlaceID == changeHistory.PlaceID && a.Caracteristica == "price_level").FirstOrDefault();
                                if (caractPriceLevel != null)
                                {
                                    caractPriceLevel.Valor = changeHistory.NewValue;
                                    context.Caracteristicas.Update(caractPriceLevel);
                                    context.SaveChanges();
                                }
                                else
                                {
                                    Caracteristicas caract = new()
                                    {
                                        PlaceID = changeHistory.PlaceID,
                                        Caracteristica = "price_level",
                                        Valor = changeHistory.NewValue
                                    };
                                    context.Caracteristicas.Add(caract);
                                    context.SaveChanges();
                                }
                            }
                            catch { }
                            break;
                        case "Lat":
                            try { 
                                establishment.Lat = changeHistory.NewValue;
                                context.Place.Update(establishment);
                                context.SaveChanges();
                            }
                            catch { }
                            break;
                        case "Lng":
                            try { 
                                establishment.Lng = changeHistory.NewValue;
                                context.Place.Update(establishment);
                                context.SaveChanges();
                            }
                            catch { }
                            break;
                        case "Name":
                            establishment.Name = changeHistory.NewValue;
                            context.Place.Update(establishment);
                            context.SaveChanges();
                            break;
                        case "Phone":
                            establishment.Phone = changeHistory.NewValue;
                            context.Place.Update(establishment);
                            context.SaveChanges();
                            break;
                        case "Address":
                            try { 
                            establishment.Address = changeHistory.NewValue;
                            context.Place.Update(establishment);
                            context.SaveChanges();
                            }
                            catch { }
                            break;
                        case "ReviewsCount":
                            establishment.ReviewsCount = Int32.Parse(changeHistory.NewValue);
                            context.Place.Update(establishment);
                            context.SaveChanges();
                            break;
                        case "Main Picture":
                            try { 
                                establishment.Photo = changeHistory.NewValue;
                                context.Place.Update(establishment);
                                context.SaveChanges();
                            }
                            catch { }
                            break;
                        case "Secundary Picture":
                            try { 
                                var existSecundaryPhotos = context.Caracteristicas.Where(a => a.PlaceID == changeHistory.PlaceID && a.Caracteristica == "photos").FirstOrDefault();
                                if (existSecundaryPhotos == null)
                                {
                                    var newPhoto = new Caracteristicas()
                                    {
                                        PlaceID = establishment.PlaceID,
                                        Caracteristica = "photos",
                                        Valor = changeHistory.NewValue
                                    };
                                    context.Caracteristicas.Add(newPhoto);
                                    context.SaveChanges();
                                }
                                else
                                {
                                    existSecundaryPhotos.Valor = changeHistory.NewValue;
                                    context.Caracteristicas.Update(existSecundaryPhotos);
                                    context.SaveChanges();
                                }
                            }
                            catch { }
                            break;
                        case "Upload Picture":
                            try { 
                                var existPhoto = context.Photo.Where(a => a.PlaceID == changeHistory.PlaceID).ToList();
                                var photos = changeHistory.NewValue.Split("##").ToList();
                                if (existPhoto != null)
                                {
                                    for (var i = 0; i < photos.Count(); i++)
                                    {
                                        if (!existPhoto.Any(a => a.Url == photos[i]))
                                        {
                                            var newPhoto = new Photo()
                                            {
                                                PlaceID = changeHistory.PlaceID,
                                                Category = "All",
                                                Url = photos[i]
                                            };
                                            context.Photo.Add(newPhoto);
                                            context.SaveChanges();
                                        }
                                    }
                                }
                                else
                                {
                                    for (var i = 0; i < photos.Count(); i++)
                                    {
                                        var newPhoto = new Photo()
                                        {
                                            PlaceID = changeHistory.PlaceID,
                                            Category = "All",
                                            Url = photos[i]
                                        };
                                        context.Photo.Add(newPhoto);
                                        context.SaveChanges();
                                    }
                                }
                            }
                            catch { }
                            break;
                        case "ZipCode":
                            try { 
                            establishment.ZipCode = changeHistory.NewValue;
                            context.Place.Update(establishment);
                            context.SaveChanges();
                            }
                            catch { }
                            break;
                        case "Review":
                            var reviews = context.Review.Where(a => a.PlaceID == establishment.PlaceID).ToList();
                            var authorReview = changeHistory.NewValue.Split("##**").ToList();

                            var authorName = authorReview[0].Split("##").ToList();
                            var authorPhoto = authorReview[1].Split("##").ToList();
                            var authorText = authorReview[2].Split("##").ToList();
                            var authorRating = authorReview[3].Split("##").ToList();
                            var authorRelativeTime = authorReview[4].Split("##").ToList();
                            var authorTime = authorReview[5].Split("##").ToList();
                            try { 
                                if (reviews != null)
                                {
                                    for (var i = 0; i < reviews.Count(); i++)
                                    {
                                        if (i == 5) { break; }
                                        reviews[i].AuthorName = authorName[i];
                                        reviews[i].AuthorPhoto = authorPhoto[i];
                                        reviews[i].Rating = Int32.Parse(authorRating[i]);
                                        reviews[i].AgeInDays = Int32.Parse(authorRelativeTime[i]);
                                        reviews[i].Age = authorTime[i];
                                        reviews[i].Text = authorText[i];
                                        context.Review.Update(reviews[i]);
                                        context.SaveChanges();
                                    }
                                }
                                else
                                {
                                    for (var i = 0; i < 5; i++)
                                    {
                                        var newReview = new Review()
                                        {
                                            PlaceID = establishment.PlaceID,
                                            AuthorName = authorName[i],
                                            AuthorPhoto = authorPhoto[i],
                                            Age = authorTime[i],
                                            AgeInDays = Int32.Parse(authorRelativeTime[i]),
                                            Rating = Int32.Parse(authorRating[i]),
                                            Text = authorName[i],
                                        };
                                        context.Review.Add(newReview);
                                        context.SaveChanges();
                                    }
                                }
                            }
                            catch { }
                            break;
                        default:
                            break;
                    }
                    context.ChangesHistory.Remove(changeHistory);
                    context.SaveChanges();
                }
                else
                {
                    return Json(new { status = "failed", message = "this change history not exist" });
                }
            }
            return Json(new { status = "success" });
        }

        [HttpPost]
        public IActionResult UpdateTag(ResponseTags response)
        {
            try
            {

                if (response.TagName == null)
                {
                    return Json(new { status = "failed", message = "Complete all fields" });
                }

                var userID = HttpContext.Session.GetString("userID");
                UserChange newUserChange = new CommonFunctions().CreateUserChange(Int32.Parse(userID), "page show tags saving", response.TagName);
                context.UserChange.Add(newUserChange);
                context.SaveChanges();

                var tagDB = context.Tag.ToList();
                var tag = tagDB.Where(a => a.TagID == response.TagID).FirstOrDefault();
                if (tag.Main == true) { return Json(new { status = "failed", message = "main tags cannot be updated" }); }
                tag.TagName = response.TagName;
                tag.Icon = response.IconCode == null ? "" : response.IconCode;
                if (response.FatherTag != null) { tag.FatherID = tagDB.Where(a => a.TagName == response.FatherTag).Select(a => a.TagID).FirstOrDefault(); }
                context.Tag.Update(tag);
                context.SaveChanges();
                return Json(new { status = "success" });
            }
            catch (Exception exc)
            {
                return Json(new { status = "failed", message = "The tag could not save" });
            }
        }
        #endregion
     
        #region Return functions

        [HttpPost]
        public IActionResult ReturnTag(int id)
        {
            try
            {
                ResponseTags tags = new();
                var tagDB = context.Tag.OrderBy(a => a.TagName).ToList();
                var tag = tagDB.Where(a => a.TagID == id).FirstOrDefault();
                var typeDB = context.Tipo.ToList();
                var types = typeDB.Select(a => a.TipoID).ToList();
                int index = 0;

                var userID = HttpContext.Session.GetString("userID");
                UserChange newUserChange = new CommonFunctions().CreateUserChange(Int32.Parse(userID), "page show tags editing", tag.TagName);
                context.UserChange.Add(newUserChange);
                context.SaveChanges();

                tags.TagDB = tag;
                tags.TagDB.Type = typeDB.Where(a=>a.TipoID == tag.TipoID).Select(a=>a.Name).FirstOrDefault();
                tags.TagDB.FatherTag = tagDB.Where(a => a.TagID == tag.FatherID).OrderBy(a => a.TagName).Select(a=>a.TagName).FirstOrDefault();
                for (var i=0;i< types.Count(); i++)
                {
                    if(types[i] == tag.TipoID) { index = i;break;}
                }
                tags.Types = context.Tipo.Select(a => a.Name).ToList();
                return Json(new { status = "success", data = tags });
            }
            catch (Exception exc)
            {
                return Json(new { status = "failed", message = "the tag was not found" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ReturnEstablishmentByGoogle(int placeID, string googlePlaceID)
        {
                ResponsePlace response = new();
                var existPlace = context.Place.Where(a => a.PlaceID == placeID).FirstOrDefault();
                if (existPlace == null) { return Json(new { status = "failed", message = "the establishment was not found" }); }
                response.PlaceDB = existPlace;
                googlePlaceID = existPlace.GooglePlaceID;

                var userID = HttpContext.Session.GetString("userID");
                UserChange newUserChange = new CommonFunctions().CreateUserChange(Int32.Parse(userID), "page show establishment saving with google", placeID.ToString());
                context.UserChange.Add(newUserChange);
                context.SaveChanges();

                var newApiRegister = new CommonFunctions().CreateApiCallRegister("placeDetail");
                context.ApiCallsRegister.Add(newApiRegister);
                context.SaveChanges();

                if (existPlace.GooglePlaceID == null)
                {
                    var placeGoogleData = new CommonFunctions().GetPlaceDetail(existPlace.Name);
                    if (placeGoogleData == null) { return Json(new { status = "failed", message = "the establishment was not found in google" }); }
                    else
                    {
                        var caracteristicas = context.Caracteristicas.Where(a => a.PlaceID == existPlace.PlaceID && a.Caracteristica == "photos").ToList();
                        existPlace.GooglePlaceID = placeGoogleData.GooglePlaceID;
                        existPlace.LastUpdated = System.DateTime.Now;
                        context.Place.Update(existPlace);
                        if (!existPlace.Photo.Contains("googleusercontent"))
                        {
                            var newChangeHistory = new ChangesHistory()
                            {
                                OldValue = existPlace.Photo,
                                NewValue = placeGoogleData.Photo,
                                PlaceID = existPlace.PlaceID,
                                Name = "Main Picture",
                                Date = System.DateTime.Now,
                            };
                            context.ChangesHistory.Update(newChangeHistory);
                        };
                        if (caracteristicas == null)
                        {
                            var newChangeHistory = new ChangesHistory()
                            {
                                OldValue = "",
                                NewValue = placeGoogleData.SecundaryImages[0],
                                PlaceID = existPlace.PlaceID,
                                Name = "Secundary Picture",
                                Date = System.DateTime.Now,
                            };
                            context.ChangesHistory.Add(newChangeHistory);
                            var newChangeHistory2 = new ChangesHistory()
                                {
                                    OldValue = "",
                                    NewValue = placeGoogleData.SecundaryImages[1],
                                    PlaceID = existPlace.PlaceID,
                                    Name = "Secundary2 Picture",
                                    Date = System.DateTime.Now,
                                };
                            context.ChangesHistory.Add(newChangeHistory);
                            var newChangeHistory3 = new ChangesHistory()
                                {
                                    OldValue = "",
                                    NewValue = placeGoogleData.SecundaryImages[2],
                                    PlaceID = existPlace.PlaceID,
                                    Name = "Secundary3 Picture",
                                    Date = System.DateTime.Now,
                                };
                            context.ChangesHistory.Add(newChangeHistory);
                        };
                        if (placeGoogleData.Lat != existPlace.Lat)
                        {
                            var newChangeHistory = new ChangesHistory()
                            {
                                OldValue = existPlace.Lat,
                                NewValue = placeGoogleData.Lat,
                                PlaceID = existPlace.PlaceID,
                                Name = "Lat",
                                Date = System.DateTime.Now,
                            };
                            context.ChangesHistory.Update(newChangeHistory);
                        };
                        if (placeGoogleData.Lng != existPlace.Lng)
                        {
                            var newChangeHistory = new ChangesHistory()
                            {
                                OldValue = existPlace.Lng,
                                NewValue = placeGoogleData.Lng,
                                PlaceID = existPlace.PlaceID,
                                Name = "Lng",
                                Date = System.DateTime.Now,
                            };
                            context.ChangesHistory.Update(newChangeHistory);
                        };
                        if (placeGoogleData.Address != existPlace.Address)
                        {
                            var newChangeHistory = new ChangesHistory()
                            {
                                OldValue = existPlace.Address,
                                NewValue = placeGoogleData.Address,
                                PlaceID = existPlace.PlaceID,
                                Name = "Address",
                                Date = System.DateTime.Now,
                            };
                            context.ChangesHistory.Update(newChangeHistory);
                        };
                        if (placeGoogleData.Rating!=null) {
                            var newChangeHistory = new ChangesHistory()
                            {
                                OldValue = existPlace.Rating,
                                NewValue = placeGoogleData.Rating,
                                PlaceID=existPlace.PlaceID,
                                Name= "Rating",
                                Date = System.DateTime.Now,
                            };
                            context.ChangesHistory.Update(newChangeHistory);
                        };
                        if (placeGoogleData.Website != null && existPlace.Website != null)
                        {
                            var newChangeHistory = new ChangesHistory()
                            {
                                OldValue = existPlace.Website,
                                NewValue = placeGoogleData.Website,
                                PlaceID = existPlace.PlaceID,
                                Name = "Website",
                                Date = System.DateTime.Now,
                            };
                            context.ChangesHistory.Update(newChangeHistory);
                        };
                        if (placeGoogleData.ReviewsCount != null && existPlace.ReviewsCount != null)
                        {
                            var newChangeHistory = new ChangesHistory()
                            {
                                OldValue = "" + existPlace.ReviewsCount,
                                NewValue = "" + placeGoogleData.ReviewsCount,
                                PlaceID = existPlace.PlaceID,
                                Name = "ReviewsCount",
                                Date = System.DateTime.Now,
                            };
                            context.ChangesHistory.Update(newChangeHistory);
                        };
                        if (placeGoogleData.Phone != null && existPlace.Phone != null)
                        {
                            var newChangeHistory = new ChangesHistory()
                            {
                                OldValue = "" + existPlace.Phone,
                                NewValue = "" + placeGoogleData.Phone,
                                PlaceID = existPlace.PlaceID,
                                Name = "Phone",
                                Date = System.DateTime.Now,
                            };
                            context.ChangesHistory.Update(newChangeHistory);
                        };
                        if (placeGoogleData.PriceLevel != null && existPlace.PriceLevel != null)
                        {
                            var newChangeHistory = new ChangesHistory()
                            {
                                OldValue = "" + existPlace.PriceLevel,
                                NewValue = "" + placeGoogleData.PriceLevel,
                                PlaceID = existPlace.PlaceID,
                                Name = "PriceLevel",
                                Date = System.DateTime.Now,
                            };
                            context.ChangesHistory.Update(newChangeHistory);
                        };
                        context.SaveChanges();
                        return Json(new { status = "success" });
                    }
                }

                if (existPlace.LastUpdated != null) 
                {
                    DateTime d1 = DateTime.Now;
                    DateTime d2 = (DateTime)existPlace.LastUpdated;
                    TimeSpan difference = d1 - d2;
                    var days = difference.TotalDays;
                    //if (days < 30)
                    //{
                    //    return Json(new { status = "failed", message = "this establishment cannot be updated because it was already updated less than 30 days ago."});
                    //}
                }

                //PlaceGoogleID for test: ChIJN1t_tDeuEmsRUsoyG83frY4
                var dict = new Dictionary<string, string>();
                var url = "https://maps.googleapis.com/maps/api/place/details/json?key=AIzaSyCRJLTDv-YeDn8TIx7uYptHtuB5DwbXYJs&language=en&place_id=" + googlePlaceID;
                var client = new HttpClient();
                var req = new HttpRequestMessage(HttpMethod.Post, url) { Content = new FormUrlEncodedContent(dict) };
                var res = await client.SendAsync(req);

                if (res.StatusCode.ToString() == "OK")
                {
                    var responseAPI = res.Content.ReadAsStringAsync().Result;
                    var jsonResponse = JsonConvert.DeserializeObject<ResponseGoogle>(responseAPI);
                    var caract = context.Caracteristicas.ToList();
                    var ratingPlace = caract.Where(a => a.PlaceID == existPlace.PlaceID && a.Caracteristica == "rating").Select(a=>a.Valor).FirstOrDefault();
                    var priceLevelPlace = caract.Where(a => a.PlaceID == existPlace.PlaceID && a.Caracteristica == "price_level").Select(a => a.Valor).FirstOrDefault();
                    var changesHistory = context.ChangesHistory.ToList();
                    existPlace.PriceLevel = priceLevelPlace;
                    existPlace.Rating = ratingPlace;
                    response.GoogleDataPlace = jsonResponse;

                try
                {

                    if ((existPlace.Photo == null || !existPlace.Photo.Contains("googleusercontent")) && jsonResponse.Result.Photos != null)
                    {
                        var existChangeHistory = changesHistory.Where(a => a.PlaceID == existPlace.PlaceID && a.Name == "Main Picture").FirstOrDefault();
                        if (existChangeHistory != null)
                        {
                            existChangeHistory.OldValue = existPlace.Photo;
                            existChangeHistory.NewValue = new CommonFunctions().GetGooglePhoto(jsonResponse.Result.Photos[0].Photo_reference);
                            existChangeHistory.Date = System.DateTime.Now;
                        }
                        else
                        {
                            ChangesHistory newChangeHistory = new()
                            {
                                PlaceID = existPlace.PlaceID,
                                GooglePlaceID = existPlace.GooglePlaceID,
                                OldValue = existPlace.Photo,
                                NewValue = new CommonFunctions().GetGooglePhoto(jsonResponse.Result.Photos[0].Photo_reference),
                                Date = System.DateTime.Now,
                                Name = "Main Picture"
                            };
                            context.ChangesHistory.Add(newChangeHistory);
                        }
                        context.SaveChanges();
                    }
                    if(jsonResponse.Result.Photos != null)
                    {
                        var photoList = new List<string>();
                        for (var i = 0; i< jsonResponse.Result.Photos.Count(); i++) { photoList.Add(new CommonFunctions().GetGooglePhoto(jsonResponse.Result.Photos[i].Photo_reference)); }
                        var existChangeHistory = changesHistory.Where(a => a.PlaceID == existPlace.PlaceID && a.Name == "Secundary Picture").FirstOrDefault();
                        if (existChangeHistory != null)
                        {
                            existChangeHistory.OldValue = "Old Secundary Photos";
                            existChangeHistory.NewValue = "" + photoList[1]+"##" + photoList[2] + "##" + photoList[3];
                            existChangeHistory.Date = System.DateTime.Now;
                        }
                        else
                        {
                            var newChangeHistory = new ChangesHistory()
                            {
                                PlaceID = placeID,
                                GooglePlaceID = googlePlaceID,
                                Date = System.DateTime.Now,
                                Name = "Secundary Picture",
                                OldValue = "Old Secundary Photos",
                                NewValue = "" + photoList[1] + "##" + photoList[2] + "##" + photoList[3]
                            };
                            context.ChangesHistory.Add(newChangeHistory);
                            context.SaveChanges();
                        }
                        existChangeHistory = changesHistory.Where(a => a.PlaceID == existPlace.PlaceID && a.Name == "Upload Picture").FirstOrDefault();
                        if (existChangeHistory != null)
                        {
                            existChangeHistory.OldValue = "Old Upload Photos";
                            existChangeHistory.NewValue = "" + photoList[4] + "##" + photoList[5] + "##" + photoList[6] + "##" + photoList[7] + "##" + photoList[8] + "##" + photoList[9];
                            existChangeHistory.Date = System.DateTime.Now;
                        }
                        else
                        {
                            var newChangeHistory = new ChangesHistory()
                            {
                                PlaceID = placeID,
                                GooglePlaceID = googlePlaceID,
                                Date = System.DateTime.Now,
                                Name = "Upload Picture",
                                OldValue = "Old Upload Photos",
                                NewValue = "" + photoList[4] + "##" + photoList[5] + "##" + photoList[6] + "##" + photoList[7] + "##" + photoList[8] + "##" + photoList[9]
                            };
                            context.ChangesHistory.Add(newChangeHistory);
                            context.SaveChanges();
                        }
                    }
                }
                catch { }
                try {
                    if (jsonResponse.Result.User_ratings_total != existPlace.ReviewsCount)
                    {
                        var existChangeHistory = changesHistory.Where(a => a.PlaceID == existPlace.PlaceID && a.Name == "ReviewsCount").FirstOrDefault();
                        if (existChangeHistory != null)
                        {
                            existChangeHistory.OldValue = existPlace.ReviewsCount.ToString();
                            existChangeHistory.NewValue = jsonResponse.Result.User_ratings_total.ToString();
                            existChangeHistory.Date = System.DateTime.Now;
                        }
                        else
                        {
                            ChangesHistory newChangeHistory = new()
                            {
                                PlaceID = existPlace.PlaceID,
                                GooglePlaceID = existPlace.GooglePlaceID,
                                OldValue = existPlace.ReviewsCount.ToString(),
                                NewValue = jsonResponse.Result.User_ratings_total.ToString(),
                                Date = System.DateTime.Now,
                                Name = "ReviewsCount"
                            };
                            context.ChangesHistory.Add(newChangeHistory);
                        }
                        context.SaveChanges();
                    }
                } catch { }
                try {
                    if (jsonResponse.Result.Website != existPlace.Website)
                    {
                        var existChangeHistory = changesHistory.Where(a => a.PlaceID == existPlace.PlaceID && a.Name == "Website").FirstOrDefault();
                        if (existChangeHistory != null)
                        {
                            existChangeHistory.OldValue = existPlace.Website;
                            existChangeHistory.NewValue = jsonResponse.Result.Website;
                            existChangeHistory.Date = System.DateTime.Now;
                        }
                        else
                        {
                            ChangesHistory newChangeHistory = new()
                            {
                                PlaceID = existPlace.PlaceID,
                                GooglePlaceID = existPlace.GooglePlaceID,
                                OldValue = existPlace.Website,
                                NewValue = jsonResponse.Result.Website,
                                Date = System.DateTime.Now,
                                Name = "Website"
                            };
                            context.ChangesHistory.Add(newChangeHistory);
                        }
                        context.SaveChanges();
                    }
                } catch { }
                try
                {
                    if (jsonResponse.Result.Rating != ratingPlace)
                    {
                        var existChangeHistory = changesHistory.Where(a => a.PlaceID == existPlace.PlaceID && a.Name == "Rating").FirstOrDefault();
                        if (existChangeHistory != null)
                        {
                            existChangeHistory.OldValue = ratingPlace;
                            existChangeHistory.NewValue = jsonResponse.Result.Rating;
                            existChangeHistory.Date = System.DateTime.Now;
                            context.ChangesHistory.Update(existChangeHistory);
                        }
                        else
                        {
                            ChangesHistory newChangeHistory = new()
                            {
                                PlaceID = existPlace.PlaceID,
                                GooglePlaceID = existPlace.GooglePlaceID,
                                OldValue = ratingPlace,
                                NewValue = jsonResponse.Result.Rating,
                                Date = System.DateTime.Now,
                                Name = "Rating"
                            };
                            context.ChangesHistory.Add(newChangeHistory);
                        }
                        context.SaveChanges();
                    }
                } catch { }
                try {
                    if (jsonResponse.Result.Geometry.Location.Lat != existPlace.Lat)
                    {
                        var existChangeHistory = changesHistory.Where(a => a.PlaceID == existPlace.PlaceID && a.Name == "Lat").FirstOrDefault();
                        if (existChangeHistory != null)
                        {
                            existChangeHistory.OldValue = existPlace.Lat;
                            existChangeHistory.NewValue = jsonResponse.Result.Geometry.Location.Lat;
                            existChangeHistory.Date = System.DateTime.Now;
                            context.ChangesHistory.Update(existChangeHistory);
                        }
                        else
                        {
                            ChangesHistory newChangeHistory = new()
                            {
                                PlaceID = existPlace.PlaceID,
                                GooglePlaceID = existPlace.GooglePlaceID,
                                OldValue = existPlace.Lat,
                                NewValue = jsonResponse.Result.Geometry.Location.Lat,
                                Date = System.DateTime.Now,
                                Name = "Lat"
                            };
                            context.ChangesHistory.Add(newChangeHistory);
                        }
                        context.SaveChanges();
                    }
                } catch { }
                try {
                    if (jsonResponse.Result.Geometry.Location.Lng != existPlace.Lng)
                    {
                        var existChangeHistory = changesHistory.Where(a => a.PlaceID == existPlace.PlaceID && a.Name == "Lng").FirstOrDefault();
                        if (existChangeHistory != null)
                        {
                            existChangeHistory.OldValue = existPlace.Lng;
                            existChangeHistory.NewValue = jsonResponse.Result.Geometry.Location.Lng;
                            existChangeHistory.Date = System.DateTime.Now;
                            context.ChangesHistory.Update(existChangeHistory);
                        }
                        else
                        {
                            ChangesHistory newChangeHistory = new()
                            {
                                PlaceID = existPlace.PlaceID,
                                GooglePlaceID = existPlace.GooglePlaceID,
                                OldValue = existPlace.Lng,
                                NewValue = jsonResponse.Result.Geometry.Location.Lng,
                                Date = System.DateTime.Now,
                                Name = "Lng"
                            };
                            context.ChangesHistory.Add(newChangeHistory);
                        }
                        context.SaveChanges();
                    }
                } catch { }
                try {
                    if (jsonResponse.Result.Name != existPlace.Name)
                    {
                        var existChangeHistory = changesHistory.Where(a => a.PlaceID == existPlace.PlaceID && a.Name == "Name").FirstOrDefault();
                        if (existChangeHistory != null)
                        {
                            existChangeHistory.OldValue = existPlace.Name;
                            existChangeHistory.NewValue = jsonResponse.Result.Name;
                            existChangeHistory.Date = System.DateTime.Now;
                            context.ChangesHistory.Update(existChangeHistory);
                        }
                        else
                        {
                            ChangesHistory newChangeHistory = new()
                            {
                                PlaceID = existPlace.PlaceID,
                                GooglePlaceID = existPlace.GooglePlaceID,
                                OldValue = existPlace.Name,
                                NewValue = jsonResponse.Result.Name,
                                Date = System.DateTime.Now,
                                Name = "Name"
                            };
                            context.ChangesHistory.Add(newChangeHistory);
                        }
                        context.SaveChanges();
                    }
                } catch { }
                try {
                    if (jsonResponse.Result.International_phone_number != existPlace.Phone)
                    {
                        var existChangeHistory = changesHistory.Where(a => a.PlaceID == existPlace.PlaceID && a.Name == "Phone").FirstOrDefault();
                        if (existChangeHistory != null)
                        {
                            existChangeHistory.OldValue = existPlace.Phone;
                            existChangeHistory.NewValue = jsonResponse.Result.International_phone_number;
                            existChangeHistory.Date = System.DateTime.Now;
                            context.ChangesHistory.Update(existChangeHistory);
                        }
                        else
                        {
                            ChangesHistory newChangeHistory = new()
                            {
                                PlaceID = existPlace.PlaceID,
                                GooglePlaceID = existPlace.GooglePlaceID,
                                OldValue = existPlace.Phone,
                                NewValue = jsonResponse.Result.International_phone_number,
                                Date = System.DateTime.Now,
                                Name = "Phone"
                            };
                            context.ChangesHistory.Add(newChangeHistory);
                        }
                        context.SaveChanges();
                    }
                } catch { }
                try {
                    if (jsonResponse.Result.Price_level != priceLevelPlace)
                    {
                        var existChangeHistory = changesHistory.Where(a => a.PlaceID == existPlace.PlaceID && a.Name == "PriceLevel").FirstOrDefault();
                        if (existChangeHistory != null)
                        {
                            existChangeHistory.OldValue = priceLevelPlace;
                            existChangeHistory.NewValue = jsonResponse.Result.Price_level;
                            existChangeHistory.Date = System.DateTime.Now;
                            context.ChangesHistory.Update(existChangeHistory);
                        }
                        else
                        {
                            ChangesHistory newChangeHistory = new()
                            {
                                PlaceID = existPlace.PlaceID,
                                GooglePlaceID = existPlace.GooglePlaceID,
                                OldValue = priceLevelPlace,
                                NewValue = jsonResponse.Result.Price_level,
                                Date = System.DateTime.Now,
                                Name = "PriceLevel"
                            };
                            context.ChangesHistory.Add(newChangeHistory);
                        }
                        context.SaveChanges();
                    }
                } catch { }
                try {
                    if (jsonResponse.Result.Formatted_address != existPlace.Address)
                    {
                        var existChangeHistory = changesHistory.Where(a => a.PlaceID == existPlace.PlaceID && a.Name == "Address").FirstOrDefault();
                        if (existChangeHistory != null)
                        {
                            existChangeHistory.OldValue = existPlace.Address;
                            existChangeHistory.NewValue = jsonResponse.Result.Formatted_address;
                            existChangeHistory.Date = System.DateTime.Now;
                            context.ChangesHistory.Update(existChangeHistory);
                        }
                        else
                        {
                            ChangesHistory newChangeHistory = new()
                            {
                                PlaceID = existPlace.PlaceID,
                                GooglePlaceID = existPlace.GooglePlaceID,
                                OldValue = existPlace.Address,
                                NewValue = jsonResponse.Result.Formatted_address,
                                Date = System.DateTime.Now,
                                Name = "Address"
                            };
                            context.ChangesHistory.Add(newChangeHistory);
                        }
                        context.SaveChanges();
                    }
                } catch { }
                try {
                    var zipCode = "";
                    for (var i = 0; i < jsonResponse.Result.Address_components?.Count(); i++)
                    {
                        if (jsonResponse.Result.Address_components[i].Types[0] == "postal_code")
                        {
                            zipCode = jsonResponse.Result.Address_components[i].Long_name;
                        }
                    }
                    if (zipCode != existPlace.ZipCode) {
                        var existChangeHistory = changesHistory.Where(a => a.PlaceID == existPlace.PlaceID && a.Name == "ZipCode").FirstOrDefault();
                        if (existChangeHistory != null)
                        {
                            existChangeHistory.OldValue = existPlace.ZipCode.ToString();
                            existChangeHistory.NewValue = zipCode;
                            existChangeHistory.Date = System.DateTime.Now;
                            context.ChangesHistory.Update(existChangeHistory);
                        }
                        else
                        {
                            ChangesHistory newChangeHistory = new()
                            {
                                PlaceID = existPlace.PlaceID,
                                GooglePlaceID = existPlace.GooglePlaceID,
                                OldValue = existPlace.ZipCode.ToString(),
                                NewValue = zipCode.ToString(),
                                Date = System.DateTime.Now,
                                Name = "ZipCode"
                            };
                            context.ChangesHistory.Add(newChangeHistory);
                        }
                        context.SaveChanges();
                    }
                } catch { }
                try {
                    var author = "";
                    var urlPhoto = "";
                    var rating = "";
                    var relativeTime = "";
                    var text = "";
                    var time = "";
                    var reviewData = "";
                    if (jsonResponse.Result.Reviews != null)
                    {
                        for (var i = 0; i < jsonResponse.Result.Reviews?.Count(); i++)
                        {
                            if (jsonResponse.Result.Reviews[i].Author_name != null)
                                author = author + jsonResponse.Result.Reviews[i].Author_name + "##";

                            if (jsonResponse.Result.Reviews[i].Profile_photo_url != null)
                                urlPhoto = urlPhoto + jsonResponse.Result.Reviews[i].Author_url + "##";

                            if (jsonResponse.Result.Reviews[i].Rating != null)
                                rating = rating + jsonResponse.Result.Reviews[i].Rating + "##";

                            if (jsonResponse.Result.Reviews[i].Relative_time_description != null)
                                relativeTime = relativeTime + jsonResponse.Result.Reviews[i].Relative_time_description + "##";

                            if (jsonResponse.Result.Reviews[i].Text != null)
                                text = text + jsonResponse.Result.Reviews[i].Text + "##";
                            time = time + jsonResponse.Result.Reviews[i].Time + "##";
                        }
                        reviewData = author + "**" + urlPhoto + "**" + text + "**" + rating + "**" + time + "**" + relativeTime;
                        var existChangeHistory = changesHistory.Where(a => a.PlaceID == existPlace.PlaceID && a.Name == "Review").FirstOrDefault();
                        if (existChangeHistory != null)
                        {
                            existChangeHistory.OldValue = "Old Review";
                            existChangeHistory.NewValue = reviewData;
                            existChangeHistory.Date = System.DateTime.Now;
                            context.ChangesHistory.Update(existChangeHistory);
                        }
                        else
                        {
                            ChangesHistory newChangeHistory = new()
                            {
                                PlaceID = existPlace.PlaceID,
                                GooglePlaceID = existPlace.GooglePlaceID,
                                OldValue = "Old Review",
                                NewValue = reviewData,
                                Date = System.DateTime.Now,
                                Name = "Review"
                            };
                            context.ChangesHistory.Add(newChangeHistory);
                        }
                        context.SaveChanges();
                    }
                } catch { }
                    existPlace.LastUpdated = System.DateTime.Now;
                    context.Place.Update(existPlace);
                try {
                    context.SaveChanges();
                    return Json(new { status = "success"});
                }
                    
                catch(Exception exc) { return Json(new { status = "failed", message = exc.InnerException.ToString() }) ; }
                }
                else { return Json(new { status = "failed", message = "the establishment was not found" }); }
        }

        #endregion
    
        #region Delete functions
        
        [HttpPost]
        public IActionResult DeleteChangeHistory(string changeHistoryID)
        {
            if (changeHistoryID == null) { return Json(new { status = "failed", message = "Select one change to delete" }); }
            var listID = changeHistoryID.Split("##").ToList().Where(a => a != "").ToList();

            var userID = HttpContext.Session.GetString("userID");
            try
            {
                foreach(var id in listID)
                {
                    var changeHistory = context.ChangesHistory.Where(a => a.ChangesHistoryID == Int32.Parse(id)).FirstOrDefault();
                    context.ChangesHistory.Remove(changeHistory);
                    context.SaveChanges();

                    UserChange newUserChange = new CommonFunctions().CreateUserChange(Int32.Parse(userID), "page show change history removing", changeHistory.Name);
                    context.UserChange.Add(newUserChange);
                    context.SaveChanges();
                }
                return Json(new { status = "success" });
            }
            catch (Exception exc)
            {
                return Json(new { status = "failed", message = "The change history could not remove" });
            }
        }

        [HttpPost]
        public IActionResult DeleteTag(int id)
        {
            try
            {
                var tag = context.Tag.Where(a => a.TagID == id).FirstOrDefault();
                if(tag.Main == true) { return Json(new { status = "failed", message = "main tags cannot be removed" }); }

                var userID = HttpContext.Session.GetString("userID");
                string tagName = tag.TagName.Length > 100 ? tag.TagName.Substring(0, 100) : tag.TagName;
                UserChange newUserChange = new CommonFunctions().CreateUserChange(Int32.Parse(userID), "page show tags removing", tagName);
                context.UserChange.Add(newUserChange);
                context.SaveChanges();

                var tipoTags = context.TipoTag.Where(a => a.TagID == tag.TagID).ToList();
                foreach(var tipoTag in tipoTags)
                {
                    context.TipoTag.Remove(tipoTag);
                    context.SaveChanges();
                }
                context.Tag.Remove(tag);
                context.SaveChanges();
                return Json(new { status = "success" });
            }
            catch (Exception exc)
            {
                return Json(new { status = "failed", message = "This tag could not remove" });
            }
        }
        #endregion

        #region Testing functions
        public IActionResult Test()
        {

            var tipos = context.Tipo.ToList();
            var tags = context.Tag.OrderBy(a => a.TagName).ToList();
            ResponseTags response = new();
            response.Types = tipos.Select(a => a.Name).ToList();
            response.TagsDB = context.Tag.OrderBy(a => a.TagName).ToList();
            for (var i = 0; i < response.TagsDB.Count(); i++)
            {
                var tag = response.TagsDB[i];
                tag.MaterialIcon = new CommonFunctions().ReturnIconFontawesome(tag.Icon);
                tag.Type = tipos.Where(a => a.TipoID == tag.TipoID).Select(a => a.Name).FirstOrDefault();
                var father = tags.Where(a => a.TagID == tag.FatherID).FirstOrDefault();
                if (father != null) { tag.FatherTag = father.TagName; }
                if (tag.Marker != null && tag.Marker.Length > 0) { tag.Marker = "data:image/png;base64," + tag.Marker; }
                if (tag.MarkerSelected != null && tag.MarkerSelected.Length > 0)
                { tag.MarkerSelected = "data:image/png;base64," + tag.MarkerSelected; }
                if (tag.Main != true)
                {
                    tag.MarkerSelected = null;
                    tag.Marker = null;
                }
            }
            return View(response);
        }
        [HttpGet]
        public IActionResult TestResponse(string id)
        {
            var tags = context.Tag.ToList();
            TablePlace response = new TablePlace
            {
                Draw = "1",
                RecordsFiltered = tags.Count(),
                RecordsTotal = tags.Count(),
                Data = tags
            };

            //var jsonResponse = JsonConvert.DeserializeObject<TablePlace>(response.ToString());
            //return response;
            var json = Json(response);
           return Json(response);
        }
        public void htmlPage()
        {
            string Url = "https://oukosher.org/restaurants/";
            //string Url = "https://www.ok.org/consumers/ok-certified-restaurants/";
            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(Url);
            myRequest.Method = "GET";
            WebResponse myResponse = myRequest.GetResponse();
            StreamReader sr = new StreamReader(myResponse.GetResponseStream(), System.Text.Encoding.UTF8);
            string result = sr.ReadToEnd();
            var list1 = result.Split("<tr class=\"item\"").ToList();
            sr.Close();
            var listPlace = new List<string>();
            foreach(var item in list1)
            {
                var data = item.Substring(0, 100);
                listPlace.Add(data);
            }
            myResponse.Close();

        }
        public void FixPhotos()
        {
            var places = context.Place.Where(a=>a.Photo != null && a.Photo.Contains("maps.googleapis.com/maps/api/place/js/PhotoService.GetPhoto")).ToList();
            if (places != null)
            {
                foreach (var place in places)
                {
                        var photo = place.Photo.Split("1s").ToList();
                        var photoSplit = photo[1].Split("&callback").ToList();
                        var photoReference = photoSplit[0];
                        var newPhoto = new CommonFunctions().GetGooglePhoto(photoReference);
                        if(newPhoto != "")
                        {
                            place.Photo = newPhoto;
                            context.Place.Update(place);
                            context.SaveChanges();
                        }
                        var newApiRegister = new CommonFunctions().CreateApiCallRegister("photos");
                        context.ApiCallsRegister.Add(newApiRegister);
                        context.SaveChanges();
                }
            }
        }
        public void FixPhotos2()
        {
            var photos = context.Photo.Where(a=>a.Url.Contains("maps.googleapis.com")).ToList();
            if (photos != null)
            {
                foreach (var photoPlace in photos)
                {
                    if (photoPlace.Url != null && photoPlace.Url.Contains("maps.googleapis.com"))
                    {
                        var photo = photoPlace.Url.Split("1s").ToList();
                        var photoSplit = photo[1].Split("&callback").ToList();
                        var photoReference = photoSplit[0];
                        var newPhoto = new CommonFunctions().GetGooglePhoto(photoReference);
                        if (newPhoto != "")
                        {
                            photoPlace.Url = newPhoto;
                            context.Photo.Update(photoPlace);
                            context.SaveChanges();
                        }
                        var newApiRegister = new CommonFunctions().CreateApiCallRegister("photos");
                        context.ApiCallsRegister.Add(newApiRegister);
                        context.SaveChanges();
                    }
                }
            }
        }
        public void FixPhotos3()
        {
            var caracteristicas = context.Caracteristicas.Where(a => a.Caracteristica == "photos").ToList();
            var caracteristicas2 = caracteristicas.Where(a => a.Caracteristica == "photos" && a.Valor.Contains("##")).ToList();
            foreach (var caract in caracteristicas2)
            {
                var caractValue = "";
                var listSecundaryPhoto = caract.Valor.Split("##").ToList();
                listSecundaryPhoto = listSecundaryPhoto.Where(a => a != "").ToList();
                foreach (var secundaryPhoto in listSecundaryPhoto)
                {
                    if (secundaryPhoto.Contains("maps.googleapis.com"))
                    {
                        caractValue = caractValue + "" + "##";
                    }
                    else if(secundaryPhoto != "##")
                    {
                        caractValue = caractValue + secundaryPhoto + "##";
                    }
                }
                caract.Valor = caractValue;
                context.Caracteristicas.Update(caract);
                context.SaveChanges();
            }
        }
        public void AddTags()
        {
            var path = @"RestaurauntsCafesDelivery.csv";
            var places = context.Place.ToList();
            var tags = context.Tag.ToList();
            var tipoTags = context.TipoTag.ToList();
            using (TextFieldParser csvParser = new TextFieldParser(path))
            {
                csvParser.CommentTokens = new string[] { "#" };
                csvParser.SetDelimiters(new string[] { "," });
                csvParser.HasFieldsEnclosedInQuotes = true;

                // Skip the row with the column names
                csvParser.ReadLine();

                while (!csvParser.EndOfData)
                {
                    // Read current line fields, pointer moves to the next line.
                    string[] fields = csvParser.ReadFields();
                    string Name = fields[1];
                    string allTags = "";
                    allTags = fields[10] + "," + fields[11];

                    var place = places.Where(a => a.Name.ToLower() == Name.ToLower()).FirstOrDefault();
                    if (place != null)
                    {
                        var newListTags = allTags.Split(",").ToList();
                        foreach(var tag in newListTags)
                        {
                            var existTag = tags.Where(a => a.TagName.ToLower() == tag.ToLower()).FirstOrDefault();
                            if (existTag != null)
                            {
                                if (tipoTags.Where(a => a.PlaceID == place.PlaceID && a.TagID == existTag.TagID).FirstOrDefault() == null)
                                {
                                    var newTipoTag = new TipoTag()
                                    {
                                        PlaceID = place.PlaceID,
                                        Main = existTag.Main,
                                        TagID = existTag.TagID
                                    };
                                    context.TipoTag.Add(newTipoTag);
                                    context.SaveChanges();
                                }
                            }
                        }
                    }
                }
            }
        }
        public void CertTag()
        {
            var tags = context.Tag.ToList();
            var tipoTags = context.TipoTag.ToList();
            var chabad = tags.Where(a => a.TagName == "Chabad (Official)").FirstOrDefault();
            var synagogue = tags.Where(a => a.TagName.ToLower() == "synagogue").FirstOrDefault();
            var chabadTags = tipoTags.Where(a => a.TagID == chabad.TagID).ToList();
            foreach (var tag in chabadTags)
            {
                var existTagSynagogue = tipoTags.Where(a => a.TagID == synagogue.TagID && a.PlaceID == tag.PlaceID).FirstOrDefault();
                if (existTagSynagogue == null)
                {
                    var mainTag = tipoTags.Where(a => a.PlaceID == tag.PlaceID && a.Main == true).FirstOrDefault();
                    if (mainTag != null)
                    {
                        mainTag.Main = false;
                        context.TipoTag.Update(mainTag);
                        context.SaveChanges();
                    }
                    TipoTag newTag = new TipoTag()
                    {
                        Main = true,
                        PlaceID = tag.PlaceID,
                        TagID = synagogue.TagID
                    };
                    context.TipoTag.Add(newTag);
                    context.SaveChanges();
                }
                else if (existTagSynagogue.Main == false)
                {
                    var mainTag = tipoTags.Where(a => a.PlaceID == tag.PlaceID && a.Main == true).FirstOrDefault();
                    if (mainTag != null)
                    {
                        mainTag.Main = false;
                        context.TipoTag.Update(mainTag);
                        context.SaveChanges();
                    }
                    existTagSynagogue.Main = true;
                    context.TipoTag.Update(existTagSynagogue);
                    context.SaveChanges();
                }
            }
            //tag chabad (Official) que sea main tag synagogue
        }

        public void OpenNowMainPicture()
        {
            var establishemnts = context.Place.Where(a => a.Permanently_closed == true).ToList();
            foreach (var establishment in establishemnts)
            {
                if(establishment.Photo != "http://ec2-44-202-212-224.compute-1.amazonaws.com/KWBfront/img/upload/b5f0da8d-ed39-44b6-b0dc-57273082a23f.jpg")
                {
                    establishment.Photo = "http://ec2-44-202-212-224.compute-1.amazonaws.com/KWBfront/img/upload/b5f0da8d-ed39-44b6-b0dc-57273082a23f.jpg";
                    context.Place.Update(establishment);
                    context.SaveChanges();
                }
            }
        }
        public void OpenNowSecundaryPicture()
        {
            var establishemnts = context.Place.Where(a => a.Permanently_closed == true).ToList();
            var caracteristicas = context.Caracteristicas.ToList();
            foreach (var establishment in establishemnts)
            {
                var caract = caracteristicas.Where(a => a.PlaceID == establishment.PlaceID && a.Caracteristica == "photos").FirstOrDefault();
                if(caract!= null)
                {
                    var photos = "";
                    var data = caract.Valor.Split("##").ToList();
                    if (!data[0].Contains("http://ec2-44-202-212-224.compute-1.amazonaws.com/KWBfront/img/upload/b5f0da8d-ed39-44b6-b0dc-57273082a23f.jpg"))
                    {
                        for(var i=0;i<data.Count();i++)
                        {
                            if (i == 0)
                            {
                                photos = photos + "http://ec2-44-202-212-224.compute-1.amazonaws.com/KWBfront/img/upload/b5f0da8d-ed39-44b6-b0dc-57273082a23f.jpg" + "##";
                            }
                            else
                            {
                                photos = photos + data[i] + "##";
                            }
                        }
                        photos = photos.Replace("####", "##");
                        caract.Valor = photos;
                        context.Caracteristicas.Update(caract);
                        context.SaveChanges();
                    }
                }
                else
                {
                    var newCaract = new Caracteristicas()
                    {
                        Caracteristica = "photos",
                        PlaceID = establishment.PlaceID,
                        Valor = "http://ec2-44-202-212-224.compute-1.amazonaws.com/KWBfront/img/upload/b5f0da8d-ed39-44b6-b0dc-57273082a23f.jpg####"
                    };
                }
            }
        }

        public void NotKosherMainPicture()
        {
            var establishemnts = context.Place.ToList();
            var tag = context.Tag.Where(a => a.TagName == "NOT KOSHER").FirstOrDefault();
            var tipoTags = context.TipoTag.Where(a => a.TagID == tag.TagID).ToList();
            foreach (var establishment in establishemnts)
            {
                if (tipoTags.Where(a => a.PlaceID == establishment.PlaceID).FirstOrDefault() != null)
                {
                    establishment.Photo = "http://ec2-44-202-212-224.compute-1.amazonaws.com/KWBfront/img/upload/image_1670283072_mano%201%20Large.jpg";
                    context.Place.Update(establishment);
                    context.SaveChanges();
                }
            }
        }
        public void QuestionableMainPicture()
        {
            var establishemnts = context.Place.ToList();
            var tag = context.Tag.Where(a => a.TagName == "Questionable").FirstOrDefault();
            var tipoTags = context.TipoTag.Where(a => a.TagID == tag.TagID).ToList();
            foreach (var establishment in establishemnts)
            {
                if (tipoTags.Where(a => a.PlaceID == establishment.PlaceID).FirstOrDefault() != null)
                {
                    establishment.Photo = "http://ec2-44-202-212-224.compute-1.amazonaws.com/KWBfront/img/upload/interrogacion-Large.jpg";
                    context.Place.Update(establishment);
                    context.SaveChanges();
                }
            }
        }
        #endregion

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
