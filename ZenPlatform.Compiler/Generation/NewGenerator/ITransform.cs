using ZenPlatform.Compiler.Generation.NewGenerator;

namespace ZenPlatform.Compiler.Generation
{
    public interface ITransform
    {
        /// <summary>
        /// Трансформировать текущий контекст
        /// </summary>
        /// <param name="context"></param>
        /// <returns>true - если трансформация была выполнена</returns>
        bool Transform(IAstNodeContext context);
    }
}