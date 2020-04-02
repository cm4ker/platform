// using System.Collections.Generic;
// using System.IO;
// using System.Linq;
// using System.Reflection;
//
// namespace ZenPlatform.Compiler.Roslyn
// {
//     public class RConstructorBuilder : SreConstructor
//     {
//         private readonly RTypeBuilder _declaringType;
//         private List<RParameterBuilder> _parameters;
//         private RBlockBuilder _body;
//
//         public RConstructorBuilder(RTypeBuilder declaringType)
//         {
//             _declaringType = declaringType;
//             _parameters = new List<RParameterBuilder>();
//             _body = new RBlockBuilder();
//         }
//
//         public SreType ReturnType { get; set; }
//
//         public RBlockBuilder Body => _body;
//
//         public RParameterBuilder DefineParameter()
//         {
//             var p = new RParameterBuilder();
//             _parameters.Add(p);
//             return p;
//         }
//
//         public void Dump(TextWriter tw)
//         {
//             tw.Write("public ");
//
//             tw.W(DeclaringType.Name);
//
//             using (tw.Parenthesis())
//             {
//                 var wasFirst = false;
//                 foreach (var parameter in _parameters)
//                 {
//                     if (wasFirst)
//                         tw.W(", ");
//
//                     parameter.Dump(tw);
//
//                     wasFirst = true;
//                 }
//             }
//
//
//             _body.Dump(tw);
//         }
//
//         public SreType DeclaringType => _declaringType;
//         public IReadOnlyList<IParameter> Parameters => _parameters;
//
//         public bool IsStatic { get; set; }
//
//         public void DumpRef(TextWriter tw)
//         {
//             tw.W(DeclaringType.FullName);
//         }
//     }
//
//     public class RConstructor : SreConstructor
//     {
//         private readonly RTypeSystem _ts;
//         private readonly ConstructorInfo _constructorInfo;
//         private readonly List<RParameter> _parameters;
//
//         public RConstructor(RTypeSystem ts, ConstructorInfo constructorInfo)
//         {
//             _constructorInfo = constructorInfo;
//             _parameters = _constructorInfo.GetParameters().Select(x => new RParameter(x)).ToList();
//             _ts = ts;
//         }
//
//         public SreType DeclaringType => _ts.Wrap(_constructorInfo.DeclaringType);
//         
//         public IReadOnlyList<IParameter> Parameters => _parameters;
//
//         public bool IsStatic => _constructorInfo.IsStatic;
//
//         public void DumpRef(TextWriter tw)
//         {
//             DeclaringType.DumpRef(tw);
//         }
//     }
// }