using Investigacion1_back.Shared.Infrastructure;

namespace Investigacion1_back.Tests.Auth;

public class PasswordServiceTests
{
    private readonly PasswordService _sut = new();

    [Fact]
    public void Hash_and_Verify_roundtrip()
    {
        var hash = _sut.Hash("Secret1");

        Assert.True(_sut.Verify("Secret1", hash));
        Assert.False(_sut.Verify("wrong-password", hash));
    }

    [Fact]
    public void Hash_uses_unique_salt_per_call()
    {
        var first = _sut.Hash("Secret1");
        var second = _sut.Hash("Secret1");

        Assert.NotEqual(first, second);
        Assert.True(_sut.Verify("Secret1", first));
        Assert.True(_sut.Verify("Secret1", second));
    }
}
