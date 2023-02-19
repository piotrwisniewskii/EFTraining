using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MyBoards.Entities.Configurations
{
    public class AdressConfiguration : IEntityTypeConfiguration<Adress>
    {
        public void Configure(EntityTypeBuilder<Adress> builder)
        {
            builder.OwnsOne(a => a.Coordinate, cmb =>
            {
                cmb.Property(c => c.Latitude).HasPrecision(18, 7);
                cmb.Property(c => c.Longitude).HasPrecision(18, 7);
            });
        }
    }
}
