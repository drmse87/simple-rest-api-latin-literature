using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace latin_web_api.Models
{
    public class AuthorGetDTO
    {
        public int AuthorId { get; set; }
        public string Praenomen { get; set; }
        public string Nomen { get; set; }
        public string Cognomen { get; set; }
        public string Period { get; set; }
        public int AddedBy { get; set; }
        public DateTime AddedDate { get; set; }
        public string Born { get; set; }
        public string Died { get; set; }
        public string Occupation { get; set; }
        public string PlaceOfBirth { get; set; }
        public string ImageURL { get; set; }
        public int? LastEditedBy { get; set; }
        public DateTime? LastEditedDate { get; set; }
        public Dictionary<string,Link> _links { get; set; }

        public static AuthorGetDTO FromModel(Author authorToConvertToResponse)
        {
            return new AuthorGetDTO {
                AuthorId = authorToConvertToResponse.AuthorId,
                Praenomen = authorToConvertToResponse.Praenomen,
                Nomen = authorToConvertToResponse.Nomen,
                Cognomen = authorToConvertToResponse.Cognomen,
                Period = authorToConvertToResponse.Period.ToString(),
                AddedBy = authorToConvertToResponse.AddedBy,
                AddedDate = authorToConvertToResponse.AddedDate,
                Born = authorToConvertToResponse.Born,
                Died = authorToConvertToResponse.Died,
                Occupation = authorToConvertToResponse.Occupation,
                PlaceOfBirth = authorToConvertToResponse.PlaceOfBirth,
                ImageURL = authorToConvertToResponse.ImageURL,
                LastEditedBy = authorToConvertToResponse.LastEditedBy,
                LastEditedDate = authorToConvertToResponse.LastEditedDate,
                _links = new Dictionary<string, Link> {
                    { "self", new Link ($"api/v1/authors/{authorToConvertToResponse.AuthorId}", "self") },
                    { "works-by-author", new Link ($"api/v1/authors/{authorToConvertToResponse.AuthorId}/works", "works-by-author") },
                    { "added-by", new Link ($"api/v1/users/{authorToConvertToResponse.AddedBy}", "added-by") }
                }
            };
        }
    }
}