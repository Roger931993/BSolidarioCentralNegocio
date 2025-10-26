using CentralNegocio.Application.Common;
using CentralNegocio.Application.Interfaces.Infraestructure;
using CentralNegocio.Domain.Common;
using CentralNegocio.Domain.Models;
using FluentValidation.Results;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Text;

namespace CentralNegocio.API.Middlewares
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LoggingMiddleware> _logger;
        private readonly IMemoryCacheLocalService _memoryCacheLocalService;
        private readonly IServiceScopeFactory _scopeFactory;

        //private readonly IDbContextFactory<LoggDbContext> _dbContext;
        private readonly IConfiguration _configuration;
        private readonly AesEncryptionService _aesService;
        private readonly string _key;

        public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger, IMemoryCacheLocalService memoryCacheLocalService, IServiceScopeFactory scopeFactory, IConfiguration configuration)
        {
            _next = next;
            _logger = logger;
            _memoryCacheLocalService = memoryCacheLocalService;
            this._scopeFactory = scopeFactory;
            //_dbContext = dbContext;
            this._configuration = configuration;
            _key = _configuration.GetSection("Settings")!.GetSection("KeyAES256")!.Value!; // Clave de 32 bytes (256 bits)
            _aesService = new AesEncryptionService(_key);
        }

        public async Task Invoke(HttpContext context)
        {
            context.Request.EnableBuffering(); // Habilita la capacidad de leer el stream varias veces

            IHeaderDictionary headers = context.Request.Headers;
            #region Validar Headers 
            if (!headers.Any(x => x.Key == "IdTraker"))
                context.Request.Headers.Add("IdTraker", Guid.NewGuid().ToString());
            #endregion
            string requestBodyText = string.Empty;

            #region Decryption
            if (!string.IsNullOrEmpty(_key))
            {
                // Desencriptar los parámetros de la URL
                DecryptPathParameters(context);

                // Descifrar los parámetros de la URL
                DecryptQueryString(context);

                // Desencriptar el cuerpo de la solicitud
                if (context.Request.HasFormContentType)
                    requestBodyText = await ReadFromDataAsync(context.Request);
                else
                {
                    await DecryptRequestBody(context);

                    requestBodyText = string.Empty;
                    var requestBodyStream = new MemoryStream();
                    await context.Request.Body.CopyToAsync(requestBodyStream);
                    requestBodyStream.Seek(0, SeekOrigin.Begin);
                    requestBodyText = await new StreamReader(requestBodyStream).ReadToEndAsync();
                    context.Request.Body.Position = 0L;
                }
            }
            else
            {
                if (context.Request.HasFormContentType)
                    requestBodyText = await ReadFromDataAsync(context.Request);
                else
                {
                    requestBodyText = string.Empty;
                    var requestBodyStream = new MemoryStream();
                    await context.Request.Body.CopyToAsync(requestBodyStream);
                    requestBodyStream.Seek(0, SeekOrigin.Begin);
                    requestBodyText = await new StreamReader(requestBodyStream).ReadToEndAsync();
                    context.Request.Body.Position = 0L;
                }
            }
            #endregion

            Stream originalResponseBody = context.Response.Body;
            Stream origianlRequestBody = context.Request.Body;

            try
            {
                #region Captura la respuesta de la solicitud        
                using MemoryStream resposeBodyStream = new MemoryStream();
                using MemoryStream resposeBodyEncryptStream = new MemoryStream();
                context.Response.Body = resposeBodyStream;
                await _next(context);
                resposeBodyStream.Seek(0L, SeekOrigin.Begin);
                string responseBodyText = await new StreamReader(resposeBodyStream).ReadToEndAsync();
                resposeBodyStream.Seek(0L, SeekOrigin.Begin);
                #endregion

                #region Guardar el log en la base de datos utilizando EF
                await Task.WhenAll(SaveLog(context, requestBodyText, responseBodyText, context.Response.StatusCode), EncryptBodyResponse(context, originalResponseBody, responseBodyText, resposeBodyStream));
                #endregion
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en tareas concurrentes en middleware");
                context.Response.Body = originalResponseBody;
                await HandleExceptionAsync(context, ex, requestBodyText);
            }
            finally
            {
                // Eliminar cache local por IdTraking
                string headerIdTraking = new string(context.Request.Headers["IdTraker"].ToString());
                if (!string.IsNullOrEmpty(headerIdTraking))
                    await _memoryCacheLocalService.DeleteCacheData(headerIdTraking);
                // Restaurar el cuerpo original para que la respuesta pueda enviarse al cliente        
                context.Request.Body = origianlRequestBody;
            }
        }

        private async Task EncryptBodyResponse(HttpContext context, Stream originalResponseBody, string responseBodyText, MemoryStream resposeBodyStream)
        {
            if (string.IsNullOrEmpty(_key))
            {
                await resposeBodyStream.CopyToAsync(originalResponseBody);
            }
            else if (context.Response.StatusCode != 200)
            {
                await resposeBodyStream.CopyToAsync(originalResponseBody);
            }
            else if (!context.Response!.ContentType!.Contains("application/json"))
            {
                await resposeBodyStream.CopyToAsync(originalResponseBody);
            }
            else
            {
                string encryptedResponse = _aesService.Encrypt(responseBodyText);
                var jsonResponse = System.Text.Json.JsonSerializer.Serialize(new { payload = encryptedResponse });
                // Escribir la respuesta cifrada en el body original
                byte[] encryptedBytes = Encoding.UTF8.GetBytes(jsonResponse);
                context.Response.Body = originalResponseBody;
                await context.Response.Body.WriteAsync(encryptedBytes, 0, encryptedBytes.Length);
            }
        }

        private async Task<string> ReadFromDataAsync(HttpRequest request)
        {
            IFormCollection form = await request.ReadFormAsync();
            Dictionary<string, string> formData = new Dictionary<string, string>();
            foreach (string key in form.Keys)
            {
                formData[key] = (string)form[key];
            }
            return System.Text.Json.JsonSerializer.Serialize(formData);
        }

        public async Task SaveLog(HttpContext context, string? RequestBody, string? ResponseBody, int statusCode)
        {
            if (context.Request.Method == "OPTIONS")//Se excluye registro de logs de petion OPTIONS
                return;

            //string IdTraking = new string(context.Request.Headers["IdTraker"].ToString());
            if (!context.Request.Headers.TryGetValue("IdTraker", out var idTrackingHeader))
                return;

            string IdTraking = idTrackingHeader.ToString();

            try
            {
                //DataCacheLocal cachelocal = await _memoryCacheLocalService.GetCachedData(IdTraking);
                //// Armar Log de la solicitud
                //LoggingMdl loggingDto = SetLog(context, RequestBody, ResponseBody, statusCode, cachelocal, IdTraking);

                ////_ = Task.Run(async () =>
                ////{
                //using var scope = _scopeFactory.CreateScope();
                //var loggService = scope.ServiceProvider.GetRequiredService<ILoggRepository>();

                ////await loggService.ExecuteSaveLogg(loggingDto);
                //api_log_cliente_header objHeader = await loggService.SaveHeader(loggingDto);
                //List<api_log_cliente_detail> objDetails = new List<api_log_cliente_detail>();
                //foreach (ApiLogsDetails item in loggingDto.Details)
                //{
                //    objDetails.Add(new api_log_cliente_detail()
                //    {
                //        api_log_cliente_header_id = objHeader.api_log_cliente_header_id,
                //        created_at = item.CreatedAt,
                //        data_message = item.DataMessage,
                //        status_code = item.StatusCode,
                //        type_process = item.TypeProcess,
                //        process_component = item.ProcessComponent
                //    });
                //}
                //await loggService.SaveDetails(objDetails);

                ////});              
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Error al guardar Logs");
            }
        }

        public LoggingMdl SetLog(HttpContext context, string? RequestBody, string? ResponseBody, int statusCode, DataCacheLocal cachelocal, string IdTraking)
        {
            LoggingMdl loggingDto = new LoggingMdl();
            loggingDto.Header = new()
            {
                CreatedAt = cachelocal != null ? cachelocal.CreatedAt : DateTime.UtcNow,
                IdTracking = Guid.Parse(IdTraking)!,
                RequestMethod = context.Request.Method,
                RequestUrl = context.Request.Path,
                ResponseCode = context.Response.StatusCode
            };
            loggingDto.Details = new List<ApiLogsDetails>();
            if (cachelocal != null)
            {
                foreach (DataCacheLocalProcess item in cachelocal!.LogProcess)
                {
                    loggingDto.Details.Add(new ApiLogsDetails()
                    {
                        CreatedAt = item.CreatedAt,
                        DataMessage = item.DataMessage,
                        StatusCode = item.StatusCode,
                        ProcessComponent = item.ProcessComponent,
                        TypeProcess = item.TypeProcess,
                    });
                }
            }
            loggingDto.Details.Add(new ApiLogsDetails()
            {
                CreatedAt = cachelocal != null ? cachelocal.CreatedAt : DateTime.UtcNow,
                StatusCode = statusCode,
                DataMessage = (string)RequestBody!,
                ProcessComponent = "Api",
                TypeProcess = "Request"
            });
            loggingDto.Details.Add(new ApiLogsDetails()
            {
                CreatedAt = DateTime.UtcNow,
                StatusCode = statusCode,
                DataMessage = (string)ResponseBody!,
                ProcessComponent = "Api",
                TypeProcess = "Response"
            });
            return loggingDto;
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex, string? BodyRequest)
        {
            JsonSerializerSettings jsonSettings = new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new DefaultNamingStrategy()
                }
            };
            context.Response.ContentType = "application/json";
            int statusCode = 500;
            string result = string.Empty;
            ValidationErrorException validation = ex as ValidationErrorException;
            if (validation == null)
            {
                ErrorCatalogException catalogException = ex as ErrorCatalogException;
                if (catalogException != null)
                {
                    statusCode = 500;
                    object objResult = new
                    {
                        CodeError = catalogException.CodeError,
                        MessageError = catalogException.MessageError,
                        ExceptionMessage = ex.Message
                    };
                    result = JsonConvert.SerializeObject(objResult, jsonSettings);
                }
            }
            else
            {
                statusCode = 500;
                string IdTraking = new string(context.Request.Headers["IdTraker"].ToString());
                result = JsonConvert.SerializeObject(new HttpValidationProblemDetails(validation.Errors)
                {
                    Status = statusCode,
                    Title = "Error Validation",
                    Type = "Validation",
                    Detail = "IdTraker" + IdTraking
                }, jsonSettings);
            }
            if (string.IsNullOrEmpty(result))
            {
                string strResult = statusCode switch
                {
                    400 => "Request have errors",
                    401 => "No Authorization",
                    404 => "No find",
                    500 => "Server Error"
                };

                object objResult = new
                {
                    status = statusCode == 400 ? 500 : statusCode,
                    MessageError = strResult,
                    ExceptionMessage = ex.Message
                };
                result = JsonConvert.SerializeObject(objResult, jsonSettings);
            }
            context.Response.StatusCode = statusCode;
            await Task.WhenAll(SaveLog(context, BodyRequest, result, context.Response.StatusCode), context.Response.WriteAsync(result));
        }

        #region Decryption
        private void DecryptQueryString(HttpContext context)
        {
            var queryString = context.Request.Query;
            var decryptedParameters = new Dictionary<string, string>();
            foreach (var param in queryString)
            {
                if (!string.IsNullOrEmpty(param.Value) && IsBase64String(param.Value!))
                {
                    var decryptedValue = _aesService.DecryptDataBody(param.Value!); // Descifrar el valor
                    decryptedParameters[param.Key] = decryptedValue;
                }
                else
                    decryptedParameters[param.Key] = param.Value!;
            }
            // Crear una nueva QueryString a partir de los parámetros descifrados
            var decryptedQueryString = QueryString.Create(decryptedParameters);

            // Reasignar la cadena de consulta modificada
            context.Request.QueryString = decryptedQueryString;
        }

        private void DecryptPathParameters(HttpContext context)
        {
            foreach (var item in context.Request.RouteValues.Where(x => x.Key.Contains("id")))
            {
                string encryptedId = Uri.UnescapeDataString(item.Value?.ToString()!);
                if (!string.IsNullOrEmpty(encryptedId) && IsBase64Url(encryptedId))
                {
                    var decryptedId = _aesService.DecryptDataUrl(encryptedId);
                    context.Request.RouteValues[item.Key] = decryptedId;
                }
            }
        }

        private async Task DecryptRequestBody(HttpContext context)
        {
            if (context.Request.Body.CanSeek)
            {
                context.Request.Body.Position = 0;
            }

            using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, true, 1024, true);
            string bodyContent = await reader.ReadToEndAsync();
            if (!string.IsNullOrEmpty(bodyContent) && IsBase64String(bodyContent))
            {
                string decryptedBody = _aesService.DecryptDataBody(bodyContent);
                byte[] bytes = Encoding.UTF8.GetBytes(decryptedBody);
                context.Request.Body = new MemoryStream(bytes);
                context.Request.Body.Position = 0;
            }
        }

        public bool IsBase64String(string base64String)
        {
            if (string.IsNullOrEmpty(base64String))
                return false;

            Span<byte> buffer = new Span<byte>(new byte[base64String.Length]);
            return Convert.TryFromBase64String(base64String, buffer, out _);
        }
        public bool IsBase64Url(string base64Url)
        {
            // Convertir Base64URL a Base64
            string base64 = base64Url
                .Replace('-', '+')
                .Replace('_', '/');

            // Agregar `=` si falta relleno
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }

            // Intentar validar con Base64 estándar
            return Convert.TryFromBase64String(base64, new byte[base64.Length], out _);
        }
        #endregion
    }
    public class ValidationErrorException : ApplicationException
    {
        public IDictionary<string, string[]> Errors { get; }
        public ValidationErrorException() : base("Errors Validation")
        {
            Errors = new Dictionary<string, string[]>();
        }
        public ValidationErrorException(IEnumerable<ValidationFailure> failures) : this()
        {
            Errors = (from e in failures group e.ErrorMessage by e.PropertyName).ToDictionary((IGrouping<string, string> failureGroup) => failureGroup.Key, (IGrouping<string, string> failureGroup) => failureGroup.ToArray());
        }



    }


}
