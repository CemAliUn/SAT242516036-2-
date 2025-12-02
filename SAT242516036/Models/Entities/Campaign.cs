using System;
using System.ComponentModel.DataAnnotations;

namespace YourProjectNamespace.Models
{
    public class Campaign
    {
        [Key]
        public int CampaignID { get; set; }
        public string CampaignName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int BonusPoints { get; set; }
    }
}
