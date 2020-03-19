using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace BufferedDataReaderDotNet.Infrastructure
{
    public interface IAsyncSerializable
    {
        Task WriteToAsync(Stream stream, CancellationToken cancellationToken);
    }
}