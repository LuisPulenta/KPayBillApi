using System;

namespace KPayBillApi.Web.Models.Request
{
    public class BillRequest
    {
        public int Id { get; set; }
        public int EmitterCompanyId { get; set; }
        public string EmitterCompanyName { get; set; }
        public int ReceiverCompanyId { get; set; }
        public string ReceiverCompanyName { get; set; }
        public string UserId { get; set; }
        public string Cuil { get; set; }
        public DateTime BillDate { get; set; }
        public string Tipo { get; set; }
        public string Letra { get; set; }
        public int PV { get; set; }
        public int Numero { get; set; }
        public string StrComprobante { get; set; }
        public decimal ImporteNeto { get; set; }
        public decimal ImporteIVA { get; set; }
        public decimal ImporteTotal { get; set; }
        public string OC { get; set; }
        public string DocContable { get; set; }
        public int Estado { get; set; }
        public string Motivo { get; set; }
        public byte[] ImageArray { get; set; }
    }
}
