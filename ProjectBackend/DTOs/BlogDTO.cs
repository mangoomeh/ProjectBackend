using System;

namespace ProjectBackend.DTOs
{
    public class BlogDTO
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string BlogImg { get; set; }
        public string Author { get; set; }
        public string AuthorImg { get; set; }
        public string VideoUrl { get; set; }
        public Boolean IsVisible { get; set; }
    }
}
