// Author: Dominic Beger (Trade/ProgTrade) 2016

using System;
using System.Globalization;
using System.Net;
using System.Net.Sockets;

namespace nUpdate.Administration.Core.Proxy
{
    internal static class Utils
    {
        internal static string GetHost(TcpClient client)
        {
            if (client == null)
                throw new ArgumentNullException(nameof(client));

            string host = "";
            try
            {
                host = ((IPEndPoint) client.Client.RemoteEndPoint).Address.ToString();
            }
            catch
            {
                // ignored
            }

            return host;
        }

        internal static string GetPort(TcpClient client)
        {
            if (client == null)
                throw new ArgumentNullException(nameof(client));

            string port = "";
            try
            {
                port = ((IPEndPoint) client.Client.RemoteEndPoint).Port.ToString(CultureInfo.InvariantCulture);
            }
            catch
            {
                // ignored
            }

            return port;
        }
    }
}