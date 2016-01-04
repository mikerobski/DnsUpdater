namespace RobskiSoft.Service.DnsUpdater.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Mail;
    using System.Text;

    /// <summary>
    /// Sends notifiations.
    /// </summary>
    public class Notifyer
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(Notifyer));
        private SmtpClient client;

        /// <summary>
        /// Initializes a new instance of the <see cref="Notifyer"/> class.
        /// </summary>
        /// <param name="smtpServer">The SMTP server.</param>
        /// <param name="smtpPort">The SMTP port.</param>
        /// <param name="smtpUsername">The SMTP username.</param>
        /// <param name="smtpPassword">The SMTP password.</param>
        public Notifyer(string smtpServer, int smtpPort, string smtpUsername, string smtpPassword)
        {
            try
            {
                client = new SmtpClient(smtpServer, smtpPort);
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
            }
            catch (Exception ex)
            {
                log.ErrorFormat(Text.Exception_Generic, ex.Message);
            }
        }

        /// <summary>
        /// Ips the change notification.
        /// </summary>
        /// <param name="fromEmail">From email.</param>
        /// <param name="toEmail">To email.</param>
        /// <param name="hostname">The hostname.</param>
        /// <param name="newIp">The new ip.</param>
        /// <param name="result">The result.</param>
        public void IpChangeNotification(string fromEmail, string toEmail, string hostname, string newIp, string result)
        {
            if (client != null)
            {
                try
                {
                    client.Send(fromEmail, toEmail,
                        string.Format(Text.NotificationSubject_Success, hostname),
                        string.Format(Text.NotificationBody_Success, hostname, newIp, result)
                    );
                }
                catch (Exception ex)
                {
                    log.ErrorFormat(Text.Exception_Generic, ex.Message);
                }
            }
        }

        /// <summary>
        /// Ips the change failed.
        /// </summary>
        /// <param name="fromEmail">From email.</param>
        /// <param name="toEmail">To email.</param>
        /// <param name="hostname">The hostname.</param>
        /// <param name="newIp">The new ip.</param>
        /// <param name="result">The result.</param>
        public void IpChangeFailed(string fromEmail, string toEmail, string hostname, string newIp, string result)
        {
            if (client != null)
            {
                try
                {
                    client.Send(fromEmail, toEmail,
                        string.Format(Text.NotificationSubject_Failure, hostname),
                        string.Format(Text.NotificationBody_Failure, hostname, newIp, result)
                    );
                }
                catch (Exception ex)
                {
                    log.ErrorFormat(Text.Exception_Generic, ex.Message);
                }
            }
        }
    }
}
