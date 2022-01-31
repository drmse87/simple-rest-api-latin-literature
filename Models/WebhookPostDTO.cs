using System.ComponentModel.DataAnnotations;
using System;

namespace latin_web_api.Models
{
    public class WebhookPostDTO
    {
        [Required]
        [Url]
        [StringLength(2000, MinimumLength = 2)]
        public string CallbackURL { get; set; }
        [Required]
        [EnumDataType(typeof(Event))]
        public Event Event { get; set; }

        public static Webhook ToModel(WebhookPostDTO postedWebhookDtoToConvert, int userId)
        {
            return new Webhook
            {
                CallbackURL = postedWebhookDtoToConvert.CallbackURL,
                Event = postedWebhookDtoToConvert.Event,
                AddedDate = DateTime.Now,
                AddedBy = userId
            };
        }
    }
}
