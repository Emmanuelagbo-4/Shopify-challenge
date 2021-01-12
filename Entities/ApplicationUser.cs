using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace CodeChallenge.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            DateCreated = DateTime.Now;
        }
        public string FirstName {get; set;}
        public string LastName {get; set;}
        public DateTime DateCreated {get; set;}
        #nullable enable
        public List<UsersFiles> UsersFiles { get; set; }
        #nullable disable
    }
}