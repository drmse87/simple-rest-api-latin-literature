using web_api_assignment.Models;
using System.Threading.Tasks;

namespace web_api_assignment
{
    public interface IWebhookService
    {
        public Task Trigger(WebhookData data);                
    }
}