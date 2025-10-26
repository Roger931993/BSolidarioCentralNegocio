namespace CentralNegocio.Infrastructure.ExternalServices.Common.Wcf
{
  public interface IWcfUrl
  {
    string Url { get; }
    TimeSpan TimeOut { get; }
    string Protocol { get; set; }
  }
}
