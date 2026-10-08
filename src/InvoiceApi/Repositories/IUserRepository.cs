using InvoiceApi.Models;

namespace InvoiceApi.Repositories;

public interface IUserRepository
{
    Task<List<User>> GetAllAsync();
    Task<User?> GetByIdAsync(int id);
    Task <User?> GetByEmailAsync(string email);
}
