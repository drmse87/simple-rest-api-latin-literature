using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using simple_web_api_latin_literature.Models;

namespace simple_web_api_latin_literature.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class WorksController : ControllerBase
    {
        private static string _errorMessageFetchingData = "An error occurred fetching data from database.";
        private static string _baseUrl = Startup.StaticConfiguration.GetSection("BaseUrl").Value;
        private static string _errorMessageAuthorNotExists = "No such author exists.";
        private static string _errorMessageWorkExists = "Work already exists.";
        private static string _errorMessageSavingData = "An error occurred saving data to database.";
        private Data.ApplicationDbContext _db;
        private IWebhookService _hooks;
        private readonly ILogger<WorksController> _logger;
        private IUserContextService _userCtx;
        public WorksController(ILogger<WorksController> logger, 
            Data.ApplicationDbContext db,
            IWebhookService hooks, 
            IUserContextService userCtx)
        {
            _db = db;
            _logger = logger;
            _userCtx = userCtx;
            _hooks = hooks;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetWorks(string genre) 
        {
            try 
            {
                IEnumerable<WorkGetDTO> existingWorks = await _db.Works
                    .OrderBy(w => w.Title)
                    .Select(w => WorkGetDTO.FromModel(w))
                    .ToListAsync();

                if (!String.IsNullOrEmpty(genre))
                {
                    existingWorks = existingWorks.Where(w => w.Genre.ToLower() == genre.ToLower());
                }

                return Ok(existingWorks);
            }
            catch (InvalidOperationException)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    _errorMessageFetchingData);
            }
        }

        [AllowAnonymous]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetWork(int id) 
        {
            Work existingWork = await _db.Works
                .Where(w => w.WorkId == id)
                .FirstOrDefaultAsync();

            if (existingWork == null) 
            {
                return NotFound();
            }

            try
            {
                WorkGetDTO workResponseDto = WorkGetDTO.FromModel(existingWork);

                return Ok(workResponseDto);
            }
            catch (InvalidOperationException)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    _errorMessageFetchingData);
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteWork(int id) 
        {
            Work existingWork = await _db.Works
                .Where(w => w.WorkId == id)
                .FirstOrDefaultAsync();

            if (existingWork == null)
            {
                return NotFound();
            }

            try
            {
                _db.Remove(existingWork);
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
        public async Task<IActionResult> UpdateWork(int id, WorkPutDTO workToBeUpdated) 
        {
            // Check that model is valid, and also that id in URI matches id in Body.
            if (!ModelState.IsValid || 
                id != workToBeUpdated.WorkId)
            {
                return BadRequest(ModelState);
            }

            // Check that work by that ID exists.
            Work existingWork = await _db.Works
                .Where(w => w.WorkId == id)
                .FirstOrDefaultAsync();
            if (existingWork == null)
            {
                return NotFound();
            }

            // Check if author exists.
            Author existingAuthor = await _db.Authors
                .FirstOrDefaultAsync(a => a.AuthorId == workToBeUpdated.AuthorId);
            if (existingAuthor == null)
            {
                return BadRequest(new { message = _errorMessageAuthorNotExists });
            }

            // Check if work with that title already exists.
            Work existingWorkWithTitle = await _db.Works
                .FirstOrDefaultAsync(w => w.Title == workToBeUpdated.Title);
            if (existingWorkWithTitle != null)
            {
                return Conflict(new { message = _errorMessageWorkExists });                                                                
            }

            try
            {
                int userId = (int)_userCtx.GetId();

                // First update work.
                _db.Entry(existingWork).CurrentValues.SetValues(workToBeUpdated);
                existingWork.LastEditedBy = userId;
                existingWork.LastEditedDate = DateTime.Now;
                await _db.SaveChangesAsync();

                // Prepare response.
                WorkGetDTO editedWorkResponse = WorkGetDTO.FromModel(existingWork);

                // Trigger webhook for edited work.
                TriggerWorkWebhook(Event.EditedWork, editedWorkResponse, userId);

                return Ok(editedWorkResponse);
            }
            catch (DbUpdateException)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    _errorMessageSavingData);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateWork(WorkPostDTO workToBeCreated)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if author exists.
            Author existingAuthor = await _db.Authors.FirstOrDefaultAsync(a => a.AuthorId == workToBeCreated.AuthorId);
            if (existingAuthor == null)
            {
                return BadRequest(new { message = _errorMessageAuthorNotExists });
            }

            // Check if work already exists.
            Work existingWork = await _db.Works
                .FirstOrDefaultAsync(w => w.Title == workToBeCreated.Title);
            if (existingWork != null)
            {
                return Conflict(new { message = _errorMessageWorkExists });                                                                
            }

            try
            {
                int userId = (int)_userCtx.GetId();
                Work newWork = WorkPostDTO.ToModel(workToBeCreated, userId);

                await _db.Works.AddAsync(newWork);
                await _db.SaveChangesAsync();

                // Prepare response.
                WorkGetDTO newWorkResponse = WorkGetDTO.FromModel(newWork);

                // Trigger webhook for new work.
                TriggerWorkWebhook(Event.EditedWork, newWorkResponse, userId);

                string locationUri = $"{_baseUrl}/api/v1/works/{newWork.WorkId}";

                return Created(locationUri, WorkGetDTO.FromModel(newWork));
            }
            catch (DbUpdateException)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    _errorMessageSavingData);
            }
        }

        private async void TriggerWorkWebhook(Event triggeringEvent, dynamic changedObject, int userId)
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
