using ePizzaHub.Core.Entities;
using ePizzaHub.Models;
using ePizzaHub.Repositories.Interfaces;
using ePizzaHub.Services.Interfaces;

namespace ePizzaHub.Services.Implementations
{
    public class AuthService : Service<User>, IAuthService
    {
        IUserRepository _userRepo;
        public AuthService(IUserRepository repository): base(repository)
        {
            _userRepo = repository;

        }
        
        public bool CreateUser(User user, string Role)
        {
            return _userRepo.CreateUser(user, Role);
        }

        public UserModel ValidateUser(string email, string password)
        {
            return _userRepo.ValidateUser(email, password);
        }
    }
}
