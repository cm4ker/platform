using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Aquila.SSH;

namespace Aquila.Shell.MiniTerm
{
    public class MyTerminal : ITerminal
    {
//        PipeStream reader;
//        AnonymousPipeServerStream writer;

        FileStream reader;
        FileStream writer;


        public event EventHandler<uint> CloseReceived;
        public event EventHandler<byte[]> DataReceived;

        public MyTerminal()
        {
//            writer = new AnonymousPipeServerStream(PipeDirection.Out);
//            reader = new AnonymousPipeClientStream(PipeDirection.In, (writer.GetClientHandleAsString()));

            reader = new FileStream("test", FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite);
            writer = new FileStream("test", FileMode.Open, FileAccess.Write, FileShare.ReadWrite);
        }

        public void Dispose()
        {
            writer.Dispose();
            reader.Dispose();
        }

        public void OnClose()
        {
            writer.WriteByte(0x03);
            writer.Flush();
        }

        public void OnInput(byte[] data)
        {
            writer.Write(data, 0, data.Length);
            writer.Flush();
        }

        public void OnSizeChanged(TerminalSize size)
        {
        }


        public void Run()
        {
            // copy all pseudoconsole output to stdout
            Task.Run(() =>
            {
                var buf = new byte[1024];
                while (true)
                {
                    var length = reader.Read(buf, 0, buf.Length);
//                    if (length == 0)
//                        break;
                    DataReceived?.Invoke(this, buf.Take(length).ToArray());

//                    var data = Encoding.UTF8.GetBytes("Hello!");
//
//                    DataReceived?.Invoke(this, data);
//
//                    Task.Delay(1000).Wait();
                }

                CloseReceived?.Invoke(this, 0);
            });
        }
    }
}