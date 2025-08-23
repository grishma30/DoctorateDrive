using DoctorateDrive.Data;
using DoctorateDrive.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace DoctorateDrive.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DoctorateDrive.Data.DoctorateDriveContext _context;

        public UserRepository(DoctorateDrive.Data.DoctorateDriveContext context)
        {
            _context = context;
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.EmailId == email);
        }

        public async Task<User?> GetUserByIdAsync(int userId)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.UserId == userId);
        }

        public async Task<User> CreateUserAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User> UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            var user = await GetUserByIdAsync(userId);
            if (user == null)
                return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UserExistsAsync(string email)
        {
            return await _context.Users
                .AnyAsync(u => u.EmailId == email);
        }

        public async Task<bool> UserExistsAsync(string email, string mobileNumber)
        {
            return await _context.Users
                .AnyAsync(u => u.EmailId == email || u.MobileNumber == mobileNumber);
        }
    }
}
