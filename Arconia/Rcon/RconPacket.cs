using System;
using System.Runtime.InteropServices;
using System.Buffers.Binary;

namespace Arconia.Rcon
{
    public enum RconPacketType
    {
        Login = 3,
        Command = 2,
        Response = 0
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
            Payload = payload;
        }

        public int Length()
        {
            return 12 + System.Text.Encoding.ASCII.GetByteCount(Payload) + 2;
        }

        public byte[] GetBytes()
        {
            byte[] array = new byte[Length()];
            Span<byte> packet = array;
            BinaryPrimitives.WriteInt32LittleEndian(packet, Length() - 4);
            BinaryPrimitives.WriteInt32LittleEndian(packet.Slice(4, 4), Id);
            BinaryPrimitives.WriteInt32LittleEndian(packet.Slice(8, 4), (int)Type);
            System.Text.Encoding.Latin1.GetBytes(Payload).AsSpan().CopyTo(packet.Slice(12));

            return array;
        }
    }
}