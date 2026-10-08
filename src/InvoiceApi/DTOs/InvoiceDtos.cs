using System.ComponentModel.DataAnnotations;

namespace InvoiceApi.DTOs;

public record InvoiceDto(
    int Id,
    string RefId,
    string Kid,
    decimal Amount,
    DateTime DueDate,
    string Status,
    int AssignedToUserId,
    string AssignedTo,
    int CommentCount);

public record CreateInvoiceDto(
    [Required, StringLength(50)] string RefId,
    [Required, RegularExpression(@"^\d{2,25}$", ErrorMessage = "KID must be 2-25 digits.")] string Kid,
    [Range(0.01, 100_000_000)] decimal Amount,
    DateTime DueDate,
    int AssignedToUserId);

public record UpdateInvoiceDto(
    [Required, StringLength(50)] string RefId,
    [Required, RegularExpression(@"^\d{2,25}$", ErrorMessage = "KID must be 2-25 digits.")] string Kid,
    [Range(0.01, 100_000_000)] decimal Amount,
    DateTime DueDate);

public record ForwardInvoiceDto(
    int ToUserId,
    [StringLength(1000)] string? Comment);
