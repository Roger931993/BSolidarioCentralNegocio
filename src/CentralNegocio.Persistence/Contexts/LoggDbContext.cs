using CentralNegocio.Domain.Models;
using CentralNegocio.Persistence.Repositories.EFCore.Config.Logs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql;
using NpgsqlTypes;
using System.Data;

namespace CentralNegocio.Persistence.Contexts
{
    public class LoggDbContext : DbContext
    {
        private const string spi_insertar_log = "spi_insertar_log";
        public LoggDbContext(DbContextOptions<LoggDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            #region Configs
            modelBuilder.ApplyConfiguration(new api_log_Cliente_headerConfig());
            modelBuilder.ApplyConfiguration(new api_log_Cliente_detailConfig());
            #endregion

            // También puedes mapear más detalles si lo deseas
            ConfigureConvertionDatetime(modelBuilder);
        }

        private static void ConfigureConvertionDatetime(ModelBuilder modelBuilder)
        {
            // Configurar ValueConverter para DateTime a UTC
            var dateTimeConverter = new ValueConverter<DateTime, DateTime>(
                   v => v.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(v, DateTimeKind.Utc) : v.ToUniversalTime(),
                   v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
               );

            var nullableDateTimeConverter = new ValueConverter<DateTime?, DateTime?>(
                v => v.HasValue ? (v.Value.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v.Value.ToUniversalTime()) : (DateTime?)null,
                v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : (DateTime?)null
            );

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var properties = entityType.ClrType.GetProperties()
                    .Where(p => p.PropertyType == typeof(DateTime) || p.PropertyType == typeof(DateTime?));

                foreach (var property in properties)
                {
                    if (property.PropertyType == typeof(DateTime))
                    {
                        modelBuilder.Entity(entityType.Name).Property(property.Name).HasConversion(dateTimeConverter);
                    }
                    else if (property.PropertyType == typeof(DateTime?))
                    {
                        modelBuilder.Entity(entityType.Name).Property(property.Name).HasConversion(nullableDateTimeConverter);
                    }
                }
            }
        }
        // Método para ejecutar el procedimiento almacenado
        public async Task ExecuteSaveLogg(LoggingMdl loggingDto)
        {
            try
            {
                // Llenar el DataTable con los datos de los detalles
                var detailsArray = loggingDto.Details.Select(d => new LogDetailsType
                {
                    CreatedAt = d.CreatedAt,
                    StatusCode = d.StatusCode,
                    TypeProcess = d.TypeProcess,
                    DataMessage = d.DataMessage,
                    ProcessComponent = d.ProcessComponent
                }).ToArray();


                var conn = (NpgsqlConnection)Database.GetDbConnection();
                await conn.OpenAsync(); // Necesario si aún no está abierta


                using var cmd = new NpgsqlCommand($"SELECT public.{spi_insertar_log}(@request_method, @request_url, @response_code, @id_tracking, @created_at, @details_list)", conn);

                cmd.Parameters.AddWithValue("request_method", loggingDto.Header.RequestMethod);
                cmd.Parameters.AddWithValue("request_url", loggingDto.Header.RequestUrl);
                cmd.Parameters.AddWithValue("response_code", loggingDto.Header.ResponseCode!.ToString());
                cmd.Parameters.AddWithValue("id_tracking", loggingDto.Header.IdTracking);
                cmd.Parameters.AddWithValue("created_at", loggingDto.Header.CreatedAt);
                cmd.Parameters.Add(new NpgsqlParameter
                {
                    ParameterName = "details_list",
                    Value = detailsArray,
                    NpgsqlDbType = NpgsqlDbType.Array,
                    DataTypeName = "log_details_type"
                });

                await cmd.ExecuteNonQueryAsync();

                await conn.CloseAsync();
            }
            catch
            {
                // No aplica excpecion si no registra el log en la BDD
            }
        }

    }

    [PgName("log_details_type")] // nombre exacto del tipo en PostgreSQL
    public class LogDetailsType
    {
        [PgName("created_at")]
        public DateTime CreatedAt { get; set; }

        [PgName("status_code")]
        public int StatusCode { get; set; }

        [PgName("type_process")]
        public string TypeProcess { get; set; }

        [PgName("data_message")]
        public string DataMessage { get; set; }

        [PgName("process_component")]
        public string ProcessComponent { get; set; }
    }
}
