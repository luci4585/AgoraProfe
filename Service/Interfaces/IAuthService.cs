using Service.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IAuthService
    {
        Task<string?> Login(LoginDTO login);
        Task<bool> ResetPassword(LoginDTO? login);
        Task<bool> CreateUserWithEmailAndPasswordAsync(string email, string password, string nombre);
    }
}

