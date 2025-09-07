using System;

namespace KPayBillApi.Web.Models.Request

{
    public class BillsResueltosRequest
    {
        public int? CompanyId { get; set; }
        public string UserId { get; set; }
        public DateTime Desde { get; set; }
        public DateTime Hasta { get; set; }
    }
}