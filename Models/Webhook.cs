using System;
using System.ComponentModel.DataAnnotations;

namespace web_api_assignment.Models
{
    public class Webhook
    {
        [Key]
        public int WebhookId { get; set; }
        [Required]
        public string CallbackURL { get; set; }
        [Required]
        public Event Event { get; set; }
        [Required]
        public int AddedBy { get; set; }
        [Required]
        public DateTime AddedDate { get; set; }
    }
}
