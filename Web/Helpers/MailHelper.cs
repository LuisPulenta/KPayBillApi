﻿using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using KPayBillApi.Common.Models;

namespace KPayBillApi.Web.Helpers
{
    public class MailHelper : IMailHelper
    {
        private readonly IConfiguration _configuration;

        public MailHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        //----------------------------------------------------------------------------------
        public Response SendMail(string to, string cc, string subject, string body)
        {
            try
            {
                string from = _configuration["Mail:From"];
                string smtp = _configuration["Mail:Smtp"];
                string port = _configuration["Mail:Port"];
                string password = _configuration["Mail:Password"];

                List<string> listaCC = cc.Split(',').ToList();
                List<string> listaTO = to.Split(',').ToList();

                MimeMessage message = new MimeMessage();
                message.From.Add(MailboxAddress.Parse(from));
                foreach (var email in listaTO)
                {
                    message.To.Add(MailboxAddress.Parse(email));
                }

                foreach (var email in listaCC)
                {
                    message.Cc.Add(MailboxAddress.Parse(email));
                }

                message.Subject = subject;

                BodyBuilder bodyBuilder = new BodyBuilder
                {
                    HtmlBody = body
                    
                };
                message.Body = bodyBuilder.ToMessageBody();

                using (SmtpClient client = new SmtpClient())
                {
                    client.Connect(smtp, int.Parse(port), true);
                    client.Authenticate(from, password);
                    client.Send(message);
                    client.Disconnect(true);
                }

                return new Response { IsSuccess = true };

            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    Result = ex
                };
            }
        }

        //----------------------------------------------------------------------------------
        public Response SendMailSinCc(string to, string subject, string body)
        {
            try
            {
                string from = _configuration["Mail:From"];
                string smtp = _configuration["Mail:Smtp"];
                string port = _configuration["Mail:Port"];
                string password = _configuration["Mail:Password"];

                List<string> listaTO = to.Split(',').ToList();

                MimeMessage message = new MimeMessage();
                message.From.Add(MailboxAddress.Parse(from));
                foreach (var email in listaTO)
                {
                    message.To.Add(MailboxAddress.Parse(email));
                }

                message.Subject = subject;

                BodyBuilder bodyBuilder = new BodyBuilder
                {
                    HtmlBody = body

                };
                message.Body = bodyBuilder.ToMessageBody();

                using (SmtpClient client = new SmtpClient())
                {
                    client.Connect(smtp, int.Parse(port), true);
                    client.Authenticate(from, password);
                    client.Send(message);
                    client.Disconnect(true);
                }

                return new Response { IsSuccess = true };

            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    Result = ex
                };
            }
        }
    }
}
