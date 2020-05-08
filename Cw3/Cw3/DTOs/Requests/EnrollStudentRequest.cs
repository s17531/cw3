using Cw3.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Cw3.DTOs.Requests
{
    public class EnrollStudentRequest
    {
    //      ADNOTACJE

    //    [MinLength]
    //    [Range(1,10)]
    //    [CreditCard]
    
        [RegularExpression("^s[0-9]+$")]
        public string IndexNumber { get; set; }

//        [Required(ErrorMessage = "Musisz podać email")]
//        [EmailAddress]
//        public string Email { get; set; }

        [Required(ErrorMessage = "Musisz podać imię")]
        [MaxLength(100)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Musisz podać nazwisko")]
        [MaxLength(100)]
        public string LastName { get; set; }
        public DateTime Birthdate { get; set; }

        [Required]
        public string Studies { get; set; }


     

    }
}
