using Avalonia.LogicalTree;

namespace Aquila.Compiler
{
    /*
     <AddReference test>
     
     using Test.Test;
     using Documents;
     using Entity;
     using Reference;
          
     public void SomeFunc(ImportedClass cl)
     {
        var a = cl.DoSomeStuff();
        var b = cl.ICanDoSomeStuff();
     }
          
    import class ImportedClass from dll reference
    {
        import method int B(int a, int b);
        import method void DoSomeStuff();
        import method void ICanDoSomeStuff();
    }   
    
    +
    +   1) TypeSystemType - control types in platform it need for generating types, migrating, not have explicit Bridge to the ClrType 
    +
    
    +
    +   2) AstType - Not calculated, not related to the ClrType. Can evaluate clr type by ast (TypeFinder) 
    +
    
    +
    +   3) RuntimeType - RuntimeType 
    +
    */
}