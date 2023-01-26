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
        ERR_NotYetImplemented = 4121,
        ERR_TypeNameCannotBeResolved = 4123, // Cannot use positional argument after argument unpacking

        ERR_InvalidMetadataConsistance = 4125,
        ERR_CannotInstantiateType = 4128,
        ERR_CannotAssignToThis = 4132,
        ERR_PrimitiveTypeNameMisused = 4139,
        ERR_SwitchNeedsValue = 4140,
        ERR_NeedsLoopOrSwitch = 4141,
        ERR_BadSourceCodeKind = 4142,
        ERR_BadDocumentationMode = 4143,
        ERR_MutuallyExclusiveOptions = 4144,
        ERR_InvalidInstrumentationKind = 4145,
        ERR_InvalidHashAlgorithmName = 4146,
        ERR_OptionMustBeAbsolutePath = 4147,
        ERR_EncodinglessSyntaxTree = 4148,
        ERR_PeWritingFailure = 4149,
        ERR_ModuleEmitFailure = 4150,
        ERR_EncUpdateFailedMissingAttribute = 4151,
        ERR_InvalidDebugInfo = 4152,
        ERR_BadAssemblyName = 4153,
        ERR_CannotEmbedWithoutPdb = 4154,
        ERR_InvalidConstantExpression = 4157,
        ERR_ReadOnlyPropertyWritten = 4159,
        ERR_MultipleAnalyzerConfigsInSameDir = 4166,
        ERR_MethodNotFound = 4167,
        ERR_MissingIdentifierSymbol = 4168,
        ERR_CantResolveSymbol = 4169,

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
        WRN_UninitializedVariableUse = 5008,
        WRN_UndefinedType = 5009,
        WRN_UnreachableCode = 5012,
        WRN_NoSourceFiles = 5014,
        WRN_FormatStringWrongArgCount = 5026,
        WRN_ParentCtorNotCalled = 5027,
        WRN_GeneratorFailedDuringInitialization = 5034,
        WRN_GeneratorFailedDuringGeneration = 5035,

        #endregion

        #region 6xxx Visible information

        INF_UnableToLoadSomeTypesInAnalyzer = 6000,
        INF_TypeNameCaseMismatch = 6003,

        #endregion
    }
}