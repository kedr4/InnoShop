using Microsoft.AspNetCore.Identity;
namespace UserService.Models
{
    public class User:IdentityUser
    {
        public int Id { get; set; } 
        public string Name { get; set; }    
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string UserName { get; set; } = string.Empty;
       // public string PasswordHash { get; set; }
    }
}
