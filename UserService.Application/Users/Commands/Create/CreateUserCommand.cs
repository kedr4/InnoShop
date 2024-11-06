using MediatR;
using UserService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.Application.Users.Commands.Create
{
    public class CreateUserCommand:IRequest<CreateUserResponse>
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string Lastname { get; set; }
        public string Role { get; set; }
        public string Password { get; set; }
    }
}
