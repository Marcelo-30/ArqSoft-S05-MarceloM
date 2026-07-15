using CitasApp.Domain.Models;
using CitasApp.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CitasApp.Infrastructure.Persistence
{
    public sealed class CitasAppDbContext : IdentityDbContext<ApplicationUser>
    {
        public CitasAppDbContext(DbContextOptions<CitasAppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Paciente> Pacientes => Set<Paciente>();

        public DbSet<Medico> Medicos => Set<Medico>();

        public DbSet<Cita> Citas => Set<Cita>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigurarPaciente(modelBuilder);
            ConfigurarMedico(modelBuilder);
            ConfigurarCita(modelBuilder);
            ConfigurarApplicationUser(modelBuilder);
        }

        private static void ConfigurarPaciente(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Paciente>(entity =>
            {
                entity.ToTable("pacientes");
                entity.HasKey(paciente => paciente.Id);

                entity.Property(paciente => paciente.Id)
                    .HasColumnName("id")
                    .HasMaxLength(36)
                    .ValueGeneratedNever();

                entity.Property(paciente => paciente.Nombre)
                    .HasColumnName("nombre")
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(paciente => paciente.Apellido)
                    .HasColumnName("apellido")
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(paciente => paciente.Email)
                    .HasColumnName("email")
                    .HasMaxLength(150)
                    .IsRequired();

                entity.Property(paciente => paciente.Telefono)
                    .HasColumnName("telefono")
                    .HasMaxLength(20)
                    .IsRequired();

                entity.HasIndex(paciente => paciente.Email).IsUnique();
            });
        }

        private static void ConfigurarMedico(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Medico>(entity =>
            {
                entity.ToTable("medicos");
                entity.HasKey(medico => medico.Id);

                entity.Property(medico => medico.Id)
                    .HasColumnName("id")
                    .HasMaxLength(36)
                    .ValueGeneratedNever();

                entity.Property(medico => medico.Nombre)
                    .HasColumnName("nombre")
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(medico => medico.Apellido)
                    .HasColumnName("apellido")
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(medico => medico.Especialidad)
                    .HasColumnName("especialidad")
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(medico => medico.NumeroLicencia)
                    .HasColumnName("numero_licencia")
                    .HasMaxLength(50)
                    .IsRequired();

                entity.HasIndex(medico => medico.NumeroLicencia).IsUnique();
            });
        }

        private static void ConfigurarCita(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cita>(entity =>
            {
                entity.ToTable("citas");
                entity.HasKey(cita => cita.Id);

                entity.Property(cita => cita.Id)
                    .HasColumnName("id")
                    .HasMaxLength(36)
                    .ValueGeneratedNever();

                entity.Property(cita => cita.PacienteId)
                    .HasColumnName("paciente_id")
                    .HasMaxLength(36)
                    .IsRequired();

                entity.Property(cita => cita.MedicoId)
                    .HasColumnName("medico_id")
                    .HasMaxLength(36)
                    .IsRequired();

                entity.Property(cita => cita.Fecha)
                    .HasColumnName("fecha")
                    .HasColumnType("date")
                    .IsRequired();

                entity.Property(cita => cita.Hora)
                    .HasColumnName("hora")
                    .HasColumnType("time without time zone")
                    .IsRequired();

                entity.Property(cita => cita.Motivo)
                    .HasColumnName("motivo")
                    .HasMaxLength(500)
                    .IsRequired();

                entity.Property(cita => cita.Estado)
                    .HasColumnName("estado")
                    .HasMaxLength(30)
                    .IsRequired();

                entity.HasOne<Paciente>()
                    .WithMany()
                    .HasForeignKey(cita => cita.PacienteId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne<Medico>()
                    .WithMany()
                    .HasForeignKey(cita => cita.MedicoId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(cita => cita.PacienteId);
                entity.HasIndex(cita => cita.MedicoId);

                entity.HasIndex(cita => new { cita.MedicoId, cita.Fecha, cita.Hora })
                    .IsUnique()
                    .HasFilter("estado <> 'Cancelada'");
            });
        }

        private static void ConfigurarApplicationUser(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.Property(user => user.MedicoId)
                    .HasColumnName("medico_id")
                    .HasMaxLength(36);

                entity.HasOne<Medico>()
                    .WithOne()
                    .HasForeignKey<ApplicationUser>(user => user.MedicoId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasIndex(user => user.MedicoId)
                    .IsUnique()
                    .HasFilter("medico_id IS NOT NULL");
            });
        }
    }
}
