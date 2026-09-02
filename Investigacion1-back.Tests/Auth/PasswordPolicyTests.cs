using Investigacion1_back.Features.Auth;

namespace Investigacion1_back.Tests.Auth;

public class PasswordPolicyTests
{
    [Theory]
    [InlineData("abc12")] // too short
    [InlineData("abcdef")] // no number
    [InlineData("123456")] // no letter
    [InlineData("")]
    [InlineData(null)]
    public void IsValid_rejects_weak_passwords(string? password)
    {
        Assert.False(PasswordPolicy.IsValid(password));
    }

    [Theory]
    [InlineData("abc123")]
    [InlineData("Password1")]
    [InlineData("a1b2c3")]
    public void IsValid_accepts_letter_and_number_min_6(string password)
    {
        Assert.True(PasswordPolicy.IsValid(password));
    }
}
