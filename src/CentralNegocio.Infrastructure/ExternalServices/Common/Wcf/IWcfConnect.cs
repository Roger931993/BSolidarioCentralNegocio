using System.ServiceModel;
using System.ServiceModel.Channels;

namespace CentralNegocio.Infrastructure.ExternalServices.Common.Wcf
{
  public interface IWcfConnect
  {
    Task<TResponse> ConnectAsync<TService, TRequest, TResponse>(TRequest request, IWcfUrl wcfUrl, IDictionary<string, string> Headers, Func<TService, TRequest, Task<TResponse>> operation, Guid IdTraking);
    Task<TResponse> ConnectAsync<TService, TRequest, TResponse>(TRequest request, IDictionary<string, string> Headers, Func<TService, TRequest, Task<TResponse>> operation, Guid IdTraking, Binding binding, EndpointAddress endpointAddress);
  }
}
