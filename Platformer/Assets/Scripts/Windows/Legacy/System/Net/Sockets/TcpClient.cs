#if UNITY_METRO && !UNITY_EDITOR

using System;
using System.IO;
using System.Net;
using MyPlugin.WACK.System.Net;

namespace System.Net.Sockets
{

    public class TcpClient
	{
 
        private TcpClientNative _tcpClientImpl;

        public TcpClient()
        {
            _tcpClientImpl = new TcpClientNative();
        }

        public int SendTimeout
        {
            get
            {
                return _tcpClientImpl.SendTimeout;
            }
            set
            {
                _tcpClientImpl.SendTimeout = value;
            }
        }

        public int ReceiveTimeout
        {
            get
            {
                return _tcpClientImpl.ReceiveTimeout;
            }
            set
            {
                _tcpClientImpl.ReceiveTimeout = value;
            }
        }

        public void Connect(string hostName, int port)
        {
            _tcpClientImpl.Connect(hostName, port);
        }

        public void Close()
        {
            _tcpClientImpl.Close();
        }

        public Stream GetStream()
        {
            return _tcpClientImpl.GetStream();
        }

        public Stream GetOutputStream()
        {
            return _tcpClientImpl.GetOutputStream();
        }

        public void WriteToOutputStream(byte[] bytes)
        {
            _tcpClientImpl.WriteToOutputStream(bytes);
        }

	}
}

#endif

