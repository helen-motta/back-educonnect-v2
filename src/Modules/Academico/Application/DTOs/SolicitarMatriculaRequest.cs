using System.ComponentModel.DataAnnotations;

namespace Modules.Academico.Application.DTOs
{
    public class SolicitarMatriculaRequest
    {
        [Required]
        public string NomeCandidato { get; set; }
        
        [Required, EmailAddress]
        public string Email { get; set; }
        
        [Required]
        public Guid CursoId { get; set; }
        
        [Required]
        public string Turno { get; set; } 

        [Required]
        public string Cpf { get; set; }
    }
}