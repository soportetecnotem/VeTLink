using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace VeTLink.DTOs.CDN
{
    public class ArchivosAzureDTO
    {
        //Asi se pone en el modelo para que no guarde en unicode
        //[Unicode(false)]
        //public string Imagen { get; set; }

        [Required]
        public required IFormFile Archivo { get; set; }
    }
}
