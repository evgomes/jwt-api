namespace JWTAPI.Mapping;
public class ModelToResourceProfile : Profile
{
    public ModelToResourceProfile()
    {
        CreateMap<User, UserResource>()
            .ForMember(u => u.Roles, opt => opt.MapFrom(u => u.UserRoles.Select(ur => ur.Role.Name)));

        CreateMap<AccessToken, AccessTokenResource>()
            .ForMember(a => a.AccessToken, opt => opt.MapFrom(a => a.Token))
            .ForMember(a => a.RefreshToken, opt => opt.MapFrom(a => a.RefreshToken.Token))
            .ForMember(a => a.Expiration, opt => opt.MapFrom(a => a.Expiration));
    }
}