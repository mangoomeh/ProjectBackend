using System;
using System.ComponentModel.DataAnnotations;

namespace ProjectBackend.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }
        public string Content { get; set; }
        public string CommentedUserName { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedTime { get; set; }

        public int BlogId { get; set; }
        public Blog Blog { get; set; }
    }
}
