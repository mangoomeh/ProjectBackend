using System;

namespace ProjectBackend.DTOs
{
    public class CommentDTO
    {
        public string Content { get; set; }
        public string CommentedUserName { get; set; }
        
        public int UserId { get; set; }
        public int BlogId { get; set; }
    }
}
