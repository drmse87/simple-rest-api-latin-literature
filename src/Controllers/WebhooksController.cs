using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using simple_rest_api_latin_literature.Models;

namespace simple_rest_api_latin_literature.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class WebhooksController : ControllerBase
    {
        private static string _errorMessageSavingData = "An error occurred saving data to database.";
        private static string _errorMessageWebhookExists = "Webhook already exists.";
        private static string _baseUrl = Startup.StaticConfiguration.GetSection("BaseUrl").Value;
        private Data.ApplicationDbContext _db;
        private IUserContextService _userCtx;
        private IWebhookService _hooks;
        private readonly ILogger<WebhooksController> _logger;
        public WebhooksController(ILogger<WebhooksController> logger, Data.ApplicationDbContext db, 
            IWebhookService hooks, 
            IUserContextService userCtx)
        {
            _db = db;
            _logger = logger;
            _hooks = hooks;
            _userCtx = userCtx;
        }

        [HttpPost]
        public async Task<IActionResult> CreateWebhook(WebhookPostDTO webhookToBeCreated)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if webhook already exists.
            Webhook webhook = await _db.Webhooks
                .FirstOrDefaultAsync(w => w.CallbackURL == webhookToBeCreated.CallbackURL
                && w.Event == webhookToBeCreated.Event);
            if (webhook != null)
            {
                return Conflict(new { message = _errorMessageWebhookExists });                                                                
            }

            try
            {
                int userId = (int)_userCtx.GetId();
                Webhook newWebhook = WebhookPostDTO.ToModel(webhookToBeCreated, userId);

                await _db.Webhooks.AddAsync(newWebhook);
                await _db.SaveChangesAsync();

                string locationUri = $"{_baseUrl}/api/v1/user/webhooks/{newWebhook.WebhookId}";

                return Created(locationUri, WebhookGetDTO.FromModel(newWebhook));
            }
            catch (DbUpdateException)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    _errorMessageSavingData);
            }
        }
    }
}
