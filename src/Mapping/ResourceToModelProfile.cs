using AutoMapper;
using JWTAPI.Controllers.Resources;
using JWTAPI.Core.Models;

namespace JWTAPI.Mapping
{
    public class ResourceToModelProfile : Profile
    {
        public ResourceToModelProfile()
        {
            CreateMap<UserCredentialsResource, User>();
        }
    }
}