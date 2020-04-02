public abstract class EntryPoint:System.Object
{
public static void Main(System.Object[] args)
{

}public static void testUI()
{
ZenPlatform.Core.Contracts.Network.IProtocolClient loc0;
loc0 = ZenPlatform.ClientRuntime.GlobalScope.Client;
return;

}
}

namespace Entity
{
public class RowDto_Store_ExampleTable:System.Object
{
System.Int32 TProp1_Typek__BackingField;
System.String TProp1_Stringk__BackingField;
System.Guid TProp1_Refk__BackingField;
public RowDto_Store_ExampleTable(): base()
{

}public System.Int32 TProp1_Type
{
get => __get_TProp1_Type();set => __set_TProp1_Type(value);
}
public System.String TProp1_String
{
get => __get_TProp1_String();set => __set_TProp1_String(value);
}
public System.Guid TProp1_Ref
{
get => __get_TProp1_Ref();set => __set_TProp1_Ref(value);
}
public System.Int32 __get_TProp1_Type()
{
return this.TProp1_Typek__BackingField;

}public void __set_TProp1_Type(System.Int32 value)
{
this.TProp1_Typek__BackingField = value;

}public System.String __get_TProp1_String()
{
return this.TProp1_Stringk__BackingField;

}public void __set_TProp1_String(System.String value)
{
this.TProp1_Stringk__BackingField = value;

}public System.Guid __get_TProp1_Ref()
{
return this.TProp1_Refk__BackingField;

}public void __set_TProp1_Ref(System.Guid value)
{
this.TProp1_Refk__BackingField = value;

}
}
}

namespace Entity
{
public class _Store:System.Object
{
System.Guid Idk__BackingField;
System.String Namek__BackingField;
System.Int32 Property1_Typek__BackingField;
System.DateTime Property1_DateTimek__BackingField;
System.Int32 Property1_Intk__BackingField;
System.Boolean Property1_Booleank__BackingField;
System.Double Property1_Numerick__BackingField;
System.Guid Property1_Refk__BackingField;
System.Byte[] Versionk__BackingField;
public _Store(): base()
{

}public System.Guid Id
{
get => __get_Id();set => __set_Id(value);
}
public System.String Name
{
get => __get_Name();set => __set_Name(value);
}
public System.Int32 Property1_Type
{
get => __get_Property1_Type();set => __set_Property1_Type(value);
}
public System.DateTime Property1_DateTime
{
get => __get_Property1_DateTime();set => __set_Property1_DateTime(value);
}
public System.Int32 Property1_Int
{
get => __get_Property1_Int();set => __set_Property1_Int(value);
}
public System.Boolean Property1_Boolean
{
get => __get_Property1_Boolean();set => __set_Property1_Boolean(value);
}
public System.Double Property1_Numeric
{
get => __get_Property1_Numeric();set => __set_Property1_Numeric(value);
}
public System.Guid Property1_Ref
{
get => __get_Property1_Ref();set => __set_Property1_Ref(value);
}
public System.Byte[] Version
{
get => __get_Version();set => __set_Version(value);
}
public System.Guid __get_Id()
{
return this.Idk__BackingField;

}public void __set_Id(System.Guid value)
{
this.Idk__BackingField = value;

}public System.String __get_Name()
{
return this.Namek__BackingField;

}public void __set_Name(System.String value)
{
this.Namek__BackingField = value;

}public System.Int32 __get_Property1_Type()
{
return this.Property1_Typek__BackingField;

}public void __set_Property1_Type(System.Int32 value)
{
this.Property1_Typek__BackingField = value;

}public System.DateTime __get_Property1_DateTime()
{
return this.Property1_DateTimek__BackingField;

}public void __set_Property1_DateTime(System.DateTime value)
{
this.Property1_DateTimek__BackingField = value;

}public System.Int32 __get_Property1_Int()
{
return this.Property1_Intk__BackingField;

}public void __set_Property1_Int(System.Int32 value)
{
this.Property1_Intk__BackingField = value;

}public System.Boolean __get_Property1_Boolean()
{
return this.Property1_Booleank__BackingField;

}public void __set_Property1_Boolean(System.Boolean value)
{
this.Property1_Booleank__BackingField = value;

}public System.Double __get_Property1_Numeric()
{
return this.Property1_Numerick__BackingField;

}public void __set_Property1_Numeric(System.Double value)
{
this.Property1_Numerick__BackingField = value;

}public System.Guid __get_Property1_Ref()
{
return this.Property1_Refk__BackingField;

}public void __set_Property1_Ref(System.Guid value)
{
this.Property1_Refk__BackingField = value;

}public System.Byte[] __get_Version()
{
return this.Versionk__BackingField;

}public void __set_Version(System.Byte[] value)
{
this.Versionk__BackingField = value;

}
}
}

namespace Entity
{
public class Store:System.Object
{
Entity._Store _dto;
public Store(Entity._Store A_1): base()
{
this._dto = A_1;

}
}
}

namespace Entity
{
public abstract class __cmd_HelloFromServer:System.Object
{
public static System.Int32 ClientCallProc(System.Int32 a)
{
ZenPlatform.Core.Contracts.Network.IProtocolClient loc0;
loc0 = ZenPlatform.ClientRuntime.GlobalScope.Client;
return loc0.Invoke<System.Int32>(new ZenPlatform.Core.Contracts.Route("__cmd_HelloFromServer.ClientCallProc"),new System.Object[1]
{
a
});

}public static void OnClientClientCallProc()
{
Entity.__cmd_HelloFromServer.ClientCallProc(10);

}public static System.String GetUserNameServer()
{
ZenPlatform.Core.Contracts.Network.IProtocolClient loc0;
loc0 = ZenPlatform.ClientRuntime.GlobalScope.Client;
return loc0.Invoke<System.String>(new ZenPlatform.Core.Contracts.Route("__cmd_HelloFromServer.GetUserNameServer"),new System.Object[0]
{

});

}public static Entity.Store GetStore()
{
ZenPlatform.Core.Contracts.Network.IProtocolClient loc0;
loc0 = ZenPlatform.ClientRuntime.GlobalScope.Client;
return loc0.Invoke<Entity.Store>(new ZenPlatform.Core.Contracts.Route("__cmd_HelloFromServer.GetStore"),new System.Object[0]
{

});

}public static Entity.Store UpdateName(Entity.Store income)
{
ZenPlatform.Core.Contracts.Network.IProtocolClient loc0;
loc0 = ZenPlatform.ClientRuntime.GlobalScope.Client;
return loc0.Invoke<Entity.Store>(new ZenPlatform.Core.Contracts.Route("__cmd_HelloFromServer.UpdateName"),new System.Object[1]
{
income
});

}public static void GetUserName()
{
Entity.__cmd_HelloFromServer.GetUserNameServer();

}
}
}

namespace Entity
{
public class StoreEditorForm:ZenPlatform.Avalonia.Wrapper.UXForm
{
public StoreEditorForm(): base()
{

}
}
}
