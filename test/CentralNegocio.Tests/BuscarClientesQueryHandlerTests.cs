using AutoMapper;
using CentralNegocio.Application.DTOs.Base;
using CentralNegocio.Application.DTOs.Clientes;
using CentralNegocio.Application.Features.Handlers;
using CentralNegocio.Application.Features.Queries;
using CentralNegocio.Application.Interfaces.Base;
using CentralNegocio.Application.Interfaces.Infraestructure;
using CentralNegocio.Domain.Common;
using FluentAssertions;
using Moq;

namespace CentralNegocio.Tests
{
    [TestFixture]
    public class BuscarClientesQueryHandlerTests
    {
        private Mock<IClientesService> _clientesServiceMock;
        private Mock<ICuentasService> _cuentasServiceMock; // aunque no se usa aquí
        private Mock<IMemoryCacheLocalService> _memoryCacheMock;
        private Mock<IMapper> _mapperMock;
        private Mock<IRedisCache> _redisCacheMock;
        private Mock<IErrorCatalogException> _errorCatalogMock;

        private BuscarClientesQueryHandler _handler;

        [SetUp]
        public void Setup()
        {
            _clientesServiceMock = new Mock<IClientesService>();
            _cuentasServiceMock = new Mock<ICuentasService>();
            _memoryCacheMock = new Mock<IMemoryCacheLocalService>();
            _mapperMock = new Mock<IMapper>();
            _redisCacheMock = new Mock<IRedisCache>();
            _errorCatalogMock = new Mock<IErrorCatalogException>();

            _handler = new BuscarClientesQueryHandler(
                _errorCatalogMock.Object,
                _redisCacheMock.Object,
                _memoryCacheMock.Object,
                _mapperMock.Object,
                _clientesServiceMock.Object,
                _cuentasServiceMock.Object
            );
        }

        [Test]
        public async Task Handle_ShouldReturnClientesList_WhenClientesExist()
        {
            // Arrange
            var idTracking = Guid.NewGuid();

            var query = new BuscarClientesQuery(
                new RequestBase<BuscarClientesRequest>
                {
                    Request = new BuscarClientesRequest(),
                    IdTraking = idTracking
                }
            );

            var clientesResponse = new ResponseBase<GetClienteResponse>
            {                
                data = new GetClienteResponse
                {
                    cliente = new List<clienteDto>
                    {
                        new clienteDto
                        {
                            cliente_id = 1,
                            primer_nombre = "Juan",
                            segundo_nombre = "Pedro",
                            apellido_paterno = "Perez",
                            apellido_materno = "Lopez",
                            username = "username1",
                            identificacion = "1234567899"
                        },
                        new clienteDto
                        {
                            cliente_id = 2,
                            primer_nombre = "Ana",
                            segundo_nombre = "Maria",
                            apellido_paterno = "Gomez",
                            apellido_materno = "Lopez",
                            username = "username2",
                            identificacion = "9876543210"
                        }
                    }
                },
                error = new Error { success = true }
            };

            _memoryCacheMock
                .Setup(x => x.GetCachedData(It.IsAny<string>()))
                .ReturnsAsync(new DataCacheLocal());

            _clientesServiceMock
                .Setup(x => x.Clientes(idTracking))
                .ReturnsAsync(clientesResponse);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.data.Should().NotBeNull();
            result.data.clientes.Should().HaveCount(2);

            var cliente1 = result.data.clientes[0];
            cliente1.primer_nombre.Should().Be("Juan");
            cliente1.apellido_paterno.Should().Be("Perez");

            var cliente2 = result.data.clientes[1];
            cliente2.primer_nombre.Should().Be("Ana");
            cliente2.apellido_paterno.Should().Be("Gomez");
        }
    }
}