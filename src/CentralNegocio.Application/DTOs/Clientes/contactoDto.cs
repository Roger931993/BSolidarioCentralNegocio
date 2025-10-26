namespace CentralNegocio.Application.DTOs.Clientes
{
    public class contactoDto
    {
        public int? contacto_id { get; set; }
        public int? cliente_id { get; set; }
        public string? descripcion { get; set; }
        public string? email { get; set; }
        public string? telefono { get; set; }

    }
}
