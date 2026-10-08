using InvoiceApi.Data;
using Microsoft.EntityFrameworkCore;
using InvoiceApi.Repositories;
using InvoiceApi.Services;
using InvoiceApi.Middleware;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;



var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>  
                       
    options.UseSqlite(builder.Configuration.GetConnectionString("Default"))); 

builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();   
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<IInvoiceService, InvoiceService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();




builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();   
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Paste the token only (without the word Bearer)."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});
            
var app = builder.Build();

app.UseMiddleware<ErrorHandlingMiddleware>();
using (var scope = app.Services.CreateScope())                        
{                                                                       
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();  
    db.Database.Migrate();                                              
    DbSeeder.Seed(db);                                                
}                                                                       





app.UseSwagger();                             
app.UseSwaggerUI();                            

app.MapGet("/", () => "Hello from Invoice API!");

app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();


app.Run();
