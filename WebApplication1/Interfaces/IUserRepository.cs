using WebApplication1.DTOs;
using WebApplication1.Helpers;
using WebApplication1.Models;

namespace WebApplication1.Interfaces
{
    public interface IUserRepository
    {
        void Update(AppUsers User);
        Task<bool> saveAllAsyns();
        Task<IEnumerable<AppUsers>> getUsersAsyns ();
        Task<AppUsers> GetUserByIdAsync(int id);
        Task<AppUsers> GetUserByNameAsync(string username);
        Task<PagedList<MemberDTO>> GetMembersAsync(UserParams userParams);
        Task<MemberDTO> GetMemberAsync(string username);

    }
}
