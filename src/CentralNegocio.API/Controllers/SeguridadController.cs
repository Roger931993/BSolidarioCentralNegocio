using CentralNegocio.API.Filters;
using CentralNegocio.Application.DTOs.Base;
using CentralNegocio.Application.Features.Commands;
using CentralNegocio.Application.Interfaces.Infraestructure;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace CentralNegocio.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SeguridadController : CommonController
    {
        public SeguridadController(IMediator mediator, IMemoryCacheLocalService memoryCacheLocalService, IRedisCache redisCache) : base(mediator, memoryCacheLocalService, redisCache)
        {
        }


        /// <summary>
        /// GenerarSession
        /// </summary>
        /// <remarks>
        /// Permiso: SeguridadController-GenerarSession
        /// <br/>
        /// Descripcion: GenerarSession
        /// </remarks>    
        [HttpPost("registrar")]
        [Permission("SeguridadController-GenerarSession")]
        [ProducesResponseType(typeof(SessionClienteResponse), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<SessionClienteResponse>> GenerarSession([FromBody] SessionClienteRequest data)
        {
            SessionClienteCommand command = new SessionClienteCommand()
            {
                Request = data
            };
            await CreateDataCacheLocal(HttpContext, command);
            ResponseBase<SessionClienteResponse> objResponse = await _mediator.Send(command);
            return OkUrban(objResponse);
        }
    }
}
