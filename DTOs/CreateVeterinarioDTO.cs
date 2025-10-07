namespace VeTLink.DTOs
{
    public class CreateVeterinarioDTO : VeterinarioDTO
    {
        public bool EsAdminClinica { get; set; }
        public int? ClinicaId { get; set; }
        public PersonaDTO Persona { get; set; } = null!;
    }
}
