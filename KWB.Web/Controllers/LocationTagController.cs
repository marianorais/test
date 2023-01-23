using KWB.Web.Models;
using KWB.Web.Models.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace KWB.Web.Controllers
{
    public class LocationTagController : Controller
    {
        private readonly AplicationDbContext context;
        public LocationTagController(AplicationDbContext context)
        {
            this.context = context;
        }
    
        #region Show functions
        public IActionResult ShowThingsToDo()
        {
            string status = HttpContext.Session.GetString("authstatus");
            string userID = HttpContext.Session.GetString("userID");
            if(status != "true")
            {
                return RedirectToAction("Login", "User");
            }
            UserChange newUserChange = new CommonFunctions().CreateUserChange(Int32.Parse(userID), "page show things to do", null);
            context.UserChange.Add(newUserChange);
            context.SaveChanges();

            ResponseThingToDo response = new();
            var locationsTags = context.LocationTag.OrderBy(a => a.Name).ToList();
            response.ThingsToDoDB = context.ThingsToDo.ToList();
            response.LocationTagDB = locationsTags.Select(a => a.Name).ToList();
            for (var i = 0; i < response.ThingsToDoDB.Count(); i++)
            {
                response.ThingsToDoDB[i].LocationTag = locationsTags.Where(a => a.CityID == response.ThingsToDoDB[i].PlaceID).Select(a => a.Name).FirstOrDefault();
            }
            return View(response);
        }
        public IActionResult ShowLocations()
        {
            string status = HttpContext.Session.GetString("authstatus");
            string userID = HttpContext.Session.GetString("userID");
            if(status != "true")
            {
                return RedirectToAction("Login", "User");
            }
            UserChange newUserChange = new CommonFunctions().CreateUserChange(Int32.Parse(userID), "page show location tag", null);
            context.UserChange.Add(newUserChange);
            context.SaveChanges();

            ResponseLocationTag response = new();
            var thingToDoDB = context.ThingsToDo.OrderBy(a => a.Value).ToList();
            var tags = context.Tag.OrderBy(a => a.TagName).ToList();
            var listThingsToDo = new List<string>();
            response.LocationTagDB = context.LocationTag.OrderBy(a => a.Name).ToList();
            response.TagsNotMain = tags.Where(a => a.Main == false).Select(a => a.TagName).ToList();
            response.TagsMain = tags.Where(a => a.Main == true).Select(a => a.TagName).ToList();
            for (var i = 0; i < response.LocationTagDB.Count(); i++)
            {
                var thingsToDo = thingToDoDB.Where(a => a.PlaceID == response.LocationTagDB[i].CityID).ToList();
                response.LocationTagDB[i].ThingsToDo = thingsToDo.Select(a => a.Value).ToList();
            }
            response.ThingsToDoDB = thingToDoDB.Select(a => a.Value).ToList();
            return View(response);
        }
        #endregion
        
        #region Add functions
        public IActionResult AddLocationTag()
        {
            var status = HttpContext.Session.GetString("authstatus");
            var userID = HttpContext.Session.GetString("userID");
            if(status != "true")
            {
                return RedirectToAction("Login", "User");
            }
            UserChange newUserChange = new CommonFunctions().CreateUserChange(Int32.Parse(userID), "page add location tag", null);
            context.UserChange.Add(newUserChange);
            context.SaveChanges();

            ResponseLocationTag response = new();
            response.ThingsToDoDB = context.ThingsToDo.OrderBy(a => a.Value).Select(a => a.Value).ToList();
            response.Tags = context.Tag.OrderBy(a => a.TagName).Select(a => a.TagName).ToList();
            return View(response);
        }
        
        [HttpPost]
        public async Task<IActionResult> AddLocationTag(ResponseLocationTag response)
        {
            try
            {
                if (response.Name == null)
                {
                    return Json(new { status = "failed", message = "name must be completed" });
                }
                if (response.Enable == true && (response.Lat == null || response.Lng == null || response.RectangleImage == null || response.SquareImage == null || response.Description == null || response.Highlights == null))
                {
                    return Json(new { status = "failed", message = "Complete all fields" });
                }

                string userID = HttpContext.Session.GetString("userID");
                UserChange newUserChange = new CommonFunctions().CreateUserChange(Int32.Parse(userID), "page add location tag saving", response.Name);
                context.UserChange.Add(newUserChange);
                context.SaveChanges();

                var allLocationTag = context.LocationTag.ToList();
                var thingsToDo = context.ThingsToDo.ToList();
                var listThingsToDoID = new List<int>();
                var CurrentDirectory = Directory.GetCurrentDirectory();
                
                string filePathSquare = CurrentDirectory + @"\wwwroot\img\upload\";
                string filePathRectangle = CurrentDirectory + @"\wwwroot\img\upload\";

                if (allLocationTag.Any(a => a.Name == response.Name)) { return Json(new { status = "failed", message = "The locationTag it already exists" }); }

                if (response.SquareImage != null && response.SquareImage.Length > 0)
                {
                    long unixTime = ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds();
                    filePathSquare = filePathSquare + "image_" + unixTime + "_";
                    filePathSquare = string.Format("{0}{1}", filePathSquare, response.SquareImage.FileName);
                    using (var stream = System.IO.File.Create(filePathSquare))
                    {
                        await response.SquareImage.CopyToAsync(stream);
                    }
                    //filePathSquare = "https://" + @"adminkwb.jojma.com.ar/img/upload/";
                    filePathSquare = "http://ec2-44-202-212-224.compute-1.amazonaws.com/KWBfront/img/upload/" + "image_" + unixTime + "_";
                    filePathSquare = string.Format("{0}{1}", filePathSquare, response.SquareImage.FileName).Replace(" ", "%20");

                }
                else { filePathSquare = null; }

                if (response.RectangleImage != null && response.RectangleImage.Length > 0)
                {
                    long unixTime = ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds();
                    filePathRectangle = filePathRectangle + "image_" + unixTime + "_";
                    filePathRectangle = string.Format("{0}{1}", filePathRectangle, response.RectangleImage.FileName);
                    using (var stream = System.IO.File.Create(filePathRectangle))
                    {
                        await response.RectangleImage.CopyToAsync(stream);
                    }
                    //filePathRectangle = "https://" + @"adminkwb.jojma.com.ar/img/upload/";
                    filePathRectangle = "http://ec2-44-202-212-224.compute-1.amazonaws.com/KWBfront/img/upload/" + "image_" + unixTime + "_";
                    filePathRectangle = string.Format("{0}{1}", filePathRectangle, response.RectangleImage.FileName).Replace(" ", "%20");
                }
                else { filePathRectangle = null; }

                var newLocationTag = new CommonFunctions().CreateLocationTag(response,filePathRectangle,filePathSquare);
                context.LocationTag.Add(newLocationTag);
                context.SaveChanges();

                if (response.ThingsToDoResponse != null && response.ThingsToDoResponse.Count() > 0 && response.ThingsToDoResponse[0] != null)
                {

                    foreach (var thingToDo in response.ThingsToDoResponse[0].ToString().Split(",").ToList())
                    {
                        if (thingToDo != null && thingToDo.Length > 0)
                        {
                            ThingsToDo newThingToDo = new()
                            {
                                Value = thingToDo,
                                PlaceID = newLocationTag.CityID
                            };
                            context.ThingsToDo.Add(newThingToDo);
                            context.SaveChanges();
                        }
                    }
                }
                if (response.Tags != null && response.Tags.Count() > 0 && response.Tags[0] != null)
                {
                    foreach (var tag in response.Tags[0].ToString().Split(",").ToList())
                    {
                        if (tag != null && tag.Length > 0)
                        {
                            var existTag = context.Tag.Where(a => a.TagName == tag).FirstOrDefault();
                            if (existTag != null)
                            {
                                var newTipoTag = new CommonFunctions().CreateTipoTag(null, newLocationTag.CityID, existTag.TagID,false);
                                context.TipoTag.Add(newTipoTag);
                                context.SaveChanges();
                            }
                        }
                    }
                }

                return Json(new { status = "success" });
            }
            catch (Exception exc)
            {
                return Json(new { status = "failed", message = "The locationTag could not save" });
            }
        }
        public IActionResult AddThingToDo()
        {
            var status = HttpContext.Session.GetString("authstatus");
            string userID = HttpContext.Session.GetString("userID");
            if(status != "true")
            {
                return RedirectToAction("Login", "User");
            }
            UserChange newUserChange = new CommonFunctions().CreateUserChange(Int32.Parse(userID), "page add things to do", null);
            context.UserChange.Add(newUserChange);
            context.SaveChanges();

            var response = new ResponseThingToDo();
            var thingToDo = context.ThingsToDo.ToList();
            var locationTag = context.LocationTag.OrderBy(a => a.Name).Select(locationTag => locationTag.Name).ToList();
            response.LocationTagDB = locationTag;
            response.ThingsToDoDB = thingToDo;
            return View(response);
        }
        [HttpPost]
        public IActionResult AddThingToDo(ResponseThingToDo response)
        {
            try
            {
                if (response.LocationTagSelected == "Select One" || response.Value == null)
                {
                    return Json(new { status = "failed", message = "Complete all fields" });
                }
                var locationTag = context.LocationTag.Where(locationTag => locationTag.Name == response.LocationTagSelected).FirstOrDefault();

                var userID = HttpContext.Session.GetString("userID");
                UserChange newUserChange = new CommonFunctions().CreateUserChange(Int32.Parse(userID), "page add things to do saving", locationTag.Name);
                context.UserChange.Add(newUserChange);
                context.SaveChanges();

                var newThingToDo = new ThingsToDo()
                {
                    PlaceID = locationTag.CityID,
                    Value = response.Value
                };
                context.ThingsToDo.Add(newThingToDo);
                context.SaveChanges();
                return Json(new { status = "success" });
            }
            catch (Exception exc)
            {
                return Json(new { status = "failed", message = exc.Message.ToString() });
            }
        }
        #endregion
        
        #region Update functions
        [HttpPost]
        public IActionResult UpdateThingToDo(ResponseThingToDo response)
        {
            try
            {
                if (response.Value == null) { return Json(new { status = "failed", message = "complete all fields" }); }
                if (response.LocationTagDB[0] == null) { return Json(new { status = "failed", message = "complete all fields" }); }

                var userID = HttpContext.Session.GetString("userID");
                UserChange newUserChange = new CommonFunctions().CreateUserChange(Int32.Parse(userID), "page show things to do saving", response.LocationTagDB[0]);
                context.UserChange.Add(newUserChange);
                context.SaveChanges();

                var locationTag = context.LocationTag.Where(a => a.Name == response.LocationTagDB[0]).FirstOrDefault();
                var thingToDo = context.ThingsToDo.Where(a => a.ID == response.ID).FirstOrDefault();
                thingToDo.Value = response.Value;
                thingToDo.PlaceID = locationTag.CityID;
                context.ThingsToDo.Update(thingToDo);
                context.SaveChanges();

                return Json(new { status = "success" });
            }
            catch (Exception exc)
            {
                return Json(new { status = "failed", message = "the Thing To Do could not save" });
            }
        }
        [HttpPost]
        public async Task<IActionResult> UpdateLocationTag(ResponseLocationTag response)
        {
            try
            {
                if (response.Name == null)
                {
                    return Json(new { status = "failed", message = "name must be completed" });
                }
                if (response.Enable == true && (response.Lat == null || response.Lng == null || response.Description == null || response.Highlights == null))
                {
                    return Json(new { status = "failed", message = "Complete all the fields" });
                }
                if (response.Enable == true && response.TagsMain[0] == "No Tag") { return Json(new { status = "failed", message = "the main tag cannot be empty" }); }
                var locationTag = context.LocationTag.Where(a => a.CityID == response.CityID).FirstOrDefault();

                var userID = HttpContext.Session.GetString("userID");
                UserChange newUserChange = new CommonFunctions().CreateUserChange(Int32.Parse(userID), "page show location tag saving", locationTag.Name);
                context.UserChange.Add(newUserChange);
                context.SaveChanges();

                var thingsToDo = context.ThingsToDo.ToList();
                var tagsDB = context.Tag.ToList();
                var tipoTags = context.TipoTag.ToList();
                locationTag.Name = response.Name;
                locationTag.Lat = response.Lat;
                locationTag.Lng = response.Lng;
                locationTag.Description = response.Description;
                locationTag.Highlights = response.Highlights;
                locationTag.Enable = response.Enable;
                if (response.SquareImage != null && response.SquareImage.Length > 0)
                {
                    long unixTime = ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds();
                    var CurrentDirectory = Directory.GetCurrentDirectory();
                    string filePath = CurrentDirectory + @"\wwwroot\img\upload\" + "image_" + unixTime + "_";
                    filePath = string.Format("{0}{1}", filePath, response.SquareImage.FileName);
                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await response.SquareImage.CopyToAsync(stream);
                    }
                    //filePath = "https://" + @"adminkwb.jojma.com.ar/img/upload/";
                    filePath = "http://ec2-44-202-212-224.compute-1.amazonaws.com/KWBfront/img/upload/" + "image_" + unixTime + "_";
                    filePath = string.Format("{0}{1}", filePath, response.SquareImage.FileName).Replace(" ", "%20");
                    locationTag.SquareImage = filePath;
                }
                if (response.RectangleImage != null && response.RectangleImage.Length > 0)
                {
                    long unixTime = ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds();
                    var CurrentDirectory = Directory.GetCurrentDirectory();
                    string filePath = CurrentDirectory + @"\wwwroot\img\upload\" + "image_" + unixTime + "_";
                    filePath = string.Format("{0}{1}", filePath, response.RectangleImage.FileName);
                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await response.RectangleImage.CopyToAsync(stream);
                    }
                    //filePath = @"adminkwb.jojma.com.ar/img/upload/";
                    filePath = "http://ec2-44-202-212-224.compute-1.amazonaws.com/KWBfront/img/upload/" + "image_" + unixTime + "_";
                    filePath = string.Format("{0}{1}", filePath, response.RectangleImage.FileName).Replace(" ", "%20");
                    locationTag.RectangleImage = filePath;
                }
                context.LocationTag.Update(locationTag);
                context.SaveChanges();
                if (response.ThingsToDoResponse[0] != null)
                {
                    var listThingsToDo = new List<string>();
                    listThingsToDo = response.ThingsToDoResponse[0].Split(",").ToList();
                    foreach (var thingToDo in listThingsToDo)
                    {
                        if (thingsToDo.Where(a => a.PlaceID == locationTag.CityID && a.Value == thingToDo).FirstOrDefault() == null)
                        {
                            ThingsToDo newThingToDo = new()
                            {
                                PlaceID = locationTag.CityID,
                                Value = thingToDo
                            };
                            context.ThingsToDo.Add(newThingToDo);
                            context.SaveChanges();
                        }
                    }
                    var oldThingsToDo = thingsToDo.Where(a => a.PlaceID == locationTag.CityID).ToList();
                    foreach (var oldThingToDo in oldThingsToDo)
                    {
                        var thingToDo = listThingsToDo.Any(w => oldThingToDo.Value.Contains(w));
                        if (thingToDo == false)
                        {
                            context.ThingsToDo.Remove(oldThingToDo);
                            context.SaveChanges();
                        }
                    }
                }
                if (response.TagsNotMain[0] != null)
                {
                    var listTags = new List<string>();
                    listTags = response.TagsNotMain[0].Split(",").ToList();
                    for (var i = 0; i < listTags.Count(); i++)
                    {
                        var tag = tagsDB.Where(a => a.TagName == listTags[i]).Select(a => a.TagID).FirstOrDefault();
                        var existTipoTag = tipoTags.Where(a => a.CityID == response.CityID && a.TagID == tag).FirstOrDefault();
                        if (existTipoTag == null)
                        {
                            TipoTag newTipoTag = new TipoTag
                            {
                                CityID = response.CityID,
                                TagID = tag,
                                Main = false
                            };
                            context.TipoTag.Add(newTipoTag);
                            context.SaveChanges();
                        }
                    }
                    var oldTags = tipoTags.Where(a => a.CityID == response.CityID).ToList();
                    for (var i = 0; i < oldTags.Count(); i++)
                    {
                        var oldTagName = tagsDB.Where(a => a.TagID == oldTags[i].TagID && a.Main == false).Select(a => a.TagName).FirstOrDefault();
                        if (oldTagName != null && !listTags.Any(a => a == oldTagName)) { context.TipoTag.Remove(oldTags[i]); context.SaveChanges(); }
                    }
                }
                if (response.TagsMain[0] != null && response.TagsMain[0] != "No Tag")
                {
                    var tag = tagsDB.Where(a => a.TagName == response.TagsMain[0] && a.Main == true && a.TagName != "must_see!").FirstOrDefault();
                    var existTag = tipoTags.Where(a => a.TagID == tag.TagID && a.CityID == response.CityID).FirstOrDefault();
                    if (tag != null && existTag == null)
                    {
                        TipoTag newTipoTag = new TipoTag
                        {
                            CityID = response.CityID,
                            TagID = tag.TagID,
                            Main = true
                        };
                        context.TipoTag.Add(newTipoTag);
                        context.SaveChanges();
                        var oldMainTag = tipoTags.Where(a => a.TagID != tag.TagID && a.CityID == response.CityID && a.Main == true).FirstOrDefault();
                        if (oldMainTag != null) { context.TipoTag.Remove(oldMainTag); context.SaveChanges(); }
                    }
                }

                return Json(new { status = "success" });
            }
            catch (Exception exc)
            {
                return Json(new { status = "failed", message = "the locationTag was not found" });
            }
        }
        #endregion
        
        #region Return functions
        [HttpPost]
        public IActionResult ReturnLocationTag(int id)
        {
            try
            {
                var locationTagSelected = context.LocationTag.Where(a => a.CityID == id).FirstOrDefault();

                var userID = HttpContext.Session.GetString("userID");
                UserChange newUserChange = new CommonFunctions().CreateUserChange(Int32.Parse(userID), "page show location tag editing", locationTagSelected.Name);
                context.UserChange.Add(newUserChange);
                context.SaveChanges();

                var thingsToDo = context.ThingsToDo.Where(a => a.PlaceID == id).OrderBy(a => a.Value).Select(a => a.Value).ToList();
                var tagsID = context.TipoTag.Where(a => a.CityID == id).Select(a => a.TagID).ToList();
                var tagsDB = context.Tag.OrderBy(a => a.TagName).ToList();
                var tagsLocation = new CommonFunctions().ReturnTag(tagsID, tagsDB);
                locationTagSelected.TagsMain = tagsLocation.Where(a => a.Main == true).Select(a => a.TagName).ToList();
                locationTagSelected.TagsNotMain = tagsLocation.Where(a => a.Main == false).Select(a => a.TagName).ToList();
                locationTagSelected.ThingsToDo = thingsToDo;
                return Json(new { status = "success", data = locationTagSelected });
            }
            catch (Exception exc)
            {
                return Json(new { status = "failed", message = "the locationTag was not found" });
            }
        }
        [HttpPost]
        public IActionResult ReturnThingToDo(int id)
        {
            try
            {
                ResponseThingToDo response = new();
                var thingToDo = context.ThingsToDo.Where(a => a.ID == id).FirstOrDefault();

                thingToDo.LocationTag = context.LocationTag.Where(a => a.CityID == thingToDo.PlaceID).Select(a => a.Name).FirstOrDefault();

                var userID = HttpContext.Session.GetString("userID");
                UserChange newUserChange = new CommonFunctions().CreateUserChange(Int32.Parse(userID), "page show things to do editing", thingToDo.LocationTag);
                context.UserChange.Add(newUserChange);
                context.SaveChanges();

                return Json(new { status = "success", data = thingToDo });
            }
            catch (Exception exc)
            {
                return Json(new { status = "failed", message = "the tag was not found" });
            }
        }
        #endregion
      
        #region Delete functions
        [HttpPost]
        public IActionResult DeleteThingToDo(int id)
        {
            try
            {
                var thingToDo = context.ThingsToDo.Where(a => a.ID == id).FirstOrDefault();
                context.ThingsToDo.Remove(thingToDo);
                context.SaveChanges();
                
                //thingToDo.LocationTag = context.LocationTag.Where(a => a.CityID == thingToDo.PlaceID).Select(a => a.Name).FirstOrDefault();
                
                //var userID = HttpContext.Session.GetString("userID");
                //UserChange newUserChange = new CommonFunctions().CreateUserChange(Int32.Parse(userID), "page show things to do removing", thingToDo.LocationTag);
                //context.UserChange.Add(newUserChange);
                //context.SaveChanges();

                return Json(new { status = "success" });
            }
            catch (Exception exc)
            {
                return Json(new { status = "failed", message = "The thing To Do could not remove" });
            }
        }
        [HttpPost]
        public IActionResult DeleteLocationTag(int id)
        {
            try
            {
                var locationTag = context.LocationTag.Where(a => a.CityID == id).FirstOrDefault();
                //locationTag.Deleted = true;
                //context.LocationTag.Update(locationTag);
                //context.SaveChanges();

                var newCaract = new Caracteristicas
                {
                    Caracteristica = "delete",
                    CityID = locationTag.CityID,
                    Valor = null
                };
                context.Caracteristicas.Add(newCaract);
                context.SaveChanges();

                var userID = HttpContext.Session.GetString("userID");
                UserChange newUserChange = new CommonFunctions().CreateUserChange(Int32.Parse(userID), "page show location tag removing", locationTag.Name);
                context.UserChange.Add(newUserChange);
                context.SaveChanges();

                var tipoTags = context.TipoTag.Where(a => a.CityID == id).ToList();
                var thingsToDo = context.ThingsToDo.Where(a => a.PlaceID == id).ToList();
                var placeLocationTags = context.PlaceLocationTag.Where(a => a.LocationTagID == id).ToList();
                var caracteristicas = context.Caracteristicas.Where(a => a.CityID == id).ToList();

                foreach (var tipoTag in tipoTags)
                {
                    context.TipoTag.Remove(tipoTag);
                    context.SaveChanges();
                }
                foreach (var thingToDo in thingsToDo)
                {
                    context.ThingsToDo.Remove(thingToDo);
                    context.SaveChanges();
                }
                foreach (var placeLocationTag in placeLocationTags)
                {
                    context.PlaceLocationTag.Remove(placeLocationTag);
                    context.SaveChanges();
                }
                foreach (var caract in caracteristicas)
                {
                    context.Caracteristicas.Remove(caract);
                    context.SaveChanges();
                }
                context.LocationTag.Remove(locationTag);
                context.SaveChanges();
                return Json(new { status = "success" });
            }
            catch (Exception exc)
            {
                return Json(new { status = "failed", message = "The place could not remove" });
            }
        }
        #endregion
    }
}
