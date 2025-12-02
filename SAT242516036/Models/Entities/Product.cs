using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace YourProjectNamespace.Models
{
    public class Product
    {
        [Key]
        public int ProductID { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Points { get; set; }

        // Navigation Property
        public ICollection<Transaction>? Transactions { get; set; }
    }
}
