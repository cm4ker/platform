using Microsoft.CodeAnalysis;

namespace Aquila.CodeAnalysis.Errors
{
    /// <summary>
    /// A database of all possible diagnostics used by Aquila compiler. The severity can be determined by the prefix:
    /// "FTL_" stands for fatal error, "ERR_" for error, "WRN_" for warning, "INF_" for visible information and
    /// "HDN_" for hidden information. Messages and other information are stored in the resources,
    /// <see cref="ErrorFacts"/> contains the naming logic.
    /// </summary>
    /// <remarks>
    /// New diagnostics must be added to the end of the corresponding severity group in order not to change the
    /// codes of the current ones.
    /// </remarks>
    internal enum ErrorCode
    {
        // 0xxx: reserved
        // 1xxx: reserved
        // 2xxx: reserved

        Void = InternalErrorCode.Void,
        Unknown = InternalErrorCode.Unknown,

        // 
        // Fatal errors
        //
        FTL_InvalidInputFileName = 3000,
        FTL_BadCodepage = 3016,
        FTL_InternalCompilerError = 3017,

        //
        // Errors
        //

        //Syntax
        ERR_SyntaxError = 4000,
        ERR_IdentifierExpectedKW,
        ERR_IdentifierExpected,
        ERR_SemicolonExpected,
        ERR_CloseParenExpected,
        ERR_LbraceExpected,
        ERR_RbraceExpected,
        ERR_IllegalEscape,
        ERR_NewlineInConst,
        ERR_BadDirectivePlacement,
        WRN_ErrorOverride,
        WRN_BadXMLRefSyntax,
        ERR_TypeParamMustBeIdentifier,
        ERR_OvlOperatorExpected,
        ERR_TripleDotNotAllowed,
        ERR_ExpectedVerbatimLiteral,
        ERR_EndifDirectiveExpected,
        ERR_EndRegionDirectiveExpected,
        ERR_UnexpectedCharacter,
        ERR_InternalError,
        ERR_FeatureIsExperimental,
        ERR_FeatureInPreview,
        WRN_LowercaseEllSuffix,
        ERR_LegacyObjectIdSyntax,
        ERR_InvalidReal,
        ERR_InvalidNumber,
        ERR_IntOverflow,
        ERR_FloatOverflow,
        ERR_OpenEndedComment,
        ERR_Merge_conflict_marker_encountered,
        ERR_InsufficientStack,
        ERR_NamespaceNotAllowedInScript,
        ERR_GlobalDefinitionOrStatementExpected,
        ERR_EOFExpected,
        ERR_InvalidExprTerm,
        ERR_BadAwaitAsIdentifier,
        ERR_UsingAfterElements,
        ERR_TopLevelStatementAfterNamespaceOrType,
        ERR_UnexpectedAliasedName,
        ERR_TypeExpected,
        ERR_BadModifierLocation,
        ERR_UnexpectedToken,
        ERR_UnexpectedSemicolon,
        ERR_SemiOrLBraceOrArrowExpected,
        ERR_SemiOrLBraceExpected,
        ERR_InvalidMemberDecl,
        ERR_NewlinesAreNotAllowedInsideANonVerbatimInterpolatedString,
        ERR_UnterminatedStringLit,
        ERR_UnescapedCurly,
        ERR_UnclosedExpressionHole,
        ERR_EscapedCurly,
        ERR_SingleLineCommentInExpressionHole,
        ERR_TooManyCharsInConst,
        ERR_EmptyCharConst,
        WRN_NonECMAFeature,
        ERR_PartialMisplaced,
        ERR_BadArraySyntax,
        ERR_NoVoidParameter,
        ERR_NoVoidHere,
        ERR_IllegalVarianceSyntax,
        ERR_ValueExpected,
        ERR_BadNewExpr,
        WRN_PrecedenceInversion,
        ERR_ConditionalInInterpolation,
        ERR_MissingArgument,
        ERR_ExpressionExpected,
        ERR_AliasQualAsExpression,
        ERR_NamespaceUnexpected,

