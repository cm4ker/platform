using System;
using System.IO;
using System.Text;

namespace Aquila.Core;

public partial class AqContext
{
    private TextWriter _outputText;
    private Stream _output;


    protected void InitOutput(Stream output)
    {
        Output = output;
    }

    public TextWriter OutputText => _outputText;

    public Stream Output
    {
        get => _output;
        set
        {
            _output = value;
            _outputText = new StreamWriter(value, Encoding.UTF8) { AutoFlush = true };
        }
    }
}