namespace InvoiceApi.Models;

public class InvoiceComment
{
    public int Id { get; set; }
    public string Comment { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int InvoiceId { get; set; }
    public Invoice Invoice { get; set; } = null!;

    public int UserId { get; set; }
    public User User { get; set; } = null!;
}
