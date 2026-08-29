using Investigacion1_back.Shared.Contracts;
using Investigacion1_back.Shared.Domain;
using Investigacion1_back.Shared.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Investigacion1_back.Features.Auth.Register;

public sealed class RegisterHandler
{
    private readonly AppDbContext _db;
    private readonly PasswordService _passwords;

    public RegisterHandler(AppDbContext db, PasswordService passwords)
    {
        _db = db;
        _passwords = passwords;
    }

    public async Task<IResult> Handle(RegisterRequest request, CancellationToken cancellationToken)
    {
        if (!PasswordPolicy.IsValid(request.Password))
        {
            return Results.BadRequest(new ErrorResponse(PasswordPolicy.ErrorMessage));
        }

        var email = request.Email?.Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
        {
            return Results.BadRequest(new ErrorResponse("A valid email is required."));
        }

        if (await _db.Users.AnyAsync(user => user.Email == email, cancellationToken))
        {
            return Results.BadRequest(new ErrorResponse("Email is already registered."));
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            PasswordHash = _passwords.Hash(request.Password),
            Role = Roles.SubscriptionL1,
            IsActive = true,
            SubscriptionExpirationDate = DateTime.UtcNow.AddYears(1)
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync(cancellationToken);

        return Results.Json(
            new RegisterResponse(
                user.Id,
                user.Email,
                user.Role,
                user.IsActive,
                user.SubscriptionExpirationDate),
            statusCode: StatusCodes.Status201Created);
    }
}
