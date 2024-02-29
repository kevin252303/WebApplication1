using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;
using SQLitePCL;
using WebApplication1.DTOs;
using WebApplication1.Helpers;
using WebApplication1.Interfaces;
using WebApplication1.Models;

namespace WebApplication1.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public UserRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<MemberDTO> GetMemberAsync(string username)
        {
            return await _context.Users
                .Where(x=>x.UserName == username)
                .ProjectTo<MemberDTO>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();
        }

        public async Task<PagedList<MemberDTO>> GetMembersAsync(UserParams userParams)
        {
            var query = _context.Users
                .ProjectTo<MemberDTO>(_mapper.ConfigurationProvider)
                .AsNoTracking();
            return await PagedList<MemberDTO>.CreateAsync(query,userParams.PageNumber,userParams.PageSize);
        }

        public async Task<AppUsers> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<AppUsers> GetUserByNameAsync(string username)
        {
            return await _context.Users
                .Include(p => p.Photos)
                .SingleOrDefaultAsync(x => x.UserName == username);
        }

        public  async Task<IEnumerable<AppUsers>> getUsersAsyns()
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
