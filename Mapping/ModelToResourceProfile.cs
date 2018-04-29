using System.Linq;
using AutoMapper;
using JWTAPI.Controllers.Resources;
using JWTAPI.Models;

namespace JWTAPI.Mapping
{
    public class ModelToResourceProfile : Profile
    {
        public ModelToResourceProfile()
        {
            CreateMap<User, UserResource>()
                .ForMember(u => u.Roles, opt => opt.MapFrom(u => u.UserRoles.Select(ur => ur.Role.Name)));
        }
    }
}