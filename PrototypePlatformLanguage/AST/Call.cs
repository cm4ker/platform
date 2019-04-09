namespace PrototypePlatformLanguage.AST
{
    /// <summary>
    /// Describes a function call.
    /// </summary>
    public class Call : Expression
    {
        /// <summary>
        /// Function to call.
        /// </summary>
        public string Name;

        /// <summary>
        /// Function arguments to pass.
        /// </summary>
        public ArgumentCollection Arguments;

        /// <summary>
        /// Creates a function call object.
        /// </summary>
        public Call(ArgumentCollection arguments, string name)
        {
            Arguments = arguments;
            Name = name;
        }
    }
}