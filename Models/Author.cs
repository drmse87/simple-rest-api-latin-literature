using System;
using System.ComponentModel.DataAnnotations;

namespace latin_web_api.Models
{
    public class Author
    {
        [Key]
        public int AuthorId { get; set; }
        [Required]
        public string Praenomen { get; set; }
        [Required]
        public string Nomen { get; set; }
        [Required]
        public string Cognomen { get; set; }
        [Required]
        public Period Period { get; set; }
        [Required]
        public int AddedBy { get; set; }
        [Required]
        public DateTime AddedDate { get; set; }
        public string Born { get; set; }
        public string Died { get; set; }
        public string Occupation { get; set; }
        public string PlaceOfBirth { get; set; }
        public string ImageURL { get; set; }
        public int? LastEditedBy { get; set; }
        public DateTime? LastEditedDate { get; set; }
    }
}

