using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace simple_rest_api_latin_literature.Models
{
    public class WorkPostDTO
    {
        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string Title { get; set; }
        [Required]
        [EnumDataType(typeof(Genre))]
        public Genre Genre { get; set; }
        [StringLength(10, MinimumLength = 2)]
        public string YearOfPublication { get; set; }
        [StringLength(500, MinimumLength = 2)]
        public string Excerpt { get; set; }
        // Foreign key
        [Required]
        public int AuthorId { get; set; }

        public static Work ToModel(WorkPostDTO workPostDtoToConvert, int userId)
        {
            return new Work 
            {
                Title = workPostDtoToConvert.Title,
                Genre = workPostDtoToConvert.Genre,
                AddedDate = DateTime.Now,
                AddedBy = userId,
                YearOfPublication = workPostDtoToConvert.YearOfPublication,
                Excerpt = workPostDtoToConvert.Excerpt,
                AuthorId = workPostDtoToConvert.AuthorId               
            };
        }
    }
}
