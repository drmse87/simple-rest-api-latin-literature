using System.ComponentModel.DataAnnotations;

namespace web_api_assignment.Models
{
    public class WebhookData
    {
        public Event Event { get; set; }
        public UserGetDTO User { get; set; }
        public dynamic Object { get; set; }
    }
}
