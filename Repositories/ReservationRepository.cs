using Microsoft.EntityFrameworkCore;
using RoadReady.Authentication;
using RoadReady.Models;
using RoadReady.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class ReservationRepository : IReservationRepository
{
    private readonly RoadReadyContext _context;

    public ReservationRepository(RoadReadyContext context)
    {
        _context = context;
    }

    public async Task<Reservation> GetReservationByIdAsync(int id)
    {
        return await _context.Reservations
                         .Include(r => r.Extras)  // Include the CarExtras in the query
                         .FirstOrDefaultAsync(r => r.ReservationId == id);
    }

    public async Task<IEnumerable<Reservation>> GetAllReservationsAsync()
    {
        return await _context.Reservations
           .Include(r => r.Extras) // Include CarExtras for each reservation
           .ToListAsync();
    }

    public async Task AddReservationAsync(Reservation reservation)
    {
        // Load CarExtras based on the CarExtraIds
        if (reservation.CarExtraIds != null && reservation.CarExtraIds.Any())
        {
            var selectedExtras = await _context.CarExtras
                                               .Where(extra => reservation.CarExtraIds.Contains(extra.ExtraId))
                                               .ToListAsync();

            // Add the selected CarExtras to the reservation
            foreach (var extra in selectedExtras)
            {
                reservation.Extras.Add(extra);
            }
        }

        await _context.Reservations.AddAsync(reservation);
        await _context.SaveChangesAsync();
    }


    public async Task UpdateReservationAsync(Reservation reservation)
    {
        var existingReservation = await _context.Reservations
            .Include(r => r.Extras)  // Include related CarExtras to handle updates
            .FirstOrDefaultAsync(r => r.ReservationId == reservation.ReservationId);

        if (existingReservation == null)
        {
            throw new Exception("Reservation not found.");
        }

        // Update reservation fields
        existingReservation.DropoffDate = reservation.DropoffDate;

        // Update the CarExtras (if needed)
        existingReservation.Extras.Clear();  // Remove all existing extras (or modify based on logic)

        foreach (var extraId in reservation.CarExtraIds)
        {
            var carExtra = await _context.CarExtras.FindAsync(extraId);
            if (carExtra != null)
            {
                existingReservation.Extras.Add(carExtra);  // Add the selected extra
            }
        }

        // Save changes to the database
        await _context.SaveChangesAsync();
    }



    public async Task DeleteReservationAsync(int id)
    {
        var reservation = await GetReservationByIdAsync(id);
        if (reservation != null)
        {
            _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();
        }
    }
}