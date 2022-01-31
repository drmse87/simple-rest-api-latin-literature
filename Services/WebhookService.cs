using latin_web_api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace latin_web_api
{
    public class WebhookService : IWebhookService
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private Data.ApplicationDbContext _db;
        public WebhookService(Data.ApplicationDbContext db)
        {
            _db = db;
        }

        public Task Trigger(WebhookData data)
        {
            IEnumerable<Task> postToSubscribers = _db.Webhooks
                .Where(h => h.Event == data.Event)
                .AsEnumerable()
                .Select(w => 
                {
                    string jsonMessage = JsonSerializer.Serialize(
                        new { 
                            event_type = data.Event.ToString(),
                            details = data.Object,
                            user = data.User
                        });

                        try
                        {
                            var content = new StringContent(jsonMessage, Encoding.UTF8, "application/json");
                            var result = _httpClient.PostAsync(w.CallbackURL, content);

                            return result;
                        }
                        catch(InvalidOperationException exception)
                        {
                            // A bad URL.
                            return Task.FromException(exception);
                        }
                        catch(HttpRequestException exception)
                        {
                            // An ok URL but did not accept POST request.
                            return Task.FromException(exception);
                        }
                });

            return Task.WhenAll(postToSubscribers);
        }
    }
}