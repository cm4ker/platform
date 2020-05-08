using System;
using System.Collections.Generic;
using System.Linq;

namespace Aquila.QueryBuilder
{

    public enum MachineContextType
    {
        None = 0,
        Select,
        From,
        Where,
        OrderBy,
        GroupBy,
        Having,
        Insert,
        Values,
        Update,
        Set,
        Delete


    }



    public class MachineContext
    {
        private MachineContextType _type;
        public MachineContextType Type {
            get
            {
                return _type;
            }
            set
            {
                _type = value;
            }
        }

        public MachineContext()
        {
            Type = MachineContextType.None;
        }


    }
}
