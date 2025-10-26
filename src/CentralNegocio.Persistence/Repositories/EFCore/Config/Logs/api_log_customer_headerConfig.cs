using CentralNegocio.Domain.Entities;
using CentralNegocio.Shared.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CentralNegocio.Persistence.Repositories.EFCore.Config.Logs
{
    public class api_log_Cliente_headerConfig : IEntityTypeConfiguration<api_log_cliente_header>
    {
        public void Configure(EntityTypeBuilder<api_log_cliente_header> builder)
        {
            builder.ToTable("api_log_cliente_header", Generals.General.schema_db_logs);
            builder.HasKey(p => p.api_log_cliente_header_id);

            builder.Property(e => e.api_log_cliente_header_id)
               .HasColumnName("api_log_cliente_header_id")
               .UseIdentityAlwaysColumn();            
        }
    }
}
