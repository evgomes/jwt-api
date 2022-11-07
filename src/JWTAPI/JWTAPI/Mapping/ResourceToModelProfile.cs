namespace JWTAPI.Mapping;
public class ResourceToModelProfile : Profile
{
    public ResourceToModelProfile()
    {
        CreateMap<UserCredentialsResource, User>();
    }
}