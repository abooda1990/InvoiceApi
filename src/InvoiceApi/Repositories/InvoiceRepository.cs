using InvoiceApi.Data;
using InvoiceApi.Models;
using Microsoft.EntityFrameworkCore;

namespace InvoiceApi.Repositories;

public class InvoiceRepository : IInvoiceRepository
{
    private readonly AppDbContext _db;

    public InvoiceRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<Invoice>> GetAllAsync(int? userId, InvoiceStatus? status)
    {
        var query = _db.Invoices
            .Include(i => i.User).ThenInclude(u => u.UserInfo)
            .Include(i => i.Comments)
            .AsQueryable();

        if (userId is not null)
        {
            query = query.Where(i => i.UserId == userId);
        }

        if (status is not null)
        {
            query = query.Where(i => i.Status == status);
        }

        return await query.OrderBy(i => i.DueDate).ToListAsync();
    }

    public Task<Invoice?> GetByIdAsync(int id)
    {
        return _db.Invoices
            .Include(i => i.User).ThenInclude(u => u.UserInfo)
            .Include(i => i.Comments).ThenInclude(c => c.User).ThenInclude(u => u.UserInfo)
            .Include(i => i.ViewAccess)
            .FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task AddAsync(Invoice invoice)
    {
        await _db.Invoices.AddAsync(invoice);
    }
     public void Delete(Invoice invoice)
    {
       _db.Invoices.Remove(invoice);
    }


    public Task SaveChangesAsync()
    {
        return _db.SaveChangesAsync();
    }
}
