using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace EntityLayer.Entity
{
    /*
     * For using IdentityUser class, download the NuGet package:
     * 
     * Microsoft.AspNetCore.Identity
     */
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string LastName { get; set; }
    }
}
