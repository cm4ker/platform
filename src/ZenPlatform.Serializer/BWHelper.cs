using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ZenPlatform.Serializer
{
    public static class BWHelper
    {
        public static void Write(this BinaryWriter bw, DateTime dateTime)
        {
            ulong ser = (ulong)(dateTime.Ticks | ((long)dateTime.Kind) << 62);
            bw.Write(ser);
        }

        public static DateTime ReadDateTime(this BinaryReader rd)
        {
            var deserialisedData = rd.ReadUInt64();
            var ticks = (long)(deserialisedData & 0x3FFFFFFFFFFFFFFF);
            DateTimeKind kind = (DateTimeKind)(deserialisedData >> 62);
            return new DateTime(ticks, kind);
        }

        public static void Write(this BinaryWriter w, Guid g)
        {
            w.WriteA(g.ToByteArray());
        }

        public static void WriteA(this BinaryWriter w, byte[] array)
        {
            w.Write(array.Length);
            w.Write(array);
        }

        public static byte[] ReadByteArray(this BinaryReader rd)
        {
            var count = rd.ReadInt32();
            return rd.ReadBytes(count);
        }

        public static Guid ReadGuid(this BinaryReader rd)
        {
            return new Guid(rd.ReadByteArray());
        }
    }

}
