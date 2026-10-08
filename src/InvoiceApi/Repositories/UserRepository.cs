using InvoiceApi.Data;
using InvoiceApi.Models;
using Microsoft.EntityFrameworkCore;

namespace InvoiceApi.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _db;

    public UserRepository(AppDbContext db)
    {
        _db = db;
    }

    public Task<List<User>> GetAllAsync()
    {
        return _db.Users
            .Include(u => u.UserInfo)
            .OrderBy(u => u.Id)
            .ToListAsync();
    }

    public Task<User?> GetByIdAsync(int id)
    {
        return _db.Users
            .Include(u => u.UserInfo)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public Task<User?> GetByEmailAsync(string email)
    {
        return _db.Users 
            .Include(u => u.UserInfo)
            .FirstOrDefaultAsync(u => u.Email == email);
    }
}
