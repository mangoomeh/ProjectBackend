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
            CreateMap<Comment, EditCommentDTO>();
            CreateMap<Blog, BlogDTO>();
            CreateMap<UserDTO, User>();
            CreateMap<RoleDTO, Role>();
            CreateMap<EditCommentDTO, Comment>();
            CreateMap<BlogDTO, Blog>();
        }
    }
}
