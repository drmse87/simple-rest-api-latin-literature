using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using web_api_assignment.Models;

namespace web_api_assignment.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AuthorsController : ControllerBase
    {
        private Data.ApplicationDbContext _db;
        private readonly ILogger<AuthorsController> _logger;
        private IUserContextService _userCtx;
        private IWebhookService _hooks;
        private static string _errorMessageFetchingData = "An error occurred fetching data from database.";
        private static string _errorMessageSavingData = "An error occurred saving data to database.";
        private static string _errorMessageAuthorExists = "Author already exists.";
        private static string _baseUrl = Startup.StaticConfiguration.GetSection("BaseUrl").Value;

        public AuthorsController(ILogger<AuthorsController> logger, 
            Data.ApplicationDbContext db,
            IWebhookService hooks, 
            IUserContextService userCtx)
        {
            _db = db;
            _logger = logger;
            _hooks = hooks;
            _userCtx = userCtx;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAuthors(string period) 
        {
            try 
            {
                IEnumerable<AuthorGetDTO> existingAuthors = await _db.Authors
                    .OrderBy(a => a.Praenomen)
                    .Select(a => AuthorGetDTO.FromModel(a))
                    .ToListAsync();

                if (!String.IsNullOrEmpty(period))
                {
                    existingAuthors = existingAuthors.Where(a => a.Period.ToLower() == period.ToLower());
                }

                return Ok(existingAuthors);
            }
            catch (InvalidOperationException)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    _errorMessageFetchingData);
            }
        }

        [AllowAnonymous]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetAuthor(int id) 
        {
            Author existingAuthor = await _db.Authors
                .Where(a => a.AuthorId == id)
                .FirstOrDefaultAsync();

            if (existingAuthor == null) 
            {
                return NotFound();
            }

            try
            {
                AuthorGetDTO authorResponseDto = AuthorGetDTO.FromModel(existingAuthor);

                return Ok(existingAuthor);
            }
            catch (InvalidOperationException)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    _errorMessageFetchingData);
            }
        }

        [AllowAnonymous]
        [HttpGet("{id:int}/works")]
        public async Task<IActionResult> GetWorksByAuthor(int id) 
        {
            Author existingAuthor = await _db.Authors
                .Where(a => a.AuthorId == id)
                .FirstOrDefaultAsync();

            if (existingAuthor == null) 
            {
                return NotFound();
            }

            try 
            {
                return Ok(await _db.Works
                    .Where(w => w.AuthorId == id)
                    .OrderBy(w => w.Title)
                    .Select(w => WorkGetDTO.FromModel(w))
                    .ToListAsync());
            }
            catch (InvalidOperationException)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    _errorMessageFetchingData);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateAuthor(AuthorPostDTO authorToBeCreated)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if author already exists.
            Author existingAuthor = await _db.Authors.FirstOrDefaultAsync(a => 
                a.Praenomen == authorToBeCreated.Praenomen && 
                a.Nomen == authorToBeCreated.Nomen && 
                a.Cognomen == authorToBeCreated.Cognomen);
            if (existingAuthor != null)
            {
                return Conflict(new { message = _errorMessageAuthorExists });                                                                
            }

            try
            {
                int userId = (int)_userCtx.GetId();
                Author newAuthor = AuthorPostDTO.ToModel(authorToBeCreated, userId);

                await _db.Authors.AddAsync(newAuthor);
                await _db.SaveChangesAsync();

                string locationUri = $"{_baseUrl}/api/v1/authors/{newAuthor.AuthorId}";

                // Prepare response.
                AuthorGetDTO newAuthorResponse = AuthorGetDTO.FromModel(newAuthor);

                // Trigger webhook for new author event.
                TriggerAuthorWebhook(Event.NewAuthor, newAuthorResponse, userId);

                return Created(locationUri, AuthorGetDTO.FromModel(newAuthor));
            }
            catch (DbUpdateException)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    _errorMessageSavingData);
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAuthor(int id) 
        {
            Author existingAuthor = await _db.Authors
                .Where(a => a.AuthorId == id)
                .FirstOrDefaultAsync();

            if (existingAuthor == null)
            {
                return NotFound();
            }

            try
            {
                _db.Remove(existingAuthor);
                await _db.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateException)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    _errorMessageSavingData);
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateAuthor(int id, AuthorPutDTO authorToBeUpdated) 
        {
            // Check that model is valid, and also that id in URI matches id in Body.
            if (!ModelState.IsValid || 
                id != authorToBeUpdated.AuthorId)
            {
                return BadRequest(ModelState);
            }

            // Check that an author by that ID exists.
            Author existingAuthor = await _db.Authors
                .Where(a => a.AuthorId == id)
                .FirstOrDefaultAsync();
            if (existingAuthor == null)
            {
                return NotFound();
            }

            // Check if the edited author already exists.
            Author existingAuthorWithSameName = await _db.Authors
                .Where(a => 
                    a.Praenomen == authorToBeUpdated.Praenomen &&
                    a.Nomen == authorToBeUpdated.Nomen &&
                    a.Cognomen == authorToBeUpdated.Cognomen)
                .FirstOrDefaultAsync();
            if (existingAuthorWithSameName != null)
            {
                return Conflict(new { message = _errorMessageAuthorExists });                                                                
            }

            try
            {
                int userId = (int)_userCtx.GetId();
                
                // First update author.
                _db.Entry(existingAuthor).CurrentValues.SetValues(authorToBeUpdated);
                existingAuthor.LastEditedBy = userId;
                existingAuthor.LastEditedDate = DateTime.Now;
                await _db.SaveChangesAsync();

                // Prepare response.
                AuthorGetDTO editedAuthorResponse = AuthorGetDTO.FromModel(existingAuthor);

                // Trigger webhook for edited author event.
                TriggerAuthorWebhook(Event.EditedAuthor, editedAuthorResponse, userId);

                return Ok(editedAuthorResponse);
            }
            catch (DbUpdateException)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    _errorMessageSavingData);
            }
        }

        private async void TriggerAuthorWebhook(Event triggeringEvent, dynamic changedObject, int userId)
        {
            User userTriggeringHook = await _db.Users
                .Where(u => u.UserId == userId)
                .FirstOrDefaultAsync();

            try
            {
                await _hooks.Trigger(
                    new WebhookData { 
                        Event = triggeringEvent, 
                        User = UserGetDTO.FromModel(userTriggeringHook),
                        Object = changedObject 
                    });                    
            }
            catch (InvalidOperationException exception)
            {
                // Log that there was a bad URL.
                _logger.LogInformation(exception.Message.ToString());
            }
            catch (HttpRequestException exception)
            {             
                // Log that the URL didn't accept POST request.
                _logger.LogInformation(exception.Message.ToString());
            }  
        }
    }
}
