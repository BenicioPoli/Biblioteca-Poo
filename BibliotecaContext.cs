using Microsoft.EntityFrameworkCore;

namespace SistemaBiblioteca {
	public class BibliotecaContext : DbContext {
		public DbSet<EstadoPrestamo> EstadosPrestamo {get; set;}
		public DbSet<EstadoReserva> EstadosReserva {get; set;}
		public DbSet<Genero> Generos {get; set;}
		public DbSet<Libro> Libros {get; set;}
		public DbSet<Prestamo> Prestamos {get; set;}
		public DbSet<Reserva> Reservas {get; set;}
		public DbSet<Socio> Socios {get; set;}
		public DbSet<TipoSocio> TiposSocio {get; set;}

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

				entity.HasOne(s => s.Tipo)
				      .WithMany()
				      .HasForeignKey("TipoSocio"); 
			});

			modelBuilder.Entity<Libro>(entity => {
				entity.ToTable("Libro");
				entity.HasKey(l => l.ISBN);

				entity.HasOne(l => l.Genero)
				      .WithMany()
				      .HasForeignKey("GeneroId");

				entity.Property<string>("GeneroId")
				      .HasColumnName("Genero");
			});

			modelBuilder.Entity<Prestamo>(entity => {
				entity.ToTable("Prestamo");
				entity.HasKey("SocioId", "LibroId");

				entity.Property<int>("SocioId").HasColumnName("Socio");
				entity.Property<string>("LibroId").HasColumnName("Libro");

				entity.HasOne(p => p.Socio)
				      .WithMany()
				      .HasForeignKey("SocioId");

				entity.HasOne(p => p.Libro)
				      .WithMany()
				      .HasForeignKey("LibroId");

				entity.HasOne(p => p.Estado)
				      .WithMany()
				      .HasForeignKey("EstadoId");

				entity.Property<int>("EstadoId").HasColumnName("Estado");
			});

			modelBuilder.Entity<Reserva>(entity => {
				entity.ToTable("Reserva");
				entity.HasKey("LibroId", "SocioId");

				entity.Property<string>("LibroId").HasColumnName("Libro");
				entity.Property<int>("SocioId").HasColumnName("Socio");

				entity.HasOne(r => r.Socio)
				      .WithMany()
				      .HasForeignKey("SocioId");

				entity.HasOne(r => r.Libro)
				      .WithMany()
				      .HasForeignKey("LibroId");

				entity.HasOne(r => r.Estado)
				      .WithMany()
				      .HasForeignKey("EstadoId");

				entity.Property<int>("EstadoId").HasColumnName("Estado");
			});
		}
	}
}
