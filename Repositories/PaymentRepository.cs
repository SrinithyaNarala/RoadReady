using Microsoft.EntityFrameworkCore;
using RoadReady.Authentication;
using RoadReady.Models;
using RoadReady.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class PaymentRepository : IPaymentRepository
{
    private readonly RoadReadyContext _context;

    public PaymentRepository(RoadReadyContext context)
    {
        _context = context;
    }

    public async Task<Payment> GetPaymentByIdAsync(int id)
    {
        return await _context.Payments.FindAsync(id);
    }

    public async Task<IEnumerable<Payment>> GetAllPaymentsAsync()
    {
        return await _context.Payments.ToListAsync();
    }

    public async Task AddPaymentAsync(Payment payment)
    {
        await _context.Payments.AddAsync(payment);
        await _context.SaveChangesAsync();
    }

    public async Task UpdatePaymentAsync(Payment payment)
    {
        // Check for a tracked instance with the same key
        var trackedEntity = _context.ChangeTracker.Entries<Payment>()
                                    .FirstOrDefault(e => e.Entity.PaymentId == payment.PaymentId);

        if (trackedEntity != null)
        {
            // Detach the tracked instance to avoid conflict
            trackedEntity.State = EntityState.Detached;
        }

        // Attach and update the payment entity
        _context.Payments.Update(payment);

        // Save changes
        await _context.SaveChangesAsync();
    }


    public async Task DeletePaymentAsync(int id)
    {
        var payment = await GetPaymentByIdAsync(id);
        if (payment != null)
        {
            _context.Payments.Remove(payment);
            await _context.SaveChangesAsync();
        }
    }
}