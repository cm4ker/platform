using System;
using System.IO;
using System.Linq;
using dnlib.DotNet;
using MoreLinq;

namespace ZenPlatform.ServerRPC
{
    public static class Test
    {
        public static void Run()
        {
            Obj1 s = new Obj1(new Dto());
            //Transfering
            Obj2 o = null;
        }
    }

    public interface IPlatformObject
    {
    }

    /*
     
      class dto
      {
        public string Test; 
        public int Int;
      }
       => 
      Test:
      count = read 8 (int)
      string = read count (string) 
      
      Int: 
      int = read 8 (int)
      
      
     */

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
        PlatformObject = 8
    }

    public class PlatformSerializer
    {
        public void Serialize(Stream stream, object obj)
        {
            if (!stream.CanWrite) throw new Exception();

            BinaryWriter bw = new BinaryWriter(stream);

            if (obj is IPlatformObject)
            {
            }

            var type = obj.GetType();

            var props = type.GetProperties();
            var ordered = props.OrderBy(x => x.Name).ToList();

            foreach (var pi in ordered)
            {
                var value = pi.GetMethod.Invoke(obj, null);

                if (value is IPlatformObject)
                {
                    Serialize(stream, value);
                }

                if (value is string s)
                {
                    bw.Write((int) ValType.String);
                    bw.Write(s.Length);
                    bw.Write(s);
                }
                else if (value is int i)
                {
                    bw.Write((int) ValType.Int);
                    
                }
            }
        }

        public object Deserialize()
        {
            return null;
        }
    }

    public class Dto
    {
        public byte Type { get; set; } = 1;

        public string S { get; set; } = "Test";

        public int I { get; set; } = 12345;
    }

    public class Obj1
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
            }
        }

        public void Save()
        {
            //We can save it
        }
    }

    public class Obj2
    {
        private readonly Dto _dto;

        public Obj2(Dto dto)
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
            }
        }
    }
}