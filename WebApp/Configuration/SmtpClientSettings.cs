using System.Security;
using System.Text;

namespace WebApp.Configuration
{
    public class SmtpClientSettings
    {
        public string FromEmail { get; set; }
        public string FromName { get; set; }
        public string Host { get; set; }
        public string Port { get; set; }
        public string Pwd { get; set; }
        public string Usr { get; set; }
    }
}
