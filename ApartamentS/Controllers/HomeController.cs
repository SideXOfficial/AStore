using ApartamentS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ApartamentS.Controllers
{
    public class HomeController : Controller
    {

        Models.ApartamentsContext db = new Models.ApartamentsContext();
        public ActionResult Index()
        {
            IEnumerable<User> _Users = db.Users;
            // передаем все объекты в динамическое свойство
            ViewBag.users = _Users;
            // возвращаем представление
            return View();
        }

        
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}