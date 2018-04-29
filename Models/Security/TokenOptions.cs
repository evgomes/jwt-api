namespace JWTAPI.Models.Security
{
    public class TokenOptions
    {
        public string Audience { get; set; }
        public string Issuer { get; set; }
        public int Expiration { get; set; }
    }
}