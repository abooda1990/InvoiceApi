using InvoiceApi.DTOs;
using InvoiceApi.Exceptions;
using InvoiceApi.Mappings;
using InvoiceApi.Models;
using InvoiceApi.Repositories;
using Microsoft.AspNetCore.Identity;

namespace InvoiceApi.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _users;
    private readonly ITokenService _tokens;
    private readonly PasswordHasher<User> _hasher = new();

    public AuthService(IUserRepository users, ITokenService tokens)
    {
        _users = users;
        _tokens = tokens;
    }

    public async Task<LoginResponseDto> LoginAsync(LoginDto dto)
    {
        var user = await _users.GetByEmailAsync(dto.Email.Trim().ToLower())
            ?? throw new UnauthorizedException("Invalid email or password.");

        var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
        if (result == PasswordVerificationResult.Failed)
            throw new UnauthorizedException("Invalid email or password.");

        return new LoginResponseDto(_tokens.CreateToken(user), user.ToDto());
    }
}
