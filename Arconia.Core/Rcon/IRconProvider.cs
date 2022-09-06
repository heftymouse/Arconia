using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arconia.Core.Rcon
{
    public interface IRconProvider : IDisposable
    {
        public Task Connect(string host, int port, string password);
        public Task SendPacketAsync(int id, string payload);
        public Task<RconPacket> GetNextPacketAsync(CancellationToken? token = null);
    }
}
