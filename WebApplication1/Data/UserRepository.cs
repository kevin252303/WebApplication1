using Microsoft.EntityFrameworkCore;
using SQLitePCL;
using WebApplication1.Interfaces;
using WebApplication1.Models;

namespace WebApplication1.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;

        public UserRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<AppUsers> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<AppUsers> GetUserByNameAsync(string username)
        {
            return await _context.Users
                .Include(p => p.Photos)
                .SingleOrDefaultAsync(x => x.Name == username);
        }

        public async Task<IEnumerable<AppUsers>> getUsersAsyns()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<bool> saveAllAsyns()
        {
            return await _context.SaveChangesAsync()>0;
        }

        public void Update(AppUsers User)
        {
            _context.Entry(User).State = EntityState.Modified;
        }
    }
}
