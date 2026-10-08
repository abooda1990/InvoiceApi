namespace InvoiceApi.DTOs;

public record LoginDto(
    string Email,
    string Password
    );

public record LoginResponseDto(
    string Token,
    UserDto User
    );
