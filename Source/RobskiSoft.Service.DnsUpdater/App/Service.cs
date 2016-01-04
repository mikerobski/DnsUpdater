namespace RobskiSoft.Service.DnsUpdater.App
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text;
    using RobskiSoft.Service.DnsUpdater.Model;
    using System.Configuration;

    /// <summary>
    /// The DNS Updater service.
    /// </summary>
    public class Service
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(Service));

        /// <summary>
        /// The Main method.
        /// </summary>
        /// <param name="args">The command-line arguments.</param>
        public static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();

            if (args == null || args.Length < 2)
            {
                log.WarnFormat(Text.Warning_InputParameterMissing);
                return;
            }

            log.DebugFormat(Text.Debug_StartingProcess);

            string username = args[0];
            string password = args[1];
            IList<string> hosts = new List<string>();
            for (int i = 2; i < args.Length; i++)
            {
                hosts.Add(args[i]);
            }

            InternetTools tools = new InternetTools();
            Dyn dyn = new Dyn(username, password);

            try
            {
                IPAddress currentIP = tools.GetExternalIP();

                Notifyer notifyer = new Notifyer(
                    ConfigurationManager.AppSettings["SmtpServer"],
                    int.Parse(ConfigurationManager.AppSettings["SmtpPort"]),
                    ConfigurationManager.AppSettings["SmtpUsername"],
                    ConfigurationManager.AppSettings["SmtpPassword"]);

                foreach (string hostname in hosts)
                {
                    log.DebugFormat(Text.Debug_CheckingHost, hostname);

                    try
                    {
                        IPAddress hostIP = tools.ResolveHostnameIP(hostname);

                        if (hostIP.ToString() == currentIP.ToString())
                        {
                            // Skip the update
                            log.DebugFormat(Text.Debug_SkippingHost, hostname);
                        }
                        else
                        {
                            // Update host IP address
                            log.DebugFormat(Text.Debug_UpdatingHost, hostname, currentIP.ToString());

                            string result;
                            bool success = dyn.UpdateHostname(hostname, currentIP, out result);

                            if (success)
                            {
                                notifyer.IpChangeNotification(
                                    ConfigurationManager.AppSettings["FromEmail"],
                                    ConfigurationManager.AppSettings["NotifyEmail"],
                                    hostname,
                                    currentIP.ToString(),
                                    result);
                            }
                            else
                            {
                                notifyer.IpChangeNotification(
                                    ConfigurationManager.AppSettings["FromEmail"],
                                    ConfigurationManager.AppSettings["NotifyEmail"],
                                    hostname,
                                    currentIP.ToString(),
                                    result);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        log.ErrorFormat(Text.Exception_Generic, ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                log.ErrorFormat(Text.Exception_Generic, ex.Message);
            }

            log.DebugFormat(Text.Debug_ProcessCompleted);
        }
    }
}
