using Xunit;

namespace Aquila.Core.Querying.Test
{
    public class TranspilingTest
    {
        private Querying.Model.QLang _m;

        public TranspilingTest()
        {
            _m = new Model.QLang(null);
        }

        [Fact]
        public void SimpleTranspilingTest()
        {
            _m.reset();

            _m.new_scope();

            _m.from();

            _m.ld_component("Entity");
            _m.ld_object_type("Invoice");
            _m.@as("A");

            _m.ld_component("Entity");
            _m.ld_object_type("Store");
            _m.@as("B");

            _m.ld_name("A");
            _m.ld_field("Store");

            _m.ld_name("B");
            _m.ld_field("Id");

            _m.eq();

            _m.join();

            _m.select();

            _m.ld_name("A");
            _m.ld_field("Store");

            _m.new_query();

            var actual = _m.Compile(null, null);

            Assert.Equal(
                "SELECT T4.Fld_0002_Ref,\nT4.Fld_0002_Type\nFROM\nObj_0002 as T4\nJOIN Obj_0001 as T2 ON T4.Fld_0002_Type = 4 and NULL = T2.Id\n",
                actual);
        }
    }
}