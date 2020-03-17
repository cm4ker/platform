using System;
using dnlib.DotNet;

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