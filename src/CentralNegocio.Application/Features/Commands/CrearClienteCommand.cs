using CentralNegocio.Application.DTOs.Base;
using CentralNegocio.Application.DTOs.Clientes;
using MediatR;

namespace CentralNegocio.Application.Features.Commands
{
    public class CrearClienteCommand : RequestBase<CrearClienteRequest>, IRequest<ResponseBase<CrearClienteResponse>>
    {

    }
    public class CrearClienteRequest
    {
        public string? primer_nombre { get; set; }
        public string? segundo_nombre { get; set; }
        public string? apellido_paterno { get; set; }
        public string? apellido_materno { get; set; }
        public string? identificacion { get; set; }
        public string? user_name { get; set; }
    }

    public class CrearClienteResponse
    {
        public clienteDto? cliente { get; set; }
    }
}
