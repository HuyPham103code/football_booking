using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using System.Text;
using FB_Booking.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();


// ============config=============

//  database
// Register your DbContext with a correct connection string
builder.Services.AddDbContext<FootballPitchBookingContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("BookingDatabase")));

////  service
builder.Services.AddScoped<FB_Booking.BBL.UserService>();
builder.Services.AddScoped<FB_Booking.BBL.BookingService>();

//  cors
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder
            .AllowAnyOrigin()  // CORS for all origins (can be restricted in production)
            .AllowAnyHeader()  // Allow all headers
            .AllowAnyMethod(); // Allow all HTTP methods (GET, POST, etc.)
    });
});



builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(x =>
    {
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]))
        };
    });

// swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API v1.0",
        Version = "v1",
        Description = "Swashbuckle",
        TermsOfService = new Uri("http://appointvn.com"),
        Contact = new OpenApiContact
        {
            Name = "Huy Pham",
            Email = "huypham1032003@gmail.com"
        },
        License = new OpenApiLicense
        {
            Name = "Apache 2.0",
            Url = new Uri("http://www.apache.org/licenses/LICENSE-2.0.html")
        }
    });
});

var app = builder.Build();

// Apply the CORS policy
app.UseCors("AllowAll");


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1.0");
        c.SwaggerEndpoint("/swagger/v2/swagger.json", "API v2.0");
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();



//  đang lỗi cái authenticated, tí tạo TestController  để test lại lần nữa
