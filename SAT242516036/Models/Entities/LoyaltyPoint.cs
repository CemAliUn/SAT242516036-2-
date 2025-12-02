using System;
using System.ComponentModel.DataAnnotations;

namespace YourProjectNamespace.Models
{
    public class LoyaltyPoint
    {
        [Key]
        public int PointID { get; set; }
        public int CustomerID { get; set; }
        public int PointsEarned { get; set; }
        public int PointsUsed { get; set; }
        public DateTime Date { get; set; }

        // Navigation Property
        public Customer? Customer { get; set; }
    }
}
