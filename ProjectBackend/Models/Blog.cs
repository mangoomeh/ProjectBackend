using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjectBackend.Models
{
    public class Blog
    {
        // Blog details
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }
        public string BlogImg { get; set; }
        public DateTime PublishedDate { get; set; }
        public Boolean IsVisible { get; set; }

        // Comments of this blog
        public List<Comment> Comments { get; set; }

        // User who posted this blog
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
