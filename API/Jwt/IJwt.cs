
using API.Domain.Entities;
using API.Dtos;

namespace API.Jwt
{
    public interface IJwt
    {
        AuthTokenDto Generate(User user);
    }
}