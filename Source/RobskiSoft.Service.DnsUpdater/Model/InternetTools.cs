namespace RobskiSoft.Service.DnsUpdater.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.NetworkInformation;
    using System.Net.Sockets;
    using System.Text;
    using Heijden.DNS;

    /// <summary>
    /// Internet tools.
    /// </summary>
    public class InternetTools
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(InternetTools));
        private static readonly string openDnsResolver1 = "208.67.222.222";
        //private static readonly string openDnsResolver2 = "208.67.220.220";

        /// <summary>
        /// Resolves the IP address of the specified hostname.
        /// </summary>
        /// <param name="hostname">The hostname.</param>
        /// <returns>IPAddress instance.</returns>
        public IPAddress ResolveHostnameIP(string hostname)
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                throw new ApplicationException(Text.Error_NoNetwork);
            }

            Resolver resolver = new Resolver(openDnsResolver1);
            IPHostEntry host = resolver.GetHostEntry(hostname);

            if (host == null || host.AddressList == null || host.AddressList.Length <= 0)
            {
                throw new ApplicationException(string.Format(Text.Error_CantResolveHostnameIP, hostname));
            }

            IPAddress result = host
                .AddressList
                .FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);

            if (result == null)
            {
                throw new ApplicationException(string.Format(Text.Error_CantResolveHostnameIP, hostname));
            }

            log.DebugFormat(Text.Debug_ResolvedHostname, hostname, result.ToString());

            return result;
        }

        /// <summary>
        /// Gets the external IP.
        /// </summary>
        /// <returns>IPAddress instance.</returns>
        public IPAddress GetExternalIP()
        {
            IPAddress result = ResolveHostnameIP("myip.opendns.com");

            log.DebugFormat(Text.Debug_ExternalIPResolved, result.ToString());

            return result;
        }
    }
}
