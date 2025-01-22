using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BangKa.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AccountController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginData)
        {
            if (loginData.Username == "admin" && loginData.Password == "password")
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, loginData.Username),
                    new Claim(ClaimTypes.Role, "Admin")
                };
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                return Ok(new { message = "Login successful" });
            }
            return Unauthorized(new { message = "Invalid username or password" });
        }

        [Authorize]
        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            // Xóa cookie xác thực khi người dùng logout
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Trả về thông báo logout thành công
            return Ok(new { message = "Logout successful" });
        }


        [Authorize]
        [HttpGet("LoadData")]
        public IActionResult LoadData()
        {
            // Danh sách tài khoản mẫu
            var accounts = new List<Account>
        {
            new Account { Username = "admin", FullName = "Administrator", Role = "Admin" },
            new Account { Username = "user1", FullName = "User One", Role = "User" },
            new Account { Username = "user2", FullName = "User Two", Role = "User" }
        };

            // Trả về danh sách tài khoản dưới dạng JSON
            return Ok(accounts);
        }
    }
    public class Account
    {
        // Tên đăng nhập của tài khoản
        public string Username { get; set; }

        // Họ tên người dùng
        public string FullName { get; set; }

        // Vai trò của tài khoản (ví dụ: Admin, User, v.v.)
        public string Role { get; set; }

        // Mật khẩu tài khoản (có thể mã hóa hoặc ẩn đi trong thực tế)
        public string Password { get; set; }

        // Constructor không tham số (optional)
        public Account() { }

        // Constructor có tham số để khởi tạo đối tượng Account
        public Account(string username, string fullName, string role, string password)
        {
            Username = username;
            FullName = fullName;
            Role = role;
            Password = password;
        }
    }
}