        //Compilation
        ERR_BadCompilationOptionValue,
        ERR_BadWin32Resource,
        ERR_BinaryFile,
        ERR_CantOpenFileWrite,
        ERR_CantOpenWin32Icon,
        ERR_CantOpenWin32Manifest,
        ERR_CantOpenWin32Resource,
        ERR_CantReadResource,
        ERR_CantReadRulesetFile,
        ERR_CompileCancelled,
        ERR_EncReferenceToAddedMember,
        ERR_ErrorBuildingWin32Resource,
        ERR_ErrorOpeningAssemblyFile,
        ERR_ErrorOpeningModuleFile,
        ERR_ExpectedSingleScript,
        ERR_FailedToCreateTempFile,
        ERR_FileNotFound,
        ERR_InvalidAssemblyMetadata,
        ERR_InvalidDebugInformationFormat,
        ERR_MetadataFileNotAssembly,
        ERR_InvalidFileAlignment,
        ERR_InvalidModuleMetadata,
        ERR_InvalidOutputName,
        ERR_InvalidPathMap,
        ERR_InvalidSubsystemVersion,
        ERR_LinkedNetmoduleMetadataMustProvideFullPEImage,
        ERR_MetadataFileNotFound,
        ERR_MetadataFileNotModule,
        ERR_MetadataNameTooLong,
        ERR_MetadataReferencesNotSupported,
        ERR_NoSourceFile,
        ERR_StartupObjectNotFound,
        ERR_OpenResponseFile,
        ERR_OutputWriteFailed,
        ERR_PdbWritingFailed,
        ERR_PermissionSetAttributeFileReadError,
        ERR_PublicKeyContainerFailure,
        ERR_PublicKeyFileFailure,
        ERR_ResourceFileNameNotUnique,
        ERR_ResourceInModule,
        ERR_ResourceNotUnique,
        ERR_TooManyUserStrings,
        ERR_NotYetImplemented, // Used for all valid Aquila constructs
        ERR_CircularBase,
        ERR_TypeNameCannotBeResolved,
        ERR_PositionalArgAfterUnpacking, // Cannot use positional argument after argument unpacking

        /// <summary>Call to a member function {0} on {1}</summary>
        ERR_MethodCalledOnNonObject,

        /// <summary>Value of type {0} cannot be passed by reference</summary>
        ERR_ValueOfTypeCannotBeAliased,

        /// <summary>"Cannot instantiate {0} {1}", e.g. "interface", the type name</summary>
        ERR_CannotInstantiateType,

        /// <summary>"{0} cannot use {1} - it is not a trait"</summary>
        ERR_CannotUseNonTrait,

        /// <summary>"Class {0} cannot extend from {1} {2}", e.g. from trait T</summary>
        ERR_CannotExtendFrom,

        /// <summary>"{0} cannot implement {1} - it is not an interface"</summary>
        ERR_CannotImplementNonInterface,

        /// <summary>Cannot re-assign $this</summary>
        ERR_CannotAssignToThis,

        /// <summary>{0}() cannot declare a return type</summary>
        ERR_CannotDeclareReturnType,

        /// <summary>A void function must not return a value</summary>
        ERR_VoidFunctionCannotReturnValue,

        /// <summary>{0} {1}() must take exactly {2} arguments</summary>
        ERR_MustTakeArgs,

        /// <summary>Function name must be a string, {0} given</summary>
        ERR_InvalidFunctionName,

        /// <summary>Cannot use the final modifier on an abstract class</summary>
        ERR_FinalAbstractClassDeclared,

        /// <summary>Access level to {0}::${1} must be {2} (as in class {3}) or weaker</summary>
        ERR_PropertyAccessibilityError,

        /// <summary>Use of primitive type '{0}' is misused</summary>
        ERR_PrimitiveTypeNameMisused,

        /// <summary>Missing value for '{0}' option</summary>
        ERR_SwitchNeedsValue,

        /// <summary>'{0}' not in the 'loop' or 'switch' context</summary>
        ERR_NeedsLoopOrSwitch,

        /// <summary>Provided source code kind is unsupported or invalid: '{0}'</summary>
        ERR_BadSourceCodeKind,

        /// <summary>Provided documentation mode is unsupported or invalid: '{0}'.</summary>
        ERR_BadDocumentationMode,

        /// <summary>Compilation options '{0}' and '{1}' can't both be specified at the same time.</summary>
        ERR_MutuallyExclusiveOptions,

        /// <summary>Invalid instrumentation kind: {0}</summary>
        ERR_InvalidInstrumentationKind,

        /// <summary>Invalid hash algorithm name: '{0}'</summary>
        ERR_InvalidHashAlgorithmName,

        /// <summary>Option '{0}' must be an absolute path.</summary>
        ERR_OptionMustBeAbsolutePath,

        /// <summary>Cannot emit debug information for a source text without encoding.</summary>
        ERR_EncodinglessSyntaxTree,

        /// <summary>An error occurred while writing the output file: {0}.</summary>
        ERR_PeWritingFailure,

        /// <summary>Failed to emit module '{0}'.</summary>
        ERR_ModuleEmitFailure,

        /// <summary>Cannot update '{0}'; attribute '{1}' is missing.</summary>
        ERR_EncUpdateFailedMissingAttribute,

        /// <summary>Unable to read debug information of method '{0}' (token 0x{1:X8}) from assembly '{2}'</summary>
        ERR_InvalidDebugInfo,

        /// <summary>Invalid assembly name: {0}</summary>
        ERR_BadAssemblyName,

        /// <summary>/embed switch is only supported when emitting Portable PDB (/debug:portable or /debug:embedded).</summary>
        ERR_CannotEmbedWithoutPdb,

        /// <summary>No overload for method {0} can be called.</summary>
        ERR_NoMatchingOverload,

        /// <summary>Default value for parameter ${0} with a {1} type can only be {1} or NULL, {2} given</summary>
        ERR_DefaultParameterValueTypeMismatch,

        /// <summary>Constant expression contains invalid operations</summary>
        ERR_InvalidConstantExpression,

