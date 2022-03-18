using System;
using System.IO;
using System.Text;
using Xunit.Abstractions;

namespace Aquila.Runtime.Tests.DB;

public class TestHelperStream : Stream
{
    private readonly ITestOutputHelper _output;

    public TestHelperStream(ITestOutputHelper output)
    {
        _output = output;
    }

    public override void Flush()
    {
        throw new NotImplementedException();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        throw new NotImplementedException();
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        throw new NotImplementedException();
    }

    public override void SetLength(long value)
    {
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        var str = Encoding.UTF8.GetString(buffer);
        _output.WriteLine(str);
    }

    public override bool CanRead => false;
    public override bool CanSeek => false;
    public override bool CanWrite => true;
    public override long Length { get; }
    public override long Position { get; set; }
}