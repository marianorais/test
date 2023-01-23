using KWB.Web.Models;
using KWB.Web.Models.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Threading.Tasks;

namespace KWB.Web.Controllers
{
    public class ServerController : Controller
    {
        private readonly AplicationDbContext context;
        public ServerController(AplicationDbContext context)
        {
            this.context = context;
        }

        #region Show views
        public IActionResult ShowSponsor()
        {
            string status = HttpContext.Session.GetString("authstatus");
            string userID = HttpContext.Session.GetString("userID");
            if(status != "true")
            {
                return RedirectToAction("Login", "User");
            }

            UserChange newUserChange = new CommonFunctions().CreateUserChange(Int32.Parse(userID), "page show sponsors", null);
            context.UserChange.Add(newUserChange);
            context.SaveChanges();

            var sponsors = context.Sponsor.ToList();
            return View(sponsors);
        }
        public IActionResult ShowFaq()
        {
            var status = HttpContext.Session.GetString("authstatus");
            var userID = HttpContext.Session.GetString("userID");
            if(status != "true")
            {
                return RedirectToAction("Login", "User");
            }

            UserChange newUserChange = new CommonFunctions().CreateUserChange(Int32.Parse(userID), "page show faqs", null);
            context.UserChange.Add(newUserChange);
            context.SaveChanges();

            var faqs = context.Faq.ToList();
            return View(faqs);
        }
        public IActionResult ApiRegister()
        {
            var status = HttpContext.Session.GetString("authstatus");
            var userID = HttpContext.Session.GetString("userID");
            if(status != "true")
            {
                return RedirectToAction("Login", "User");
            }

            UserChange newUserChange = new CommonFunctions().CreateUserChange(Int32.Parse(userID), "page show api register", null);
            context.UserChange.Add(newUserChange);
            context.SaveChanges();

            var apisRegister = context.ApiCallsRegister.ToList();
            List<ApiCallsRegister> distinct = apisRegister.GroupBy(api => api.Name).Select(x => new ApiCallsRegister
            {
                Name = x.Key,
                Quantity = x.Sum(y => y.Count)
            }).ToList();

            return View(distinct);
        }
        public IActionResult ShowUserReport()
        {
            string status = HttpContext.Session.GetString("authstatus");
            string userID = HttpContext.Session.GetString("userID");
            if(status != "true")
            {
                return RedirectToAction("Login", "User");
            }

            UserChange newUserChange = new CommonFunctions().CreateUserChange(Int32.Parse(userID), "page show user reports", null);
            context.UserChange.Add(newUserChange);
            context.SaveChanges();

            ResponseUserReport response = new();
            var userReports = context.UserReport.ToList();
            var place = context.Place.ToList();
            var users = context.User.ToList();
            response.UserReportDB = userReports;
            for (var i = 0; i < userReports.Count(); i++)
            {
                response.UserReportDB[i].Place = place.Where(a => a.PlaceID == response.UserReportDB[i].PlaceID).Select(a => a.Name).FirstOrDefault();
                response.UserReportDB[i].User = users.Where(a => a.UserId == response.UserReportDB[i].UserID).FirstOrDefault();
            }
            return View(response);
        }
        public IActionResult ShowSuggestion()
        {
            var status = HttpContext.Session.GetString("authstatus");
            var userID = HttpContext.Session.GetString("userID");
            if(status != "true")
            {
                return RedirectToAction("Login", "User");
            }

            UserChange newUserChange = new CommonFunctions().CreateUserChange(Int32.Parse(userID), "page show suggestions", null);
            context.UserChange.Add(newUserChange);
            context.SaveChanges();

            ResponseSuggestion response = new();
            response.SuggestionsDB = context.Suggestion.ToList();
            return View(response);
        }
        public IActionResult ShowContactMessage()
        {
            var status = HttpContext.Session.GetString("authstatus");
            var userID = HttpContext.Session.GetString("userID");
            if(status != "true")
            {
                return RedirectToAction("Login", "User");
            }

            UserChange newUserChange = new CommonFunctions().CreateUserChange(Int32.Parse(userID), "page show contact message", null);
            context.UserChange.Add(newUserChange);
            context.SaveChanges();

            var certifications = context.Certification.ToList();
            ResponseContactMessage response = new();
            response.ContactMessageDB = context.ContactMessage.ToList();
            return View(response);
        }
        #endregion

