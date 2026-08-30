using Investigacion1_back.Shared.Contracts;
using Investigacion1_back.Shared.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Investigacion1_back.Features.Reservations.GetReservations;

public sealed class GetReservationsHandler
{
    private readonly AppDbContext _db;

    public GetReservationsHandler(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IResult> Handle(GetReservationsQuery query, CancellationToken cancellationToken)
    {
        var reservations = await _db.Reservations
            .AsNoTracking()
            .Include(candidate => candidate.Room)
            .OrderBy(candidate => candidate.CheckInDate)
            .ToListAsync(cancellationToken);

        return Results.Ok(reservations.Select(ReservationResponse.From));
    }
}