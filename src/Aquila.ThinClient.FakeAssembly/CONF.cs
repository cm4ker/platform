public abstract class EntryPoint : System.Object
{
    static Aquila.Core.Contracts.Network.IInvokeService _is;
    static Aquila.Core.Contracts.ILinkFactory _lf;
    public static void Main(System.Object[] args)
    {
        EntryPoint._is = ((Aquila.Core.Contracts.Network.IInvokeService)args[0]);
        EntryPoint._lf = ((Aquila.Core.Contracts.ILinkFactory)args[1]);
        EntryPoint._is.Register(new Aquila.Core.Contracts.Route("__cmd_HelloFromServer.ClientCallProc"), dlgt_ClientCallProc);
        EntryPoint._is.Register(new Aquila.Core.Contracts.Route("__cmd_HelloFromServer.CreateAndSaveStore"), dlgt_CreateAndSaveStore);
        EntryPoint._is.Register(new Aquila.Core.Contracts.Route("__cmd_HelloFromServer.GetUserNameServer"), dlgt_GetUserNameServer);
        EntryPoint._is.Register(new Aquila.Core.Contracts.Route("__cmd_HelloFromServer.GetStore"), dlgt_GetStore);
        EntryPoint._is.Register(new Aquila.Core.Contracts.Route("__cmd_HelloFromServer.UpdateName"), dlgt_UpdateName);
        EntryPoint._is.Register(new Aquila.Core.Contracts.Route("UX.Editor"), dlgt_Editor);
        EntryPoint._lf.Register(110, fac_StoreLink);
    }

    public static System.Object dlgt_ClientCallProc(Aquila.Core.Contracts.Network.InvokeContext context, System.Object[] args)
    {
        return Entity.__cmd_HelloFromServer.ClientCallProc(((System.Int32)args[0]));
    }

    public static System.Object dlgt_CreateAndSaveStore(Aquila.Core.Contracts.Network.InvokeContext context, System.Object[] args)
    {
        Entity.__cmd_HelloFromServer.CreateAndSaveStore();
        return null;
    }

    public static System.Object dlgt_GetUserNameServer(Aquila.Core.Contracts.Network.InvokeContext context, System.Object[] args)
    {
        return Entity.__cmd_HelloFromServer.GetUserNameServer();
    }

    public static System.Object dlgt_GetStore(Aquila.Core.Contracts.Network.InvokeContext context, System.Object[] args)
    {
        return Entity.__cmd_HelloFromServer.GetStore();
    }

    public static System.Object dlgt_UpdateName(Aquila.Core.Contracts.Network.InvokeContext context, System.Object[] args)
    {
        return Entity.__cmd_HelloFromServer.UpdateName(((Entity.Store)args[0]));
    }

    public static System.Object dlgt_Editor(Aquila.Core.Contracts.Network.InvokeContext context, System.Object[] args)
    {
        return Entity.__StoreEditorForm.Get();
    }

    public static Aquila.Core.Contracts.ILink fac_StoreLink(System.Guid p1, System.String p2)
    {
        return new Entity.StoreLink(p1, p2);
    }
}

namespace Entity
{
    public class EntityLink : System.Object, Aquila.Core.Contracts.ILink
    {
        System.Guid ik__BackingField;
        System.Int32 tk__BackingField;
        System.String pk__BackingField;
        public EntityLink(System.Guid A_1, System.Int32 A_2, System.String A_3): base()
        {
            this.ik__BackingField = A_1;
            this.tk__BackingField = A_2;
            this.pk__BackingField = A_3;
        }

        public System.Guid Id
        {
            get => __get_Id();
        }

        public System.Int32 TypeId
        {
            get => __get_TypeId();
        }

        public System.String Presentation
        {
            get => __get_Presentation();
        }

        public virtual System.Guid __get_Id()
        {
            return this.ik__BackingField;
        }

        public virtual System.Int32 __get_TypeId()
        {
            return this.tk__BackingField;
        }

        public virtual System.String __get_Presentation()
        {
            return this.pk__BackingField;
        }
    }
}

