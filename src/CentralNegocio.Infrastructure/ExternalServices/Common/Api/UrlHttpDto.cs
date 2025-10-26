namespace CentralNegocio.Infrastructure.ExternalServices.Common.Api
{
    public class UrlHttpDto : IApiUrl
    {
        public string Url { get; set; }
        public TimeSpan TimeOut { get; set; }
        public string Protocol { get; set; }
    }
}
