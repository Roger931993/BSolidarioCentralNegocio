namespace CentralNegocio.Infrastructure.ExternalServices.Common.Wcf
{
  public class WcfUrl : IWcfUrl
  {
    public string Url { get; set; }
    public TimeSpan TimeOut { get; set; }
    public string Protocol { get; set; }
  }
}
