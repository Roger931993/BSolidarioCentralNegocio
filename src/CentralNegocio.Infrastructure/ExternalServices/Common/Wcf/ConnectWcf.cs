using CentralNegocio.Application.Interfaces.Infraestructure;
using CentralNegocio.Domain.Common;
using System.Runtime.ExceptionServices;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Xml.Serialization;

namespace CentralNegocio.Infrastructure.ExternalServices.Common.Wcf
{
    public class ConnectWcf : IWcfConnect
    {
        private readonly IMemoryCacheLocalService _memoryCacheLocalService;

        public ConnectWcf(IMemoryCacheLocalService memoryCacheLocalService)
        {
            _memoryCacheLocalService = memoryCacheLocalService;
        }

        public async Task<TResponse> ConnectAsync<TService, TRequest, TResponse>(TRequest request, IWcfUrl wcfUrl, IDictionary<string, string> Headers, Func<TService, TRequest, Task<TResponse>> operation, Guid IdTraking)
        {
            DataCacheLocal cachelocal = await _memoryCacheLocalService.GetCachedData(IdTraking.ToString());
            int statusCode = 200;
            try
            {
                TResponse response;
                try
                {
                    string protocol = wcfUrl.Protocol;

                    Binding binding2 = protocol switch
                    {
                        "https" => CrearBindingHttps(),
                        "http" => CrearBindingHttp(),
                        "netTcp" => CrearBindingNetTcp()
                    };

                    Binding binding = binding2;
                    binding.OpenTimeout = wcfUrl.TimeOut;
                    binding.CloseTimeout = wcfUrl.TimeOut;
                    binding.SendTimeout = wcfUrl.TimeOut;
                    binding.ReceiveTimeout = wcfUrl.TimeOut;
                    EndpointAddress endpoint = new EndpointAddress(wcfUrl.Url);
                    ChannelFactory<TService> channelFactory = new ChannelFactory<TService>(binding, endpoint);
                    if (Headers != null)
                    {
                        channelFactory.Endpoint.EndpointBehaviors.Add(new ClienteHeaderEndpointBehavior(Headers));
                    }

                    TService client = channelFactory.CreateChannel();
                    TResponse result = await operation(client, request);
                    await AddLogOutput(result, statusCode, cachelocal);
                    ((IClientChannel)(object)client).Close();
                    channelFactory.Close();
                    response = result;
                }
                catch (Exception ex2)
                {
                    Exception ex = ex2;
                    statusCode = 500;
                    Exception ex3 = ex2 as Exception;
                    if (ex3 == null)
                        throw ex2;

                    await AddLogError(request, statusCode, ex, cachelocal);
                    ExceptionDispatchInfo.Capture(ex3).Throw();
                    goto end_exception;
                }
                return response;

            end_exception:;
            }
            finally
            {
                await AddLogInput(request, statusCode, cachelocal);
            }
            TResponse result2 = default(TResponse);
            return result2;
        }

        public async Task<TResponse> ConnectAsync<TService, TRequest, TResponse>(TRequest request, IDictionary<string, string> Headers, Func<TService, TRequest, Task<TResponse>> operation, Guid IdTraking, Binding binding, EndpointAddress endpointAddress)
        {
            DataCacheLocal cachelocal = await _memoryCacheLocalService.GetCachedData(IdTraking.ToString());
            int statusCode = 200;
            try
            {
                TResponse response;
                try
                {
                    ChannelFactory<TService> channelFactory = new ChannelFactory<TService>(binding, endpointAddress);
                    if (Headers != null)
                    {
                        channelFactory.Endpoint.EndpointBehaviors.Add(new ClienteHeaderEndpointBehavior(Headers));
                    }

                    TService client = channelFactory.CreateChannel();
                    TResponse result = await operation(client, request);
                    await AddLogOutput(result, statusCode, cachelocal);
                    ((IClientChannel)(object)client).Close();
                    channelFactory.Close();
                    response = result;
                }
                catch (Exception ex2)
                {
                    Exception ex = ex2;
                    statusCode = 500;
                    Exception ex3 = ex2 as Exception;
                    if (ex3 == null)
                        throw ex2;
                    await AddLogError(request, statusCode, ex, cachelocal);
                    ExceptionDispatchInfo.Capture(ex3).Throw();
                    goto end_exception;
                }
                return response;

            end_exception:;
            }
            finally
            {
                await AddLogInput(request, statusCode, cachelocal);
            }
            TResponse result2 = default(TResponse);
            return result2;
        }

