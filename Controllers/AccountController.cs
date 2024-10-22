using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using UserService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Azure.Core;
using Microsoft.AspNetCore.Identity.Data;

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
        private readonly EmailService _emailService; 
        private readonly UserManager<User> _userManager; 

        public AccountController(UserContext context, IPasswordHasher<User> passwordHasher, IConfiguration configuration, ILogger<AccountController> logger, EmailService emailService, UserManager<User> userManager)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _jwtSecret = configuration["JwtSettings:Secret"];
            _issuer = configuration["JwtSettings:Issuer"];
            _audience = configuration["JwtSettings:Audience"];
            _logger = logger;
            _emailService = emailService; 
            _userManager = userManager; 
        }

        public class RegisterRequest
        {
            public string Email { get; set; }
            public string Password { get; set; }
            public string Name { get; set; }
            public string Lastname { get; set; }
            public string Role { get; set; }
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] RegisterRequest request)
        {
            // Проверяем, существует ли уже пользователь с таким email
            var existingUser = await _context.Users.SingleOrDefaultAsync(u => u.Email == request.Email);
            if (existingUser != null)
            {
                return BadRequest("A user with this email already exists.");
            }

            // Создаем нового пользователя
            var user = new User
            {
                UserName = request.Email,
                Email = request.Email,
                Name = request.Name,
                Lastname = request.Lastname,
                Role = request.Role
            };

            // Хешируем пароль
            user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Генерация токена подтверждения
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = $"{Request.Scheme}://{Request.Host}/api/Account/confirm?userId={user.Id}&token={Uri.EscapeDataString(token)}";
            var subject = "Confirm your email";
            var body = $"Hello, {user.Name},<br><br>Please confirm your account by clicking <a href='{confirmationLink}'>here</a>.";

            // Отправка письма
            var emailSent = await _emailService.SendEmailAsync(user.Email, subject, body);
            if (!emailSent)
            {
                _logger.LogWarning($"Failed to send confirmation email to {user.Email}");
                return StatusCode(500, "Error sending confirmation email.");
            }

            return CreatedAtAction(nameof(Register), new { id = user.Id }, user);
        }

        [HttpGet("confirm")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return BadRequest("Invalid user.");
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                return BadRequest("Invalid or expired token.");
            }

            return Ok("Email confirmed successfully!");
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

            // Проверка пароля
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


      
         [HttpPost("forgot-password")]
         public async Task<ActionResult> ForgotPassword([FromBody] string email)
         {
             var user = await _userManager.FindByEmailAsync(email);
             if (user == null)
             {
                 _logger.LogWarning("Password reset requested for non-existent email: {Email}", email);
                 return BadRequest("No user found with that email address.");
             }

             var token = await _userManager.GeneratePasswordResetTokenAsync(user);
             _logger.LogInformation("Password reset token generated for user {UserId}: {Token}", user.Id, token);

             var resetLink = $"{Request.Scheme}://{Request.Host}/api/Account/reset-password?userId={user.Id}&token={Uri.EscapeDataString(token)}";
             var subject = "Reset your password";
             var body = $"Hello, {user.Name},<br><br>Please reset your password by clicking <a href='{resetLink}'>here</a>.";

             var emailSent = await _emailService.SendEmailAsync(user.Email, subject, body);
             if (!emailSent)
             {
                 _logger.LogWarning("Failed to send password reset email to {Email}", user.Email);
                 return StatusCode(500, "Error sending password reset email.");
             }

             _logger.LogInformation("Password reset email sent successfully to {Email}", user.Email);
             return Ok("Password reset email sent successfully.");
         }

         [HttpPost("reset-password")]
         public async Task<ActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
         {
            if (request == null)
            {
                return BadRequest("Invalid request.");
            }
            var user = await _userManager.FindByIdAsync(request.UserId);
             if (user == null)
             {
                 _logger.LogWarning("Password reset attempt with invalid user ID: {UserId}", request.UserId);
                 return BadRequest("Invalid user.");
             }

             _logger.LogInformation("Attempting password reset for user {UserId} with token {Token}", request.UserId, request.Token);
            // Проверка валидности токена
            var isValidToken = await _userManager.VerifyUserTokenAsync(user, "Default", "ResetPassword", request.Token);
            if (!isValidToken)
            {
                _logger.LogWarning("Password reset failed for user {UserId} due to invalid or expired token.", request.UserId);
                return BadRequest("Invalid or expired token.");
            }

            var result = await _userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);
             if (!result.Succeeded)
             {
                 _logger.LogWarning("Password reset failed for user {UserId} due to invalid or expired token.", request.UserId);
                 return BadRequest("Invalid or expired token.");
             }

             _logger.LogInformation("Password reset successfully for user {UserId}", user.Id);
             return Ok("Password has been reset successfully!");
         }
        
    }

    public class ResetPasswordRequest
    {
        public string UserId { get; set; }
        public string Token { get; set; }
        public string NewPassword { get; set; }
    }



    public class UserManagerResponse
    {
        public string Message { get; set; }
        public bool IsSuccess { get; set; }
    }
}
