using RZD.MailSender.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RZD.MailSender
{
    public class MailSender
    {
        private readonly string _smtpHost;
        private readonly int _smtpPort;
        private readonly string _smtpUser;
        private readonly string _smtpPass;
        private readonly string _from;

        public MailSender(string smtpHost, int smtpPort, string smtpUser, string smtpPass, string from)
        {
            _smtpHost = smtpHost;
            _smtpPort = smtpPort;
            _smtpUser = smtpUser;
            _smtpPass = smtpPass;
            _from = from;
        }

        public void SendEmail(EmailMessage message)
        {
            using (var client = new SmtpClient(_smtpHost, _smtpPort))
            {
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(_smtpUser, _smtpPass);

                var mailMessage = new MailMessage(_from, message.To, message.Subject, message.Body)
                {
                    IsBodyHtml = message.IsHtml
                };

                client.Send(mailMessage);
            }
        }
    }
}
