using System.ComponentModel.DataAnnotations;
using System;
using System.Web.Helpers;

namespace simple_rest_api_latin_literature.Models
{
    public class UserPostDTO
    {
        [Required]
        [StringLength(50, MinimumLength = 4)]
        public string Username { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 8)]
        public string Password { get; set; }

        public static User ToModel(UserPostDTO postedUserToConvert)
        {
            return new User
            {
                Username = postedUserToConvert.Username,
                Password = Crypto.HashPassword(postedUserToConvert.Password),
                RegisterDate = DateTime.Now
            };
        }
    }
}
