#if UNITY_METRO && !UNITY_EDITOR

using System;
using System.IO;
using System.Net;

namespace System.Net.Sockets
{

    public interface ITcpClient
    {
        int SendTimeout { get; set; }
        int ReceiveTimeout { get; set; }
        void Connect(string hostName, int port);
        void Connect(IPAddress ipAddress, int port);
        void Close();
        Stream GetStream();
        Stream GetOutputStream();
        void WriteToOutputStream(byte[] bytes);
    }

    public class TcpClient
	{
        public static Func<ITcpClient> DoGetTcpClientImpl;

        private ITcpClient _tcpClientImpl;

        public TcpClient()
        {
            _tcpClientImpl = DoGetTcpClientImpl();
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

        public void Connect(IPAddress ipAddress, int port)
        {
            _tcpClientImpl.Connect(ipAddress, port);
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

