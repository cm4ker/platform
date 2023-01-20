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

        #region 3xxx Fatal errors

        FTL_InvalidInputFileName = 3000,
        FTL_BadCodepage = 3016,
        FTL_InternalCompilerError = 3017,

        #endregion

        #region 4xxx Errors

        #region Syntax

        ERR_SyntaxError = 4000,
        ERR_IdentifierExpectedKW = 4001,
        ERR_IdentifierExpected = 4002,
        ERR_SemicolonExpected = 4003,
        ERR_CloseParenExpected = 4004,
        ERR_LbraceExpected = 4005,
        ERR_RbraceExpected = 4006,
        ERR_IllegalEscape = 4007,
        ERR_NewlineInConst = 4008,
        ERR_BadDirectivePlacement = 4009,

        WRN_ErrorOverride = 4010,
        WRN_BadXMLRefSyntax = 4011,
        ERR_TypeParamMustBeIdentifier = 4012,
        ERR_OvlOperatorExpected = 4013,
        ERR_TripleDotNotAllowed = 4014,
        ERR_ExpectedVerbatimLiteral = 4015,
        ERR_EndifDirectiveExpected = 4016,
        ERR_EndRegionDirectiveExpected = 4017,
        ERR_UnexpectedCharacter = 4018,
        ERR_InternalError = 4019,

        ERR_FeatureIsExperimental = 4020,
        ERR_FeatureInPreview = 4021,
        WRN_LowercaseEllSuffix = 4022,
        ERR_LegacyObjectIdSyntax = 4023,
        ERR_InvalidReal = 4024,
        ERR_InvalidNumber = 4025,
        ERR_IntOverflow = 4026,
        ERR_FloatOverflow = 4027,
        ERR_OpenEndedComment = 4028,
        ERR_Merge_conflict_marker_encountered = 4029,

        ERR_InsufficientStack = 4030,
        ERR_NamespaceNotAllowedInScript = 4031,
        ERR_GlobalDefinitionOrStatementExpected = 4032,
        ERR_EOFExpected = 4033,
        ERR_InvalidExprTerm = 4034,
        ERR_BadAwaitAsIdentifier = 4035,
        ERR_UsingAfterElements = 4036,
        ERR_TopLevelStatementAfterNamespaceOrType = 4037,
        ERR_UnexpectedAliasedName = 4038,
        ERR_TypeExpected = 4039,
        ERR_BadModifierLocation = 4040,
        ERR_UnexpectedToken = 4041,
        ERR_UnexpectedSemicolon = 4042,
        ERR_SemiOrLBraceOrArrowExpected = 4043,
        ERR_SemiOrLBraceExpected = 4044,
        ERR_InvalidMemberDecl = 4045,
        ERR_NewlinesAreNotAllowedInsideANonVerbatimInterpolatedString = 4046,
        ERR_UnterminatedStringLit = 4047,
        ERR_UnescapedCurly = 4048,
        ERR_UnclosedExpressionHole = 4049,
        ERR_EscapedCurly = 4050,
        ERR_SingleLineCommentInExpressionHole = 4051,
        ERR_TooManyCharsInConst = 4052,
        ERR_EmptyCharConst = 4053,
        WRN_NonECMAFeature = 4054,
        ERR_PartialMisplaced = 4055,
        ERR_BadArraySyntax = 4056,
        ERR_NoVoidParameter = 4057,
        ERR_NoVoidHere = 4058,
        ERR_IllegalVarianceSyntax = 4059,
        ERR_ValueExpected = 4060,
        ERR_BadNewExpr = 4061,
        WRN_PrecedenceInversion = 4062,
        ERR_ConditionalInInterpolation = 4063,
        ERR_MissingArgument = 4064,
        ERR_ExpressionExpected = 4065,
        ERR_AliasQualAsExpression = 4066,
        ERR_NamespaceUnexpected = 4067,
        ERR_BadMemberFlag = 4068,
        ERR_ConstValueRequired = 4069,
        ERR_FixedDimsRequired = 4070,
        ERR_CStyleArray = 4071,
        ERR_ArraySizeInDeclaration = 4072,
        ERR_BadVarDecl = 4073,
        ERR_MultiTypeInDeclaration = 4074,
        ERR_ElseCannotStartStatement = 4075,
        ERR_InExpected = 4076,
        ERR_ExpectedEndTry = 4077,
        ERR_NameExpected = 4078,

        #endregion

        #region Compilation

        ERR_BadCompilationOptionValue = 4079,
        ERR_BadWin32Resource = 4080,
        ERR_BinaryFile = 4081,
        ERR_CantOpenFileWrite = 4082,
        ERR_CantOpenWin32Icon = 4083,
        ERR_CantOpenWin32Manifest = 4084,
        ERR_CantOpenWin32Resource = 4085,
        ERR_CantReadResource = 4086,
        ERR_CantReadRulesetFile = 4087,
        ERR_CompileCancelled = 4088,
        ERR_EncReferenceToAddedMember = 4089,
        ERR_ErrorBuildingWin32Resource = 4090,
        ERR_ErrorOpeningAssemblyFile = 4091,
        ERR_ErrorOpeningModuleFile = 4092,
        ERR_ExpectedSingleScript = 4093,
        ERR_FailedToCreateTempFile = 4094,
        ERR_FileNotFound = 4095,
        ERR_InvalidAssemblyMetadata = 4096,
        ERR_InvalidDebugInformationFormat = 4097,
        ERR_MetadataFileNotAssembly = 4098,
        ERR_InvalidFileAlignment = 4099,
        ERR_InvalidModuleMetadata = 4100,
        ERR_InvalidOutputName = 4101,
        ERR_InvalidPathMap = 4102,
        ERR_InvalidSubsystemVersion = 4103,
        ERR_LinkedNetmoduleMetadataMustProvideFullPEImage = 4104,
        ERR_MetadataFileNotFound = 4105,
        ERR_MetadataFileNotModule = 4106,
        ERR_MetadataNameTooLong = 4107,
        ERR_MetadataReferencesNotSupported = 4108,
        ERR_NoSourceFile = 4109,
        ERR_StartupObjectNotFound = 4110,
        ERR_OpenResponseFile = 4111,
        ERR_OutputWriteFailed = 4112,
        ERR_PdbWritingFailed = 4113,
        ERR_PermissionSetAttributeFileReadError = 4114,
        ERR_PublicKeyContainerFailure = 4115,
        ERR_PublicKeyFileFailure = 4116,
        ERR_ResourceFileNameNotUnique = 4117,
        ERR_ResourceInModule = 4118,
        ERR_ResourceNotUnique = 4119,
        ERR_TooManyUserStrings = 4120,
        ERR_NotYetImplemented = 4121, // Used for all valid Aquila constructs
        ERR_CircularBase = 4122,
        ERR_TypeNameCannotBeResolved = 4123,
        ERR_PositionalArgAfterUnpacking = 4124, // Cannot use positional argument after argument unpacking

        ERR_InvalidMetadataConsistance = 4125,

        /// <summary>Call to a member function {0} on {1}</summary>
        ERR_MethodCalledOnNonObject = 4126,

        /// <summary>Value of type {0} cannot be passed by reference</summary>
        ERR_ValueOfTypeCannotBeAliased = 4127,

        /// <summary>"Cannot instantiate {0} {1}", e.g. "interface", the type name</summary>
        ERR_CannotInstantiateType = 4128,

        /// <summary>"{0} cannot use {1} - it is not a trait"</summary>
        ERR_CannotUseNonTrait = 4129,

        /// <summary>"Class {0} cannot extend from {1} {2}", e.g. from trait T</summary>
        ERR_CannotExtendFrom = 4130,

        /// <summary>"{0} cannot implement {1} - it is not an interface"</summary>
        ERR_CannotImplementNonInterface = 4131,

        /// <summary>Cannot re-assign $this</summary>
        ERR_CannotAssignToThis = 4132,

        /// <summary>{0}() cannot declare a return type</summary>
        ERR_CannotDeclareReturnType = 4133,

        /// <summary>A void function must not return a value</summary>
        ERR_VoidFunctionCannotReturnValue = 4134,

        /// <summary>{0} {1}() must take exactly {2} arguments</summary>
        ERR_MustTakeArgs = 4135,

        /// <summary>Function name must be a string, {0} given</summary>
        ERR_InvalidFunctionName = 4136,

        /// <summary>Cannot use the final modifier on an abstract class</summary>
        ERR_FinalAbstractClassDeclared = 4137,

        /// <summary>Access level to {0}::${1} must be {2} (as in class {3}) or weaker</summary>
        ERR_PropertyAccessibilityError = 4138,

        /// <summary>Use of primitive type '{0}' is misused</summary>
        ERR_PrimitiveTypeNameMisused = 4139,

        /// <summary>Missing value for '{0}' option</summary>
        ERR_SwitchNeedsValue = 4140,

        /// <summary>'{0}' not in the 'loop' or 'switch' context</summary>
        ERR_NeedsLoopOrSwitch = 4141,

        /// <summary>Provided source code kind is unsupported or invalid: '{0}'</summary>
        ERR_BadSourceCodeKind = 4142,

        /// <summary>Provided documentation mode is unsupported or invalid: '{0}'.</summary>
        ERR_BadDocumentationMode = 4143,

        /// <summary>Compilation options '{0}' and '{1}' can't both be specified at the same time.</summary>
        ERR_MutuallyExclusiveOptions = 4144,

        /// <summary>Invalid instrumentation kind: {0}</summary>
        ERR_InvalidInstrumentationKind = 4145,

        /// <summary>Invalid hash algorithm name: '{0}'</summary>
        ERR_InvalidHashAlgorithmName = 4146,

        /// <summary>Option '{0}' must be an absolute path.</summary>
        ERR_OptionMustBeAbsolutePath = 4147,

        /// <summary>Cannot emit debug information for a source text without encoding.</summary>
        ERR_EncodinglessSyntaxTree = 4148,

        /// <summary>An error occurred while writing the output file: {0}.</summary>
        ERR_PeWritingFailure = 4149,

        /// <summary>Failed to emit module '{0}'.</summary>
        ERR_ModuleEmitFailure = 4150,

        /// <summary>Cannot update '{0}'; attribute '{1}' is missing.</summary>
        ERR_EncUpdateFailedMissingAttribute = 4151,

        /// <summary>Unable to read debug information of method '{0}' (token 0x{1:X8}) from assembly '{2}'</summary>
        ERR_InvalidDebugInfo = 4152,

        /// <summary>Invalid assembly name: {0}</summary>
        ERR_BadAssemblyName = 4153,

        /// <summary>/embed switch is only supported when emitting Portable PDB (/debug:portable or /debug:embedded).</summary>
        ERR_CannotEmbedWithoutPdb = 4154,

        /// <summary>No overload for method {0} can be called.</summary>
        ERR_NoMatchingOverload = 4155,

        /// <summary>Default value for parameter ${0} with a {1} type can only be {1} or NULL, {2} given</summary>
        ERR_DefaultParameterValueTypeMismatch = 4156,

        /// <summary>Constant expression contains invalid operations</summary>
        ERR_InvalidConstantExpression = 4157,

        /// <summary>Using $this when not in object context</summary>
        ERR_ThisOutOfObjectContext = 4158,

        /// <summary>Cannot set read-only property {0}::${1}</summary>
        ERR_ReadOnlyPropertyWritten = 4159,

        /// <summary>Only the last parameter can be variadic</summary>
        ERR_VariadicParameterNotLast = 4160,
        ERR_CtorPropertyVariadic = 4161,
        ERR_CtorPropertyAbstractCtor = 4162,
        ERR_CtorPropertyNotCtor = 4163,
        ERR_CtorPropertyStaticCtor = 4164,

        /// <summary>Property {0}::${1} cannot have type {2}</summary>
        ERR_PropertyTypeNotAllowed = 4165,

        /// <summary>Multiple analyzer config files cannot be in the same directory ('{0}').</summary>
        ERR_MultipleAnalyzerConfigsInSameDir = 4166,

        /// <summary>Method '{0}' not found for type {1}.</summary>
        ERR_MethodNotFound = 4167,

        ERR_MissingIdentifierSymbol = 4168,

        #endregion

        #endregion

        #region 5xxx Warnings

        //Parse
        WRN_XMLParseError = 5000,

        //Compilation
        WRN_AnalyzerCannotBeCreated = 5001,
        WRN_NoAnalyzerInAssembly = 5002,
        WRN_NoConfigNotOnCommandLine = 5003,
        WRN_PdbLocalNameTooLong = 5004,
        WRN_PdbUsingNameTooLong = 5005,
        WRN_UnableToLoadAnalyzer = 5006,
        WRN_UndefinedFunctionCall = 5007,
        WRN_UninitializedVariableUse = 5008,
        WRN_UndefinedType = 5009,
        WRN_UndefinedMethodCall = 5010,

        /// <summary>The declaration of class, interface or trait is ambiguous since its base types cannot be resolved.</summary>
        WRN_AmbiguousDeclaration = 5011,
        WRN_UnreachableCode = 5012,
        WRN_NotYetImplementedIgnored = 5013,
        WRN_NoSourceFiles = 5014,

        /// <summary>{0}() expects {1} parameter(s), {2} given</summary>
        WRN_TooManyArguments = 5015,

        /// <summary>{0}() expects at least {1} parameter(s), {2} given</summary>
        WRN_MissingArguments = 5016,

        /// <summary>Assertion will always fail</summary>
        WRN_AssertAlwaysFail = 5017,

        /// <summary>Using string as the assertion is deprecated</summary>
        WRN_StringAssertionDeprecated = 5018,

        /// <summary>Deprecated: {0} '{1}' has been deprecated. {2}</summary>
        WRN_SymbolDeprecated = 5019,

        /// <summary>The expression is not being read. Did you mean to assign it somewhere?</summary>
        WRN_ExpressionNotRead = 5020,

        /// <summary>Assignment made to same variable; did you mean to assign something else?</summary>
        WRN_AssigningSameVariable = 5021,

        /// <summary>Invalid array key type: {0}.</summary>
        WRN_InvalidArrayKeyType = 5022,

        /// <summary>Duplicate array key: '{0}'.</summary>
        WRN_DuplicateArrayKey = 5023,

        /// <summary>Cloning of non-object: {0}.</summary>
        WRN_CloneNonObject = 5024,

        /// <summary>Using non-iterable type in foreach: {0}.</summary>
        WRN_ForeachNonIterable = 5025,

        /// <summary>Call to '{0}()' expects {1} argument(s), {2} given.</summary>
        WRN_FormatStringWrongArgCount = 5026,

        /// <summary>Missing the call of parent::__construct from {0}::__construct.</summary>
        WRN_ParentCtorNotCalled = 5027,

        /// <summary>Method {0}::__toString() must return a string value</summary>
        WRN_ToStringMustReturnString = 5028,

        /// <summary>Argument has no value, parameter will be always NULL</summary>
        WRN_ArgumentVoid = 5029,

        /// <summary>PCRE pattern parse error: {0} at offset {1}</summary>
        WRN_PCRE_Pattern_Error = 5030,

        /// <summary>{0} '{1}' is already defined</summary>
        WRN_TypeNameInUse = 5031,

        /// <summary>Script file '{0}' could not be resolved, the script inclusion is unbound.</summary>
        WRN_CannotIncludeFile = 5032,

        /// <summary>Called from the global scope</summary>
        WRN_CalledFromGlobalScope = 5033,

        /// <summary>Generator '{0}' failed to initialize. It will not contribute to the output and compilation errors may occur as a result. Exception was of type '{1}' with message '{2}'</summary>
        WRN_GeneratorFailedDuringInitialization = 5034,

        /// <summary>Generator '{0}' failed to generate source. It will not contribute to the output and compilation errors may occur as a result. Exception was of type '{1}' with message '{2}'</summary>
        WRN_GeneratorFailedDuringGeneration = 5035,

        #endregion

        #region 6xxx Visible information

        //
        // Visible information
        //
        INF_UnableToLoadSomeTypesInAnalyzer = 6000,
        INF_EvalDiscouraged = 6001,
        INF_RedundantCast = 6002,

        /// <summary>Name '{0}' does not match the expected name '{1}', letter casing mismatch.</summary>
        INF_TypeNameCaseMismatch = 6003,

        /// <summary></summary>
        INF_DestructDiscouraged = 6004,

        /// <summary>Overriden function name '{0}' does not match it's parent name '{1}', letter casing mismatch.</summary>
        INF_OverrideNameCaseMismatch = 6005,

        /// <summary>The symbol can not been resolved </summary>
        INF_CantResolveSymbol = 6006

        #endregion
    }
}