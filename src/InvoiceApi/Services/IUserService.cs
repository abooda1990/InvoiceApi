using InvoiceApi.DTOs;

namespace InvoiceApi.Services;

public interface IUserService
{
    Task<List<UserDto>> GetAllAsync();
    Task<UserDto> GetByIdAsync(int id);
}
