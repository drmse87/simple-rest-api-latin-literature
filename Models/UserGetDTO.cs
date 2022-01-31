using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace latin_web_api.Models
{
    public class UserGetDTO
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public DateTime RegisterDate { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public Dictionary<string, Link> _links { get; set; }

        public static UserGetDTO FromModel(User userToConvertToResponse)
        {
            return new UserGetDTO {
                UserId = userToConvertToResponse.UserId,
                Username = userToConvertToResponse.Username,
                RegisterDate = userToConvertToResponse.RegisterDate,
                LastLoginDate = userToConvertToResponse.LastLoginDate,
                _links = new Dictionary<string, Link> {
                    { "self", new Link ($"api/v1/users/{userToConvertToResponse.UserId}", "self") },
                    { "authors-added-by-user ", new Link ($"api/v1/users/{userToConvertToResponse.UserId}/authors", "authors-added-by-user") },
                    { "works-added-by-user", new Link ($"api/v1/users/{userToConvertToResponse.UserId}/works", "works-added-by-user") }
                }
            };
        }
    }
}
