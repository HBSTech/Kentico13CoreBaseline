﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DancingGoat.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Please enter your email / user name")]
        [DisplayName("Email / User name")]
        [MaxLength(100, ErrorMessage = "Maximum allowed length of the input text is {1}")]
        public string UserName { get; set; }


        [DataType(DataType.Password)]
        [DisplayName("Password")]
        [MaxLength(100, ErrorMessage = "Maximum allowed length of the input text is {1}")]
        public string Password { get; set; }


        [DisplayName("Stay signed in")]
        public bool StaySignedIn { get; set; }
    }
}