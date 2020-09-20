using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Utils.Wrappers.Implementations;
using Utils.Wrappers.Interfaces;

namespace Utils
{
    public static class NetworkExtensions
    {
        [DllImport("wininet.dll")]
        private extern static bool InternetGetConnectedState(out int Description, int ReservedValue);
        [DllImport("ws2_32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int closesocket(uint s);

        public static bool CheckNetworkConnectivity()
        {
            int desc;
            return InternetGetConnectedState(out desc, 0);
        }

        public static bool IsConnected(this ISocket socket)
        {
            if (socket is SocketWrapped socketWrapped)
            {
                return socketWrapped.InternalSocket.IsConnected();
            }
            else if (socket is System.Net.Sockets.Socket socket2)
            {
                return socket2.IsConnected();
            }
            else
            {
                return DebugUtils.BreakReturn(false);
            }
        }

        public static bool IsConnected(this SocketWrapped socketWrapped)
        {
            return socketWrapped.InternalSocket.IsConnected();
        }

        public static bool IsConnected(this System.Net.Sockets.Socket socket)
        {
            try
            {
                return !(socket.Poll(1, SelectMode.SelectRead) && socket.Available == 0);
            }
            catch (SocketException) 
            {
                return false; 
            }
        }

        public static void Shutdown(this ISocket socket)
        {
            var handle = (uint)socket.Handle;

            try
            {
                socket.Shutdown(SocketShutdown.Both);
            }
            catch
            {
            }

            try
            {
                socket.Close();
            }
            catch
            {
            }

            closesocket(handle);

            socket.Dispose();
        }
    }
}
