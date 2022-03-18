using System;
using System.IO;
using Aquila.Core;
using Aquila.QueryBuilder;

namespace SharpFileSystem.Database
{
    public class DBFileStream : Stream
    {
        private readonly FileSystemPath _path;
        private MemoryStream _ms;
        private bool isExists;
        private DatabaseFileSystem _fs;
        private FileAccess _access;

        public DBFileStream(FileSystemPath path, DatabaseFileSystem fs, FileAccess access)
        {
            _path = path;
            _fs = fs;
            _access = access;

            if (fs.Exists(path) && (access & FileAccess.Read) != 0)
            {
                Reload();
            }
            else
                _ms = new MemoryStream();
        }

        private void Reload()
        {
            void Gen(QueryMachine qm)
            {
                qm.bg_query()
                    .m_from()
                    .ld_table(_fs.TableName)
                    .m_where()
                    .ld_column("Path")
                    .ld_param("Path")
                    .eq()
                    .ld_column("Name")
                    .ld_param("Name")
                    .eq()
                    .and()
                    .m_select()
                    .ld_column("Data")
                    .st_query();
            }

            using (var cmd = _fs.Context.CreateCommand(Gen))
            {
                cmd.AddParameterWithValue("Path", _path.ParentPath.ToString());
                cmd.AddParameterWithValue("Name", _path.EntityName);

                _ms = new MemoryStream((byte[])cmd.ExecuteScalar());
            }
        }

        private void Insert()
        {
            void Gen(QueryMachine qm)
            {
                qm
                    .bg_query()
                    .m_values()
                    .ld_param("Path")
                    .ld_param("Name")
                    .ld_param("IsDirectory")
                    .ld_param("Data")
                    .m_insert()
                    .ld_table(_fs.TableName)
                    .ld_column("Path")
                    .ld_column("Name")
                    .ld_column("IsDirectory")
                    .ld_column("Data")
                    .st_query();
            }

            using (var cmd = _fs.Context.CreateCommand(Gen))
            {
                cmd.AddParameterWithValue("Path", _path.ParentPath.ToString());
                cmd.AddParameterWithValue("Name", _path.EntityName);
                cmd.AddParameterWithValue("IsDirectory", _path.IsDirectory);
                cmd.AddParameterWithValue("Data", _ms.ToArray());

                cmd.ExecuteNonQuery();
            }
        }

        private void Update()
        {
            void Gen(QueryMachine qm)
            {
                qm
                    .bg_query()
                    .m_where()
                    .ld_column("Path")
                    .ld_param("Path")
                    .eq()
                    .ld_column("Name")
                    .ld_param("Name")
                    .eq()
                    .and()
                    .m_set()
                    .ld_column("Data")
                    .ld_param("Data")
                    .assign()
                    .m_update()
                    .ld_table(_fs.TableName)
                    .st_query();
            }

            using (var cmd = _fs.Context.CreateCommand(Gen))
            {
                cmd.AddParameterWithValue("Path", _path.ParentPath.ToString());
                cmd.AddParameterWithValue("Name", _path.EntityName);
                cmd.AddParameterWithValue("Data", _ms.ToArray());

                cmd.ExecuteNonQuery();
            }
        }

        public override void Flush()
        {
            _fs.Context.BeginTransaction();

            if (_fs.Exists(_path))
            {
                Update();
            }
            else
            {
                Insert();
            }

            _fs.Context.CommitTransaction();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return _ms.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return _ms.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            _ms.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if ((_access & FileAccess.Write) == 0) throw new Exception("This file opened in read only mode");

            _ms.Write(buffer, offset, count);
        }

        public override bool CanRead => true;
        public override bool CanSeek => true;
        public override bool CanWrite => true;
        public override long Length => _ms.Length;

        public override long Position
        {
            get => _ms.Position;
            set => _ms.Position = value;
        }

        protected override void Dispose(bool disposing)
        {
            Flush();
            _ms.Dispose();
            base.Dispose(disposing);
        }
    }
}