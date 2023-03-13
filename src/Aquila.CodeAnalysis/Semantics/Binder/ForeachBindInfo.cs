using Microsoft.CodeAnalysis;

namespace Aquila.CodeAnalysis.Semantics;

public struct ForeachBindInfo
{
    public IPropertySymbol CurrentMember;
    public IMethodSymbol MoveNextMember;
    public ITypeSymbol EnumeratorSymbol;
    public IMethodSymbol GetEnumerator;
    public IMethodSymbol DisposeMember;

    public BoundReferenceEx EnumeratorVar;
    public BoundReferenceEx CurrentVar;
    public BoundCallEx MoveNextEx;
    public BoundAssignEx EnumeratorAssignmentEx;
    public BoundCallEx DisposeCall;
}