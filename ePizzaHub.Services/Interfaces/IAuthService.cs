using ePizzaHub.Core.Entities;
using ePizzaHub.Models;
using ePizzaHub.Services.Implementations;

namespace ePizzaHub.Services.Interfaces
{
    public interface IAuthService: IService<User>
    {
        bool CreateUser(User user, string Role);
        UserModel ValidateUser(string email, string password);
    }
}
