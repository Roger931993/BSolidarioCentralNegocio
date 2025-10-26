using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralNegocio.Application.DTOs
{
    public class proyeccionDto
    {
        public int id { get; set; }
        public int? mes { get; set; }
        public int? año { get; set; }
        public decimal? interes { get; set; }
        public decimal? monto_interes { get; set; }
        public decimal? monto_total { get; set; }

    }
}
