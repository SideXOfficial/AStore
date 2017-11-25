using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ApartamentS.Models
{
    public class Picture
    {
        [Key]
        public int Id { get; set; }

        public string Picture_Path { get; set; }

        public int Id_ad { get; set; }
        
    }
}