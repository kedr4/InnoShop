using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.Application.Users.Commands.ResetPassword
{
    public class ResetPasswordCommand : IRequest<ResetPasswordResponse>
    {
        public string UserId { get; set; }
        public string Token { get; set; }
        public string NewPassword { get; set; }

        public ResetPasswordCommand(string userId, string token, string newPassword)
        {
            UserId = userId;
            Token = token;
            NewPassword = newPassword;
        }
    }
}
