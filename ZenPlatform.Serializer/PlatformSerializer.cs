using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
                bw.Write((int)ValType.DtoObject);

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


        public object Deserialize(Stream stream)
        {
            BinaryReader reader = new BinaryReader(stream);

            var val = (ValType)reader.ReadInt32();

            switch (val)
            {
                case ValType.DtoObject:
                    var objectName = reader.ReadString();
                    var dtoName = reader.ReadString();


                    var objectType = Type.GetType(objectName) ?? throw new Exception($"Unknown type {objectName}");
                    var dtoType = Type.GetType(dtoName) ?? throw new Exception($"Unknown type {dtoName}");

                    var dto = Activator.CreateInstance(dtoType);

                    var props = dtoType.GetProperties();
                    var ordered = props.OrderBy(x => x.Name).ToList();

                    foreach (var pi in ordered)
                    {
                        if (pi.PropertyType == typeof(string))
                            pi.SetMethod.Invoke(dto, new object[] { reader.ReadString() });
                        if (pi.PropertyType == typeof(int))
                            pi.SetMethod.Invoke(dto, new object[] { reader.ReadInt32() });
                        if (pi.PropertyType == typeof(byte))
                            pi.SetMethod.Invoke(dto, new object[] { reader.ReadByte() });
                        if (pi.PropertyType == typeof(DateTime))
                            pi.SetMethod.Invoke(dto, new object[] { reader.ReadDateTime() });
                        if (pi.PropertyType == typeof(Guid))
                            pi.SetMethod.Invoke(dto, new object[] { reader.ReadGuid() });
                        if (pi.PropertyType == typeof(byte[]))
                            pi.SetMethod.Invoke(dto, new object[] { reader.ReadByteArray() });
                    }

                    return Activator.CreateInstance(objectType, dto);


                default:
                    throw new NotImplementedException();
            }

            throw new NotImplementedException();
        }


        public object Deserialize(byte[] bytes)
        {
            return this.Deserialize(new MemoryStream(bytes));
        }


        public object[] Deserialize(byte[][] data)
        {
            var list = new List<object>();
            foreach (var bytes in data)
                list.Add(this.Deserialize(new MemoryStream(bytes)));

            return list.ToArray();
        }

    }
}
