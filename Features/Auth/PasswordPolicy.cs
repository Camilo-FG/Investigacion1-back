namespace Investigacion1_back.Features.Auth;

public static class PasswordPolicy
{
    public const string ErrorMessage =
        "Password must be at least 6 characters and contain a letter and a number.";

    public static bool IsValid(string? password)
    {
        if (string.IsNullOrEmpty(password) || password.Length < 6)
        {
            return false;
        }

        return password.Any(char.IsLetter) && password.Any(char.IsDigit);
    }
}
