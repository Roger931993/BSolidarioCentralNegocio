using CentralNegocio.Application.DTOs.Base;
using CentralNegocio.Application.DTOs.Clientes;
using CentralNegocio.Application.DTOs.Cuentas;
using MediatR;

namespace CentralNegocio.Application.Features.Queries
{
    public record BuscarClienteQuery(RequestBase<BuscarClienteRequest> request) : IRequest<ResponseBase<BuscarClienteResponse>>;

    public class BuscarClienteRequest
    {
        public int? cliente_id { get; set; }
    }

    public class BuscarClienteResponse
    {
        public clienteDto? cliente { get; set; }
        public List<cuentaDto>? cuentas { get; set; }
    }
}
