using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ApartamentS.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter correct First_Name")]
        public string First_Name { get; set; }

        [Required(ErrorMessage = "Please enter correct Second_Name")]
        public string Second_Name { get; set; }

        [Required(ErrorMessage = "Please enter correct Patronymic")]
        public string Patronymic { get; set; }

        [Required(ErrorMessage = "Please enter correct Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please enter correct Login")]
        public string Login { get; set; }

        public DateTime DataCreat { get; set; }
    }
}