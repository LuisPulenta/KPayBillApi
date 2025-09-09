using System.ComponentModel.DataAnnotations;

namespace KPayBillApi.Web.Data.Entities
{
    public class VistaUserDocument
    {
        [Key]
        public string UserId { get; set; }

        public int EmitterCompanyId { get; set; }
        public int ReceiverCompanyId { get; set; }
        public int? Documents { get; set; }
    }
}