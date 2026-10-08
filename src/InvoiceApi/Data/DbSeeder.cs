using InvoiceApi.Models;
using Microsoft.AspNetCore.Identity;

namespace InvoiceApi.Data;

public static class DbSeeder
{
    public static void Seed(AppDbContext db)
    {
        if (db.Users.Any())
        {
            return;
        }

        var kari = new User
        {
            Email = "kari@example.no",
            Role = UserRole.Approver,
            UserInfo = new UserInfo { FirstName = "Kari", LastName = "Nordmann", PhoneNumber = "+47 900 00 001" }
        };

        var ola = new User
        {
            Email = "ola@example.no",
            Role = UserRole.Approver,
            UserInfo = new UserInfo { FirstName = "Ola", LastName = "Hansen" }
        };

        var ingrid = new User
        {
            Email = "ingrid@example.no",
            Role = UserRole.Viewer,
            UserInfo = new UserInfo { FirstName = "Ingrid", LastName = "Hans" }
        };
        
        var karim = new User
        {
            Email = "karim@example.no",
            Role = UserRole.Admin,
            UserInfo = new UserInfo { FirstName = "karim", LastName = "Daher" }
        };

        db.Users.AddRange(kari, ola, ingrid, karim);

        var hasher = new PasswordHasher<User>();
        kari.PasswordHash = hasher.HashPassword(kari, "Kari123!");
        ola.PasswordHash = hasher.HashPassword(ola, "Ola123!");
        ingrid.PasswordHash = hasher.HashPassword(ingrid, "Ingrid123!");
        karim.PasswordHash = hasher.HashPassword(karim, "Karim123!");


        var office = new Invoice { RefId = "SUP-1001", Kid = "1234567890", Amount = 12500m, DueDate = DateTime.UtcNow.AddDays(14), User = kari };
        var cloud  = new Invoice { RefId = "SUP-1002", Kid = "2345678901", Amount = 3499.90m, DueDate = DateTime.UtcNow.AddDays(7), User = kari };
        var coffee = new Invoice { RefId = "SUP-1003", Kid = "3456789012", Amount = 870m, DueDate = DateTime.UtcNow.AddDays(3), User = kari };
        var laptop = new Invoice { RefId = "SUP-1004", Kid = "4567890123", Amount = 28990m, DueDate = DateTime.UtcNow.AddDays(-2), User = ola, Status = InvoiceStatus.Approved };
        var travel = new Invoice { RefId = "SUP-1005", Kid = "5678901234", Amount = 5400m, DueDate = DateTime.UtcNow.AddDays(10), User = ola, Status = InvoiceStatus.Rejected };

        office.Comments.Add(new InvoiceComment { Comment = "Please check the quantity before approving.", User = ola });
        office.ViewAccess.Add(new InvoiceViewAccess { User = ingrid });

        db.Invoices.AddRange(office, cloud, coffee, laptop, travel);

        db.SaveChanges();
    }
}
