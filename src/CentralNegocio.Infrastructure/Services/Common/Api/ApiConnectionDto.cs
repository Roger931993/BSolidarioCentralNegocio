namespace CentralNegocio.Infrastructure.Services.Common.Api
{
    public class ApiConnectionDto
    {
        public Dictionary<string, IApiUrl>? Values { get; set; }
        public ApiConnectionDto(Dictionary<string, IApiUrl>? values)
        {
            Values = values;
        }
    }
}
