namespace InvoiceApi.Models;

public class Invoice
{
    public int Id { get; set; }
    public string RefId { get; set; } = string.Empty;
    public string Kid { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime DueDate { get; set; }
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Pending;

    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public List<InvoiceComment> Comments { get; set; } = new();
    public List<InvoiceViewAccess> ViewAccess { get; set; } = new();

}
