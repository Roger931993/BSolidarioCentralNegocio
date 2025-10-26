using CentralNegocio.Application.DTOs.Base;
using MediatR;

namespace CentralNegocio.Application.Features.Commands
{
    public class SessionClienteCommand : RequestBase<SessionClienteRequest>, IRequest<ResponseBase<SessionClienteResponse>>
    {
    }

    public class SessionClienteRequest
    {
        public string? usuario { get; set; }
        public string? clave { get; set; }        
    }

    public class SessionClienteResponse
    {
        public Guid? session_id { get; set; }
    }
}
