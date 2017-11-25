using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ApartamentS.Models
{
    public class Ad
    {
        [Key]
        public int Id { get; set; }

        public int Author_Id { get; set; }

        public string Author_Name { get; set; }

        public DateTime DataCreat { get; set; }

        [Required(ErrorMessage = "Please type correct Title")]
        public string Title { get; set; }

        public string Description { get; set; }

        public string Address { get; set; }

        [Required(ErrorMessage = "Please type correct Phone")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Please type correct Price")]
        public int Price { get; set; }
        
    }
}