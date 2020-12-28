//using Generic.Attributes;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Generic.Models.User
{
    public class BasicUser
    {
        [Required]
        [DisplayName("User Name")]
        //[LocalizedUserDoesntExist(ErrorMessage = "User already exists with this username.")]
        public string UserName { get; set; }

        [Required]
        [DisplayName("Email")]
        [DataType(DataType.EmailAddress)]
        //[LocalizedUserDoesntExist(ErrorMessage = "User already exists with this email address.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string UserEmail { get; set; }

        [Required]
        [DisplayName("First Name")]
        public string FirstName { get; set; }

        [Required]
        [DisplayName("Last Name")]
        public string LastName { get; set; }

    }
}