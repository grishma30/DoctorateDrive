using DoctorateDrive.Models;
using System.Threading.Tasks;

namespace DoctorateDrive.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> GetUserByIdAsync(int userId);
        Task<User> CreateUserAsync(User user);
        Task<User> UpdateUserAsync(User user);
        Task<bool> DeleteUserAsync(int userId);
        Task<bool> UserExistsAsync(string email);
        Task<bool> UserExistsAsync(string email, string mobileNumber);
    }
}
