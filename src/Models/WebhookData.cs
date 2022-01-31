using System.ComponentModel.DataAnnotations;

namespace simple_rest_api_latin_literature.Models
{
    public class WebhookData
    {
        public Event Event { get; set; }
        public UserGetDTO User { get; set; }
        public dynamic Object { get; set; }
    }
}
