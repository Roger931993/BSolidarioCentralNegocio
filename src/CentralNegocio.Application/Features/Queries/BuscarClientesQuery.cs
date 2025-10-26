using CentralNegocio.Application.DTOs.Base;
using CentralNegocio.Application.DTOs.Clientes;
using MediatR;

namespace CentralNegocio.Application.Features.Queries
{
    public record BuscarClientesQuery(RequestBase<BuscarClientesRequest> request) : IRequest<ResponseBase<BuscarClientesResponse>>;

    public class BuscarClientesRequest
    {        
    }

    public class BuscarClientesResponse
    {
        public List<clienteDto>? clientes { get; set; }        
    }
}
