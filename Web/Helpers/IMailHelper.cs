using KPayBillApi.Common.Models;

namespace KPayBillApi.Web.Helpers
{
    public interface IMailHelper
    {
        Response SendMail(string to, string cc, string subject, string body);
        Response SendMailSinCc(string to, string subject, string body);
    }
}
