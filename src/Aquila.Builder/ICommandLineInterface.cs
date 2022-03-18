using System.Collections.Generic;
using System.Text;

namespace Aquila.Cli
{
    public interface ICommandLineInterface
    {
        int Execute(string[] args);
    }
}
