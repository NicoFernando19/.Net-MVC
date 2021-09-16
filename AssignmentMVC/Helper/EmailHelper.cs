using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace AssignmentMVC.Helper
{
    public class EmailHelper
    {
        public static void SendEmail(string[] to, string from, string subject, string message)
        {
            var mailMessage = new MailMessage
            {
                Subject = subject,
                Body = message,
                IsBodyHtml = true,
                From = new MailAddress(from)
            };
            foreach (var address in to) { mailMessage.To.Add(address); }
            using (var smtp = new SmtpClient())
            {
                smtp.Host = "smtp-mail.outlook.com";
                smtp.Port = 587;
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Credentials = new NetworkCredential("fortesting1981@outlook.com", "testing198!");

                try
                {
                    smtp.Send(mailMessage);
                }
                catch (Exception e) { Console.WriteLine($"ERROR:{e.Message}"); }
            }
        }

        public static void SendEmail(string v1, string v2, string v3, string v4)
        {
            throw new NotImplementedException();
        }
    }
}
