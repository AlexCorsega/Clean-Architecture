using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Todo_App.Domain.Entities;

namespace Todo_App.Infrastructure.Persistence.Configurations;
public class TagConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.Property(s => s.Name)
             .HasMaxLength(50)
             .IsRequired();
        builder.HasOne(s => s.TodoItem)
            .WithMany(s => s.Tags)
            .HasForeignKey(s => s.TodoItemId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
