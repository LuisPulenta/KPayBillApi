using System.ComponentModel.DataAnnotations;

namespace KPayBillApi.Web.Data.Entities
{
    public class Supplier
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Cuil-Cuit")]
        [MaxLength(15, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string Cuil { get; set; }

        [Display(Name = "Proveedor")]
        [MaxLength(50, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string Name { get; set; }

        [Display(Name = "Dirección")]
        [MaxLength(50, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string Address { get; set; }

        [Display(Name = "Teléfono")]
        [MaxLength(12, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string Phone { get; set; }

        [Display(Name = "Email")]
        [MaxLength(30, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string Email { get; set; }

        [Display(Name = "Activo")]
        public bool Active { get; set; }

        [Display(Name = "Empresa")]
        public string? ForCompanyName { get; set; }

        [Display(Name = "Empresa")]
        public int? ForCompanyId { get; set; }

        [Display(Name = "Empresa")]
        public string? FromCompanyName { get; set; }

        [Display(Name = "Empresa")]
        public int? FromCompanyId { get; set; }
    }
}