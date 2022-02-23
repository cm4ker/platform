using System.Threading.Tasks;
using Aquila.CodeAnalysis;


namespace Aquila.LanguageServer;

public class CompilationManager
{
    private readonly ProjectHolder _holder;
    private static AquilaCompilation _currentCompilation;
    private ProjectHandler _handler;

    public CompilationManager(ProjectHolder holder)
    {
        _holder = holder;
    }

    internal async Task<AquilaCompilation> GetCompilation()
    {
        return (await _holder.GetHandlerAsync()).Compilation;
    }
}