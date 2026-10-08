using InvoiceApi.DTOs;
using InvoiceApi.Models;

namespace InvoiceApi.Services;

public interface IInvoiceService
{
    Task<List<InvoiceDto>> GetAllAsync(int? userId, InvoiceStatus? status);
    Task<InvoiceDto> GetByIdAsync(int id);
    Task<List<CommentDto>> GetCommentsAsync(int id);
    Task<InvoiceDto> CreateAsync(CreateInvoiceDto dto);
    Task<InvoiceDto> UpdateAsync(int id, int currentUserId, UpdateInvoiceDto dto);
    Task<InvoiceDto> ApproveAsync(int id, int currentUserId);
    Task<InvoiceDto> RejectAsync(int id, int currentUserId);
    Task<InvoiceDto> ForwardAsync(int id, int currentUserId, ForwardInvoiceDto dto);
    Task<CommentDto> AddCommentAsync(int id, int currentUserId, CreateCommentDto dto);
    Task DeleteAsync (int id);
}
