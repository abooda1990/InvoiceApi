using InvoiceApi.Models;
using InvoiceApi.Repositories;

namespace InvoiceApi.Tests.Fakes;

public class FakeInvoiceRepository : IInvoiceRepository
{
    public List<Invoice> Invoices { get; } = new();
    public int SaveCount { get; private set; }

    public Task<List<Invoice>> GetAllAsync(int? userId, InvoiceStatus? status)
    {
        var result = Invoices
            .Where(i => userId == null || i.UserId == userId)
            .Where(i => status == null || i.Status == status)
            .ToList();

        return Task.FromResult(result);
    }

    public Task<Invoice?> GetByIdAsync(int id)
    {
        var invoice = Invoices.FirstOrDefault(i => i.Id == id);
        return Task.FromResult(invoice);
    }

    public Task AddAsync(Invoice invoice)
    {
        invoice.Id = Invoices.Count + 1;
        Invoices.Add(invoice);
        return Task.CompletedTask;
    }
     public void Delete(Invoice invoice)
    {
       Invoices.Remove(invoice);
    }

    public Task SaveChangesAsync()
    {
        SaveCount++;
        return Task.CompletedTask;
    }
}
