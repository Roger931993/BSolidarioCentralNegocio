using CentralNegocio.Application.DTOs;
using CentralNegocio.Application.DTOs.Base;
using CentralNegocio.Application.DTOs.Clientes;
using CentralNegocio.Application.DTOs.Cuentas;
using MediatR;

namespace CentralNegocio.Application.Features.Queries
{
    public record ProyeccionAhorroQuery(RequestBase<ProyeccionAhorroRequest> request) : IRequest<ResponseBase<ProyeccionAhorroResponse>>;

    public class ProyeccionAhorroRequest
    {
        public int? cuenta_id { get; set; }
        public int? cliente_id { get; set; }
    }

    public class ProyeccionAhorroResponse
    {
        public clienteDto? cliente { get; set; }
        public cuentaDto? cuenta { get; set; }
        public List<proyeccionDto>? proyeccion { get; set; }
    }
}
