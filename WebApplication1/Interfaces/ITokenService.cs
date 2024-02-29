using WebApplication1.Models;

namespace WebApplication1.Interfaces
{
    public interface ITokenServise
    {
        string CreateToken(AppUsers users);
    }
}
