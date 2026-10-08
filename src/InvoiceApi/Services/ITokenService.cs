using InvoiceApi.Models;

namespace InvoiceApi.Services;

public interface ITokenService
{
    string CreateToken(User user);
}
