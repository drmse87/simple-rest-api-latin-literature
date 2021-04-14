using System;
using System.ComponentModel.DataAnnotations;

namespace web_api_assignment.Models
{
    public class AuthorPostDTO
    {
        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string Praenomen { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string Nomen { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string Cognomen { get; set; }
        [Required]
        [EnumDataType(typeof(Period))]
        public Period Period { get; set; }
        [StringLength(10, MinimumLength = 2)]
        public string Born { get; set; }

        [StringLength(10, MinimumLength = 2)]
        public string Died { get; set; }
        [StringLength(50, MinimumLength = 2)]
        public string Occupation { get; set; }
        [StringLength(50, MinimumLength = 2)]
        public string PlaceOfBirth { get; set; }
        [Url]
        [StringLength(2000, MinimumLength = 2)]
        public string ImageURL { get; set; }
        public static Author ToModel(AuthorPostDTO postedAuthorToConvert, int userId)
        {
            return new Author 
            {
                Praenomen = postedAuthorToConvert.Praenomen,
                Nomen = postedAuthorToConvert.Nomen,
                Cognomen = postedAuthorToConvert.Cognomen,
                Period = postedAuthorToConvert.Period,
                Born = postedAuthorToConvert.Born,
                Died = postedAuthorToConvert.Died,
                Occupation = postedAuthorToConvert.Occupation,
                PlaceOfBirth = postedAuthorToConvert.PlaceOfBirth,
                ImageURL = postedAuthorToConvert.ImageURL,
                AddedBy = userId,
                AddedDate = DateTime.Now
            };
        }
    }
}