namespace Entity
{
    public class RowDto_Store_ExampleTable : System.Object
    {
        System.Int32 TProp1_Typek__BackingField;
        System.String TProp1_Stringk__BackingField;
        System.Guid TProp1_Refk__BackingField;
        public RowDto_Store_ExampleTable(): base()
        {
        }

        [Aquila.EntityComponent.Entity.MapToAttribute("Fld_105_Type")]
        public System.Int32 TProp1_Type
        {
            get => __get_TProp1_Type();
            set => __set_TProp1_Type(value);
        }

        [Aquila.EntityComponent.Entity.MapToAttribute("Fld_105_String")]
        [Aquila.EntityComponent.Entity.NeedInitAttribute]
        public System.String TProp1_String
        {
            get => __get_TProp1_String();
            set => __set_TProp1_String(value);
        }

        [Aquila.EntityComponent.Entity.MapToAttribute("Fld_105_Ref")]
        [Aquila.EntityComponent.Entity.NeedInitAttribute]
        public System.Guid TProp1_Ref
        {
            get => __get_TProp1_Ref();
            set => __set_TProp1_Ref(value);
        }

        public System.Int32 __get_TProp1_Type()
        {
            return this.TProp1_Typek__BackingField;
        }

        public void __set_TProp1_Type(System.Int32 value)
        {
            this.TProp1_Typek__BackingField = value;
        }

        public System.String __get_TProp1_String()
        {
            return this.TProp1_Stringk__BackingField;
        }

        public void __set_TProp1_String(System.String value)
        {
            this.TProp1_Stringk__BackingField = value;
        }

        public System.Guid __get_TProp1_Ref()
        {
            return this.TProp1_Refk__BackingField;
        }

        public void __set_TProp1_Ref(System.Guid value)
        {
            this.TProp1_Refk__BackingField = value;
        }
    }
}

namespace Entity
{
    public class _Store : System.Object, Aquila.Compiler.Contracts.ICanMap
    {
        System.Guid Idk__BackingField;
        System.String Namek__BackingField;
        System.Int32 Property1_Typek__BackingField;
        System.DateTime Property1_DateTimek__BackingField;
        System.Int32 Property1_Intk__BackingField;
        System.Boolean Property1_Booleank__BackingField;
        System.Double Property1_Numerick__BackingField;
        System.Guid Property1_Refk__BackingField;
        System.Collections.Generic.List<Entity.RowDto_Store_ExampleTable> ExampleTablek__BackingField;
        System.Byte[] Versionk__BackingField;
        public _Store(): base()
        {
            this.ExampleTablek__BackingField = new System.Collections.Generic.List<Entity.RowDto_Store_ExampleTable>();
        }

        [Aquila.EntityComponent.Entity.MapToAttribute("Fld_101")]
        public System.Guid Id
        {
            get => __get_Id();
            set => __set_Id(value);
        }

        [Aquila.EntityComponent.Entity.MapToAttribute("Fld_102")]
        [Aquila.EntityComponent.Entity.NeedInitAttribute]
        public System.String Name
        {
            get => __get_Name();
            set => __set_Name(value);
        }

        [Aquila.EntityComponent.Entity.MapToAttribute("Fld_103_Type")]
        public System.Int32 Property1_Type
        {
            get => __get_Property1_Type();
            set => __set_Property1_Type(value);
        }

        [Aquila.EntityComponent.Entity.MapToAttribute("Fld_103_DateTime")]
        [Aquila.EntityComponent.Entity.NeedInitAttribute]
        public System.DateTime Property1_DateTime
        {
            get => __get_Property1_DateTime();
            set => __set_Property1_DateTime(value);
        }

        [Aquila.EntityComponent.Entity.MapToAttribute("Fld_103_Int")]
        [Aquila.EntityComponent.Entity.NeedInitAttribute]
        public System.Int32 Property1_Int
        {
            get => __get_Property1_Int();
            set => __set_Property1_Int(value);
        }

