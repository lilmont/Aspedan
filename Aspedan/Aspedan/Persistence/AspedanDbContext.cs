namespace Aspedan.Persistence;

public class AspedanDbContext : DbContext
{
	public DbSet<TaskItem> TaskItems { get; set; }

	public AspedanDbContext(DbContextOptions<AspedanDbContext> options) : base(options)
	{
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);
		modelBuilder.
			ApplyConfigurationsFromAssembly(typeof(AspedanDbContext).Assembly);
	}
}
