using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Portable.Xaml;
using ZenPlatform.Avalonia.Wrapper;

namespace ZenPlatform.Serializer
{
    public class PlatformSerializer
    {
        public void Serialize(Stream stream, object obj)
        {
            if (!stream.CanWrite) throw new Exception();

            BinaryWriter bw = new BinaryWriter(stream);

            if (obj is IDtoObject c)
            {
                bw.Write((int) ValType.DtoObject);

                var dto = c.GetDto();

                var dtoType = dto.GetType();
                var objectType = obj.GetType();

                bw.Write(objectType.FullName); //we need set information what we need to restore on the another side
                bw.Write(dtoType.FullName);

                var props = dtoType.GetProperties();
                var ordered = props.OrderBy(x => x.Name).ToList();

                foreach (var pi in ordered)
                {
                    var value = pi.GetMethod.Invoke(dto, null);

                    if (value == null && pi.PropertyType == typeof(byte[]))
                        value = new byte[0];

                    if (value is string s)
                        bw.Write(s);
                    else if (value is int i)
                        bw.Write(i);
                    else if (value is byte b)
                        bw.Write(b);
                    else if (value is DateTime dt)
                        bw.Write(dt);
                    else if (value is Guid g)
                        bw.Write(g);
                    else if (value is byte[] ba)
                        bw.WriteA(ba);
                }
            }

            if (obj is UXElement)
            {
                bw.Write((int) ValType.UXObject);
                bw.Write(XamlServices.Save(obj));
            }
            else if (obj is int i)
            {
                bw.Write((int) ValType.Int);
                bw.Write(i);
            }
            else if (obj is string s)
            {
                bw.Write((int) ValType.String);
                bw.Write(s);
            }
        }

        public byte[] Serialize(object obj)
        {
            var stream = new MemoryStream();

            Serialize(stream, obj);

            stream.Seek(0, SeekOrigin.Begin);

            return stream.ToArray();
        }

        public byte[][] Serialize(object[] objs)
        {
            var list = new List<byte[]>();

            foreach (var obj in objs)
                list.Add(Serialize(obj));

            return list.ToArray();
        }


        public object Deserialize(Stream stream, bool isClient)
        {
            BinaryReader reader = new BinaryReader(stream);

            var val = (ValType) reader.ReadInt32();

            switch (val)
            {
                case ValType.DtoObject:
                    var objectName = reader.ReadString();
                    var dtoName = reader.ReadString();


                    //TODO Remove this ugly hack and provide clear 
                    Assembly platformAsm;

                    if (!isClient)
                        platformAsm = AppDomain.CurrentDomain.GetAssemblies()
                            .FirstOrDefault(x => x.GetName().Name == "LibraryServer");
                    else
                        platformAsm = AppDomain.CurrentDomain.GetAssemblies()
                            .FirstOrDefault(x => x.GetName().Name == "LibraryClient");


                    var objectType = platformAsm.GetType(objectName) ??
                                     throw new Exception($"Unknown type {objectName}");
                    var dtoType = platformAsm.GetType(dtoName) ?? throw new Exception($"Unknown type {dtoName}");

                    var dto = Activator.CreateInstance(dtoType);

                    var props = dtoType.GetProperties();
                    var ordered = props.OrderBy(x => x.Name).ToList();

                    foreach (var pi in ordered)
                    {
                        if (pi.PropertyType == typeof(string))
                            pi.SetMethod.Invoke(dto, new object[] {reader.ReadString()});
                        if (pi.PropertyType == typeof(int))
                            pi.SetMethod.Invoke(dto, new object[] {reader.ReadInt32()});
                        if (pi.PropertyType == typeof(byte))
                            pi.SetMethod.Invoke(dto, new object[] {reader.ReadByte()});
                        if (pi.PropertyType == typeof(DateTime))
                            pi.SetMethod.Invoke(dto, new object[] {reader.ReadDateTime()});
                        if (pi.PropertyType == typeof(Guid))
                            pi.SetMethod.Invoke(dto, new object[] {reader.ReadGuid()});
                        if (pi.PropertyType == typeof(byte[]))
                            pi.SetMethod.Invoke(dto, new object[] {reader.ReadByteArray()});
                    }

                    return Activator.CreateInstance(objectType, dto);
                case ValType.UXObject:
                    var xaml = reader.ReadString();
                    return XamlServices.Parse(xaml);
                case ValType.Int:
                    return reader.ReadInt32();
                case ValType.String:
                    return reader.ReadString();
                default:
                    throw new NotImplementedException();
            }

            throw new NotImplementedException();
        }


        public object Deserialize(byte[] bytes, bool isClient)
        {
            return this.Deserialize(new MemoryStream(bytes), isClient);
        }


        public object[] Deserialize(byte[][] data, bool isClient)
        {
            var list = new List<object>();
            foreach (var bytes in data)
                list.Add(this.Deserialize(new MemoryStream(bytes), isClient));

            return list.ToArray();
        }
    }
}