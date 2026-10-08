namespace InvoiceApi.Models;

public class User
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public UserRole Role { get; set; } = UserRole.Viewer;

    public int UserInfoId { get; set; }
    public UserInfo UserInfo { get; set; } = null!;

    public List<Invoice> Invoices { get; set; } = new();
}
