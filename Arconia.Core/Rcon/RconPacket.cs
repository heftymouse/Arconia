using System;
using System.Buffers.Binary;
using System.Text;

namespace Arconia.Core.Rcon
{
    public enum RconPacketType
    {
        Login = 3,
        Command = 2,
        Response = 0,
        Unknown = -1
    }

    public readonly record struct RconPacket
    {
        public int Id { get; init; }

        public RconPacketType Type { get; init; }

        public string Payload { get; init; }

        public RconPacket(int id, RconPacketType type, string payload)
        {
            Id = id;
            Type = type;
            Payload = payload.Trim();
        }

        public int Length
        {
            get
            {
                return 12 + Encoding.Latin1.GetByteCount(Payload) + 2;
            }
        }

        public byte[] GetBytes()
        {
            byte[] array = new byte[Length];
            Span<byte> packet = array;
            BinaryPrimitives.WriteInt32LittleEndian(packet, Length - 4);
            BinaryPrimitives.WriteInt32LittleEndian(packet.Slice(4, 4), Id);
            BinaryPrimitives.WriteInt32LittleEndian(packet.Slice(8, 4), (int)Type);
            Encoding.Latin1.GetBytes(Payload).AsSpan().CopyTo(packet.Slice(12));

            return array;
        }
    }
}