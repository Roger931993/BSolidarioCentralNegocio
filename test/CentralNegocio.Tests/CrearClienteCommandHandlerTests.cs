using AutoMapper;
using CentralNegocio.Application.DTOs.Base;
using CentralNegocio.Application.DTOs.Clientes;
using CentralNegocio.Application.Features.Commands;
using CentralNegocio.Application.Features.Handlers;
using CentralNegocio.Application.Interfaces.Base;
using CentralNegocio.Application.Interfaces.Infraestructure;
using CentralNegocio.Domain.Common;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralNegocio.Tests
{
    [TestFixture]
    public class CrearClienteCommandHandlerTests
    {
        private Mock<IClientesService> _clientesServiceMock;
        private Mock<IMemoryCacheLocalService> _memoryCacheMock;
        private Mock<IMapper> _mapperMock;
        private Mock<IRedisCache> _redisCacheMock;
        private Mock<IErrorCatalogException> _errorCatalogMock;

        private CrearClienteCommandHandler _handler;

        [SetUp]
        public void Setup()
        {
            _clientesServiceMock = new Mock<IClientesService>();
            _memoryCacheMock = new Mock<IMemoryCacheLocalService>();
            _mapperMock = new Mock<IMapper>();
            _redisCacheMock = new Mock<IRedisCache>();
            _errorCatalogMock = new Mock<IErrorCatalogException>();

            _handler = new CrearClienteCommandHandler(
                _errorCatalogMock.Object,
                _redisCacheMock.Object,
                _memoryCacheMock.Object,
                _mapperMock.Object,
                _clientesServiceMock.Object
            );
        }

        [Test]
        public async Task Handle_ShouldRegisterNewCliente_WhenClienteDoesNotExist()
        {
            // Arrange
            var trackingId = Guid.NewGuid();
            var command = new CrearClienteCommand
            {
                IdTraking = trackingId,
                Request = new CrearClienteRequest
                {
                    primer_nombre = "Juan",
                    segundo_nombre = "Pedro",
                    apellido_paterno = "Perez",
                    apellido_materno = "Lopez",
                    identificacion = "1234567899",
                    user_name = "juanperez"
                }
            };

            var getClienteResponse = new ResponseBase<GetClienteResponse>
            {                
                data = new GetClienteResponse { cliente = new List<clienteDto>() },
                error = new Error { success = true }
            };

            var registerResponse = new ResponseBase<RegisterClienteResponse>
            {                
                data = new RegisterClienteResponse
                {
                    cliente = new clienteDto
                    {
                        primer_nombre = "Juan",
                        segundo_nombre = "Pedro",
                        apellido_paterno = "Perez",
                        apellido_materno = "Lopez",
                        identificacion = "1234567899",
                        username = "juanperez",
                        cliente_id = 1
                    }
                },
                error = new Error { success = true }
            };

            _memoryCacheMock
                .Setup(x => x.GetCachedData(It.IsAny<string>()))
                .ReturnsAsync(new DataCacheLocal());

            _clientesServiceMock
                .Setup(x => x.ClientePorCedula(command.Request.identificacion, trackingId))
                .ReturnsAsync(getClienteResponse);

            _clientesServiceMock
                .Setup(x => x.Register(It.IsAny<RegisterClienteRequest>(), trackingId))
                .ReturnsAsync(registerResponse);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.data.Should().NotBeNull();
            result.data.cliente.Should().NotBeNull();
            result.data.cliente.cliente_id.Should().Be(1);
            result.data.cliente.primer_nombre.Should().Be("Juan");
        }

        [Test]
        public async Task Handle_ShouldRegisterExistingCliente_WhenClienteAlreadyExists()
        {
            // Arrange
            var trackingId = Guid.NewGuid();
            var existingCliente = new clienteDto
            {
                cliente_id = 10,
                primer_nombre = "Luis",
                segundo_nombre = "Carlos",
                apellido_paterno = "Gomez",
                apellido_materno = "Diaz",
                identificacion = "9876543210",
                username = "luisg"
            };

            var command = new CrearClienteCommand
            {
                IdTraking = trackingId,
                Request = new CrearClienteRequest
                {
                    primer_nombre = "Luis",
                    segundo_nombre = "Carlos",
                    apellido_paterno = "Gomez",
                    apellido_materno = "Diaz",
                    identificacion = "9876543210",
                    user_name = "luisg"
                }
            };

            var getClienteResponse = new ResponseBase<GetClienteResponse>
            {
                data = new GetClienteResponse { cliente = new List<clienteDto>(){

                } },
                error = new Error { success = true }
            };

            var registerResponse = new ResponseBase<RegisterClienteResponse>
            {                
                data = new RegisterClienteResponse { cliente = existingCliente },
                error = new Error { success = true }
            };

            _memoryCacheMock
                .Setup(x => x.GetCachedData(It.IsAny<string>()))
                .ReturnsAsync(new DataCacheLocal());

            _clientesServiceMock
                .Setup(x => x.ClientePorCedula(command.Request.identificacion, trackingId))
                .ReturnsAsync(getClienteResponse);

            _clientesServiceMock
                .Setup(x => x.Register(It.IsAny<RegisterClienteRequest>(), trackingId))
                .ReturnsAsync(registerResponse);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.data.Should().NotBeNull();
            result.data.cliente.cliente_id.Should().Be(existingCliente.cliente_id);
            result.data.cliente.primer_nombre.Should().Be("Luis");
        }
    }
}
