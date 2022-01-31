using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace latin_web_api.Models
{
    public class WebhookGetDTO
    {
        public int WebhookId { get; set; }
        public string CallbackURL { get; set; }
        public string Event { get; set; }
        public int AddedBy { get; set; }
        public DateTime AddedDate { get; set; }
        public Dictionary<string, Link> _links { get; set; }
        public static WebhookGetDTO FromModel(Webhook webhookToConvertToResponse)
        {
            return new WebhookGetDTO {
                WebhookId = webhookToConvertToResponse.WebhookId,
                CallbackURL = webhookToConvertToResponse.CallbackURL,
                Event = webhookToConvertToResponse.Event.ToString(),
                AddedBy = webhookToConvertToResponse.AddedBy,
                AddedDate = webhookToConvertToResponse.AddedDate,
                _links = new Dictionary<string, Link> {
                    { "not-implemented-self", new Link ($"api/v1/user/webhooks/{webhookToConvertToResponse.WebhookId}", "not-implemented-self") },
                    { "created-by", new Link ($"api/v1/users/{webhookToConvertToResponse.AddedBy}", "created-by") }
                }
            };
        }
    }
}
