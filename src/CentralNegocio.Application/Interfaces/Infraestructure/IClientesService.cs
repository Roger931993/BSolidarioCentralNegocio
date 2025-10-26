using CentralNegocio.Application.DTOs.Base;
using CentralNegocio.Application.DTOs.Clientes;

namespace CentralNegocio.Application.Interfaces.Infraestructure
{
    public interface IClientesService
    {
        Task<ResponseBase<GetClienteResponse>> Clientes(Guid IdTraker);
        Task<ResponseBase<GetClienteResponse>> ClientePorId(int cliente_id, Guid IdTraker);
        Task<ResponseBase<GetClienteResponse>> ClientePorCedula(string identificacion, Guid IdTraker);
        Task<ResponseBase<GetClienteResponse>> ClientePorUserName(string username, Guid IdTraker);
        Task<ResponseBase<RegisterClienteResponse>> Register(RegisterClienteRequest request, Guid IdTraker);

    }
}
