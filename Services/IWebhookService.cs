using latin_web_api.Models;
using System.Threading.Tasks;

namespace latin_web_api
{
    public interface IWebhookService
    {
        public Task Trigger(WebhookData data);                
    }
}