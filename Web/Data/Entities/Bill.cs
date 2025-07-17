using System;
using System.ComponentModel.DataAnnotations;
using KPayBillApi.Common.Enums;

namespace KPayBillApi.Web.Data.Entities
{
    public class Bill
    {
        [Key]
        public int Id { get; set; }

        public int IdCompany { get; set; }

        public string Cuil { get; set; }

        [Display(Name = "Fecha Carga")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public DateTime CreateDate { get; set; }

        [Display(Name = "Fecha Factura")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public DateTime BillDate { get; set; }

        [Display(Name = "Tipo")]
        [MaxLength(3, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string Tipo { get; set; }

        [Display(Name = "Letra")]
        [MaxLength(1, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string Letra { get; set; }

        [Display(Name = "PV")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public int PV { get; set; }

        [Display(Name = "Numero")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public int Numero { get; set; }

        [Display(Name = "StrComprobante\r\n")]
        [MaxLength(20, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string StrComprobante { get; set; }

        [Display(Name = "Importe Neto")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public decimal ImporteNeto { get; set; }

        [Display(Name = "Importe IVA")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public decimal ImporteIVA { get; set; }

        [Display(Name = "Importe Total")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public decimal ImporteTotal { get; set; }

        [Display(Name = "Archivo")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string Archivo { get; set; }

        [Display(Name = "OC")]
        public string OC { get; set; }

        [Display(Name = "DocContable")]
        public string DocContable { get; set; }

        [Display(Name = "Estado")]
        public BillState Estado { get; set; }

        [Display(Name = "Motivo")]
        public string Motivo { get; set; }
    }
}