        [Aquila.EntityComponent.Entity.MapToAttribute("Fld_103_Boolean")]
        [Aquila.EntityComponent.Entity.NeedInitAttribute]
        public System.Boolean Property1_Boolean
        {
            get => __get_Property1_Boolean();
            set => __set_Property1_Boolean(value);
        }

        [Aquila.EntityComponent.Entity.MapToAttribute("Fld_103_Numeric")]
        [Aquila.EntityComponent.Entity.NeedInitAttribute]
        public System.Double Property1_Numeric
        {
            get => __get_Property1_Numeric();
            set => __set_Property1_Numeric(value);
        }

        [Aquila.EntityComponent.Entity.MapToAttribute("Fld_103_Ref")]
        [Aquila.EntityComponent.Entity.NeedInitAttribute]
        public System.Guid Property1_Ref
        {
            get => __get_Property1_Ref();
            set => __set_Property1_Ref(value);
        }

        public System.Collections.Generic.List<Entity.RowDto_Store_ExampleTable> ExampleTable
        {
            get => __get_ExampleTable();
        }

        public System.Byte[] Version
        {
            get => __get_Version();
            set => __set_Version(value);
        }

        public System.Guid __get_Id()
        {
            return this.Idk__BackingField;
        }

        public void __set_Id(System.Guid value)
        {
            this.Idk__BackingField = value;
        }

        public System.String __get_Name()
        {
            return this.Namek__BackingField;
        }

        public void __set_Name(System.String value)
        {
            this.Namek__BackingField = value;
        }

        public System.Int32 __get_Property1_Type()
        {
            return this.Property1_Typek__BackingField;
        }

        public void __set_Property1_Type(System.Int32 value)
        {
            this.Property1_Typek__BackingField = value;
        }

        public System.DateTime __get_Property1_DateTime()
        {
            return this.Property1_DateTimek__BackingField;
        }

        public void __set_Property1_DateTime(System.DateTime value)
        {
            this.Property1_DateTimek__BackingField = value;
        }

        public System.Int32 __get_Property1_Int()
        {
            return this.Property1_Intk__BackingField;
        }

        public void __set_Property1_Int(System.Int32 value)
        {
            this.Property1_Intk__BackingField = value;
        }

        public System.Boolean __get_Property1_Boolean()
        {
            return this.Property1_Booleank__BackingField;
        }

        public void __set_Property1_Boolean(System.Boolean value)
        {
            this.Property1_Booleank__BackingField = value;
        }

        public System.Double __get_Property1_Numeric()
        {
            return this.Property1_Numerick__BackingField;
        }

        public void __set_Property1_Numeric(System.Double value)
        {
            this.Property1_Numerick__BackingField = value;
        }

        public System.Guid __get_Property1_Ref()
        {
            return this.Property1_Refk__BackingField;
        }

        public void __set_Property1_Ref(System.Guid value)
        {
            this.Property1_Refk__BackingField = value;
        }

        public System.Collections.Generic.List<Entity.RowDto_Store_ExampleTable> __get_ExampleTable()
        {
            return this.ExampleTablek__BackingField;
        }

        public System.Byte[] __get_Version()
        {
            return this.Versionk__BackingField;
        }

        public void __set_Version(System.Byte[] value)
        {
            this.Versionk__BackingField = value;
        }

        public virtual void Map(System.Data.Common.DbDataReader reader)
        {
            this.Id = ((System.Guid)reader["Fld_101"]);
            this.Name = ((System.String)reader["Fld_102"]);
            this.Property1_Type = ((System.Int32)reader["Fld_103_Type"]);
            this.Property1_DateTime = ((System.DateTime)reader["Fld_103_DateTime"]);
            this.Property1_Int = ((System.Int32)reader["Fld_103_Int"]);
            this.Property1_Boolean = ((System.Boolean)reader["Fld_103_Boolean"]);
            this.Property1_Numeric = ((System.Double)reader["Fld_103_Numeric"]);
            this.Property1_Ref = ((System.Guid)reader["Fld_103_Ref"]);
        }
    }
}

