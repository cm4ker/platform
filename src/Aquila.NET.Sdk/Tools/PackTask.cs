using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Threading;
using Aquila.Syntax.Syntax;
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


        public override bool Execute()
        {
            DirectoryInfo info = new DirectoryInfo(OutputPath);

            using (var package = ZipFile.Open(OutputPackageFullPath, ZipArchiveMode.Create))
            {
                foreach (var file in info.GetFiles())
                {
                    package.CreateEntryFromFile(file.FullName, file.Name);
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
    }
}