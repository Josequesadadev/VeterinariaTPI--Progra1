using Microsoft.EntityFrameworkCore;
using Veterinaria.Logica.Models;

namespace Veterinaria.Logica.Data
{
    public class VeterinariaDbContext : DbContext
    {
        public VeterinariaDbContext(DbContextOptions<VeterinariaDbContext> options) : base(options)
        {
        }

        public virtual DbSet<Usuario> Usuarios { get; set; }
        public virtual DbSet<Cliente> Clientes { get; set; }
        public virtual DbSet<TipoMascota> TiposMascota { get; set; }
        public virtual DbSet<TipoAtencion> TiposAtencion { get; set; }
        public virtual DbSet<Mascota> Mascotas { get; set; }
        public virtual DbSet<Atencion> Atenciones { get; set; }

        // Mapeo objeto-relacional: une cada clase y propiedad con su tabla y columna
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.ToTable("Usuarios");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id_usuario");
                entity.Property(e => e.NombreUsuario).HasColumnName("nombre_usuario").HasMaxLength(50).IsUnicode(false).IsRequired();
                entity.Property(e => e.Email).HasColumnName("email").HasMaxLength(100).IsUnicode(false).IsRequired();
                entity.Property(e => e.PasswordHash).HasColumnName("password_hash").HasMaxLength(255).IsUnicode(false).IsRequired();
                entity.Property(e => e.Rol).HasColumnName("rol").HasConversion<string>().HasMaxLength(20);
                entity.Property(e => e.FechaAlta).HasColumnName("fecha_alta").HasColumnType("datetime");
                entity.Property(e => e.Activo).HasColumnName("activo");
            });

            modelBuilder.Entity<Cliente>(entity =>
            {
                entity.ToTable("Clientes");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id_cliente");
                entity.Property(e => e.Codigo).HasColumnName("codigo").HasMaxLength(10).IsUnicode(false).IsRequired();
                entity.Property(e => e.Nombre).HasColumnName("nombre").HasMaxLength(50).IsUnicode(false).IsRequired();
                entity.Property(e => e.Apellido).HasColumnName("apellido").HasMaxLength(50).IsUnicode(false).IsRequired();
                entity.Property(e => e.Telefono).HasColumnName("telefono").HasMaxLength(20).IsUnicode(false);
                entity.Property(e => e.Activo).HasColumnName("activo");
                // El enum Sexo se guarda como un caracter: M, F o X
                entity.Property(e => e.Sexo)
                    .HasColumnName("sexo")
                    .HasColumnType("char(1)")
                    .HasConversion(
                        v => v == Sexo.Masculino ? "M" : v == Sexo.Femenino ? "F" : "X",
                        v => v == "M" ? Sexo.Masculino : v == "F" ? Sexo.Femenino : Sexo.Otro);
                entity.Ignore(e => e.NombreCompleto);
            });

            modelBuilder.Entity<TipoMascota>(entity =>
            {
                entity.ToTable("TiposMascota");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id_tipo_mascota");
                entity.Property(e => e.Nombre).HasColumnName("nombre").HasMaxLength(50).IsUnicode(false).IsRequired();
                entity.Property(e => e.Activo).HasColumnName("activo");
            });

            modelBuilder.Entity<TipoAtencion>(entity =>
            {
                entity.ToTable("TiposAtencion");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id_tipo_atencion");
                entity.Property(e => e.Nombre).HasColumnName("nombre").HasMaxLength(50).IsUnicode(false).IsRequired();
                entity.Property(e => e.Activo).HasColumnName("activo");
            });

            modelBuilder.Entity<Mascota>(entity =>
            {
                entity.ToTable("Mascotas");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id_mascota");
                entity.Property(e => e.Nombre).HasColumnName("nombre").HasMaxLength(50).IsUnicode(false).IsRequired();
                entity.Property(e => e.Edad).HasColumnName("edad");
                entity.Property(e => e.IdTipoMascota).HasColumnName("id_tipo_mascota");
                entity.Property(e => e.IdCliente).HasColumnName("id_cliente");
                entity.Property(e => e.Activo).HasColumnName("activo");

                entity.HasOne(d => d.TipoMascota).WithMany(p => p.Mascotas)
                    .HasForeignKey(d => d.IdTipoMascota)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Mascotas_TiposMascota");

                entity.HasOne(d => d.Cliente).WithMany(p => p.Mascotas)
                    .HasForeignKey(d => d.IdCliente)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Mascotas_Clientes");
            });

            modelBuilder.Entity<Atencion>(entity =>
            {
                entity.ToTable("Atenciones");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id_atencion");
                entity.Property(e => e.IdMascota).HasColumnName("id_mascota");
                entity.Property(e => e.IdTipoAtencion).HasColumnName("id_tipo_atencion");
                entity.Property(e => e.Fecha).HasColumnName("fecha").HasColumnType("datetime");
                entity.Property(e => e.Descripcion).HasColumnName("descripcion").HasMaxLength(255).IsUnicode(false).IsRequired();
                entity.Property(e => e.Importe).HasColumnName("importe").HasColumnType("decimal(10,2)");
                entity.Property(e => e.Activo).HasColumnName("activo");

                entity.HasOne(d => d.Mascota).WithMany(p => p.Atenciones)
                    .HasForeignKey(d => d.IdMascota)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Atenciones_Mascotas");

                entity.HasOne(d => d.TipoAtencion).WithMany(p => p.Atenciones)
                    .HasForeignKey(d => d.IdTipoAtencion)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Atenciones_TiposAtencion");
            });
        }
    }
}
