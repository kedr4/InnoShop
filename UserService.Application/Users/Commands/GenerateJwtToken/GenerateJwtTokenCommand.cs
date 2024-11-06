using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserService.Models;

namespace UserService.Application.Users.Commands.GenerateJwtToken
{
    public class GenerateJwtTokenCommand : IRequest<string>
    {
        public User User { get; }

        public GenerateJwtTokenCommand(User user)
        {
            User = user;
        }
    }

}
