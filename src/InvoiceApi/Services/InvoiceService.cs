using InvoiceApi.DTOs;
using InvoiceApi.Exceptions;
using InvoiceApi.Mappings;
using InvoiceApi.Models;
using InvoiceApi.Repositories;

namespace InvoiceApi.Services;

public class InvoiceService : IInvoiceService
{
    private readonly IInvoiceRepository _invoices;
    private readonly IUserRepository _users;

    public InvoiceService(IInvoiceRepository invoices, IUserRepository users)
    {
        _invoices = invoices;
        _users = users;
    }

    // ───────────── القراءة ─────────────

    public async Task<List<InvoiceDto>> GetAllAsync(int? userId, InvoiceStatus? status)
    {
        var invoices = await _invoices.GetAllAsync(userId, status);
        return invoices.Select(i => i.ToDto()).ToList();
    }

    public async Task<InvoiceDto> GetByIdAsync(int id)
    {
        var invoice = await GetInvoiceOrThrowAsync(id);
        return invoice.ToDto();
    }

    public async Task<List<CommentDto>> GetCommentsAsync(int id)
    {
        var invoice = await GetInvoiceOrThrowAsync(id);
        return invoice.Comments
            .OrderBy(c => c.CreatedAt)
            .Select(c => c.ToDto())
            .ToList();
    }

    // ───────────── الإنشاء والتعديل ─────────────

    public async Task<InvoiceDto> CreateAsync(CreateInvoiceDto dto)
    {
        var assignee = await GetUserOrThrowAsync(dto.AssignedToUserId);
        EnsureCanBeResponsible(assignee);

        var invoice = new Invoice
        {
            RefId = dto.RefId,
            Kid = dto.Kid,
            Amount = dto.Amount,
            DueDate = dto.DueDate,
            User = assignee
        };

        await _invoices.AddAsync(invoice);
        await _invoices.SaveChangesAsync();

        return invoice.ToDto();
    }

    public async Task<InvoiceDto> UpdateAsync(int id, int currentUserId, UpdateInvoiceDto dto)
    {
        var invoice = await GetInvoiceOrThrowAsync(id);
        var currentUser = await GetUserOrThrowAsync(currentUserId);

        EnsureIsOpen(invoice);
        EnsureIsResponsible(invoice, currentUser);

        invoice.RefId = dto.RefId;
        invoice.Kid = dto.Kid;
        invoice.Amount = dto.Amount;
        invoice.DueDate = dto.DueDate;

        await _invoices.SaveChangesAsync();

        return invoice.ToDto();
    }
    public async Task DeleteAsync(int id)
    {
        var invoice =  await GetInvoiceOrThrowAsync(id);
       if(invoice.Status == InvoiceStatus.Approved)
        {
            throw new BusinessRuleException(
                "Approved invoices cannot be deleted.");
        }
        _invoices.Delete(invoice);

        await  _invoices.SaveChangesAsync();
   
    
    }
    // ───────────── القرارات ─────────────

    public Task<InvoiceDto> ApproveAsync(int id, int currentUserId)
    {
        return DecideAsync(id, currentUserId, InvoiceStatus.Approved);
    }

    public Task<InvoiceDto> RejectAsync(int id, int currentUserId)
    {
        return DecideAsync(id, currentUserId, InvoiceStatus.Rejected);
    }

    public async Task<InvoiceDto> ForwardAsync(int id, int currentUserId, ForwardInvoiceDto dto)
    {
        var invoice = await GetInvoiceOrThrowAsync(id);
        var currentUser = await GetUserOrThrowAsync(currentUserId);

        EnsureIsOpen(invoice);
        EnsureIsResponsible(invoice, currentUser);

        if (dto.ToUserId == invoice.UserId)
        {
            throw new BusinessRuleException("The invoice is already assigned to this user.");
        }

        var target = await GetUserOrThrowAsync(dto.ToUserId);
        EnsureCanBeResponsible(target);

        invoice.UserId = target.Id;
        invoice.User = target;
        invoice.Status = InvoiceStatus.Forwarded;

        if (!string.IsNullOrWhiteSpace(dto.Comment))
        {
            invoice.Comments.Add(new InvoiceComment { Comment = dto.Comment, User = currentUser });
        }

        await _invoices.SaveChangesAsync();

        return invoice.ToDto();
    }

    // ───────────── التعليقات ─────────────

    public async Task<CommentDto> AddCommentAsync(int id, int currentUserId, CreateCommentDto dto)
    {
        var invoice = await GetInvoiceOrThrowAsync(id);
        var currentUser = await GetUserOrThrowAsync(currentUserId);

        EnsureCanView(invoice, currentUser);

        var comment = new InvoiceComment { Comment = dto.Comment, User = currentUser };
        invoice.Comments.Add(comment);

        await _invoices.SaveChangesAsync();

        return comment.ToDto();
    }

    // ───────────── دوال مساعدة (private) ─────────────

    private async Task<InvoiceDto> DecideAsync(int id, int currentUserId, InvoiceStatus decision)
    {
        var invoice = await GetInvoiceOrThrowAsync(id);
        var currentUser = await GetUserOrThrowAsync(currentUserId);

        EnsureIsOpen(invoice);
        EnsureIsResponsible(invoice, currentUser);

        invoice.Status = decision;
        await _invoices.SaveChangesAsync();

        return invoice.ToDto();
    }

    private async Task<Invoice> GetInvoiceOrThrowAsync(int id)
    {
        return await _invoices.GetByIdAsync(id)
            ?? throw new NotFoundException($"Invoice {id} was not found.");
    }

    private async Task<User> GetUserOrThrowAsync(int id)
    {
        return await _users.GetByIdAsync(id)
            ?? throw new NotFoundException($"User {id} was not found.");
    }

    private static void EnsureIsOpen(Invoice invoice)
    {
        if (invoice.Status is not (InvoiceStatus.Pending or InvoiceStatus.Forwarded))
        {
            throw new BusinessRuleException(
                $"Invoice {invoice.Id} is already {invoice.Status} and can no longer be changed.");
        }
    }

    private static void EnsureCanBeResponsible(User user)
    {
        if (user.Role == UserRole.Viewer)
        {
            throw new BusinessRuleException(
                $"{user.GetFullName()} is a viewer and cannot be responsible for invoices.");
        }
    }

    private static void EnsureIsResponsible(Invoice invoice, User user)
    {
        if (user.Role == UserRole.Admin)
        {
            return;
        }

        if (invoice.UserId != user.Id)
        {
            throw new ForbiddenException("Only the responsible user can act on this invoice.");
        }
    }

    private static void EnsureCanView(Invoice invoice, User user)
    {
        var canView = user.Role == UserRole.Admin
            || invoice.UserId == user.Id
            || invoice.ViewAccess.Any(a => a.UserId == user.Id);

        if (!canView)
        {
            throw new ForbiddenException("You do not have access to this invoice.");
        }
    }
}
