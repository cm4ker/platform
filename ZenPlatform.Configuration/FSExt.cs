using System.IO;
using SharpFileSystem;
using ZenPlatform.Configuration.Structure;

namespace ZenPlatform.Configuration
{
    public static class FileSystemExtensions
    {
        public static T Deserialize<T>(this IFileSystem fs, string path)
        {
            try
            {
                using (var stream = fs.OpenFile(FileSystemPath.Parse(path), FileAccess.Read))
                {
                    return XCHelper.DeserializeFromStream<T>(stream);
                }
            }
            catch
            {
                return default;
            }
        }

        public static T Deserialize<T>(this IFileSystem fs, FileSystemPath path)
        {
           return Deserialize<T>(fs, path.ToString());
        }

        public static byte[] GetBytes(this IFileSystem fs, string path)
        {
            using (var stream = fs.OpenFile(FileSystemPath.Parse(path), FileAccess.Read))
            {
                var memoryStream = new MemoryStream();
                stream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }

        public static void Serialize(this IFileSystem fs, string path, object obj)
        {
            using (var stream = fs.OpenFile(FileSystemPath.Parse(path), FileAccess.Read))
                obj.SerializeToStream().CopyTo(stream);
        }

        public static void SaveBytes(IFileSystem fs, string path, byte[] data)
        {
            using (var stream = fs.OpenFile(FileSystemPath.Parse(path), FileAccess.Read))
                stream.Write(data, 0, data.Length);
        }
    }
}