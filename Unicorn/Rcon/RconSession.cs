using System;
using System.Buffers.Binary;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Windows.Networking;
using Windows.Networking.Sockets;
using System.Diagnostics;

namespace Arconia.Rcon
{
    internal class RconSession : IDisposable
    {
        StreamSocket rconSocket;

        string ServerIP;
        int Port;
        private bool disposedValue;

        public event EventHandler<RconPacket> PacketReceived;

        async public Task Connect(string ip, int port, string password)
        {
            int id = new Random().Next(255);
            rconSocket = new();
            await rconSocket.ConnectAsync(new HostName(ip), port.ToString());
            var outStream = rconSocket.OutputStream.AsStreamForWrite();
            RconPacket packet = new(id, RconPacketType.Login, password);
            await outStream.WriteAsync(packet.GetBytes(), 0, packet.Length());
            await outStream.FlushAsync();

            var inStream = rconSocket.InputStream.AsStreamForRead();
            int length;
            byte[] lenBuf = new byte[4];
            await inStream.ReadAsync(lenBuf, 0, 4);
            length = BinaryPrimitives.ReadInt32LittleEndian(lenBuf);
            byte[] data = new byte[length];
            await inStream.ReadAsync(data, 0, length);
            
            if (!(data[0] == id && data[4] == 2))
            {
                throw new RconAuthException();
            }
            
            ServerIP = ip;
            Port = port; 
        }

        public async Task SendPacketAsync(int id, string payload)
        {
            var outStream = rconSocket.OutputStream.AsStreamForWrite();
            RconPacket sendPacket = new(id, RconPacketType.Command, payload);
            await outStream.WriteAsync(sendPacket.GetBytes(), 0, sendPacket.Length());
            await outStream.FlushAsync();
        }

        public async Task GetNextPacketAsync(CancellationToken token)
        {
            while(!token.IsCancellationRequested)
            {
                var inStream = rconSocket.InputStream.AsStreamForRead();
                byte[] lenBuf = new byte[4];
                await inStream.ReadAsync(lenBuf, 0, 4, token);
                int length = BinaryPrimitives.ReadInt32LittleEndian(lenBuf);

                byte[] data = new byte[length];
                await inStream.ReadAsync(data, 0, length);

                RconPacket CreatePacket(byte[] bytes)
                {
                    Span<byte> buffer = new Span<byte>(bytes);
                    return new(
                            BinaryPrimitives.ReadInt32LittleEndian(buffer.Slice(0, 4)),
                            RconPacketType.Response,
                            System.Text.Encoding.ASCII.GetString(buffer[8..^2])
                        );
                }

                EventHandler<RconPacket> receivedEvent = PacketReceived;

                if (receivedEvent != null)
                {
                    receivedEvent(this, CreatePacket(data));
                }
            }

            token.ThrowIfCancellationRequested();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                rconSocket.Dispose();
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        ~RconSession()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
