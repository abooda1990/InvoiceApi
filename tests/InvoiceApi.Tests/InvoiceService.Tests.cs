using InvoiceApi.Exceptions;
using InvoiceApi.Models;
using InvoiceApi.Services;
using InvoiceApi.Tests.Fakes;
using NUnit.Framework;

namespace InvoiceApi.Tests;

[TestFixture]
public class InvoiceServiceTests
{
    private FakeInvoiceRepository _invoices = null!;
    private FakeUserRepository _users = null!;
    private InvoiceService _service = null!;

    private User _approver = null!;
    private User _otherApprover = null!;
    private User _admin = null!;

    [SetUp]
    public void SetUp()
    {
        _invoices = new FakeInvoiceRepository();
        _users = new FakeUserRepository();
        _service = new InvoiceService(_invoices, _users);

        _approver = AddUser(1, UserRole.Approver);
        _otherApprover = AddUser(2, UserRole.Approver);
        _admin = AddUser(3, UserRole.Admin);
    }

    // ───────────── Tests ─────────────

    [Test]
    public async Task Approve_PendingInvoice_ByResponsibleUser_SetsApproved()
    {
        // Arrange
        var invoice = AddInvoice(_approver, InvoiceStatus.Pending);

        // Act
        var result = await _service.ApproveAsync(invoice.Id, _approver.Id);

        // Assert
        Assert.That(result.Status, Is.EqualTo("Approved"));
        Assert.That(_invoices.SaveCount, Is.EqualTo(1));
    }

    [Test]
    public void Approve_RejectedInvoice_ThrowsBusinessRule()
    {
        var invoice = AddInvoice(_approver, InvoiceStatus.Rejected);

        Assert.ThrowsAsync<BusinessRuleException>(
            () => _service.ApproveAsync(invoice.Id, _approver.Id));

        Assert.That(_invoices.SaveCount, Is.EqualTo(0));
    }

    [Test]
    public void Approve_ByAnotherApprover_ThrowsForbidden()
    {
        var invoice = AddInvoice(_approver, InvoiceStatus.Pending);

        Assert.ThrowsAsync<ForbiddenException>(
            () => _service.ApproveAsync(invoice.Id, _otherApprover.Id));
    }

    [Test]
    public async Task Approve_ByAdmin_OnSomeoneElsesInvoice_Succeeds()
    {
        var invoice = AddInvoice(_approver, InvoiceStatus.Pending);

        var result = await _service.ApproveAsync(invoice.Id, _admin.Id);

        Assert.That(result.Status, Is.EqualTo("Approved"));
    }

    [Test]
    public void GetById_UnknownInvoice_ThrowsNotFound()
    {
        Assert.ThrowsAsync<NotFoundException>(
            () => _service.GetByIdAsync(999));
    }

    [Test]
    public async Task Delete_PendingInvoice_RemovesItAndSaves()
    {
        var invoice = AddInvoice(_approver, InvoiceStatus.Pending);

        await _service.DeleteAsync(invoice.Id);

        Assert.That(_invoices.Invoices, Does.Not.Contain(invoice));
        Assert.That(_invoices.SaveCount, Is.EqualTo(1));
    }

    [Test]
    public async Task Delete_RejectedInvoice_IsAllowed()
    {
        var invoice = AddInvoice(_approver, InvoiceStatus.Rejected);

        await _service.DeleteAsync(invoice.Id);

        Assert.That(_invoices.Invoices, Does.Not.Contain(invoice));
    }

    [Test]
    public void Delete_ApprovedInvoice_ThrowsBusinessRule_AndKeepsIt()
    {
        var invoice = AddInvoice(_approver, InvoiceStatus.Approved);

        Assert.ThrowsAsync<BusinessRuleException>(
            () => _service.DeleteAsync(invoice.Id));

        Assert.That(_invoices.Invoices, Does.Contain(invoice));
        Assert.That(_invoices.SaveCount, Is.EqualTo(0));
    }

    [Test]
    public void Delete_UnknownInvoice_ThrowsNotFound()
    {
        Assert.ThrowsAsync<NotFoundException>(
            () => _service.DeleteAsync(999));

        Assert.That(_invoices.SaveCount, Is.EqualTo(0));
    }

    // ───────────── Helpers ─────────────

    private User AddUser(int id, UserRole role)
    {
        var user = new User
        {
            Id = id,
            Role = role,
            UserInfo = new UserInfo { FirstName = "User", LastName = id.ToString() }
        };
        _users.Users.Add(user);
        return user;
    }

    private Invoice AddInvoice(User owner, InvoiceStatus status)
    {
        var invoice = new Invoice
        {
            Id = _invoices.Invoices.Count + 1,
            RefId = "REF-1",
            Kid = "12345",
            Amount = 100,
            DueDate = DateTime.UtcNow.AddDays(14),
            Status = status,
            UserId = owner.Id,
            User = owner
        };
        _invoices.Invoices.Add(invoice);
        return invoice;
    }
}
