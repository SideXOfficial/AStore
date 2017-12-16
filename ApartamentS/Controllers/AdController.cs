using ApartamentS.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;

namespace ApartamentS.Controllers
{
    public class AdController : Controller
    {
        Models.ApartamentsContext db = new Models.ApartamentsContext();

        public ActionResult Ads()
        {
            string find;
            IQueryable<Ad> _Ads = db.Ads;
            if ((find = this.Request.QueryString["Title"]) != null)
            {
                Ad _ads = new Ad();
                ViewBag.ads = LuceneSearch.Search(find);
            }
            else
            {
                ViewBag.ads = _Ads;
            }
            return View();
        }

        public ActionResult Find(Ad _ad)
        {
            return RedirectToAction("Ads", new { Title = _ad.Title});
        }
        [HttpGet]
        [Authorize]
        public ActionResult add_Ad()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public ActionResult add_Ad(Ad _ad, Picture _picture, IEnumerable<HttpPostedFileBase> upload)
        {
            if (ModelState.IsValid)
            {
                using (ApartamentsContext db = new ApartamentsContext())
                {
                    _ad.Author_Name = User.Identity.Name;

                    var auth_user = db.Users.Single(u => u.Login == _ad.Author_Name);

                    _ad.Author_Id = auth_user.Id;

                    foreach (HttpPostedFileBase picture in upload)
                    {
                        if (picture != null)
                        {
                            var path = Path.Combine(Server.MapPath("~/Images/"), picture.FileName); //файл
                            picture.SaveAs(path);
                            _picture.Picture_Path = "/Images/" + picture.FileName; // путь к файлу в базу - его имя
                            _picture.Id_ad = (int)db.Ads.LongCount() + 14 ;
                            db.Pictures.Add(_picture);
                            db.SaveChanges();

                        }
                    }

                    _ad.DataCreat = DateTime.Now;
                    // добавляем информацию о покупке в базу данных
                    db.Ads.Add(_ad);
                    // сохраняем в бд все изменения
                    db.SaveChanges();

                    LuceneSearch.AddUpdateLuceneIndex(AdRepository.GetAll());

                    return RedirectToAction("Ads");
                }
            }
            return View();
        }
    }
}