namespace Entity
{
    public class RWRowDtoWrapperStore_ExampleTable : System.Object
    {
        Entity.RowDto_Store_ExampleTable _dtoRow;
        System.Object TProp1k__BackingField;
        public RWRowDtoWrapperStore_ExampleTable(Entity.RowDto_Store_ExampleTable A_1): base()
        {
            this._dtoRow = A_1;
            return;
        }

        public System.Object TProp1
        {
            get => __get_TProp1();
            set => __set_TProp1(value);
        }

        public System.Object __get_TProp1()
        {
            if (this._dtoRow.__get_TProp1_Type() == 6)
            {
                return this._dtoRow.__get_TProp1_String();
            }

            ;
            if (this._dtoRow.__get_TProp1_Type() == 110)
            {
                return this._dtoRow.__get_TProp1_Ref();
            }

            ;
            throw new System.Exception();
        }

        public void __set_TProp1(System.Object value)
        {
            if (value is System.String)
            {
                this._dtoRow.__set_TProp1_String(((System.String)value));
                this._dtoRow.__set_TProp1_Type(6);
                return;
            }

            ;
            if (value is Entity.StoreLink)
            {
                this._dtoRow.__set_TProp1_Ref(((Entity.StoreLink)value).__get_Id());
                this._dtoRow.__set_TProp1_Type(110);
                return;
            }

            ;
            throw new System.Exception();
        }
    }
}

namespace Entity
{
    public class RWRowCollectionStore_ExampleTable : Aquila.EntityComponent.Compilation.EntityTable<Entity.RowDto_Store_ExampleTable, Entity.RWRowDtoWrapperStore_ExampleTable>
    {
        public RWRowCollectionStore_ExampleTable(System.Collections.Generic.List<Entity.RowDto_Store_ExampleTable> A_1): base(A_1)
        {
        }

        public override Entity.RWRowDtoWrapperStore_ExampleTable Add()
        {
            Entity.RowDto_Store_ExampleTable loc0;
            Entity.RWRowDtoWrapperStore_ExampleTable loc1;
            loc0 = new Entity.RowDto_Store_ExampleTable();
            loc1 = new Entity.RWRowDtoWrapperStore_ExampleTable(loc0);
            this.DtoRef.Add(loc0);
            this.ObjRef.Add(loc1);
            return loc1;
        }
    }
}

namespace Entity
{
    public class Store : System.Object
    {
        Entity._Store _dto;
        System.Guid Idk__BackingField;
        System.String Namek__BackingField;
        System.Object Property1k__BackingField;
        Entity.RWRowCollectionStore_ExampleTable ExampleTablek__BackingField;
        public Store(Entity._Store A_1): base()
        {
            this._dto = A_1;
        }

        public System.Guid Id
        {
            get => __get_Id();
            set => __set_Id(value);
        }

        public System.String Name
        {
            get => __get_Name();
            set => __set_Name(value);
        }

        public System.Object Property1
        {
            get => __get_Property1();
            set => __set_Property1(value);
        }

        public Entity.RWRowCollectionStore_ExampleTable ExampleTable
        {
            get => __get_ExampleTable();
        }

        public void In(System.Int32 i)
        {
        }

        public void In(System.String s)
        {
        }

        public void Overload()
        {
            this.In(100);
            this.Save();
        }

        public System.Guid __get_Id()
        {
            return this._dto.__get_Id();
        }

        public void __set_Id(System.Guid value)
        {
            this._dto.__set_Id(value);
        }

        public System.String __get_Name()
        {
            return this._dto.__get_Name();
        }

        public void __set_Name(System.String value)
        {
            this._dto.__set_Name(value);
        }

