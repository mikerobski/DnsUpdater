namespace RobskiSoft.Service.DnsUpdater.Model
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;

    /// <summary>
    /// Dyn-specific methods.
    /// </summary>
    public class Dyn
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(Dyn));
        private string authInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="Dyn"/> class.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        public Dyn(string username, string password)
        {
            authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(username + ":" + password));
        }

        /// <summary>
        /// Gets the external IP.
        /// </summary>
        /// <returns>IPAddress instance.</returns>
        public IPAddress GetExternalIP()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://checkip.dyndns.org");
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            StreamReader sr = new StreamReader(response.GetResponseStream());
            string result = sr.ReadToEnd().Trim();
            string[] a = result.Split(':');
            string a2 = a[1].Substring(1);
            string[] a3 = a2.Split('<');
            string a4 = a3[0];

            log.DebugFormat(Text.Debug_ExternalIPResolved, a4);

            return IPAddress.Parse(a4);
        }

        /// <summary>
        /// Updates the hostname.
        /// </summary>
        /// <param name="hostname">The hostname.</param>
        /// <param name="ip">The IP address.</param>
        /// <param name="result">The result.</param>
        /// <returns>True if the update was successful, false otherwise.</returns>
        public bool UpdateHostname(string hostname, IPAddress ip, out string result)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(string.Format("https://members.dyndns.org/nic/update?hostname={0}&myip={1}", hostname, ip.ToString()));
            request.Host = "members.dyndns.org";
            request.UserAgent = "Dyn";
            request.Headers["Authorization"] = "Basic " + authInfo;

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            StreamReader sr = new StreamReader(response.GetResponseStream());
            result = sr.ReadToEnd().Trim();

            if (!result.Equals("good " + ip.ToString()))
            {
                log.DebugFormat(Text.Warning_UpdateHostFailed, hostname, ip.ToString(), result);
                return false;
            }

            log.InfoFormat(Text.Info_UpdatedHost, hostname, ip.ToString(), result);
            return true;
        }
    }
}
