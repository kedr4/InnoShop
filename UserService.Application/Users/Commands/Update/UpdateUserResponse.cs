using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.Application.Users.Commands.Update
{
    public class UpdateUserResponse
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }

        public UpdateUserResponse(bool success, string errorMessage = null)
        {
            Success = success;
            ErrorMessage = errorMessage;
        }
    }
}
