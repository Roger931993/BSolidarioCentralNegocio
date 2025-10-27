using AutoMapper;
using CentralNegocio.Application.DTOs.Base;
using CentralNegocio.Application.DTOs.Clientes;
using CentralNegocio.Application.DTOs.Cuentas;
using CentralNegocio.Application.Features.Commands;
using CentralNegocio.Application.Features.Handlers;
using CentralNegocio.Application.Interfaces.Base;
using CentralNegocio.Application.Interfaces.Infraestructure;
using CentralNegocio.Domain.Common;
using FluentAssertions;
using Moq;

namespace CentralNegocio.Tests
{
    [TestFixture]
    public class CrearCuentaAhorroPlanCommandHandlerTests
    {
        private Mock<IClientesService> _clientesServiceMock;
        private Mock<ICuentasService> _cuentasServiceMock;
        private Mock<IMemoryCacheLocalService> _memoryCacheMock;
        private Mock<IMapper> _mapperMock;
        private Mock<IRedisCache> _redisCacheMock;
        private Mock<IErrorCatalogException> _errorCatalogMock;

        private CrearCuentaAhorroPlanCommandHandler _handler;

        [SetUp]
        public void Setup()
        {
            _clientesServiceMock = new Mock<IClientesService>();
            _cuentasServiceMock = new Mock<ICuentasService>();
            _memoryCacheMock = new Mock<IMemoryCacheLocalService>();
            _mapperMock = new Mock<IMapper>();
            _redisCacheMock = new Mock<IRedisCache>();
            _errorCatalogMock = new Mock<IErrorCatalogException>();

            _handler = new CrearCuentaAhorroPlanCommandHandler(
                _errorCatalogMock.Object,
                _redisCacheMock.Object,
                _memoryCacheMock.Object,
                _mapperMock.Object,
                _clientesServiceMock.Object,
                _cuentasServiceMock.Object
            );
        }

        [Test]
        public async Task Handle_ShouldCreateCuentaAhorroPlan_WhenValidRequest()
        {
            // Arrange
            var trackingId = Guid.NewGuid();
            var command = new CrearCuentaAhorroPlanCommand
            {
                IdTraking = trackingId,
                Request = new CrearCuentaAhorroPlanRequest
                {
                    cliente_id = 1,
                    cuenta_id = 1,
                    monto = 200,
                    moneda = "USD"
                }
            };

            var cliente = new clienteDto
            {
                cliente_id = 1,
                primer_nombre = "Juan",
                segundo_nombre = "Pedro",
                apellido_paterno = "Perez",
                apellido_materno = "Lopez",
                username = "juanperez"
            };

            var cuenta = new cuentaDto
            {
                cuenta_id = 1,
                saldo_disponible = 500,
                saldo_actual = 500,                
            };

            var numeroCuenta = new CrearNumeroCuentaResponse
            {
                numero_cuenta = "1234567890",                
            };

            var nuevaCuenta = new cuentaDto
            {
                cuenta_id = 2,
                saldo_disponible = 0,
                saldo_actual = 0,
                numero_cuenta = numeroCuenta.numero_cuenta,                
            };

            // Mock cache
            _memoryCacheMock.Setup(x => x.GetCachedData(It.IsAny<string>()))
                            .ReturnsAsync(new DataCacheLocal());

            // Mock servicios
            _clientesServiceMock.Setup(x => x.ClientePorId(1, trackingId))
                                .ReturnsAsync(new ResponseBase<GetClienteResponse>
                                {                                    
                                    data = new GetClienteResponse { cliente =  new List<clienteDto>(){ cliente } },
                                    error = new Error { success = true }
                                });

            _cuentasServiceMock.Setup(x => x.CuentaPorId(1, trackingId))
                               .ReturnsAsync(new ResponseBase<GetCuentaResponse>
                               {                                   
                                   data = new GetCuentaResponse { cuenta = new List<cuentaDto>() { cuenta } },
                                   error = new Error { success = true }
                               });

            _cuentasServiceMock.Setup(x => x.GenerarNumeroCuenta(It.IsAny<CrearNumeroCuentaRequest>(), trackingId))
                               .ReturnsAsync(new ResponseBase<CrearNumeroCuentaResponse>
                               {                                   
                                   data = numeroCuenta,
                                   error = new Error { success = true }
                               });

            _cuentasServiceMock.Setup(x => x.RegisterCuenta(It.IsAny<RegisterCuentaRequest>(), trackingId))
                               .ReturnsAsync(new ResponseBase<RegisterCuentaResponse>
                               {                                   
                                   data = new RegisterCuentaResponse { cuenta = nuevaCuenta },
                                   error = new Error { success = true, codeError = 100000, messageError = string.Empty }
                               });

            _cuentasServiceMock.Setup(x => x.CuentaPorId(2, trackingId))
                       .ReturnsAsync(new ResponseBase<GetCuentaResponse>
                       {
                           data = new GetCuentaResponse { cuenta = new List<cuentaDto>() { cuenta } },
                           error = new Error { success = true }
                       });

            _cuentasServiceMock
              .Setup(x => x.RegistrarMovimiento(It.IsAny<RegisterMovimientoRequest>(), trackingId))
              .Callback<RegisterMovimientoRequest, Guid>((req, id) => { /* no hace nada */ });
              

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.data.Should().NotBeNull();
            result.data.cuenta.Should().NotBeNull();
            result.data.cuenta.numero_cuenta.Should().Be(numeroCuenta.numero_cuenta);
        }
    }
}
