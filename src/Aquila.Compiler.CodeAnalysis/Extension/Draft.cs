namespace Aquila.Language.Ast.Extension
{
    /*
     We must create extendable compiler

     Component will be involved into the compile and analyses process

     0) Create compilation
     1) Create syntax tree <- here we can't extend the platform
     2) Create symbols <- here we can't extend the platform
     3)
     */


    /*
    1) CreateAllTypeDefs
    2) CreateDependentStructures (Properties, Methods, Fields, etc)
    3) CreateSymbols for the Compilation on Binding

    Here component must register TypeSymbols, and etc
                     V
    SyntaxTree -> Binding -> BoundTree

        V

     SOME CLASS STRUCTURE

     platformcode

     SOME CLASS STRUCTURE


Example

    GlobalScopeBinding
        - RegisterGlobalNamespaceSymbols
            - RegisterNestedNamespsaceSymbol
            - RegisterNamedTypeSymbol
            - RegisterPlatformNestedSymbol
            - RegisterPlatformNamedTypeSymbol

    public class Store <---- this is an object this symbol is avaliable in code
    {
        public Guid Id { get; set; } <- this symbol avaliable in code

        public StoreLink Link { get; set; } <- this symbol avaliable in code

        public void Save() <- this symbol avaliable in code
        {
        }

        public void Remove() <- this symbol avaliable in code
        {
        }

        // Platform properties
        ...
        //

        // Platform code
        ...
        //

    }

     */
}