        public System.Object __get_Property1()
        {
            if (this._dto.__get_Property1_Type() == 3)
            {
                return this._dto.__get_Property1_DateTime();
            }

            ;
            if (this._dto.__get_Property1_Type() == 7)
            {
                return this._dto.__get_Property1_Int();
            }

            ;
            if (this._dto.__get_Property1_Type() == 2)
            {
                return this._dto.__get_Property1_Boolean();
            }

            ;
            if (this._dto.__get_Property1_Type() == 5)
            {
                return this._dto.__get_Property1_Numeric();
            }

            ;
            if (this._dto.__get_Property1_Type() == 110)
            {
                return this._dto.__get_Property1_Ref();
            }

            ;
            throw new System.Exception();
        }

        public void __set_Property1(System.Object value)
        {
            if (value is System.DateTime)
            {
                this._dto.__set_Property1_DateTime(((System.DateTime)value));
                this._dto.__set_Property1_Type(3);
                return;
            }

            ;
            if (value is System.Int32)
            {
                this._dto.__set_Property1_Int(((System.Int32)value));
                this._dto.__set_Property1_Type(7);
                return;
            }

            ;
            if (value is System.Boolean)
            {
                this._dto.__set_Property1_Boolean(((System.Boolean)value));
                this._dto.__set_Property1_Type(2);
                return;
            }

            ;
            if (value is System.Double)
            {
                this._dto.__set_Property1_Numeric(((System.Double)value));
                this._dto.__set_Property1_Type(5);
                return;
            }

            ;
            if (value is Entity.StoreLink)
            {
                this._dto.__set_Property1_Ref(((Entity.StoreLink)value).__get_Id());
                this._dto.__set_Property1_Type(110);
                return;
            }

            ;
            throw new System.Exception();
        }

        public Entity.RWRowCollectionStore_ExampleTable __get_ExampleTable()
        {
            return new Entity.RWRowCollectionStore_ExampleTable(this._dto.__get_ExampleTable());
        }

        public void Save()
        {
            Entity.StoreManager.Save(this._dto);
        }
    }
}

namespace Entity
{
    public abstract class __cmd_HelloFromServer : System.Object
    {
        public static System.Int32 ClientCallProc(System.Int32 a)
        {
            a = a + 1;
            return a;
        }

        public static void ExecuteSql()
        {
            Aquila.ServerRuntime.PlatformQuery loc0;
            Aquila.ServerRuntime.PlatformReader loc1;
            loc0 = new Aquila.ServerRuntime.PlatformQuery();
            loc0.Text = "FROM Entity.Store SELECT Id";
            loc1 = loc0.ExecuteReader();
            while (loc1.Read())
            {
                System.Guid loc2;
                loc2 = ((System.Guid)loc1["Id"]);
            }

            ;
        }

        public static void CreateAndSaveStore()
        {
            Entity.Store loc0;
            loc0 = Entity.StoreManager.Create();
            loc0.Name = "Souths park";
            loc0.Save();
        }

        public static System.String GetUserNameServer()
        {
            Aquila.Core.PlatformContext loc0;
            Entity.__cmd_HelloFromServer.CreateAndSaveStore();
            loc0 = Aquila.Core.ContextHelper.GetContext();
            return loc0.UserName;
        }

        public static Entity.Store GetStore()
        {
            Entity.Store loc0;
            loc0 = Entity.StoreManager.Create();
            loc0.Name = "Souths park";
            return loc0;
        }

        public static Entity.Store UpdateName(Entity.Store income)
        {
            income.Name = "Changed!!!!";
            return income;
        }
    }
}

namespace Entity
{
    public class StoreEditorForm : Aquila.Avalonia.Wrapper.UXForm
    {
    }
}

namespace Entity
{
    public abstract class __StoreEditorForm : System.Object
    {
        static System.String _markup;
        public __StoreEditorForm(): base()
        {
            Entity.__StoreEditorForm._markup = "<p:StoreEditorForm \r\n                xmlns:p=\"clr-namespace:Entity;assembly=LibraryServer\"    \r\n                xmlns=\"clr-namespace:Aquila.Avalonia.Wrapper;assembly=Aquila.Avalonia.Wrapper\">\r\n  <UXGroup Orientation=\"Vertical\">\r\n    <UXTextBox />\r\n    <UXTextBox />\r\n    <UXGroup Orientation = \"Horizontal\"> \r\n        <UXCheckBox />\r\n<UXCheckBox />\r\n<UXCheckBox />\r\n<UXCheckBox />\r\n    </UXGroup>\r\n  </UXGroup>\r\n</p:StoreEditorForm>";
            return;
        }

