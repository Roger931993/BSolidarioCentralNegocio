using CentralNegocio.API.Filters;
using CentralNegocio.Application.DTOs.Base;
using CentralNegocio.Application.Features.Commands;
using CentralNegocio.Application.Features.Queries;
using CentralNegocio.Application.Interfaces.Infraestructure;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace CentralNegocio.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OperacionController : CommonController
    {
        public OperacionController(IMediator mediator, IMemoryCacheLocalService memoryCacheLocalService, IRedisCache redisCache) : base(mediator, memoryCacheLocalService, redisCache)
        {
        }



        /// <summary>
        /// Obtener todos los clientes
        /// </summary>
        /// <remarks>
        /// Permiso: OperacionController-BuscarClientes
        /// <br/>
        /// Descripcion: Obtener todos los clientes
        /// </remarks>    
        [HttpGet("clientes")]
        [Permission("OperacionController-BuscarClientes")]
        [ProducesResponseType(typeof(BuscarClientesResponse), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<BuscarClientesResponse>> BuscarClientes()
        {
            RequestBase<BuscarClientesRequest> request = new RequestBase<BuscarClientesRequest>()
            {
                Request = new BuscarClientesRequest()
                {                    
                }
            };
            await CreateDataCacheLocal(HttpContext, request);
            ResponseBase<BuscarClientesResponse> objResponse = await _mediator.Send(new BuscarClientesQuery(request));
            return OkUrban(objResponse);
        }

        /// <summary>
        /// Obtener cliente
        /// </summary>
        /// <remarks>
        /// Permiso: OperacionController-BuscarCliente
        /// <br/>
        /// Descripcion: Obtener cliente
        /// </remarks>    
        [HttpGet("cliente/{id}")]
        [Permission("OperacionController-BuscarCliente")]
        [ProducesResponseType(typeof(BuscarClienteResponse), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<BuscarClienteResponse>> BuscarCliente(string id)
        {
            RequestBase<BuscarClienteRequest> request = new RequestBase<BuscarClienteRequest>()
            {
                Request = new BuscarClienteRequest()
                {
                    identificacion = id
                }
            };
            await CreateDataCacheLocal(HttpContext, request);
            ResponseBase<BuscarClienteResponse> objResponse = await _mediator.Send(new BuscarClienteQuery(request));
            return OkUrban(objResponse);
        }

        /// <summary>
        /// Obtener cuenta
        /// </summary>
        /// <remarks>
        /// Permiso: OperacionController-BuscarCliente
        /// <br/>
        /// Descripcion: Obtener cuenta
        /// </remarks>    
        [HttpGet("cuenta/{id}")]
        [Permission("OperacionController-BuscarCliente")]
        [ProducesResponseType(typeof(BuscarCuentaResponse), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<BuscarCuentaResponse>> BuscarCuenta(int id)
        {
            RequestBase<BuscarCuentaRequest> request = new RequestBase<BuscarCuentaRequest>()
            {
                Request = new BuscarCuentaRequest()
                {
                    cuenta_id = id
                }
            };
            await CreateDataCacheLocal(HttpContext, request);
            ResponseBase<BuscarCuentaResponse> objResponse = await _mediator.Send(new BuscarCuentaQuery(request));
            return OkUrban(objResponse);
        }


        /// <summary>
        /// Obtener proyeccion cliente y cuenta
        /// </summary>
        /// <remarks>
        /// Permiso: OperacionController-ProyeccionAhorroPlan
        /// <br/>
        /// Descripcion: Obtener proyeccion cliente y cuenta
        /// </remarks>    
        [HttpGet("cliente/{id}/cuenta/{id2}")]
        [Permission("OperacionController-ProyeccionAhorroPlan")]
        [ProducesResponseType(typeof(ProyeccionAhorroResponse), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ProyeccionAhorroResponse>> ProyeccionAhorroPlan(int id, int id2)
        {
            RequestBase<ProyeccionAhorroRequest> request = new RequestBase<ProyeccionAhorroRequest>()
            {
                Request = new ProyeccionAhorroRequest()
                {
                    cliente_id = id,
                    cuenta_id = id2
                    
                }
            };
            await CreateDataCacheLocal(HttpContext, request);
            ResponseBase<ProyeccionAhorroResponse> objResponse = await _mediator.Send(new ProyeccionAhorroQuery(request));
            return OkUrban(objResponse);
        }


        /// <summary>
        /// Registrar cliente
        /// </summary>
        /// <remarks>
        /// Permiso: OperacionController-RegistrarCliente
        /// <br/>
        /// Descripcion: Registrar cliente
        /// </remarks>    
        [HttpPost("registrar-cliente")]        
        [Permission("OperacionController-RegistrarCliente")]
        [ProducesResponseType(typeof(CrearClienteResponse), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CrearClienteResponse>> RegistrarCliente([FromBody] CrearClienteRequest data)
        {
            CrearClienteCommand command = new CrearClienteCommand()
            {
                Request = data
            };
            await CreateDataCacheLocal(HttpContext, command);
            ResponseBase<CrearClienteResponse> objResponse = await _mediator.Send(command);
            return OkUrban(objResponse);
        }



        /// <summary>
        /// Registrar cuenta
        /// </summary>
        /// <remarks>
        /// Permiso: OperacionController-CrearCuenta
        /// <br/>
        /// Descripcion: Registrar cuenta
        /// </remarks>            
        [HttpPost("crear-cuenta")]
        [Permission("OperacionController-CrearCuenta")]
        [ProducesResponseType(typeof(CrearCuentaResponse), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CrearCuentaResponse>> CrearCuenta([FromBody] CrearCuentaRequest data)
        {
            CrearCuentaCommand command = new CrearCuentaCommand()
            {
                Request = data
            };
            await CreateDataCacheLocal(HttpContext, command);
            ResponseBase<CrearCuentaResponse> objResponse = await _mediator.Send(command);
            return OkUrban(objResponse);
        }

        /// <summary>
        /// Registrar cuenta ahorro plan
        /// </summary>
        /// <remarks>
        /// Permiso: OperacionController-CrearCuentaAhorroPlan
        /// <br/>
        /// Descripcion: Registrar cuenta ahorro plan
        /// </remarks>            
        [HttpPost("crear-cuenta-ahorro-plan")]
        [Permission("OperacionController-CrearCuentaAhorroPlan")]
        [ProducesResponseType(typeof(CrearCuentaAhorroPlanResponse), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CrearCuentaAhorroPlanResponse>> CrearCuentaAhorroPlan([FromBody] CrearCuentaAhorroPlanRequest data)
        {
            CrearCuentaAhorroPlanCommand command = new CrearCuentaAhorroPlanCommand()
            {
                Request = data
            };
            await CreateDataCacheLocal(HttpContext, command);
            ResponseBase<CrearCuentaAhorroPlanResponse> objResponse = await _mediator.Send(command);
            return OkUrban(objResponse);
        }
    }
}