        /// <summary>Using $this when not in object context</summary>
        ERR_ThisOutOfObjectContext,

        /// <summary>Cannot set read-only property {0}::${1}</summary>
        ERR_ReadOnlyPropertyWritten,

        /// <summary>Only the last parameter can be variadic</summary>
        ERR_VariadicParameterNotLast,
        ERR_CtorPropertyVariadic,
        ERR_CtorPropertyAbstractCtor,
        ERR_CtorPropertyNotCtor,
        ERR_CtorPropertyStaticCtor,

        /// <summary>Property {0}::${1} cannot have type {2}</summary>
        ERR_PropertyTypeNotAllowed,

        /// <summary>Multiple analyzer config files cannot be in the same directory ('{0}').</summary>
        ERR_MultipleAnalyzerConfigsInSameDir,

        /// <summary>Method '{0}' not found for type {1}.</summary>
        ERR_MethodNotFound,


        //
        // Warnings
        //
        //Parse
        WRN_XMLParseError = 5000,


        //Compilation
        WRN_AnalyzerCannotBeCreated,
        WRN_NoAnalyzerInAssembly,
        WRN_NoConfigNotOnCommandLine,
        WRN_PdbLocalNameTooLong,
        WRN_PdbUsingNameTooLong,
        WRN_UnableToLoadAnalyzer,
        WRN_UndefinedFunctionCall,
        WRN_UninitializedVariableUse,
        WRN_UndefinedType,
        WRN_UndefinedMethodCall,

        /// <summary>The declaration of class, interface or trait is ambiguous since its base types cannot be resolved.</summary>
        WRN_AmbiguousDeclaration,
        WRN_UnreachableCode,
        WRN_NotYetImplementedIgnored,
        WRN_NoSourceFiles,

        /// <summary>{0}() expects {1} parameter(s), {2} given</summary>
        WRN_TooManyArguments,

        /// <summary>{0}() expects at least {1} parameter(s), {2} given</summary>
        WRN_MissingArguments,

        /// <summary>Assertion will always fail</summary>
        WRN_AssertAlwaysFail,

        /// <summary>Using string as the assertion is deprecated</summary>
        WRN_StringAssertionDeprecated,

        /// <summary>Deprecated: {0} '{1}' has been deprecated. {2}</summary>
        WRN_SymbolDeprecated,

        /// <summary>The expression is not being read. Did you mean to assign it somewhere?</summary>
        WRN_ExpressionNotRead,

        /// <summary>Assignment made to same variable; did you mean to assign something else?</summary>
        WRN_AssigningSameVariable,

        /// <summary>Invalid array key type: {0}.</summary>
        WRN_InvalidArrayKeyType,

        /// <summary>Duplicate array key: '{0}'.</summary>
        WRN_DuplicateArrayKey,

        /// <summary>Cloning of non-object: {0}.</summary>
        WRN_CloneNonObject,

        /// <summary>Using non-iterable type in foreach: {0}.</summary>
        WRN_ForeachNonIterable,

        /// <summary>Call to '{0}()' expects {1} argument(s), {2} given.</summary>
        WRN_FormatStringWrongArgCount,

        /// <summary>Missing the call of parent::__construct from {0}::__construct.</summary>
        WRN_ParentCtorNotCalled,

        /// <summary>Method {0}::__toString() must return a string value</summary>
        WRN_ToStringMustReturnString,

        /// <summary>Argument has no value, parameter will be always NULL</summary>
        WRN_ArgumentVoid,

        /// <summary>PCRE pattern parse error: {0} at offset {1}</summary>
        WRN_PCRE_Pattern_Error,

        /// <summary>{0} '{1}' is already defined</summary>
        WRN_TypeNameInUse,

        /// <summary>Script file '{0}' could not be resolved, the script inclusion is unbound.</summary>
        WRN_CannotIncludeFile,

        /// <summary>Called from the global scope</summary>
        WRN_CalledFromGlobalScope,

        /// <summary>Generator '{0}' failed to initialize. It will not contribute to the output and compilation errors may occur as a result. Exception was of type '{1}' with message '{2}'</summary>
        WRN_GeneratorFailedDuringInitialization,

        /// <summary>Generator '{0}' failed to generate source. It will not contribute to the output and compilation errors may occur as a result. Exception was of type '{1}' with message '{2}'</summary>
        WRN_GeneratorFailedDuringGeneration,

        //
        // Visible information
        //
        INF_UnableToLoadSomeTypesInAnalyzer = 6000,
        INF_EvalDiscouraged,
        INF_RedundantCast,

        /// <summary>Name '{0}' does not match the expected name '{1}', letter casing mismatch.</summary>
        INF_TypeNameCaseMismatch,

        /// <summary></summary>
        INF_DestructDiscouraged,

        /// <summary>Overriden function name '{0}' does not match it's parent name '{1}', letter casing mismatch.</summary>
        INF_OverrideNameCaseMismatch,

        /// <summary>The symbol can not been resolved </summary>
        INF_CantResolveSymbol
    }
}