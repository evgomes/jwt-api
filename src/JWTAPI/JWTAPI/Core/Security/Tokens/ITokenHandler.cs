namespace JWTAPI.Core.Security.Tokens;

public interface ITokenHandler
{
     AccessToken CreateAccessToken(User user);
     RefreshToken TakeRefreshToken(string token, string userEmail);
     void RevokeRefreshToken(string token, string userEmail);
}