using System.ComponentModel.DataAnnotations;

namespace KPayBillApi.Web.Data.Entities
{
    public class UserCompany
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }

        public int CompanyId { get; set; }
    }
}


