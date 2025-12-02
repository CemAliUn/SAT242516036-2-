using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace YourProjectNamespace.Models
{
    public class Customer
    {
        [Key]
        public int CustomerID { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public DateTime JoinDate { get; set; }

        // Navigation Properties
        public ICollection<Transaction>? Transactions { get; set; }
        public ICollection<LoyaltyPoint>? LoyaltyPoints { get; set; }
    }
}
