using CentralNegocio.Application.DTOs.Base;
using CentralNegocio.Application.DTOs.Cuentas;
using MediatR;

namespace CentralNegocio.Application.Features.Commands
{
    public class CrearCuentaAhorroPlanCommand : RequestBase<CrearCuentaAhorroPlanRequest>, IRequest<ResponseBase<CrearCuentaAhorroPlanResponse>>
    {
    }

    public class CrearCuentaAhorroPlanRequest
    {
        public int? cuenta_id { get; set; }
        public int? cliente_id { get; set; }        
        public string? moneda { get; set; }
        public decimal? monto { get; set; }

    }
    public class CrearCuentaAhorroPlanResponse
    {
        public cuentaDto? cuenta { get; set; }
    }
}
