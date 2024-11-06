using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserService.Infrastructure.Database;

namespace UserService.Application.Users.Commands.Update
{
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UpdateUserResponse>
    {
        private readonly DatabaseContext _context;

        public UpdateUserCommandHandler(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<UpdateUserResponse> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
        {
            var user = await _context.Users.FindAsync(command.UserId);

            if (user == null)
            {
                return new UpdateUserResponse(false, "User not found");
            }

            // Обновляем поля пользователя
            user.Name = command.Name;
            user.Lastname = command.Lastname;
            user.Email = command.Email;
            user.Role = command.Role;

            try
            {
                await _context.SaveChangesAsync();
                return new UpdateUserResponse(true);
            }
            catch (DbUpdateConcurrencyException)
            {
                return new UpdateUserResponse(false, "Concurrency error occurred");
            }
        }
    }

}
