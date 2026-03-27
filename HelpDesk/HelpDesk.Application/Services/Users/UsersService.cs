using HelpDesk.Application.Services.Auth;
using HelpDesk.Application.Services.Users.Requests;
using HelpDesk.Core.Abstractions;
using HelpDesk.Core.Models;
using HelpDesk.Shared.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDesk.Application.Services.Users
{
    public class UsersService : BaseService
    {
        private readonly IUsersRepository _usersRepository;
        public UsersService(IUnitOfWork unitOfWork, IUsersRepository usersRepository) : base(unitOfWork) 
        { 
            _usersRepository = usersRepository;
        }

        public async Task<Result<IEnumerable<UserDto>>> GetAllUsers(GetUsersRequest request)
        {
            UserDto[] users = (await _usersRepository.Get(new SearchOptions
            {
                Limit = request.Limit,
                Offset = request.Offset,
            })).Select(MapToUserDto).ToArray();

            return users;
        }

        public async Task<Result<UserDto>> DeleteUser(Guid userId)
        {
            var user = await _usersRepository.Get(userId);
            if (user == null)
                return new Error("User not found");

            user.IsDeleted = true;
            _usersRepository.Update(user);
            await UnitOfWork.SaveChangesAsync();

            return MapToUserDto(user);
        }

        private UserDto MapToUserDto(User user) => new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            CreatedAt = user.CreatedAt,
            Role = user.Role
        };
    }
}
