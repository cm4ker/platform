using Antlr4.Runtime;

namespace ZenPlatform.Compiler.Preprocessor
{
    public class PreProcessorVisitor : ZSharpPreprocessorParserBaseVisitor<object>
    {
        public override object VisitPreprocessor_directive(
            ZSharpPreprocessorParser.Preprocessor_directiveContext context)
        {
            return base.VisitPreprocessor_directive(context);
        }

        public override object VisitPreprocessorConditional(
            ZSharpPreprocessorParser.PreprocessorConditionalContext context)
        {
           
        }
    }
}