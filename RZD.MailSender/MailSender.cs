using RZD.MailSender.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using RZD.Common.Configs;

namespace RZD.MailSender
{
    public class MailSender
    {
        private readonly MailSenderConfig _mailSenderConfig;

        public MailSender(IOptions<MailSenderConfig> mailSenderConfig)
        {
            _mailSenderConfig = mailSenderConfig.Value;
        }

        public void SendEmail(EmailMessage message)
        {
            using (var client = new SmtpClient(_mailSenderConfig.SmtpHost, _mailSenderConfig.SmtpPort))
            {
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(_mailSenderConfig.SmtpUser, _mailSenderConfig.SmtpPass);

                var mailMessage = new MailMessage(_mailSenderConfig.From, message.To, message.Subject, message.Body)
                {
                    IsBodyHtml = message.IsHtml
                };

                client.Send(mailMessage);
            }
        }
    }
}
