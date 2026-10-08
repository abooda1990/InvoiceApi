namespace InvoiceApi.Models;

public class InvoiceViewAccess
{
    public int InvoiceId { get; set; }
    public Invoice Invoice { get; set; } = null!;

    public int UserId { get; set; }
    public User User { get; set; } = null!;
}

