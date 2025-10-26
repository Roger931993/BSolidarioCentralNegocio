using CentralNegocio.Application.DTOs.Base;
using CentralNegocio.Application.DTOs.Cuentas;
using MediatR;

namespace CentralNegocio.Application.Features.Commands
{
    public class CrearDepositoCommand : RequestBase<CrearDepositoRequest>, IRequest<ResponseBase<CrearDepositoResponse>>
    {
    }

    public class CrearDepositoRequest
    {
        public int? cliente_id { get; set; }
        public int? cuenta_id { get; set; }
        public decimal? monto { get; set; }

    }
    public class CrearDepositoResponse
    {
        public cuentaDto? cuenta { get; set; }
    }
}
