using System;
using System.ComponentModel.DataAnnotations;

namespace YourProjectNamespace.Models
{
    public class Transaction
    {
        [Key]
        public int TransactionID { get; set; }
        public int CustomerID { get; set; }
        public int ProductID { get; set; }
        public int Quantity { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime TransactionDate { get; set; }

        // Navigation Properties
        public Customer? Customer { get; set; }
        public Product? Product { get; set; }
    }
}
