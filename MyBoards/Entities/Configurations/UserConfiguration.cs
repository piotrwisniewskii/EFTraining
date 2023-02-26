using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MyBoards.Entities.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
           builder.HasOne(u => u.Adress)
                .WithOne(a => a.User)
                .HasForeignKey<Adress>(a => a.UserId);

            builder.HasIndex(u => new { u.Email, u.FullName });
        }
    }
}
