using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.Application.Users.Commands.Update
{
    public class UpdateUserCommand : IRequest<UpdateUserResponse>
    {
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }

        public UpdateUserCommand(string userId, string name, string lastname, string email, string role)
        {
            UserId = userId;
            Name = name;
            Lastname = lastname;
            Email = email;
            Role = role;
        }
    }

}
