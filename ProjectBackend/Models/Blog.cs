using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjectBackend.Models
{
    public class Blog
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string BlogImg { get; set; }
        public string Author { get; set; }
        public DateTime PublishDate { get; set; }
        public Boolean IsVisible { get; set; }

        public List<Comment> Comments { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
    }
}
