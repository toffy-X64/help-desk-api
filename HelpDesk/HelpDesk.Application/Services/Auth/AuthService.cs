using FluentValidation;
using FluentValidation.Results;

using HelpDesk.Core.Abstractions;
using HelpDesk.Core.Models;
using HelpDesk.Core.Models.Users;

using HelpDesk.Shared.Common;

namespace HelpDesk.Application.Services.Auth
{
    public class AuthService : BaseService
    {
        private readonly IUsersRepository _usersRepository;
        private readonly IJwtTokenService _jwtService;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IValidator<RegisterRequest> _registerValidator;
        private readonly IValidator<LoginRequest> _loginValidator;

        public AuthService(
            IUnitOfWork unitOfWork, 
            IUsersRepository usersRepository, 
            IJwtTokenService jwtService,
            IPasswordHasher passwordHasher,

            IValidator<RegisterRequest> registerValidator,
            IValidator<LoginRequest> loginValidator
        ) 
            : base(unitOfWork) 
        {
            _usersRepository = usersRepository;
            _jwtService = jwtService;
            _passwordHasher = passwordHasher;
            _registerValidator = registerValidator;
            _loginValidator = loginValidator;
        }

        public async Task<Result<UserDto>> Register(RegisterRequest request)
        {
            ValidationResult result = await _registerValidator.ValidateAsync(request);
            if (!result.IsValid)
            {
                var firstError = result.Errors.First();
                return new Error(firstError.ErrorMessage, firstError.PropertyName);
            }

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
                PasswordHash = _passwordHasher.HashPassword(request.Password),
                CreatedAt = DateTime.UtcNow,
                Role = "User"
            };
            _usersRepository.Add(user);
            await UnitOfWork.SaveChangesAsync();

            string token = _jwtService.GenerateUserToken(user);
            return ToUserDto(user, token);
        }

        public async Task<Result<UserDto>> Login(LoginRequest request)
        {
            ValidationResult result = await _loginValidator.ValidateAsync(request);
            if (!result.IsValid)
            {
                var firstError = result.Errors.First();
                return new Error(firstError.ErrorMessage, firstError.PropertyName);
            }

            var user = await _usersRepository.GetByEmail(request.Email);
            if (user == null)
            {
                return UserErrors.InvalidCredentials;
            }

            bool isPasswordCorrect = _passwordHasher.VerifyPassword(request.Password, user.PasswordHash);
            if (!isPasswordCorrect) 
            {
                return UserErrors.InvalidCredentials;
            }

            string token = _jwtService.GenerateUserToken(user);
            return ToUserDto(user, token);
        }

        public async Task<Result<UserDto>> Me(Guid userId)
        {
            var user = await _usersRepository.Get(userId);
            if (user == null) 
            {
                return UserErrors.InvalidCredentials;
            }

            string token = _jwtService.GenerateUserToken(user);
            return ToUserDto(user, token);
        }

        private UserDto ToUserDto(User user, string accessToken) => new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            CreatedAt = user.CreatedAt,
            Role = user.Role,
            AccessToken = accessToken
        };
    }
}
