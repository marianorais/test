using KWB.Web.Models;
using KWB.Web.Models.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace KWB.Web.Controllers
{
    public class UserController : Controller
    {
        private readonly AplicationDbContext context;
        public UserController(AplicationDbContext context)
        {
            this.context = context;
        }

        #region Login functions
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(string UserName, string Password)
        {
            try
            {
                if(UserName == null || Password == null)
                {
                    return Json(new { status = "failed", message = "The password or the username is incorrect" });
                }
                var user = context.User.Where(a => (UserName == a.Name || UserName == a.LastName || UserName == a.Email) && a.Password == Password).FirstOrDefault();
                if (user != null)
                {
                    HttpContext.Session.SetString("authstatus", "true");
                    HttpContext.Session.SetString("userID", user.UserId.ToString());
                    HttpContext.Session.SetString("userName", user.CompleteName);

                    UserChange newUserChange = new CommonFunctions().CreateUserChange(user.UserId, "Login",user.Name);
                    context.UserChange.Add(newUserChange);
                    context.SaveChanges();

                    return Json(new { status = "success" });
                }
                else
                {
                    return Json(new { status = "failed", message = "The password or the username is incorrect" });
                }
            }
            catch(Exception exc)
            {
                return Json(new { status = "failed", message = exc.Message.ToString() });
            }
            
        }
        #endregion
     
        #region Show functions
        public IActionResult UserChange()
        {
            var status = HttpContext.Session.GetString("authstatus");
            var userID = HttpContext.Session.GetString("userID");
            if(status != "true")
            {
                return RedirectToAction("Login", "User");
            }

            UserChange newUserChange = new CommonFunctions().CreateUserChange(Int32.Parse(userID), "page user changes", null);
            context.UserChange.Add(newUserChange);
            context.SaveChanges();

            var users = context.User.ToList();
            var userChanges = context.UserChange.ToList();
            foreach(var userChange in userChanges)
            {
                userChange.UserName = users.Where(a=>a.UserId == userChange.UserID).Select(a=>a.Name).FirstOrDefault();
                userChange.InterfaceName = "";
            }
            return View(userChanges);
        }
        public IActionResult ShowUsers()
        {
            var status = HttpContext.Session.GetString("authstatus");
            var userID = HttpContext.Session.GetString("userID");
            if (status != "true")
            {
                return RedirectToAction("Login", "User");
            }
            
            UserChange newUserChange = new CommonFunctions().CreateUserChange(Int32.Parse(userID), "page show users", null);
            context.UserChange.Add(newUserChange);
            context.SaveChanges();

            ResponseUser response = new();
            response.UsersDB = context.User.ToList();

            for (var i = 0; i < response.UsersDB.Count(); i++)
            {
                if (response.UsersDB[i].Picture != null && !response.UsersDB[i].Picture.Contains("data:image"))
                {
                    response.UsersDB[i].Picture = "data:image/png;base64," + response.UsersDB[i].Picture;
                }
            }
            return View(response);
        }
        public IActionResult ShowFavorites()
        {
            var status = HttpContext.Session.GetString("authstatus");
            if(status != "true")
            {
                return RedirectToAction("Login", "User");
            }
            
            var userID = HttpContext.Session.GetString("userID");
            UserChange newUserChange = new CommonFunctions().CreateUserChange(Int32.Parse(userID), "page show favorites", null);
            context.UserChange.Add(newUserChange);
            context.SaveChanges();

            ResponseFavorite response = new();
            var places = context.Place.ToList();
            var users = context.User.ToList();
            response.FavoritesDB = context.Favorite.ToList();
            for (var i = 0; i < response.FavoritesDB.Count(); i++)
            {
                response.FavoritesDB[i].Place = places.Where(a => a.PlaceID == response.FavoritesDB[i].PlaceID).Select(a => a.Name).FirstOrDefault();
                response.FavoritesDB[i].User = users.Where(a => a.UserId == response.FavoritesDB[i].UserID).Select(a => a.Name).FirstOrDefault();
            }
            return View(response);
        }
        #endregion

        #region Delete functions
        [HttpPost]
        public IActionResult DeleteUser(int id)
        {
            try
            {

                var user = context.User.Where(a => a.UserId == id).FirstOrDefault();

                var userID = HttpContext.Session.GetString("userID");
                UserChange newUserChange = new CommonFunctions().CreateUserChange(Int32.Parse(userID), "page show users removing", user.Email);
                context.UserChange.Add(newUserChange);
                context.SaveChanges();

                var favs = context.Favorite.Where(a => a.UserID == user.UserId).ToList();
                var token = context.Tokens.Where(a => a.UserID == user.UserId).FirstOrDefault();
                if (token != null)
                {
                    context.Tokens.Remove(token);
                    context.SaveChanges();
                }
                foreach (var fav in favs)
                {
                    context.Favorite.Remove(fav);
                    context.SaveChanges();
                }
                context.User.Remove(user);
                context.SaveChanges();
                return Json(new { status = "success" });
            }
            catch (Exception exc)
            {
                return Json(new { status = "failed", message = "The user could not remove" });
            }
        }
        [HttpPost]
        public IActionResult DeleteFavorite(int id)
        {
            try
            {
                var favorite = context.Favorite.Where(a => a.FavoriteID == id).FirstOrDefault();
                context.Favorite.Remove(favorite);
                context.SaveChanges();

                var userID = HttpContext.Session.GetString("userID");
                UserChange newUserChange = new CommonFunctions().CreateUserChange(Int32.Parse(userID), "page show favorites removing", favorite.UserID + " " + favorite.PlaceID);
                context.UserChange.Add(newUserChange);
                context.SaveChanges();

                return Json(new { status = "success" });
            }
            catch (Exception exc)
            {
                return Json(new { status = "failed", message = "The favorite could not remove" });
            }
        }
        #endregion
    }
}