        public static System.String Get()
        {
            Entity.StoreEditorForm loc0;
            loc0 = ((Entity.StoreEditorForm)Aquila.ServerRuntime.XamlService.Parse(Entity.__StoreEditorForm._markup));
            loc0.CreateOnServer();
            return Aquila.ServerRuntime.XamlService.Save(loc0);
        }
    }
}

namespace Entity
{
    public class StoreManager : System.Object
    {
        public static Entity.Store Create()
        {
            Entity._Store loc0;
            loc0 = new Entity._Store();
            loc0.Id = System.Guid.NewGuid();
            loc0.Name = "";
            loc0.Property1_DateTime = Aquila.Core.ContextHelper.GetContext().DataContext.Types.DateTime.MinValue;
            return new Entity.Store(loc0);
        }

        public static Entity.StoreLink Get(System.Guid id)
        {
            return new Entity.StoreLink(id);
        }

        public static Entity._Store GetDto(System.Guid id)
        {
            System.Data.Common.DbCommand loc0;
            System.Data.Common.DbDataReader loc1;
            System.Data.Common.DbParameter loc2;
            Entity._Store loc3;
            loc0 = Aquila.Core.ContextHelper.GetContext().DataContext.CreateCommand();
            loc0.CommandText = "SELECT T0.Fld_103_Ref,\nT0.Fld_103_Numeric,\nT0.Fld_103_Boolean,\nT0.Fld_103_Int,\nT0.Fld_103_DateTime,\nT0.Fld_103_Type,\nT0.Fld_102\nFROM\nObj_106 as T0\nWHERE\n@P_0 = T0.Fld_101\n";
            loc2 = loc0.CreateParameter();
            loc2.ParameterName = "P_0";
            loc2.Value = id;
            loc0.Parameters.Add(loc2);
            loc1 = loc0.ExecuteReader();
            loc1.Read();
            loc3 = new Entity._Store();
            loc3.Map(loc1);
            loc1.Dispose();
            loc0.Dispose();
            return loc3;
        }

        public static void Save(Entity._Store dto)
        {
            System.Data.Common.DbParameter loc0;
            System.Data.Common.DbCommand loc1;
            loc1 = Aquila.Core.ContextHelper.GetContext().DataContext.CreateCommand();
            if (dto.Version != null)
            {
                loc1.CommandText = "UPDATE Obj_106 as T0\nSET T0.Fld_103_Ref = @P_7, T0.Fld_103_Numeric = @P_6, T0.Fld_103_Boolean = @P_5, T0.Fld_103_Int = @P_4, T0.Fld_103_DateTime = @P_3, T0.Fld_103_Type = @P_2, T0.Fld_102 = @P_1\nWHERE\n@P_0 = T0.Fld_101\n";
            }
            else
            {
                loc1.CommandText = "INSERT INTO Obj_106(Fld_103_Ref, Fld_103_Numeric, Fld_103_Boolean, Fld_103_Int, Fld_103_DateTime, Fld_103_Type, Fld_102, Fld_101)\nVALUES\n(@P_7, @P_6, @P_5, @P_4, @P_3, @P_2, @P_1, @P_0)\n";
            }

            ;
            loc0 = loc1.CreateParameter();
            loc1.Parameters.Add(loc0);
            loc0.ParameterName = "P_0";
            loc0.Value = dto.Id;
            loc0 = loc1.CreateParameter();
            loc1.Parameters.Add(loc0);
            loc0.ParameterName = "P_1";
            loc0.Value = dto.Name;
            loc0 = loc1.CreateParameter();
            loc1.Parameters.Add(loc0);
            loc0.ParameterName = "P_2";
            loc0.Value = dto.Property1_Type;
            loc0 = loc1.CreateParameter();
            loc1.Parameters.Add(loc0);
            loc0.ParameterName = "P_3";
            loc0.Value = dto.Property1_DateTime;
            loc0 = loc1.CreateParameter();
            loc1.Parameters.Add(loc0);
            loc0.ParameterName = "P_4";
            loc0.Value = dto.Property1_Int;
            loc0 = loc1.CreateParameter();
            loc1.Parameters.Add(loc0);
            loc0.ParameterName = "P_5";
            loc0.Value = dto.Property1_Boolean;
            loc0 = loc1.CreateParameter();
            loc1.Parameters.Add(loc0);
            loc0.ParameterName = "P_6";
            loc0.Value = dto.Property1_Numeric;
            loc0 = loc1.CreateParameter();
            loc1.Parameters.Add(loc0);
            loc0.ParameterName = "P_7";
            loc0.Value = dto.Property1_Ref;
            loc1.ExecuteNonQuery();
        }
    }
}

