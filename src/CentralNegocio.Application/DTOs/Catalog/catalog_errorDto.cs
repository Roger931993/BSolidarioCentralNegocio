namespace CentralNegocio.Application.DTOs.Catalog
{
    public class catalog_errorDto
    {
        public int catalog_error_id { get; set; }
        public string? error_description { get; set; }
        public string? error_priority { get; set; }
        public int error_status_code { get; set; }
    }
}
