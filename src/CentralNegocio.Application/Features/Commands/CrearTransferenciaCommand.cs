using CentralNegocio.Application.DTOs.Base;
using CentralNegocio.Application.DTOs.Cuentas;
using MediatR;

namespace CentralNegocio.Application.Features.Commands
{
    public class CrearTransferenciaCommand : RequestBase<CrearTransferenciaRequest>, IRequest<ResponseBase<CrearTransferenciaResponse>>
    {
    }

    public class CrearTransferenciaRequest
    {
        public int? cliente_id_origen { get; set; }
        public int? cuenta_id_origen { get; set; }
        public int? cliente_id_destino { get; set; }
        public int? cuenta_id_destino { get; set; }
        public decimal? monto { get; set; }
        public string? motivo { get; set; }

    }
    public class CrearTransferenciaResponse
    {
        public cuentaDto? cuenta_origen { get; set; }
        public cuentaDto? cuenta_destino { get; set; }
    }
}
