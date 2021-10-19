﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Xunit;
using Xunit.Sdk;

namespace ScriptsTest
{
    /// <summary>
    /// Provides enumeration of aquila test files to be used by xUnit.
    /// </summary>
    sealed class ScriptsListDataAttribute : DataAttribute
    {
        static string GetRootDirectory()
        {
            var d = Directory.GetCurrentDirectory();
            while (!File.Exists(Path.Combine(d, "Aquila.sln")))
            {
                d = Path.GetDirectoryName(d);
            }

            return d;
        }

        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            var testspath = Path.Combine(GetRootDirectory(), "tests");
            Assert.True(Directory.Exists(testspath), $"Tests directory '{testspath}' cannot be found.");

            var files = Directory.GetFiles(testspath, "*.aq", SearchOption.AllDirectories);
            Assert.NotEmpty(files);

            // separate file name and path so we see the file name in Test Explorer
            return files.Select(f => new object[]
                { Path.GetFileName(f), Path.GetDirectoryName(f).Substring(testspath.Length), testspath });
        }
    }
}