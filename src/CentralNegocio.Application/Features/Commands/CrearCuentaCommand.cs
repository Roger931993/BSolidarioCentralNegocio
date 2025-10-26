using CentralNegocio.Application.DTOs.Base;
using CentralNegocio.Application.DTOs.Cuentas;
using MediatR;

namespace CentralNegocio.Application.Features.Commands
{
    public class CrearCuentaCommand : RequestBase<CrearCuentaRequest>, IRequest<ResponseBase<CrearCuentaResponse>>
    {
    }

    public class CrearCuentaRequest
    {
        public int? cliente_id { get; set; }
        public int? producto_id { get; set; }
        public string? moneda { get; set; }
        public string? tipo_cuenta { get; set; }

    }
    public class CrearCuentaResponse
    {
        public cuentaDto? cuenta { get; set; }
    }
}
