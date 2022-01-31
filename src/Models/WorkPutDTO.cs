using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace simple_web_api_latin_literature.Models
{
    public class WorkPutDTO
    {
        [Required]
        public int WorkId { get; set; }
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
    }
}
