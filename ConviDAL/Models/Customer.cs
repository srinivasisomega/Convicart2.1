using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConviDAL.Models
{
    
        public class Customer : IdentityUser
        {
            
            [StringLength(50)]
            [RegularExpression(@"^[A-Za-z]+$", ErrorMessage = "First name must contain only letters.")]
            public string? FirstName { get; set; }

            
            [StringLength(50)]
            [RegularExpression(@"^[A-Za-z]+$", ErrorMessage = "Last name must contain only letters.")]
            public string? LastName { get; set; }

            [Range(0, 150, ErrorMessage = "Age must be between 0 and 150.")]
            public int? Age { get; set; }

            
            [StringLength(10)]
            [RegularExpression(@"^(Male|Female|Other)$", ErrorMessage = "Gender must be 'Male', 'Female', or 'Other'.")]
            public string? Gender { get; set; }

            
            [StringLength(250)]
            public string? Address { get; set; }

            // Override Email property with additional regex validation
            [Required]
            [EmailAddress]
            [RegularExpression(@"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$", ErrorMessage = "Invalid email format.")]
            public override string  Email { get; set; }

            // Override PhoneNumber property with regex for specific phone number format
            
            [Phone]
            [RegularExpression(@"^\+?\d{10,15}$", ErrorMessage = "Phone number must be 10-15 digits and can include '+' at the start.")]
            public override string? PhoneNumber { get; set; }
        }

    

}
