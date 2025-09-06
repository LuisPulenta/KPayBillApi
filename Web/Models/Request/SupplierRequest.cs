namespace KPayBillApi.Web.Models.Request
{
    public class SupplierRequest
    {
        public int Id { get; set; }
        public string Cuil { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public bool Active { get; set; }
        public int ForCompanyId { get; set; }
        public string ForCompanyName { get; set; }
    }
}