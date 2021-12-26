namespace JWTAPI.Core.Security.Tokens
{
    public class AccessToken : JsonWebToken
    {
        public RefreshToken RefreshToken { get; private set; }

        public AccessToken(string token, long expiration, RefreshToken refreshToken) : base(token, expiration)
        {
            if(refreshToken == null)
                throw new ArgumentException("Specify a valid refresh token.");
                
            RefreshToken = refreshToken;
        }
    }
}