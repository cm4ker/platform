using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace BufferedDataReaderDotNet.Extensions
{
    internal static class BinaryReaderExtensions
    {
        public static byte[] ReadBytes(this BinaryReader binaryReader)
        {
            return binaryReader.ReadBytes(binaryReader.ReadInt32());
        }

        public static DataTable ReadDataTable(this BinaryReader binaryReader)
        {
            var dataTable = new DataTable();

            try
            {
                using (var memoryStream = new MemoryStream(binaryReader.ReadBytes()))
                    dataTable.ReadXml(memoryStream);

                return dataTable;
            }
            catch
            {
                dataTable.Dispose();

                throw;
            }
        }

        public static DateTime ReadDateTime(this BinaryReader binaryReader)
        {
            return DateTime.FromBinary(binaryReader.ReadInt64());
        }

        // ReSharper disable InconsistentNaming
        public static List<long> ReadInt64s(this BinaryReader binaryReader)
        {
            var int64Count = binaryReader.ReadInt32();
            var int64s = new List<long>(int64Count);

            while (int64Count-- > 0)
                int64s.Add(binaryReader.ReadInt64());

            return int64s;
        }
        // ReSharper restore InconsistentNaming

        public static Guid ReadGuid(this BinaryReader binaryReader)
        {
            return new Guid(binaryReader.ReadBytes(16));
        }

        public static List<string> ReadStrings(this BinaryReader binaryReader)
        {
            var stringCount = binaryReader.ReadInt32();
            var strings = new List<string>(stringCount);

            while (stringCount-- > 0)
                strings.Add(binaryReader.ReadString());

            return strings;
        }

        public static TimeSpan ReadTimeSpan(this BinaryReader binaryReader)
        {
            return TimeSpan.FromTicks(binaryReader.ReadInt64());
        }

        public static List<Type> ReadTypes(this BinaryReader binaryReader)
        {
            var typeCount = binaryReader.ReadInt32();
            var types = new List<Type>(typeCount);

            while (typeCount-- > 0)
            {
                var assemblyQualifiedName = binaryReader.ReadString();
                var type = Type.GetType(assemblyQualifiedName);

                types.Add(type);
            }

            return types;
        }
    }
}