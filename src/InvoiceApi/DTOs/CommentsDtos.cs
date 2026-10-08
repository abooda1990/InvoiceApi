using System.ComponentModel.DataAnnotations;

namespace InvoiceApi.DTOs;

public record CommentDto(
    int Id,
    string Comment,
    string Author,
    DateTime CreatedAt);

public record CreateCommentDto(
    [Required, StringLength(1000, MinimumLength = 1)] string Comment);