        #region Add views
        public IActionResult AddSponsor()
        {
            //string status = HttpContext.Session.GetString("authstatus");
            //string userID = HttpContext.Session.GetString("userID");
            //if(status != "true")
            //{
            //    return RedirectToAction("Login", "User");
            //}

            //UserChange newUserChange = new CommonFunctions().CreateUserChange(Int32.Parse(userID), "page add sponsor", null);
            //context.UserChange.Add(newUserChange);
            //context.SaveChanges();

            var sponsors = context.Sponsor.ToList();
            return View(sponsors);
        }
        [HttpPost]
        public async Task<JsonResult> AddSponsor(Sponsor sponsor)
        {
            try
            {
                if (sponsor.Name == null)
                {
                    return Json(new { status = "failed", message = "Complete all fields" });
                }
                var sponsors = context.Sponsor.ToList();
                string filepath = ""; 
                string CurrentDirectory = Directory.GetCurrentDirectory();

                if (sponsors.Any(a=>a.Name == sponsor.Name) )
                {
                    return Json(new { status = "failed", message = "This sponsor name is already exist" });
                }

                string userID = HttpContext.Session.GetString("userID");
                UserChange newUserChange = new CommonFunctions().CreateUserChange(Int32.Parse(userID), "page add sponsor saving", sponsor.Name);
                context.UserChange.Add(newUserChange);
                context.SaveChanges();

                if (sponsor.ImageFile != null && sponsor.ImageFile.Length > 0)
                {
                    filepath = CurrentDirectory + @"\wwwroot\img\upload\";
                    long unixTime = ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds();
                    filepath = filepath + "image_" + unixTime + "_";
                    filepath = string.Format("{0}{1}", filepath, sponsor.ImageFile.FileName);
                    using (var stream = System.IO.File.Create(filepath))
                    {
                        await sponsor.ImageFile.CopyToAsync(stream);
                    }
                    //filePathSquare = "https://" + @"adminkwb.jojma.com.ar/img/upload/";
                    filepath = "http://ec2-44-202-212-224.compute-1.amazonaws.com/KWBfront/img/upload/" + "image_" + unixTime + "_";
                    filepath = string.Format("{0}{1}", filepath, sponsor.ImageFile.FileName).Replace(" ", "%20");
                }
                sponsor.Url = filepath;
                sponsor.DateCreated = System.DateTime.Now;
                sponsor.Name = sponsor.Name;

                context.Sponsor.Add(sponsor);
                context.SaveChanges();


                return Json(new { status = "success"});

            }
            catch (Exception exc)
            {
                return Json(new { status = "failed", message = exc.InnerException.ToString() });
            }
        }
        public IActionResult AddFaq()
        {
            string status = HttpContext.Session.GetString("authstatus");
            string userID = HttpContext.Session.GetString("userID");
            if(status != "true")
            {
                return RedirectToAction("Login", "User");
            }

            UserChange newUserChange = new CommonFunctions().CreateUserChange(Int32.Parse(userID), "page add faq", null);
            context.UserChange.Add(newUserChange);
            context.SaveChanges();

            var faqs = context.Faq.ToList();
            return View(faqs);
        }
        [HttpPost]
        public JsonResult AddFaq(Faq faq)
        {
            try
            {
                var faqs = context.Faq.ToList();
                if (faq.Enable == true && (faq.Question == null || faq.Answer == null)) { return Json(new { status = "failed", message = "Complete all fields" }); };

                var existFaq = faqs.Where(a => a.Question == faq.Question && a.Answer == faq.Answer).FirstOrDefault();
                if (existFaq != null)
                {
                    return Json(new { status = "failed", message = "this faq is already exist" });
                }
                else
                {
                    context.Faq.Add(faq);
                    context.SaveChanges();

                    string userID = HttpContext.Session.GetString("userID");
                    UserChange newUserChange = new CommonFunctions().CreateUserChange(Int32.Parse(userID), "page add faq saving", faq.FaqID.ToString());
                    context.UserChange.Add(newUserChange);
                    context.SaveChanges();

                    return Json(new { status = "success" });
                }
            }
            catch (Exception exc)
            {
                return Json(new { status = "failed", message = exc.InnerException.ToString() });
            }
        }
        #endregion

        #region Update functions

