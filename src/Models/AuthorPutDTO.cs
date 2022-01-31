using System;
using System.ComponentModel.DataAnnotations;

namespace simple_rest_api_latin_literature.Models
{
    public class AuthorPutDTO
    {
        [Required]
        public int AuthorId { get; set; }
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
    }
}

