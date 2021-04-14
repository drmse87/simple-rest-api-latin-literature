using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace web_api_assignment.Models
{
    public class WorkGetDTO
    {
        public int WorkId { get; set; }
        public string Title { get; set; }
        public string Genre { get; set; }
        public int AddedBy { get; set; }
        public DateTime AddedDate { get; set; }
        public string YearOfPublication { get; set; }
        public string Excerpt { get; set; }
        public int? LastEditedBy { get; set; }
        public DateTime? LastEditedDate { get; set; }
        public int AuthorId { get; set; }
        public Dictionary<string, Link> _links { get; set; }
        public static WorkGetDTO FromModel(Work workToConvertToResponse)
        {
            return new WorkGetDTO {
                WorkId = workToConvertToResponse.WorkId,
                Title = workToConvertToResponse.Title,
                Genre = workToConvertToResponse.Genre.ToString(),
                AddedBy = workToConvertToResponse.AddedBy,
                AddedDate = workToConvertToResponse.AddedDate,
                YearOfPublication = workToConvertToResponse.YearOfPublication,
                Excerpt = workToConvertToResponse.Excerpt,
                LastEditedBy = workToConvertToResponse.LastEditedBy,
                LastEditedDate = workToConvertToResponse.LastEditedDate,
                AuthorId = workToConvertToResponse.AuthorId,
                _links = new Dictionary<string, Link> {
                    { "self", new Link ($"api/v1/works/{workToConvertToResponse.WorkId}", "self") },
                    { "author", new Link ($"api/v1/authors/{workToConvertToResponse.AuthorId}", "author") },
                    { "added-by", new Link ($"api/v1/users/{workToConvertToResponse.AddedBy}", "added-by")}
                }
            };
        }
    }
}
