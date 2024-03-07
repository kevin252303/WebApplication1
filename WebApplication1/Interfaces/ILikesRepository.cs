using WebApplication1.DTOs;
using WebApplication1.Helpers;
using WebApplication1.Models;

namespace WebApplication1.Interfaces
{
    public interface ILikesRepository
    {
        Task<UserLike> GetUserLike(int sourceuserId, int targetuserId);
        Task<AppUsers> GetUserWithLikes(int userId);
        Task<PagedList<LikeDTO>> GetUserLikes(LikesParams likesParams);
    }
}
