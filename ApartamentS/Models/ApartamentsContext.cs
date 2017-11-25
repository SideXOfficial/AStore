using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ApartamentS.Models
{
    public class ApartamentsContext : DbContext
    {
        //public ApartamentsContext() : base("ApartamentsContext") { }
        public DbSet<User> Users { get; set; }
        public DbSet<Ad> Ads { get; set; }
        public DbSet<Picture> Pictures {get; set;}
    }
}