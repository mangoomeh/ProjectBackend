namespace ProjectBackend.DTOs
{
    public class UserDTO
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ProfileImgUrl { get; set; }

        public int RoleId { get; set; }

    }
}
