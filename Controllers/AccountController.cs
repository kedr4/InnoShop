using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using UserService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;

namespace UserService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly string _jwtSecret;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly ILogger<AccountController> _logger;   

        public AccountController(UserContext context, IPasswordHasher<User> passwordHasher, IConfiguration configuration, ILogger<AccountController> logger)
        {
            _context = context;
            _passwordHasher = passwordHasher;

            // Загрузка конфигурации JWT из appsettings.json
            _jwtSecret = configuration["JwtSettings:Secret"];
            _issuer = configuration["JwtSettings:Issuer"];
            _audience = configuration["JwtSettings:Audience"];
            _logger = logger;
        }

        // Регистрация пользователя
        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(User user)
        {
            // Хешируем пароль перед сохранением
            user.PasswordHash = _passwordHasher.HashPassword(user, user.PasswordHash);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Register), new { id = user.Id }, user);
        }

        // Аутентификация пользователя и генерация JWT токена
        [HttpPost("login")]
        public async Task<ActionResult<UserManagerResponse>> Login(string email, string password)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                return new UserManagerResponse
                {
                    Message = "No user found with that email address.",
                    IsSuccess = false,
                };
            }

            // Check if the password is correct
            var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
            if (passwordVerificationResult == PasswordVerificationResult.Failed)
            {
                return new UserManagerResponse
                {
                    Message = "Incorrect password.",
                    IsSuccess = false,
                };
            }

            // Генерация токена
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtSecret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email)
        }),
                Expires = DateTime.UtcNow.AddMinutes(60),
                Issuer = _issuer,
                Audience = _audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            _logger.LogInformation("JWT token generated: {Token}", tokenHandler.WriteToken(token));

            return Ok(token);
        }

    }

    public class UserManagerResponse
    {
        public string Message { get; set; }
        public bool IsSuccess { get; set; }
    }

}
