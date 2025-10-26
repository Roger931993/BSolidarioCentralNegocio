namespace CentralNegocio.Infrastructure.Services.Common.Api
{
    public interface IApiUrl
    {
        string Url { get; set; }
        TimeSpan TimeOut { get; set; }
        string Protocol { get; set; }
    }
}
