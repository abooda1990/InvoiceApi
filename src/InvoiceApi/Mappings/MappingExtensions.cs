using InvoiceApi.DTOs;
using InvoiceApi.Models;

namespace InvoiceApi.Mappings;

public static class MappingExtensions
{
    public static string GetFullName(this User user)
    {
        return $"{user.UserInfo.FirstName} {user.UserInfo.LastName}";
    }

    public static UserDto ToDto(this User user)
    {
        return new UserDto(
            user.Id,
            user.Email,
            user.GetFullName(),
            user.Role.ToString());
    }

    public static InvoiceDto ToDto(this Invoice invoice)
    {
        return new InvoiceDto(
            invoice.Id,
            invoice.RefId,
            invoice.Kid,
            invoice.Amount,
            invoice.DueDate,
            invoice.Status.ToString(),
            invoice.UserId,
            invoice.User.GetFullName(),
            invoice.Comments.Count);
    }

    public static CommentDto ToDto(this InvoiceComment comment)
    {
        return new CommentDto(
            comment.Id,
            comment.Comment,
            comment.User.GetFullName(),
            comment.CreatedAt);
    }
}
