using RZD.Common.Configs.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RZD.Common.Configs
{
    public class MailSenderConfig: IBaseConfig
    {
        public static string Section => "MailSender";

        public string SmtpHost { get; set; }
        public int SmtpPort { get; set; }
        public string SmtpUser { get; set; }

        public string SmtpPass { get; set; }
        public string From { get; set; }
    }
}
