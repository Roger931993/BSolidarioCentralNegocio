using CentralNegocio.Domain.Entities;
using CentralNegocio.Shared.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CentralNegocio.Persistence.Repositories.EFCore.Config
{
    internal class Cliente_contactConfig : IEntityTypeConfiguration<contacto>
    {
        public void Configure(EntityTypeBuilder<contacto> builder)
        {
            builder.ToTable("contacto", Generals.General.schema_db_cliente);
            builder.HasKey(p => p.contacto_id);

            // Relación muchos a uno
            builder.HasOne(p => p.cliente).WithMany(p => p.contacto).HasForeignKey(p => p.cliente_id).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
