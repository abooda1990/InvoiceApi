using InvoiceApi.Models;
using InvoiceApi.Repositories;

namespace InvoiceApi.Tests.Fakes;

public class FakeUserRepository : IUserRepository
{
    public List<User> Users { get; } = new();

    public Task<List<User>> GetAllAsync()
    {
        return Task.FromResult(Users.ToList());
    }

    public Task<User?> GetByIdAsync(int id)
    {
        var user = Users.FirstOrDefault(u => u.Id == id);
        return Task.FromResult(user);
    }
        public Task<User?> GetByEmailAsync(string email)
    {
        var user = Users.FirstOrDefault(u => u.Email == email);
        return Task.FromResult(user);
    }

}
