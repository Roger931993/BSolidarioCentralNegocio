using AutoMapper;
using CentralNegocio.Application.DTOs.Base;
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
    [TestFixture]
    public class BuscarCuentaQueryHandlerTests
    {
        private Mock<ICuentasService> _cuentasServiceMock;
        private Mock<IMemoryCacheLocalService> _memoryCacheMock;
        private Mock<IMapper> _mapperMock;
        private Mock<IRedisCache> _redisCacheMock;
        private Mock<IErrorCatalogException> _errorCatalogMock;

        private BuscarCuentaQueryHandler _handler;

        [SetUp]
        public void Setup()
        {
            _cuentasServiceMock = new Mock<ICuentasService>();
            _memoryCacheMock = new Mock<IMemoryCacheLocalService>();
            _mapperMock = new Mock<IMapper>();
            _redisCacheMock = new Mock<IRedisCache>();
            _errorCatalogMock = new Mock<IErrorCatalogException>();

            _handler = new BuscarCuentaQueryHandler(
                _errorCatalogMock.Object,
                _redisCacheMock.Object,
                _memoryCacheMock.Object,
                _mapperMock.Object,
                _cuentasServiceMock.Object
            );
        }

        [Test]
        public async Task Handle_ShouldReturnCuenta_WhenCuentaExists()
        {
            // Arrange
            var idTracking = Guid.NewGuid();
            var cuentaId = 101;

            var query = new BuscarCuentaQuery(
                new RequestBase<BuscarCuentaRequest>
                {
                    Request = new BuscarCuentaRequest { cuenta_id = cuentaId },
                    IdTraking = idTracking
                }
            );

            var cuentasResponse = new ResponseBase<GetCuentaResponse>
            {                
                data = new GetCuentaResponse
                {
                    cuenta = new System.Collections.Generic.List<cuentaDto>
                    {
                        new cuentaDto
                        {
                            cuenta_id = cuentaId,
                            cliente_id = 1,
                            tipo_cuenta = "AHO"
                        }
                    }
                },
                error = new Error { success = true }
            };

            _memoryCacheMock
                .Setup(x => x.GetCachedData(It.IsAny<string>()))
                .ReturnsAsync(new DataCacheLocal());

            _cuentasServiceMock
                .Setup(x => x.CuentaPorId(cuentaId, idTracking))
                .ReturnsAsync(cuentasResponse);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.data.Should().NotBeNull();
            result.data.cuentas.Should().NotBeNull();
            result.data.cuentas.cuenta_id.Should().Be(cuentaId);
            result.data.cuentas.cliente_id.Should().Be(1);
            result.data.cuentas.tipo_cuenta.Should().Be("AHO");
        }
    }
}