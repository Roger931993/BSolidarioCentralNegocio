using CentralNegocio.Domain.Entities;
using CentralNegocio.Shared.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CentralNegocio.Persistence.Repositories.EFCore.Config
{
    public class ClienteConfig : IEntityTypeConfiguration<cliente>
    {
        public void Configure(EntityTypeBuilder<cliente> builder)
        {
            builder.ToTable("cliente", Generals.General.schema_db_cliente);
            builder.HasKey(p => p.cliente_id);            

            // Relación muchos a uno
            //builder.HasOne(p => p.invoice).WithMany(p => p.invoice_cancel).HasForeignKey(p => p.invoice_id).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
