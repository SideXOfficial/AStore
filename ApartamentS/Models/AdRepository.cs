using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApartamentS.Models
{
    public static class AdRepository
    {
        private static ApartamentsContext db = new ApartamentsContext();

        public static Ad Get(int id)
        {
            return GetAll().SingleOrDefault(x => x.Id.Equals(id));
        }

        public static List<Ad> GetAll()
        {
            return db.Ads.ToList<Ad>(); 
        }

    }
}