        [HttpPost]
        public async Task<JsonResult> UpdateSponsor(Sponsor sponsor)
        {
            if (sponsor.Name == null)
            {
                return Json(new { status = "failed", message = "complete all fields" });
            }
            var existSponsor = context.Sponsor.Where(a => a.SponsorID == sponsor.SponsorID).FirstOrDefault();
            if (existSponsor == null) { return Json(new { status = "failed", message = "this sponsor not exist, refresh the page" }); }

            var CurrentDirectory = Directory.GetCurrentDirectory();
            string filepath = CurrentDirectory + @"\wwwroot\img\upload\";

            string userID = HttpContext.Session.GetString("userID");
            UserChange newUserChange = new CommonFunctions().CreateUserChange(Int32.Parse(userID), "page show sponsor saving", sponsor.Name);
            context.UserChange.Add(newUserChange);
            context.SaveChanges();

            if (sponsor.ImageFile != null && sponsor.ImageFile.Length > 0)
            {
                long unixTime = ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds();
                filepath = filepath + "image_" + unixTime + "_";
                filepath = string.Format("{0}{1}", filepath, sponsor.ImageFile.FileName);
                using (var stream = System.IO.File.Create(filepath))
                {
                    await sponsor.ImageFile.CopyToAsync(stream);
                }
                //filePathSquare = "https://" + @"adminkwb.jojma.com.ar/img/upload/";
                filepath = "http://ec2-44-202-212-224.compute-1.amazonaws.com/KWBfront/img/upload/" + "image_" + unixTime + "_";
                filepath = string.Format("{0}{1}", filepath, sponsor.ImageFile.FileName).Replace(" ", "%20");
                existSponsor.Url = filepath;
            }
            else { existSponsor.Url = sponsor.Url == "Choose file" ? null : sponsor.Url; }
            existSponsor.Name = sponsor.Name;
            existSponsor.Text = sponsor.Text;
            existSponsor.DateExpiry = sponsor.DateExpiry;
            existSponsor.Main = sponsor.Main;
            existSponsor.Enable = sponsor.Enable;

            context.Sponsor.Update(existSponsor);
            context.SaveChanges();
            return Json(new { status = "success" });
        }
        
        [HttpPost]
        public JsonResult UpdateFaq(Faq faq)
        {
            if (faq.Enable == true && (faq.Question == null || faq.Answer == null))
            {
                return Json(new { status = "failed", message = "complete all fields" });
            }
            var existFaq = context.Faq.Where(a => a.FaqID == faq.FaqID).FirstOrDefault();
            if (existFaq == null) { return Json(new { status = "failed", message = "this faq not exist, refresh the page" }); }

            string userID = HttpContext.Session.GetString("userID");
            UserChange newUserChange = new CommonFunctions().CreateUserChange(Int32.Parse(userID), "page show faq saving", faq.FaqID.ToString());
            context.UserChange.Add(newUserChange);
            context.SaveChanges();

            existFaq.Answer = faq.Answer;
            existFaq.Question = faq.Question;
            existFaq.Enable = faq.Enable;
            context.Faq.Update(existFaq);
            context.SaveChanges();
            return Json(new { status = "success" });
        }
        
        [HttpPost]
        public IActionResult UpdateUserReport()
        {
            try
            {
                var usersReports = context.UserReport.ToList();
                foreach(var userReport in usersReports)
                {
                    userReport.WasSeen = true;
                    context.UserReport.Update(userReport);
                }
                context.SaveChanges();
                return Json(new { status = "success" });

            }
            catch(Exception exc) {
                return Json(new { status = "failed", message = exc.Message.ToString() });
            }
        }
        [HttpPost]
        public IActionResult UpdateSuggestion()
        {
            try
            {
                var suggestions = context.Suggestion.ToList();
                foreach (var suggestion in suggestions)
                {
                    suggestion.WasSeen = true;
                    context.Suggestion.Update(suggestion);
                }
                context.SaveChanges();
                return Json(new { status = "success" });

            }
            catch (Exception exc)
            {
                return Json(new { status = "failed", message = exc.Message.ToString() });
            }
        }
        [HttpPost]
        public IActionResult UpdateContactMessage()
        {
            try
            {
                var contactsMessage = context.ContactMessage.ToList();
                foreach (var contactMessage in contactsMessage)
                {
                    contactMessage.WasSeen = true;
                    context.ContactMessage.Update(contactMessage);
                }
                context.SaveChanges();
                return Json(new { status = "success" });

            }
            catch (Exception exc)
            {
                return Json(new { status = "failed", message = exc.Message.ToString() });
            }
        }
        #endregion

        #region Return functions
        [HttpPost]
        public JsonResult ReturnSponsor(int id)
        {
            var sponsor = context.Sponsor.Where(a => a.SponsorID == id).FirstOrDefault();

            string userID = HttpContext.Session.GetString("userID");
            UserChange newUserChange = new CommonFunctions().CreateUserChange(Int32.Parse(userID), "page show sponsor editing", sponsor.Name);
            context.UserChange.Add(newUserChange);
            context.SaveChanges();
                
            if (sponsor != null) { return Json(new { status = "success", data = sponsor }); }
            return Json(new { status = "failed", message = "faq not found" });
        }

