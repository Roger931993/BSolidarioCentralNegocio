using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace CentralNegocio.Infrastructure.ExternalServices.Common.Wcf
{
  public class ClienteHeaderEndpointBehavior: IEndpointBehavior
  {
    private readonly IDictionary<string, string> _header;

    public ClienteHeaderEndpointBehavior(IDictionary<string,string> Header)
    {
      _header = Header;
    }
    public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameter)
    {

    }
    public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
    {
      clientRuntime.ClientMessageInspectors.Add(new ClienteHeaderMeesageInspector(_header));
    }

    public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
    {     
    }

    public void Validate(ServiceEndpoint endpoint)
    {     
    }
  }

  public class ClienteHeaderMeesageInspector : IClientMessageInspector
  {
    private readonly IDictionary<string, string> _header;

    public ClienteHeaderMeesageInspector(IDictionary<string,string> header)
    {
      this._header = header;
    }

    public void AfterReceiveReply(ref Message reply, object correlationState)
    {     
    }

    public object BeforeSendRequest(ref Message request, IClientChannel channel)
    {
      if (_header!= null)
      {
        foreach (KeyValuePair<string, string> item in _header)
        {
          request.Headers.Add(MessageHeader.CreateHeader(item.Key, "http://temp.uri", item.Value));
        }
      }
      return null;
    }
  }
}
