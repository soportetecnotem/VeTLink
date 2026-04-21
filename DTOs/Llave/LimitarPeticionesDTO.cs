using System.ComponentModel.DataAnnotations;

namespace VeTLink.DTOs.Llave
{
    public class LimitarPeticionesDTO
    {
        public const string Seccion = "limitarPeticiones";
        [Required]
        public int PeticionesPorDiaGratuito { get; set; }
    }
}
