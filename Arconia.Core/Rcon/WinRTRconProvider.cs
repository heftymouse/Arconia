using System;
using System.Buffers.Binary;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Windows.Networking;
using Windows.Networking.Sockets;
using System.Diagnostics;

namespace Arconia.Core.Rcon
{
    public class WinRTRconProvider : IRconProvider
    {
        private StreamSocket rconSocket;
        private bool disposedValue;

        public WinRTRconProvider()
        {
            rconSocket = new();
        }

        public async Task Connect(string ip, int port, string password)
        {
            await rconSocket.ConnectAsync(new HostName(ip), port.ToString());
            int id = new Random().Next(255);
            await SendPacketAsync(id, password, RconPacketType.Login);

            RconPacket loginResponse = await GetNextPacketAsync();

            if (!(loginResponse.Id == id))
            {
                throw new RconAuthException();
            }
        }

        public async Task SendPacketAsync(int id, string payload, RconPacketType type)
        {
            var outStream = rconSocket.OutputStream.AsStreamForWrite();
            RconPacket sendPacket = new(id, type, payload);
            await outStream.WriteAsync(sendPacket.GetBytes(), 0, sendPacket.Length);
            await outStream.FlushAsync();
        }

        public async Task SendPacketAsync(int id, string payload)
        {
            await SendPacketAsync(id, payload, RconPacketType.Command);
        }

        public async Task<RconPacket> GetNextPacketAsync(CancellationToken? token = null)
        {
            var inStream = rconSocket.InputStream.AsStreamForRead();
            byte[] lenBuf = new byte[4];
            await inStream.ReadAsync(lenBuf, 0, 4, token ?? CancellationToken.None);
            int length = BinaryPrimitives.ReadInt32LittleEndian(lenBuf);

            byte[] data = new byte[length];
            await inStream.ReadAsync(data, 0, length, token ?? CancellationToken.None);

            RconPacket CreatePacket(byte[] bytes)
            {
                Span<byte> buffer = new Span<byte>(bytes);
                return new(
                        BinaryPrimitives.ReadInt32LittleEndian(buffer.Slice(0, 4)),
                        Enum.IsDefined(typeof(RconPacketType), BinaryPrimitives.ReadInt32LittleEndian(buffer.Slice(4, 4))) ? (RconPacketType)BinaryPrimitives.ReadInt32LittleEndian(buffer.Slice(4, 4)) : RconPacketType.Unknown,
                        System.Text.Encoding.Latin1.GetString(buffer[8..^2])
                    );
            }

            return CreatePacket(data);
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
        ~WinRTRconProvider()
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
