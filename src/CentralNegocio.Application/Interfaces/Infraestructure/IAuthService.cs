using CentralNegocio.Application.DTOs.Auth;

namespace CentralNegocio.Application.Interfaces.Infraestructure
{
    public interface IAuthService
    {
        Task<AuthResponse> GetToken(Guid IdTraker);
    }
}
