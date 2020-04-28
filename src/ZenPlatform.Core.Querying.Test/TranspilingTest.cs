using ZenPlatform.Configuration.Structure;
using ZenPlatform.Test.Tools;

namespace ZenPlatform.Core.Querying.Test
{
    public class TranspilingTest
    {
        private Project conf;
        private Querying.Model.QLang _m;

        public TranspilingTest()
        {
            conf = ConfigurationFactory.Create();
            _m = new Querying.Model.QLang(conf.TypeManager);
        }

        // [Fact]
        // public void SimpleTranspilingTest()
        // {
        //     _m.reset();
        //
        //     _m.new_scope();
        //
        //     _m.m_from();
        //
        //     _m.ld_component("Entity");
        //     _m.ld_object_type("Invoice");
        //     _m.@as("A");
        //
        //     _m.ld_component("Entity");
        //     _m.ld_object_type("Store");
        //     _m.@as("B");
        //
        //     _m.ld_name("A");
        //     _m.ld_field("Store");
        //
        //     _m.ld_name("B");
        //     _m.ld_field("Id");
        //
        //     _m.eq();
        //
        //     _m.join();
        //
        //     _m.m_select();
        //
        //     _m.ld_name("A");
        //     _m.ld_field("Store");
        //
        //     _m.new_query();
        //
        //     var actual = _m.Compile(true);
        //
        //     Assert.Equal("SELECT T4.Fld_0002_Ref,\nT4.Fld_0002_Type\nFROM\nObj_0002 as T4\nJOIN Obj_0001 as T2 ON T4.Fld_0002_Type = 4 and NULL = T2.Id\n", actual);
        // }
    }
}