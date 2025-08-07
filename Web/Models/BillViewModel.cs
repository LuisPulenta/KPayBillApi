using KPayBillApi.Common.Enums;
using KPayBillApi.Web.Data.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KPayBillApi.Web.Models
{
    public class BillViewModel
    {

        public int Id { get; set; }
        public int EmitterCompanyId { get; set; }
        public string EmitterCompanyName { get; set; }
        public int ReceiverCompanyId { get; set; }
        public string ReceiverCompanyName { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Cuil { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime BillDate { get; set; }
        public string Tipo { get; set; }
        public string Letra { get; set; }
        public int PV { get; set; }
        public int Numero { get; set; }
        public string StrComprobante { get; set; }
        public decimal ImporteNeto { get; set; }
        public decimal ImporteIVA { get; set; }
        public decimal ImporteTotal { get; set; }
        public string Archivo { get; set; }
        public string OC { get; set; }
        public string DocContable { get; set; }
        public BillState Estado { get; set; }
        public string Motivo { get; set; }
        public string NroDocRel { get; set; }
        public string ArchivoFullPath => string.IsNullOrEmpty(Archivo)
        ? $"https://gaos2.keypress.com.ar/KPayBillApi/images/documents/noimage.png"
        : $"https://gaos2.keypress.com.ar/KPayBillApi{Archivo.Substring(1)}";       
    }
}




