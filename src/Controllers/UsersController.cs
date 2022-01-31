using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using simple_web_api_latin_literature.Models;

namespace simple_web_api_latin_literature.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class UsersController : ControllerBase
    {
        private static string _errorMessageFetchingData = "An error occurred fetching data from database.";
        private static string _errorMessageSavingData = "An error occurred saving data to database.";
        private static string _errorMessageUsernameExists = "Username already exists, try another.";
        private static string userIdClaimName = "UserId";
        private static double minutesUntilTokenExpires = 30;
        private string _baseUrl;
        private string _secretKey;
        public IConfiguration _conf { get; }
        private Data.ApplicationDbContext _db;
        private readonly ILogger<UsersController> _logger;
        public UsersController(ILogger<UsersController> logger, Data.ApplicationDbContext db, IConfiguration conf)
        {
            _conf = conf;
            _db = db;
            _logger = logger;
            _baseUrl = _conf["BaseUrl"];
            _secretKey = _conf["SecurityKey"];
        }

        [Route("login")]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody]UserPostDTO userAttemptingToLogin)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if username even exists.
            User existingUser = await _db.Users.FirstOrDefaultAsync(u => u.Username == userAttemptingToLogin.Username);
            if (existingUser == null)
            {
                return Unauthorized();
            }

            // Check if password matches.
            User user = await _db.Users.FirstOrDefaultAsync(u => u.Username == userAttemptingToLogin.Username);
            if (!Crypto.VerifyHashedPassword(user.Password, userAttemptingToLogin.Password))
            {
                return Unauthorized();
            }

            try
            {
                // Generate JWT.
                string accessToken = GenerateJWT(user);
                user.LastLoginDate = DateTime.Now;
                await _db.SaveChangesAsync();

                return Ok(new { access_token = accessToken } );
            }
            catch (DbUpdateException)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    _errorMessageSavingData);                
            }
        }

        [HttpPost]
        public async Task<IActionResult> Register(UserPostDTO userAttemptingToRegister)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if username already exists.
            User existingUser = await _db.Users.FirstOrDefaultAsync(u => u.Username == userAttemptingToRegister.Username);
            if (existingUser != null)
            {
                return Conflict(new { message = _errorMessageUsernameExists });                                                                
            }

            try 
            {
                User newUser = UserPostDTO.ToModel(userAttemptingToRegister);

                await _db.Users.AddAsync(newUser);
                await _db.SaveChangesAsync();

                string locationUri = $"{_baseUrl}/api/v1/users/{newUser.UserId}";

                return Created(locationUri, UserGetDTO.FromModel(newUser));
            }
            catch(DbUpdateException)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    _errorMessageSavingData);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers() 
        {
            try 
            {
                return Ok(await _db.Users
                    .OrderBy(u => u.Username)
                    .Select(u => UserGetDTO.FromModel(u))
                    .ToListAsync());
            }
            catch (InvalidOperationException)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    _errorMessageFetchingData);
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetUser(int id) 
        {
            User existingUser = await _db.Users
                .Where(u => u.UserId == id)
                .FirstOrDefaultAsync();

            if (existingUser == null) 
            {
                return NotFound();
            }

            try
            {
                UserGetDTO userResponseDto = UserGetDTO.FromModel(existingUser);

                return Ok(userResponseDto);
            }
            catch (InvalidOperationException)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    _errorMessageFetchingData);
            }
        }

        [HttpGet("{id:int}/works")]
        public async Task<IActionResult> GetWorksAddedByUser(int id) 
        {
            try 
            {
                return Ok(await _db.Works
                    .Where(w => w.AddedBy == id)
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

        [HttpGet("{id:int}/authors")]
        public async Task<IActionResult> GetAuthorsAddedByUser(int id) 
        {
            try 
            {
                return Ok(await _db.Authors
                    .Where(a => a.AddedBy == id)
                    .OrderBy(a => a.Praenomen)
                    .Select(a => AuthorGetDTO.FromModel(a))
                    .ToListAsync());
            }
            catch (InvalidOperationException)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    _errorMessageFetchingData);
            }
        }
        
        private string GenerateJWT(User userLoggingIn)
        {
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            SigningCredentials credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            Claim[] claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userLoggingIn.Username),
                new Claim(userIdClaimName, userLoggingIn.UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };
            JwtSecurityToken token = new JwtSecurityToken(
                audience: _baseUrl,
                claims: claims,
                expires: DateTime.Now.AddMinutes(minutesUntilTokenExpires),
                issuer: _baseUrl,
                signingCredentials: credentials
            );

            return tokenHandler.WriteToken(token);
        }
    }
}
