namespace JWTPAPI.Tests.Security.Tokens;

public class TokenHandlerTests
{
    private Mock<IOptions<TokenOptions>> _tokenOptions;
    private Mock<IPasswordHasher> _passwordHasher;
    private SigningConfigurations _signingConfigurations;
    private User _user;

    private ITokenHandler _tokenHandler;
    private string _testKey = "just_a_long_test_key_to_use_for_tokens";

    public TokenHandlerTests()
    {
        SetupMocks();
        _tokenHandler = new TokenHandler(_tokenOptions.Object, _signingConfigurations, _passwordHasher.Object);
    }

    private void SetupMocks()
    {
        _tokenOptions = new Mock<IOptions<TokenOptions>>();
        _tokenOptions.Setup(to => to.Value).Returns(new TokenOptions
        {
            Audience = "Testing",
                Issuer = "Testing",
                AccessTokenExpiration = 30,
                RefreshTokenExpiration = 60
        });

        _passwordHasher = new Mock<IPasswordHasher>();
        _passwordHasher.Setup(ph => ph.HashPassword(It.IsAny<string>())).Returns("123");

        _signingConfigurations = new SigningConfigurations(_testKey);

        _user = new User
        {
            Id = 1,
            Email = "test@test.com",
            Password = "123",
            UserRoles = new Collection<UserRole>
            {
                new UserRole
                {
                    Role = new Role
                    {
                        Id = 1,
                        Name = ApplicationRole.Common.ToString()
                    }
                }
            }
        };
    }

    [Fact]
    public void Should_Create_Access_Token()
    {
        var accessToken = _tokenHandler.CreateAccessToken(_user);

        Assert.NotNull(accessToken);
        Assert.NotNull(accessToken.RefreshToken);
        Assert.NotEmpty(accessToken.Token);
        Assert.NotEmpty(accessToken.RefreshToken.Token);
        Assert.True(accessToken.Expiration > DateTime.UtcNow.Ticks);
        Assert.True(accessToken.RefreshToken.Expiration > DateTime.UtcNow.Ticks);
        Assert.True(accessToken.RefreshToken.Expiration > accessToken.Expiration);
    }

    [Fact]
    public void Should_Take_Existing_Refresh_Token()
    {
        var accessToken = _tokenHandler.CreateAccessToken(_user);
        var refreshToken = _tokenHandler.TakeRefreshToken(accessToken.RefreshToken.Token, "test@test.com");

        Assert.NotNull(refreshToken);
        Assert.Equal(accessToken.RefreshToken.Token, refreshToken.Token);
        Assert.Equal(accessToken.RefreshToken.Expiration, refreshToken.Expiration);
    }

    [Fact]
    public void Should_Return_Null_For_Empty_Refresh_Token_When_Trying_To_Take_Refresh_Token()
    {
        var refreshToken = _tokenHandler.TakeRefreshToken(string.Empty, "test@test.com");
        Assert.Null(refreshToken);
    }

		[Fact]
    public void Should_Return_Null_For_Empty_Email_When_Trying_To_Take_Refresh_Token()
		{
        var accessToken = _tokenHandler.CreateAccessToken(_user);
        var refreshToken = _tokenHandler.TakeRefreshToken(accessToken.RefreshToken.Token, string.Empty);

        Assert.Null(refreshToken);
    }

    [Fact]
    public void Should_Return_Null_For_Invalid_Refresh_Token_When_Trying_To_Take_Refresh_oken()
    {
        var refreshToken = _tokenHandler.TakeRefreshToken("invalid_token", "test@test.com");
        Assert.Null(refreshToken);
    }

    [Fact]
    public void Should_Return_Null_For_Invalid_Email_When_Trying_To_Take_Refresh_Token()
    {
        var accessToken = _tokenHandler.CreateAccessToken(_user);
        var refreshToken = _tokenHandler.TakeRefreshToken(accessToken.RefreshToken.Token, "admin@admin.com");
        Assert.Null(refreshToken);
    }

    [Fact]
    public void Should_Not_Take_Refresh_Token_That_Was_Already_Taken()
    {
        var accessToken = _tokenHandler.CreateAccessToken(_user);
        var refreshToken = _tokenHandler.TakeRefreshToken(accessToken.RefreshToken.Token, "test@test.com");
        var refreshTokenSecondTime = _tokenHandler.TakeRefreshToken(accessToken.RefreshToken.Token, "test@test.com");

        Assert.NotNull(refreshToken);
        Assert.Null(refreshTokenSecondTime);
    }

    [Fact]
    public void Should_Revoke_Refresh_Token()
    {
        var accessToken = _tokenHandler.CreateAccessToken(_user);
        _tokenHandler.RevokeRefreshToken(accessToken.RefreshToken.Token, "test@test.com");
        var refreshToken = _tokenHandler.TakeRefreshToken(accessToken.RefreshToken.Token, "test@test.com");

        Assert.Null(refreshToken);
    }
}