namespace Entity
{
    public class RORowDtoWrapperStoreLink_ExampleTable : System.Object
    {
        Entity.RowDto_Store_ExampleTable _dtoRow;
        System.Object TProp1k__BackingField;
        public RORowDtoWrapperStoreLink_ExampleTable(Entity.RowDto_Store_ExampleTable A_1): base()
        {
            this._dtoRow = A_1;
            return;
        }

        public System.Object TProp1
        {
            get => __get_TProp1();
        }

        public System.Object __get_TProp1()
        {
            if (this._dtoRow.__get_TProp1_Type() == 6)
            {
                return this._dtoRow.__get_TProp1_String();
            }

            ;
            if (this._dtoRow.__get_TProp1_Type() == 110)
            {
                return this._dtoRow.__get_TProp1_Ref();
            }

            ;
            throw new System.Exception();
        }
    }
}

namespace Entity
{
    public class StoreLink : Entity.EntityLink
    {
        Entity._Store _dto;
        System.Object Property1k__BackingField;
        public StoreLink(System.Guid A_1, System.String A_2): base(A_1, 110, A_2)
        {
        }

        public StoreLink(System.Guid A_1): base(A_1, 110, System.String.Format("Link: [{0}]", A_1))
        {
        }

        public System.Object Property1
        {
            get => __get_Property1();
        }

        public System.Object __get_Property1()
        {
            if (this._dto == null)
            {
                this.Reload();
            }

            ;
            if (this._dto.__get_Property1_Type() == 3)
            {
                return this._dto.__get_Property1_DateTime();
            }

            ;
            if (this._dto.__get_Property1_Type() == 7)
            {
                return this._dto.__get_Property1_Type();
            }

            ;
            if (this._dto.__get_Property1_Type() == 2)
            {
                return this._dto.__get_Property1_Boolean();
            }

            ;
            if (this._dto.__get_Property1_Type() == 5)
            {
                return this._dto.__get_Property1_Numeric();
            }

            ;
            if (this._dto.__get_Property1_Type() == 110)
            {
                return this._dto.__get_Property1_Ref();
            }

            ;
            throw new System.Exception();
        }

        public void Reload()
        {
            this._dto = Entity.StoreManager.GetDto(this.Id);
        }
    }
}

namespace SerializableTypes
{
    [System.Runtime.Serialization.DataContractAttribute]
    public class ST1 : System.Object
    {
        System.Collections.Generic.List<System.String> Namek__BackingField;
        [System.Runtime.Serialization.DataMemberAttribute]
        public System.Collections.Generic.List<System.String> Name
        {
            get => __get_Name();
            set => __set_Name(value);
        }

        public System.Collections.Generic.List<System.String> __get_Name()
        {
            return this.Namek__BackingField;
        }

        public void __set_Name(System.Collections.Generic.List<System.String> value)
        {
            this.Namek__BackingField = value;
        }
    }
}

namespace WebServices
{
    [System.ServiceModel.ServiceContractAttribute]
    public class WebService1 : System.Object
    {
    }
}