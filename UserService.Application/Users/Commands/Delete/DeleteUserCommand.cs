using MediatR;
using UserService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.Application.Users.Commands.Delete

{
    public class DeleteUserCommand:IRequest<DeleteUserResponse>
    {
        public string UserId { get; set; }

        public DeleteUserCommand(string userId)
        {
            UserId = userId;
        }
    }
}