        private static Binding CrearBindingHttp()
        {
            BasicHttpBinding basicHttpBinding = new BasicHttpBinding(BasicHttpSecurityMode.TransportCredentialOnly)
            {
                MaxBufferPoolSize = 2147483647L,
                MaxBufferSize = int.MaxValue,
                TransferMode = TransferMode.Buffered,
                MaxReceivedMessageSize = 2147483647L,
                ReaderQuotas =
        {
          MaxDepth = int.MaxValue,
          MaxStringContentLength = int.MaxValue,
          MaxArrayLength = int.MaxValue,
          MaxBytesPerRead = int.MaxValue,
          MaxNameTableCharCount = int.MaxValue,
        },
            }; ;
            basicHttpBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Ntlm;
            return basicHttpBinding;
        }

        private static Binding CrearBindingHttps()
        {
            BasicHttpBinding basicHttpBinding = new BasicHttpBinding(BasicHttpSecurityMode.Transport)
            {
                MaxBufferPoolSize = 2147483647L,
                MaxBufferSize = int.MaxValue,
                TransferMode = TransferMode.Buffered,
                MaxReceivedMessageSize = 2147483647L,
                ReaderQuotas =
        {
          MaxDepth = int.MaxValue,
          MaxStringContentLength = int.MaxValue,
          MaxArrayLength = int.MaxValue,
          MaxBytesPerRead = int.MaxValue,
          MaxNameTableCharCount = int.MaxValue,
        },
            };
            basicHttpBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Ntlm;
            return basicHttpBinding;
        }
        private static Binding CrearBindingNetTcp()
        {
            return new NetTcpBinding()
            {
                MaxBufferPoolSize = 2147483647L,
                MaxBufferSize = int.MaxValue,
                TransferMode = TransferMode.Buffered,
                MaxReceivedMessageSize = 2147483647L,
                ReaderQuotas =
        {
          MaxDepth = int.MaxValue,
          MaxStringContentLength = int.MaxValue,
          MaxArrayLength = int.MaxValue,
          MaxBytesPerRead = int.MaxValue,
          MaxNameTableCharCount = int.MaxValue,
        },
                Security =
        {
          Mode = SecurityMode.Transport,
          Transport =
          {
            ClientCredentialType = TcpClientCredentialType.Windows
          }
        }
            };
        }

        private static async Task AddLogInput<TRequest>(TRequest? request, int statusCode, DataCacheLocal dataCache)
        {
            // Crear un objeto XmlSerializer para la clase TRequest
            XmlSerializer serializer = new XmlSerializer(typeof(TRequest));
            string xmlString = string.Empty;
            // Crear un StringWriter para escribir el XML en un string
            using (StringWriter writer = new StringWriter())
            {
                // Serializar el objeto a XML y escribirlo en el StringWriter
                serializer.Serialize(writer, request);

                // Obtener el XML como un string
                xmlString = writer.ToString();
            }
            dataCache.AddLogProcces(new DataCacheLocalProcess()
            {
                TypeProcess = "In",
                ProcessComponent = "Adapter",
                CreatedAt = DateTime.UtcNow,
                StatusCode = statusCode,
                DataMessage = xmlString,
            });
        }

        private static async Task AddLogError<TRequest>(TRequest? request, int statusCode, Exception ex, DataCacheLocal dataCache)
        {
            dataCache.AddLogProcces(new DataCacheLocalProcess()
            {
                TypeProcess = "Error",
                ProcessComponent = "Adapter",
                CreatedAt = DateTime.UtcNow,
                StatusCode = statusCode,
                DataMessage = $"{Newtonsoft.Json.JsonConvert.SerializeObject(ex, Newtonsoft.Json.Formatting.Indented)}",
            });
        }

        private static async Task AddLogOutput<TResponse>(TResponse? response, int statusCode, DataCacheLocal dataCache)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(TResponse));
            string xmlString = string.Empty;
            // Crear un StringWriter para escribir el XML en un string
            using (StringWriter writer = new StringWriter())
            {
                // Serializar el objeto a XML y escribirlo en el StringWriter
                serializer.Serialize(writer, response);

                // Obtener el XML como un string
                xmlString = writer.ToString();
            }
            dataCache.AddLogProcces(new DataCacheLocalProcess()
            {
                TypeProcess = "Out",
                ProcessComponent = "Adapter",
                CreatedAt = DateTime.UtcNow,
                StatusCode = statusCode,
                DataMessage = xmlString,
            });
        }
    }
}
