using System.ComponentModel.DataAnnotations;

namespace Web.Data.Entities
{
    public class Reason
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Motivo")]
        [MaxLength(20, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string Name { get; set; }
    }
}
