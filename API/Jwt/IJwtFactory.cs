using API.Domain.Entities;
using System.Security.Claims;
using System.Threading.Tasks;
namespace API.Jwt
{
    public interface IJwtFactory
    {

        Task<string> GenerateEncodedToken(User user);




    }
}