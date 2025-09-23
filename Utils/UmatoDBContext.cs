using Microsoft.EntityFrameworkCore;
using System.Reflection;
using UmatoMusume.Models;

namespace UmatoMusume.Utils
{
	public class UmatoDBContext : DbContext
	{
		public DbSet<RectConfig> RectConfigs { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlite($"Data Source=UmamusumePD.sqlite", option =>
			{
				option.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName);
			});

			SQLitePCL.Batteries.Init();
			base.OnConfiguring(optionsBuilder);
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<RectConfig>(entity =>
			{
				entity.HasKey(e => e.Id);
				entity.Property(e => e.Id).IsRequired();
				entity.Property(e => e.RectName).IsRequired();
				entity.Property(e => e.X).IsRequired();
				entity.Property(e => e.Y).IsRequired();
				entity.Property(e => e.Width).IsRequired();
				entity.Property(e => e.Height).IsRequired();
			});
			base.OnModelCreating(modelBuilder);
		}
	}
}
