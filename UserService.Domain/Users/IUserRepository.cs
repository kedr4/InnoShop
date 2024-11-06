﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserService.Models;

namespace UserService.Domain.Users
{
    public interface IUserRepository
    {
        public Task<string> CreateUserAsync(User user);
        public Task<User> GetUserByIdAsync(string Id);
        public Task<List<User>> GetAllUsersAsync();
        public Task<string> UpdateUserAsync(User user);
        public Task<bool> DeleteUserAsync(User user);

    }
}
