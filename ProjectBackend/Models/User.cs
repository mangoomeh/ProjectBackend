using System.Collections.Generic;

namespace ProjectBackend.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }

        public int RoleId { get; set; }
        public Role Role { get; set; }

        public List<Comment> Comments { get; set; }
    }
}
