using System.Security.Claims;
using Investigacion1_back.Shared.Auth;
using Investigacion1_back.Shared.Contracts;
using Investigacion1_back.Shared.Domain;
using Investigacion1_back.Shared.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Investigacion1_back.Features.Auth.AdminRegister;

public sealed class AdminRegisterHandler
{
    private readonly AppDbContext _db;
    private readonly PasswordService _passwords;

    public AdminRegisterHandler(AppDbContext db, PasswordService passwords)
    {
        _db = db;
        _passwords = passwords;
    }

    public async Task<IResult> Handle(
        AdminRegisterRequest request,
        ClaimsPrincipal caller,
        CancellationToken cancellationToken)
    {
        var adminExists = await _db.Users.AnyAsync(
            user => user.Role == Roles.Admin,
            cancellationToken);

        if (adminExists)
        {
            if (caller.Identity?.IsAuthenticated != true)
            {
                return Results.Json(
                    new ErrorResponse("Authentication is required to create another Admin."),
                    statusCode: StatusCodes.Status401Unauthorized);
            }

            if (caller.GetRole() != Roles.Admin)
            {
                return Results.Json(
                    new ErrorResponse("Only an Admin can create another Admin."),
                    statusCode: StatusCodes.Status403Forbidden);
            }
        }

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
            Role = Roles.Admin,
            IsActive = true,
            SubscriptionExpirationDate = DateTime.UtcNow.AddYears(100)
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync(cancellationToken);

        return Results.Json(
            new AdminRegisterResponse(
                user.Id,
                user.Email,
                user.Role,
                user.IsActive,
                user.SubscriptionExpirationDate),
            statusCode: StatusCodes.Status201Created);
    }
}
