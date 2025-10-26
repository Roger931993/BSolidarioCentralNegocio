using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralNegocio.Application.DTOs.Cuentas
{
    public class cuentaDto
    {
        public int? cuenta_id { get; set; }
        public string? numero_cuenta { get; set; }
        public int? cliente_id { get; set; }
        public int? producto_id { get; set; }
        public int? agencia_id { get; set; }
        public string? moneda { get; set; }
        public string? tipo_cuenta { get; set; }
        public DateTime? fecha_apertura { get; set; }
        public DateTime? fecha_cierre { get; set; }
        public decimal? saldo_actual { get; set; }
        public decimal? saldo_disponible { get; set; }
        public decimal? tasa_interes { get; set; }
        public DateTime? fecha_ultima_transaccion { get; set; }
        public string? usuario_creacion { get; set; }
    }
}
