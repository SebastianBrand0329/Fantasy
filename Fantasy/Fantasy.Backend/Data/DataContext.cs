using Fantasy.Shared.Entities;
using Microsoft.EntityFrameworkCore;

namespace Fantasy.Backend.Data;

public class DataContext : DbContext
{
	public DataContext(DbContextOptions<DataContext> options) : base(options)
	{
	}

	public DbSet<Country> Countries { get; set; }
	public DbSet<ListaNegraWeb> Placas { get; set; }
	public DbSet<ListaNegraWebV1> PlacasPrueba { get; set; }
	public DbSet<ListaNegraWebV2> PlacasPruebas { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);
		modelBuilder.Entity<Country>().HasIndex(x => x.Name).IsUnique();
	}
}