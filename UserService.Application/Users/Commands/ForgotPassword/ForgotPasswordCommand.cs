using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.Application.Users.Commands.ForgotPassword
{
        public class ForgotPasswordCommand : IRequest<ForgotPasswordResponse>
        {
            public string Email { get; set; }

            public ForgotPasswordCommand(string email)
            {
                Email = email;
            }
        }

}
