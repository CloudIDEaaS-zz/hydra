using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using Utils.Wrappers.Interfaces;
using Utils.Wrappers.Implementations;

namespace Utils
{
    public class SocketFactory : ISocketFactory
    {
        public ISocket CreateSocket(AddressFamily addressFamily, SocketType socketType, ProtocolType protocolType)
        {
            return new Wrappers.Implementations.Socket(addressFamily, socketType, protocolType);
        }
    }
}
