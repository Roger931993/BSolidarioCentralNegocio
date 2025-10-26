namespace CentralNegocio.Application.DTOs.Auth
{
    public class AuthResponse
    {
        public string? access_token { get; set; }
        public string? token_type { get; set; }
        public int? expires_in { get; set; }
        public DateTime? creation_date { get; set; }      
    }
}
