using System;
using JWTAPI.Core.Security.Hashing;
using JWTAPI.Security.Hashing;
using Xunit;

namespace JWTPAPI.Tests.Security.Hashing
{
    public class PasswordHasherTests
    {
        private IPasswordHasher _passwordHasher = new PasswordHasher();

        [Fact]
        public void Should_Throw_Exception_For_Empty_Password_When_Hashing()
        {
            var password = "";
            Assert.Throws<ArgumentNullException>(() => _passwordHasher.HashPassword(password));
        }

        [Fact]
        public void Should_Hash_Passwords()
        {
            var firstPassword = "123456";
            var secondPassword = "123456";

            var firstPasswordAsHash = _passwordHasher.HashPassword(firstPassword);
            var secondPasswordAsHash = _passwordHasher.HashPassword(secondPassword);

            Assert.NotSame(firstPasswordAsHash, firstPassword);
            Assert.NotSame(secondPasswordAsHash, secondPassword);
            Assert.NotSame(firstPasswordAsHash, secondPasswordAsHash);
        }

        [Fact]
        public void Should_Match_Password_For_Valid_Hash()
        {
            var firstPassword = "123456";
            var firstPasswordAsHash = _passwordHasher.HashPassword(firstPassword);

            Assert.True(_passwordHasher.PasswordMatches(firstPassword, firstPasswordAsHash));
        }

        [Fact]
        public void Should_Return_False_For_Different_Hasher_Passwords()
        {
            var firstPassword = "123456";
            var secondPassword = "654321";

            var firstPasswordAsHash = _passwordHasher.HashPassword(firstPassword);
            var secondPasswordAsHash = _passwordHasher.HashPassword(secondPassword);

            Assert.False(_passwordHasher.PasswordMatches(firstPassword, secondPasswordAsHash));
            Assert.False(_passwordHasher.PasswordMatches(secondPassword, firstPasswordAsHash));
        }
    }
}