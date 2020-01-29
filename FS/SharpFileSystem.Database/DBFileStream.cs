using System;
using System.Data.Common;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace SharpFileSystem.Database
{
    public class DBFileStream : Stream
    {
        private readonly FileSystemPath _path;
        private readonly DbCommand _cmd;
        private readonly string _tableName;
        private MemoryStream _ms;
        private bool isExists;

        public DBFileStream(FileSystemPath path, DbCommand cmd, string tableName, bool load)
        {
            _path = path;
            _ms = new MemoryStream();

            _cmd = cmd;
            _tableName = tableName;

            _cmd.CommandText = $"SELECT 1 FROM {_tableName} WHERE path = @path AND name = @name";
            var param = _cmd.CreateParameter();
            param.Value = _path.ParentPath.ToString();
            param.ParameterName = "path";
            _cmd.Parameters.Add(param);

            param = _cmd.CreateParameter();
            param.Value = _path.EntityName;
            param.ParameterName = "name";
            _cmd.Parameters.Add(param);

            isExists = (_cmd.ExecuteScalar() != null);

            if (load && isExists)
            {
                _cmd.CommandText = $"SELECT Data FROM {_tableName} WHERE path = @path AND name = @name";
                _ms = new MemoryStream((byte[]) _cmd.ExecuteScalar());
            }
        }

        public override void Flush()
        {
            var tran = _cmd.Connection.BeginTransaction();
            _cmd.Transaction = tran;
            DbParameter param;

            _cmd.Parameters.Clear();

            if (!isExists)
            {
                _cmd.CommandText =
                    $"INSERT INTO {_tableName}(Path, Name, IsDirectory, Data) VALUES( @Path, @Name, @IsDirectory, @Data)";

                param = _cmd.CreateParameter();
                param.Value = _path.IsDirectory;
                param.ParameterName = "IsDirectory";
                _cmd.Parameters.Add(param);
            }
            else
            {
                _cmd.CommandText = $"UPDATE {_tableName} SET Data = @Data WHERE Path = @Path AND Name = @Name";
            }

            param = _cmd.CreateParameter();
            param.Value = _path.ParentPath.ToString();
            param.ParameterName = "path";
            _cmd.Parameters.Add(param);

            param = _cmd.CreateParameter();
            param.Value = _path.EntityName;
            param.ParameterName = "name";
            _cmd.Parameters.Add(param);

            param = _cmd.CreateParameter();
            param.Value = _ms.ToArray();
            param.ParameterName = "Data";
            _cmd.Parameters.Add(param);

            _cmd.ExecuteNonQuery();

            tran.Commit();
            isExists = true;
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
            base.Dispose(disposing);
            _cmd.Dispose();
        }
    }
}
