using InvoiceApi.Models;

namespace InvoiceApi.Repositories;

public interface IInvoiceRepository
{
    Task<List<Invoice>> GetAllAsync(int? userId, InvoiceStatus? status);
    Task<Invoice?> GetByIdAsync(int id);
    Task AddAsync(Invoice invoice);
    void Delete(Invoice invoice);
    Task SaveChangesAsync();
}
