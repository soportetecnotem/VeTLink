namespace VeTLink.Models
{
    public class ComportamientoSocializacion
    {
        public Guid Id { get; set; }
        public DateTime Fecha { get; set; }
        public string? CompProblematico { get; set; }
        public string? Observaciones { get; set; }

        public Guid HistorialMedicoId { get; set; }
        public HistorialMedico? HistorialMedico { get; set; }

        public int NivelActividadId { get; set; }
        public NivelActividad? NivelActividad { get; set; }

        public int ReaccionPersonaId { get; set; }
        public ReaccionSocial? ReaccionPersona { get; set; }

        public int ReaccionAnimalId { get; set; }
        public ReaccionSocial? ReaccionAnimal { get; set; }


    }
}
