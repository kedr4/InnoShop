using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserService.Models;

namespace UserService.Application.Users.Queries
{
    public class GetUsersQuery : IRequest<List<User>>
    {
    }
}
