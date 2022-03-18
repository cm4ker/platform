using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using SharpFileSystem.Database;
using SharpFileSystem.IO;
using Xunit;
using Aquila.Data;
using Aquila.Metadata;
using Aquila.QueryBuilder;
using RamDisk;
using SqlInMemory;

namespace SharpFileSystem.Tests.FileSystems
{
    public abstract class DbFileSystemTestBase : IDisposable
    {
        public static string DefaultConnetionString =
            "Data Source=.;Initial Catalog=FSDB;Integrated Security=true;TrustServerCertificate=True";

        private readonly DataConnectionContext _context;
        private readonly IDisposable _inmemDb;
        protected DatabaseFileSystem FileSystem { get; set; }

        public DbFileSystemTestBase()
        {
            try
            {
                RamDrive.Unmount('Y');
            }
            catch (InvalidOperationException ex)
            {
                // do nothing
            }

            _inmemDb = SqlInMemoryDb.Create(DefaultConnetionString, drive: 'Y');

            _context = new DataConnectionContext(SqlDatabaseType.SqlServer, DefaultConnetionString,
                IsolationLevel.ReadCommitted);

            FileSystem = new DatabaseFileSystem("vTable", _context);
        }

        public void Dispose()
        {
            FileSystem.ClearTable();
            FileSystem.Dispose();
            _context.Dispose();
            _inmemDb.Dispose();
        }
    }

    public class DbFileSystemTest : DbFileSystemTestBase
    {
        FileSystemPath RootFilePath { get; } = FileSystemPath.Root.AppendFile("x");


        [Fact]
        public void CreateFile()
        {
            // File shouldn’t exist prior to creation.
            Assert.False(FileSystem.Exists(RootFilePath));

            var content = new byte[] { 0xde, 0xad, 0xbe, 0xef, };
            using (var xStream = FileSystem.CreateFile(RootFilePath))
            {
                // File now should exist.
                Assert.True(FileSystem.Exists(RootFilePath));

                xStream.Write(content, 0, content.Length);
            }

            // File should still exist and have content.
            Assert.True(FileSystem.Exists(RootFilePath));
            using (var xStream = FileSystem.OpenFile(RootFilePath, FileAccess.Read))
            {
                var readContent = new byte[2 * content.Length];
                Assert.Equal(content.Length, xStream.Read(readContent, 0, readContent.Length));
                Assert.Equal(
                    content,
                    // trim to the length that was read.
                    readContent.Take(content.Length).ToArray());

                // Trying to read beyond end of file should return 0.
                Assert.Equal(0, xStream.Read(readContent, 0, readContent.Length));
            }
        }

        [Fact]
        public void CreateFile_Exists()
        {
            Assert.False(FileSystem.Exists(RootFilePath));

            // Place initial content.
            using (var stream = FileSystem.CreateFile(RootFilePath))
            {
                stream.Write(Encoding.UTF8.GetBytes("asdf"));
            }

            Assert.True(FileSystem.Exists(RootFilePath));

            // Replace—truncates.
            var content = Encoding.UTF8.GetBytes("b");
            using (var stream = FileSystem.CreateFile(RootFilePath))
            {
                stream.Write(content);
            }

            Assert.True(FileSystem.Exists(RootFilePath));
            using (var stream = FileSystem.OpenFile(RootFilePath, FileAccess.Read))
                Assert.Equal(content, stream.ReadAllBytes());
        }

        [Fact]
        public void CreateFile_Empty()
        {
            using (var stream = FileSystem.CreateFile(RootFilePath))
            {
            }

            Assert.True(FileSystem.Exists(RootFilePath));

            using (var stream = FileSystem.OpenFile(RootFilePath, FileAccess.Read))
            {
                Assert.Equal(
                    new byte[] { },
                    stream.ReadAllBytes());
            }
        }

        [Fact]
        public void GetEntities()
        {
            for (var i = 0; i < 10; i++)
            {
                Console.WriteLine($"i={i}");
                using (var stream = FileSystem.CreateFile(RootFilePath))
                {
                }

                // Should exist once.
                Assert.Equal(
                    new[] { RootFilePath, },
                    FileSystem.GetEntities(FileSystemPath.Root).ToArray());
            }
        }
    }
}