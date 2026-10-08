using InvoiceApi.DTOs;

namespace InvoiceApi.Services;

public interface IAuthService
{
    Task<LoginResponseDto> LoginAsync(LoginDto dto);
}
