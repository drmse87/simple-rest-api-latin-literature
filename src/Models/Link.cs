using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;

namespace simple_rest_api_latin_literature.Models
{
    public class Link
    {
        private static string _baseUrl = Startup.StaticConfiguration.GetSection("BaseUrl").Value;
        private string _endpoint;
        private string _href;
        private string _rel;
        public string Href 
        { 
            get
            {
                return _href;
            }
            set
            {
                _href = value;
            } 
        }
        public string Rel
        { 
            get
            {
                return _rel;
            }
            set
            {
                _rel = value;
            } 
        }
        public Link (string endpoint, string rel)
        {
            Rel = rel;
            _endpoint = endpoint;
            Href = $"{_baseUrl}/{_endpoint}"; 
        }
    } 
}
