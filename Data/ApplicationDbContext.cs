using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VeTLink.Models;

namespace VeTLink.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext (DbContextOptions options) : base(options)
        {
        }

        public DbSet<Alergia> Alergias { get; set; }
        public DbSet<Bano> Banos { get; set; }
        public DbSet<CarnetPreventivo> Carnet { get; set; }
        public DbSet<Cirugia> Cirugias { get; set; }
        public DbSet<Clinica> Clinicas { get; set; }
        public DbSet<Comportamiento> Comportamientos { get; set; }
        public DbSet<ComportamientoSocializacion> ComportamientosSocializacion { get; set; }
        public DbSet<CondicionCorporal> CondicionesCorporal { get; set; }
        public DbSet<ConsultaMedica> ConsultasMedicas { get; set; }
        public DbSet<Desparasitacion> Desparasitaciones { get; set; }
        public DbSet<Diagnostico> Diagnosticos { get; set; }
        public DbSet<Direccion> Direcciones { get; set; }
        public DbSet<Dueno> Duenos { get; set; }
        public DbSet<Enfermedad> Enfermedades { get; set; }
        public DbSet<EstadoGeneral> EstadosGeneral { get; set; }
        public DbSet<EstadoSuscripcion> EstadosSuscripcion { get; set; }
        public DbSet<EventoCalendario> EventosCalendario { get; set; }
        public DbSet<EvidenciaEvolucion> EvidenciasEvolucion { get; set; }
        public DbSet<EvolucionClinica> EvolucionesClinicas { get; set; }
        public DbSet<ExploracionFisica> ExploracionesFisicas { get; set; }
        public DbSet<Hidratacion> Hidrataciones { get; set; }
        public DbSet<HistorialEnfermedad> HistorialEnfermedades { get; set; }
        public DbSet<HistorialMedico> HistorialesMedicos { get; set; }
        public DbSet<HistorialReproductivo> HistorialesReproductivos { get; set; }
        public DbSet<Mascota> Mascotas { get; set; }
        public DbSet<Medicamento> Medicamentos { get; set; }
        public DbSet<Mucosa> Mucosas { get; set; }
        public DbSet<Modulo> Modulos { get; set; }
        public DbSet<NivelActividad> NivelActividades { get; set; }
        public DbSet<Persona> Personas { get; set; }
        public DbSet<Plan> Planes { get; set; }
        public DbSet<Profilaxis> LimpiezaProfilaxis { get; set; }
        public DbSet<PruebaLaboratorio> PruebasLaboratorios { get; set; }
        public DbSet<ReaccionSocial> ReaccionesSociales { get; set; }
        public DbSet<Receta> Recetas { get; set; }
        public DbSet<SintomaActual> SintomasActuales { get; set; }
        public DbSet<Suscripcion> Suscripciones { get; set; }
        public DbSet<Sucursal> Sucursales { get; set; }
        public DbSet<TipoCirugia> TiposCirugias { get; set; }
        public DbSet<TipoPrueba> TiposPruebas { get; set; }
        public DbSet<TipoServicio> TiposServicios { get; set; }
        public DbSet<TipoTratamiento> TiposTratamientos { get; set; }
        public DbSet<TipoUsuario> TiposUsuarios { get; set; }
        public DbSet<Tratamiento> Tratamientos { get; set; }
        public DbSet<UnidadTiempo> UnidadesTiempo { get; set; }
        public DbSet<Vacuna> Vacunas { get; set; }
        public DbSet<Veterinario> Veterinarios { get; set; }
        public DbSet<ViaAdministracion> ViasAdministracion { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Aplicar DeleteBehavior.Restrict a TODAS las relaciones
            foreach (var relationship in modelBuilder.Model.GetEntityTypes()
                .SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }
    }
}
