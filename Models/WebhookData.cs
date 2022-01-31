using System.ComponentModel.DataAnnotations;

namespace latin_web_api.Models
{
    public class WebhookData
    {
        public Event Event { get; set; }
        public UserGetDTO User { get; set; }
        public dynamic Object { get; set; }
    }
}
