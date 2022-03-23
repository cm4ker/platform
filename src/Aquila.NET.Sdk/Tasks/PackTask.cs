using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Threading;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using SimpleJSON;

namespace Aquila.NET.Sdk.Tools
{
    /// <summary>
    /// Pack the solution for deploying
    /// </summary>
    public class PackTask : Task, ICancelableTask
    {
        /// <summary></summary>
        [Required]
        public string OutputPath { get; set; }

        /// <summary></summary>
        [Required]
        public string OutputPackagePath { get; set; }

        /// <summary></summary>
        [Required]
        public string OutputPackageFullPath { get; set; }

        /// <summary></summary>
        [Required]
        public string OutputName { get; set; }

        /// <summary></summary>
        [Required]
        public string TempOutputPath { get; set; }

        [Required]
        public string ProjectDirectory { get; set; }

        /// <summary>
        /// Used for debugging purposes.
        /// If enabled a debugger is attached to the current process upon the task execution.
        /// </summary>
        public bool DebuggerAttach { get; set; } = false;

        public ITaskItem[] Metadata { get; set; }

        public override bool Execute()
        {
            DirectoryInfo info = new DirectoryInfo(OutputPath);

            if (DebuggerAttach)
            {
                Console.WriteLine(
                    $@"Attach information: prc='{Process.GetCurrentProcess().ProcessName}'; pid={Process.GetCurrentProcess().Id}");
                Console.WriteLine(@"Waiting for debugger attach...");

                while (!Debugger.IsAttached && !IsCanceled())
                {
                    Thread.Sleep(1000);
                }

                if (!IsCanceled())
                    Console.WriteLine(@"Attached!");
                else
                {
                    Console.WriteLine(@"Cancelled!");
                }
            }

            
            
            using (var package = ZipFile.Open(OutputPackageFullPath, ZipArchiveMode.Create))
            {
                foreach (var file in info.GetFiles())
                {
                    if (Path.GetFileName(file.Name) == OutputName)
                    {
                    }

                    package.CreateEntryFromFile(file.FullName, file.Name);
                }

                foreach (var md in Metadata)
                {
                    var sourceFileName = Path.Combine(ProjectDirectory, md.ToString());
                    var destinationPath = Path.Combine("Metadata", md.ToString());

                    package.CreateEntryFromFile(sourceFileName, destinationPath);
                }
            }

            return true;
        }

        private CancellationTokenSource _cancellation = new CancellationTokenSource();

        /// <summary>
        /// Cancels the task nicely.
        /// </summary>
        public void Cancel()
        {
            _cancellation.Cancel();
        }

        /// <summary>
        /// Gets value indicating user has canceled the task.
        /// </summary>
        public bool IsCanceled()
        {
            return _cancellation != null && _cancellation.IsCancellationRequested;
        }
    }
}