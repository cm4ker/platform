using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace BufferedDataReaderDotNet.Extensions
{
    internal static class BinaryWriterExtensions
    {
        public static void WriteBytes(this BinaryWriter binaryWriter, byte[] bytes)
        {
            binaryWriter.Write(bytes.Length);
            binaryWriter.Write(bytes);
        }

        public static void WriteDataTable(this BinaryWriter binaryWriter, DataTable dataTable)
        {
            using (var memoryStream = new MemoryStream())
            {
                dataTable.WriteXml(memoryStream, XmlWriteMode.WriteSchema, true);
                binaryWriter.WriteBytes(memoryStream.ToArray());
            }
        }

        public static void WriteDateTime(this BinaryWriter binaryWriter, DateTime dateTime)
        {
            binaryWriter.Write(dateTime.ToBinary());
        }

        public static void WriteGuid(this BinaryWriter binaryWriter, Guid guid)
        {
            binaryWriter.Write(guid.ToByteArray());
        }

        public static void WriteStrings(this BinaryWriter binaryWriter, IReadOnlyCollection<string> strings)
        {
            binaryWriter.Write(strings.Count);

            foreach (var @string in strings)
                binaryWriter.Write(@string);
        }

        public static void WriteTimeSpan(this BinaryWriter binaryWriter, TimeSpan timeSpan)
        {
            binaryWriter.Write(timeSpan.Ticks);
        }

        public static void WriteTypes(this BinaryWriter binaryWriter, IReadOnlyCollection<Type> types)
        {
            binaryWriter.Write(types.Count);

            foreach (var type in types)
            {
                // ReSharper disable once AssignNullToNotNullAttribute
                binaryWriter.Write(type.AssemblyQualifiedName);
            }
        }
    }
}