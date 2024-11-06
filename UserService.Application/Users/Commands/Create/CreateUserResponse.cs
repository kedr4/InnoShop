using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.Application.Users.Commands.Create
{
    public class CreateUserResponse
    {
        public string Id { get; set; }
        public string Email { get; set; }
    }
}
