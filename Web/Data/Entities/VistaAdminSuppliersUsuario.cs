using System.ComponentModel.DataAnnotations;

namespace KPayBillApi.Web.Data.Entities
{
    public class VistaAdminSuppliersUsuario
    {
        [Key]
        public string Id { get; set; }

        public int CompanyId { get; set; }
        public int? Suppliers { get; set; }
        public int? Pagadores { get; set; }
        public int? Usuarios { get; set; }
    }
}