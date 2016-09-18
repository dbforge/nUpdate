// Author: Dominic Beger (Trade/ProgTrade) 2016

using System;
using System.Net.Sockets;
using nUpdate.Administration.Core.Proxy.EventArgs;

namespace nUpdate.Administration.Core.Proxy
{
    /// <summary>
    ///     Proxy client interface.  This is the interface that all proxy clients must implement.
    /// </summary>
    public interface IProxyClient
    {
        /// <summary>
        ///     Gets or sets proxy host name or IP address.
        /// </summary>
        string ProxyHost { get; set; }

        /// <summary>
        ///     Gets or sets proxy port number.
        /// </summary>
        int ProxyPort { get; set; }

        /// <summary>
        ///     Gets String representing the name of the proxy.
        /// </summary>
        string ProxyName { get; }

        /// <summary>
        ///     Gets or set the TcpClient object if one was specified in the constructor.
        /// </summary>
        TcpClient TcpClient { get; set; }

        /// <summary>
        ///     Event handler for CreateConnectionAsync method completed.
        /// </summary>
        event EventHandler<CreateConnectionAsyncCompletedEventArgs> CreateConnectionAsyncCompleted;

        /// <summary>
        ///     Creates a remote TCP connection through a proxy server to the destination host on the destination port.
        /// </summary>
        /// <param name="destinationHost">Destination host name or IP address.</param>
        /// <param name="destinationPort">Port number to connect to on the destination host.</param>
        /// <returns>
        ///     Returns an open TcpClient object that can be used normally to communicate
        ///     with the destination server
        /// </returns>
        /// <remarks>
        ///     This method creates a connection to the proxy server and instructs the proxy server
        ///     to make a pass through connection to the specified destination host on the specified
        ///     port.
        /// </remarks>
        TcpClient CreateConnection(string destinationHost, int destinationPort);

        /// <summary>
        ///     Asynchronously creates a remote TCP connection through a proxy server to the destination host on the destination
        ///     port.
        /// </summary>
        /// <param name="destinationHost">Destination host name or IP address.</param>
        /// <param name="destinationPort">Port number to connect to on the destination host.</param>
        /// <returns>
        ///     Returns an open TcpClient object that can be used normally to communicate
        ///     with the destination server
        /// </returns>
        /// <remarks>
        ///     This method creates a connection to the proxy server and instructs the proxy server
        ///     to make a pass through connection to the specified destination host on the specified
        ///     port.
        /// </remarks>
        void CreateConnectionAsync(string destinationHost, int destinationPort);
    }
}