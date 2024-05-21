namespace Aspedan.Entities.Configuration;

public class TaskItemConfiguration : IEntityTypeConfiguration<TaskItem>
{
	public void Configure(EntityTypeBuilder<TaskItem> builder)
	{
		builder.Property(p => p.Title).IsRequired().HasMaxLength(50);
		builder.Property(p => p.Description).IsRequired().HasMaxLength(1000);
	}
}
