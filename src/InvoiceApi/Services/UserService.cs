using InvoiceApi.DTOs;
using InvoiceApi.Exceptions;
using InvoiceApi.Mappings;
using InvoiceApi.Repositories;

namespace InvoiceApi.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _users;

    public UserService(IUserRepository users)
    {
        _users = users;
    }

    public async Task<List<UserDto>> GetAllAsync()
    {
        var users = await _users.GetAllAsync();
        return users.Select(u => u.ToDto()).ToList();
    }

    public async Task<UserDto> GetByIdAsync(int id)
    {
        var user = await _users.GetByIdAsync(id)
            ?? throw new NotFoundException($"User {id} was not found.");

        return user.ToDto();
    }
}
