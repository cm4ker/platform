/*
 1) We Building code by CLR types
 
 2) We Building Query By the DBTypes
 
 
 Entity - Store - Property1 - numeric(10, 2)
                - Property2 - int, bool, numeric, ref
                

                ProtoObject
                 |       | 
            TypeInfo    DbInfo
                |                
              IType    
                |
          UserAccessMode
            

                
 public void A()
 {
    var q = Q"Select Prop1, Prop2 From Store";
    var reader = q.Execute();
    
    while(reader.Next())
    {
        //DbType -> ClrType
        //mapper (TypeId(Db) => TypeId(Clr))
    
        var a = reader.Prop1; //- Clr value
    }
 }
 */