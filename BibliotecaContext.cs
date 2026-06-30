using Microsoft.EntityFrameworkCore;

namespace SistemaBiblioteca {
	public class BibliotecaContext : DbContext {
		public DbSet<EstadoPrestamo> EstadosPrestamo { get; set; }
		public DbSet<EstadoReserva> EstadosReserva { get; set; }
		public DbSet<Genero> Generos { get; set; }
		public DbSet<Libro> Libros { get; set; }
		public DbSet<Prestamo> Prestamos { get; set; }
		public DbSet<Reserva> Reservas { get; set; }
		public DbSet<Socio> Socios { get; set; }
		public DbSet<TipoSocio> TiposSocio { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
			optionsBuilder.UseSqlite("Data Source=Biblioteca.db");
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder) {
			modelBuilder.Entity<EstadoPrestamo>(entity => {
				entity.ToTable("EstadoPrestamo");
				entity.HasKey(e => e.idEstado);
			});

			modelBuilder.Entity<EstadoReserva>(entity => {
				entity.ToTable("EstadoReserva");
				entity.HasKey(e => e.idEstado);
			});

			modelBuilder.Entity<Genero>(entity => {
				entity.ToTable("Genero");
				entity.HasKey(g => g.ID);  
			});

			modelBuilder.Entity<TipoSocio>(entity => {
				entity.ToTable("TipoSocio");
				entity.HasKey(t => t.ID);
				entity.Property(t => t.ID).HasColumnName("idDeTipo");
				entity.Property(t => t.NombreTipo).HasColumnName("Tipo");
				entity.Property(t => t.MaxSimultaneos).HasColumnName("MaxLibros");
			});

			modelBuilder.Entity<Socio>(entity => {
				entity.ToTable("Socio");
				entity.HasKey(s => s.NroSocio);
				entity.Property(s => s.NroSocio).ValueGeneratedOnAdd();

				entity.Property(s => s.TipoId).HasColumnName("TipoSocio");
				entity.HasOne(s => s.Tipo)
				      .WithMany()
				      .HasForeignKey(s => s.TipoId);  

				entity.HasMany(s => s.Prestamos)
				      .WithOne(p => p.Socio!)
				      .HasForeignKey(p => p.SocioId)
				      .OnDelete(DeleteBehavior.Cascade);

				entity.HasMany(s => s.Reservas)
				      .WithOne(r => r.Socio!)
				      .HasForeignKey(r => r.SocioId)
				      .OnDelete(DeleteBehavior.Cascade);
			});

			modelBuilder.Entity<Libro>(entity => {
				entity.ToTable("Libro");
				entity.HasKey(l => l.ISBN);

				entity.HasMany(l => l.Generos)
				      .WithMany()
				      .UsingEntity<Dictionary<string,object>>(
						"LibroGenero",
						j => j.HasOne<Genero>().WithMany().HasForeignKey("GeneroId"),
						j => j.HasOne<Libro>().WithMany().HasForeignKey("ISBN"),
						j => {
							j.ToTable("LibroGenero");
							j.HasKey("ISBN", "GeneroId");
						}
					);

                entity.HasMany<Prestamo>()
                      .WithOne(p => p.Libro!)
                      .HasForeignKey(p => p.LibroISBN);

                entity.HasMany<Reserva>()
                      .WithOne(r => r.Libro!)
                      .HasForeignKey(r => r.LibroISBN);
			});

			modelBuilder.Entity<Prestamo>(entity => {
				entity.ToTable("Prestamo");
				entity.HasKey(p => new { p.SocioId, p.LibroISBN });

				entity.Property(p => p.SocioId).HasColumnName("Socio");
				entity.Property(p => p.LibroISBN).HasColumnName("Libro");
				entity.Property(p => p.EstadoId).HasColumnName("Estado");

				entity.HasOne(p => p.Estado).WithMany().HasForeignKey(p => p.EstadoId);
			});

			modelBuilder.Entity<Reserva>(entity => {
				entity.ToTable("Reserva");
				entity.HasKey(r => new { r.LibroISBN, r.SocioId });

				entity.Property(r => r.LibroISBN).HasColumnName("Libro");
				entity.Property(r => r.SocioId).HasColumnName("Socio");
				entity.Property(r => r.EstadoId).HasColumnName("Estado");

				entity.HasOne(r => r.Estado).WithMany().HasForeignKey(r => r.EstadoId);
			});
		}
	}
}
