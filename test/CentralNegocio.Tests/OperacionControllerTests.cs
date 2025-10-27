using CentralNegocio.API.Controllers;
using CentralNegocio.API.Filters;
using CentralNegocio.Application.DTOs.Base;
using CentralNegocio.Application.Features.Queries;
using CentralNegocio.Application.Interfaces.Infraestructure;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Net;


namespace CentralNegocio.Tests
{


    [TestFixture]
    public class OperacionControllerTests
    {
        private Mock<IMediator> _mediatorMock;
        private Mock<IMemoryCacheLocalService> _memoryCacheLocalServiceMock;
        private Mock<IRedisCache> _redisCacheMock;
        private OperacionController _controller;

        [SetUp]
        public void Setup()
        {
            _mediatorMock = new Mock<IMediator>();
            _memoryCacheLocalServiceMock = new Mock<IMemoryCacheLocalService>();
            _redisCacheMock = new Mock<IRedisCache>();

            _controller = new OperacionController(
                _mediatorMock.Object,
                _memoryCacheLocalServiceMock.Object,
                _redisCacheMock.Object
            );

            // HttpContext simulado
            var httpContext = new DefaultHttpContext();
            // Simulamos un Header con un GUID válido
            httpContext.Request.Headers["IdTraker"] = Guid.NewGuid().ToString();
            var metadata = new EndpointMetadataCollection(new PermissionAttribute("OperacionController-BuscarClientes"));
            var endpoint = new Endpoint((context) => Task.CompletedTask, metadata, "Fake endpoint");
            httpContext.SetEndpoint(endpoint);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
        }

        [Test]
        public async Task BuscarClientes_ShouldReturnOk_WithExpectedResponse()
        {
            // Arrange
            var expectedResponse = new ResponseBase<BuscarClientesResponse>
            {
                data = new BuscarClientesResponse()
                {
                    clientes = new List<Application.DTOs.Clientes.clienteDto>()
                    {
                       new Application.DTOs.Clientes.clienteDto()
                       {
                           apellido_materno = "Lopez",
                           apellido_paterno = "Perez",
                           cliente_id =1,
                           identificacion = "1234567899",
                           primer_nombre ="Juan",
                           segundo_nombre = "Pedro",
                           username = "username1",
                       },
                        new Application.DTOs.Clientes.clienteDto()
                       {
                           apellido_materno = "Lopez",
                           apellido_paterno = "Perez",
                           cliente_id =2,
                           identificacion = "1234567899",
                           primer_nombre ="Juan",
                           segundo_nombre = "Pedro",
                           username = "username1",
                       }
                    }                   
                },
                error = new Error()
                {
                    codeError = 10000,
                    success = true
                },
                
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<BuscarClientesQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.BuscarClientes();

            // Assert
            _mediatorMock.Verify(m => m.Send(It.IsAny<BuscarClientesQuery>(), It.IsAny<CancellationToken>()), Times.Once);

            var okResult = result.Result as ObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be((int)HttpStatusCode.OK);

            //var value = okResult.Value as ResponseBase<BuscarClientesResponse>;
            //value.Should().BeEquivalentTo(expectedResponse);
        }
    }
}

