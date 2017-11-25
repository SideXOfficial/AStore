using ApartamentS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace ApartamentS.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ViewResult Register()
        {
            ViewBag.Message = "";
            return View();
        }

        [HttpPost]
        public ActionResult Register(User UserRegister)
        {
            if (ModelState.IsValid)
            {
                using (ApartamentsContext db = new ApartamentsContext())
                {
                    try
                    {
                        var test = db.Users.Single(u => u.Login == UserRegister.Login && u.Password == UserRegister.Password);
                        ModelState.Clear();
                        ViewBag.Message = "User is found.";
                        return View();
                    }
                    catch
                    {
                        UserRegister.DataCreat = DateTime.Now;
                        // добавляем информацию о покупке в базу данных
                        db.Users.Add(UserRegister);
                        // сохраняем в бд все изменения
                        db.SaveChanges();
                        ModelState.Clear();
                        ViewBag.Message = "";
                        FormsAuthentication.SetAuthCookie(UserRegister.Login, true);
                    }
                }
                //ViewBag.Message = "Welcome to my first site, " + UserRegister.First_Name.ToString()+ " " + UserRegister.Second_Name.ToString();
                return RedirectToAction("Index", "Home");
            }
            else return View();

        }

        [HttpGet]
        public ActionResult Login()
        {
            ViewBag.Message = "";
            return View();
        }

        [HttpPost]
        public ActionResult Login(User user)
        {
            using (ApartamentsContext db = new ApartamentsContext())
            {
                try
                {
                    var auth_user = db.Users.Single(u => u.Login == user.Login && u.Password == user.Password);

                    FormsAuthentication.SetAuthCookie(auth_user.Login, true);

                    ViewBag.Message = "";
                    string return_url;
                    if ((return_url = this.Request.QueryString["ReturnUrl"]) != null)
                    {
                        string go = "http://" + Request.UrlReferrer.Authority.ToString() + return_url;
                        return Redirect(go);
                    }
                    return RedirectToAction("Index", "Home");
                }
                catch
                {
                    ViewBag.Message = "Username or password is wrong.";
                }
            }
            return View();
        }
        
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

    }
}