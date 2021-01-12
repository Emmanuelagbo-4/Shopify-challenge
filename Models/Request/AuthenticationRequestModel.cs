using System.ComponentModel.DataAnnotations;

namespace CodeChallenge.Models.Request
{
    public class AuthenticationRequestModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}