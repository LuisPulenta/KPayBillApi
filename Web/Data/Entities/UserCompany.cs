using System;
using System.ComponentModel.DataAnnotations;

namespace KPayBillApi.Web.Data.Entities
{
    public class UserCompany
    {
        [Key]
        public int Id { get; set; }

        public string UserId { get; set; }

        public int CompanyId { get; set; }

        public static implicit operator UserCompany(AdminCompany v)
        {
            throw new NotImplementedException();
        }
    }
}


