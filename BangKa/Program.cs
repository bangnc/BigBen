
using BangKa.SignalR;
using BangKaData.DBContext;
using BangKaData.Repositories;
using BangKaService.Interfaces;
using BangKaService.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace WebApplication1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAngularApp", policy =>
                {
                    policy.WithOrigins("http://127.0.0.1:8080")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var jwtSettings = builder.Configuration.GetSection("Jwt");
            var secretKey = Encoding.UTF8.GetBytes(jwtSettings["Secret"]);

            builder.Services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtSettings["Issuer"],
                        ValidAudience = jwtSettings["Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(secretKey)
                    };
                });

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                 .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, cookieOptions =>
                 {
                     cookieOptions.Cookie.SameSite = SameSiteMode.None; // Đảm bảo cookie có thể được gửi qua CORS
                     cookieOptions.Cookie.HttpOnly = true;
                     cookieOptions.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                     cookieOptions.LoginPath = "/Account/Login"; // Đường dẫn đến trang đăng nhập
                     cookieOptions.LogoutPath = "/Account/Logout"; // Đường dẫn đến trang đăng xuất
                     cookieOptions.AccessDeniedPath = "/Account/AccessDenied"; // Trang khi bị từ chối truy cập
                     cookieOptions.Cookie.Name = "MyAppCookie"; // Tên cookie
                     cookieOptions.ExpireTimeSpan = TimeSpan.FromHours(1); // Thời gian sống của cookie
                 });

            builder.Services.AddAuthorization();
            builder.Services.AddSignalR();
            // add service
            builder.Services.AddDbContext<AppDbContext>(options =>
                                                        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<IProductService, ProductService>();

            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = builder.Configuration.GetConnectionString("Redis");
            });
            //
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseCors("AllowAngularApp");
            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapHub<ChatHub>("/chatHub");
            app.MapControllers();


            app.Run();
        }
    }
}
