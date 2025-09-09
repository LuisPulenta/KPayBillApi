namespace KPayBillApi.Web.Models
{
    public class UserViewModel
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int UserTypeId { get; set; }
        public string UserTypeName { get; set; }
        public int? CompanyId { get; set; }
        public string? CompanyName { get; set; }
        public string Email { get; set; }
        public bool EmailConfirm { get; set; }
        public string PhoneNumber { get; set; }
        public bool Active { get; set; }
        public string FullName => $"{FirstName} {LastName}";
        public int? Suppliers { get; set; }
        public int? Usuarios { get; set; }
    }
}