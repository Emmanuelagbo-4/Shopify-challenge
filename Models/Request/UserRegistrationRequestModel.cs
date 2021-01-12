using System.ComponentModel.DataAnnotations;

namespace CodeChallenge.Models.Request
{
    public class UserRegistrationRequestModel
    {
        [Required]
        public string FirstName {get; set;}
        [Required]
        public string LastName {get; set;}
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        
        [Required]
        [MinLength(6)]
        [Compare("Confirm_Password", ErrorMessage = "Password Confirmation failed")]
        public string Password { get; set; }
        public string Confirm_Password { get; set; }
    }
}