using System.Collections.Generic;

namespace ProjectBackend.Models
{
    public class Role
    {
        // Role Details
        public int Id { get; set; }
        public string RoleName { get; set; }

        // Users with this Role
        public List<User> Users { get; set; }
    }
}
