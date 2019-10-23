using System.Collections.Generic;

namespace ZenPlatform.Compiler.Cecil.CopyFeature
{
    public class RepackOptions
    {
        public bool UnionMerge { get; set; }

        public bool RenameInternalized { get; set; }

        public string RepackDropAttribute { get; set; }

        public List<string> AllowedDuplicateTypes { get; set; }

        public List<string> AllowedDuplicateNameSpaces { get; set; }

        public bool AllowZeroPeKind { get; set; }

        public string TargetPlatformVersion { get; set; }

        public string TargetPlatformDirectory { get; set; }

        public List<string> ResolveFiles()
        {
            return new List<string>();
        }

        public void Validate()
        {
        }


        public string ToCommandLine()
        {
            return "";
        }

        public bool DebugInfo { get; set; }
    }
}