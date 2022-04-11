using System;
using System.ComponentModel.DataAnnotations;

namespace ProjectBackend.Models
{
    public class Comment
    {
        // Comment details
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime CreatedTime { get; set; }
        
        // User posting this comment
        public int UserId { get; set; }
        
        // Blog for which this comment is for
        public int BlogId { get; set; }
        public Blog Blog { get; set; }
    }
}
