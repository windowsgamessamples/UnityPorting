using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

#if NETFX_CORE || WINDOWS_PHONE
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
#else
using System.Net.Sockets;
#endif


namespace LegacySystem.Net.Sockets
{

    public class TcpClient
    {
#if NETFX_CORE || WINDOWS_PHONE
        private StreamSocket _socket = null;
        DataWriter _writer;

        private async Task EnsureSocket(string hostName, int port)
        {
            try
            {
                var host = new HostName(hostName);
                _socket = new StreamSocket();
                await _socket.ConnectAsync(host, port.ToString(), SocketProtectionLevel.SslAllowNullEncryption);
            }
            catch (Exception ex)
            {
                // If this is an unknown status it means that the error is fatal and retry will likely fail.
                if (SocketError.GetStatus(ex.HResult) == SocketErrorStatus.Unknown)
                {
                    // TODO abort any retry attempts on Unity side
                    throw;
                }
            }
        }

        private async Task WriteToOutputStreamAsync(byte[] bytes)
        {

            if (_socket == null) return;
            _writer = new DataWriter(_socket.OutputStream);
            _writer.WriteBytes(bytes);

            var debugString = UTF8Encoding.UTF8.GetString(bytes, 0, bytes.Length);

            try
            {
                await _writer.StoreAsync();
                await _socket.OutputStream.FlushAsync();

                _writer.DetachStream();
                _writer.Dispose();
            }
            catch (Exception exception)
            {
                // If this is an unknown status it means that the error if fatal and retry will likely fail.
                if (SocketError.GetStatus(exception.HResult) == SocketErrorStatus.Unknown)
                {
                    // TODO abort any retry attempts on Unity side
                    throw;
                }
            }
        }
#endif

        public int SendTimeout { get; set; }
        public int ReceiveTimeout { get; set; }

        public void Connect(string hostName, int port)
        {
#if NETFX_CORE
            var thread = EnsureSocket(hostName, port);
            thread.Wait();
#else
            throw new NotImplementedException();
#endif
        }

        public Stream GetStream()
        {
#if NETFX_CORE || WINDOWS_PHONE
            if (_socket == null) return null;
            return _socket.InputStream.AsStreamForRead();
#else
            throw new NotImplementedException();
#endif
        }

        public Stream GetOutputStream()
        {
#if NETFX_CORE || WINDOWS_PHONE
            if (_socket == null) return null;
            return _socket.OutputStream.AsStreamForWrite();
#else
            throw new NotImplementedException();
#endif
        }

        public void Close()
        {
#if NETFX_CORE || WINDOWS_PHONE
            if (_socket != null)
            {
                _socket.Dispose();
            }
#else
            throw new NotImplementedException();
#endif
        }

        public void WriteToOutputStream(byte[] bytes)
        {
#if NETFX_CORE || WINDOWS_PHONE
            var thread = WriteToOutputStreamAsync(bytes);
            thread.Wait();
#else
            throw new NotImplementedException();
#endif
        }


    }
}
