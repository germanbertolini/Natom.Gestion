using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Natom.Gestion.WebApp.Clientes.Backend.Biz.Helpers
{
    public static class EmailHelper
    {
        public static async Task EnviarMailAsync(IConfiguration configuration, string asunto, string htmlBody, string emailDestinatario, string nombreDestinatario = null)
        {
            var fromAddress = new MailAddress(configuration["Email:SMTP_User"], configuration["Email:Sender_Name"]);
            var toAddress = new MailAddress(emailDestinatario, nombreDestinatario);

            var smtp = new SmtpClient
            {
                Host = configuration["Email:SMTP_Host"],
                Port = Convert.ToInt32(configuration["Email:SMTP_Port"]),
                EnableSsl = Convert.ToBoolean(configuration["Email:SMTP_EnableSSL"]),
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(configuration["Email:SMTP_User"], configuration["Email:SMTP_Password"])
            };
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                IsBodyHtml = true,
                Subject = asunto,
                Body = htmlBody
            })
            {
                await smtp.SendMailAsync(message);
            }
        }
    }
}
