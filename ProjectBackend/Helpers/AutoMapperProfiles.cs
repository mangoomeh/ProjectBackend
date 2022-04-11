using AutoMapper;
using ProjectBackend.DTOs;
using ProjectBackend.Models;

namespace ProjectBackend.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User, UserDTO>();
            CreateMap<Role, RoleDTO>();
            CreateMap<Comment, CommentDTO>();
            CreateMap<Blog, BlogDTO>();
            CreateMap<UserDTO, User>();
            CreateMap<RoleDTO, Role>();
            CreateMap<CommentDTO, Comment>();
            CreateMap<BlogDTO, Blog>();
        }
    }
}
