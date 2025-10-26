using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralNegocio.Application.DTOs.Cuentas
{
    public class movimientoDto
    {
        public int? movimiento_id { get; set; }
        public int? cuenta_id { get; set; }
        public DateTime fecha_hora { get; set; }
        public string? tipo_movimiento { get; set; }
        public decimal? monto { get; set; }
        public string? naturaleza { get; set; }
        public decimal? saldo_resultante { get; set; }
        public string? motivo { get; set; }
        public string? referencia { get; set; }
        public string? estado_movimiento { get; set; }

        public cuentaDto? cuenta { get; set; }
    }
}
