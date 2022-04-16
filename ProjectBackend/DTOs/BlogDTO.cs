using System;

namespace ProjectBackend.DTOs
{
    public class BlogDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }
        public string BlogImgUrl { get; set; }
        public Boolean IsVisible { get; set; }

        public int UserId { get; set; }
    }
}
