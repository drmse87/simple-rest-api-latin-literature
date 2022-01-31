using simple_web_api_latin_literature.Models;
using System.Threading.Tasks;

namespace simple_web_api_latin_literature
{
    public interface IWebhookService
    {
        public Task Trigger(WebhookData data);                
    }
}