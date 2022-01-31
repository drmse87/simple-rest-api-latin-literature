using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace simple_web_api_latin_literature.Models
{
    public class Work
    {
        [Key]
        public int WorkId { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public Genre Genre { get; set; }
        [Required]
        public int AddedBy { get; set; }
        [Required]
        public DateTime AddedDate { get; set; }
        public string YearOfPublication { get; set; }
        public string Excerpt { get; set; }
        public int? LastEditedBy { get; set; }
        public DateTime? LastEditedDate { get; set; }

        // Foreign key
        [Required]
        public int AuthorId { get; set; }
    }
}
