using System;
using System.IO;
using System.Linq;

namespace ZenPlatform.ServerRPC
{
    public static class Test
    {
        public static void Run()
        {
            Obj1 s = new Obj1(new Dto());

            s.Value = Guid.NewGuid();

            var ms = new MemoryStream();
            PlatformSerializer.Serialize(ms, s);
            //Transfering
            ms.Seek(0, SeekOrigin.Begin);

            Obj1 o = (Obj1) PlatformSerializer.Deserialize(ms);
        }
    }

    public interface IDtoObject
    {
        object GetDto();
    }

    public enum ValType
    {
        Unknown = 0,
        String = 1,
        Int = 2,
        Char = 3,
        Boolean = 4,
        Long = 5,
        ByteArray = 6,
        Datetime = 7,
        DtoObject = 8
    }

    public static class BWHelper
    {
        public static void Write(this BinaryWriter bw, DateTime dateTime)
        {
            ulong ser = (ulong) (dateTime.Ticks | ((long) dateTime.Kind) << 62);
            bw.Write(ser);
        }

        public static DateTime ReadDateTime(this BinaryReader rd)
        {
            var deserialisedData = rd.ReadUInt64();
            var ticks = (long) (deserialisedData & 0x3FFFFFFFFFFFFFFF);
            DateTimeKind kind = (DateTimeKind) (deserialisedData >> 62);
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

    public static class PlatformSerializer
    {
        public static void Serialize(Stream stream, object obj)
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

        public static object Deserialize(Stream stream)
        {
            BinaryReader reader = new BinaryReader(stream);

            var val = (ValType) reader.ReadInt32();

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

                    break;

                default:
                    throw new NotImplementedException();
            }

            return null;
        }
    }

    public class Dto
    {
        public byte Type { get; set; } = 1;

        public string S { get; set; } = "Test";

        public int I { get; set; } = 12345;

        public DateTime D { get; set; } = DateTime.Today;

        public Guid G { get; set; } = Guid.Empty;
    }

    public class Obj1 : IDtoObject
    {
        private readonly Dto _dto;

        public Obj1(Dto dto)
        {
            _dto = dto;
        }

        public object Value
        {
            get
            {
                switch (_dto.Type)
                {
                    case 0: return _dto.I;
                    case 1: return _dto.S;
                    case 2: return _dto.D;
                    case 3: return _dto.G;
                    default: throw new Exception("Unknown type");
                }
            }
            set
            {
                if (value is string s)
                {
                    _dto.Type = 1;
                    _dto.S = s;
                }
                else if (value is int i)
                {
                    _dto.Type = 0;
                    _dto.I = i;
                }
                else if (value is DateTime d)
                {
                    _dto.Type = 2;
                    _dto.D = d;
                }
                else if (value is Guid g)
                {
                    _dto.Type = 3;
                    _dto.G = g;
                }
            }
        }

        public void Save()
        {
            //We can save it
        }

        public object GetDto()
        {
            return _dto;
        }
    }
}