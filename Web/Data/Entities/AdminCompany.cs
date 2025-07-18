using System.ComponentModel.DataAnnotations;

namespace KPayBillApi.Web.Data.Entities
{
    public class AdminCompany
    {
        [Key]
        public int Id { get; set; }

        public string UserId { get; set; }

        public int CompanyId { get; set; }
    }
}


