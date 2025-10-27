using AutoMapper;
using CentralNegocio.Application.DTOs.Base;
using CentralNegocio.Application.DTOs.Clientes;
using CentralNegocio.Application.DTOs.Cuentas;
using CentralNegocio.Application.Features.Handlers;
using CentralNegocio.Application.Features.Queries;
using CentralNegocio.Application.Interfaces.Base;
using CentralNegocio.Application.Interfaces.Infraestructure;
using CentralNegocio.Domain.Common;
using FluentAssertions;
using Moq;

namespace CentralNegocio.Tests
{
    public class BuscarClienteQueryHandlerTest
    {

        private Mock<IClientesService> _clientesServiceMock;
        private Mock<ICuentasService> _cuentasServiceMock;
        private Mock<IMemoryCacheLocalService> _memoryCacheMock;
        private Mock<IMapper> _mapperMock;
        private Mock<IRedisCache> _redisCacheMock;
        private Mock<IErrorCatalogException> _errorCatalogMock;
        private BuscarClienteQueryHandler _handler;

        [SetUp]
        public void Setup()
        {
            _clientesServiceMock = new Mock<IClientesService>();
            _cuentasServiceMock = new Mock<ICuentasService>();
            _memoryCacheMock = new Mock<IMemoryCacheLocalService>();
            _mapperMock = new Mock<IMapper>();
            _redisCacheMock = new Mock<IRedisCache>();
            _errorCatalogMock = new Mock<IErrorCatalogException>();

            _handler = new BuscarClienteQueryHandler(
                _errorCatalogMock.Object,
                _redisCacheMock.Object,
                _memoryCacheMock.Object,
                _mapperMock.Object,
                _clientesServiceMock.Object,
                _cuentasServiceMock.Object
            );
        }

        [Test]
        public async Task Handle_ShouldReturnClienteAndCuenta_WhenClienteExists()
        {
            // Arrange
            var idTracking = Guid.NewGuid();
            var clienteId = 1;

            var buscarClienteQuery = new BuscarClienteQuery(
                new RequestBase<BuscarClienteRequest>
                {
                    Request = new BuscarClienteRequest { cliente_id = clienteId },
                    IdTraking = idTracking
                }
            );

            var clienteResponse = new ResponseBase<GetClienteResponse>
            {                
                data = new GetClienteResponse
                {                   
                    cliente = new List<clienteDto>
                    {
                        new clienteDto
                        {
                            cliente_id = clienteId,
                            primer_nombre = "Juan",
                            segundo_nombre = "Pedro",
                            apellido_paterno = "Perez",
                            apellido_materno = "Lopez",
                            username = "username1",
                            identificacion = "1234567899"
                        }
                    },
                    
                },
                error = new Error { success = true }
            };

            var cuentasResponse = new ResponseBase<GetCuentaResponse>
            {                
                data = new GetCuentaResponse
                {
                    cuenta = new List<cuentaDto>
                    {
                        new cuentaDto { cuenta_id = 1, cliente_id = clienteId, tipo_cuenta = "AHO", producto_id = 1 },                        
                    }
                },
                error = new Error { success = true }
            };

            _memoryCacheMock
                .Setup(x => x.GetCachedData(It.IsAny<string>()))
                .ReturnsAsync(new DataCacheLocal());

            _clientesServiceMock
                .Setup(x => x.ClientePorId(clienteId, idTracking))
                .ReturnsAsync(clienteResponse);

            _cuentasServiceMock
                .Setup(x => x.CuentaPorClienteId(clienteId, idTracking))
                .ReturnsAsync(cuentasResponse);

            // Act
            var result = await _handler.Handle(buscarClienteQuery, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.errorCatalogException.Should().BeNull();
            result.data!.cliente.Should().NotBeNull();
            result.data.cuentas.Should().HaveCount(1);

            var cliente = result.data.cliente;
            cliente.primer_nombre.Should().Be("Juan");
            cliente.apellido_paterno.Should().Be("Perez");

            var cuenta = result.data.cuentas.First();
            cuenta.cuenta_id.Should().Be(1);
            cuenta.cliente_id.Should().Be(clienteId);
        }
    }
}
