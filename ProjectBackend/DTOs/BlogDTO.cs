using System;

namespace ProjectBackend.DTOs
{
    public class BlogDTO
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string BlogImg { get; set; }
        public Boolean IsVisible { get; set; }

        public int UserId { get; set; }
    }
}
