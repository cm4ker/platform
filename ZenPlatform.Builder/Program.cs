namespace ZenPlatform.Cli
{
    public class Program
    {
        public static int Main(params string[] args)
        {
            return CliBuilder.Build(args);
        }
    }
}