        [HttpPost]
        public JsonResult ReturnFaq(int id)
        {
            var faq = context.Faq.Where(a => a.FaqID == id).FirstOrDefault();

            string userID = HttpContext.Session.GetString("userID");
            UserChange newUserChange = new CommonFunctions().CreateUserChange(Int32.Parse(userID), "page show faq editing", id.ToString());
            context.UserChange.Add(newUserChange);
            context.SaveChanges();

            if (faq != null) { return Json(new { status = "success", data = faq }); }
            return Json(new { status = "failed", message = "faq not found" });
        }
        #endregion

        #region Delete functions
        [HttpPost]
        public IActionResult DeleteSponsor(int id)
        {
            try
            {
                var sponsor = context.Sponsor.Where(a => a.SponsorID == id).FirstOrDefault();
                context.Sponsor.Remove(sponsor);
                context.SaveChanges();

                string userID = HttpContext.Session.GetString("userID");
                UserChange newUserChange = new CommonFunctions().CreateUserChange(Int32.Parse(userID), "page show sponsor removing", sponsor.Name);
                context.UserChange.Add(newUserChange);
                context.SaveChanges();

                return Json(new { status = "success" });
            }
            catch (Exception exc)
            {
                return Json(new { status = "failed", message = "The sponsor could not remove" });
            }
        }
        [HttpPost]
        public IActionResult DeleteContactMessage(int id)
        {
            try
            {
                var contactMessage = context.ContactMessage.Where(a => a.ContactMessageID == id).FirstOrDefault();
                context.ContactMessage.Remove(contactMessage);
                context.SaveChanges();

                string userID = HttpContext.Session.GetString("userID");
                UserChange newUserChange = new CommonFunctions().CreateUserChange(Int32.Parse(userID), "page show contact message removing", contactMessage.Message);
                context.UserChange.Add(newUserChange);
                context.SaveChanges();

                return Json(new { status = "success" });
            }
            catch (Exception exc)
            {
                return Json(new { status = "failed", message = "The contactMessage could not remove" });
            }
        }
        [HttpPost]
        public IActionResult DeleteSuggestion(int id)
        {
            try
            {
                var suggestion = context.Suggestion.Where(a => a.SuggestID == id).FirstOrDefault();
                context.Suggestion.Remove(suggestion);
                context.SaveChanges();

                string userID = HttpContext.Session.GetString("userID");
                UserChange newUserChange = new CommonFunctions().CreateUserChange(Int32.Parse(userID), "page show suggestion removing", suggestion.Name);
                context.UserChange.Add(newUserChange);
                context.SaveChanges();

                return Json(new { status = "success" });
            }
            catch (Exception exc)
            {
                return Json(new { status = "failed", message = "The contactMessage could not remove" });
            }
        }
        [HttpPost]
        public IActionResult DeleteUserReport(int id)
        {
            try
            {
                var userReport = context.UserReport.Where(a => a.UserReportID == id).FirstOrDefault();
                context.UserReport.Remove(userReport);
                context.SaveChanges();

                string userReportMessage = userReport.Message.Length > 100 ? userReport.Message.Substring(0, 100) : userReport.Message;
                string userID = HttpContext.Session.GetString("userID");
                UserChange newUserChange = new CommonFunctions().CreateUserChange(Int32.Parse(userID), "page show user report removing", userReportMessage);
                context.UserChange.Add(newUserChange);
                context.SaveChanges();

                return Json(new { status = "success" });
            }
            catch (Exception exc)
            {
                return Json(new { status = "failed", message = "The contactMessage could not remove" });
            }
        }

        [HttpPost]
        public JsonResult DeleteFaq(int id)
        {
            try
            {

                var existFaq = context.Faq.Where(a => a.FaqID == id).FirstOrDefault();
                if (existFaq != null)
                {
                    string userID = HttpContext.Session.GetString("userID");
                    string questionSaved = existFaq.Question.Length > 100 ? existFaq.Question.Substring(0,100) : existFaq.Question;
                    UserChange newUserChange = new CommonFunctions().CreateUserChange(Int32.Parse(userID), "page show faq removing", questionSaved);
                    context.UserChange.Add(newUserChange);
                    context.SaveChanges();

                    context.Faq.Remove(existFaq);
                    context.SaveChanges();
                    return Json(new { status = "success" });
                }
                else { return Json(new { status = "failed", message = "this faq not exist" }); }
            }
            catch (Exception exc)
            {
                return Json(new { status = "failed", message = exc.InnerException.ToString() });
            }

        }
        #endregion
    }
}
