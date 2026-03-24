using HelpDesk.Core.Abstractions;
using HelpDesk.Core.Models;
using HelpDesk.Core.Models.Users;
using HelpDesk.Shared.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDesk.Application.Services.Auth
{
    public class AuthService : BaseService
    {
        private readonly IUsersRepository _usersRepository;

        public AuthService(IUnitOfWork unitOfWork, IUsersRepository usersRepository) : base(unitOfWork) 
        { 
            _usersRepository = usersRepository;
        }

        public async Task<Result<UserDto>> Register(RegisterRequest request)
        {
            var isEmailTaken = await _usersRepository.IsEmailTaken(request.Email);
            if (isEmailTaken)
            {
                return UserErrors.EmailAlreadyTaken;
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                Name = request.Name,
                PasswordHash = request.Password,
                CreatedAt = DateTime.UtcNow,
                Role = "User"
            };
            _usersRepository.Add(user);
            await UnitOfWork.SaveChangesAsync();

            return ToUserDto(user);
        }

        public async Task<Result<UserDto>> Login(LoginRequest request)
        {
            return UserErrors.InvalidCredentials;
        }

        private UserDto ToUserDto(User user) => new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            CreatedAt = user.CreatedAt,
            Role = user.Role
        };
    }
}
