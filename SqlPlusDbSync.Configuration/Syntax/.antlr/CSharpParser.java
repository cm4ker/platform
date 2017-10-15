// Generated from c:\Users\n.zenkov\Documents\DEV_ASNA\ASNA\Devs\SqlPlusDbSync\SqlPlusDbSync\SqlPlusDbSync.Platform\Syntax/CSharpParser.g4 by ANTLR 4.7
import org.antlr.v4.runtime.atn.*;
import org.antlr.v4.runtime.dfa.DFA;
import org.antlr.v4.runtime.*;
import org.antlr.v4.runtime.misc.*;
import org.antlr.v4.runtime.tree.*;
import java.util.List;
import java.util.Iterator;
import java.util.ArrayList;

@SuppressWarnings({"all", "warnings", "unchecked", "unused", "cast"})
public class CSharpParser extends Parser {
	static { RuntimeMetaData.checkVersion("4.7", RuntimeMetaData.VERSION); }

	protected static final DFA[] _decisionToDFA;
	protected static final PredictionContextCache _sharedContextCache =
		new PredictionContextCache();
	public static final int
		BYTE_ORDER_MARK=1, SINGLE_LINE_DOC_COMMENT=2, DELIMITED_DOC_COMMENT=3, 
		SINGLE_LINE_COMMENT=4, DELIMITED_COMMENT=5, WHITESPACES=6, SHARP=7, ABSTRACT=8, 
		ADD=9, ALIAS=10, ARGLIST=11, AS=12, ASCENDING=13, ASYNC=14, AWAIT=15, 
		BASE=16, BOOL=17, BREAK=18, BY=19, BYTE=20, CASE=21, CATCH=22, CHAR=23, 
		CHECKED=24, CLASS=25, CONST=26, CONTINUE=27, DECIMAL=28, DEFAULT=29, DELEGATE=30, 
		DESCENDING=31, DO=32, DOUBLE=33, DYNAMIC=34, ELSE=35, ENUM=36, EQUALS=37, 
		EVENT=38, EXPLICIT=39, EXTERN=40, FALSE=41, FINALLY=42, FIXED=43, FLOAT=44, 
		FOR=45, FOREACH=46, FROM=47, GET=48, GOTO=49, GROUP=50, IF=51, IMPLICIT=52, 
		IN=53, INT=54, INTERFACE=55, INTERNAL=56, INTO=57, IS=58, JOIN=59, LET=60, 
		LOCK=61, LONG=62, NAMEOF=63, NAMESPACE=64, NEW=65, NULL=66, OBJECT=67, 
		ON=68, OPERATOR=69, ORDERBY=70, OUT=71, OVERRIDE=72, PARAMS=73, PARTIAL=74, 
		PRIVATE=75, PROTECTED=76, PUBLIC=77, READONLY=78, REF=79, REMOVE=80, RETURN=81, 
		SBYTE=82, SEALED=83, SELECT=84, SET=85, SHORT=86, SIZEOF=87, STACKALLOC=88, 
		STATIC=89, STRING=90, STRUCT=91, SWITCH=92, THIS=93, THROW=94, TRUE=95, 
		TRY=96, TYPEOF=97, UINT=98, ULONG=99, UNCHECKED=100, UNSAFE=101, USHORT=102, 
		USING=103, VIRTUAL=104, VOID=105, VOLATILE=106, WHEN=107, WHERE=108, WHILE=109, 
		YIELD=110, IDENTIFIER=111, LITERAL_ACCESS=112, INTEGER_LITERAL=113, HEX_INTEGER_LITERAL=114, 
		REAL_LITERAL=115, CHARACTER_LITERAL=116, REGULAR_STRING=117, VERBATIUM_STRING=118, 
		INTERPOLATED_REGULAR_STRING_START=119, INTERPOLATED_VERBATIUM_STRING_START=120, 
		OPEN_BRACE=121, CLOSE_BRACE=122, OPEN_BRACKET=123, CLOSE_BRACKET=124, 
		OPEN_PARENS=125, CLOSE_PARENS=126, DOT=127, COMMA=128, COLON=129, SEMICOLON=130, 
		PLUS=131, MINUS=132, STAR=133, DIV=134, PERCENT=135, AMP=136, BITWISE_OR=137, 
		CARET=138, BANG=139, TILDE=140, ASSIGNMENT=141, LT=142, GT=143, INTERR=144, 
		DOUBLE_COLON=145, OP_COALESCING=146, OP_INC=147, OP_DEC=148, OP_AND=149, 
		OP_OR=150, OP_PTR=151, OP_EQ=152, OP_NE=153, OP_LE=154, OP_GE=155, OP_ADD_ASSIGNMENT=156, 
		OP_SUB_ASSIGNMENT=157, OP_MULT_ASSIGNMENT=158, OP_DIV_ASSIGNMENT=159, 
		OP_MOD_ASSIGNMENT=160, OP_AND_ASSIGNMENT=161, OP_OR_ASSIGNMENT=162, OP_XOR_ASSIGNMENT=163, 
		OP_LEFT_SHIFT=164, OP_LEFT_SHIFT_ASSIGNMENT=165, DOUBLE_CURLY_INSIDE=166, 
		OPEN_BRACE_INSIDE=167, REGULAR_CHAR_INSIDE=168, VERBATIUM_DOUBLE_QUOTE_INSIDE=169, 
		DOUBLE_QUOTE_INSIDE=170, REGULAR_STRING_INSIDE=171, VERBATIUM_INSIDE_STRING=172, 
		CLOSE_BRACE_INSIDE=173, FORMAT_STRING=174, DIRECTIVE_WHITESPACES=175, 
		DIGITS=176, DEFINE=177, UNDEF=178, ELIF=179, ENDIF=180, LINE=181, ERROR=182, 
		WARNING=183, REGION=184, ENDREGION=185, PRAGMA=186, DIRECTIVE_HIDDEN=187, 
		CONDITIONAL_SYMBOL=188, DIRECTIVE_NEW_LINE=189, TEXT=190, DOUBLE_CURLY_CLOSE_INSIDE=191;
	public static final int
		RULE_compilation_unit = 0, RULE_namespace_or_type_name = 1, RULE_type = 2, 
		RULE_base_type = 3, RULE_simple_type = 4, RULE_numeric_type = 5, RULE_integral_type = 6, 
		RULE_floating_point_type = 7, RULE_class_type = 8, RULE_type_argument_list = 9, 
		RULE_argument_list = 10, RULE_argument = 11, RULE_expression = 12, RULE_non_assignment_expression = 13, 
		RULE_assignment = 14, RULE_assignment_operator = 15, RULE_conditional_expression = 16, 
		RULE_null_coalescing_expression = 17, RULE_conditional_or_expression = 18, 
		RULE_conditional_and_expression = 19, RULE_inclusive_or_expression = 20, 
		RULE_exclusive_or_expression = 21, RULE_and_expression = 22, RULE_equality_expression = 23, 
		RULE_relational_expression = 24, RULE_shift_expression = 25, RULE_additive_expression = 26, 
		RULE_multiplicative_expression = 27, RULE_unary_expression = 28, RULE_primary_expression = 29, 
		RULE_primary_expression_start = 30, RULE_member_access = 31, RULE_bracket_expression = 32, 
		RULE_indexer_argument = 33, RULE_predefined_type = 34, RULE_expression_list = 35, 
		RULE_object_or_collection_initializer = 36, RULE_object_initializer = 37, 
		RULE_member_initializer_list = 38, RULE_member_initializer = 39, RULE_initializer_value = 40, 
		RULE_collection_initializer = 41, RULE_element_initializer = 42, RULE_anonymous_object_initializer = 43, 
		RULE_member_declarator_list = 44, RULE_member_declarator = 45, RULE_unbound_type_name = 46, 
		RULE_generic_dimension_specifier = 47, RULE_isType = 48, RULE_lambda_expression = 49, 
		RULE_anonymous_function_signature = 50, RULE_explicit_anonymous_function_parameter_list = 51, 
		RULE_explicit_anonymous_function_parameter = 52, RULE_implicit_anonymous_function_parameter_list = 53, 
		RULE_anonymous_function_body = 54, RULE_query_expression = 55, RULE_from_clause = 56, 
		RULE_query_body = 57, RULE_query_body_clause = 58, RULE_let_clause = 59, 
		RULE_where_clause = 60, RULE_combined_join_clause = 61, RULE_orderby_clause = 62, 
		RULE_ordering = 63, RULE_select_or_group_clause = 64, RULE_query_continuation = 65, 
		RULE_statement = 66, RULE_embedded_statement = 67, RULE_simple_embedded_statement = 68, 
		RULE_block = 69, RULE_local_variable_declaration = 70, RULE_local_variable_declarator = 71, 
		RULE_local_variable_initializer = 72, RULE_local_constant_declaration = 73, 
		RULE_if_body = 74, RULE_switch_section = 75, RULE_switch_label = 76, RULE_statement_list = 77, 
		RULE_for_initializer = 78, RULE_for_iterator = 79, RULE_catch_clauses = 80, 
		RULE_specific_catch_clause = 81, RULE_general_catch_clause = 82, RULE_exception_filter = 83, 
		RULE_finally_clause = 84, RULE_resource_acquisition = 85, RULE_namespace_declaration = 86, 
		RULE_qualified_identifier = 87, RULE_namespace_body = 88, RULE_extern_alias_directives = 89, 
		RULE_extern_alias_directive = 90, RULE_using_directives = 91, RULE_using_directive = 92, 
		RULE_namespace_member_declarations = 93, RULE_namespace_member_declaration = 94, 
		RULE_type_declaration = 95, RULE_qualified_alias_member = 96, RULE_type_parameter_list = 97, 
		RULE_type_parameter = 98, RULE_class_base = 99, RULE_interface_type_list = 100, 
		RULE_type_parameter_constraints_clauses = 101, RULE_type_parameter_constraints_clause = 102, 
		RULE_type_parameter_constraints = 103, RULE_primary_constraint = 104, 
		RULE_secondary_constraints = 105, RULE_constructor_constraint = 106, RULE_class_body = 107, 
		RULE_class_member_declarations = 108, RULE_class_member_declaration = 109, 
		RULE_all_member_modifiers = 110, RULE_all_member_modifier = 111, RULE_common_member_declaration = 112, 
		RULE_typed_member_declaration = 113, RULE_constant_declarators = 114, 
		RULE_constant_declarator = 115, RULE_variable_declarators = 116, RULE_variable_declarator = 117, 
		RULE_variable_initializer = 118, RULE_return_type = 119, RULE_member_name = 120, 
		RULE_method_body = 121, RULE_formal_parameter_list = 122, RULE_fixed_parameters = 123, 
		RULE_fixed_parameter = 124, RULE_parameter_modifier = 125, RULE_parameter_array = 126, 
		RULE_accessor_declarations = 127, RULE_get_accessor_declaration = 128, 
		RULE_set_accessor_declaration = 129, RULE_accessor_modifier = 130, RULE_accessor_body = 131, 
		RULE_event_accessor_declarations = 132, RULE_add_accessor_declaration = 133, 
		RULE_remove_accessor_declaration = 134, RULE_overloadable_operator = 135, 
		RULE_conversion_operator_declarator = 136, RULE_constructor_initializer = 137, 
		RULE_body = 138, RULE_struct_interfaces = 139, RULE_struct_body = 140, 
		RULE_struct_member_declaration = 141, RULE_array_type = 142, RULE_rank_specifier = 143, 
		RULE_array_initializer = 144, RULE_variant_type_parameter_list = 145, 
		RULE_variant_type_parameter = 146, RULE_variance_annotation = 147, RULE_interface_base = 148, 
		RULE_interface_body = 149, RULE_interface_member_declaration = 150, RULE_interface_accessors = 151, 
		RULE_enum_base = 152, RULE_enum_body = 153, RULE_enum_member_declaration = 154, 
		RULE_global_attribute_section = 155, RULE_global_attribute_target = 156, 
		RULE_attributes = 157, RULE_attribute_section = 158, RULE_attribute_target = 159, 
		RULE_attribute_list = 160, RULE_attribute = 161, RULE_attribute_argument = 162, 
		RULE_pointer_type = 163, RULE_fixed_pointer_declarators = 164, RULE_fixed_pointer_declarator = 165, 
		RULE_fixed_pointer_initializer = 166, RULE_fixed_size_buffer_declarator = 167, 
		RULE_local_variable_initializer_unsafe = 168, RULE_right_arrow = 169, 
		RULE_right_shift = 170, RULE_right_shift_assignment = 171, RULE_literal = 172, 
		RULE_boolean_literal = 173, RULE_string_literal = 174, RULE_interpolated_regular_string = 175, 
		RULE_interpolated_verbatium_string = 176, RULE_interpolated_regular_string_part = 177, 
		RULE_interpolated_verbatium_string_part = 178, RULE_interpolated_string_expression = 179, 
		RULE_keyword = 180, RULE_class_definition = 181, RULE_struct_definition = 182, 
		RULE_interface_definition = 183, RULE_enum_definition = 184, RULE_delegate_definition = 185, 
		RULE_event_declaration = 186, RULE_field_declaration = 187, RULE_property_declaration = 188, 
		RULE_constant_declaration = 189, RULE_indexer_declaration = 190, RULE_destructor_definition = 191, 
		RULE_constructor_declaration = 192, RULE_method_declaration = 193, RULE_method_member_name = 194, 
		RULE_operator_declaration = 195, RULE_arg_declaration = 196, RULE_method_invocation = 197, 
		RULE_object_creation_expression = 198, RULE_identifier = 199;
	public static final String[] ruleNames = {
		"compilation_unit", "namespace_or_type_name", "type", "base_type", "simple_type", 
		"numeric_type", "integral_type", "floating_point_type", "class_type", 
		"type_argument_list", "argument_list", "argument", "expression", "non_assignment_expression", 
		"assignment", "assignment_operator", "conditional_expression", "null_coalescing_expression", 
		"conditional_or_expression", "conditional_and_expression", "inclusive_or_expression", 
		"exclusive_or_expression", "and_expression", "equality_expression", "relational_expression", 
		"shift_expression", "additive_expression", "multiplicative_expression", 
		"unary_expression", "primary_expression", "primary_expression_start", 
		"member_access", "bracket_expression", "indexer_argument", "predefined_type", 
		"expression_list", "object_or_collection_initializer", "object_initializer", 
		"member_initializer_list", "member_initializer", "initializer_value", 
		"collection_initializer", "element_initializer", "anonymous_object_initializer", 
		"member_declarator_list", "member_declarator", "unbound_type_name", "generic_dimension_specifier", 
		"isType", "lambda_expression", "anonymous_function_signature", "explicit_anonymous_function_parameter_list", 
		"explicit_anonymous_function_parameter", "implicit_anonymous_function_parameter_list", 
		"anonymous_function_body", "query_expression", "from_clause", "query_body", 
		"query_body_clause", "let_clause", "where_clause", "combined_join_clause", 
		"orderby_clause", "ordering", "select_or_group_clause", "query_continuation", 
		"statement", "embedded_statement", "simple_embedded_statement", "block", 
		"local_variable_declaration", "local_variable_declarator", "local_variable_initializer", 
		"local_constant_declaration", "if_body", "switch_section", "switch_label", 
		"statement_list", "for_initializer", "for_iterator", "catch_clauses", 
		"specific_catch_clause", "general_catch_clause", "exception_filter", "finally_clause", 
		"resource_acquisition", "namespace_declaration", "qualified_identifier", 
		"namespace_body", "extern_alias_directives", "extern_alias_directive", 
		"using_directives", "using_directive", "namespace_member_declarations", 
		"namespace_member_declaration", "type_declaration", "qualified_alias_member", 
		"type_parameter_list", "type_parameter", "class_base", "interface_type_list", 
		"type_parameter_constraints_clauses", "type_parameter_constraints_clause", 
		"type_parameter_constraints", "primary_constraint", "secondary_constraints", 
		"constructor_constraint", "class_body", "class_member_declarations", "class_member_declaration", 
		"all_member_modifiers", "all_member_modifier", "common_member_declaration", 
		"typed_member_declaration", "constant_declarators", "constant_declarator", 
		"variable_declarators", "variable_declarator", "variable_initializer", 
		"return_type", "member_name", "method_body", "formal_parameter_list", 
		"fixed_parameters", "fixed_parameter", "parameter_modifier", "parameter_array", 
		"accessor_declarations", "get_accessor_declaration", "set_accessor_declaration", 
		"accessor_modifier", "accessor_body", "event_accessor_declarations", "add_accessor_declaration", 
		"remove_accessor_declaration", "overloadable_operator", "conversion_operator_declarator", 
		"constructor_initializer", "body", "struct_interfaces", "struct_body", 
		"struct_member_declaration", "array_type", "rank_specifier", "array_initializer", 
		"variant_type_parameter_list", "variant_type_parameter", "variance_annotation", 
		"interface_base", "interface_body", "interface_member_declaration", "interface_accessors", 
		"enum_base", "enum_body", "enum_member_declaration", "global_attribute_section", 
		"global_attribute_target", "attributes", "attribute_section", "attribute_target", 
		"attribute_list", "attribute", "attribute_argument", "pointer_type", "fixed_pointer_declarators", 
		"fixed_pointer_declarator", "fixed_pointer_initializer", "fixed_size_buffer_declarator", 
		"local_variable_initializer_unsafe", "right_arrow", "right_shift", "right_shift_assignment", 
		"literal", "boolean_literal", "string_literal", "interpolated_regular_string", 
		"interpolated_verbatium_string", "interpolated_regular_string_part", "interpolated_verbatium_string_part", 
		"interpolated_string_expression", "keyword", "class_definition", "struct_definition", 
		"interface_definition", "enum_definition", "delegate_definition", "event_declaration", 
		"field_declaration", "property_declaration", "constant_declaration", "indexer_declaration", 
		"destructor_definition", "constructor_declaration", "method_declaration", 
		"method_member_name", "operator_declaration", "arg_declaration", "method_invocation", 
		"object_creation_expression", "identifier"
	};

	private static final String[] _LITERAL_NAMES = {
		null, "'\u00EF\u00BB\u00BF'", null, null, null, null, null, "'#'", "'abstract'", 
		"'add'", "'alias'", "'__arglist'", "'as'", "'ascending'", "'async'", "'await'", 
		"'base'", "'bool'", "'break'", "'by'", "'byte'", "'case'", "'catch'", 
		"'char'", "'checked'", "'class'", "'const'", "'continue'", "'decimal'", 
		"'default'", "'delegate'", "'descending'", "'do'", "'double'", "'dynamic'", 
		"'else'", "'enum'", "'equals'", "'event'", "'explicit'", "'extern'", "'false'", 
		"'finally'", "'fixed'", "'float'", "'for'", "'foreach'", "'from'", "'get'", 
		"'goto'", "'group'", "'if'", "'implicit'", "'in'", "'int'", "'interface'", 
		"'internal'", "'into'", "'is'", "'join'", "'let'", "'lock'", "'long'", 
		"'nameof'", "'namespace'", "'new'", "'null'", "'object'", "'on'", "'operator'", 
		"'orderby'", "'out'", "'override'", "'params'", "'partial'", "'private'", 
		"'protected'", "'public'", "'readonly'", "'ref'", "'remove'", "'return'", 
		"'sbyte'", "'sealed'", "'select'", "'set'", "'short'", "'sizeof'", "'stackalloc'", 
		"'static'", "'string'", "'struct'", "'switch'", "'this'", "'throw'", "'true'", 
		"'try'", "'typeof'", "'uint'", "'ulong'", "'unchecked'", "'unsafe'", "'ushort'", 
		"'using'", "'virtual'", "'void'", "'volatile'", "'when'", "'where'", "'while'", 
		"'yield'", null, null, null, null, null, null, null, null, null, null, 
		"'{'", "'}'", "'['", "']'", "'('", "')'", "'.'", "','", "':'", "';'", 
		"'+'", "'-'", "'*'", "'/'", "'%'", "'&'", "'|'", "'^'", "'!'", "'~'", 
		"'='", "'<'", "'>'", "'?'", "'::'", "'??'", "'++'", "'--'", "'&&'", "'||'", 
		"'->'", "'=='", "'!='", "'<='", "'>='", "'+='", "'-='", "'*='", "'/='", 
		"'%='", "'&='", "'|='", "'^='", "'<<'", "'<<='", "'{{'", null, null, null, 
		null, null, null, null, null, null, null, "'define'", "'undef'", "'elif'", 
		"'endif'", "'line'", null, null, null, null, null, "'hidden'", null, null, 
		null, "'}}'"
	};
	private static final String[] _SYMBOLIC_NAMES = {
		null, "BYTE_ORDER_MARK", "SINGLE_LINE_DOC_COMMENT", "DELIMITED_DOC_COMMENT", 
		"SINGLE_LINE_COMMENT", "DELIMITED_COMMENT", "WHITESPACES", "SHARP", "ABSTRACT", 
		"ADD", "ALIAS", "ARGLIST", "AS", "ASCENDING", "ASYNC", "AWAIT", "BASE", 
		"BOOL", "BREAK", "BY", "BYTE", "CASE", "CATCH", "CHAR", "CHECKED", "CLASS", 
		"CONST", "CONTINUE", "DECIMAL", "DEFAULT", "DELEGATE", "DESCENDING", "DO", 
		"DOUBLE", "DYNAMIC", "ELSE", "ENUM", "EQUALS", "EVENT", "EXPLICIT", "EXTERN", 
		"FALSE", "FINALLY", "FIXED", "FLOAT", "FOR", "FOREACH", "FROM", "GET", 
		"GOTO", "GROUP", "IF", "IMPLICIT", "IN", "INT", "INTERFACE", "INTERNAL", 
		"INTO", "IS", "JOIN", "LET", "LOCK", "LONG", "NAMEOF", "NAMESPACE", "NEW", 
		"NULL", "OBJECT", "ON", "OPERATOR", "ORDERBY", "OUT", "OVERRIDE", "PARAMS", 
		"PARTIAL", "PRIVATE", "PROTECTED", "PUBLIC", "READONLY", "REF", "REMOVE", 
		"RETURN", "SBYTE", "SEALED", "SELECT", "SET", "SHORT", "SIZEOF", "STACKALLOC", 
		"STATIC", "STRING", "STRUCT", "SWITCH", "THIS", "THROW", "TRUE", "TRY", 
		"TYPEOF", "UINT", "ULONG", "UNCHECKED", "UNSAFE", "USHORT", "USING", "VIRTUAL", 
		"VOID", "VOLATILE", "WHEN", "WHERE", "WHILE", "YIELD", "IDENTIFIER", "LITERAL_ACCESS", 
		"INTEGER_LITERAL", "HEX_INTEGER_LITERAL", "REAL_LITERAL", "CHARACTER_LITERAL", 
		"REGULAR_STRING", "VERBATIUM_STRING", "INTERPOLATED_REGULAR_STRING_START", 
		"INTERPOLATED_VERBATIUM_STRING_START", "OPEN_BRACE", "CLOSE_BRACE", "OPEN_BRACKET", 
		"CLOSE_BRACKET", "OPEN_PARENS", "CLOSE_PARENS", "DOT", "COMMA", "COLON", 
		"SEMICOLON", "PLUS", "MINUS", "STAR", "DIV", "PERCENT", "AMP", "BITWISE_OR", 
		"CARET", "BANG", "TILDE", "ASSIGNMENT", "LT", "GT", "INTERR", "DOUBLE_COLON", 
		"OP_COALESCING", "OP_INC", "OP_DEC", "OP_AND", "OP_OR", "OP_PTR", "OP_EQ", 
		"OP_NE", "OP_LE", "OP_GE", "OP_ADD_ASSIGNMENT", "OP_SUB_ASSIGNMENT", "OP_MULT_ASSIGNMENT", 
		"OP_DIV_ASSIGNMENT", "OP_MOD_ASSIGNMENT", "OP_AND_ASSIGNMENT", "OP_OR_ASSIGNMENT", 
		"OP_XOR_ASSIGNMENT", "OP_LEFT_SHIFT", "OP_LEFT_SHIFT_ASSIGNMENT", "DOUBLE_CURLY_INSIDE", 
		"OPEN_BRACE_INSIDE", "REGULAR_CHAR_INSIDE", "VERBATIUM_DOUBLE_QUOTE_INSIDE", 
		"DOUBLE_QUOTE_INSIDE", "REGULAR_STRING_INSIDE", "VERBATIUM_INSIDE_STRING", 
		"CLOSE_BRACE_INSIDE", "FORMAT_STRING", "DIRECTIVE_WHITESPACES", "DIGITS", 
		"DEFINE", "UNDEF", "ELIF", "ENDIF", "LINE", "ERROR", "WARNING", "REGION", 
		"ENDREGION", "PRAGMA", "DIRECTIVE_HIDDEN", "CONDITIONAL_SYMBOL", "DIRECTIVE_NEW_LINE", 
		"TEXT", "DOUBLE_CURLY_CLOSE_INSIDE"
	};
	public static final Vocabulary VOCABULARY = new VocabularyImpl(_LITERAL_NAMES, _SYMBOLIC_NAMES);

	/**
	 * @deprecated Use {@link #VOCABULARY} instead.
	 */
	@Deprecated
	public static final String[] tokenNames;
	static {
		tokenNames = new String[_SYMBOLIC_NAMES.length];
		for (int i = 0; i < tokenNames.length; i++) {
			tokenNames[i] = VOCABULARY.getLiteralName(i);
			if (tokenNames[i] == null) {
				tokenNames[i] = VOCABULARY.getSymbolicName(i);
			}

			if (tokenNames[i] == null) {
				tokenNames[i] = "<INVALID>";
			}
		}
	}

	@Override
	@Deprecated
	public String[] getTokenNames() {
		return tokenNames;
	}

	@Override

	public Vocabulary getVocabulary() {
		return VOCABULARY;
	}

	@Override
	public String getGrammarFileName() { return "CSharpParser.g4"; }

	@Override
	public String[] getRuleNames() { return ruleNames; }

	@Override
	public String getSerializedATN() { return _serializedATN; }

	@Override
	public ATN getATN() { return _ATN; }

	public CSharpParser(TokenStream input) {
		super(input);
		_interp = new ParserATNSimulator(this,_ATN,_decisionToDFA,_sharedContextCache);
	}
	public static class Compilation_unitContext extends ParserRuleContext {
		public TerminalNode EOF() { return getToken(CSharpParser.EOF, 0); }
		public TerminalNode BYTE_ORDER_MARK() { return getToken(CSharpParser.BYTE_ORDER_MARK, 0); }
		public Extern_alias_directivesContext extern_alias_directives() {
			return getRuleContext(Extern_alias_directivesContext.class,0);
		}
		public Using_directivesContext using_directives() {
			return getRuleContext(Using_directivesContext.class,0);
		}
		public List<Global_attribute_sectionContext> global_attribute_section() {
			return getRuleContexts(Global_attribute_sectionContext.class);
		}
		public Global_attribute_sectionContext global_attribute_section(int i) {
			return getRuleContext(Global_attribute_sectionContext.class,i);
		}
		public Namespace_member_declarationsContext namespace_member_declarations() {
			return getRuleContext(Namespace_member_declarationsContext.class,0);
		}
		public Compilation_unitContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_compilation_unit; }
	}

	public final Compilation_unitContext compilation_unit() throws RecognitionException {
		Compilation_unitContext _localctx = new Compilation_unitContext(_ctx, getState());
		enterRule(_localctx, 0, RULE_compilation_unit);
		int _la;
		try {
			int _alt;
			enterOuterAlt(_localctx, 1);
			{
			setState(401);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==BYTE_ORDER_MARK) {
				{
				setState(400);
				match(BYTE_ORDER_MARK);
				}
			}

			setState(404);
			_errHandler.sync(this);
			switch ( getInterpreter().adaptivePredict(_input,1,_ctx) ) {
			case 1:
				{
				setState(403);
				extern_alias_directives();
				}
				break;
			}
			setState(407);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==USING) {
				{
				setState(406);
				using_directives();
				}
			}

			setState(412);
			_errHandler.sync(this);
			_alt = getInterpreter().adaptivePredict(_input,3,_ctx);
			while ( _alt!=2 && _alt!=org.antlr.v4.runtime.atn.ATN.INVALID_ALT_NUMBER ) {
				if ( _alt==1 ) {
					{
					{
					setState(409);
					global_attribute_section();
					}
					} 
				}
				setState(414);
				_errHandler.sync(this);
				_alt = getInterpreter().adaptivePredict(_input,3,_ctx);
			}
			setState(416);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if ((((_la) & ~0x3f) == 0 && ((1L << _la) & ((1L << ABSTRACT) | (1L << ASYNC) | (1L << CLASS) | (1L << DELEGATE) | (1L << ENUM) | (1L << EXTERN) | (1L << INTERFACE) | (1L << INTERNAL))) != 0) || ((((_la - 64)) & ~0x3f) == 0 && ((1L << (_la - 64)) & ((1L << (NAMESPACE - 64)) | (1L << (NEW - 64)) | (1L << (OVERRIDE - 64)) | (1L << (PARTIAL - 64)) | (1L << (PRIVATE - 64)) | (1L << (PROTECTED - 64)) | (1L << (PUBLIC - 64)) | (1L << (READONLY - 64)) | (1L << (SEALED - 64)) | (1L << (STATIC - 64)) | (1L << (STRUCT - 64)) | (1L << (UNSAFE - 64)) | (1L << (VIRTUAL - 64)) | (1L << (VOLATILE - 64)) | (1L << (OPEN_BRACKET - 64)))) != 0)) {
				{
				setState(415);
				namespace_member_declarations();
				}
			}

			setState(418);
			match(EOF);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Namespace_or_type_nameContext extends ParserRuleContext {
		public List<IdentifierContext> identifier() {
			return getRuleContexts(IdentifierContext.class);
		}
		public IdentifierContext identifier(int i) {
			return getRuleContext(IdentifierContext.class,i);
		}
		public Qualified_alias_memberContext qualified_alias_member() {
			return getRuleContext(Qualified_alias_memberContext.class,0);
		}
		public List<Type_argument_listContext> type_argument_list() {
			return getRuleContexts(Type_argument_listContext.class);
		}
		public Type_argument_listContext type_argument_list(int i) {
			return getRuleContext(Type_argument_listContext.class,i);
		}
		public Namespace_or_type_nameContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_namespace_or_type_name; }
	}

	public final Namespace_or_type_nameContext namespace_or_type_name() throws RecognitionException {
		Namespace_or_type_nameContext _localctx = new Namespace_or_type_nameContext(_ctx, getState());
		enterRule(_localctx, 2, RULE_namespace_or_type_name);
		try {
			int _alt;
			enterOuterAlt(_localctx, 1);
			{
			setState(425);
			_errHandler.sync(this);
			switch ( getInterpreter().adaptivePredict(_input,6,_ctx) ) {
			case 1:
				{
				setState(420);
				identifier();
				setState(422);
				_errHandler.sync(this);
				switch ( getInterpreter().adaptivePredict(_input,5,_ctx) ) {
				case 1:
					{
					setState(421);
					type_argument_list();
					}
					break;
				}
				}
				break;
			case 2:
				{
				setState(424);
				qualified_alias_member();
				}
				break;
			}
			setState(434);
			_errHandler.sync(this);
			_alt = getInterpreter().adaptivePredict(_input,8,_ctx);
			while ( _alt!=2 && _alt!=org.antlr.v4.runtime.atn.ATN.INVALID_ALT_NUMBER ) {
				if ( _alt==1 ) {
					{
					{
					setState(427);
					match(DOT);
					setState(428);
					identifier();
					setState(430);
					_errHandler.sync(this);
					switch ( getInterpreter().adaptivePredict(_input,7,_ctx) ) {
					case 1:
						{
						setState(429);
						type_argument_list();
						}
						break;
					}
					}
					} 
				}
				setState(436);
				_errHandler.sync(this);
				_alt = getInterpreter().adaptivePredict(_input,8,_ctx);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class TypeContext extends ParserRuleContext {
		public Base_typeContext base_type() {
			return getRuleContext(Base_typeContext.class,0);
		}
		public List<Rank_specifierContext> rank_specifier() {
			return getRuleContexts(Rank_specifierContext.class);
		}
		public Rank_specifierContext rank_specifier(int i) {
			return getRuleContext(Rank_specifierContext.class,i);
		}
		public TypeContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_type; }
	}

	public final TypeContext type() throws RecognitionException {
		TypeContext _localctx = new TypeContext(_ctx, getState());
		enterRule(_localctx, 4, RULE_type);
		try {
			int _alt;
			enterOuterAlt(_localctx, 1);
			{
			setState(437);
			base_type();
			setState(443);
			_errHandler.sync(this);
			_alt = getInterpreter().adaptivePredict(_input,10,_ctx);
			while ( _alt!=2 && _alt!=org.antlr.v4.runtime.atn.ATN.INVALID_ALT_NUMBER ) {
				if ( _alt==1 ) {
					{
					setState(441);
					_errHandler.sync(this);
					switch (_input.LA(1)) {
					case INTERR:
						{
						setState(438);
						match(INTERR);
						}
						break;
					case OPEN_BRACKET:
						{
						setState(439);
						rank_specifier();
						}
						break;
					case STAR:
						{
						setState(440);
						match(STAR);
						}
						break;
					default:
						throw new NoViableAltException(this);
					}
					} 
				}
				setState(445);
				_errHandler.sync(this);
				_alt = getInterpreter().adaptivePredict(_input,10,_ctx);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Base_typeContext extends ParserRuleContext {
		public Simple_typeContext simple_type() {
			return getRuleContext(Simple_typeContext.class,0);
		}
		public Class_typeContext class_type() {
			return getRuleContext(Class_typeContext.class,0);
		}
		public TerminalNode VOID() { return getToken(CSharpParser.VOID, 0); }
		public Base_typeContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_base_type; }
	}

	public final Base_typeContext base_type() throws RecognitionException {
		Base_typeContext _localctx = new Base_typeContext(_ctx, getState());
		enterRule(_localctx, 6, RULE_base_type);
		try {
			setState(450);
			_errHandler.sync(this);
			switch (_input.LA(1)) {
			case BOOL:
			case BYTE:
			case CHAR:
			case DECIMAL:
			case DOUBLE:
			case FLOAT:
			case INT:
			case LONG:
			case SBYTE:
			case SHORT:
			case UINT:
			case ULONG:
			case USHORT:
				enterOuterAlt(_localctx, 1);
				{
				setState(446);
				simple_type();
				}
				break;
			case ADD:
			case ALIAS:
			case ARGLIST:
			case ASCENDING:
			case ASYNC:
			case AWAIT:
			case BY:
			case DESCENDING:
			case DYNAMIC:
			case EQUALS:
			case FROM:
			case GET:
			case GROUP:
			case INTO:
			case JOIN:
			case LET:
			case NAMEOF:
			case OBJECT:
			case ON:
			case ORDERBY:
			case PARTIAL:
			case REMOVE:
			case SELECT:
			case SET:
			case STRING:
			case WHEN:
			case WHERE:
			case YIELD:
			case IDENTIFIER:
				enterOuterAlt(_localctx, 2);
				{
				setState(447);
				class_type();
				}
				break;
			case VOID:
				enterOuterAlt(_localctx, 3);
				{
				setState(448);
				match(VOID);
				setState(449);
				match(STAR);
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Simple_typeContext extends ParserRuleContext {
		public Numeric_typeContext numeric_type() {
			return getRuleContext(Numeric_typeContext.class,0);
		}
		public TerminalNode BOOL() { return getToken(CSharpParser.BOOL, 0); }
		public Simple_typeContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_simple_type; }
	}

	public final Simple_typeContext simple_type() throws RecognitionException {
		Simple_typeContext _localctx = new Simple_typeContext(_ctx, getState());
		enterRule(_localctx, 8, RULE_simple_type);
		try {
			setState(454);
			_errHandler.sync(this);
			switch (_input.LA(1)) {
			case BYTE:
			case CHAR:
			case DECIMAL:
			case DOUBLE:
			case FLOAT:
			case INT:
			case LONG:
			case SBYTE:
			case SHORT:
			case UINT:
			case ULONG:
			case USHORT:
				enterOuterAlt(_localctx, 1);
				{
				setState(452);
				numeric_type();
				}
				break;
			case BOOL:
				enterOuterAlt(_localctx, 2);
				{
				setState(453);
				match(BOOL);
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Numeric_typeContext extends ParserRuleContext {
		public Integral_typeContext integral_type() {
			return getRuleContext(Integral_typeContext.class,0);
		}
		public Floating_point_typeContext floating_point_type() {
			return getRuleContext(Floating_point_typeContext.class,0);
		}
		public TerminalNode DECIMAL() { return getToken(CSharpParser.DECIMAL, 0); }
		public Numeric_typeContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_numeric_type; }
	}

	public final Numeric_typeContext numeric_type() throws RecognitionException {
		Numeric_typeContext _localctx = new Numeric_typeContext(_ctx, getState());
		enterRule(_localctx, 10, RULE_numeric_type);
		try {
			setState(459);
			_errHandler.sync(this);
			switch (_input.LA(1)) {
			case BYTE:
			case CHAR:
			case INT:
			case LONG:
			case SBYTE:
			case SHORT:
			case UINT:
			case ULONG:
			case USHORT:
				enterOuterAlt(_localctx, 1);
				{
				setState(456);
				integral_type();
				}
				break;
			case DOUBLE:
			case FLOAT:
				enterOuterAlt(_localctx, 2);
				{
				setState(457);
				floating_point_type();
				}
				break;
			case DECIMAL:
				enterOuterAlt(_localctx, 3);
				{
				setState(458);
				match(DECIMAL);
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Integral_typeContext extends ParserRuleContext {
		public TerminalNode SBYTE() { return getToken(CSharpParser.SBYTE, 0); }
		public TerminalNode BYTE() { return getToken(CSharpParser.BYTE, 0); }
		public TerminalNode SHORT() { return getToken(CSharpParser.SHORT, 0); }
		public TerminalNode USHORT() { return getToken(CSharpParser.USHORT, 0); }
		public TerminalNode INT() { return getToken(CSharpParser.INT, 0); }
		public TerminalNode UINT() { return getToken(CSharpParser.UINT, 0); }
		public TerminalNode LONG() { return getToken(CSharpParser.LONG, 0); }
		public TerminalNode ULONG() { return getToken(CSharpParser.ULONG, 0); }
		public TerminalNode CHAR() { return getToken(CSharpParser.CHAR, 0); }
		public Integral_typeContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_integral_type; }
	}

	public final Integral_typeContext integral_type() throws RecognitionException {
		Integral_typeContext _localctx = new Integral_typeContext(_ctx, getState());
		enterRule(_localctx, 12, RULE_integral_type);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(461);
			_la = _input.LA(1);
			if ( !((((_la) & ~0x3f) == 0 && ((1L << _la) & ((1L << BYTE) | (1L << CHAR) | (1L << INT) | (1L << LONG))) != 0) || ((((_la - 82)) & ~0x3f) == 0 && ((1L << (_la - 82)) & ((1L << (SBYTE - 82)) | (1L << (SHORT - 82)) | (1L << (UINT - 82)) | (1L << (ULONG - 82)) | (1L << (USHORT - 82)))) != 0)) ) {
			_errHandler.recoverInline(this);
			}
			else {
				if ( _input.LA(1)==Token.EOF ) matchedEOF = true;
				_errHandler.reportMatch(this);
				consume();
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Floating_point_typeContext extends ParserRuleContext {
		public TerminalNode FLOAT() { return getToken(CSharpParser.FLOAT, 0); }
		public TerminalNode DOUBLE() { return getToken(CSharpParser.DOUBLE, 0); }
		public Floating_point_typeContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_floating_point_type; }
	}

	public final Floating_point_typeContext floating_point_type() throws RecognitionException {
		Floating_point_typeContext _localctx = new Floating_point_typeContext(_ctx, getState());
		enterRule(_localctx, 14, RULE_floating_point_type);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(463);
			_la = _input.LA(1);
			if ( !(_la==DOUBLE || _la==FLOAT) ) {
			_errHandler.recoverInline(this);
			}
			else {
				if ( _input.LA(1)==Token.EOF ) matchedEOF = true;
				_errHandler.reportMatch(this);
				consume();
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Class_typeContext extends ParserRuleContext {
		public Namespace_or_type_nameContext namespace_or_type_name() {
			return getRuleContext(Namespace_or_type_nameContext.class,0);
		}
		public TerminalNode OBJECT() { return getToken(CSharpParser.OBJECT, 0); }
		public TerminalNode DYNAMIC() { return getToken(CSharpParser.DYNAMIC, 0); }
		public TerminalNode STRING() { return getToken(CSharpParser.STRING, 0); }
		public Class_typeContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_class_type; }
	}

	public final Class_typeContext class_type() throws RecognitionException {
		Class_typeContext _localctx = new Class_typeContext(_ctx, getState());
		enterRule(_localctx, 16, RULE_class_type);
		try {
			setState(469);
			_errHandler.sync(this);
			switch ( getInterpreter().adaptivePredict(_input,14,_ctx) ) {
			case 1:
				enterOuterAlt(_localctx, 1);
				{
				setState(465);
				namespace_or_type_name();
				}
				break;
			case 2:
				enterOuterAlt(_localctx, 2);
				{
				setState(466);
				match(OBJECT);
				}
				break;
			case 3:
				enterOuterAlt(_localctx, 3);
				{
				setState(467);
				match(DYNAMIC);
				}
				break;
			case 4:
				enterOuterAlt(_localctx, 4);
				{
				setState(468);
				match(STRING);
				}
				break;
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Type_argument_listContext extends ParserRuleContext {
		public List<TypeContext> type() {
			return getRuleContexts(TypeContext.class);
		}
		public TypeContext type(int i) {
			return getRuleContext(TypeContext.class,i);
		}
		public Type_argument_listContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_type_argument_list; }
	}

	public final Type_argument_listContext type_argument_list() throws RecognitionException {
		Type_argument_listContext _localctx = new Type_argument_listContext(_ctx, getState());
		enterRule(_localctx, 18, RULE_type_argument_list);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(471);
			match(LT);
			setState(472);
			type();
			setState(477);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while (_la==COMMA) {
				{
				{
				setState(473);
				match(COMMA);
				setState(474);
				type();
				}
				}
				setState(479);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			setState(480);
			match(GT);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Argument_listContext extends ParserRuleContext {
		public List<ArgumentContext> argument() {
			return getRuleContexts(ArgumentContext.class);
		}
		public ArgumentContext argument(int i) {
			return getRuleContext(ArgumentContext.class,i);
		}
		public Argument_listContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_argument_list; }
	}

	public final Argument_listContext argument_list() throws RecognitionException {
		Argument_listContext _localctx = new Argument_listContext(_ctx, getState());
		enterRule(_localctx, 20, RULE_argument_list);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(482);
			argument();
			setState(487);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while (_la==COMMA) {
				{
				{
				setState(483);
				match(COMMA);
				setState(484);
				argument();
				}
				}
				setState(489);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class ArgumentContext extends ParserRuleContext {
		public Token refout;
		public ExpressionContext expression() {
			return getRuleContext(ExpressionContext.class,0);
		}
		public IdentifierContext identifier() {
			return getRuleContext(IdentifierContext.class,0);
		}
		public TerminalNode REF() { return getToken(CSharpParser.REF, 0); }
		public TerminalNode OUT() { return getToken(CSharpParser.OUT, 0); }
		public ArgumentContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_argument; }
	}

	public final ArgumentContext argument() throws RecognitionException {
		ArgumentContext _localctx = new ArgumentContext(_ctx, getState());
		enterRule(_localctx, 22, RULE_argument);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(493);
			_errHandler.sync(this);
			switch ( getInterpreter().adaptivePredict(_input,17,_ctx) ) {
			case 1:
				{
				setState(490);
				identifier();
				setState(491);
				match(COLON);
				}
				break;
			}
			setState(496);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==OUT || _la==REF) {
				{
				setState(495);
				((ArgumentContext)_localctx).refout = _input.LT(1);
				_la = _input.LA(1);
				if ( !(_la==OUT || _la==REF) ) {
					((ArgumentContext)_localctx).refout = (Token)_errHandler.recoverInline(this);
				}
				else {
					if ( _input.LA(1)==Token.EOF ) matchedEOF = true;
					_errHandler.reportMatch(this);
					consume();
				}
				}
			}

			setState(498);
			expression();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class ExpressionContext extends ParserRuleContext {
		public AssignmentContext assignment() {
			return getRuleContext(AssignmentContext.class,0);
		}
		public Non_assignment_expressionContext non_assignment_expression() {
			return getRuleContext(Non_assignment_expressionContext.class,0);
		}
		public ExpressionContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_expression; }
	}

	public final ExpressionContext expression() throws RecognitionException {
		ExpressionContext _localctx = new ExpressionContext(_ctx, getState());
		enterRule(_localctx, 24, RULE_expression);
		try {
			setState(502);
			_errHandler.sync(this);
			switch ( getInterpreter().adaptivePredict(_input,19,_ctx) ) {
			case 1:
				enterOuterAlt(_localctx, 1);
				{
				setState(500);
				assignment();
				}
				break;
			case 2:
				enterOuterAlt(_localctx, 2);
				{
				setState(501);
				non_assignment_expression();
				}
				break;
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Non_assignment_expressionContext extends ParserRuleContext {
		public Lambda_expressionContext lambda_expression() {
			return getRuleContext(Lambda_expressionContext.class,0);
		}
		public Query_expressionContext query_expression() {
			return getRuleContext(Query_expressionContext.class,0);
		}
		public Conditional_expressionContext conditional_expression() {
			return getRuleContext(Conditional_expressionContext.class,0);
		}
		public Non_assignment_expressionContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_non_assignment_expression; }
	}

	public final Non_assignment_expressionContext non_assignment_expression() throws RecognitionException {
		Non_assignment_expressionContext _localctx = new Non_assignment_expressionContext(_ctx, getState());
		enterRule(_localctx, 26, RULE_non_assignment_expression);
		try {
			setState(507);
			_errHandler.sync(this);
			switch ( getInterpreter().adaptivePredict(_input,20,_ctx) ) {
			case 1:
				enterOuterAlt(_localctx, 1);
				{
				setState(504);
				lambda_expression();
				}
				break;
			case 2:
				enterOuterAlt(_localctx, 2);
				{
				setState(505);
				query_expression();
				}
				break;
			case 3:
				enterOuterAlt(_localctx, 3);
				{
				setState(506);
				conditional_expression();
				}
				break;
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class AssignmentContext extends ParserRuleContext {
		public Unary_expressionContext unary_expression() {
			return getRuleContext(Unary_expressionContext.class,0);
		}
		public Assignment_operatorContext assignment_operator() {
			return getRuleContext(Assignment_operatorContext.class,0);
		}
		public ExpressionContext expression() {
			return getRuleContext(ExpressionContext.class,0);
		}
		public AssignmentContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_assignment; }
	}

	public final AssignmentContext assignment() throws RecognitionException {
		AssignmentContext _localctx = new AssignmentContext(_ctx, getState());
		enterRule(_localctx, 28, RULE_assignment);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(509);
			unary_expression();
			setState(510);
			assignment_operator();
			setState(511);
			expression();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Assignment_operatorContext extends ParserRuleContext {
		public Right_shift_assignmentContext right_shift_assignment() {
			return getRuleContext(Right_shift_assignmentContext.class,0);
		}
		public Assignment_operatorContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_assignment_operator; }
	}

	public final Assignment_operatorContext assignment_operator() throws RecognitionException {
		Assignment_operatorContext _localctx = new Assignment_operatorContext(_ctx, getState());
		enterRule(_localctx, 30, RULE_assignment_operator);
		try {
			setState(524);
			_errHandler.sync(this);
			switch (_input.LA(1)) {
			case ASSIGNMENT:
				enterOuterAlt(_localctx, 1);
				{
				setState(513);
				match(ASSIGNMENT);
				}
				break;
			case OP_ADD_ASSIGNMENT:
				enterOuterAlt(_localctx, 2);
				{
				setState(514);
				match(OP_ADD_ASSIGNMENT);
				}
				break;
			case OP_SUB_ASSIGNMENT:
				enterOuterAlt(_localctx, 3);
				{
				setState(515);
				match(OP_SUB_ASSIGNMENT);
				}
				break;
			case OP_MULT_ASSIGNMENT:
				enterOuterAlt(_localctx, 4);
				{
				setState(516);
				match(OP_MULT_ASSIGNMENT);
				}
				break;
			case OP_DIV_ASSIGNMENT:
				enterOuterAlt(_localctx, 5);
				{
				setState(517);
				match(OP_DIV_ASSIGNMENT);
				}
				break;
			case OP_MOD_ASSIGNMENT:
				enterOuterAlt(_localctx, 6);
				{
				setState(518);
				match(OP_MOD_ASSIGNMENT);
				}
				break;
			case OP_AND_ASSIGNMENT:
				enterOuterAlt(_localctx, 7);
				{
				setState(519);
				match(OP_AND_ASSIGNMENT);
				}
				break;
			case OP_OR_ASSIGNMENT:
				enterOuterAlt(_localctx, 8);
				{
				setState(520);
				match(OP_OR_ASSIGNMENT);
				}
				break;
			case OP_XOR_ASSIGNMENT:
				enterOuterAlt(_localctx, 9);
				{
				setState(521);
				match(OP_XOR_ASSIGNMENT);
				}
				break;
			case OP_LEFT_SHIFT_ASSIGNMENT:
				enterOuterAlt(_localctx, 10);
				{
				setState(522);
				match(OP_LEFT_SHIFT_ASSIGNMENT);
				}
				break;
			case GT:
				enterOuterAlt(_localctx, 11);
				{
				setState(523);
				right_shift_assignment();
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Conditional_expressionContext extends ParserRuleContext {
		public Null_coalescing_expressionContext null_coalescing_expression() {
			return getRuleContext(Null_coalescing_expressionContext.class,0);
		}
		public List<ExpressionContext> expression() {
			return getRuleContexts(ExpressionContext.class);
		}
		public ExpressionContext expression(int i) {
			return getRuleContext(ExpressionContext.class,i);
		}
		public Conditional_expressionContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_conditional_expression; }
	}

	public final Conditional_expressionContext conditional_expression() throws RecognitionException {
		Conditional_expressionContext _localctx = new Conditional_expressionContext(_ctx, getState());
		enterRule(_localctx, 32, RULE_conditional_expression);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(526);
			null_coalescing_expression();
			setState(532);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==INTERR) {
				{
				setState(527);
				match(INTERR);
				setState(528);
				expression();
				setState(529);
				match(COLON);
				setState(530);
				expression();
				}
			}

			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Null_coalescing_expressionContext extends ParserRuleContext {
		public Conditional_or_expressionContext conditional_or_expression() {
			return getRuleContext(Conditional_or_expressionContext.class,0);
		}
		public Null_coalescing_expressionContext null_coalescing_expression() {
			return getRuleContext(Null_coalescing_expressionContext.class,0);
		}
		public Null_coalescing_expressionContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_null_coalescing_expression; }
	}

	public final Null_coalescing_expressionContext null_coalescing_expression() throws RecognitionException {
		Null_coalescing_expressionContext _localctx = new Null_coalescing_expressionContext(_ctx, getState());
		enterRule(_localctx, 34, RULE_null_coalescing_expression);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(534);
			conditional_or_expression();
			setState(537);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==OP_COALESCING) {
				{
				setState(535);
				match(OP_COALESCING);
				setState(536);
				null_coalescing_expression();
				}
			}

			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Conditional_or_expressionContext extends ParserRuleContext {
		public List<Conditional_and_expressionContext> conditional_and_expression() {
			return getRuleContexts(Conditional_and_expressionContext.class);
		}
		public Conditional_and_expressionContext conditional_and_expression(int i) {
			return getRuleContext(Conditional_and_expressionContext.class,i);
		}
		public List<TerminalNode> OP_OR() { return getTokens(CSharpParser.OP_OR); }
		public TerminalNode OP_OR(int i) {
			return getToken(CSharpParser.OP_OR, i);
		}
		public Conditional_or_expressionContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_conditional_or_expression; }
	}

	public final Conditional_or_expressionContext conditional_or_expression() throws RecognitionException {
		Conditional_or_expressionContext _localctx = new Conditional_or_expressionContext(_ctx, getState());
		enterRule(_localctx, 36, RULE_conditional_or_expression);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(539);
			conditional_and_expression();
			setState(544);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while (_la==OP_OR) {
				{
				{
				setState(540);
				match(OP_OR);
				setState(541);
				conditional_and_expression();
				}
				}
				setState(546);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Conditional_and_expressionContext extends ParserRuleContext {
		public List<Inclusive_or_expressionContext> inclusive_or_expression() {
			return getRuleContexts(Inclusive_or_expressionContext.class);
		}
		public Inclusive_or_expressionContext inclusive_or_expression(int i) {
			return getRuleContext(Inclusive_or_expressionContext.class,i);
		}
		public List<TerminalNode> OP_AND() { return getTokens(CSharpParser.OP_AND); }
		public TerminalNode OP_AND(int i) {
			return getToken(CSharpParser.OP_AND, i);
		}
		public Conditional_and_expressionContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_conditional_and_expression; }
	}

	public final Conditional_and_expressionContext conditional_and_expression() throws RecognitionException {
		Conditional_and_expressionContext _localctx = new Conditional_and_expressionContext(_ctx, getState());
		enterRule(_localctx, 38, RULE_conditional_and_expression);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(547);
			inclusive_or_expression();
			setState(552);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while (_la==OP_AND) {
				{
				{
				setState(548);
				match(OP_AND);
				setState(549);
				inclusive_or_expression();
				}
				}
				setState(554);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Inclusive_or_expressionContext extends ParserRuleContext {
		public List<Exclusive_or_expressionContext> exclusive_or_expression() {
			return getRuleContexts(Exclusive_or_expressionContext.class);
		}
		public Exclusive_or_expressionContext exclusive_or_expression(int i) {
			return getRuleContext(Exclusive_or_expressionContext.class,i);
		}
		public Inclusive_or_expressionContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_inclusive_or_expression; }
	}

	public final Inclusive_or_expressionContext inclusive_or_expression() throws RecognitionException {
		Inclusive_or_expressionContext _localctx = new Inclusive_or_expressionContext(_ctx, getState());
		enterRule(_localctx, 40, RULE_inclusive_or_expression);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(555);
			exclusive_or_expression();
			setState(560);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while (_la==BITWISE_OR) {
				{
				{
				setState(556);
				match(BITWISE_OR);
				setState(557);
				exclusive_or_expression();
				}
				}
				setState(562);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Exclusive_or_expressionContext extends ParserRuleContext {
		public List<And_expressionContext> and_expression() {
			return getRuleContexts(And_expressionContext.class);
		}
		public And_expressionContext and_expression(int i) {
			return getRuleContext(And_expressionContext.class,i);
		}
		public Exclusive_or_expressionContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_exclusive_or_expression; }
	}

	public final Exclusive_or_expressionContext exclusive_or_expression() throws RecognitionException {
		Exclusive_or_expressionContext _localctx = new Exclusive_or_expressionContext(_ctx, getState());
		enterRule(_localctx, 42, RULE_exclusive_or_expression);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(563);
			and_expression();
			setState(568);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while (_la==CARET) {
				{
				{
				setState(564);
				match(CARET);
				setState(565);
				and_expression();
				}
				}
				setState(570);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class And_expressionContext extends ParserRuleContext {
		public List<Equality_expressionContext> equality_expression() {
			return getRuleContexts(Equality_expressionContext.class);
		}
		public Equality_expressionContext equality_expression(int i) {
			return getRuleContext(Equality_expressionContext.class,i);
		}
		public And_expressionContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_and_expression; }
	}

	public final And_expressionContext and_expression() throws RecognitionException {
		And_expressionContext _localctx = new And_expressionContext(_ctx, getState());
		enterRule(_localctx, 44, RULE_and_expression);
		try {
			int _alt;
			enterOuterAlt(_localctx, 1);
			{
			setState(571);
			equality_expression();
			setState(576);
			_errHandler.sync(this);
			_alt = getInterpreter().adaptivePredict(_input,28,_ctx);
			while ( _alt!=2 && _alt!=org.antlr.v4.runtime.atn.ATN.INVALID_ALT_NUMBER ) {
				if ( _alt==1 ) {
					{
					{
					setState(572);
					match(AMP);
					setState(573);
					equality_expression();
					}
					} 
				}
				setState(578);
				_errHandler.sync(this);
				_alt = getInterpreter().adaptivePredict(_input,28,_ctx);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Equality_expressionContext extends ParserRuleContext {
		public List<Relational_expressionContext> relational_expression() {
			return getRuleContexts(Relational_expressionContext.class);
		}
		public Relational_expressionContext relational_expression(int i) {
			return getRuleContext(Relational_expressionContext.class,i);
		}
		public List<TerminalNode> OP_EQ() { return getTokens(CSharpParser.OP_EQ); }
		public TerminalNode OP_EQ(int i) {
			return getToken(CSharpParser.OP_EQ, i);
		}
		public List<TerminalNode> OP_NE() { return getTokens(CSharpParser.OP_NE); }
		public TerminalNode OP_NE(int i) {
			return getToken(CSharpParser.OP_NE, i);
		}
		public Equality_expressionContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_equality_expression; }
	}

	public final Equality_expressionContext equality_expression() throws RecognitionException {
		Equality_expressionContext _localctx = new Equality_expressionContext(_ctx, getState());
		enterRule(_localctx, 46, RULE_equality_expression);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(579);
			relational_expression();
			setState(584);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while (_la==OP_EQ || _la==OP_NE) {
				{
				{
				setState(580);
				_la = _input.LA(1);
				if ( !(_la==OP_EQ || _la==OP_NE) ) {
				_errHandler.recoverInline(this);
				}
				else {
					if ( _input.LA(1)==Token.EOF ) matchedEOF = true;
					_errHandler.reportMatch(this);
					consume();
				}
				setState(581);
				relational_expression();
				}
				}
				setState(586);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Relational_expressionContext extends ParserRuleContext {
		public List<Shift_expressionContext> shift_expression() {
			return getRuleContexts(Shift_expressionContext.class);
		}
		public Shift_expressionContext shift_expression(int i) {
			return getRuleContext(Shift_expressionContext.class,i);
		}
		public List<TerminalNode> IS() { return getTokens(CSharpParser.IS); }
		public TerminalNode IS(int i) {
			return getToken(CSharpParser.IS, i);
		}
		public List<IsTypeContext> isType() {
			return getRuleContexts(IsTypeContext.class);
		}
		public IsTypeContext isType(int i) {
			return getRuleContext(IsTypeContext.class,i);
		}
		public List<TerminalNode> AS() { return getTokens(CSharpParser.AS); }
		public TerminalNode AS(int i) {
			return getToken(CSharpParser.AS, i);
		}
		public List<TypeContext> type() {
			return getRuleContexts(TypeContext.class);
		}
		public TypeContext type(int i) {
			return getRuleContext(TypeContext.class,i);
		}
		public Relational_expressionContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_relational_expression; }
	}

	public final Relational_expressionContext relational_expression() throws RecognitionException {
		Relational_expressionContext _localctx = new Relational_expressionContext(_ctx, getState());
		enterRule(_localctx, 48, RULE_relational_expression);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(587);
			shift_expression();
			setState(596);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while (_la==AS || _la==IS || ((((_la - 142)) & ~0x3f) == 0 && ((1L << (_la - 142)) & ((1L << (LT - 142)) | (1L << (GT - 142)) | (1L << (OP_LE - 142)) | (1L << (OP_GE - 142)))) != 0)) {
				{
				setState(594);
				_errHandler.sync(this);
				switch (_input.LA(1)) {
				case LT:
				case GT:
				case OP_LE:
				case OP_GE:
					{
					setState(588);
					_la = _input.LA(1);
					if ( !(((((_la - 142)) & ~0x3f) == 0 && ((1L << (_la - 142)) & ((1L << (LT - 142)) | (1L << (GT - 142)) | (1L << (OP_LE - 142)) | (1L << (OP_GE - 142)))) != 0)) ) {
					_errHandler.recoverInline(this);
					}
					else {
						if ( _input.LA(1)==Token.EOF ) matchedEOF = true;
						_errHandler.reportMatch(this);
						consume();
					}
					setState(589);
					shift_expression();
					}
					break;
				case IS:
					{
					setState(590);
					match(IS);
					setState(591);
					isType();
					}
					break;
				case AS:
					{
					setState(592);
					match(AS);
					setState(593);
					type();
					}
					break;
				default:
					throw new NoViableAltException(this);
				}
				}
				setState(598);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Shift_expressionContext extends ParserRuleContext {
		public List<Additive_expressionContext> additive_expression() {
			return getRuleContexts(Additive_expressionContext.class);
		}
		public Additive_expressionContext additive_expression(int i) {
			return getRuleContext(Additive_expressionContext.class,i);
		}
		public List<Right_shiftContext> right_shift() {
			return getRuleContexts(Right_shiftContext.class);
		}
		public Right_shiftContext right_shift(int i) {
			return getRuleContext(Right_shiftContext.class,i);
		}
		public Shift_expressionContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_shift_expression; }
	}

	public final Shift_expressionContext shift_expression() throws RecognitionException {
		Shift_expressionContext _localctx = new Shift_expressionContext(_ctx, getState());
		enterRule(_localctx, 50, RULE_shift_expression);
		try {
			int _alt;
			enterOuterAlt(_localctx, 1);
			{
			setState(599);
			additive_expression();
			setState(607);
			_errHandler.sync(this);
			_alt = getInterpreter().adaptivePredict(_input,33,_ctx);
			while ( _alt!=2 && _alt!=org.antlr.v4.runtime.atn.ATN.INVALID_ALT_NUMBER ) {
				if ( _alt==1 ) {
					{
					{
					setState(602);
					_errHandler.sync(this);
					switch (_input.LA(1)) {
					case OP_LEFT_SHIFT:
						{
						setState(600);
						match(OP_LEFT_SHIFT);
						}
						break;
					case GT:
						{
						setState(601);
						right_shift();
						}
						break;
					default:
						throw new NoViableAltException(this);
					}
					setState(604);
					additive_expression();
					}
					} 
				}
				setState(609);
				_errHandler.sync(this);
				_alt = getInterpreter().adaptivePredict(_input,33,_ctx);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Additive_expressionContext extends ParserRuleContext {
		public List<Multiplicative_expressionContext> multiplicative_expression() {
			return getRuleContexts(Multiplicative_expressionContext.class);
		}
		public Multiplicative_expressionContext multiplicative_expression(int i) {
			return getRuleContext(Multiplicative_expressionContext.class,i);
		}
		public Additive_expressionContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_additive_expression; }
	}

	public final Additive_expressionContext additive_expression() throws RecognitionException {
		Additive_expressionContext _localctx = new Additive_expressionContext(_ctx, getState());
		enterRule(_localctx, 52, RULE_additive_expression);
		int _la;
		try {
			int _alt;
			enterOuterAlt(_localctx, 1);
			{
			setState(610);
			multiplicative_expression();
			setState(615);
			_errHandler.sync(this);
			_alt = getInterpreter().adaptivePredict(_input,34,_ctx);
			while ( _alt!=2 && _alt!=org.antlr.v4.runtime.atn.ATN.INVALID_ALT_NUMBER ) {
				if ( _alt==1 ) {
					{
					{
					setState(611);
					_la = _input.LA(1);
					if ( !(_la==PLUS || _la==MINUS) ) {
					_errHandler.recoverInline(this);
					}
					else {
						if ( _input.LA(1)==Token.EOF ) matchedEOF = true;
						_errHandler.reportMatch(this);
						consume();
					}
					setState(612);
					multiplicative_expression();
					}
					} 
				}
				setState(617);
				_errHandler.sync(this);
				_alt = getInterpreter().adaptivePredict(_input,34,_ctx);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Multiplicative_expressionContext extends ParserRuleContext {
		public List<Unary_expressionContext> unary_expression() {
			return getRuleContexts(Unary_expressionContext.class);
		}
		public Unary_expressionContext unary_expression(int i) {
			return getRuleContext(Unary_expressionContext.class,i);
		}
		public Multiplicative_expressionContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_multiplicative_expression; }
	}

	public final Multiplicative_expressionContext multiplicative_expression() throws RecognitionException {
		Multiplicative_expressionContext _localctx = new Multiplicative_expressionContext(_ctx, getState());
		enterRule(_localctx, 54, RULE_multiplicative_expression);
		int _la;
		try {
			int _alt;
			enterOuterAlt(_localctx, 1);
			{
			setState(618);
			unary_expression();
			setState(623);
			_errHandler.sync(this);
			_alt = getInterpreter().adaptivePredict(_input,35,_ctx);
			while ( _alt!=2 && _alt!=org.antlr.v4.runtime.atn.ATN.INVALID_ALT_NUMBER ) {
				if ( _alt==1 ) {
					{
					{
					setState(619);
					_la = _input.LA(1);
					if ( !(((((_la - 133)) & ~0x3f) == 0 && ((1L << (_la - 133)) & ((1L << (STAR - 133)) | (1L << (DIV - 133)) | (1L << (PERCENT - 133)))) != 0)) ) {
					_errHandler.recoverInline(this);
					}
					else {
						if ( _input.LA(1)==Token.EOF ) matchedEOF = true;
						_errHandler.reportMatch(this);
						consume();
					}
					setState(620);
					unary_expression();
					}
					} 
				}
				setState(625);
				_errHandler.sync(this);
				_alt = getInterpreter().adaptivePredict(_input,35,_ctx);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Unary_expressionContext extends ParserRuleContext {
		public Primary_expressionContext primary_expression() {
			return getRuleContext(Primary_expressionContext.class,0);
		}
		public Unary_expressionContext unary_expression() {
			return getRuleContext(Unary_expressionContext.class,0);
		}
		public TerminalNode BANG() { return getToken(CSharpParser.BANG, 0); }
		public TerminalNode OPEN_PARENS() { return getToken(CSharpParser.OPEN_PARENS, 0); }
		public TypeContext type() {
			return getRuleContext(TypeContext.class,0);
		}
		public TerminalNode CLOSE_PARENS() { return getToken(CSharpParser.CLOSE_PARENS, 0); }
		public TerminalNode AWAIT() { return getToken(CSharpParser.AWAIT, 0); }
		public Unary_expressionContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_unary_expression; }
	}

	public final Unary_expressionContext unary_expression() throws RecognitionException {
		Unary_expressionContext _localctx = new Unary_expressionContext(_ctx, getState());
		enterRule(_localctx, 56, RULE_unary_expression);
		try {
			setState(650);
			_errHandler.sync(this);
			switch ( getInterpreter().adaptivePredict(_input,36,_ctx) ) {
			case 1:
				enterOuterAlt(_localctx, 1);
				{
				setState(626);
				primary_expression();
				}
				break;
			case 2:
				enterOuterAlt(_localctx, 2);
				{
				setState(627);
				match(PLUS);
				setState(628);
				unary_expression();
				}
				break;
			case 3:
				enterOuterAlt(_localctx, 3);
				{
				setState(629);
				match(MINUS);
				setState(630);
				unary_expression();
				}
				break;
			case 4:
				enterOuterAlt(_localctx, 4);
				{
				setState(631);
				match(BANG);
				setState(632);
				unary_expression();
				}
				break;
			case 5:
				enterOuterAlt(_localctx, 5);
				{
				setState(633);
				match(TILDE);
				setState(634);
				unary_expression();
				}
				break;
			case 6:
				enterOuterAlt(_localctx, 6);
				{
				setState(635);
				match(OP_INC);
				setState(636);
				unary_expression();
				}
				break;
			case 7:
				enterOuterAlt(_localctx, 7);
				{
				setState(637);
				match(OP_DEC);
				setState(638);
				unary_expression();
				}
				break;
			case 8:
				enterOuterAlt(_localctx, 8);
				{
				setState(639);
				match(OPEN_PARENS);
				setState(640);
				type();
				setState(641);
				match(CLOSE_PARENS);
				setState(642);
				unary_expression();
				}
				break;
			case 9:
				enterOuterAlt(_localctx, 9);
				{
				setState(644);
				match(AWAIT);
				setState(645);
				unary_expression();
				}
				break;
			case 10:
				enterOuterAlt(_localctx, 10);
				{
				setState(646);
				match(AMP);
				setState(647);
				unary_expression();
				}
				break;
			case 11:
				enterOuterAlt(_localctx, 11);
				{
				setState(648);
				match(STAR);
				setState(649);
				unary_expression();
				}
				break;
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Primary_expressionContext extends ParserRuleContext {
		public Primary_expression_startContext pe;
		public Primary_expression_startContext primary_expression_start() {
			return getRuleContext(Primary_expression_startContext.class,0);
		}
		public List<Bracket_expressionContext> bracket_expression() {
			return getRuleContexts(Bracket_expressionContext.class);
		}
		public Bracket_expressionContext bracket_expression(int i) {
			return getRuleContext(Bracket_expressionContext.class,i);
		}
		public List<Member_accessContext> member_access() {
			return getRuleContexts(Member_accessContext.class);
		}
		public Member_accessContext member_access(int i) {
			return getRuleContext(Member_accessContext.class,i);
		}
		public List<Method_invocationContext> method_invocation() {
			return getRuleContexts(Method_invocationContext.class);
		}
		public Method_invocationContext method_invocation(int i) {
			return getRuleContext(Method_invocationContext.class,i);
		}
		public List<IdentifierContext> identifier() {
			return getRuleContexts(IdentifierContext.class);
		}
		public IdentifierContext identifier(int i) {
			return getRuleContext(IdentifierContext.class,i);
		}
		public Primary_expressionContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_primary_expression; }
	}

	public final Primary_expressionContext primary_expression() throws RecognitionException {
		Primary_expressionContext _localctx = new Primary_expressionContext(_ctx, getState());
		enterRule(_localctx, 58, RULE_primary_expression);
		try {
			int _alt;
			enterOuterAlt(_localctx, 1);
			{
			setState(652);
			((Primary_expressionContext)_localctx).pe = primary_expression_start();
			setState(656);
			_errHandler.sync(this);
			_alt = getInterpreter().adaptivePredict(_input,37,_ctx);
			while ( _alt!=2 && _alt!=org.antlr.v4.runtime.atn.ATN.INVALID_ALT_NUMBER ) {
				if ( _alt==1 ) {
					{
					{
					setState(653);
					bracket_expression();
					}
					} 
				}
				setState(658);
				_errHandler.sync(this);
				_alt = getInterpreter().adaptivePredict(_input,37,_ctx);
			}
			setState(675);
			_errHandler.sync(this);
			_alt = getInterpreter().adaptivePredict(_input,40,_ctx);
			while ( _alt!=2 && _alt!=org.antlr.v4.runtime.atn.ATN.INVALID_ALT_NUMBER ) {
				if ( _alt==1 ) {
					{
					{
					setState(665);
					_errHandler.sync(this);
					switch (_input.LA(1)) {
					case DOT:
					case INTERR:
						{
						setState(659);
						member_access();
						}
						break;
					case OPEN_PARENS:
						{
						setState(660);
						method_invocation();
						}
						break;
					case OP_INC:
						{
						setState(661);
						match(OP_INC);
						}
						break;
					case OP_DEC:
						{
						setState(662);
						match(OP_DEC);
						}
						break;
					case OP_PTR:
						{
						setState(663);
						match(OP_PTR);
						setState(664);
						identifier();
						}
						break;
					default:
						throw new NoViableAltException(this);
					}
					setState(670);
					_errHandler.sync(this);
					_alt = getInterpreter().adaptivePredict(_input,39,_ctx);
					while ( _alt!=2 && _alt!=org.antlr.v4.runtime.atn.ATN.INVALID_ALT_NUMBER ) {
						if ( _alt==1 ) {
							{
							{
							setState(667);
							bracket_expression();
							}
							} 
						}
						setState(672);
						_errHandler.sync(this);
						_alt = getInterpreter().adaptivePredict(_input,39,_ctx);
					}
					}
					} 
				}
				setState(677);
				_errHandler.sync(this);
				_alt = getInterpreter().adaptivePredict(_input,40,_ctx);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Primary_expression_startContext extends ParserRuleContext {
		public Primary_expression_startContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_primary_expression_start; }
	 
		public Primary_expression_startContext() { }
		public void copyFrom(Primary_expression_startContext ctx) {
			super.copyFrom(ctx);
		}
	}
	public static class LiteralAccessExpressionContext extends Primary_expression_startContext {
		public TerminalNode LITERAL_ACCESS() { return getToken(CSharpParser.LITERAL_ACCESS, 0); }
		public LiteralAccessExpressionContext(Primary_expression_startContext ctx) { copyFrom(ctx); }
	}
	public static class DefaultValueExpressionContext extends Primary_expression_startContext {
		public TerminalNode DEFAULT() { return getToken(CSharpParser.DEFAULT, 0); }
		public TerminalNode OPEN_PARENS() { return getToken(CSharpParser.OPEN_PARENS, 0); }
		public TypeContext type() {
			return getRuleContext(TypeContext.class,0);
		}
		public TerminalNode CLOSE_PARENS() { return getToken(CSharpParser.CLOSE_PARENS, 0); }
		public DefaultValueExpressionContext(Primary_expression_startContext ctx) { copyFrom(ctx); }
	}
	public static class BaseAccessExpressionContext extends Primary_expression_startContext {
		public TerminalNode BASE() { return getToken(CSharpParser.BASE, 0); }
		public IdentifierContext identifier() {
			return getRuleContext(IdentifierContext.class,0);
		}
		public Expression_listContext expression_list() {
			return getRuleContext(Expression_listContext.class,0);
		}
		public Type_argument_listContext type_argument_list() {
			return getRuleContext(Type_argument_listContext.class,0);
		}
		public BaseAccessExpressionContext(Primary_expression_startContext ctx) { copyFrom(ctx); }
	}
	public static class SizeofExpressionContext extends Primary_expression_startContext {
		public TerminalNode SIZEOF() { return getToken(CSharpParser.SIZEOF, 0); }
		public TerminalNode OPEN_PARENS() { return getToken(CSharpParser.OPEN_PARENS, 0); }
		public TypeContext type() {
			return getRuleContext(TypeContext.class,0);
		}
		public TerminalNode CLOSE_PARENS() { return getToken(CSharpParser.CLOSE_PARENS, 0); }
		public SizeofExpressionContext(Primary_expression_startContext ctx) { copyFrom(ctx); }
	}
	public static class ParenthesisExpressionsContext extends Primary_expression_startContext {
		public TerminalNode OPEN_PARENS() { return getToken(CSharpParser.OPEN_PARENS, 0); }
		public ExpressionContext expression() {
			return getRuleContext(ExpressionContext.class,0);
		}
		public TerminalNode CLOSE_PARENS() { return getToken(CSharpParser.CLOSE_PARENS, 0); }
		public ParenthesisExpressionsContext(Primary_expression_startContext ctx) { copyFrom(ctx); }
	}
	public static class ThisReferenceExpressionContext extends Primary_expression_startContext {
		public TerminalNode THIS() { return getToken(CSharpParser.THIS, 0); }
		public ThisReferenceExpressionContext(Primary_expression_startContext ctx) { copyFrom(ctx); }
	}
	public static class ObjectCreationExpressionContext extends Primary_expression_startContext {
		public TerminalNode NEW() { return getToken(CSharpParser.NEW, 0); }
		public TypeContext type() {
			return getRuleContext(TypeContext.class,0);
		}
		public Anonymous_object_initializerContext anonymous_object_initializer() {
			return getRuleContext(Anonymous_object_initializerContext.class,0);
		}
		public List<Rank_specifierContext> rank_specifier() {
			return getRuleContexts(Rank_specifierContext.class);
		}
		public Rank_specifierContext rank_specifier(int i) {
			return getRuleContext(Rank_specifierContext.class,i);
		}
		public Array_initializerContext array_initializer() {
			return getRuleContext(Array_initializerContext.class,0);
		}
		public Object_creation_expressionContext object_creation_expression() {
			return getRuleContext(Object_creation_expressionContext.class,0);
		}
		public Object_or_collection_initializerContext object_or_collection_initializer() {
			return getRuleContext(Object_or_collection_initializerContext.class,0);
		}
		public Expression_listContext expression_list() {
			return getRuleContext(Expression_listContext.class,0);
		}
		public ObjectCreationExpressionContext(Primary_expression_startContext ctx) { copyFrom(ctx); }
	}
	public static class AnonymousMethodExpressionContext extends Primary_expression_startContext {
		public TerminalNode DELEGATE() { return getToken(CSharpParser.DELEGATE, 0); }
		public BlockContext block() {
			return getRuleContext(BlockContext.class,0);
		}
		public TerminalNode ASYNC() { return getToken(CSharpParser.ASYNC, 0); }
		public TerminalNode OPEN_PARENS() { return getToken(CSharpParser.OPEN_PARENS, 0); }
		public TerminalNode CLOSE_PARENS() { return getToken(CSharpParser.CLOSE_PARENS, 0); }
		public Explicit_anonymous_function_parameter_listContext explicit_anonymous_function_parameter_list() {
			return getRuleContext(Explicit_anonymous_function_parameter_listContext.class,0);
		}
		public AnonymousMethodExpressionContext(Primary_expression_startContext ctx) { copyFrom(ctx); }
	}
	public static class TypeofExpressionContext extends Primary_expression_startContext {
		public TerminalNode TYPEOF() { return getToken(CSharpParser.TYPEOF, 0); }
		public TerminalNode OPEN_PARENS() { return getToken(CSharpParser.OPEN_PARENS, 0); }
		public TerminalNode CLOSE_PARENS() { return getToken(CSharpParser.CLOSE_PARENS, 0); }
		public Unbound_type_nameContext unbound_type_name() {
			return getRuleContext(Unbound_type_nameContext.class,0);
		}
		public TypeContext type() {
			return getRuleContext(TypeContext.class,0);
		}
		public TerminalNode VOID() { return getToken(CSharpParser.VOID, 0); }
		public TypeofExpressionContext(Primary_expression_startContext ctx) { copyFrom(ctx); }
	}
	public static class UncheckedExpressionContext extends Primary_expression_startContext {
		public TerminalNode UNCHECKED() { return getToken(CSharpParser.UNCHECKED, 0); }
		public TerminalNode OPEN_PARENS() { return getToken(CSharpParser.OPEN_PARENS, 0); }
		public ExpressionContext expression() {
			return getRuleContext(ExpressionContext.class,0);
		}
		public TerminalNode CLOSE_PARENS() { return getToken(CSharpParser.CLOSE_PARENS, 0); }
		public UncheckedExpressionContext(Primary_expression_startContext ctx) { copyFrom(ctx); }
	}
	public static class SimpleNameExpressionContext extends Primary_expression_startContext {
		public IdentifierContext identifier() {
			return getRuleContext(IdentifierContext.class,0);
		}
		public Type_argument_listContext type_argument_list() {
			return getRuleContext(Type_argument_listContext.class,0);
		}
		public SimpleNameExpressionContext(Primary_expression_startContext ctx) { copyFrom(ctx); }
	}
	public static class MemberAccessExpressionContext extends Primary_expression_startContext {
		public Predefined_typeContext predefined_type() {
			return getRuleContext(Predefined_typeContext.class,0);
		}
		public Qualified_alias_memberContext qualified_alias_member() {
			return getRuleContext(Qualified_alias_memberContext.class,0);
		}
		public MemberAccessExpressionContext(Primary_expression_startContext ctx) { copyFrom(ctx); }
	}
	public static class CheckedExpressionContext extends Primary_expression_startContext {
		public TerminalNode CHECKED() { return getToken(CSharpParser.CHECKED, 0); }
		public TerminalNode OPEN_PARENS() { return getToken(CSharpParser.OPEN_PARENS, 0); }
		public ExpressionContext expression() {
			return getRuleContext(ExpressionContext.class,0);
		}
		public TerminalNode CLOSE_PARENS() { return getToken(CSharpParser.CLOSE_PARENS, 0); }
		public CheckedExpressionContext(Primary_expression_startContext ctx) { copyFrom(ctx); }
	}
	public static class LiteralExpressionContext extends Primary_expression_startContext {
		public LiteralContext literal() {
			return getRuleContext(LiteralContext.class,0);
		}
		public LiteralExpressionContext(Primary_expression_startContext ctx) { copyFrom(ctx); }
	}
	public static class NameofExpressionContext extends Primary_expression_startContext {
		public TerminalNode NAMEOF() { return getToken(CSharpParser.NAMEOF, 0); }
		public TerminalNode OPEN_PARENS() { return getToken(CSharpParser.OPEN_PARENS, 0); }
		public List<IdentifierContext> identifier() {
			return getRuleContexts(IdentifierContext.class);
		}
		public IdentifierContext identifier(int i) {
			return getRuleContext(IdentifierContext.class,i);
		}
		public TerminalNode CLOSE_PARENS() { return getToken(CSharpParser.CLOSE_PARENS, 0); }
		public NameofExpressionContext(Primary_expression_startContext ctx) { copyFrom(ctx); }
	}

	public final Primary_expression_startContext primary_expression_start() throws RecognitionException {
		Primary_expression_startContext _localctx = new Primary_expression_startContext(_ctx, getState());
		enterRule(_localctx, 60, RULE_primary_expression_start);
		int _la;
		try {
			int _alt;
			setState(787);
			_errHandler.sync(this);
			switch ( getInterpreter().adaptivePredict(_input,54,_ctx) ) {
			case 1:
				_localctx = new LiteralExpressionContext(_localctx);
				enterOuterAlt(_localctx, 1);
				{
				setState(678);
				literal();
				}
				break;
			case 2:
				_localctx = new SimpleNameExpressionContext(_localctx);
				enterOuterAlt(_localctx, 2);
				{
				setState(679);
				identifier();
				setState(681);
				_errHandler.sync(this);
				switch ( getInterpreter().adaptivePredict(_input,41,_ctx) ) {
				case 1:
					{
					setState(680);
					type_argument_list();
					}
					break;
				}
				}
				break;
			case 3:
				_localctx = new ParenthesisExpressionsContext(_localctx);
				enterOuterAlt(_localctx, 3);
				{
				setState(683);
				match(OPEN_PARENS);
				setState(684);
				expression();
				setState(685);
				match(CLOSE_PARENS);
				}
				break;
			case 4:
				_localctx = new MemberAccessExpressionContext(_localctx);
				enterOuterAlt(_localctx, 4);
				{
				setState(687);
				predefined_type();
				}
				break;
			case 5:
				_localctx = new MemberAccessExpressionContext(_localctx);
				enterOuterAlt(_localctx, 5);
				{
				setState(688);
				qualified_alias_member();
				}
				break;
			case 6:
				_localctx = new LiteralAccessExpressionContext(_localctx);
				enterOuterAlt(_localctx, 6);
				{
				setState(689);
				match(LITERAL_ACCESS);
				}
				break;
			case 7:
				_localctx = new ThisReferenceExpressionContext(_localctx);
				enterOuterAlt(_localctx, 7);
				{
				setState(690);
				match(THIS);
				}
				break;
			case 8:
				_localctx = new BaseAccessExpressionContext(_localctx);
				enterOuterAlt(_localctx, 8);
				{
				setState(691);
				match(BASE);
				setState(701);
				_errHandler.sync(this);
				switch (_input.LA(1)) {
				case DOT:
					{
					setState(692);
					match(DOT);
					setState(693);
					identifier();
					setState(695);
					_errHandler.sync(this);
					switch ( getInterpreter().adaptivePredict(_input,42,_ctx) ) {
					case 1:
						{
						setState(694);
						type_argument_list();
						}
						break;
					}
					}
					break;
				case OPEN_BRACKET:
					{
					setState(697);
					match(OPEN_BRACKET);
					setState(698);
					expression_list();
					setState(699);
					match(CLOSE_BRACKET);
					}
					break;
				default:
					throw new NoViableAltException(this);
				}
				}
				break;
			case 9:
				_localctx = new ObjectCreationExpressionContext(_localctx);
				enterOuterAlt(_localctx, 9);
				{
				setState(703);
				match(NEW);
				setState(732);
				_errHandler.sync(this);
				switch (_input.LA(1)) {
				case ADD:
				case ALIAS:
				case ARGLIST:
				case ASCENDING:
				case ASYNC:
				case AWAIT:
				case BOOL:
				case BY:
				case BYTE:
				case CHAR:
				case DECIMAL:
				case DESCENDING:
				case DOUBLE:
				case DYNAMIC:
				case EQUALS:
				case FLOAT:
				case FROM:
				case GET:
				case GROUP:
				case INT:
				case INTO:
				case JOIN:
				case LET:
				case LONG:
				case NAMEOF:
				case OBJECT:
				case ON:
				case ORDERBY:
				case PARTIAL:
				case REMOVE:
				case SBYTE:
				case SELECT:
				case SET:
				case SHORT:
				case STRING:
				case UINT:
				case ULONG:
				case USHORT:
				case VOID:
				case WHEN:
				case WHERE:
				case YIELD:
				case IDENTIFIER:
					{
					setState(704);
					type();
					setState(726);
					_errHandler.sync(this);
					switch ( getInterpreter().adaptivePredict(_input,47,_ctx) ) {
					case 1:
						{
						setState(705);
						object_creation_expression();
						}
						break;
					case 2:
						{
						setState(706);
						object_or_collection_initializer();
						}
						break;
					case 3:
						{
						setState(707);
						match(OPEN_BRACKET);
						setState(708);
						expression_list();
						setState(709);
						match(CLOSE_BRACKET);
						setState(713);
						_errHandler.sync(this);
						_alt = getInterpreter().adaptivePredict(_input,44,_ctx);
						while ( _alt!=2 && _alt!=org.antlr.v4.runtime.atn.ATN.INVALID_ALT_NUMBER ) {
							if ( _alt==1 ) {
								{
								{
								setState(710);
								rank_specifier();
								}
								} 
							}
							setState(715);
							_errHandler.sync(this);
							_alt = getInterpreter().adaptivePredict(_input,44,_ctx);
						}
						setState(717);
						_errHandler.sync(this);
						_la = _input.LA(1);
						if (_la==OPEN_BRACE) {
							{
							setState(716);
							array_initializer();
							}
						}

						}
						break;
					case 4:
						{
						setState(720); 
						_errHandler.sync(this);
						_la = _input.LA(1);
						do {
							{
							{
							setState(719);
							rank_specifier();
							}
							}
							setState(722); 
							_errHandler.sync(this);
							_la = _input.LA(1);
						} while ( _la==OPEN_BRACKET );
						setState(724);
						array_initializer();
						}
						break;
					}
					}
					break;
				case OPEN_BRACE:
					{
					setState(728);
					anonymous_object_initializer();
					}
					break;
				case OPEN_BRACKET:
					{
					setState(729);
					rank_specifier();
					setState(730);
					array_initializer();
					}
					break;
				default:
					throw new NoViableAltException(this);
				}
				}
				break;
			case 10:
				_localctx = new TypeofExpressionContext(_localctx);
				enterOuterAlt(_localctx, 10);
				{
				setState(734);
				match(TYPEOF);
				setState(735);
				match(OPEN_PARENS);
				setState(739);
				_errHandler.sync(this);
				switch ( getInterpreter().adaptivePredict(_input,49,_ctx) ) {
				case 1:
					{
					setState(736);
					unbound_type_name();
					}
					break;
				case 2:
					{
					setState(737);
					type();
					}
					break;
				case 3:
					{
					setState(738);
					match(VOID);
					}
					break;
				}
				setState(741);
				match(CLOSE_PARENS);
				}
				break;
			case 11:
				_localctx = new CheckedExpressionContext(_localctx);
				enterOuterAlt(_localctx, 11);
				{
				setState(742);
				match(CHECKED);
				setState(743);
				match(OPEN_PARENS);
				setState(744);
				expression();
				setState(745);
				match(CLOSE_PARENS);
				}
				break;
			case 12:
				_localctx = new UncheckedExpressionContext(_localctx);
				enterOuterAlt(_localctx, 12);
				{
				setState(747);
				match(UNCHECKED);
				setState(748);
				match(OPEN_PARENS);
				setState(749);
				expression();
				setState(750);
				match(CLOSE_PARENS);
				}
				break;
			case 13:
				_localctx = new DefaultValueExpressionContext(_localctx);
				enterOuterAlt(_localctx, 13);
				{
				setState(752);
				match(DEFAULT);
				setState(753);
				match(OPEN_PARENS);
				setState(754);
				type();
				setState(755);
				match(CLOSE_PARENS);
				}
				break;
			case 14:
				_localctx = new AnonymousMethodExpressionContext(_localctx);
				enterOuterAlt(_localctx, 14);
				{
				setState(758);
				_errHandler.sync(this);
				_la = _input.LA(1);
				if (_la==ASYNC) {
					{
					setState(757);
					match(ASYNC);
					}
				}

				setState(760);
				match(DELEGATE);
				setState(766);
				_errHandler.sync(this);
				_la = _input.LA(1);
				if (_la==OPEN_PARENS) {
					{
					setState(761);
					match(OPEN_PARENS);
					setState(763);
					_errHandler.sync(this);
					_la = _input.LA(1);
					if ((((_la) & ~0x3f) == 0 && ((1L << _la) & ((1L << ADD) | (1L << ALIAS) | (1L << ARGLIST) | (1L << ASCENDING) | (1L << ASYNC) | (1L << AWAIT) | (1L << BOOL) | (1L << BY) | (1L << BYTE) | (1L << CHAR) | (1L << DECIMAL) | (1L << DESCENDING) | (1L << DOUBLE) | (1L << DYNAMIC) | (1L << EQUALS) | (1L << FLOAT) | (1L << FROM) | (1L << GET) | (1L << GROUP) | (1L << INT) | (1L << INTO) | (1L << JOIN) | (1L << LET) | (1L << LONG) | (1L << NAMEOF))) != 0) || ((((_la - 67)) & ~0x3f) == 0 && ((1L << (_la - 67)) & ((1L << (OBJECT - 67)) | (1L << (ON - 67)) | (1L << (ORDERBY - 67)) | (1L << (OUT - 67)) | (1L << (PARTIAL - 67)) | (1L << (REF - 67)) | (1L << (REMOVE - 67)) | (1L << (SBYTE - 67)) | (1L << (SELECT - 67)) | (1L << (SET - 67)) | (1L << (SHORT - 67)) | (1L << (STRING - 67)) | (1L << (UINT - 67)) | (1L << (ULONG - 67)) | (1L << (USHORT - 67)) | (1L << (VOID - 67)) | (1L << (WHEN - 67)) | (1L << (WHERE - 67)) | (1L << (YIELD - 67)) | (1L << (IDENTIFIER - 67)))) != 0)) {
						{
						setState(762);
						explicit_anonymous_function_parameter_list();
						}
					}

					setState(765);
					match(CLOSE_PARENS);
					}
				}

				setState(768);
				block();
				}
				break;
			case 15:
				_localctx = new SizeofExpressionContext(_localctx);
				enterOuterAlt(_localctx, 15);
				{
				setState(769);
				match(SIZEOF);
				setState(770);
				match(OPEN_PARENS);
				setState(771);
				type();
				setState(772);
				match(CLOSE_PARENS);
				}
				break;
			case 16:
				_localctx = new NameofExpressionContext(_localctx);
				enterOuterAlt(_localctx, 16);
				{
				setState(774);
				match(NAMEOF);
				setState(775);
				match(OPEN_PARENS);
				setState(781);
				_errHandler.sync(this);
				_alt = getInterpreter().adaptivePredict(_input,53,_ctx);
				while ( _alt!=2 && _alt!=org.antlr.v4.runtime.atn.ATN.INVALID_ALT_NUMBER ) {
					if ( _alt==1 ) {
						{
						{
						setState(776);
						identifier();
						setState(777);
						match(DOT);
						}
						} 
					}
					setState(783);
					_errHandler.sync(this);
					_alt = getInterpreter().adaptivePredict(_input,53,_ctx);
				}
				setState(784);
				identifier();
				setState(785);
				match(CLOSE_PARENS);
				}
				break;
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Member_accessContext extends ParserRuleContext {
		public IdentifierContext identifier() {
			return getRuleContext(IdentifierContext.class,0);
		}
		public Type_argument_listContext type_argument_list() {
			return getRuleContext(Type_argument_listContext.class,0);
		}
		public Member_accessContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_member_access; }
	}

	public final Member_accessContext member_access() throws RecognitionException {
		Member_accessContext _localctx = new Member_accessContext(_ctx, getState());
		enterRule(_localctx, 62, RULE_member_access);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(790);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==INTERR) {
				{
				setState(789);
				match(INTERR);
				}
			}

			setState(792);
			match(DOT);
			setState(793);
			identifier();
			setState(795);
			_errHandler.sync(this);
			switch ( getInterpreter().adaptivePredict(_input,56,_ctx) ) {
			case 1:
				{
				setState(794);
				type_argument_list();
				}
				break;
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Bracket_expressionContext extends ParserRuleContext {
		public List<Indexer_argumentContext> indexer_argument() {
			return getRuleContexts(Indexer_argumentContext.class);
		}
		public Indexer_argumentContext indexer_argument(int i) {
			return getRuleContext(Indexer_argumentContext.class,i);
		}
		public Bracket_expressionContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_bracket_expression; }
	}

	public final Bracket_expressionContext bracket_expression() throws RecognitionException {
		Bracket_expressionContext _localctx = new Bracket_expressionContext(_ctx, getState());
		enterRule(_localctx, 64, RULE_bracket_expression);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(798);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==INTERR) {
				{
				setState(797);
				match(INTERR);
				}
			}

			setState(800);
			match(OPEN_BRACKET);
			setState(801);
			indexer_argument();
			setState(806);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while (_la==COMMA) {
				{
				{
				setState(802);
				match(COMMA);
				setState(803);
				indexer_argument();
				}
				}
				setState(808);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			setState(809);
			match(CLOSE_BRACKET);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Indexer_argumentContext extends ParserRuleContext {
		public ExpressionContext expression() {
			return getRuleContext(ExpressionContext.class,0);
		}
		public IdentifierContext identifier() {
			return getRuleContext(IdentifierContext.class,0);
		}
		public Indexer_argumentContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_indexer_argument; }
	}

	public final Indexer_argumentContext indexer_argument() throws RecognitionException {
		Indexer_argumentContext _localctx = new Indexer_argumentContext(_ctx, getState());
		enterRule(_localctx, 66, RULE_indexer_argument);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(814);
			_errHandler.sync(this);
			switch ( getInterpreter().adaptivePredict(_input,59,_ctx) ) {
			case 1:
				{
				setState(811);
				identifier();
				setState(812);
				match(COLON);
				}
				break;
			}
			setState(816);
			expression();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Predefined_typeContext extends ParserRuleContext {
		public TerminalNode BOOL() { return getToken(CSharpParser.BOOL, 0); }
		public TerminalNode BYTE() { return getToken(CSharpParser.BYTE, 0); }
		public TerminalNode CHAR() { return getToken(CSharpParser.CHAR, 0); }
		public TerminalNode DECIMAL() { return getToken(CSharpParser.DECIMAL, 0); }
		public TerminalNode DOUBLE() { return getToken(CSharpParser.DOUBLE, 0); }
		public TerminalNode FLOAT() { return getToken(CSharpParser.FLOAT, 0); }
		public TerminalNode INT() { return getToken(CSharpParser.INT, 0); }
		public TerminalNode LONG() { return getToken(CSharpParser.LONG, 0); }
		public TerminalNode OBJECT() { return getToken(CSharpParser.OBJECT, 0); }
		public TerminalNode SBYTE() { return getToken(CSharpParser.SBYTE, 0); }
		public TerminalNode SHORT() { return getToken(CSharpParser.SHORT, 0); }
		public TerminalNode STRING() { return getToken(CSharpParser.STRING, 0); }
		public TerminalNode UINT() { return getToken(CSharpParser.UINT, 0); }
		public TerminalNode ULONG() { return getToken(CSharpParser.ULONG, 0); }
		public TerminalNode USHORT() { return getToken(CSharpParser.USHORT, 0); }
		public Predefined_typeContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_predefined_type; }
	}

	public final Predefined_typeContext predefined_type() throws RecognitionException {
		Predefined_typeContext _localctx = new Predefined_typeContext(_ctx, getState());
		enterRule(_localctx, 68, RULE_predefined_type);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(818);
			_la = _input.LA(1);
			if ( !((((_la) & ~0x3f) == 0 && ((1L << _la) & ((1L << BOOL) | (1L << BYTE) | (1L << CHAR) | (1L << DECIMAL) | (1L << DOUBLE) | (1L << FLOAT) | (1L << INT) | (1L << LONG))) != 0) || ((((_la - 67)) & ~0x3f) == 0 && ((1L << (_la - 67)) & ((1L << (OBJECT - 67)) | (1L << (SBYTE - 67)) | (1L << (SHORT - 67)) | (1L << (STRING - 67)) | (1L << (UINT - 67)) | (1L << (ULONG - 67)) | (1L << (USHORT - 67)))) != 0)) ) {
			_errHandler.recoverInline(this);
			}
			else {
				if ( _input.LA(1)==Token.EOF ) matchedEOF = true;
				_errHandler.reportMatch(this);
				consume();
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Expression_listContext extends ParserRuleContext {
		public List<ExpressionContext> expression() {
			return getRuleContexts(ExpressionContext.class);
		}
		public ExpressionContext expression(int i) {
			return getRuleContext(ExpressionContext.class,i);
		}
		public Expression_listContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_expression_list; }
	}

	public final Expression_listContext expression_list() throws RecognitionException {
		Expression_listContext _localctx = new Expression_listContext(_ctx, getState());
		enterRule(_localctx, 70, RULE_expression_list);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(820);
			expression();
			setState(825);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while (_la==COMMA) {
				{
				{
				setState(821);
				match(COMMA);
				setState(822);
				expression();
				}
				}
				setState(827);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Object_or_collection_initializerContext extends ParserRuleContext {
		public Object_initializerContext object_initializer() {
			return getRuleContext(Object_initializerContext.class,0);
		}
		public Collection_initializerContext collection_initializer() {
			return getRuleContext(Collection_initializerContext.class,0);
		}
		public Object_or_collection_initializerContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_object_or_collection_initializer; }
	}

	public final Object_or_collection_initializerContext object_or_collection_initializer() throws RecognitionException {
		Object_or_collection_initializerContext _localctx = new Object_or_collection_initializerContext(_ctx, getState());
		enterRule(_localctx, 72, RULE_object_or_collection_initializer);
		try {
			setState(830);
			_errHandler.sync(this);
			switch ( getInterpreter().adaptivePredict(_input,61,_ctx) ) {
			case 1:
				enterOuterAlt(_localctx, 1);
				{
				setState(828);
				object_initializer();
				}
				break;
			case 2:
				enterOuterAlt(_localctx, 2);
				{
				setState(829);
				collection_initializer();
				}
				break;
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Object_initializerContext extends ParserRuleContext {
		public TerminalNode OPEN_BRACE() { return getToken(CSharpParser.OPEN_BRACE, 0); }
		public TerminalNode CLOSE_BRACE() { return getToken(CSharpParser.CLOSE_BRACE, 0); }
		public Member_initializer_listContext member_initializer_list() {
			return getRuleContext(Member_initializer_listContext.class,0);
		}
		public Object_initializerContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_object_initializer; }
	}

	public final Object_initializerContext object_initializer() throws RecognitionException {
		Object_initializerContext _localctx = new Object_initializerContext(_ctx, getState());
		enterRule(_localctx, 74, RULE_object_initializer);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(832);
			match(OPEN_BRACE);
			setState(837);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if ((((_la) & ~0x3f) == 0 && ((1L << _la) & ((1L << ADD) | (1L << ALIAS) | (1L << ARGLIST) | (1L << ASCENDING) | (1L << ASYNC) | (1L << AWAIT) | (1L << BY) | (1L << DESCENDING) | (1L << DYNAMIC) | (1L << EQUALS) | (1L << FROM) | (1L << GET) | (1L << GROUP) | (1L << INTO) | (1L << JOIN) | (1L << LET) | (1L << NAMEOF))) != 0) || ((((_la - 68)) & ~0x3f) == 0 && ((1L << (_la - 68)) & ((1L << (ON - 68)) | (1L << (ORDERBY - 68)) | (1L << (PARTIAL - 68)) | (1L << (REMOVE - 68)) | (1L << (SELECT - 68)) | (1L << (SET - 68)) | (1L << (WHEN - 68)) | (1L << (WHERE - 68)) | (1L << (YIELD - 68)) | (1L << (IDENTIFIER - 68)) | (1L << (OPEN_BRACKET - 68)))) != 0)) {
				{
				setState(833);
				member_initializer_list();
				setState(835);
				_errHandler.sync(this);
				_la = _input.LA(1);
				if (_la==COMMA) {
					{
					setState(834);
					match(COMMA);
					}
				}

				}
			}

			setState(839);
			match(CLOSE_BRACE);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Member_initializer_listContext extends ParserRuleContext {
		public List<Member_initializerContext> member_initializer() {
			return getRuleContexts(Member_initializerContext.class);
		}
		public Member_initializerContext member_initializer(int i) {
			return getRuleContext(Member_initializerContext.class,i);
		}
		public Member_initializer_listContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_member_initializer_list; }
	}

	public final Member_initializer_listContext member_initializer_list() throws RecognitionException {
		Member_initializer_listContext _localctx = new Member_initializer_listContext(_ctx, getState());
		enterRule(_localctx, 76, RULE_member_initializer_list);
		try {
			int _alt;
			enterOuterAlt(_localctx, 1);
			{
			setState(841);
			member_initializer();
			setState(846);
			_errHandler.sync(this);
			_alt = getInterpreter().adaptivePredict(_input,64,_ctx);
			while ( _alt!=2 && _alt!=org.antlr.v4.runtime.atn.ATN.INVALID_ALT_NUMBER ) {
				if ( _alt==1 ) {
					{
					{
					setState(842);
					match(COMMA);
					setState(843);
					member_initializer();
					}
					} 
				}
				setState(848);
				_errHandler.sync(this);
				_alt = getInterpreter().adaptivePredict(_input,64,_ctx);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Member_initializerContext extends ParserRuleContext {
		public Initializer_valueContext initializer_value() {
			return getRuleContext(Initializer_valueContext.class,0);
		}
		public IdentifierContext identifier() {
			return getRuleContext(IdentifierContext.class,0);
		}
		public ExpressionContext expression() {
			return getRuleContext(ExpressionContext.class,0);
		}
		public Member_initializerContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_member_initializer; }
	}

	public final Member_initializerContext member_initializer() throws RecognitionException {
		Member_initializerContext _localctx = new Member_initializerContext(_ctx, getState());
		enterRule(_localctx, 78, RULE_member_initializer);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(854);
			_errHandler.sync(this);
			switch (_input.LA(1)) {
			case ADD:
			case ALIAS:
			case ARGLIST:
			case ASCENDING:
			case ASYNC:
			case AWAIT:
			case BY:
			case DESCENDING:
			case DYNAMIC:
			case EQUALS:
			case FROM:
			case GET:
			case GROUP:
			case INTO:
			case JOIN:
			case LET:
			case NAMEOF:
			case ON:
			case ORDERBY:
			case PARTIAL:
			case REMOVE:
			case SELECT:
			case SET:
			case WHEN:
			case WHERE:
			case YIELD:
			case IDENTIFIER:
				{
				setState(849);
				identifier();
				}
				break;
			case OPEN_BRACKET:
				{
				setState(850);
				match(OPEN_BRACKET);
				setState(851);
				expression();
				setState(852);
				match(CLOSE_BRACKET);
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
			setState(856);
			match(ASSIGNMENT);
			setState(857);
			initializer_value();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Initializer_valueContext extends ParserRuleContext {
		public ExpressionContext expression() {
			return getRuleContext(ExpressionContext.class,0);
		}
		public Object_or_collection_initializerContext object_or_collection_initializer() {
			return getRuleContext(Object_or_collection_initializerContext.class,0);
		}
		public Initializer_valueContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_initializer_value; }
	}

	public final Initializer_valueContext initializer_value() throws RecognitionException {
		Initializer_valueContext _localctx = new Initializer_valueContext(_ctx, getState());
		enterRule(_localctx, 80, RULE_initializer_value);
		try {
			setState(861);
			_errHandler.sync(this);
			switch (_input.LA(1)) {
			case ADD:
			case ALIAS:
			case ARGLIST:
			case ASCENDING:
			case ASYNC:
			case AWAIT:
			case BASE:
			case BOOL:
			case BY:
			case BYTE:
			case CHAR:
			case CHECKED:
			case DECIMAL:
			case DEFAULT:
			case DELEGATE:
			case DESCENDING:
			case DOUBLE:
			case DYNAMIC:
			case EQUALS:
			case FALSE:
			case FLOAT:
			case FROM:
			case GET:
			case GROUP:
			case INT:
			case INTO:
			case JOIN:
			case LET:
			case LONG:
			case NAMEOF:
			case NEW:
			case NULL:
			case OBJECT:
			case ON:
			case ORDERBY:
			case PARTIAL:
			case REMOVE:
			case SBYTE:
			case SELECT:
			case SET:
			case SHORT:
			case SIZEOF:
			case STRING:
			case THIS:
			case TRUE:
			case TYPEOF:
			case UINT:
			case ULONG:
			case UNCHECKED:
			case USHORT:
			case WHEN:
			case WHERE:
			case YIELD:
			case IDENTIFIER:
			case LITERAL_ACCESS:
			case INTEGER_LITERAL:
			case HEX_INTEGER_LITERAL:
			case REAL_LITERAL:
			case CHARACTER_LITERAL:
			case REGULAR_STRING:
			case VERBATIUM_STRING:
			case INTERPOLATED_REGULAR_STRING_START:
			case INTERPOLATED_VERBATIUM_STRING_START:
			case OPEN_PARENS:
			case PLUS:
			case MINUS:
			case STAR:
			case AMP:
			case BANG:
			case TILDE:
			case OP_INC:
			case OP_DEC:
				enterOuterAlt(_localctx, 1);
				{
				setState(859);
				expression();
				}
				break;
			case OPEN_BRACE:
				enterOuterAlt(_localctx, 2);
				{
				setState(860);
				object_or_collection_initializer();
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Collection_initializerContext extends ParserRuleContext {
		public TerminalNode OPEN_BRACE() { return getToken(CSharpParser.OPEN_BRACE, 0); }
		public List<Element_initializerContext> element_initializer() {
			return getRuleContexts(Element_initializerContext.class);
		}
		public Element_initializerContext element_initializer(int i) {
			return getRuleContext(Element_initializerContext.class,i);
		}
		public TerminalNode CLOSE_BRACE() { return getToken(CSharpParser.CLOSE_BRACE, 0); }
		public Collection_initializerContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_collection_initializer; }
	}

	public final Collection_initializerContext collection_initializer() throws RecognitionException {
		Collection_initializerContext _localctx = new Collection_initializerContext(_ctx, getState());
		enterRule(_localctx, 82, RULE_collection_initializer);
		int _la;
		try {
			int _alt;
			enterOuterAlt(_localctx, 1);
			{
			setState(863);
			match(OPEN_BRACE);
			setState(864);
			element_initializer();
			setState(869);
			_errHandler.sync(this);
			_alt = getInterpreter().adaptivePredict(_input,67,_ctx);
			while ( _alt!=2 && _alt!=org.antlr.v4.runtime.atn.ATN.INVALID_ALT_NUMBER ) {
				if ( _alt==1 ) {
					{
					{
					setState(865);
					match(COMMA);
					setState(866);
					element_initializer();
					}
					} 
				}
				setState(871);
				_errHandler.sync(this);
				_alt = getInterpreter().adaptivePredict(_input,67,_ctx);
			}
			setState(873);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==COMMA) {
				{
				setState(872);
				match(COMMA);
				}
			}

			setState(875);
			match(CLOSE_BRACE);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Element_initializerContext extends ParserRuleContext {
		public Non_assignment_expressionContext non_assignment_expression() {
			return getRuleContext(Non_assignment_expressionContext.class,0);
		}
		public TerminalNode OPEN_BRACE() { return getToken(CSharpParser.OPEN_BRACE, 0); }
		public Expression_listContext expression_list() {
			return getRuleContext(Expression_listContext.class,0);
		}
		public TerminalNode CLOSE_BRACE() { return getToken(CSharpParser.CLOSE_BRACE, 0); }
		public Element_initializerContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_element_initializer; }
	}

	public final Element_initializerContext element_initializer() throws RecognitionException {
		Element_initializerContext _localctx = new Element_initializerContext(_ctx, getState());
		enterRule(_localctx, 84, RULE_element_initializer);
		try {
			setState(882);
			_errHandler.sync(this);
			switch (_input.LA(1)) {
			case ADD:
			case ALIAS:
			case ARGLIST:
			case ASCENDING:
			case ASYNC:
			case AWAIT:
			case BASE:
			case BOOL:
			case BY:
			case BYTE:
			case CHAR:
			case CHECKED:
			case DECIMAL:
			case DEFAULT:
			case DELEGATE:
			case DESCENDING:
			case DOUBLE:
			case DYNAMIC:
			case EQUALS:
			case FALSE:
			case FLOAT:
			case FROM:
			case GET:
			case GROUP:
			case INT:
			case INTO:
			case JOIN:
			case LET:
			case LONG:
			case NAMEOF:
			case NEW:
			case NULL:
			case OBJECT:
			case ON:
			case ORDERBY:
			case PARTIAL:
			case REMOVE:
			case SBYTE:
			case SELECT:
			case SET:
			case SHORT:
			case SIZEOF:
			case STRING:
			case THIS:
			case TRUE:
			case TYPEOF:
			case UINT:
			case ULONG:
			case UNCHECKED:
			case USHORT:
			case WHEN:
			case WHERE:
			case YIELD:
			case IDENTIFIER:
			case LITERAL_ACCESS:
			case INTEGER_LITERAL:
			case HEX_INTEGER_LITERAL:
			case REAL_LITERAL:
			case CHARACTER_LITERAL:
			case REGULAR_STRING:
			case VERBATIUM_STRING:
			case INTERPOLATED_REGULAR_STRING_START:
			case INTERPOLATED_VERBATIUM_STRING_START:
			case OPEN_PARENS:
			case PLUS:
			case MINUS:
			case STAR:
			case AMP:
			case BANG:
			case TILDE:
			case OP_INC:
			case OP_DEC:
				enterOuterAlt(_localctx, 1);
				{
				setState(877);
				non_assignment_expression();
				}
				break;
			case OPEN_BRACE:
				enterOuterAlt(_localctx, 2);
				{
				setState(878);
				match(OPEN_BRACE);
				setState(879);
				expression_list();
				setState(880);
				match(CLOSE_BRACE);
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Anonymous_object_initializerContext extends ParserRuleContext {
		public TerminalNode OPEN_BRACE() { return getToken(CSharpParser.OPEN_BRACE, 0); }
		public TerminalNode CLOSE_BRACE() { return getToken(CSharpParser.CLOSE_BRACE, 0); }
		public Member_declarator_listContext member_declarator_list() {
			return getRuleContext(Member_declarator_listContext.class,0);
		}
		public Anonymous_object_initializerContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_anonymous_object_initializer; }
	}

	public final Anonymous_object_initializerContext anonymous_object_initializer() throws RecognitionException {
		Anonymous_object_initializerContext _localctx = new Anonymous_object_initializerContext(_ctx, getState());
		enterRule(_localctx, 86, RULE_anonymous_object_initializer);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(884);
			match(OPEN_BRACE);
			setState(889);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if ((((_la) & ~0x3f) == 0 && ((1L << _la) & ((1L << ADD) | (1L << ALIAS) | (1L << ARGLIST) | (1L << ASCENDING) | (1L << ASYNC) | (1L << AWAIT) | (1L << BASE) | (1L << BOOL) | (1L << BY) | (1L << BYTE) | (1L << CHAR) | (1L << CHECKED) | (1L << DECIMAL) | (1L << DEFAULT) | (1L << DELEGATE) | (1L << DESCENDING) | (1L << DOUBLE) | (1L << DYNAMIC) | (1L << EQUALS) | (1L << FALSE) | (1L << FLOAT) | (1L << FROM) | (1L << GET) | (1L << GROUP) | (1L << INT) | (1L << INTO) | (1L << JOIN) | (1L << LET) | (1L << LONG) | (1L << NAMEOF))) != 0) || ((((_la - 65)) & ~0x3f) == 0 && ((1L << (_la - 65)) & ((1L << (NEW - 65)) | (1L << (NULL - 65)) | (1L << (OBJECT - 65)) | (1L << (ON - 65)) | (1L << (ORDERBY - 65)) | (1L << (PARTIAL - 65)) | (1L << (REMOVE - 65)) | (1L << (SBYTE - 65)) | (1L << (SELECT - 65)) | (1L << (SET - 65)) | (1L << (SHORT - 65)) | (1L << (SIZEOF - 65)) | (1L << (STRING - 65)) | (1L << (THIS - 65)) | (1L << (TRUE - 65)) | (1L << (TYPEOF - 65)) | (1L << (UINT - 65)) | (1L << (ULONG - 65)) | (1L << (UNCHECKED - 65)) | (1L << (USHORT - 65)) | (1L << (WHEN - 65)) | (1L << (WHERE - 65)) | (1L << (YIELD - 65)) | (1L << (IDENTIFIER - 65)) | (1L << (LITERAL_ACCESS - 65)) | (1L << (INTEGER_LITERAL - 65)) | (1L << (HEX_INTEGER_LITERAL - 65)) | (1L << (REAL_LITERAL - 65)) | (1L << (CHARACTER_LITERAL - 65)) | (1L << (REGULAR_STRING - 65)) | (1L << (VERBATIUM_STRING - 65)) | (1L << (INTERPOLATED_REGULAR_STRING_START - 65)) | (1L << (INTERPOLATED_VERBATIUM_STRING_START - 65)) | (1L << (OPEN_PARENS - 65)))) != 0)) {
				{
				setState(885);
				member_declarator_list();
				setState(887);
				_errHandler.sync(this);
				_la = _input.LA(1);
				if (_la==COMMA) {
					{
					setState(886);
					match(COMMA);
					}
				}

				}
			}

			setState(891);
			match(CLOSE_BRACE);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Member_declarator_listContext extends ParserRuleContext {
		public List<Member_declaratorContext> member_declarator() {
			return getRuleContexts(Member_declaratorContext.class);
		}
		public Member_declaratorContext member_declarator(int i) {
			return getRuleContext(Member_declaratorContext.class,i);
		}
		public Member_declarator_listContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_member_declarator_list; }
	}

	public final Member_declarator_listContext member_declarator_list() throws RecognitionException {
		Member_declarator_listContext _localctx = new Member_declarator_listContext(_ctx, getState());
		enterRule(_localctx, 88, RULE_member_declarator_list);
		try {
			int _alt;
			enterOuterAlt(_localctx, 1);
			{
			setState(893);
			member_declarator();
			setState(898);
			_errHandler.sync(this);
			_alt = getInterpreter().adaptivePredict(_input,72,_ctx);
			while ( _alt!=2 && _alt!=org.antlr.v4.runtime.atn.ATN.INVALID_ALT_NUMBER ) {
				if ( _alt==1 ) {
					{
					{
					setState(894);
					match(COMMA);
					setState(895);
					member_declarator();
					}
					} 
				}
				setState(900);
				_errHandler.sync(this);
				_alt = getInterpreter().adaptivePredict(_input,72,_ctx);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Member_declaratorContext extends ParserRuleContext {
		public Primary_expressionContext primary_expression() {
			return getRuleContext(Primary_expressionContext.class,0);
		}
		public IdentifierContext identifier() {
			return getRuleContext(IdentifierContext.class,0);
		}
		public ExpressionContext expression() {
			return getRuleContext(ExpressionContext.class,0);
		}
		public Member_declaratorContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_member_declarator; }
	}

	public final Member_declaratorContext member_declarator() throws RecognitionException {
		Member_declaratorContext _localctx = new Member_declaratorContext(_ctx, getState());
		enterRule(_localctx, 90, RULE_member_declarator);
		try {
			setState(906);
			_errHandler.sync(this);
			switch ( getInterpreter().adaptivePredict(_input,73,_ctx) ) {
			case 1:
				enterOuterAlt(_localctx, 1);
				{
				setState(901);
				primary_expression();
				}
				break;
			case 2:
				enterOuterAlt(_localctx, 2);
				{
				setState(902);
				identifier();
				setState(903);
				match(ASSIGNMENT);
				setState(904);
				expression();
				}
				break;
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Unbound_type_nameContext extends ParserRuleContext {
		public List<IdentifierContext> identifier() {
			return getRuleContexts(IdentifierContext.class);
		}
		public IdentifierContext identifier(int i) {
			return getRuleContext(IdentifierContext.class,i);
		}
		public List<Generic_dimension_specifierContext> generic_dimension_specifier() {
			return getRuleContexts(Generic_dimension_specifierContext.class);
		}
		public Generic_dimension_specifierContext generic_dimension_specifier(int i) {
			return getRuleContext(Generic_dimension_specifierContext.class,i);
		}
		public Unbound_type_nameContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_unbound_type_name; }
	}

	public final Unbound_type_nameContext unbound_type_name() throws RecognitionException {
		Unbound_type_nameContext _localctx = new Unbound_type_nameContext(_ctx, getState());
		enterRule(_localctx, 92, RULE_unbound_type_name);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(908);
			identifier();
			setState(917);
			_errHandler.sync(this);
			switch (_input.LA(1)) {
			case CLOSE_PARENS:
			case DOT:
			case LT:
				{
				setState(910);
				_errHandler.sync(this);
				_la = _input.LA(1);
				if (_la==LT) {
					{
					setState(909);
					generic_dimension_specifier();
					}
				}

				}
				break;
			case DOUBLE_COLON:
				{
				setState(912);
				match(DOUBLE_COLON);
				setState(913);
				identifier();
				setState(915);
				_errHandler.sync(this);
				_la = _input.LA(1);
				if (_la==LT) {
					{
					setState(914);
					generic_dimension_specifier();
					}
				}

				}
				break;
			default:
				throw new NoViableAltException(this);
			}
			setState(926);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while (_la==DOT) {
				{
				{
				setState(919);
				match(DOT);
				setState(920);
				identifier();
				setState(922);
				_errHandler.sync(this);
				_la = _input.LA(1);
				if (_la==LT) {
					{
					setState(921);
					generic_dimension_specifier();
					}
				}

				}
				}
				setState(928);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Generic_dimension_specifierContext extends ParserRuleContext {
		public Generic_dimension_specifierContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_generic_dimension_specifier; }
	}

	public final Generic_dimension_specifierContext generic_dimension_specifier() throws RecognitionException {
		Generic_dimension_specifierContext _localctx = new Generic_dimension_specifierContext(_ctx, getState());
		enterRule(_localctx, 94, RULE_generic_dimension_specifier);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(929);
			match(LT);
			setState(933);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while (_la==COMMA) {
				{
				{
				setState(930);
				match(COMMA);
				}
				}
				setState(935);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			setState(936);
			match(GT);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class IsTypeContext extends ParserRuleContext {
		public Base_typeContext base_type() {
			return getRuleContext(Base_typeContext.class,0);
		}
		public List<Rank_specifierContext> rank_specifier() {
			return getRuleContexts(Rank_specifierContext.class);
		}
		public Rank_specifierContext rank_specifier(int i) {
			return getRuleContext(Rank_specifierContext.class,i);
		}
		public IsTypeContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_isType; }
	}

	public final IsTypeContext isType() throws RecognitionException {
		IsTypeContext _localctx = new IsTypeContext(_ctx, getState());
		enterRule(_localctx, 96, RULE_isType);
		try {
			int _alt;
			enterOuterAlt(_localctx, 1);
			{
			setState(938);
			base_type();
			setState(943);
			_errHandler.sync(this);
			_alt = getInterpreter().adaptivePredict(_input,81,_ctx);
			while ( _alt!=2 && _alt!=org.antlr.v4.runtime.atn.ATN.INVALID_ALT_NUMBER ) {
				if ( _alt==1 ) {
					{
					setState(941);
					_errHandler.sync(this);
					switch (_input.LA(1)) {
					case OPEN_BRACKET:
						{
						setState(939);
						rank_specifier();
						}
						break;
					case STAR:
						{
						setState(940);
						match(STAR);
						}
						break;
					default:
						throw new NoViableAltException(this);
					}
					} 
				}
				setState(945);
				_errHandler.sync(this);
				_alt = getInterpreter().adaptivePredict(_input,81,_ctx);
			}
			setState(947);
			_errHandler.sync(this);
			switch ( getInterpreter().adaptivePredict(_input,82,_ctx) ) {
			case 1:
				{
				setState(946);
				match(INTERR);
				}
				break;
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Lambda_expressionContext extends ParserRuleContext {
		public Anonymous_function_signatureContext anonymous_function_signature() {
			return getRuleContext(Anonymous_function_signatureContext.class,0);
		}
		public Right_arrowContext right_arrow() {
			return getRuleContext(Right_arrowContext.class,0);
		}
		public Anonymous_function_bodyContext anonymous_function_body() {
			return getRuleContext(Anonymous_function_bodyContext.class,0);
		}
		public TerminalNode ASYNC() { return getToken(CSharpParser.ASYNC, 0); }
		public Lambda_expressionContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_lambda_expression; }
	}

	public final Lambda_expressionContext lambda_expression() throws RecognitionException {
		Lambda_expressionContext _localctx = new Lambda_expressionContext(_ctx, getState());
		enterRule(_localctx, 98, RULE_lambda_expression);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(950);
			_errHandler.sync(this);
			switch ( getInterpreter().adaptivePredict(_input,83,_ctx) ) {
			case 1:
				{
				setState(949);
				match(ASYNC);
				}
				break;
			}
			setState(952);
			anonymous_function_signature();
			setState(953);
			right_arrow();
			setState(954);
			anonymous_function_body();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Anonymous_function_signatureContext extends ParserRuleContext {
		public TerminalNode OPEN_PARENS() { return getToken(CSharpParser.OPEN_PARENS, 0); }
		public TerminalNode CLOSE_PARENS() { return getToken(CSharpParser.CLOSE_PARENS, 0); }
		public Explicit_anonymous_function_parameter_listContext explicit_anonymous_function_parameter_list() {
			return getRuleContext(Explicit_anonymous_function_parameter_listContext.class,0);
		}
		public Implicit_anonymous_function_parameter_listContext implicit_anonymous_function_parameter_list() {
			return getRuleContext(Implicit_anonymous_function_parameter_listContext.class,0);
		}
		public IdentifierContext identifier() {
			return getRuleContext(IdentifierContext.class,0);
		}
		public Anonymous_function_signatureContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_anonymous_function_signature; }
	}

	public final Anonymous_function_signatureContext anonymous_function_signature() throws RecognitionException {
		Anonymous_function_signatureContext _localctx = new Anonymous_function_signatureContext(_ctx, getState());
		enterRule(_localctx, 100, RULE_anonymous_function_signature);
		try {
			setState(967);
			_errHandler.sync(this);
			switch ( getInterpreter().adaptivePredict(_input,84,_ctx) ) {
			case 1:
				enterOuterAlt(_localctx, 1);
				{
				setState(956);
				match(OPEN_PARENS);
				setState(957);
				match(CLOSE_PARENS);
				}
				break;
			case 2:
				enterOuterAlt(_localctx, 2);
				{
				setState(958);
				match(OPEN_PARENS);
				setState(959);
				explicit_anonymous_function_parameter_list();
				setState(960);
				match(CLOSE_PARENS);
				}
				break;
			case 3:
				enterOuterAlt(_localctx, 3);
				{
				setState(962);
				match(OPEN_PARENS);
				setState(963);
				implicit_anonymous_function_parameter_list();
				setState(964);
				match(CLOSE_PARENS);
				}
				break;
			case 4:
				enterOuterAlt(_localctx, 4);
				{
				setState(966);
				identifier();
				}
				break;
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Explicit_anonymous_function_parameter_listContext extends ParserRuleContext {
		public List<Explicit_anonymous_function_parameterContext> explicit_anonymous_function_parameter() {
			return getRuleContexts(Explicit_anonymous_function_parameterContext.class);
		}
		public Explicit_anonymous_function_parameterContext explicit_anonymous_function_parameter(int i) {
			return getRuleContext(Explicit_anonymous_function_parameterContext.class,i);
		}
		public Explicit_anonymous_function_parameter_listContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_explicit_anonymous_function_parameter_list; }
	}

	public final Explicit_anonymous_function_parameter_listContext explicit_anonymous_function_parameter_list() throws RecognitionException {
		Explicit_anonymous_function_parameter_listContext _localctx = new Explicit_anonymous_function_parameter_listContext(_ctx, getState());
		enterRule(_localctx, 102, RULE_explicit_anonymous_function_parameter_list);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(969);
			explicit_anonymous_function_parameter();
			setState(974);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while (_la==COMMA) {
				{
				{
				setState(970);
				match(COMMA);
				setState(971);
				explicit_anonymous_function_parameter();
				}
				}
				setState(976);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Explicit_anonymous_function_parameterContext extends ParserRuleContext {
		public Token refout;
		public TypeContext type() {
			return getRuleContext(TypeContext.class,0);
		}
		public IdentifierContext identifier() {
			return getRuleContext(IdentifierContext.class,0);
		}
		public TerminalNode REF() { return getToken(CSharpParser.REF, 0); }
		public TerminalNode OUT() { return getToken(CSharpParser.OUT, 0); }
		public Explicit_anonymous_function_parameterContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_explicit_anonymous_function_parameter; }
	}

	public final Explicit_anonymous_function_parameterContext explicit_anonymous_function_parameter() throws RecognitionException {
		Explicit_anonymous_function_parameterContext _localctx = new Explicit_anonymous_function_parameterContext(_ctx, getState());
		enterRule(_localctx, 104, RULE_explicit_anonymous_function_parameter);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(978);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==OUT || _la==REF) {
				{
				setState(977);
				((Explicit_anonymous_function_parameterContext)_localctx).refout = _input.LT(1);
				_la = _input.LA(1);
				if ( !(_la==OUT || _la==REF) ) {
					((Explicit_anonymous_function_parameterContext)_localctx).refout = (Token)_errHandler.recoverInline(this);
				}
				else {
					if ( _input.LA(1)==Token.EOF ) matchedEOF = true;
					_errHandler.reportMatch(this);
					consume();
				}
				}
			}

			setState(980);
			type();
			setState(981);
			identifier();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Implicit_anonymous_function_parameter_listContext extends ParserRuleContext {
		public List<IdentifierContext> identifier() {
			return getRuleContexts(IdentifierContext.class);
		}
		public IdentifierContext identifier(int i) {
			return getRuleContext(IdentifierContext.class,i);
		}
		public Implicit_anonymous_function_parameter_listContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_implicit_anonymous_function_parameter_list; }
	}

	public final Implicit_anonymous_function_parameter_listContext implicit_anonymous_function_parameter_list() throws RecognitionException {
		Implicit_anonymous_function_parameter_listContext _localctx = new Implicit_anonymous_function_parameter_listContext(_ctx, getState());
		enterRule(_localctx, 106, RULE_implicit_anonymous_function_parameter_list);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(983);
			identifier();
			setState(988);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while (_la==COMMA) {
				{
				{
				setState(984);
				match(COMMA);
				setState(985);
				identifier();
				}
				}
				setState(990);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Anonymous_function_bodyContext extends ParserRuleContext {
		public ExpressionContext expression() {
			return getRuleContext(ExpressionContext.class,0);
		}
		public BlockContext block() {
			return getRuleContext(BlockContext.class,0);
		}
		public Anonymous_function_bodyContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_anonymous_function_body; }
	}

	public final Anonymous_function_bodyContext anonymous_function_body() throws RecognitionException {
		Anonymous_function_bodyContext _localctx = new Anonymous_function_bodyContext(_ctx, getState());
		enterRule(_localctx, 108, RULE_anonymous_function_body);
		try {
			setState(993);
			_errHandler.sync(this);
			switch (_input.LA(1)) {
			case ADD:
			case ALIAS:
			case ARGLIST:
			case ASCENDING:
			case ASYNC:
			case AWAIT:
			case BASE:
			case BOOL:
			case BY:
			case BYTE:
			case CHAR:
			case CHECKED:
			case DECIMAL:
			case DEFAULT:
			case DELEGATE:
			case DESCENDING:
			case DOUBLE:
			case DYNAMIC:
			case EQUALS:
			case FALSE:
			case FLOAT:
			case FROM:
			case GET:
			case GROUP:
			case INT:
			case INTO:
			case JOIN:
			case LET:
			case LONG:
			case NAMEOF:
			case NEW:
			case NULL:
			case OBJECT:
			case ON:
			case ORDERBY:
			case PARTIAL:
			case REMOVE:
			case SBYTE:
			case SELECT:
			case SET:
			case SHORT:
			case SIZEOF:
			case STRING:
			case THIS:
			case TRUE:
			case TYPEOF:
			case UINT:
			case ULONG:
			case UNCHECKED:
			case USHORT:
			case WHEN:
			case WHERE:
			case YIELD:
			case IDENTIFIER:
			case LITERAL_ACCESS:
			case INTEGER_LITERAL:
			case HEX_INTEGER_LITERAL:
			case REAL_LITERAL:
			case CHARACTER_LITERAL:
			case REGULAR_STRING:
			case VERBATIUM_STRING:
			case INTERPOLATED_REGULAR_STRING_START:
			case INTERPOLATED_VERBATIUM_STRING_START:
			case OPEN_PARENS:
			case PLUS:
			case MINUS:
			case STAR:
			case AMP:
			case BANG:
			case TILDE:
			case OP_INC:
			case OP_DEC:
				enterOuterAlt(_localctx, 1);
				{
				setState(991);
				expression();
				}
				break;
			case OPEN_BRACE:
				enterOuterAlt(_localctx, 2);
				{
				setState(992);
				block();
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Query_expressionContext extends ParserRuleContext {
		public From_clauseContext from_clause() {
			return getRuleContext(From_clauseContext.class,0);
		}
		public Query_bodyContext query_body() {
			return getRuleContext(Query_bodyContext.class,0);
		}
		public Query_expressionContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_query_expression; }
	}

	public final Query_expressionContext query_expression() throws RecognitionException {
		Query_expressionContext _localctx = new Query_expressionContext(_ctx, getState());
		enterRule(_localctx, 110, RULE_query_expression);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(995);
			from_clause();
			setState(996);
			query_body();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class From_clauseContext extends ParserRuleContext {
		public TerminalNode FROM() { return getToken(CSharpParser.FROM, 0); }
		public IdentifierContext identifier() {
			return getRuleContext(IdentifierContext.class,0);
		}
		public TerminalNode IN() { return getToken(CSharpParser.IN, 0); }
		public ExpressionContext expression() {
			return getRuleContext(ExpressionContext.class,0);
		}
		public TypeContext type() {
			return getRuleContext(TypeContext.class,0);
		}
		public From_clauseContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_from_clause; }
	}

	public final From_clauseContext from_clause() throws RecognitionException {
		From_clauseContext _localctx = new From_clauseContext(_ctx, getState());
		enterRule(_localctx, 112, RULE_from_clause);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(998);
			match(FROM);
			setState(1000);
			_errHandler.sync(this);
			switch ( getInterpreter().adaptivePredict(_input,89,_ctx) ) {
			case 1:
				{
				setState(999);
				type();
				}
				break;
			}
			setState(1002);
			identifier();
			setState(1003);
			match(IN);
			setState(1004);
			expression();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Query_bodyContext extends ParserRuleContext {
		public Select_or_group_clauseContext select_or_group_clause() {
			return getRuleContext(Select_or_group_clauseContext.class,0);
		}
		public List<Query_body_clauseContext> query_body_clause() {
			return getRuleContexts(Query_body_clauseContext.class);
		}
		public Query_body_clauseContext query_body_clause(int i) {
			return getRuleContext(Query_body_clauseContext.class,i);
		}
		public Query_continuationContext query_continuation() {
			return getRuleContext(Query_continuationContext.class,0);
		}
		public Query_bodyContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_query_body; }
	}

	public final Query_bodyContext query_body() throws RecognitionException {
		Query_bodyContext _localctx = new Query_bodyContext(_ctx, getState());
		enterRule(_localctx, 114, RULE_query_body);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(1009);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while (((((_la - 47)) & ~0x3f) == 0 && ((1L << (_la - 47)) & ((1L << (FROM - 47)) | (1L << (JOIN - 47)) | (1L << (LET - 47)) | (1L << (ORDERBY - 47)) | (1L << (WHERE - 47)))) != 0)) {
				{
				{
				setState(1006);
				query_body_clause();
				}
				}
				setState(1011);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			setState(1012);
			select_or_group_clause();
			setState(1014);
			_errHandler.sync(this);
			switch ( getInterpreter().adaptivePredict(_input,91,_ctx) ) {
			case 1:
				{
				setState(1013);
				query_continuation();
				}
				break;
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Query_body_clauseContext extends ParserRuleContext {
		public From_clauseContext from_clause() {
			return getRuleContext(From_clauseContext.class,0);
		}
		public Let_clauseContext let_clause() {
			return getRuleContext(Let_clauseContext.class,0);
		}
		public Where_clauseContext where_clause() {
			return getRuleContext(Where_clauseContext.class,0);
		}
		public Combined_join_clauseContext combined_join_clause() {
			return getRuleContext(Combined_join_clauseContext.class,0);
		}
		public Orderby_clauseContext orderby_clause() {
			return getRuleContext(Orderby_clauseContext.class,0);
		}
		public Query_body_clauseContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_query_body_clause; }
	}

	public final Query_body_clauseContext query_body_clause() throws RecognitionException {
		Query_body_clauseContext _localctx = new Query_body_clauseContext(_ctx, getState());
		enterRule(_localctx, 116, RULE_query_body_clause);
		try {
			setState(1021);
			_errHandler.sync(this);
			switch (_input.LA(1)) {
			case FROM:
				enterOuterAlt(_localctx, 1);
				{
				setState(1016);
				from_clause();
				}
				break;
			case LET:
				enterOuterAlt(_localctx, 2);
				{
				setState(1017);
				let_clause();
				}
				break;
			case WHERE:
				enterOuterAlt(_localctx, 3);
				{
				setState(1018);
				where_clause();
				}
				break;
			case JOIN:
				enterOuterAlt(_localctx, 4);
				{
				setState(1019);
				combined_join_clause();
				}
				break;
			case ORDERBY:
				enterOuterAlt(_localctx, 5);
				{
				setState(1020);
				orderby_clause();
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Let_clauseContext extends ParserRuleContext {
		public TerminalNode LET() { return getToken(CSharpParser.LET, 0); }
		public IdentifierContext identifier() {
			return getRuleContext(IdentifierContext.class,0);
		}
		public ExpressionContext expression() {
			return getRuleContext(ExpressionContext.class,0);
		}
		public Let_clauseContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_let_clause; }
	}

	public final Let_clauseContext let_clause() throws RecognitionException {
		Let_clauseContext _localctx = new Let_clauseContext(_ctx, getState());
		enterRule(_localctx, 118, RULE_let_clause);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(1023);
			match(LET);
			setState(1024);
			identifier();
			setState(1025);
			match(ASSIGNMENT);
			setState(1026);
			expression();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Where_clauseContext extends ParserRuleContext {
		public TerminalNode WHERE() { return getToken(CSharpParser.WHERE, 0); }
		public ExpressionContext expression() {
			return getRuleContext(ExpressionContext.class,0);
		}
		public Where_clauseContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_where_clause; }
	}

	public final Where_clauseContext where_clause() throws RecognitionException {
		Where_clauseContext _localctx = new Where_clauseContext(_ctx, getState());
		enterRule(_localctx, 120, RULE_where_clause);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(1028);
			match(WHERE);
			setState(1029);
			expression();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Combined_join_clauseContext extends ParserRuleContext {
		public TerminalNode JOIN() { return getToken(CSharpParser.JOIN, 0); }
		public List<IdentifierContext> identifier() {
			return getRuleContexts(IdentifierContext.class);
		}
		public IdentifierContext identifier(int i) {
			return getRuleContext(IdentifierContext.class,i);
		}
		public TerminalNode IN() { return getToken(CSharpParser.IN, 0); }
		public List<ExpressionContext> expression() {
			return getRuleContexts(ExpressionContext.class);
		}
		public ExpressionContext expression(int i) {
			return getRuleContext(ExpressionContext.class,i);
		}
		public TerminalNode ON() { return getToken(CSharpParser.ON, 0); }
		public TerminalNode EQUALS() { return getToken(CSharpParser.EQUALS, 0); }
		public TypeContext type() {
			return getRuleContext(TypeContext.class,0);
		}
		public TerminalNode INTO() { return getToken(CSharpParser.INTO, 0); }
		public Combined_join_clauseContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_combined_join_clause; }
	}

	public final Combined_join_clauseContext combined_join_clause() throws RecognitionException {
		Combined_join_clauseContext _localctx = new Combined_join_clauseContext(_ctx, getState());
		enterRule(_localctx, 122, RULE_combined_join_clause);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(1031);
			match(JOIN);
			setState(1033);
			_errHandler.sync(this);
			switch ( getInterpreter().adaptivePredict(_input,93,_ctx) ) {
			case 1:
				{
				setState(1032);
				type();
				}
				break;
			}
			setState(1035);
			identifier();
			setState(1036);
			match(IN);
			setState(1037);
			expression();
			setState(1038);
			match(ON);
			setState(1039);
			expression();
			setState(1040);
			match(EQUALS);
			setState(1041);
			expression();
			setState(1044);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==INTO) {
				{
				setState(1042);
				match(INTO);
				setState(1043);
				identifier();
				}
			}

			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Orderby_clauseContext extends ParserRuleContext {
		public TerminalNode ORDERBY() { return getToken(CSharpParser.ORDERBY, 0); }
		public List<OrderingContext> ordering() {
			return getRuleContexts(OrderingContext.class);
		}
		public OrderingContext ordering(int i) {
			return getRuleContext(OrderingContext.class,i);
		}
		public Orderby_clauseContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_orderby_clause; }
	}

	public final Orderby_clauseContext orderby_clause() throws RecognitionException {
		Orderby_clauseContext _localctx = new Orderby_clauseContext(_ctx, getState());
		enterRule(_localctx, 124, RULE_orderby_clause);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(1046);
			match(ORDERBY);
			setState(1047);
			ordering();
			setState(1052);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while (_la==COMMA) {
				{
				{
				setState(1048);
				match(COMMA);
				setState(1049);
				ordering();
				}
				}
				setState(1054);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class OrderingContext extends ParserRuleContext {
		public Token dir;
		public ExpressionContext expression() {
			return getRuleContext(ExpressionContext.class,0);
		}
		public TerminalNode ASCENDING() { return getToken(CSharpParser.ASCENDING, 0); }
		public TerminalNode DESCENDING() { return getToken(CSharpParser.DESCENDING, 0); }
		public OrderingContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_ordering; }
	}

	public final OrderingContext ordering() throws RecognitionException {
		OrderingContext _localctx = new OrderingContext(_ctx, getState());
		enterRule(_localctx, 126, RULE_ordering);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(1055);
			expression();
			setState(1057);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==ASCENDING || _la==DESCENDING) {
				{
				setState(1056);
				((OrderingContext)_localctx).dir = _input.LT(1);
				_la = _input.LA(1);
				if ( !(_la==ASCENDING || _la==DESCENDING) ) {
					((OrderingContext)_localctx).dir = (Token)_errHandler.recoverInline(this);
				}
				else {
					if ( _input.LA(1)==Token.EOF ) matchedEOF = true;
					_errHandler.reportMatch(this);
					consume();
				}
				}
			}

			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Select_or_group_clauseContext extends ParserRuleContext {
		public TerminalNode SELECT() { return getToken(CSharpParser.SELECT, 0); }
		public List<ExpressionContext> expression() {
			return getRuleContexts(ExpressionContext.class);
		}
		public ExpressionContext expression(int i) {
			return getRuleContext(ExpressionContext.class,i);
		}
		public TerminalNode GROUP() { return getToken(CSharpParser.GROUP, 0); }
		public TerminalNode BY() { return getToken(CSharpParser.BY, 0); }
		public Select_or_group_clauseContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_select_or_group_clause; }
	}

	public final Select_or_group_clauseContext select_or_group_clause() throws RecognitionException {
		Select_or_group_clauseContext _localctx = new Select_or_group_clauseContext(_ctx, getState());
		enterRule(_localctx, 128, RULE_select_or_group_clause);
		try {
			setState(1066);
			_errHandler.sync(this);
			switch (_input.LA(1)) {
			case SELECT:
				enterOuterAlt(_localctx, 1);
				{
				setState(1059);
				match(SELECT);
				setState(1060);
				expression();
				}
				break;
			case GROUP:
				enterOuterAlt(_localctx, 2);
				{
				setState(1061);
				match(GROUP);
				setState(1062);
				expression();
				setState(1063);
				match(BY);
				setState(1064);
				expression();
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Query_continuationContext extends ParserRuleContext {
		public TerminalNode INTO() { return getToken(CSharpParser.INTO, 0); }
		public IdentifierContext identifier() {
			return getRuleContext(IdentifierContext.class,0);
		}
		public Query_bodyContext query_body() {
			return getRuleContext(Query_bodyContext.class,0);
		}
		public Query_continuationContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_query_continuation; }
	}

	public final Query_continuationContext query_continuation() throws RecognitionException {
		Query_continuationContext _localctx = new Query_continuationContext(_ctx, getState());
		enterRule(_localctx, 130, RULE_query_continuation);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(1068);
			match(INTO);
			setState(1069);
			identifier();
			setState(1070);
			query_body();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class StatementContext extends ParserRuleContext {
		public StatementContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_statement; }
	 
		public StatementContext() { }
		public void copyFrom(StatementContext ctx) {
			super.copyFrom(ctx);
		}
	}
	public static class DeclarationStatementContext extends StatementContext {
		public Local_variable_declarationContext local_variable_declaration() {
			return getRuleContext(Local_variable_declarationContext.class,0);
		}
		public Local_constant_declarationContext local_constant_declaration() {
			return getRuleContext(Local_constant_declarationContext.class,0);
		}
		public DeclarationStatementContext(StatementContext ctx) { copyFrom(ctx); }
	}
	public static class EmbeddedStatementContext extends StatementContext {
		public Embedded_statementContext embedded_statement() {
			return getRuleContext(Embedded_statementContext.class,0);
		}
		public EmbeddedStatementContext(StatementContext ctx) { copyFrom(ctx); }
	}
	public static class LabeledStatementContext extends StatementContext {
		public IdentifierContext identifier() {
			return getRuleContext(IdentifierContext.class,0);
		}
		public StatementContext statement() {
			return getRuleContext(StatementContext.class,0);
		}
		public LabeledStatementContext(StatementContext ctx) { copyFrom(ctx); }
	}

	public final StatementContext statement() throws RecognitionException {
		StatementContext _localctx = new StatementContext(_ctx, getState());
		enterRule(_localctx, 132, RULE_statement);
		try {
			setState(1083);
			_errHandler.sync(this);
			switch ( getInterpreter().adaptivePredict(_input,99,_ctx) ) {
			case 1:
				_localctx = new LabeledStatementContext(_localctx);
				enterOuterAlt(_localctx, 1);
				{
				setState(1072);
				identifier();
				setState(1073);
				match(COLON);
				setState(1074);
				statement();
				}
				break;
			case 2:
				_localctx = new DeclarationStatementContext(_localctx);
				enterOuterAlt(_localctx, 2);
				{
				setState(1078);
				_errHandler.sync(this);
				switch (_input.LA(1)) {
				case ADD:
				case ALIAS:
				case ARGLIST:
				case ASCENDING:
				case ASYNC:
				case AWAIT:
				case BOOL:
				case BY:
				case BYTE:
				case CHAR:
				case DECIMAL:
				case DESCENDING:
				case DOUBLE:
				case DYNAMIC:
				case EQUALS:
				case FLOAT:
				case FROM:
				case GET:
				case GROUP:
				case INT:
				case INTO:
				case JOIN:
				case LET:
				case LONG:
				case NAMEOF:
				case OBJECT:
				case ON:
				case ORDERBY:
				case PARTIAL:
				case REMOVE:
				case SBYTE:
				case SELECT:
				case SET:
				case SHORT:
				case STRING:
				case UINT:
				case ULONG:
				case USHORT:
				case VOID:
				case WHEN:
				case WHERE:
				case YIELD:
				case IDENTIFIER:
					{
					setState(1076);
					local_variable_declaration();
					}
					break;
				case CONST:
					{
					setState(1077);
					local_constant_declaration();
					}
					break;
				default:
					throw new NoViableAltException(this);
				}
				setState(1080);
				match(SEMICOLON);
				}
				break;
			case 3:
				_localctx = new EmbeddedStatementContext(_localctx);
				enterOuterAlt(_localctx, 3);
				{
				setState(1082);
				embedded_statement();
				}
				break;
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Embedded_statementContext extends ParserRuleContext {
		public BlockContext block() {
			return getRuleContext(BlockContext.class,0);
		}
		public Simple_embedded_statementContext simple_embedded_statement() {
			return getRuleContext(Simple_embedded_statementContext.class,0);
		}
		public Embedded_statementContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_embedded_statement; }
	}

	public final Embedded_statementContext embedded_statement() throws RecognitionException {
		Embedded_statementContext _localctx = new Embedded_statementContext(_ctx, getState());
		enterRule(_localctx, 134, RULE_embedded_statement);
		try {
			setState(1087);
			_errHandler.sync(this);
			switch (_input.LA(1)) {
			case OPEN_BRACE:
				enterOuterAlt(_localctx, 1);
				{
				setState(1085);
				block();
				}
				break;
			case ADD:
			case ALIAS:
			case ARGLIST:
			case ASCENDING:
			case ASYNC:
			case AWAIT:
			case BASE:
			case BOOL:
			case BREAK:
			case BY:
			case BYTE:
			case CHAR:
			case CHECKED:
			case CONTINUE:
			case DECIMAL:
			case DEFAULT:
			case DELEGATE:
			case DESCENDING:
			case DO:
			case DOUBLE:
			case DYNAMIC:
			case EQUALS:
			case FALSE:
			case FIXED:
			case FLOAT:
			case FOR:
			case FOREACH:
			case FROM:
			case GET:
			case GOTO:
			case GROUP:
			case IF:
			case INT:
			case INTO:
			case JOIN:
			case LET:
			case LOCK:
			case LONG:
			case NAMEOF:
			case NEW:
			case NULL:
			case OBJECT:
			case ON:
			case ORDERBY:
			case PARTIAL:
			case REMOVE:
			case RETURN:
			case SBYTE:
			case SELECT:
			case SET:
			case SHORT:
			case SIZEOF:
			case STRING:
			case SWITCH:
			case THIS:
			case THROW:
			case TRUE:
			case TRY:
			case TYPEOF:
			case UINT:
			case ULONG:
			case UNCHECKED:
			case UNSAFE:
			case USHORT:
			case USING:
			case WHEN:
			case WHERE:
			case WHILE:
			case YIELD:
			case IDENTIFIER:
			case LITERAL_ACCESS:
			case INTEGER_LITERAL:
			case HEX_INTEGER_LITERAL:
			case REAL_LITERAL:
			case CHARACTER_LITERAL:
			case REGULAR_STRING:
			case VERBATIUM_STRING:
			case INTERPOLATED_REGULAR_STRING_START:
			case INTERPOLATED_VERBATIUM_STRING_START:
			case OPEN_PARENS:
			case SEMICOLON:
			case PLUS:
			case MINUS:
			case STAR:
			case AMP:
			case BANG:
			case TILDE:
			case OP_INC:
			case OP_DEC:
				enterOuterAlt(_localctx, 2);
				{
				setState(1086);
				simple_embedded_statement();
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Simple_embedded_statementContext extends ParserRuleContext {
		public Simple_embedded_statementContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_simple_embedded_statement; }
	 
		public Simple_embedded_statementContext() { }
		public void copyFrom(Simple_embedded_statementContext ctx) {
			super.copyFrom(ctx);
		}
	}
	public static class EmptyStatementContext extends Simple_embedded_statementContext {
		public EmptyStatementContext(Simple_embedded_statementContext ctx) { copyFrom(ctx); }
	}
	public static class TryStatementContext extends Simple_embedded_statementContext {
		public TerminalNode TRY() { return getToken(CSharpParser.TRY, 0); }
		public BlockContext block() {
			return getRuleContext(BlockContext.class,0);
		}
		public Catch_clausesContext catch_clauses() {
			return getRuleContext(Catch_clausesContext.class,0);
		}
		public Finally_clauseContext finally_clause() {
			return getRuleContext(Finally_clauseContext.class,0);
		}
		public TryStatementContext(Simple_embedded_statementContext ctx) { copyFrom(ctx); }
	}
	public static class CheckedStatementContext extends Simple_embedded_statementContext {
		public TerminalNode CHECKED() { return getToken(CSharpParser.CHECKED, 0); }
		public BlockContext block() {
			return getRuleContext(BlockContext.class,0);
		}
		public CheckedStatementContext(Simple_embedded_statementContext ctx) { copyFrom(ctx); }
	}
	public static class ThrowStatementContext extends Simple_embedded_statementContext {
		public TerminalNode THROW() { return getToken(CSharpParser.THROW, 0); }
		public ExpressionContext expression() {
			return getRuleContext(ExpressionContext.class,0);
		}
		public ThrowStatementContext(Simple_embedded_statementContext ctx) { copyFrom(ctx); }
	}
	public static class ForeschStatementContext extends Simple_embedded_statementContext {
		public TerminalNode FOREACH() { return getToken(CSharpParser.FOREACH, 0); }
		public TerminalNode OPEN_PARENS() { return getToken(CSharpParser.OPEN_PARENS, 0); }
		public TypeContext type() {
			return getRuleContext(TypeContext.class,0);
		}
		public IdentifierContext identifier() {
			return getRuleContext(IdentifierContext.class,0);
		}
		public TerminalNode IN() { return getToken(CSharpParser.IN, 0); }
		public ExpressionContext expression() {
			return getRuleContext(ExpressionContext.class,0);
		}
		public TerminalNode CLOSE_PARENS() { return getToken(CSharpParser.CLOSE_PARENS, 0); }
		public Embedded_statementContext embedded_statement() {
			return getRuleContext(Embedded_statementContext.class,0);
		}
		public ForeschStatementContext(Simple_embedded_statementContext ctx) { copyFrom(ctx); }
	}
	public static class UnsafeStatementContext extends Simple_embedded_statementContext {
		public TerminalNode UNSAFE() { return getToken(CSharpParser.UNSAFE, 0); }
		public BlockContext block() {
			return getRuleContext(BlockContext.class,0);
		}
		public UnsafeStatementContext(Simple_embedded_statementContext ctx) { copyFrom(ctx); }
	}
	public static class ForStatementContext extends Simple_embedded_statementContext {
		public TerminalNode FOR() { return getToken(CSharpParser.FOR, 0); }
		public TerminalNode OPEN_PARENS() { return getToken(CSharpParser.OPEN_PARENS, 0); }
		public TerminalNode CLOSE_PARENS() { return getToken(CSharpParser.CLOSE_PARENS, 0); }
		public Embedded_statementContext embedded_statement() {
			return getRuleContext(Embedded_statementContext.class,0);
		}
		public For_initializerContext for_initializer() {
			return getRuleContext(For_initializerContext.class,0);
		}
		public ExpressionContext expression() {
			return getRuleContext(ExpressionContext.class,0);
		}
		public For_iteratorContext for_iterator() {
			return getRuleContext(For_iteratorContext.class,0);
		}
		public ForStatementContext(Simple_embedded_statementContext ctx) { copyFrom(ctx); }
	}
	public static class BreakStatementContext extends Simple_embedded_statementContext {
		public TerminalNode BREAK() { return getToken(CSharpParser.BREAK, 0); }
		public BreakStatementContext(Simple_embedded_statementContext ctx) { copyFrom(ctx); }
	}
	public static class IfStatementContext extends Simple_embedded_statementContext {
		public TerminalNode IF() { return getToken(CSharpParser.IF, 0); }
		public TerminalNode OPEN_PARENS() { return getToken(CSharpParser.OPEN_PARENS, 0); }
		public ExpressionContext expression() {
			return getRuleContext(ExpressionContext.class,0);
		}
		public TerminalNode CLOSE_PARENS() { return getToken(CSharpParser.CLOSE_PARENS, 0); }
		public List<If_bodyContext> if_body() {
			return getRuleContexts(If_bodyContext.class);
		}
		public If_bodyContext if_body(int i) {
			return getRuleContext(If_bodyContext.class,i);
		}
		public TerminalNode ELSE() { return getToken(CSharpParser.ELSE, 0); }
		public IfStatementContext(Simple_embedded_statementContext ctx) { copyFrom(ctx); }
	}
	public static class ReturnStatementContext extends Simple_embedded_statementContext {
		public TerminalNode RETURN() { return getToken(CSharpParser.RETURN, 0); }
		public ExpressionContext expression() {
			return getRuleContext(ExpressionContext.class,0);
		}
		public ReturnStatementContext(Simple_embedded_statementContext ctx) { copyFrom(ctx); }
	}
	public static class GotoStatementContext extends Simple_embedded_statementContext {
		public TerminalNode GOTO() { return getToken(CSharpParser.GOTO, 0); }
		public IdentifierContext identifier() {
			return getRuleContext(IdentifierContext.class,0);
		}
		public TerminalNode CASE() { return getToken(CSharpParser.CASE, 0); }
		public ExpressionContext expression() {
			return getRuleContext(ExpressionContext.class,0);
		}
		public TerminalNode DEFAULT() { return getToken(CSharpParser.DEFAULT, 0); }
		public GotoStatementContext(Simple_embedded_statementContext ctx) { copyFrom(ctx); }
	}
	public static class SwitchStatementContext extends Simple_embedded_statementContext {
		public TerminalNode SWITCH() { return getToken(CSharpParser.SWITCH, 0); }
		public TerminalNode OPEN_PARENS() { return getToken(CSharpParser.OPEN_PARENS, 0); }
		public ExpressionContext expression() {
			return getRuleContext(ExpressionContext.class,0);
		}
		public TerminalNode CLOSE_PARENS() { return getToken(CSharpParser.CLOSE_PARENS, 0); }
		public TerminalNode OPEN_BRACE() { return getToken(CSharpParser.OPEN_BRACE, 0); }
		public TerminalNode CLOSE_BRACE() { return getToken(CSharpParser.CLOSE_BRACE, 0); }
		public List<Switch_sectionContext> switch_section() {
			return getRuleContexts(Switch_sectionContext.class);
		}
		public Switch_sectionContext switch_section(int i) {
			return getRuleContext(Switch_sectionContext.class,i);
		}
		public SwitchStatementContext(Simple_embedded_statementContext ctx) { copyFrom(ctx); }
	}
	public static class FixedStatementContext extends Simple_embedded_statementContext {
		public TerminalNode FIXED() { return getToken(CSharpParser.FIXED, 0); }
		public TerminalNode OPEN_PARENS() { return getToken(CSharpParser.OPEN_PARENS, 0); }
		public Pointer_typeContext pointer_type() {
			return getRuleContext(Pointer_typeContext.class,0);
		}
		public Fixed_pointer_declaratorsContext fixed_pointer_declarators() {
			return getRuleContext(Fixed_pointer_declaratorsContext.class,0);
		}
		public TerminalNode CLOSE_PARENS() { return getToken(CSharpParser.CLOSE_PARENS, 0); }
		public Embedded_statementContext embedded_statement() {
			return getRuleContext(Embedded_statementContext.class,0);
		}
		public FixedStatementContext(Simple_embedded_statementContext ctx) { copyFrom(ctx); }
	}
	public static class WhileStatementContext extends Simple_embedded_statementContext {
		public TerminalNode WHILE() { return getToken(CSharpParser.WHILE, 0); }
		public TerminalNode OPEN_PARENS() { return getToken(CSharpParser.OPEN_PARENS, 0); }
		public ExpressionContext expression() {
			return getRuleContext(ExpressionContext.class,0);
		}
		public TerminalNode CLOSE_PARENS() { return getToken(CSharpParser.CLOSE_PARENS, 0); }
		public Embedded_statementContext embedded_statement() {
			return getRuleContext(Embedded_statementContext.class,0);
		}
		public WhileStatementContext(Simple_embedded_statementContext ctx) { copyFrom(ctx); }
	}
	public static class DoStatementContext extends Simple_embedded_statementContext {
		public TerminalNode DO() { return getToken(CSharpParser.DO, 0); }
		public Embedded_statementContext embedded_statement() {
			return getRuleContext(Embedded_statementContext.class,0);
		}
		public TerminalNode WHILE() { return getToken(CSharpParser.WHILE, 0); }
		public TerminalNode OPEN_PARENS() { return getToken(CSharpParser.OPEN_PARENS, 0); }
		public ExpressionContext expression() {
			return getRuleContext(ExpressionContext.class,0);
		}
		public TerminalNode CLOSE_PARENS() { return getToken(CSharpParser.CLOSE_PARENS, 0); }
		public DoStatementContext(Simple_embedded_statementContext ctx) { copyFrom(ctx); }
	}
	public static class UncheckedStatementContext extends Simple_embedded_statementContext {
		public TerminalNode UNCHECKED() { return getToken(CSharpParser.UNCHECKED, 0); }
		public BlockContext block() {
			return getRuleContext(BlockContext.class,0);
		}
		public UncheckedStatementContext(Simple_embedded_statementContext ctx) { copyFrom(ctx); }
	}
	public static class ExpressionStatementContext extends Simple_embedded_statementContext {
		public ExpressionContext expression() {
			return getRuleContext(ExpressionContext.class,0);
		}
		public ExpressionStatementContext(Simple_embedded_statementContext ctx) { copyFrom(ctx); }
	}
	public static class ContinueStatementContext extends Simple_embedded_statementContext {
		public TerminalNode CONTINUE() { return getToken(CSharpParser.CONTINUE, 0); }
		public ContinueStatementContext(Simple_embedded_statementContext ctx) { copyFrom(ctx); }
	}
	public static class UsingStatementContext extends Simple_embedded_statementContext {
		public TerminalNode USING() { return getToken(CSharpParser.USING, 0); }
		public TerminalNode OPEN_PARENS() { return getToken(CSharpParser.OPEN_PARENS, 0); }
		public Resource_acquisitionContext resource_acquisition() {
			return getRuleContext(Resource_acquisitionContext.class,0);
		}
		public TerminalNode CLOSE_PARENS() { return getToken(CSharpParser.CLOSE_PARENS, 0); }
		public Embedded_statementContext embedded_statement() {
			return getRuleContext(Embedded_statementContext.class,0);
		}
		public UsingStatementContext(Simple_embedded_statementContext ctx) { copyFrom(ctx); }
	}
	public static class LockStatementContext extends Simple_embedded_statementContext {
		public TerminalNode LOCK() { return getToken(CSharpParser.LOCK, 0); }
		public TerminalNode OPEN_PARENS() { return getToken(CSharpParser.OPEN_PARENS, 0); }
		public ExpressionContext expression() {
			return getRuleContext(ExpressionContext.class,0);
		}
		public TerminalNode CLOSE_PARENS() { return getToken(CSharpParser.CLOSE_PARENS, 0); }
		public Embedded_statementContext embedded_statement() {
			return getRuleContext(Embedded_statementContext.class,0);
		}
		public LockStatementContext(Simple_embedded_statementContext ctx) { copyFrom(ctx); }
	}
	public static class YieldStatementContext extends Simple_embedded_statementContext {
		public TerminalNode YIELD() { return getToken(CSharpParser.YIELD, 0); }
		public TerminalNode RETURN() { return getToken(CSharpParser.RETURN, 0); }
		public ExpressionContext expression() {
			return getRuleContext(ExpressionContext.class,0);
		}
		public TerminalNode BREAK() { return getToken(CSharpParser.BREAK, 0); }
		public YieldStatementContext(Simple_embedded_statementContext ctx) { copyFrom(ctx); }
	}

	public final Simple_embedded_statementContext simple_embedded_statement() throws RecognitionException {
		Simple_embedded_statementContext _localctx = new Simple_embedded_statementContext(_ctx, getState());
		enterRule(_localctx, 136, RULE_simple_embedded_statement);
		int _la;
		try {
			setState(1216);
			_errHandler.sync(this);
			switch ( getInterpreter().adaptivePredict(_input,112,_ctx) ) {
			case 1:
				_localctx = new EmptyStatementContext(_localctx);
				enterOuterAlt(_localctx, 1);
				{
				setState(1089);
				match(SEMICOLON);
				}
				break;
			case 2:
				_localctx = new ExpressionStatementContext(_localctx);
				enterOuterAlt(_localctx, 2);
				{
				setState(1090);
				expression();
				setState(1091);
				match(SEMICOLON);
				}
				break;
			case 3:
				_localctx = new IfStatementContext(_localctx);
				enterOuterAlt(_localctx, 3);
				{
				setState(1093);
				match(IF);
				setState(1094);
				match(OPEN_PARENS);
				setState(1095);
				expression();
				setState(1096);
				match(CLOSE_PARENS);
				setState(1097);
				if_body();
				setState(1100);
				_errHandler.sync(this);
				switch ( getInterpreter().adaptivePredict(_input,101,_ctx) ) {
				case 1:
					{
					setState(1098);
					match(ELSE);
					setState(1099);
					if_body();
					}
					break;
				}
				}
				break;
			case 4:
				_localctx = new SwitchStatementContext(_localctx);
				enterOuterAlt(_localctx, 4);
				{
				setState(1102);
				match(SWITCH);
				setState(1103);
				match(OPEN_PARENS);
				setState(1104);
				expression();
				setState(1105);
				match(CLOSE_PARENS);
				setState(1106);
				match(OPEN_BRACE);
				setState(1110);
				_errHandler.sync(this);
				_la = _input.LA(1);
				while (_la==CASE || _la==DEFAULT) {
					{
					{
					setState(1107);
					switch_section();
					}
					}
					setState(1112);
					_errHandler.sync(this);
					_la = _input.LA(1);
				}
				setState(1113);
				match(CLOSE_BRACE);
				}
				break;
			case 5:
				_localctx = new WhileStatementContext(_localctx);
				enterOuterAlt(_localctx, 5);
				{
				setState(1115);
				match(WHILE);
				setState(1116);
				match(OPEN_PARENS);
				setState(1117);
				expression();
				setState(1118);
				match(CLOSE_PARENS);
				setState(1119);
				embedded_statement();
				}
				break;
			case 6:
				_localctx = new DoStatementContext(_localctx);
				enterOuterAlt(_localctx, 6);
				{
				setState(1121);
				match(DO);
				setState(1122);
				embedded_statement();
				setState(1123);
				match(WHILE);
				setState(1124);
				match(OPEN_PARENS);
				setState(1125);
				expression();
				setState(1126);
				match(CLOSE_PARENS);
				setState(1127);
				match(SEMICOLON);
				}
				break;
			case 7:
				_localctx = new ForStatementContext(_localctx);
				enterOuterAlt(_localctx, 7);
				{
				setState(1129);
				match(FOR);
				setState(1130);
				match(OPEN_PARENS);
				setState(1132);
				_errHandler.sync(this);
				_la = _input.LA(1);
				if ((((_la) & ~0x3f) == 0 && ((1L << _la) & ((1L << ADD) | (1L << ALIAS) | (1L << ARGLIST) | (1L << ASCENDING) | (1L << ASYNC) | (1L << AWAIT) | (1L << BASE) | (1L << BOOL) | (1L << BY) | (1L << BYTE) | (1L << CHAR) | (1L << CHECKED) | (1L << DECIMAL) | (1L << DEFAULT) | (1L << DELEGATE) | (1L << DESCENDING) | (1L << DOUBLE) | (1L << DYNAMIC) | (1L << EQUALS) | (1L << FALSE) | (1L << FLOAT) | (1L << FROM) | (1L << GET) | (1L << GROUP) | (1L << INT) | (1L << INTO) | (1L << JOIN) | (1L << LET) | (1L << LONG) | (1L << NAMEOF))) != 0) || ((((_la - 65)) & ~0x3f) == 0 && ((1L << (_la - 65)) & ((1L << (NEW - 65)) | (1L << (NULL - 65)) | (1L << (OBJECT - 65)) | (1L << (ON - 65)) | (1L << (ORDERBY - 65)) | (1L << (PARTIAL - 65)) | (1L << (REMOVE - 65)) | (1L << (SBYTE - 65)) | (1L << (SELECT - 65)) | (1L << (SET - 65)) | (1L << (SHORT - 65)) | (1L << (SIZEOF - 65)) | (1L << (STRING - 65)) | (1L << (THIS - 65)) | (1L << (TRUE - 65)) | (1L << (TYPEOF - 65)) | (1L << (UINT - 65)) | (1L << (ULONG - 65)) | (1L << (UNCHECKED - 65)) | (1L << (USHORT - 65)) | (1L << (VOID - 65)) | (1L << (WHEN - 65)) | (1L << (WHERE - 65)) | (1L << (YIELD - 65)) | (1L << (IDENTIFIER - 65)) | (1L << (LITERAL_ACCESS - 65)) | (1L << (INTEGER_LITERAL - 65)) | (1L << (HEX_INTEGER_LITERAL - 65)) | (1L << (REAL_LITERAL - 65)) | (1L << (CHARACTER_LITERAL - 65)) | (1L << (REGULAR_STRING - 65)) | (1L << (VERBATIUM_STRING - 65)) | (1L << (INTERPOLATED_REGULAR_STRING_START - 65)) | (1L << (INTERPOLATED_VERBATIUM_STRING_START - 65)) | (1L << (OPEN_PARENS - 65)))) != 0) || ((((_la - 131)) & ~0x3f) == 0 && ((1L << (_la - 131)) & ((1L << (PLUS - 131)) | (1L << (MINUS - 131)) | (1L << (STAR - 131)) | (1L << (AMP - 131)) | (1L << (BANG - 131)) | (1L << (TILDE - 131)) | (1L << (OP_INC - 131)) | (1L << (OP_DEC - 131)))) != 0)) {
					{
					setState(1131);
					for_initializer();
					}
				}

				setState(1134);
				match(SEMICOLON);
				setState(1136);
				_errHandler.sync(this);
				_la = _input.LA(1);
				if ((((_la) & ~0x3f) == 0 && ((1L << _la) & ((1L << ADD) | (1L << ALIAS) | (1L << ARGLIST) | (1L << ASCENDING) | (1L << ASYNC) | (1L << AWAIT) | (1L << BASE) | (1L << BOOL) | (1L << BY) | (1L << BYTE) | (1L << CHAR) | (1L << CHECKED) | (1L << DECIMAL) | (1L << DEFAULT) | (1L << DELEGATE) | (1L << DESCENDING) | (1L << DOUBLE) | (1L << DYNAMIC) | (1L << EQUALS) | (1L << FALSE) | (1L << FLOAT) | (1L << FROM) | (1L << GET) | (1L << GROUP) | (1L << INT) | (1L << INTO) | (1L << JOIN) | (1L << LET) | (1L << LONG) | (1L << NAMEOF))) != 0) || ((((_la - 65)) & ~0x3f) == 0 && ((1L << (_la - 65)) & ((1L << (NEW - 65)) | (1L << (NULL - 65)) | (1L << (OBJECT - 65)) | (1L << (ON - 65)) | (1L << (ORDERBY - 65)) | (1L << (PARTIAL - 65)) | (1L << (REMOVE - 65)) | (1L << (SBYTE - 65)) | (1L << (SELECT - 65)) | (1L << (SET - 65)) | (1L << (SHORT - 65)) | (1L << (SIZEOF - 65)) | (1L << (STRING - 65)) | (1L << (THIS - 65)) | (1L << (TRUE - 65)) | (1L << (TYPEOF - 65)) | (1L << (UINT - 65)) | (1L << (ULONG - 65)) | (1L << (UNCHECKED - 65)) | (1L << (USHORT - 65)) | (1L << (WHEN - 65)) | (1L << (WHERE - 65)) | (1L << (YIELD - 65)) | (1L << (IDENTIFIER - 65)) | (1L << (LITERAL_ACCESS - 65)) | (1L << (INTEGER_LITERAL - 65)) | (1L << (HEX_INTEGER_LITERAL - 65)) | (1L << (REAL_LITERAL - 65)) | (1L << (CHARACTER_LITERAL - 65)) | (1L << (REGULAR_STRING - 65)) | (1L << (VERBATIUM_STRING - 65)) | (1L << (INTERPOLATED_REGULAR_STRING_START - 65)) | (1L << (INTERPOLATED_VERBATIUM_STRING_START - 65)) | (1L << (OPEN_PARENS - 65)))) != 0) || ((((_la - 131)) & ~0x3f) == 0 && ((1L << (_la - 131)) & ((1L << (PLUS - 131)) | (1L << (MINUS - 131)) | (1L << (STAR - 131)) | (1L << (AMP - 131)) | (1L << (BANG - 131)) | (1L << (TILDE - 131)) | (1L << (OP_INC - 131)) | (1L << (OP_DEC - 131)))) != 0)) {
					{
					setState(1135);
					expression();
					}
				}

				setState(1138);
				match(SEMICOLON);
				setState(1140);
				_errHandler.sync(this);
				_la = _input.LA(1);
				if ((((_la) & ~0x3f) == 0 && ((1L << _la) & ((1L << ADD) | (1L << ALIAS) | (1L << ARGLIST) | (1L << ASCENDING) | (1L << ASYNC) | (1L << AWAIT) | (1L << BASE) | (1L << BOOL) | (1L << BY) | (1L << BYTE) | (1L << CHAR) | (1L << CHECKED) | (1L << DECIMAL) | (1L << DEFAULT) | (1L << DELEGATE) | (1L << DESCENDING) | (1L << DOUBLE) | (1L << DYNAMIC) | (1L << EQUALS) | (1L << FALSE) | (1L << FLOAT) | (1L << FROM) | (1L << GET) | (1L << GROUP) | (1L << INT) | (1L << INTO) | (1L << JOIN) | (1L << LET) | (1L << LONG) | (1L << NAMEOF))) != 0) || ((((_la - 65)) & ~0x3f) == 0 && ((1L << (_la - 65)) & ((1L << (NEW - 65)) | (1L << (NULL - 65)) | (1L << (OBJECT - 65)) | (1L << (ON - 65)) | (1L << (ORDERBY - 65)) | (1L << (PARTIAL - 65)) | (1L << (REMOVE - 65)) | (1L << (SBYTE - 65)) | (1L << (SELECT - 65)) | (1L << (SET - 65)) | (1L << (SHORT - 65)) | (1L << (SIZEOF - 65)) | (1L << (STRING - 65)) | (1L << (THIS - 65)) | (1L << (TRUE - 65)) | (1L << (TYPEOF - 65)) | (1L << (UINT - 65)) | (1L << (ULONG - 65)) | (1L << (UNCHECKED - 65)) | (1L << (USHORT - 65)) | (1L << (WHEN - 65)) | (1L << (WHERE - 65)) | (1L << (YIELD - 65)) | (1L << (IDENTIFIER - 65)) | (1L << (LITERAL_ACCESS - 65)) | (1L << (INTEGER_LITERAL - 65)) | (1L << (HEX_INTEGER_LITERAL - 65)) | (1L << (REAL_LITERAL - 65)) | (1L << (CHARACTER_LITERAL - 65)) | (1L << (REGULAR_STRING - 65)) | (1L << (VERBATIUM_STRING - 65)) | (1L << (INTERPOLATED_REGULAR_STRING_START - 65)) | (1L << (INTERPOLATED_VERBATIUM_STRING_START - 65)) | (1L << (OPEN_PARENS - 65)))) != 0) || ((((_la - 131)) & ~0x3f) == 0 && ((1L << (_la - 131)) & ((1L << (PLUS - 131)) | (1L << (MINUS - 131)) | (1L << (STAR - 131)) | (1L << (AMP - 131)) | (1L << (BANG - 131)) | (1L << (TILDE - 131)) | (1L << (OP_INC - 131)) | (1L << (OP_DEC - 131)))) != 0)) {
					{
					setState(1139);
					for_iterator();
					}
				}

				setState(1142);
				match(CLOSE_PARENS);
				setState(1143);
				embedded_statement();
				}
				break;
			case 8:
				_localctx = new ForeschStatementContext(_localctx);
				enterOuterAlt(_localctx, 8);
				{
				setState(1144);
				match(FOREACH);
				setState(1145);
				match(OPEN_PARENS);
				setState(1146);
				type();
				setState(1147);
				identifier();
				setState(1148);
				match(IN);
				setState(1149);
				expression();
				setState(1150);
				match(CLOSE_PARENS);
				setState(1151);
				embedded_statement();
				}
				break;
			case 9:
				_localctx = new BreakStatementContext(_localctx);
				enterOuterAlt(_localctx, 9);
				{
				setState(1153);
				match(BREAK);
				setState(1154);
				match(SEMICOLON);
				}
				break;
			case 10:
				_localctx = new ContinueStatementContext(_localctx);
				enterOuterAlt(_localctx, 10);
				{
				setState(1155);
				match(CONTINUE);
				setState(1156);
				match(SEMICOLON);
				}
				break;
			case 11:
				_localctx = new GotoStatementContext(_localctx);
				enterOuterAlt(_localctx, 11);
				{
				setState(1157);
				match(GOTO);
				setState(1162);
				_errHandler.sync(this);
				switch (_input.LA(1)) {
				case ADD:
				case ALIAS:
				case ARGLIST:
				case ASCENDING:
				case ASYNC:
				case AWAIT:
				case BY:
				case DESCENDING:
				case DYNAMIC:
				case EQUALS:
				case FROM:
				case GET:
				case GROUP:
				case INTO:
				case JOIN:
				case LET:
				case NAMEOF:
				case ON:
				case ORDERBY:
				case PARTIAL:
				case REMOVE:
				case SELECT:
				case SET:
				case WHEN:
				case WHERE:
				case YIELD:
				case IDENTIFIER:
					{
					setState(1158);
					identifier();
					}
					break;
				case CASE:
					{
					setState(1159);
					match(CASE);
					setState(1160);
					expression();
					}
					break;
				case DEFAULT:
					{
					setState(1161);
					match(DEFAULT);
					}
					break;
				default:
					throw new NoViableAltException(this);
				}
				setState(1164);
				match(SEMICOLON);
				}
				break;
			case 12:
				_localctx = new ReturnStatementContext(_localctx);
				enterOuterAlt(_localctx, 12);
				{
				setState(1165);
				match(RETURN);
				setState(1167);
				_errHandler.sync(this);
				_la = _input.LA(1);
				if ((((_la) & ~0x3f) == 0 && ((1L << _la) & ((1L << ADD) | (1L << ALIAS) | (1L << ARGLIST) | (1L << ASCENDING) | (1L << ASYNC) | (1L << AWAIT) | (1L << BASE) | (1L << BOOL) | (1L << BY) | (1L << BYTE) | (1L << CHAR) | (1L << CHECKED) | (1L << DECIMAL) | (1L << DEFAULT) | (1L << DELEGATE) | (1L << DESCENDING) | (1L << DOUBLE) | (1L << DYNAMIC) | (1L << EQUALS) | (1L << FALSE) | (1L << FLOAT) | (1L << FROM) | (1L << GET) | (1L << GROUP) | (1L << INT) | (1L << INTO) | (1L << JOIN) | (1L << LET) | (1L << LONG) | (1L << NAMEOF))) != 0) || ((((_la - 65)) & ~0x3f) == 0 && ((1L << (_la - 65)) & ((1L << (NEW - 65)) | (1L << (NULL - 65)) | (1L << (OBJECT - 65)) | (1L << (ON - 65)) | (1L << (ORDERBY - 65)) | (1L << (PARTIAL - 65)) | (1L << (REMOVE - 65)) | (1L << (SBYTE - 65)) | (1L << (SELECT - 65)) | (1L << (SET - 65)) | (1L << (SHORT - 65)) | (1L << (SIZEOF - 65)) | (1L << (STRING - 65)) | (1L << (THIS - 65)) | (1L << (TRUE - 65)) | (1L << (TYPEOF - 65)) | (1L << (UINT - 65)) | (1L << (ULONG - 65)) | (1L << (UNCHECKED - 65)) | (1L << (USHORT - 65)) | (1L << (WHEN - 65)) | (1L << (WHERE - 65)) | (1L << (YIELD - 65)) | (1L << (IDENTIFIER - 65)) | (1L << (LITERAL_ACCESS - 65)) | (1L << (INTEGER_LITERAL - 65)) | (1L << (HEX_INTEGER_LITERAL - 65)) | (1L << (REAL_LITERAL - 65)) | (1L << (CHARACTER_LITERAL - 65)) | (1L << (REGULAR_STRING - 65)) | (1L << (VERBATIUM_STRING - 65)) | (1L << (INTERPOLATED_REGULAR_STRING_START - 65)) | (1L << (INTERPOLATED_VERBATIUM_STRING_START - 65)) | (1L << (OPEN_PARENS - 65)))) != 0) || ((((_la - 131)) & ~0x3f) == 0 && ((1L << (_la - 131)) & ((1L << (PLUS - 131)) | (1L << (MINUS - 131)) | (1L << (STAR - 131)) | (1L << (AMP - 131)) | (1L << (BANG - 131)) | (1L << (TILDE - 131)) | (1L << (OP_INC - 131)) | (1L << (OP_DEC - 131)))) != 0)) {
					{
					setState(1166);
					expression();
					}
				}

				setState(1169);
				match(SEMICOLON);
				}
				break;
			case 13:
				_localctx = new ThrowStatementContext(_localctx);
				enterOuterAlt(_localctx, 13);
				{
				setState(1170);
				match(THROW);
				setState(1172);
				_errHandler.sync(this);
				_la = _input.LA(1);
				if ((((_la) & ~0x3f) == 0 && ((1L << _la) & ((1L << ADD) | (1L << ALIAS) | (1L << ARGLIST) | (1L << ASCENDING) | (1L << ASYNC) | (1L << AWAIT) | (1L << BASE) | (1L << BOOL) | (1L << BY) | (1L << BYTE) | (1L << CHAR) | (1L << CHECKED) | (1L << DECIMAL) | (1L << DEFAULT) | (1L << DELEGATE) | (1L << DESCENDING) | (1L << DOUBLE) | (1L << DYNAMIC) | (1L << EQUALS) | (1L << FALSE) | (1L << FLOAT) | (1L << FROM) | (1L << GET) | (1L << GROUP) | (1L << INT) | (1L << INTO) | (1L << JOIN) | (1L << LET) | (1L << LONG) | (1L << NAMEOF))) != 0) || ((((_la - 65)) & ~0x3f) == 0 && ((1L << (_la - 65)) & ((1L << (NEW - 65)) | (1L << (NULL - 65)) | (1L << (OBJECT - 65)) | (1L << (ON - 65)) | (1L << (ORDERBY - 65)) | (1L << (PARTIAL - 65)) | (1L << (REMOVE - 65)) | (1L << (SBYTE - 65)) | (1L << (SELECT - 65)) | (1L << (SET - 65)) | (1L << (SHORT - 65)) | (1L << (SIZEOF - 65)) | (1L << (STRING - 65)) | (1L << (THIS - 65)) | (1L << (TRUE - 65)) | (1L << (TYPEOF - 65)) | (1L << (UINT - 65)) | (1L << (ULONG - 65)) | (1L << (UNCHECKED - 65)) | (1L << (USHORT - 65)) | (1L << (WHEN - 65)) | (1L << (WHERE - 65)) | (1L << (YIELD - 65)) | (1L << (IDENTIFIER - 65)) | (1L << (LITERAL_ACCESS - 65)) | (1L << (INTEGER_LITERAL - 65)) | (1L << (HEX_INTEGER_LITERAL - 65)) | (1L << (REAL_LITERAL - 65)) | (1L << (CHARACTER_LITERAL - 65)) | (1L << (REGULAR_STRING - 65)) | (1L << (VERBATIUM_STRING - 65)) | (1L << (INTERPOLATED_REGULAR_STRING_START - 65)) | (1L << (INTERPOLATED_VERBATIUM_STRING_START - 65)) | (1L << (OPEN_PARENS - 65)))) != 0) || ((((_la - 131)) & ~0x3f) == 0 && ((1L << (_la - 131)) & ((1L << (PLUS - 131)) | (1L << (MINUS - 131)) | (1L << (STAR - 131)) | (1L << (AMP - 131)) | (1L << (BANG - 131)) | (1L << (TILDE - 131)) | (1L << (OP_INC - 131)) | (1L << (OP_DEC - 131)))) != 0)) {
					{
					setState(1171);
					expression();
					}
				}

				setState(1174);
				match(SEMICOLON);
				}
				break;
			case 14:
				_localctx = new TryStatementContext(_localctx);
				enterOuterAlt(_localctx, 14);
				{
				setState(1175);
				match(TRY);
				setState(1176);
				block();
				setState(1182);
				_errHandler.sync(this);
				switch (_input.LA(1)) {
				case CATCH:
					{
					setState(1177);
					catch_clauses();
					setState(1179);
					_errHandler.sync(this);
					_la = _input.LA(1);
					if (_la==FINALLY) {
						{
						setState(1178);
						finally_clause();
						}
					}

					}
					break;
				case FINALLY:
					{
					setState(1181);
					finally_clause();
					}
					break;
				default:
					throw new NoViableAltException(this);
				}
				}
				break;
			case 15:
				_localctx = new CheckedStatementContext(_localctx);
				enterOuterAlt(_localctx, 15);
				{
				setState(1184);
				match(CHECKED);
				setState(1185);
				block();
				}
				break;
			case 16:
				_localctx = new UncheckedStatementContext(_localctx);
				enterOuterAlt(_localctx, 16);
				{
				setState(1186);
				match(UNCHECKED);
				setState(1187);
				block();
				}
				break;
			case 17:
				_localctx = new LockStatementContext(_localctx);
				enterOuterAlt(_localctx, 17);
				{
				setState(1188);
				match(LOCK);
				setState(1189);
				match(OPEN_PARENS);
				setState(1190);
				expression();
				setState(1191);
				match(CLOSE_PARENS);
				setState(1192);
				embedded_statement();
				}
				break;
			case 18:
				_localctx = new UsingStatementContext(_localctx);
				enterOuterAlt(_localctx, 18);
				{
				setState(1194);
				match(USING);
				setState(1195);
				match(OPEN_PARENS);
				setState(1196);
				resource_acquisition();
				setState(1197);
				match(CLOSE_PARENS);
				setState(1198);
				embedded_statement();
				}
				break;
			case 19:
				_localctx = new YieldStatementContext(_localctx);
				enterOuterAlt(_localctx, 19);
				{
				setState(1200);
				match(YIELD);
				setState(1204);
				_errHandler.sync(this);
				switch (_input.LA(1)) {
				case RETURN:
					{
					setState(1201);
					match(RETURN);
					setState(1202);
					expression();
					}
					break;
				case BREAK:
					{
					setState(1203);
					match(BREAK);
					}
					break;
				default:
					throw new NoViableAltException(this);
				}
				setState(1206);
				match(SEMICOLON);
				}
				break;
			case 20:
				_localctx = new UnsafeStatementContext(_localctx);
				enterOuterAlt(_localctx, 20);
				{
				setState(1207);
				match(UNSAFE);
				setState(1208);
				block();
				}
				break;
			case 21:
				_localctx = new FixedStatementContext(_localctx);
				enterOuterAlt(_localctx, 21);
				{
				setState(1209);
				match(FIXED);
				setState(1210);
				match(OPEN_PARENS);
				setState(1211);
				pointer_type();
				setState(1212);
				fixed_pointer_declarators();
				setState(1213);
				match(CLOSE_PARENS);
				setState(1214);
				embedded_statement();
				}
				break;
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class BlockContext extends ParserRuleContext {
		public TerminalNode OPEN_BRACE() { return getToken(CSharpParser.OPEN_BRACE, 0); }
		public TerminalNode CLOSE_BRACE() { return getToken(CSharpParser.CLOSE_BRACE, 0); }
		public Statement_listContext statement_list() {
			return getRuleContext(Statement_listContext.class,0);
		}
		public BlockContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_block; }
	}

	public final BlockContext block() throws RecognitionException {
		BlockContext _localctx = new BlockContext(_ctx, getState());
		enterRule(_localctx, 138, RULE_block);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(1218);
			match(OPEN_BRACE);
			setState(1220);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if ((((_la) & ~0x3f) == 0 && ((1L << _la) & ((1L << ADD) | (1L << ALIAS) | (1L << ARGLIST) | (1L << ASCENDING) | (1L << ASYNC) | (1L << AWAIT) | (1L << BASE) | (1L << BOOL) | (1L << BREAK) | (1L << BY) | (1L << BYTE) | (1L << CHAR) | (1L << CHECKED) | (1L << CONST) | (1L << CONTINUE) | (1L << DECIMAL) | (1L << DEFAULT) | (1L << DELEGATE) | (1L << DESCENDING) | (1L << DO) | (1L << DOUBLE) | (1L << DYNAMIC) | (1L << EQUALS) | (1L << FALSE) | (1L << FIXED) | (1L << FLOAT) | (1L << FOR) | (1L << FOREACH) | (1L << FROM) | (1L << GET) | (1L << GOTO) | (1L << GROUP) | (1L << IF) | (1L << INT) | (1L << INTO) | (1L << JOIN) | (1L << LET) | (1L << LOCK) | (1L << LONG) | (1L << NAMEOF))) != 0) || ((((_la - 65)) & ~0x3f) == 0 && ((1L << (_la - 65)) & ((1L << (NEW - 65)) | (1L << (NULL - 65)) | (1L << (OBJECT - 65)) | (1L << (ON - 65)) | (1L << (ORDERBY - 65)) | (1L << (PARTIAL - 65)) | (1L << (REMOVE - 65)) | (1L << (RETURN - 65)) | (1L << (SBYTE - 65)) | (1L << (SELECT - 65)) | (1L << (SET - 65)) | (1L << (SHORT - 65)) | (1L << (SIZEOF - 65)) | (1L << (STRING - 65)) | (1L << (SWITCH - 65)) | (1L << (THIS - 65)) | (1L << (THROW - 65)) | (1L << (TRUE - 65)) | (1L << (TRY - 65)) | (1L << (TYPEOF - 65)) | (1L << (UINT - 65)) | (1L << (ULONG - 65)) | (1L << (UNCHECKED - 65)) | (1L << (UNSAFE - 65)) | (1L << (USHORT - 65)) | (1L << (USING - 65)) | (1L << (VOID - 65)) | (1L << (WHEN - 65)) | (1L << (WHERE - 65)) | (1L << (WHILE - 65)) | (1L << (YIELD - 65)) | (1L << (IDENTIFIER - 65)) | (1L << (LITERAL_ACCESS - 65)) | (1L << (INTEGER_LITERAL - 65)) | (1L << (HEX_INTEGER_LITERAL - 65)) | (1L << (REAL_LITERAL - 65)) | (1L << (CHARACTER_LITERAL - 65)) | (1L << (REGULAR_STRING - 65)) | (1L << (VERBATIUM_STRING - 65)) | (1L << (INTERPOLATED_REGULAR_STRING_START - 65)) | (1L << (INTERPOLATED_VERBATIUM_STRING_START - 65)) | (1L << (OPEN_BRACE - 65)) | (1L << (OPEN_PARENS - 65)))) != 0) || ((((_la - 130)) & ~0x3f) == 0 && ((1L << (_la - 130)) & ((1L << (SEMICOLON - 130)) | (1L << (PLUS - 130)) | (1L << (MINUS - 130)) | (1L << (STAR - 130)) | (1L << (AMP - 130)) | (1L << (BANG - 130)) | (1L << (TILDE - 130)) | (1L << (OP_INC - 130)) | (1L << (OP_DEC - 130)))) != 0)) {
				{
				setState(1219);
				statement_list();
				}
			}

			setState(1222);
			match(CLOSE_BRACE);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Local_variable_declarationContext extends ParserRuleContext {
		public TypeContext type() {
			return getRuleContext(TypeContext.class,0);
		}
		public List<Local_variable_declaratorContext> local_variable_declarator() {
			return getRuleContexts(Local_variable_declaratorContext.class);
		}
		public Local_variable_declaratorContext local_variable_declarator(int i) {
			return getRuleContext(Local_variable_declaratorContext.class,i);
		}
		public Local_variable_declarationContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_local_variable_declaration; }
	}

	public final Local_variable_declarationContext local_variable_declaration() throws RecognitionException {
		Local_variable_declarationContext _localctx = new Local_variable_declarationContext(_ctx, getState());
		enterRule(_localctx, 140, RULE_local_variable_declaration);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(1224);
			type();
			setState(1225);
			local_variable_declarator();
			setState(1230);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while (_la==COMMA) {
				{
				{
				setState(1226);
				match(COMMA);
				setState(1227);
				local_variable_declarator();
				}
				}
				setState(1232);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Local_variable_declaratorContext extends ParserRuleContext {
		public IdentifierContext identifier() {
			return getRuleContext(IdentifierContext.class,0);
		}
		public Local_variable_initializerContext local_variable_initializer() {
			return getRuleContext(Local_variable_initializerContext.class,0);
		}
		public Local_variable_declaratorContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_local_variable_declarator; }
	}

	public final Local_variable_declaratorContext local_variable_declarator() throws RecognitionException {
		Local_variable_declaratorContext _localctx = new Local_variable_declaratorContext(_ctx, getState());
		enterRule(_localctx, 142, RULE_local_variable_declarator);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(1233);
			identifier();
			setState(1236);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==ASSIGNMENT) {
				{
				setState(1234);
				match(ASSIGNMENT);
				setState(1235);
				local_variable_initializer();
				}
			}

			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Local_variable_initializerContext extends ParserRuleContext {
		public ExpressionContext expression() {
			return getRuleContext(ExpressionContext.class,0);
		}
		public Array_initializerContext array_initializer() {
			return getRuleContext(Array_initializerContext.class,0);
		}
		public Local_variable_initializer_unsafeContext local_variable_initializer_unsafe() {
			return getRuleContext(Local_variable_initializer_unsafeContext.class,0);
		}
		public Local_variable_initializerContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_local_variable_initializer; }
	}

	public final Local_variable_initializerContext local_variable_initializer() throws RecognitionException {
		Local_variable_initializerContext _localctx = new Local_variable_initializerContext(_ctx, getState());
		enterRule(_localctx, 144, RULE_local_variable_initializer);
		try {
			setState(1241);
			_errHandler.sync(this);
			switch (_input.LA(1)) {
			case ADD:
			case ALIAS:
			case ARGLIST:
			case ASCENDING:
			case ASYNC:
			case AWAIT:
			case BASE:
			case BOOL:
			case BY:
			case BYTE:
			case CHAR:
			case CHECKED:
			case DECIMAL:
			case DEFAULT:
			case DELEGATE:
			case DESCENDING:
			case DOUBLE:
			case DYNAMIC:
			case EQUALS:
			case FALSE:
			case FLOAT:
			case FROM:
			case GET:
			case GROUP:
			case INT:
			case INTO:
			case JOIN:
			case LET:
			case LONG:
			case NAMEOF:
			case NEW:
			case NULL:
			case OBJECT:
			case ON:
			case ORDERBY:
			case PARTIAL:
			case REMOVE:
			case SBYTE:
			case SELECT:
			case SET:
			case SHORT:
			case SIZEOF:
			case STRING:
			case THIS:
			case TRUE:
			case TYPEOF:
			case UINT:
			case ULONG:
			case UNCHECKED:
			case USHORT:
			case WHEN:
			case WHERE:
			case YIELD:
			case IDENTIFIER:
			case LITERAL_ACCESS:
			case INTEGER_LITERAL:
			case HEX_INTEGER_LITERAL:
			case REAL_LITERAL:
			case CHARACTER_LITERAL:
			case REGULAR_STRING:
			case VERBATIUM_STRING:
			case INTERPOLATED_REGULAR_STRING_START:
			case INTERPOLATED_VERBATIUM_STRING_START:
			case OPEN_PARENS:
			case PLUS:
			case MINUS:
			case STAR:
			case AMP:
			case BANG:
			case TILDE:
			case OP_INC:
			case OP_DEC:
				enterOuterAlt(_localctx, 1);
				{
				setState(1238);
				expression();
				}
				break;
			case OPEN_BRACE:
				enterOuterAlt(_localctx, 2);
				{
				setState(1239);
				array_initializer();
				}
				break;
			case STACKALLOC:
				enterOuterAlt(_localctx, 3);
				{
				setState(1240);
				local_variable_initializer_unsafe();
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Local_constant_declarationContext extends ParserRuleContext {
		public TerminalNode CONST() { return getToken(CSharpParser.CONST, 0); }
		public TypeContext type() {
			return getRuleContext(TypeContext.class,0);
		}
		public Constant_declaratorsContext constant_declarators() {
			return getRuleContext(Constant_declaratorsContext.class,0);
		}
		public Local_constant_declarationContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_local_constant_declaration; }
	}

	public final Local_constant_declarationContext local_constant_declaration() throws RecognitionException {
		Local_constant_declarationContext _localctx = new Local_constant_declarationContext(_ctx, getState());
		enterRule(_localctx, 146, RULE_local_constant_declaration);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(1243);
			match(CONST);
			setState(1244);
			type();
			setState(1245);
			constant_declarators();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class If_bodyContext extends ParserRuleContext {
		public BlockContext block() {
			return getRuleContext(BlockContext.class,0);
		}
		public Simple_embedded_statementContext simple_embedded_statement() {
			return getRuleContext(Simple_embedded_statementContext.class,0);
		}
		public If_bodyContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_if_body; }
	}

	public final If_bodyContext if_body() throws RecognitionException {
		If_bodyContext _localctx = new If_bodyContext(_ctx, getState());
		enterRule(_localctx, 148, RULE_if_body);
		try {
			setState(1249);
			_errHandler.sync(this);
			switch (_input.LA(1)) {
			case OPEN_BRACE:
				enterOuterAlt(_localctx, 1);
				{
				setState(1247);
				block();
				}
				break;
			case ADD:
			case ALIAS:
			case ARGLIST:
			case ASCENDING:
			case ASYNC:
			case AWAIT:
			case BASE:
			case BOOL:
			case BREAK:
			case BY:
			case BYTE:
			case CHAR:
			case CHECKED:
			case CONTINUE:
			case DECIMAL:
			case DEFAULT:
			case DELEGATE:
			case DESCENDING:
			case DO:
			case DOUBLE:
			case DYNAMIC:
			case EQUALS:
			case FALSE:
			case FIXED:
			case FLOAT:
			case FOR:
			case FOREACH:
			case FROM:
			case GET:
			case GOTO:
			case GROUP:
			case IF:
			case INT:
			case INTO:
			case JOIN:
			case LET:
			case LOCK:
			case LONG:
			case NAMEOF:
			case NEW:
			case NULL:
			case OBJECT:
			case ON:
			case ORDERBY:
			case PARTIAL:
			case REMOVE:
			case RETURN:
			case SBYTE:
			case SELECT:
			case SET:
			case SHORT:
			case SIZEOF:
			case STRING:
			case SWITCH:
			case THIS:
			case THROW:
			case TRUE:
			case TRY:
			case TYPEOF:
			case UINT:
			case ULONG:
			case UNCHECKED:
			case UNSAFE:
			case USHORT:
			case USING:
			case WHEN:
			case WHERE:
			case WHILE:
			case YIELD:
			case IDENTIFIER:
			case LITERAL_ACCESS:
			case INTEGER_LITERAL:
			case HEX_INTEGER_LITERAL:
			case REAL_LITERAL:
			case CHARACTER_LITERAL:
			case REGULAR_STRING:
			case VERBATIUM_STRING:
			case INTERPOLATED_REGULAR_STRING_START:
			case INTERPOLATED_VERBATIUM_STRING_START:
			case OPEN_PARENS:
			case SEMICOLON:
			case PLUS:
			case MINUS:
			case STAR:
			case AMP:
			case BANG:
			case TILDE:
			case OP_INC:
			case OP_DEC:
				enterOuterAlt(_localctx, 2);
				{
				setState(1248);
				simple_embedded_statement();
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Switch_sectionContext extends ParserRuleContext {
		public Statement_listContext statement_list() {
			return getRuleContext(Statement_listContext.class,0);
		}
		public List<Switch_labelContext> switch_label() {
			return getRuleContexts(Switch_labelContext.class);
		}
		public Switch_labelContext switch_label(int i) {
			return getRuleContext(Switch_labelContext.class,i);
		}
		public Switch_sectionContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_switch_section; }
	}

	public final Switch_sectionContext switch_section() throws RecognitionException {
		Switch_sectionContext _localctx = new Switch_sectionContext(_ctx, getState());
		enterRule(_localctx, 150, RULE_switch_section);
		try {
			int _alt;
			enterOuterAlt(_localctx, 1);
			{
			setState(1252); 
			_errHandler.sync(this);
			_alt = 1;
			do {
				switch (_alt) {
				case 1:
					{
					{
					setState(1251);
					switch_label();
					}
					}
					break;
				default:
					throw new NoViableAltException(this);
				}
				setState(1254); 
				_errHandler.sync(this);
				_alt = getInterpreter().adaptivePredict(_input,118,_ctx);
			} while ( _alt!=2 && _alt!=org.antlr.v4.runtime.atn.ATN.INVALID_ALT_NUMBER );
			setState(1256);
			statement_list();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Switch_labelContext extends ParserRuleContext {
		public TerminalNode CASE() { return getToken(CSharpParser.CASE, 0); }
		public ExpressionContext expression() {
			return getRuleContext(ExpressionContext.class,0);
		}
		public TerminalNode DEFAULT() { return getToken(CSharpParser.DEFAULT, 0); }
		public Switch_labelContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_switch_label; }
	}

	public final Switch_labelContext switch_label() throws RecognitionException {
		Switch_labelContext _localctx = new Switch_labelContext(_ctx, getState());
		enterRule(_localctx, 152, RULE_switch_label);
		try {
			setState(1264);
			_errHandler.sync(this);
			switch (_input.LA(1)) {
			case CASE:
				enterOuterAlt(_localctx, 1);
				{
				setState(1258);
				match(CASE);
				setState(1259);
				expression();
				setState(1260);
				match(COLON);
				}
				break;
			case DEFAULT:
				enterOuterAlt(_localctx, 2);
				{
				setState(1262);
				match(DEFAULT);
				setState(1263);
				match(COLON);
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Statement_listContext extends ParserRuleContext {
		public List<StatementContext> statement() {
			return getRuleContexts(StatementContext.class);
		}
		public StatementContext statement(int i) {
			return getRuleContext(StatementContext.class,i);
		}
		public Statement_listContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_statement_list; }
	}

	public final Statement_listContext statement_list() throws RecognitionException {
		Statement_listContext _localctx = new Statement_listContext(_ctx, getState());
		enterRule(_localctx, 154, RULE_statement_list);
		try {
			int _alt;
			enterOuterAlt(_localctx, 1);
			{
			setState(1267); 
			_errHandler.sync(this);
			_alt = 1;
			do {
				switch (_alt) {
				case 1:
					{
					{
					setState(1266);
					statement();
					}
					}
					break;
				default:
					throw new NoViableAltException(this);
				}
				setState(1269); 
				_errHandler.sync(this);
				_alt = getInterpreter().adaptivePredict(_input,120,_ctx);
			} while ( _alt!=2 && _alt!=org.antlr.v4.runtime.atn.ATN.INVALID_ALT_NUMBER );
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class For_initializerContext extends ParserRuleContext {
		public Local_variable_declarationContext local_variable_declaration() {
			return getRuleContext(Local_variable_declarationContext.class,0);
		}
		public List<ExpressionContext> expression() {
			return getRuleContexts(ExpressionContext.class);
		}
		public ExpressionContext expression(int i) {
			return getRuleContext(ExpressionContext.class,i);
		}
		public For_initializerContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_for_initializer; }
	}

	public final For_initializerContext for_initializer() throws RecognitionException {
		For_initializerContext _localctx = new For_initializerContext(_ctx, getState());
		enterRule(_localctx, 156, RULE_for_initializer);
		int _la;
		try {
			setState(1280);
			_errHandler.sync(this);
			switch ( getInterpreter().adaptivePredict(_input,122,_ctx) ) {
			case 1:
				enterOuterAlt(_localctx, 1);
				{
				setState(1271);
				local_variable_declaration();
				}
				break;
			case 2:
				enterOuterAlt(_localctx, 2);
				{
				setState(1272);
				expression();
				setState(1277);
				_errHandler.sync(this);
				_la = _input.LA(1);
				while (_la==COMMA) {
					{
					{
					setState(1273);
					match(COMMA);
					setState(1274);
					expression();
					}
					}
					setState(1279);
					_errHandler.sync(this);
					_la = _input.LA(1);
				}
				}
				break;
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class For_iteratorContext extends ParserRuleContext {
		public List<ExpressionContext> expression() {
			return getRuleContexts(ExpressionContext.class);
		}
		public ExpressionContext expression(int i) {
			return getRuleContext(ExpressionContext.class,i);
		}
		public For_iteratorContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_for_iterator; }
	}

	public final For_iteratorContext for_iterator() throws RecognitionException {
		For_iteratorContext _localctx = new For_iteratorContext(_ctx, getState());
		enterRule(_localctx, 158, RULE_for_iterator);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(1282);
			expression();
			setState(1287);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while (_la==COMMA) {
				{
				{
				setState(1283);
				match(COMMA);
				setState(1284);
				expression();
				}
				}
				setState(1289);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Catch_clausesContext extends ParserRuleContext {
		public List<Specific_catch_clauseContext> specific_catch_clause() {
			return getRuleContexts(Specific_catch_clauseContext.class);
		}
		public Specific_catch_clauseContext specific_catch_clause(int i) {
			return getRuleContext(Specific_catch_clauseContext.class,i);
		}
		public General_catch_clauseContext general_catch_clause() {
			return getRuleContext(General_catch_clauseContext.class,0);
		}
		public Catch_clausesContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_catch_clauses; }
	}

	public final Catch_clausesContext catch_clauses() throws RecognitionException {
		Catch_clausesContext _localctx = new Catch_clausesContext(_ctx, getState());
		enterRule(_localctx, 160, RULE_catch_clauses);
		int _la;
		try {
			int _alt;
			setState(1301);
			_errHandler.sync(this);
			switch ( getInterpreter().adaptivePredict(_input,126,_ctx) ) {
			case 1:
				enterOuterAlt(_localctx, 1);
				{
				setState(1290);
				specific_catch_clause();
				setState(1294);
				_errHandler.sync(this);
				_alt = getInterpreter().adaptivePredict(_input,124,_ctx);
				while ( _alt!=2 && _alt!=org.antlr.v4.runtime.atn.ATN.INVALID_ALT_NUMBER ) {
					if ( _alt==1 ) {
						{
						{
						setState(1291);
						specific_catch_clause();
						}
						} 
					}
					setState(1296);
					_errHandler.sync(this);
					_alt = getInterpreter().adaptivePredict(_input,124,_ctx);
				}
				setState(1298);
				_errHandler.sync(this);
				_la = _input.LA(1);
				if (_la==CATCH) {
					{
					setState(1297);
					general_catch_clause();
					}
				}

				}
				break;
			case 2:
				enterOuterAlt(_localctx, 2);
				{
				setState(1300);
				general_catch_clause();
				}
				break;
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Specific_catch_clauseContext extends ParserRuleContext {
		public TerminalNode CATCH() { return getToken(CSharpParser.CATCH, 0); }
		public TerminalNode OPEN_PARENS() { return getToken(CSharpParser.OPEN_PARENS, 0); }
		public Class_typeContext class_type() {
			return getRuleContext(Class_typeContext.class,0);
		}
		public TerminalNode CLOSE_PARENS() { return getToken(CSharpParser.CLOSE_PARENS, 0); }
		public BlockContext block() {
			return getRuleContext(BlockContext.class,0);
		}
		public IdentifierContext identifier() {
			return getRuleContext(IdentifierContext.class,0);
		}
		public Exception_filterContext exception_filter() {
			return getRuleContext(Exception_filterContext.class,0);
		}
		public Specific_catch_clauseContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_specific_catch_clause; }
	}

	public final Specific_catch_clauseContext specific_catch_clause() throws RecognitionException {
		Specific_catch_clauseContext _localctx = new Specific_catch_clauseContext(_ctx, getState());
		enterRule(_localctx, 162, RULE_specific_catch_clause);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(1303);
			match(CATCH);
			setState(1304);
			match(OPEN_PARENS);
			setState(1305);
			class_type();
			setState(1307);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if ((((_la) & ~0x3f) == 0 && ((1L << _la) & ((1L << ADD) | (1L << ALIAS) | (1L << ARGLIST) | (1L << ASCENDING) | (1L << ASYNC) | (1L << AWAIT) | (1L << BY) | (1L << DESCENDING) | (1L << DYNAMIC) | (1L << EQUALS) | (1L << FROM) | (1L << GET) | (1L << GROUP) | (1L << INTO) | (1L << JOIN) | (1L << LET) | (1L << NAMEOF))) != 0) || ((((_la - 68)) & ~0x3f) == 0 && ((1L << (_la - 68)) & ((1L << (ON - 68)) | (1L << (ORDERBY - 68)) | (1L << (PARTIAL - 68)) | (1L << (REMOVE - 68)) | (1L << (SELECT - 68)) | (1L << (SET - 68)) | (1L << (WHEN - 68)) | (1L << (WHERE - 68)) | (1L << (YIELD - 68)) | (1L << (IDENTIFIER - 68)))) != 0)) {
				{
				setState(1306);
				identifier();
				}
			}

			setState(1309);
			match(CLOSE_PARENS);
			setState(1311);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==WHEN) {
				{
				setState(1310);
				exception_filter();
				}
			}

			setState(1313);
			block();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class General_catch_clauseContext extends ParserRuleContext {
		public TerminalNode CATCH() { return getToken(CSharpParser.CATCH, 0); }
		public BlockContext block() {
			return getRuleContext(BlockContext.class,0);
		}
		public Exception_filterContext exception_filter() {
			return getRuleContext(Exception_filterContext.class,0);
		}
		public General_catch_clauseContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_general_catch_clause; }
	}

	public final General_catch_clauseContext general_catch_clause() throws RecognitionException {
		General_catch_clauseContext _localctx = new General_catch_clauseContext(_ctx, getState());
		enterRule(_localctx, 164, RULE_general_catch_clause);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(1315);
			match(CATCH);
			setState(1317);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==WHEN) {
				{
				setState(1316);
				exception_filter();
				}
			}

			setState(1319);
			block();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Exception_filterContext extends ParserRuleContext {
		public TerminalNode WHEN() { return getToken(CSharpParser.WHEN, 0); }
		public TerminalNode OPEN_PARENS() { return getToken(CSharpParser.OPEN_PARENS, 0); }
		public ExpressionContext expression() {
			return getRuleContext(ExpressionContext.class,0);
		}
		public TerminalNode CLOSE_PARENS() { return getToken(CSharpParser.CLOSE_PARENS, 0); }
		public Exception_filterContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_exception_filter; }
	}

	public final Exception_filterContext exception_filter() throws RecognitionException {
		Exception_filterContext _localctx = new Exception_filterContext(_ctx, getState());
		enterRule(_localctx, 166, RULE_exception_filter);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(1321);
			match(WHEN);
			setState(1322);
			match(OPEN_PARENS);
			setState(1323);
			expression();
			setState(1324);
			match(CLOSE_PARENS);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Finally_clauseContext extends ParserRuleContext {
		public TerminalNode FINALLY() { return getToken(CSharpParser.FINALLY, 0); }
		public BlockContext block() {
			return getRuleContext(BlockContext.class,0);
		}
		public Finally_clauseContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_finally_clause; }
	}

	public final Finally_clauseContext finally_clause() throws RecognitionException {
		Finally_clauseContext _localctx = new Finally_clauseContext(_ctx, getState());
		enterRule(_localctx, 168, RULE_finally_clause);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(1326);
			match(FINALLY);
			setState(1327);
			block();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Resource_acquisitionContext extends ParserRuleContext {
		public Local_variable_declarationContext local_variable_declaration() {
			return getRuleContext(Local_variable_declarationContext.class,0);
		}
		public ExpressionContext expression() {
			return getRuleContext(ExpressionContext.class,0);
		}
		public Resource_acquisitionContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_resource_acquisition; }
	}

	public final Resource_acquisitionContext resource_acquisition() throws RecognitionException {
		Resource_acquisitionContext _localctx = new Resource_acquisitionContext(_ctx, getState());
		enterRule(_localctx, 170, RULE_resource_acquisition);
		try {
			setState(1331);
			_errHandler.sync(this);
			switch ( getInterpreter().adaptivePredict(_input,130,_ctx) ) {
			case 1:
				enterOuterAlt(_localctx, 1);
				{
				setState(1329);
				local_variable_declaration();
				}
				break;
			case 2:
				enterOuterAlt(_localctx, 2);
				{
				setState(1330);
				expression();
				}
				break;
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Namespace_declarationContext extends ParserRuleContext {
		public Qualified_identifierContext qi;
		public TerminalNode NAMESPACE() { return getToken(CSharpParser.NAMESPACE, 0); }
		public Namespace_bodyContext namespace_body() {
			return getRuleContext(Namespace_bodyContext.class,0);
		}
		public Qualified_identifierContext qualified_identifier() {
			return getRuleContext(Qualified_identifierContext.class,0);
		}
		public Namespace_declarationContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_namespace_declaration; }
	}

	public final Namespace_declarationContext namespace_declaration() throws RecognitionException {
		Namespace_declarationContext _localctx = new Namespace_declarationContext(_ctx, getState());
		enterRule(_localctx, 172, RULE_namespace_declaration);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(1333);
			match(NAMESPACE);
			setState(1334);
			((Namespace_declarationContext)_localctx).qi = qualified_identifier();
			setState(1335);
			namespace_body();
			setState(1337);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==SEMICOLON) {
				{
				setState(1336);
				match(SEMICOLON);
				}
			}

			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Qualified_identifierContext extends ParserRuleContext {
		public List<IdentifierContext> identifier() {
			return getRuleContexts(IdentifierContext.class);
		}
		public IdentifierContext identifier(int i) {
			return getRuleContext(IdentifierContext.class,i);
		}
		public Qualified_identifierContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_qualified_identifier; }
	}

	public final Qualified_identifierContext qualified_identifier() throws RecognitionException {
		Qualified_identifierContext _localctx = new Qualified_identifierContext(_ctx, getState());
		enterRule(_localctx, 174, RULE_qualified_identifier);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(1339);
			identifier();
			setState(1344);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while (_la==DOT) {
				{
				{
				setState(1340);
				match(DOT);
				setState(1341);
				identifier();
				}
				}
				setState(1346);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Namespace_bodyContext extends ParserRuleContext {
		public TerminalNode OPEN_BRACE() { return getToken(CSharpParser.OPEN_BRACE, 0); }
		public TerminalNode CLOSE_BRACE() { return getToken(CSharpParser.CLOSE_BRACE, 0); }
		public Extern_alias_directivesContext extern_alias_directives() {
			return getRuleContext(Extern_alias_directivesContext.class,0);
		}
		public Using_directivesContext using_directives() {
			return getRuleContext(Using_directivesContext.class,0);
		}
		public Namespace_member_declarationsContext namespace_member_declarations() {
			return getRuleContext(Namespace_member_declarationsContext.class,0);
		}
		public Namespace_bodyContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_namespace_body; }
	}

	public final Namespace_bodyContext namespace_body() throws RecognitionException {
		Namespace_bodyContext _localctx = new Namespace_bodyContext(_ctx, getState());
		enterRule(_localctx, 176, RULE_namespace_body);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(1347);
			match(OPEN_BRACE);
			setState(1349);
			_errHandler.sync(this);
			switch ( getInterpreter().adaptivePredict(_input,133,_ctx) ) {
			case 1:
				{
				setState(1348);
				extern_alias_directives();
				}
				break;
			}
			setState(1352);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==USING) {
				{
				setState(1351);
				using_directives();
				}
			}

			setState(1355);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if ((((_la) & ~0x3f) == 0 && ((1L << _la) & ((1L << ABSTRACT) | (1L << ASYNC) | (1L << CLASS) | (1L << DELEGATE) | (1L << ENUM) | (1L << EXTERN) | (1L << INTERFACE) | (1L << INTERNAL))) != 0) || ((((_la - 64)) & ~0x3f) == 0 && ((1L << (_la - 64)) & ((1L << (NAMESPACE - 64)) | (1L << (NEW - 64)) | (1L << (OVERRIDE - 64)) | (1L << (PARTIAL - 64)) | (1L << (PRIVATE - 64)) | (1L << (PROTECTED - 64)) | (1L << (PUBLIC - 64)) | (1L << (READONLY - 64)) | (1L << (SEALED - 64)) | (1L << (STATIC - 64)) | (1L << (STRUCT - 64)) | (1L << (UNSAFE - 64)) | (1L << (VIRTUAL - 64)) | (1L << (VOLATILE - 64)) | (1L << (OPEN_BRACKET - 64)))) != 0)) {
				{
				setState(1354);
				namespace_member_declarations();
				}
			}

			setState(1357);
			match(CLOSE_BRACE);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Extern_alias_directivesContext extends ParserRuleContext {
		public List<Extern_alias_directiveContext> extern_alias_directive() {
			return getRuleContexts(Extern_alias_directiveContext.class);
		}
		public Extern_alias_directiveContext extern_alias_directive(int i) {
			return getRuleContext(Extern_alias_directiveContext.class,i);
		}
		public Extern_alias_directivesContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_extern_alias_directives; }
	}

	public final Extern_alias_directivesContext extern_alias_directives() throws RecognitionException {
		Extern_alias_directivesContext _localctx = new Extern_alias_directivesContext(_ctx, getState());
		enterRule(_localctx, 178, RULE_extern_alias_directives);
		try {
			int _alt;
			enterOuterAlt(_localctx, 1);
			{
			setState(1360); 
			_errHandler.sync(this);
			_alt = 1;
			do {
				switch (_alt) {
				case 1:
					{
					{
					setState(1359);
					extern_alias_directive();
					}
					}
					break;
				default:
					throw new NoViableAltException(this);
				}
				setState(1362); 
				_errHandler.sync(this);
				_alt = getInterpreter().adaptivePredict(_input,136,_ctx);
			} while ( _alt!=2 && _alt!=org.antlr.v4.runtime.atn.ATN.INVALID_ALT_NUMBER );
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Extern_alias_directiveContext extends ParserRuleContext {
		public TerminalNode EXTERN() { return getToken(CSharpParser.EXTERN, 0); }
		public TerminalNode ALIAS() { return getToken(CSharpParser.ALIAS, 0); }
		public IdentifierContext identifier() {
			return getRuleContext(IdentifierContext.class,0);
		}
		public Extern_alias_directiveContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_extern_alias_directive; }
	}

	public final Extern_alias_directiveContext extern_alias_directive() throws RecognitionException {
		Extern_alias_directiveContext _localctx = new Extern_alias_directiveContext(_ctx, getState());
		enterRule(_localctx, 180, RULE_extern_alias_directive);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(1364);
			match(EXTERN);
			setState(1365);
			match(ALIAS);
			setState(1366);
			identifier();
			setState(1367);
			match(SEMICOLON);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Using_directivesContext extends ParserRuleContext {
		public List<Using_directiveContext> using_directive() {
			return getRuleContexts(Using_directiveContext.class);
		}
		public Using_directiveContext using_directive(int i) {
			return getRuleContext(Using_directiveContext.class,i);
		}
		public Using_directivesContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_using_directives; }
	}

	public final Using_directivesContext using_directives() throws RecognitionException {
		Using_directivesContext _localctx = new Using_directivesContext(_ctx, getState());
		enterRule(_localctx, 182, RULE_using_directives);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(1370); 
			_errHandler.sync(this);
			_la = _input.LA(1);
			do {
				{
				{
				setState(1369);
				using_directive();
				}
				}
				setState(1372); 
				_errHandler.sync(this);
				_la = _input.LA(1);
			} while ( _la==USING );
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Using_directiveContext extends ParserRuleContext {
		public Using_directiveContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_using_directive; }
	 
		public Using_directiveContext() { }
		public void copyFrom(Using_directiveContext ctx) {
			super.copyFrom(ctx);
		}
	}
	public static class UsingAliasDirectiveContext extends Using_directiveContext {
		public TerminalNode USING() { return getToken(CSharpParser.USING, 0); }
		public IdentifierContext identifier() {
			return getRuleContext(IdentifierContext.class,0);
		}
		public Namespace_or_type_nameContext namespace_or_type_name() {
			return getRuleContext(Namespace_or_type_nameContext.class,0);
		}
		public UsingAliasDirectiveContext(Using_directiveContext ctx) { copyFrom(ctx); }
	}
	public static class UsingNamespaceDirectiveContext extends Using_directiveContext {
		public TerminalNode USING() { return getToken(CSharpParser.USING, 0); }
		public Namespace_or_type_nameContext namespace_or_type_name() {
			return getRuleContext(Namespace_or_type_nameContext.class,0);
		}
		public UsingNamespaceDirectiveContext(Using_directiveContext ctx) { copyFrom(ctx); }
	}
	public static class UsingStaticDirectiveContext extends Using_directiveContext {
		public TerminalNode USING() { return getToken(CSharpParser.USING, 0); }
		public TerminalNode STATIC() { return getToken(CSharpParser.STATIC, 0); }
		public Namespace_or_type_nameContext namespace_or_type_name() {
			return getRuleContext(Namespace_or_type_nameContext.class,0);
		}
		public UsingStaticDirectiveContext(Using_directiveContext ctx) { copyFrom(ctx); }
	}

	public final Using_directiveContext using_directive() throws RecognitionException {
		Using_directiveContext _localctx = new Using_directiveContext(_ctx, getState());
		enterRule(_localctx, 184, RULE_using_directive);
		try {
			setState(1389);
			_errHandler.sync(this);
			switch ( getInterpreter().adaptivePredict(_input,138,_ctx) ) {
			case 1:
				_localctx = new UsingAliasDirectiveContext(_localctx);
				enterOuterAlt(_localctx, 1);
				{
				setState(1374);
				match(USING);
				setState(1375);
				identifier();
				setState(1376);
				match(ASSIGNMENT);
				setState(1377);
				namespace_or_type_name();
				setState(1378);
				match(SEMICOLON);
				}
				break;
			case 2:
				_localctx = new UsingNamespaceDirectiveContext(_localctx);
				enterOuterAlt(_localctx, 2);
				{
				setState(1380);
				match(USING);
				setState(1381);
				namespace_or_type_name();
				setState(1382);
				match(SEMICOLON);
				}
				break;
			case 3:
				_localctx = new UsingStaticDirectiveContext(_localctx);
				enterOuterAlt(_localctx, 3);
				{
				setState(1384);
				match(USING);
				setState(1385);
				match(STATIC);
				setState(1386);
				namespace_or_type_name();
				setState(1387);
				match(SEMICOLON);
				}
				break;
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Namespace_member_declarationsContext extends ParserRuleContext {
		public List<Namespace_member_declarationContext> namespace_member_declaration() {
			return getRuleContexts(Namespace_member_declarationContext.class);
		}
		public Namespace_member_declarationContext namespace_member_declaration(int i) {
			return getRuleContext(Namespace_member_declarationContext.class,i);
		}
		public Namespace_member_declarationsContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_namespace_member_declarations; }
	}

	public final Namespace_member_declarationsContext namespace_member_declarations() throws RecognitionException {
		Namespace_member_declarationsContext _localctx = new Namespace_member_declarationsContext(_ctx, getState());
		enterRule(_localctx, 186, RULE_namespace_member_declarations);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(1392); 
			_errHandler.sync(this);
			_la = _input.LA(1);
			do {
				{
				{
				setState(1391);
				namespace_member_declaration();
				}
				}
				setState(1394); 
				_errHandler.sync(this);
				_la = _input.LA(1);
			} while ( (((_la) & ~0x3f) == 0 && ((1L << _la) & ((1L << ABSTRACT) | (1L << ASYNC) | (1L << CLASS) | (1L << DELEGATE) | (1L << ENUM) | (1L << EXTERN) | (1L << INTERFACE) | (1L << INTERNAL))) != 0) || ((((_la - 64)) & ~0x3f) == 0 && ((1L << (_la - 64)) & ((1L << (NAMESPACE - 64)) | (1L << (NEW - 64)) | (1L << (OVERRIDE - 64)) | (1L << (PARTIAL - 64)) | (1L << (PRIVATE - 64)) | (1L << (PROTECTED - 64)) | (1L << (PUBLIC - 64)) | (1L << (READONLY - 64)) | (1L << (SEALED - 64)) | (1L << (STATIC - 64)) | (1L << (STRUCT - 64)) | (1L << (UNSAFE - 64)) | (1L << (VIRTUAL - 64)) | (1L << (VOLATILE - 64)) | (1L << (OPEN_BRACKET - 64)))) != 0) );
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Namespace_member_declarationContext extends ParserRuleContext {
		public Namespace_declarationContext namespace_declaration() {
			return getRuleContext(Namespace_declarationContext.class,0);
		}
		public Type_declarationContext type_declaration() {
			return getRuleContext(Type_declarationContext.class,0);
		}
		public Namespace_member_declarationContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_namespace_member_declaration; }
	}

	public final Namespace_member_declarationContext namespace_member_declaration() throws RecognitionException {
		Namespace_member_declarationContext _localctx = new Namespace_member_declarationContext(_ctx, getState());
		enterRule(_localctx, 188, RULE_namespace_member_declaration);
		try {
			setState(1398);
			_errHandler.sync(this);
			switch (_input.LA(1)) {
			case NAMESPACE:
				enterOuterAlt(_localctx, 1);
				{
				setState(1396);
				namespace_declaration();
				}
				break;
			case ABSTRACT:
			case ASYNC:
			case CLASS:
			case DELEGATE:
			case ENUM:
			case EXTERN:
			case INTERFACE:
			case INTERNAL:
			case NEW:
			case OVERRIDE:
			case PARTIAL:
			case PRIVATE:
			case PROTECTED:
			case PUBLIC:
			case READONLY:
			case SEALED:
			case STATIC:
			case STRUCT:
			case UNSAFE:
			case VIRTUAL:
			case VOLATILE:
			case OPEN_BRACKET:
				enterOuterAlt(_localctx, 2);
				{
				setState(1397);
				type_declaration();
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Type_declarationContext extends ParserRuleContext {
		public Class_definitionContext class_definition() {
			return getRuleContext(Class_definitionContext.class,0);
		}
		public Struct_definitionContext struct_definition() {
			return getRuleContext(Struct_definitionContext.class,0);
		}
		public Interface_definitionContext interface_definition() {
			return getRuleContext(Interface_definitionContext.class,0);
		}
		public Enum_definitionContext enum_definition() {
			return getRuleContext(Enum_definitionContext.class,0);
		}
		public Delegate_definitionContext delegate_definition() {
			return getRuleContext(Delegate_definitionContext.class,0);
		}
		public AttributesContext attributes() {
			return getRuleContext(AttributesContext.class,0);
		}
		public All_member_modifiersContext all_member_modifiers() {
			return getRuleContext(All_member_modifiersContext.class,0);
		}
		public Type_declarationContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_type_declaration; }
	}

	public final Type_declarationContext type_declaration() throws RecognitionException {
		Type_declarationContext _localctx = new Type_declarationContext(_ctx, getState());
		enterRule(_localctx, 190, RULE_type_declaration);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(1401);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==OPEN_BRACKET) {
				{
				setState(1400);
				attributes();
				}
			}

			setState(1404);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if ((((_la) & ~0x3f) == 0 && ((1L << _la) & ((1L << ABSTRACT) | (1L << ASYNC) | (1L << EXTERN) | (1L << INTERNAL))) != 0) || ((((_la - 65)) & ~0x3f) == 0 && ((1L << (_la - 65)) & ((1L << (NEW - 65)) | (1L << (OVERRIDE - 65)) | (1L << (PARTIAL - 65)) | (1L << (PRIVATE - 65)) | (1L << (PROTECTED - 65)) | (1L << (PUBLIC - 65)) | (1L << (READONLY - 65)) | (1L << (SEALED - 65)) | (1L << (STATIC - 65)) | (1L << (UNSAFE - 65)) | (1L << (VIRTUAL - 65)) | (1L << (VOLATILE - 65)))) != 0)) {
				{
				setState(1403);
				all_member_modifiers();
				}
			}

			setState(1411);
			_errHandler.sync(this);
			switch (_input.LA(1)) {
			case CLASS:
				{
				setState(1406);
				class_definition();
				}
				break;
			case STRUCT:
				{
				setState(1407);
				struct_definition();
				}
				break;
			case INTERFACE:
				{
				setState(1408);
				interface_definition();
				}
				break;
			case ENUM:
				{
				setState(1409);
				enum_definition();
				}
				break;
			case DELEGATE:
				{
				setState(1410);
				delegate_definition();
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Qualified_alias_memberContext extends ParserRuleContext {
		public List<IdentifierContext> identifier() {
			return getRuleContexts(IdentifierContext.class);
		}
		public IdentifierContext identifier(int i) {
			return getRuleContext(IdentifierContext.class,i);
		}
		public Type_argument_listContext type_argument_list() {
			return getRuleContext(Type_argument_listContext.class,0);
		}
		public Qualified_alias_memberContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_qualified_alias_member; }
	}

	public final Qualified_alias_memberContext qualified_alias_member() throws RecognitionException {
		Qualified_alias_memberContext _localctx = new Qualified_alias_memberContext(_ctx, getState());
		enterRule(_localctx, 192, RULE_qualified_alias_member);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(1413);
			identifier();
			setState(1414);
			match(DOUBLE_COLON);
			setState(1415);
			identifier();
			setState(1417);
			_errHandler.sync(this);
			switch ( getInterpreter().adaptivePredict(_input,144,_ctx) ) {
			case 1:
				{
				setState(1416);
				type_argument_list();
				}
				break;
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Type_parameter_listContext extends ParserRuleContext {
		public List<Type_parameterContext> type_parameter() {
			return getRuleContexts(Type_parameterContext.class);
		}
		public Type_parameterContext type_parameter(int i) {
			return getRuleContext(Type_parameterContext.class,i);
		}
		public Type_parameter_listContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_type_parameter_list; }
	}

	public final Type_parameter_listContext type_parameter_list() throws RecognitionException {
		Type_parameter_listContext _localctx = new Type_parameter_listContext(_ctx, getState());
		enterRule(_localctx, 194, RULE_type_parameter_list);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(1419);
			match(LT);
			setState(1420);
			type_parameter();
			setState(1425);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while (_la==COMMA) {
				{
				{
				setState(1421);
				match(COMMA);
				setState(1422);
				type_parameter();
				}
				}
				setState(1427);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			setState(1428);
			match(GT);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Type_parameterContext extends ParserRuleContext {
		public IdentifierContext identifier() {
			return getRuleContext(IdentifierContext.class,0);
		}
		public AttributesContext attributes() {
			return getRuleContext(AttributesContext.class,0);
		}
		public Type_parameterContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_type_parameter; }
	}

	public final Type_parameterContext type_parameter() throws RecognitionException {
		Type_parameterContext _localctx = new Type_parameterContext(_ctx, getState());
		enterRule(_localctx, 196, RULE_type_parameter);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(1431);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==OPEN_BRACKET) {
				{
				setState(1430);
				attributes();
				}
			}

			setState(1433);
			identifier();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Class_baseContext extends ParserRuleContext {
		public Class_typeContext class_type() {
			return getRuleContext(Class_typeContext.class,0);
		}
		public List<Namespace_or_type_nameContext> namespace_or_type_name() {
			return getRuleContexts(Namespace_or_type_nameContext.class);
		}
		public Namespace_or_type_nameContext namespace_or_type_name(int i) {
			return getRuleContext(Namespace_or_type_nameContext.class,i);
		}
		public Class_baseContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_class_base; }
	}

	public final Class_baseContext class_base() throws RecognitionException {
		Class_baseContext _localctx = new Class_baseContext(_ctx, getState());
		enterRule(_localctx, 198, RULE_class_base);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(1435);
			match(COLON);
			setState(1436);
			class_type();
			setState(1441);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while (_la==COMMA) {
				{
				{
				setState(1437);
				match(COMMA);
				setState(1438);
				namespace_or_type_name();
				}
				}
				setState(1443);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Interface_type_listContext extends ParserRuleContext {
		public List<Namespace_or_type_nameContext> namespace_or_type_name() {
			return getRuleContexts(Namespace_or_type_nameContext.class);
		}
		public Namespace_or_type_nameContext namespace_or_type_name(int i) {
			return getRuleContext(Namespace_or_type_nameContext.class,i);
		}
		public Interface_type_listContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_interface_type_list; }
	}

	public final Interface_type_listContext interface_type_list() throws RecognitionException {
		Interface_type_listContext _localctx = new Interface_type_listContext(_ctx, getState());
		enterRule(_localctx, 200, RULE_interface_type_list);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(1444);
			namespace_or_type_name();
			setState(1449);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while (_la==COMMA) {
				{
				{
				setState(1445);
				match(COMMA);
				setState(1446);
				namespace_or_type_name();
				}
				}
				setState(1451);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Type_parameter_constraints_clausesContext extends ParserRuleContext {
		public List<Type_parameter_constraints_clauseContext> type_parameter_constraints_clause() {
			return getRuleContexts(Type_parameter_constraints_clauseContext.class);
		}
		public Type_parameter_constraints_clauseContext type_parameter_constraints_clause(int i) {
			return getRuleContext(Type_parameter_constraints_clauseContext.class,i);
		}
		public Type_parameter_constraints_clausesContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_type_parameter_constraints_clauses; }
	}

	public final Type_parameter_constraints_clausesContext type_parameter_constraints_clauses() throws RecognitionException {
		Type_parameter_constraints_clausesContext _localctx = new Type_parameter_constraints_clausesContext(_ctx, getState());
		enterRule(_localctx, 202, RULE_type_parameter_constraints_clauses);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(1453); 
			_errHandler.sync(this);
			_la = _input.LA(1);
			do {
				{
				{
				setState(1452);
				type_parameter_constraints_clause();
				}
				}
				setState(1455); 
				_errHandler.sync(this);
				_la = _input.LA(1);
			} while ( _la==WHERE );
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Type_parameter_constraints_clauseContext extends ParserRuleContext {
		public TerminalNode WHERE() { return getToken(CSharpParser.WHERE, 0); }
		public IdentifierContext identifier() {
			return getRuleContext(IdentifierContext.class,0);
		}
		public Type_parameter_constraintsContext type_parameter_constraints() {
			return getRuleContext(Type_parameter_constraintsContext.class,0);
		}
		public Type_parameter_constraints_clauseContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_type_parameter_constraints_clause; }
	}

	public final Type_parameter_constraints_clauseContext type_parameter_constraints_clause() throws RecognitionException {
		Type_parameter_constraints_clauseContext _localctx = new Type_parameter_constraints_clauseContext(_ctx, getState());
		enterRule(_localctx, 204, RULE_type_parameter_constraints_clause);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(1457);
			match(WHERE);
			setState(1458);
			identifier();
			setState(1459);
			match(COLON);
			setState(1460);
			type_parameter_constraints();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Type_parameter_constraintsContext extends ParserRuleContext {
		public Constructor_constraintContext constructor_constraint() {
			return getRuleContext(Constructor_constraintContext.class,0);
		}
		public Primary_constraintContext primary_constraint() {
			return getRuleContext(Primary_constraintContext.class,0);
		}
		public Secondary_constraintsContext secondary_constraints() {
			return getRuleContext(Secondary_constraintsContext.class,0);
		}
		public Type_parameter_constraintsContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_type_parameter_constraints; }
	}

	public final Type_parameter_constraintsContext type_parameter_constraints() throws RecognitionException {
		Type_parameter_constraintsContext _localctx = new Type_parameter_constraintsContext(_ctx, getState());
		enterRule(_localctx, 206, RULE_type_parameter_constraints);
		int _la;
		try {
			setState(1472);
			_errHandler.sync(this);
			switch (_input.LA(1)) {
			case NEW:
				enterOuterAlt(_localctx, 1);
				{
				setState(1462);
				constructor_constraint();
				}
				break;
			case ADD:
			case ALIAS:
			case ARGLIST:
			case ASCENDING:
			case ASYNC:
			case AWAIT:
			case BY:
			case CLASS:
			case DESCENDING:
			case DYNAMIC:
			case EQUALS:
			case FROM:
			case GET:
			case GROUP:
			case INTO:
			case JOIN:
			case LET:
			case NAMEOF:
			case OBJECT:
			case ON:
			case ORDERBY:
			case PARTIAL:
			case REMOVE:
			case SELECT:
			case SET:
			case STRING:
			case STRUCT:
			case WHEN:
			case WHERE:
			case YIELD:
			case IDENTIFIER:
				enterOuterAlt(_localctx, 2);
				{
				setState(1463);
				primary_constraint();
				setState(1466);
				_errHandler.sync(this);
				switch ( getInterpreter().adaptivePredict(_input,150,_ctx) ) {
				case 1:
					{
					setState(1464);
					match(COMMA);
					setState(1465);
					secondary_constraints();
					}
					break;
				}
				setState(1470);
				_errHandler.sync(this);
				_la = _input.LA(1);
				if (_la==COMMA) {
					{
					setState(1468);
					match(COMMA);
					setState(1469);
					constructor_constraint();
					}
				}

				}
				break;
			default:
				throw new NoViableAltException(this);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Primary_constraintContext extends ParserRuleContext {
		public Class_typeContext class_type() {
			return getRuleContext(Class_typeContext.class,0);
		}
		public TerminalNode CLASS() { return getToken(CSharpParser.CLASS, 0); }
		public TerminalNode STRUCT() { return getToken(CSharpParser.STRUCT, 0); }
		public Primary_constraintContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_primary_constraint; }
	}

	public final Primary_constraintContext primary_constraint() throws RecognitionException {
		Primary_constraintContext _localctx = new Primary_constraintContext(_ctx, getState());
		enterRule(_localctx, 208, RULE_primary_constraint);
		try {
			setState(1477);
			_errHandler.sync(this);
			switch (_input.LA(1)) {
			case ADD:
			case ALIAS:
			case ARGLIST:
			case ASCENDING:
			case ASYNC:
			case AWAIT:
			case BY:
			case DESCENDING:
			case DYNAMIC:
			case EQUALS:
			case FROM:
			case GET:
			case GROUP:
			case INTO:
			case JOIN:
			case LET:
			case NAMEOF:
			case OBJECT:
			case ON:
			case ORDERBY:
			case PARTIAL:
			case REMOVE:
			case SELECT:
			case SET:
			case STRING:
			case WHEN:
			case WHERE:
			case YIELD:
			case IDENTIFIER:
				enterOuterAlt(_localctx, 1);
				{
				setState(1474);
				class_type();
				}
				break;
			case CLASS:
				enterOuterAlt(_localctx, 2);
				{
				setState(1475);
				match(CLASS);
				}
				break;
			case STRUCT:
				enterOuterAlt(_localctx, 3);
				{
				setState(1476);
				match(STRUCT);
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Secondary_constraintsContext extends ParserRuleContext {
		public List<Namespace_or_type_nameContext> namespace_or_type_name() {
			return getRuleContexts(Namespace_or_type_nameContext.class);
		}
		public Namespace_or_type_nameContext namespace_or_type_name(int i) {
			return getRuleContext(Namespace_or_type_nameContext.class,i);
		}
		public Secondary_constraintsContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_secondary_constraints; }
	}

	public final Secondary_constraintsContext secondary_constraints() throws RecognitionException {
		Secondary_constraintsContext _localctx = new Secondary_constraintsContext(_ctx, getState());
		enterRule(_localctx, 210, RULE_secondary_constraints);
		try {
			int _alt;
			enterOuterAlt(_localctx, 1);
			{
			setState(1479);
			namespace_or_type_name();
			setState(1484);
			_errHandler.sync(this);
			_alt = getInterpreter().adaptivePredict(_input,154,_ctx);
			while ( _alt!=2 && _alt!=org.antlr.v4.runtime.atn.ATN.INVALID_ALT_NUMBER ) {
				if ( _alt==1 ) {
					{
					{
					setState(1480);
					match(COMMA);
					setState(1481);
					namespace_or_type_name();
					}
					} 
				}
				setState(1486);
				_errHandler.sync(this);
				_alt = getInterpreter().adaptivePredict(_input,154,_ctx);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Constructor_constraintContext extends ParserRuleContext {
		public TerminalNode NEW() { return getToken(CSharpParser.NEW, 0); }
		public TerminalNode OPEN_PARENS() { return getToken(CSharpParser.OPEN_PARENS, 0); }
		public TerminalNode CLOSE_PARENS() { return getToken(CSharpParser.CLOSE_PARENS, 0); }
		public Constructor_constraintContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_constructor_constraint; }
	}

	public final Constructor_constraintContext constructor_constraint() throws RecognitionException {
		Constructor_constraintContext _localctx = new Constructor_constraintContext(_ctx, getState());
		enterRule(_localctx, 212, RULE_constructor_constraint);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(1487);
			match(NEW);
			setState(1488);
			match(OPEN_PARENS);
			setState(1489);
			match(CLOSE_PARENS);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Class_bodyContext extends ParserRuleContext {
		public TerminalNode OPEN_BRACE() { return getToken(CSharpParser.OPEN_BRACE, 0); }
		public TerminalNode CLOSE_BRACE() { return getToken(CSharpParser.CLOSE_BRACE, 0); }
		public Class_member_declarationsContext class_member_declarations() {
			return getRuleContext(Class_member_declarationsContext.class,0);
		}
		public Class_bodyContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_class_body; }
	}

	public final Class_bodyContext class_body() throws RecognitionException {
		Class_bodyContext _localctx = new Class_bodyContext(_ctx, getState());
		enterRule(_localctx, 214, RULE_class_body);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(1491);
			match(OPEN_BRACE);
			setState(1493);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if ((((_la) & ~0x3f) == 0 && ((1L << _la) & ((1L << ABSTRACT) | (1L << ADD) | (1L << ALIAS) | (1L << ARGLIST) | (1L << ASCENDING) | (1L << ASYNC) | (1L << AWAIT) | (1L << BOOL) | (1L << BY) | (1L << BYTE) | (1L << CHAR) | (1L << CLASS) | (1L << CONST) | (1L << DECIMAL) | (1L << DELEGATE) | (1L << DESCENDING) | (1L << DOUBLE) | (1L << DYNAMIC) | (1L << ENUM) | (1L << EQUALS) | (1L << EVENT) | (1L << EXPLICIT) | (1L << EXTERN) | (1L << FLOAT) | (1L << FROM) | (1L << GET) | (1L << GROUP) | (1L << IMPLICIT) | (1L << INT) | (1L << INTERFACE) | (1L << INTERNAL) | (1L << INTO) | (1L << JOIN) | (1L << LET) | (1L << LONG) | (1L << NAMEOF))) != 0) || ((((_la - 65)) & ~0x3f) == 0 && ((1L << (_la - 65)) & ((1L << (NEW - 65)) | (1L << (OBJECT - 65)) | (1L << (ON - 65)) | (1L << (ORDERBY - 65)) | (1L << (OVERRIDE - 65)) | (1L << (PARTIAL - 65)) | (1L << (PRIVATE - 65)) | (1L << (PROTECTED - 65)) | (1L << (PUBLIC - 65)) | (1L << (READONLY - 65)) | (1L << (REMOVE - 65)) | (1L << (SBYTE - 65)) | (1L << (SEALED - 65)) | (1L << (SELECT - 65)) | (1L << (SET - 65)) | (1L << (SHORT - 65)) | (1L << (STATIC - 65)) | (1L << (STRING - 65)) | (1L << (STRUCT - 65)) | (1L << (UINT - 65)) | (1L << (ULONG - 65)) | (1L << (UNSAFE - 65)) | (1L << (USHORT - 65)) | (1L << (VIRTUAL - 65)) | (1L << (VOID - 65)) | (1L << (VOLATILE - 65)) | (1L << (WHEN - 65)) | (1L << (WHERE - 65)) | (1L << (YIELD - 65)) | (1L << (IDENTIFIER - 65)) | (1L << (OPEN_BRACKET - 65)))) != 0) || _la==TILDE) {
				{
				setState(1492);
				class_member_declarations();
				}
			}

			setState(1495);
			match(CLOSE_BRACE);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Class_member_declarationsContext extends ParserRuleContext {
		public List<Class_member_declarationContext> class_member_declaration() {
			return getRuleContexts(Class_member_declarationContext.class);
		}
		public Class_member_declarationContext class_member_declaration(int i) {
			return getRuleContext(Class_member_declarationContext.class,i);
		}
		public Class_member_declarationsContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_class_member_declarations; }
	}

	public final Class_member_declarationsContext class_member_declarations() throws RecognitionException {
		Class_member_declarationsContext _localctx = new Class_member_declarationsContext(_ctx, getState());
		enterRule(_localctx, 216, RULE_class_member_declarations);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(1498); 
			_errHandler.sync(this);
			_la = _input.LA(1);
			do {
				{
				{
				setState(1497);
				class_member_declaration();
				}
				}
				setState(1500); 
				_errHandler.sync(this);
				_la = _input.LA(1);
			} while ( (((_la) & ~0x3f) == 0 && ((1L << _la) & ((1L << ABSTRACT) | (1L << ADD) | (1L << ALIAS) | (1L << ARGLIST) | (1L << ASCENDING) | (1L << ASYNC) | (1L << AWAIT) | (1L << BOOL) | (1L << BY) | (1L << BYTE) | (1L << CHAR) | (1L << CLASS) | (1L << CONST) | (1L << DECIMAL) | (1L << DELEGATE) | (1L << DESCENDING) | (1L << DOUBLE) | (1L << DYNAMIC) | (1L << ENUM) | (1L << EQUALS) | (1L << EVENT) | (1L << EXPLICIT) | (1L << EXTERN) | (1L << FLOAT) | (1L << FROM) | (1L << GET) | (1L << GROUP) | (1L << IMPLICIT) | (1L << INT) | (1L << INTERFACE) | (1L << INTERNAL) | (1L << INTO) | (1L << JOIN) | (1L << LET) | (1L << LONG) | (1L << NAMEOF))) != 0) || ((((_la - 65)) & ~0x3f) == 0 && ((1L << (_la - 65)) & ((1L << (NEW - 65)) | (1L << (OBJECT - 65)) | (1L << (ON - 65)) | (1L << (ORDERBY - 65)) | (1L << (OVERRIDE - 65)) | (1L << (PARTIAL - 65)) | (1L << (PRIVATE - 65)) | (1L << (PROTECTED - 65)) | (1L << (PUBLIC - 65)) | (1L << (READONLY - 65)) | (1L << (REMOVE - 65)) | (1L << (SBYTE - 65)) | (1L << (SEALED - 65)) | (1L << (SELECT - 65)) | (1L << (SET - 65)) | (1L << (SHORT - 65)) | (1L << (STATIC - 65)) | (1L << (STRING - 65)) | (1L << (STRUCT - 65)) | (1L << (UINT - 65)) | (1L << (ULONG - 65)) | (1L << (UNSAFE - 65)) | (1L << (USHORT - 65)) | (1L << (VIRTUAL - 65)) | (1L << (VOID - 65)) | (1L << (VOLATILE - 65)) | (1L << (WHEN - 65)) | (1L << (WHERE - 65)) | (1L << (YIELD - 65)) | (1L << (IDENTIFIER - 65)) | (1L << (OPEN_BRACKET - 65)))) != 0) || _la==TILDE );
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Class_member_declarationContext extends ParserRuleContext {
		public Common_member_declarationContext common_member_declaration() {
			return getRuleContext(Common_member_declarationContext.class,0);
		}
		public Destructor_definitionContext destructor_definition() {
			return getRuleContext(Destructor_definitionContext.class,0);
		}
		public AttributesContext attributes() {
			return getRuleContext(AttributesContext.class,0);
		}
		public All_member_modifiersContext all_member_modifiers() {
			return getRuleContext(All_member_modifiersContext.class,0);
		}
		public Class_member_declarationContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_class_member_declaration; }
	}

	public final Class_member_declarationContext class_member_declaration() throws RecognitionException {
		Class_member_declarationContext _localctx = new Class_member_declarationContext(_ctx, getState());
		enterRule(_localctx, 218, RULE_class_member_declaration);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(1503);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==OPEN_BRACKET) {
				{
				setState(1502);
				attributes();
				}
			}

			setState(1506);
			_errHandler.sync(this);
			switch ( getInterpreter().adaptivePredict(_input,158,_ctx) ) {
			case 1:
				{
				setState(1505);
				all_member_modifiers();
				}
				break;
			}
			setState(1510);
			_errHandler.sync(this);
			switch (_input.LA(1)) {
			case ADD:
			case ALIAS:
			case ARGLIST:
			case ASCENDING:
			case ASYNC:
			case AWAIT:
			case BOOL:
			case BY:
			case BYTE:
			case CHAR:
			case CLASS:
			case CONST:
			case DECIMAL:
			case DELEGATE:
			case DESCENDING:
			case DOUBLE:
			case DYNAMIC:
			case ENUM:
			case EQUALS:
			case EVENT:
			case EXPLICIT:
			case FLOAT:
			case FROM:
			case GET:
			case GROUP:
			case IMPLICIT:
			case INT:
			case INTERFACE:
			case INTO:
			case JOIN:
			case LET:
			case LONG:
			case NAMEOF:
			case OBJECT:
			case ON:
			case ORDERBY:
			case PARTIAL:
			case REMOVE:
			case SBYTE:
			case SELECT:
			case SET:
			case SHORT:
			case STRING:
			case STRUCT:
			case UINT:
			case ULONG:
			case USHORT:
			case VOID:
			case WHEN:
			case WHERE:
			case YIELD:
			case IDENTIFIER:
				{
				setState(1508);
				common_member_declaration();
				}
				break;
			case TILDE:
				{
				setState(1509);
				destructor_definition();
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class All_member_modifiersContext extends ParserRuleContext {
		public List<All_member_modifierContext> all_member_modifier() {
			return getRuleContexts(All_member_modifierContext.class);
		}
		public All_member_modifierContext all_member_modifier(int i) {
			return getRuleContext(All_member_modifierContext.class,i);
		}
		public All_member_modifiersContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_all_member_modifiers; }
	}

	public final All_member_modifiersContext all_member_modifiers() throws RecognitionException {
		All_member_modifiersContext _localctx = new All_member_modifiersContext(_ctx, getState());
		enterRule(_localctx, 220, RULE_all_member_modifiers);
		try {
			int _alt;
			enterOuterAlt(_localctx, 1);
			{
			setState(1513); 
			_errHandler.sync(this);
			_alt = 1;
			do {
				switch (_alt) {
				case 1:
					{
					{
					setState(1512);
					all_member_modifier();
					}
					}
					break;
				default:
					throw new NoViableAltException(this);
				}
				setState(1515); 
				_errHandler.sync(this);
				_alt = getInterpreter().adaptivePredict(_input,160,_ctx);
			} while ( _alt!=2 && _alt!=org.antlr.v4.runtime.atn.ATN.INVALID_ALT_NUMBER );
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class All_member_modifierContext extends ParserRuleContext {
		public TerminalNode NEW() { return getToken(CSharpParser.NEW, 0); }
		public TerminalNode PUBLIC() { return getToken(CSharpParser.PUBLIC, 0); }
		public TerminalNode PROTECTED() { return getToken(CSharpParser.PROTECTED, 0); }
		public TerminalNode INTERNAL() { return getToken(CSharpParser.INTERNAL, 0); }
		public TerminalNode PRIVATE() { return getToken(CSharpParser.PRIVATE, 0); }
		public TerminalNode READONLY() { return getToken(CSharpParser.READONLY, 0); }
		public TerminalNode VOLATILE() { return getToken(CSharpParser.VOLATILE, 0); }
		public TerminalNode VIRTUAL() { return getToken(CSharpParser.VIRTUAL, 0); }
		public TerminalNode SEALED() { return getToken(CSharpParser.SEALED, 0); }
		public TerminalNode OVERRIDE() { return getToken(CSharpParser.OVERRIDE, 0); }
		public TerminalNode ABSTRACT() { return getToken(CSharpParser.ABSTRACT, 0); }
		public TerminalNode STATIC() { return getToken(CSharpParser.STATIC, 0); }
		public TerminalNode UNSAFE() { return getToken(CSharpParser.UNSAFE, 0); }
		public TerminalNode EXTERN() { return getToken(CSharpParser.EXTERN, 0); }
		public TerminalNode PARTIAL() { return getToken(CSharpParser.PARTIAL, 0); }
		public TerminalNode ASYNC() { return getToken(CSharpParser.ASYNC, 0); }
		public All_member_modifierContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_all_member_modifier; }
	}

	public final All_member_modifierContext all_member_modifier() throws RecognitionException {
		All_member_modifierContext _localctx = new All_member_modifierContext(_ctx, getState());
		enterRule(_localctx, 222, RULE_all_member_modifier);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(1517);
			_la = _input.LA(1);
			if ( !((((_la) & ~0x3f) == 0 && ((1L << _la) & ((1L << ABSTRACT) | (1L << ASYNC) | (1L << EXTERN) | (1L << INTERNAL))) != 0) || ((((_la - 65)) & ~0x3f) == 0 && ((1L << (_la - 65)) & ((1L << (NEW - 65)) | (1L << (OVERRIDE - 65)) | (1L << (PARTIAL - 65)) | (1L << (PRIVATE - 65)) | (1L << (PROTECTED - 65)) | (1L << (PUBLIC - 65)) | (1L << (READONLY - 65)) | (1L << (SEALED - 65)) | (1L << (STATIC - 65)) | (1L << (UNSAFE - 65)) | (1L << (VIRTUAL - 65)) | (1L << (VOLATILE - 65)))) != 0)) ) {
			_errHandler.recoverInline(this);
			}
			else {
				if ( _input.LA(1)==Token.EOF ) matchedEOF = true;
				_errHandler.reportMatch(this);
				consume();
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Common_member_declarationContext extends ParserRuleContext {
		public Constant_declarationContext constant_declaration() {
			return getRuleContext(Constant_declarationContext.class,0);
		}
		public Typed_member_declarationContext typed_member_declaration() {
			return getRuleContext(Typed_member_declarationContext.class,0);
		}
		public Event_declarationContext event_declaration() {
			return getRuleContext(Event_declarationContext.class,0);
		}
		public Conversion_operator_declaratorContext conversion_operator_declarator() {
			return getRuleContext(Conversion_operator_declaratorContext.class,0);
		}
		public BodyContext body() {
			return getRuleContext(BodyContext.class,0);
		}
		public Right_arrowContext right_arrow() {
			return getRuleContext(Right_arrowContext.class,0);
		}
		public ExpressionContext expression() {
			return getRuleContext(ExpressionContext.class,0);
		}
		public Constructor_declarationContext constructor_declaration() {
			return getRuleContext(Constructor_declarationContext.class,0);
		}
		public TerminalNode VOID() { return getToken(CSharpParser.VOID, 0); }
		public Method_declarationContext method_declaration() {
			return getRuleContext(Method_declarationContext.class,0);
		}
		public Class_definitionContext class_definition() {
			return getRuleContext(Class_definitionContext.class,0);
		}
		public Struct_definitionContext struct_definition() {
			return getRuleContext(Struct_definitionContext.class,0);
		}
		public Interface_definitionContext interface_definition() {
			return getRuleContext(Interface_definitionContext.class,0);
		}
		public Enum_definitionContext enum_definition() {
			return getRuleContext(Enum_definitionContext.class,0);
		}
		public Delegate_definitionContext delegate_definition() {
			return getRuleContext(Delegate_definitionContext.class,0);
		}
		public Common_member_declarationContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_common_member_declaration; }
	}

	public final Common_member_declarationContext common_member_declaration() throws RecognitionException {
		Common_member_declarationContext _localctx = new Common_member_declarationContext(_ctx, getState());
		enterRule(_localctx, 224, RULE_common_member_declaration);
		try {
			setState(1538);
			_errHandler.sync(this);
			switch ( getInterpreter().adaptivePredict(_input,162,_ctx) ) {
			case 1:
				enterOuterAlt(_localctx, 1);
				{
				setState(1519);
				constant_declaration();
				}
				break;
			case 2:
				enterOuterAlt(_localctx, 2);
				{
				setState(1520);
				typed_member_declaration();
				}
				break;
			case 3:
				enterOuterAlt(_localctx, 3);
				{
				setState(1521);
				event_declaration();
				}
				break;
			case 4:
				enterOuterAlt(_localctx, 4);
				{
				setState(1522);
				conversion_operator_declarator();
				setState(1528);
				_errHandler.sync(this);
				switch (_input.LA(1)) {
				case OPEN_BRACE:
				case SEMICOLON:
					{
					setState(1523);
					body();
					}
					break;
				case ASSIGNMENT:
					{
					setState(1524);
					right_arrow();
					setState(1525);
					expression();
					setState(1526);
					match(SEMICOLON);
					}
					break;
				default:
					throw new NoViableAltException(this);
				}
				}
				break;
			case 5:
				enterOuterAlt(_localctx, 5);
				{
				setState(1530);
				constructor_declaration();
				}
				break;
			case 6:
				enterOuterAlt(_localctx, 6);
				{
				setState(1531);
				match(VOID);
				setState(1532);
				method_declaration();
				}
				break;
			case 7:
				enterOuterAlt(_localctx, 7);
				{
				setState(1533);
				class_definition();
				}
				break;
			case 8:
				enterOuterAlt(_localctx, 8);
				{
				setState(1534);
				struct_definition();
				}
				break;
			case 9:
				enterOuterAlt(_localctx, 9);
				{
				setState(1535);
				interface_definition();
				}
				break;
			case 10:
				enterOuterAlt(_localctx, 10);
				{
				setState(1536);
				enum_definition();
				}
				break;
			case 11:
				enterOuterAlt(_localctx, 11);
				{
				setState(1537);
				delegate_definition();
				}
				break;
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Typed_member_declarationContext extends ParserRuleContext {
		public TypeContext type() {
			return getRuleContext(TypeContext.class,0);
		}
		public Namespace_or_type_nameContext namespace_or_type_name() {
			return getRuleContext(Namespace_or_type_nameContext.class,0);
		}
		public Indexer_declarationContext indexer_declaration() {
			return getRuleContext(Indexer_declarationContext.class,0);
		}
		public Method_declarationContext method_declaration() {
			return getRuleContext(Method_declarationContext.class,0);
		}
		public Property_declarationContext property_declaration() {
			return getRuleContext(Property_declarationContext.class,0);
		}
		public Operator_declarationContext operator_declaration() {
			return getRuleContext(Operator_declarationContext.class,0);
		}
		public Field_declarationContext field_declaration() {
			return getRuleContext(Field_declarationContext.class,0);
		}
		public Typed_member_declarationContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_typed_member_declaration; }
	}

	public final Typed_member_declarationContext typed_member_declaration() throws RecognitionException {
		Typed_member_declarationContext _localctx = new Typed_member_declarationContext(_ctx, getState());
		enterRule(_localctx, 226, RULE_typed_member_declaration);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(1540);
			type();
			setState(1550);
			_errHandler.sync(this);
			switch ( getInterpreter().adaptivePredict(_input,163,_ctx) ) {
			case 1:
				{
				setState(1541);
				namespace_or_type_name();
				setState(1542);
				match(DOT);
				setState(1543);
				indexer_declaration();
				}
				break;
			case 2:
				{
				setState(1545);
				method_declaration();
				}
				break;
			case 3:
				{
				setState(1546);
				property_declaration();
				}
				break;
			case 4:
				{
				setState(1547);
				indexer_declaration();
				}
				break;
			case 5:
				{
				setState(1548);
				operator_declaration();
				}
				break;
			case 6:
				{
				setState(1549);
				field_declaration();
				}
				break;
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Constant_declaratorsContext extends ParserRuleContext {
		public List<Constant_declaratorContext> constant_declarator() {
			return getRuleContexts(Constant_declaratorContext.class);
		}
		public Constant_declaratorContext constant_declarator(int i) {
			return getRuleContext(Constant_declaratorContext.class,i);
		}
		public Constant_declaratorsContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_constant_declarators; }
	}

	public final Constant_declaratorsContext constant_declarators() throws RecognitionException {
		Constant_declaratorsContext _localctx = new Constant_declaratorsContext(_ctx, getState());
		enterRule(_localctx, 228, RULE_constant_declarators);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(1552);
			constant_declarator();
			setState(1557);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while (_la==COMMA) {
				{
				{
				setState(1553);
				match(COMMA);
				setState(1554);
				constant_declarator();
				}
				}
				setState(1559);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Constant_declaratorContext extends ParserRuleContext {
		public IdentifierContext identifier() {
			return getRuleContext(IdentifierContext.class,0);
		}
		public ExpressionContext expression() {
			return getRuleContext(ExpressionContext.class,0);
		}
		public Constant_declaratorContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_constant_declarator; }
	}

	public final Constant_declaratorContext constant_declarator() throws RecognitionException {
		Constant_declaratorContext _localctx = new Constant_declaratorContext(_ctx, getState());
		enterRule(_localctx, 230, RULE_constant_declarator);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(1560);
			identifier();
			setState(1561);
			match(ASSIGNMENT);
			setState(1562);
			expression();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Variable_declaratorsContext extends ParserRuleContext {
		public List<Variable_declaratorContext> variable_declarator() {
			return getRuleContexts(Variable_declaratorContext.class);
		}
		public Variable_declaratorContext variable_declarator(int i) {
			return getRuleContext(Variable_declaratorContext.class,i);
		}
		public Variable_declaratorsContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_variable_declarators; }
	}

	public final Variable_declaratorsContext variable_declarators() throws RecognitionException {
		Variable_declaratorsContext _localctx = new Variable_declaratorsContext(_ctx, getState());
		enterRule(_localctx, 232, RULE_variable_declarators);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(1564);
			variable_declarator();
			setState(1569);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while (_la==COMMA) {
				{
				{
				setState(1565);
				match(COMMA);
				setState(1566);
				variable_declarator();
				}
				}
				setState(1571);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Variable_declaratorContext extends ParserRuleContext {
		public IdentifierContext identifier() {
			return getRuleContext(IdentifierContext.class,0);
		}
		public Variable_initializerContext variable_initializer() {
			return getRuleContext(Variable_initializerContext.class,0);
		}
		public Variable_declaratorContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_variable_declarator; }
	}

	public final Variable_declaratorContext variable_declarator() throws RecognitionException {
		Variable_declaratorContext _localctx = new Variable_declaratorContext(_ctx, getState());
		enterRule(_localctx, 234, RULE_variable_declarator);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(1572);
			identifier();
			setState(1575);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==ASSIGNMENT) {
				{
				setState(1573);
				match(ASSIGNMENT);
				setState(1574);
				variable_initializer();
				}
			}

			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Variable_initializerContext extends ParserRuleContext {
		public ExpressionContext expression() {
			return getRuleContext(ExpressionContext.class,0);
		}
		public Array_initializerContext array_initializer() {
			return getRuleContext(Array_initializerContext.class,0);
		}
		public Variable_initializerContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_variable_initializer; }
	}

	public final Variable_initializerContext variable_initializer() throws RecognitionException {
		Variable_initializerContext _localctx = new Variable_initializerContext(_ctx, getState());
		enterRule(_localctx, 236, RULE_variable_initializer);
		try {
			setState(1579);
			_errHandler.sync(this);
			switch (_input.LA(1)) {
			case ADD:
			case ALIAS:
			case ARGLIST:
			case ASCENDING:
			case ASYNC:
			case AWAIT:
			case BASE:
			case BOOL:
			case BY:
			case BYTE:
			case CHAR:
			case CHECKED:
			case DECIMAL:
			case DEFAULT:
			case DELEGATE:
			case DESCENDING:
			case DOUBLE:
			case DYNAMIC:
			case EQUALS:
			case FALSE:
			case FLOAT:
			case FROM:
			case GET:
			case GROUP:
			case INT:
			case INTO:
			case JOIN:
			case LET:
			case LONG:
			case NAMEOF:
			case NEW:
			case NULL:
			case OBJECT:
			case ON:
			case ORDERBY:
			case PARTIAL:
			case REMOVE:
			case SBYTE:
			case SELECT:
			case SET:
			case SHORT:
			case SIZEOF:
			case STRING:
			case THIS:
			case TRUE:
			case TYPEOF:
			case UINT:
			case ULONG:
			case UNCHECKED:
			case USHORT:
			case WHEN:
			case WHERE:
			case YIELD:
			case IDENTIFIER:
			case LITERAL_ACCESS:
			case INTEGER_LITERAL:
			case HEX_INTEGER_LITERAL:
			case REAL_LITERAL:
			case CHARACTER_LITERAL:
			case REGULAR_STRING:
			case VERBATIUM_STRING:
			case INTERPOLATED_REGULAR_STRING_START:
			case INTERPOLATED_VERBATIUM_STRING_START:
			case OPEN_PARENS:
			case PLUS:
			case MINUS:
			case STAR:
			case AMP:
			case BANG:
			case TILDE:
			case OP_INC:
			case OP_DEC:
				enterOuterAlt(_localctx, 1);
				{
				setState(1577);
				expression();
				}
				break;
			case OPEN_BRACE:
				enterOuterAlt(_localctx, 2);
				{
				setState(1578);
				array_initializer();
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Return_typeContext extends ParserRuleContext {
		public TypeContext type() {
			return getRuleContext(TypeContext.class,0);
		}
		public TerminalNode VOID() { return getToken(CSharpParser.VOID, 0); }
		public Return_typeContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_return_type; }
	}

	public final Return_typeContext return_type() throws RecognitionException {
		Return_typeContext _localctx = new Return_typeContext(_ctx, getState());
		enterRule(_localctx, 238, RULE_return_type);
		try {
			setState(1583);
			_errHandler.sync(this);
			switch ( getInterpreter().adaptivePredict(_input,168,_ctx) ) {
			case 1:
				enterOuterAlt(_localctx, 1);
				{
				setState(1581);
				type();
				}
				break;
			case 2:
				enterOuterAlt(_localctx, 2);
				{
				setState(1582);
				match(VOID);
				}
				break;
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Member_nameContext extends ParserRuleContext {
		public Namespace_or_type_nameContext namespace_or_type_name() {
			return getRuleContext(Namespace_or_type_nameContext.class,0);
		}
		public Member_nameContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_member_name; }
	}

	public final Member_nameContext member_name() throws RecognitionException {
		Member_nameContext _localctx = new Member_nameContext(_ctx, getState());
		enterRule(_localctx, 240, RULE_member_name);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(1585);
			namespace_or_type_name();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Method_bodyContext extends ParserRuleContext {
		public BlockContext block() {
			return getRuleContext(BlockContext.class,0);
		}
		public Method_bodyContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_method_body; }
	}

	public final Method_bodyContext method_body() throws RecognitionException {
		Method_bodyContext _localctx = new Method_bodyContext(_ctx, getState());
		enterRule(_localctx, 242, RULE_method_body);
		try {
			setState(1589);
			_errHandler.sync(this);
			switch (_input.LA(1)) {
			case OPEN_BRACE:
				enterOuterAlt(_localctx, 1);
				{
				setState(1587);
				block();
				}
				break;
			case SEMICOLON:
				enterOuterAlt(_localctx, 2);
				{
				setState(1588);
				match(SEMICOLON);
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Formal_parameter_listContext extends ParserRuleContext {
		public Parameter_arrayContext parameter_array() {
			return getRuleContext(Parameter_arrayContext.class,0);
		}
		public Fixed_parametersContext fixed_parameters() {
			return getRuleContext(Fixed_parametersContext.class,0);
		}
		public Formal_parameter_listContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_formal_parameter_list; }
	}

	public final Formal_parameter_listContext formal_parameter_list() throws RecognitionException {
		Formal_parameter_listContext _localctx = new Formal_parameter_listContext(_ctx, getState());
		enterRule(_localctx, 244, RULE_formal_parameter_list);
		int _la;
		try {
			setState(1597);
			_errHandler.sync(this);
			switch ( getInterpreter().adaptivePredict(_input,171,_ctx) ) {
			case 1:
				enterOuterAlt(_localctx, 1);
				{
				setState(1591);
				parameter_array();
				}
				break;
			case 2:
				enterOuterAlt(_localctx, 2);
				{
				setState(1592);
				fixed_parameters();
				setState(1595);
				_errHandler.sync(this);
				_la = _input.LA(1);
				if (_la==COMMA) {
					{
					setState(1593);
					match(COMMA);
					setState(1594);
					parameter_array();
					}
				}

				}
				break;
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Fixed_parametersContext extends ParserRuleContext {
		public List<Fixed_parameterContext> fixed_parameter() {
			return getRuleContexts(Fixed_parameterContext.class);
		}
		public Fixed_parameterContext fixed_parameter(int i) {
			return getRuleContext(Fixed_parameterContext.class,i);
		}
		public Fixed_parametersContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_fixed_parameters; }
	}

	public final Fixed_parametersContext fixed_parameters() throws RecognitionException {
		Fixed_parametersContext _localctx = new Fixed_parametersContext(_ctx, getState());
		enterRule(_localctx, 246, RULE_fixed_parameters);
		try {
			int _alt;
			enterOuterAlt(_localctx, 1);
			{
			setState(1599);
			fixed_parameter();
			setState(1604);
			_errHandler.sync(this);
			_alt = getInterpreter().adaptivePredict(_input,172,_ctx);
			while ( _alt!=2 && _alt!=org.antlr.v4.runtime.atn.ATN.INVALID_ALT_NUMBER ) {
				if ( _alt==1 ) {
					{
					{
					setState(1600);
					match(COMMA);
					setState(1601);
					fixed_parameter();
					}
					} 
				}
				setState(1606);
				_errHandler.sync(this);
				_alt = getInterpreter().adaptivePredict(_input,172,_ctx);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Fixed_parameterContext extends ParserRuleContext {
		public Arg_declarationContext arg_declaration() {
			return getRuleContext(Arg_declarationContext.class,0);
		}
		public AttributesContext attributes() {
			return getRuleContext(AttributesContext.class,0);
		}
		public Parameter_modifierContext parameter_modifier() {
			return getRuleContext(Parameter_modifierContext.class,0);
		}
		public TerminalNode ARGLIST() { return getToken(CSharpParser.ARGLIST, 0); }
		public Fixed_parameterContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_fixed_parameter; }
	}

	public final Fixed_parameterContext fixed_parameter() throws RecognitionException {
		Fixed_parameterContext _localctx = new Fixed_parameterContext(_ctx, getState());
		enterRule(_localctx, 248, RULE_fixed_parameter);
		int _la;
		try {
			setState(1615);
			_errHandler.sync(this);
			switch ( getInterpreter().adaptivePredict(_input,175,_ctx) ) {
			case 1:
				enterOuterAlt(_localctx, 1);
				{
				setState(1608);
				_errHandler.sync(this);
				_la = _input.LA(1);
				if (_la==OPEN_BRACKET) {
					{
					setState(1607);
					attributes();
					}
				}

				setState(1611);
				_errHandler.sync(this);
				_la = _input.LA(1);
				if (((((_la - 71)) & ~0x3f) == 0 && ((1L << (_la - 71)) & ((1L << (OUT - 71)) | (1L << (REF - 71)) | (1L << (THIS - 71)))) != 0)) {
					{
					setState(1610);
					parameter_modifier();
					}
				}

				setState(1613);
				arg_declaration();
				}
				break;
			case 2:
				enterOuterAlt(_localctx, 2);
				{
				setState(1614);
				match(ARGLIST);
				}
				break;
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Parameter_modifierContext extends ParserRuleContext {
		public TerminalNode REF() { return getToken(CSharpParser.REF, 0); }
		public TerminalNode OUT() { return getToken(CSharpParser.OUT, 0); }
		public TerminalNode THIS() { return getToken(CSharpParser.THIS, 0); }
		public Parameter_modifierContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_parameter_modifier; }
	}

	public final Parameter_modifierContext parameter_modifier() throws RecognitionException {
		Parameter_modifierContext _localctx = new Parameter_modifierContext(_ctx, getState());
		enterRule(_localctx, 250, RULE_parameter_modifier);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(1617);
			_la = _input.LA(1);
			if ( !(((((_la - 71)) & ~0x3f) == 0 && ((1L << (_la - 71)) & ((1L << (OUT - 71)) | (1L << (REF - 71)) | (1L << (THIS - 71)))) != 0)) ) {
			_errHandler.recoverInline(this);
			}
			else {
				if ( _input.LA(1)==Token.EOF ) matchedEOF = true;
				_errHandler.reportMatch(this);
				consume();
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Parameter_arrayContext extends ParserRuleContext {
		public TerminalNode PARAMS() { return getToken(CSharpParser.PARAMS, 0); }
		public Array_typeContext array_type() {
			return getRuleContext(Array_typeContext.class,0);
		}
		public IdentifierContext identifier() {
			return getRuleContext(IdentifierContext.class,0);
		}
		public AttributesContext attributes() {
			return getRuleContext(AttributesContext.class,0);
		}
		public Parameter_arrayContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_parameter_array; }
	}

	public final Parameter_arrayContext parameter_array() throws RecognitionException {
		Parameter_arrayContext _localctx = new Parameter_arrayContext(_ctx, getState());
		enterRule(_localctx, 252, RULE_parameter_array);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(1620);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==OPEN_BRACKET) {
				{
				setState(1619);
				attributes();
				}
			}

			setState(1622);
			match(PARAMS);
			setState(1623);
			array_type();
			setState(1624);
			identifier();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Accessor_declarationsContext extends ParserRuleContext {
		public AttributesContext attrs;
		public Accessor_modifierContext mods;
		public TerminalNode GET() { return getToken(CSharpParser.GET, 0); }
		public Accessor_bodyContext accessor_body() {
			return getRuleContext(Accessor_bodyContext.class,0);
		}
		public TerminalNode SET() { return getToken(CSharpParser.SET, 0); }
		public AttributesContext attributes() {
			return getRuleContext(AttributesContext.class,0);
		}
		public Accessor_modifierContext accessor_modifier() {
			return getRuleContext(Accessor_modifierContext.class,0);
		}
		public Set_accessor_declarationContext set_accessor_declaration() {
			return getRuleContext(Set_accessor_declarationContext.class,0);
		}
		public Get_accessor_declarationContext get_accessor_declaration() {
			return getRuleContext(Get_accessor_declarationContext.class,0);
		}
		public Accessor_declarationsContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_accessor_declarations; }
	}

	public final Accessor_declarationsContext accessor_declarations() throws RecognitionException {
		Accessor_declarationsContext _localctx = new Accessor_declarationsContext(_ctx, getState());
		enterRule(_localctx, 254, RULE_accessor_declarations);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(1627);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==OPEN_BRACKET) {
				{
				setState(1626);
				((Accessor_declarationsContext)_localctx).attrs = attributes();
				}
			}

			setState(1630);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (((((_la - 56)) & ~0x3f) == 0 && ((1L << (_la - 56)) & ((1L << (INTERNAL - 56)) | (1L << (PRIVATE - 56)) | (1L << (PROTECTED - 56)))) != 0)) {
				{
				setState(1629);
				((Accessor_declarationsContext)_localctx).mods = accessor_modifier();
				}
			}

			setState(1642);
			_errHandler.sync(this);
			switch (_input.LA(1)) {
			case GET:
				{
				setState(1632);
				match(GET);
				setState(1633);
				accessor_body();
				setState(1635);
				_errHandler.sync(this);
				_la = _input.LA(1);
				if (_la==INTERNAL || ((((_la - 75)) & ~0x3f) == 0 && ((1L << (_la - 75)) & ((1L << (PRIVATE - 75)) | (1L << (PROTECTED - 75)) | (1L << (SET - 75)) | (1L << (OPEN_BRACKET - 75)))) != 0)) {
					{
					setState(1634);
					set_accessor_declaration();
					}
				}

				}
				break;
			case SET:
				{
				setState(1637);
				match(SET);
				setState(1638);
				accessor_body();
				setState(1640);
				_errHandler.sync(this);
				_la = _input.LA(1);
				if (_la==GET || _la==INTERNAL || ((((_la - 75)) & ~0x3f) == 0 && ((1L << (_la - 75)) & ((1L << (PRIVATE - 75)) | (1L << (PROTECTED - 75)) | (1L << (OPEN_BRACKET - 75)))) != 0)) {
					{
					setState(1639);
					get_accessor_declaration();
					}
				}

				}
				break;
			default:
				throw new NoViableAltException(this);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Get_accessor_declarationContext extends ParserRuleContext {
		public TerminalNode GET() { return getToken(CSharpParser.GET, 0); }
		public Accessor_bodyContext accessor_body() {
			return getRuleContext(Accessor_bodyContext.class,0);
		}
		public AttributesContext attributes() {
			return getRuleContext(AttributesContext.class,0);
		}
		public Accessor_modifierContext accessor_modifier() {
			return getRuleContext(Accessor_modifierContext.class,0);
		}
		public Get_accessor_declarationContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_get_accessor_declaration; }
	}

	public final Get_accessor_declarationContext get_accessor_declaration() throws RecognitionException {
		Get_accessor_declarationContext _localctx = new Get_accessor_declarationContext(_ctx, getState());
		enterRule(_localctx, 256, RULE_get_accessor_declaration);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(1645);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==OPEN_BRACKET) {
				{
				setState(1644);
				attributes();
				}
			}

			setState(1648);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (((((_la - 56)) & ~0x3f) == 0 && ((1L << (_la - 56)) & ((1L << (INTERNAL - 56)) | (1L << (PRIVATE - 56)) | (1L << (PROTECTED - 56)))) != 0)) {
				{
				setState(1647);
				accessor_modifier();
				}
			}

			setState(1650);
			match(GET);
			setState(1651);
			accessor_body();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Set_accessor_declarationContext extends ParserRuleContext {
		public TerminalNode SET() { return getToken(CSharpParser.SET, 0); }
		public Accessor_bodyContext accessor_body() {
			return getRuleContext(Accessor_bodyContext.class,0);
		}
		public AttributesContext attributes() {
			return getRuleContext(AttributesContext.class,0);
		}
		public Accessor_modifierContext accessor_modifier() {
			return getRuleContext(Accessor_modifierContext.class,0);
		}
		public Set_accessor_declarationContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_set_accessor_declaration; }
	}

	public final Set_accessor_declarationContext set_accessor_declaration() throws RecognitionException {
		Set_accessor_declarationContext _localctx = new Set_accessor_declarationContext(_ctx, getState());
		enterRule(_localctx, 258, RULE_set_accessor_declaration);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(1654);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==OPEN_BRACKET) {
				{
				setState(1653);
				attributes();
				}
			}

			setState(1657);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (((((_la - 56)) & ~0x3f) == 0 && ((1L << (_la - 56)) & ((1L << (INTERNAL - 56)) | (1L << (PRIVATE - 56)) | (1L << (PROTECTED - 56)))) != 0)) {
				{
				setState(1656);
				accessor_modifier();
				}
			}

			setState(1659);
			match(SET);
			setState(1660);
			accessor_body();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Accessor_modifierContext extends ParserRuleContext {
		public TerminalNode PROTECTED() { return getToken(CSharpParser.PROTECTED, 0); }
		public TerminalNode INTERNAL() { return getToken(CSharpParser.INTERNAL, 0); }
		public TerminalNode PRIVATE() { return getToken(CSharpParser.PRIVATE, 0); }
		public Accessor_modifierContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_accessor_modifier; }
	}

	public final Accessor_modifierContext accessor_modifier() throws RecognitionException {
		Accessor_modifierContext _localctx = new Accessor_modifierContext(_ctx, getState());
		enterRule(_localctx, 260, RULE_accessor_modifier);
		try {
			setState(1669);
			_errHandler.sync(this);
			switch ( getInterpreter().adaptivePredict(_input,186,_ctx) ) {
			case 1:
				enterOuterAlt(_localctx, 1);
				{
				setState(1662);
				match(PROTECTED);
				}
				break;
			case 2:
				enterOuterAlt(_localctx, 2);
				{
				setState(1663);
				match(INTERNAL);
				}
				break;
			case 3:
				enterOuterAlt(_localctx, 3);
				{
				setState(1664);
				match(PRIVATE);
				}
				break;
			case 4:
				enterOuterAlt(_localctx, 4);
				{
				setState(1665);
				match(PROTECTED);
				setState(1666);
				match(INTERNAL);
				}
				break;
			case 5:
				enterOuterAlt(_localctx, 5);
				{
				setState(1667);
				match(INTERNAL);
				setState(1668);
				match(PROTECTED);
				}
				break;
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Accessor_bodyContext extends ParserRuleContext {
		public BlockContext block() {
			return getRuleContext(BlockContext.class,0);
		}
		public Accessor_bodyContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_accessor_body; }
	}

	public final Accessor_bodyContext accessor_body() throws RecognitionException {
		Accessor_bodyContext _localctx = new Accessor_bodyContext(_ctx, getState());
		enterRule(_localctx, 262, RULE_accessor_body);
		try {
			setState(1673);
			_errHandler.sync(this);
			switch (_input.LA(1)) {
			case OPEN_BRACE:
				enterOuterAlt(_localctx, 1);
				{
				setState(1671);
				block();
				}
				break;
			case SEMICOLON:
				enterOuterAlt(_localctx, 2);
				{
				setState(1672);
				match(SEMICOLON);
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Event_accessor_declarationsContext extends ParserRuleContext {
		public TerminalNode ADD() { return getToken(CSharpParser.ADD, 0); }
		public BlockContext block() {
			return getRuleContext(BlockContext.class,0);
		}
		public Remove_accessor_declarationContext remove_accessor_declaration() {
			return getRuleContext(Remove_accessor_declarationContext.class,0);
		}
		public TerminalNode REMOVE() { return getToken(CSharpParser.REMOVE, 0); }
		public Add_accessor_declarationContext add_accessor_declaration() {
			return getRuleContext(Add_accessor_declarationContext.class,0);
		}
		public AttributesContext attributes() {
			return getRuleContext(AttributesContext.class,0);
		}
		public Event_accessor_declarationsContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_event_accessor_declarations; }
	}

	public final Event_accessor_declarationsContext event_accessor_declarations() throws RecognitionException {
		Event_accessor_declarationsContext _localctx = new Event_accessor_declarationsContext(_ctx, getState());
		enterRule(_localctx, 264, RULE_event_accessor_declarations);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(1676);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==OPEN_BRACKET) {
				{
				setState(1675);
				attributes();
				}
			}

			setState(1686);
			_errHandler.sync(this);
			switch (_input.LA(1)) {
			case ADD:
				{
				setState(1678);
				match(ADD);
				setState(1679);
				block();
				setState(1680);
				remove_accessor_declaration();
				}
				break;
			case REMOVE:
				{
				setState(1682);
				match(REMOVE);
				setState(1683);
				block();
				setState(1684);
				add_accessor_declaration();
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Add_accessor_declarationContext extends ParserRuleContext {
		public TerminalNode ADD() { return getToken(CSharpParser.ADD, 0); }
		public BlockContext block() {
			return getRuleContext(BlockContext.class,0);
		}
		public AttributesContext attributes() {
			return getRuleContext(AttributesContext.class,0);
		}
		public Add_accessor_declarationContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_add_accessor_declaration; }
	}

	public final Add_accessor_declarationContext add_accessor_declaration() throws RecognitionException {
		Add_accessor_declarationContext _localctx = new Add_accessor_declarationContext(_ctx, getState());
		enterRule(_localctx, 266, RULE_add_accessor_declaration);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(1689);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==OPEN_BRACKET) {
				{
				setState(1688);
				attributes();
				}
			}

			setState(1691);
			match(ADD);
			setState(1692);
			block();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Remove_accessor_declarationContext extends ParserRuleContext {
		public TerminalNode REMOVE() { return getToken(CSharpParser.REMOVE, 0); }
		public BlockContext block() {
			return getRuleContext(BlockContext.class,0);
		}
		public AttributesContext attributes() {
			return getRuleContext(AttributesContext.class,0);
		}
		public Remove_accessor_declarationContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_remove_accessor_declaration; }
	}

	public final Remove_accessor_declarationContext remove_accessor_declaration() throws RecognitionException {
		Remove_accessor_declarationContext _localctx = new Remove_accessor_declarationContext(_ctx, getState());
		enterRule(_localctx, 268, RULE_remove_accessor_declaration);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(1695);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==OPEN_BRACKET) {
				{
				setState(1694);
				attributes();
				}
			}

			setState(1697);
			match(REMOVE);
			setState(1698);
			block();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Overloadable_operatorContext extends ParserRuleContext {
		public TerminalNode BANG() { return getToken(CSharpParser.BANG, 0); }
		public TerminalNode TRUE() { return getToken(CSharpParser.TRUE, 0); }
		public TerminalNode FALSE() { return getToken(CSharpParser.FALSE, 0); }
		public Right_shiftContext right_shift() {
			return getRuleContext(Right_shiftContext.class,0);
		}
		public TerminalNode OP_EQ() { return getToken(CSharpParser.OP_EQ, 0); }
		public TerminalNode OP_NE() { return getToken(CSharpParser.OP_NE, 0); }
		public Overloadable_operatorContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_overloadable_operator; }
	}

	public final Overloadable_operatorContext overloadable_operator() throws RecognitionException {
		Overloadable_operatorContext _localctx = new Overloadable_operatorContext(_ctx, getState());
		enterRule(_localctx, 270, RULE_overloadable_operator);
		try {
			setState(1722);
			_errHandler.sync(this);
			switch ( getInterpreter().adaptivePredict(_input,192,_ctx) ) {
			case 1:
				enterOuterAlt(_localctx, 1);
				{
				setState(1700);
				match(PLUS);
				}
				break;
			case 2:
				enterOuterAlt(_localctx, 2);
				{
				setState(1701);
				match(MINUS);
				}
				break;
			case 3:
				enterOuterAlt(_localctx, 3);
				{
				setState(1702);
				match(BANG);
				}
				break;
			case 4:
				enterOuterAlt(_localctx, 4);
				{
				setState(1703);
				match(TILDE);
				}
				break;
			case 5:
				enterOuterAlt(_localctx, 5);
				{
				setState(1704);
				match(OP_INC);
				}
				break;
			case 6:
				enterOuterAlt(_localctx, 6);
				{
				setState(1705);
				match(OP_DEC);
				}
				break;
			case 7:
				enterOuterAlt(_localctx, 7);
				{
				setState(1706);
				match(TRUE);
				}
				break;
			case 8:
				enterOuterAlt(_localctx, 8);
				{
				setState(1707);
				match(FALSE);
				}
				break;
			case 9:
				enterOuterAlt(_localctx, 9);
				{
				setState(1708);
				match(STAR);
				}
				break;
			case 10:
				enterOuterAlt(_localctx, 10);
				{
				setState(1709);
				match(DIV);
				}
				break;
			case 11:
				enterOuterAlt(_localctx, 11);
				{
				setState(1710);
				match(PERCENT);
				}
				break;
			case 12:
				enterOuterAlt(_localctx, 12);
				{
				setState(1711);
				match(AMP);
				}
				break;
			case 13:
				enterOuterAlt(_localctx, 13);
				{
				setState(1712);
				match(BITWISE_OR);
				}
				break;
			case 14:
				enterOuterAlt(_localctx, 14);
				{
				setState(1713);
				match(CARET);
				}
				break;
			case 15:
				enterOuterAlt(_localctx, 15);
				{
				setState(1714);
				match(OP_LEFT_SHIFT);
				}
				break;
			case 16:
				enterOuterAlt(_localctx, 16);
				{
				setState(1715);
				right_shift();
				}
				break;
			case 17:
				enterOuterAlt(_localctx, 17);
				{
				setState(1716);
				match(OP_EQ);
				}
				break;
			case 18:
				enterOuterAlt(_localctx, 18);
				{
				setState(1717);
				match(OP_NE);
				}
				break;
			case 19:
				enterOuterAlt(_localctx, 19);
				{
				setState(1718);
				match(GT);
				}
				break;
			case 20:
				enterOuterAlt(_localctx, 20);
				{
				setState(1719);
				match(LT);
				}
				break;
			case 21:
				enterOuterAlt(_localctx, 21);
				{
				setState(1720);
				match(OP_GE);
				}
				break;
			case 22:
				enterOuterAlt(_localctx, 22);
				{
				setState(1721);
				match(OP_LE);
				}
				break;
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Conversion_operator_declaratorContext extends ParserRuleContext {
		public TerminalNode OPERATOR() { return getToken(CSharpParser.OPERATOR, 0); }
		public TypeContext type() {
			return getRuleContext(TypeContext.class,0);
		}
		public TerminalNode OPEN_PARENS() { return getToken(CSharpParser.OPEN_PARENS, 0); }
		public Arg_declarationContext arg_declaration() {
			return getRuleContext(Arg_declarationContext.class,0);
		}
		public TerminalNode CLOSE_PARENS() { return getToken(CSharpParser.CLOSE_PARENS, 0); }
		public TerminalNode IMPLICIT() { return getToken(CSharpParser.IMPLICIT, 0); }
		public TerminalNode EXPLICIT() { return getToken(CSharpParser.EXPLICIT, 0); }
		public Conversion_operator_declaratorContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_conversion_operator_declarator; }
	}

	public final Conversion_operator_declaratorContext conversion_operator_declarator() throws RecognitionException {
		Conversion_operator_declaratorContext _localctx = new Conversion_operator_declaratorContext(_ctx, getState());
		enterRule(_localctx, 272, RULE_conversion_operator_declarator);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(1724);
			_la = _input.LA(1);
			if ( !(_la==EXPLICIT || _la==IMPLICIT) ) {
			_errHandler.recoverInline(this);
			}
			else {
				if ( _input.LA(1)==Token.EOF ) matchedEOF = true;
				_errHandler.reportMatch(this);
				consume();
			}
			setState(1725);
			match(OPERATOR);
			setState(1726);
			type();
			setState(1727);
			match(OPEN_PARENS);
			setState(1728);
			arg_declaration();
			setState(1729);
			match(CLOSE_PARENS);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Constructor_initializerContext extends ParserRuleContext {
		public TerminalNode OPEN_PARENS() { return getToken(CSharpParser.OPEN_PARENS, 0); }
		public TerminalNode CLOSE_PARENS() { return getToken(CSharpParser.CLOSE_PARENS, 0); }
		public TerminalNode BASE() { return getToken(CSharpParser.BASE, 0); }
		public TerminalNode THIS() { return getToken(CSharpParser.THIS, 0); }
		public Argument_listContext argument_list() {
			return getRuleContext(Argument_listContext.class,0);
		}
		public Constructor_initializerContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_constructor_initializer; }
	}

	public final Constructor_initializerContext constructor_initializer() throws RecognitionException {
		Constructor_initializerContext _localctx = new Constructor_initializerContext(_ctx, getState());
		enterRule(_localctx, 274, RULE_constructor_initializer);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(1731);
			match(COLON);
			setState(1732);
			_la = _input.LA(1);
			if ( !(_la==BASE || _la==THIS) ) {
			_errHandler.recoverInline(this);
			}
			else {
				if ( _input.LA(1)==Token.EOF ) matchedEOF = true;
				_errHandler.reportMatch(this);
				consume();
			}
			setState(1733);
			match(OPEN_PARENS);
			setState(1735);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if ((((_la) & ~0x3f) == 0 && ((1L << _la) & ((1L << ADD) | (1L << ALIAS) | (1L << ARGLIST) | (1L << ASCENDING) | (1L << ASYNC) | (1L << AWAIT) | (1L << BASE) | (1L << BOOL) | (1L << BY) | (1L << BYTE) | (1L << CHAR) | (1L << CHECKED) | (1L << DECIMAL) | (1L << DEFAULT) | (1L << DELEGATE) | (1L << DESCENDING) | (1L << DOUBLE) | (1L << DYNAMIC) | (1L << EQUALS) | (1L << FALSE) | (1L << FLOAT) | (1L << FROM) | (1L << GET) | (1L << GROUP) | (1L << INT) | (1L << INTO) | (1L << JOIN) | (1L << LET) | (1L << LONG) | (1L << NAMEOF))) != 0) || ((((_la - 65)) & ~0x3f) == 0 && ((1L << (_la - 65)) & ((1L << (NEW - 65)) | (1L << (NULL - 65)) | (1L << (OBJECT - 65)) | (1L << (ON - 65)) | (1L << (ORDERBY - 65)) | (1L << (OUT - 65)) | (1L << (PARTIAL - 65)) | (1L << (REF - 65)) | (1L << (REMOVE - 65)) | (1L << (SBYTE - 65)) | (1L << (SELECT - 65)) | (1L << (SET - 65)) | (1L << (SHORT - 65)) | (1L << (SIZEOF - 65)) | (1L << (STRING - 65)) | (1L << (THIS - 65)) | (1L << (TRUE - 65)) | (1L << (TYPEOF - 65)) | (1L << (UINT - 65)) | (1L << (ULONG - 65)) | (1L << (UNCHECKED - 65)) | (1L << (USHORT - 65)) | (1L << (WHEN - 65)) | (1L << (WHERE - 65)) | (1L << (YIELD - 65)) | (1L << (IDENTIFIER - 65)) | (1L << (LITERAL_ACCESS - 65)) | (1L << (INTEGER_LITERAL - 65)) | (1L << (HEX_INTEGER_LITERAL - 65)) | (1L << (REAL_LITERAL - 65)) | (1L << (CHARACTER_LITERAL - 65)) | (1L << (REGULAR_STRING - 65)) | (1L << (VERBATIUM_STRING - 65)) | (1L << (INTERPOLATED_REGULAR_STRING_START - 65)) | (1L << (INTERPOLATED_VERBATIUM_STRING_START - 65)) | (1L << (OPEN_PARENS - 65)))) != 0) || ((((_la - 131)) & ~0x3f) == 0 && ((1L << (_la - 131)) & ((1L << (PLUS - 131)) | (1L << (MINUS - 131)) | (1L << (STAR - 131)) | (1L << (AMP - 131)) | (1L << (BANG - 131)) | (1L << (TILDE - 131)) | (1L << (OP_INC - 131)) | (1L << (OP_DEC - 131)))) != 0)) {
				{
				setState(1734);
				argument_list();
				}
			}

			setState(1737);
			match(CLOSE_PARENS);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class BodyContext extends ParserRuleContext {
		public BlockContext block() {
			return getRuleContext(BlockContext.class,0);
		}
		public BodyContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_body; }
	}

	public final BodyContext body() throws RecognitionException {
		BodyContext _localctx = new BodyContext(_ctx, getState());
		enterRule(_localctx, 276, RULE_body);
		try {
			setState(1741);
			_errHandler.sync(this);
			switch (_input.LA(1)) {
			case OPEN_BRACE:
				enterOuterAlt(_localctx, 1);
				{
				setState(1739);
				block();
				}
				break;
			case SEMICOLON:
				enterOuterAlt(_localctx, 2);
				{
				setState(1740);
				match(SEMICOLON);
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Struct_interfacesContext extends ParserRuleContext {
		public Interface_type_listContext interface_type_list() {
			return getRuleContext(Interface_type_listContext.class,0);
		}
		public Struct_interfacesContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_struct_interfaces; }
	}

	public final Struct_interfacesContext struct_interfaces() throws RecognitionException {
		Struct_interfacesContext _localctx = new Struct_interfacesContext(_ctx, getState());
		enterRule(_localctx, 278, RULE_struct_interfaces);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(1743);
			match(COLON);
			setState(1744);
			interface_type_list();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Struct_bodyContext extends ParserRuleContext {
		public TerminalNode OPEN_BRACE() { return getToken(CSharpParser.OPEN_BRACE, 0); }
		public TerminalNode CLOSE_BRACE() { return getToken(CSharpParser.CLOSE_BRACE, 0); }
		public List<Struct_member_declarationContext> struct_member_declaration() {
			return getRuleContexts(Struct_member_declarationContext.class);
		}
		public Struct_member_declarationContext struct_member_declaration(int i) {
			return getRuleContext(Struct_member_declarationContext.class,i);
		}
		public Struct_bodyContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_struct_body; }
	}

	public final Struct_bodyContext struct_body() throws RecognitionException {
		Struct_bodyContext _localctx = new Struct_bodyContext(_ctx, getState());
		enterRule(_localctx, 280, RULE_struct_body);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(1746);
			match(OPEN_BRACE);
			setState(1750);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while ((((_la) & ~0x3f) == 0 && ((1L << _la) & ((1L << ABSTRACT) | (1L << ADD) | (1L << ALIAS) | (1L << ARGLIST) | (1L << ASCENDING) | (1L << ASYNC) | (1L << AWAIT) | (1L << BOOL) | (1L << BY) | (1L << BYTE) | (1L << CHAR) | (1L << CLASS) | (1L << CONST) | (1L << DECIMAL) | (1L << DELEGATE) | (1L << DESCENDING) | (1L << DOUBLE) | (1L << DYNAMIC) | (1L << ENUM) | (1L << EQUALS) | (1L << EVENT) | (1L << EXPLICIT) | (1L << EXTERN) | (1L << FIXED) | (1L << FLOAT) | (1L << FROM) | (1L << GET) | (1L << GROUP) | (1L << IMPLICIT) | (1L << INT) | (1L << INTERFACE) | (1L << INTERNAL) | (1L << INTO) | (1L << JOIN) | (1L << LET) | (1L << LONG) | (1L << NAMEOF))) != 0) || ((((_la - 65)) & ~0x3f) == 0 && ((1L << (_la - 65)) & ((1L << (NEW - 65)) | (1L << (OBJECT - 65)) | (1L << (ON - 65)) | (1L << (ORDERBY - 65)) | (1L << (OVERRIDE - 65)) | (1L << (PARTIAL - 65)) | (1L << (PRIVATE - 65)) | (1L << (PROTECTED - 65)) | (1L << (PUBLIC - 65)) | (1L << (READONLY - 65)) | (1L << (REMOVE - 65)) | (1L << (SBYTE - 65)) | (1L << (SEALED - 65)) | (1L << (SELECT - 65)) | (1L << (SET - 65)) | (1L << (SHORT - 65)) | (1L << (STATIC - 65)) | (1L << (STRING - 65)) | (1L << (STRUCT - 65)) | (1L << (UINT - 65)) | (1L << (ULONG - 65)) | (1L << (UNSAFE - 65)) | (1L << (USHORT - 65)) | (1L << (VIRTUAL - 65)) | (1L << (VOID - 65)) | (1L << (VOLATILE - 65)) | (1L << (WHEN - 65)) | (1L << (WHERE - 65)) | (1L << (YIELD - 65)) | (1L << (IDENTIFIER - 65)) | (1L << (OPEN_BRACKET - 65)))) != 0)) {
				{
				{
				setState(1747);
				struct_member_declaration();
				}
				}
				setState(1752);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			setState(1753);
			match(CLOSE_BRACE);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Struct_member_declarationContext extends ParserRuleContext {
		public Common_member_declarationContext common_member_declaration() {
			return getRuleContext(Common_member_declarationContext.class,0);
		}
		public TerminalNode FIXED() { return getToken(CSharpParser.FIXED, 0); }
		public TypeContext type() {
			return getRuleContext(TypeContext.class,0);
		}
		public AttributesContext attributes() {
			return getRuleContext(AttributesContext.class,0);
		}
		public All_member_modifiersContext all_member_modifiers() {
			return getRuleContext(All_member_modifiersContext.class,0);
		}
		public List<Fixed_size_buffer_declaratorContext> fixed_size_buffer_declarator() {
			return getRuleContexts(Fixed_size_buffer_declaratorContext.class);
		}
		public Fixed_size_buffer_declaratorContext fixed_size_buffer_declarator(int i) {
			return getRuleContext(Fixed_size_buffer_declaratorContext.class,i);
		}
		public Struct_member_declarationContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_struct_member_declaration; }
	}

	public final Struct_member_declarationContext struct_member_declaration() throws RecognitionException {
		Struct_member_declarationContext _localctx = new Struct_member_declarationContext(_ctx, getState());
		enterRule(_localctx, 282, RULE_struct_member_declaration);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(1756);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==OPEN_BRACKET) {
				{
				setState(1755);
				attributes();
				}
			}

			setState(1759);
			_errHandler.sync(this);
			switch ( getInterpreter().adaptivePredict(_input,197,_ctx) ) {
			case 1:
				{
				setState(1758);
				all_member_modifiers();
				}
				break;
			}
			setState(1771);
			_errHandler.sync(this);
			switch (_input.LA(1)) {
			case ADD:
			case ALIAS:
			case ARGLIST:
			case ASCENDING:
			case ASYNC:
			case AWAIT:
			case BOOL:
			case BY:
			case BYTE:
			case CHAR:
			case CLASS:
			case CONST:
			case DECIMAL:
			case DELEGATE:
			case DESCENDING:
			case DOUBLE:
			case DYNAMIC:
			case ENUM:
			case EQUALS:
			case EVENT:
			case EXPLICIT:
			case FLOAT:
			case FROM:
			case GET:
			case GROUP:
			case IMPLICIT:
			case INT:
			case INTERFACE:
			case INTO:
			case JOIN:
			case LET:
			case LONG:
			case NAMEOF:
			case OBJECT:
			case ON:
			case ORDERBY:
			case PARTIAL:
			case REMOVE:
			case SBYTE:
			case SELECT:
			case SET:
			case SHORT:
			case STRING:
			case STRUCT:
			case UINT:
			case ULONG:
			case USHORT:
			case VOID:
			case WHEN:
			case WHERE:
			case YIELD:
			case IDENTIFIER:
				{
				setState(1761);
				common_member_declaration();
				}
				break;
			case FIXED:
				{
				setState(1762);
				match(FIXED);
				setState(1763);
				type();
				setState(1765); 
				_errHandler.sync(this);
				_la = _input.LA(1);
				do {
					{
					{
					setState(1764);
					fixed_size_buffer_declarator();
					}
					}
					setState(1767); 
					_errHandler.sync(this);
					_la = _input.LA(1);
				} while ( (((_la) & ~0x3f) == 0 && ((1L << _la) & ((1L << ADD) | (1L << ALIAS) | (1L << ARGLIST) | (1L << ASCENDING) | (1L << ASYNC) | (1L << AWAIT) | (1L << BY) | (1L << DESCENDING) | (1L << DYNAMIC) | (1L << EQUALS) | (1L << FROM) | (1L << GET) | (1L << GROUP) | (1L << INTO) | (1L << JOIN) | (1L << LET) | (1L << NAMEOF))) != 0) || ((((_la - 68)) & ~0x3f) == 0 && ((1L << (_la - 68)) & ((1L << (ON - 68)) | (1L << (ORDERBY - 68)) | (1L << (PARTIAL - 68)) | (1L << (REMOVE - 68)) | (1L << (SELECT - 68)) | (1L << (SET - 68)) | (1L << (WHEN - 68)) | (1L << (WHERE - 68)) | (1L << (YIELD - 68)) | (1L << (IDENTIFIER - 68)))) != 0) );
				setState(1769);
				match(SEMICOLON);
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Array_typeContext extends ParserRuleContext {
		public Base_typeContext base_type() {
			return getRuleContext(Base_typeContext.class,0);
		}
		public List<Rank_specifierContext> rank_specifier() {
			return getRuleContexts(Rank_specifierContext.class);
		}
		public Rank_specifierContext rank_specifier(int i) {
			return getRuleContext(Rank_specifierContext.class,i);
		}
		public Array_typeContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_array_type; }
	}

	public final Array_typeContext array_type() throws RecognitionException {
		Array_typeContext _localctx = new Array_typeContext(_ctx, getState());
		enterRule(_localctx, 284, RULE_array_type);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(1773);
			base_type();
			setState(1781); 
			_errHandler.sync(this);
			_la = _input.LA(1);
			do {
				{
				{
				setState(1777);
				_errHandler.sync(this);
				_la = _input.LA(1);
				while (_la==STAR || _la==INTERR) {
					{
					{
					setState(1774);
					_la = _input.LA(1);
					if ( !(_la==STAR || _la==INTERR) ) {
					_errHandler.recoverInline(this);
					}
					else {
						if ( _input.LA(1)==Token.EOF ) matchedEOF = true;
						_errHandler.reportMatch(this);
						consume();
					}
					}
					}
					setState(1779);
					_errHandler.sync(this);
					_la = _input.LA(1);
				}
				setState(1780);
				rank_specifier();
				}
				}
				setState(1783); 
				_errHandler.sync(this);
				_la = _input.LA(1);
			} while ( ((((_la - 123)) & ~0x3f) == 0 && ((1L << (_la - 123)) & ((1L << (OPEN_BRACKET - 123)) | (1L << (STAR - 123)) | (1L << (INTERR - 123)))) != 0) );
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Rank_specifierContext extends ParserRuleContext {
		public Rank_specifierContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_rank_specifier; }
	}

	public final Rank_specifierContext rank_specifier() throws RecognitionException {
		Rank_specifierContext _localctx = new Rank_specifierContext(_ctx, getState());
		enterRule(_localctx, 286, RULE_rank_specifier);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(1785);
			match(OPEN_BRACKET);
			setState(1789);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while (_la==COMMA) {
				{
				{
				setState(1786);
				match(COMMA);
				}
				}
				setState(1791);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			setState(1792);
			match(CLOSE_BRACKET);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Array_initializerContext extends ParserRuleContext {
		public TerminalNode OPEN_BRACE() { return getToken(CSharpParser.OPEN_BRACE, 0); }
		public TerminalNode CLOSE_BRACE() { return getToken(CSharpParser.CLOSE_BRACE, 0); }
		public List<Variable_initializerContext> variable_initializer() {
			return getRuleContexts(Variable_initializerContext.class);
		}
		public Variable_initializerContext variable_initializer(int i) {
			return getRuleContext(Variable_initializerContext.class,i);
		}
		public Array_initializerContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_array_initializer; }
	}

	public final Array_initializerContext array_initializer() throws RecognitionException {
		Array_initializerContext _localctx = new Array_initializerContext(_ctx, getState());
		enterRule(_localctx, 288, RULE_array_initializer);
		int _la;
		try {
			int _alt;
			enterOuterAlt(_localctx, 1);
			{
			setState(1794);
			match(OPEN_BRACE);
			setState(1806);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if ((((_la) & ~0x3f) == 0 && ((1L << _la) & ((1L << ADD) | (1L << ALIAS) | (1L << ARGLIST) | (1L << ASCENDING) | (1L << ASYNC) | (1L << AWAIT) | (1L << BASE) | (1L << BOOL) | (1L << BY) | (1L << BYTE) | (1L << CHAR) | (1L << CHECKED) | (1L << DECIMAL) | (1L << DEFAULT) | (1L << DELEGATE) | (1L << DESCENDING) | (1L << DOUBLE) | (1L << DYNAMIC) | (1L << EQUALS) | (1L << FALSE) | (1L << FLOAT) | (1L << FROM) | (1L << GET) | (1L << GROUP) | (1L << INT) | (1L << INTO) | (1L << JOIN) | (1L << LET) | (1L << LONG) | (1L << NAMEOF))) != 0) || ((((_la - 65)) & ~0x3f) == 0 && ((1L << (_la - 65)) & ((1L << (NEW - 65)) | (1L << (NULL - 65)) | (1L << (OBJECT - 65)) | (1L << (ON - 65)) | (1L << (ORDERBY - 65)) | (1L << (PARTIAL - 65)) | (1L << (REMOVE - 65)) | (1L << (SBYTE - 65)) | (1L << (SELECT - 65)) | (1L << (SET - 65)) | (1L << (SHORT - 65)) | (1L << (SIZEOF - 65)) | (1L << (STRING - 65)) | (1L << (THIS - 65)) | (1L << (TRUE - 65)) | (1L << (TYPEOF - 65)) | (1L << (UINT - 65)) | (1L << (ULONG - 65)) | (1L << (UNCHECKED - 65)) | (1L << (USHORT - 65)) | (1L << (WHEN - 65)) | (1L << (WHERE - 65)) | (1L << (YIELD - 65)) | (1L << (IDENTIFIER - 65)) | (1L << (LITERAL_ACCESS - 65)) | (1L << (INTEGER_LITERAL - 65)) | (1L << (HEX_INTEGER_LITERAL - 65)) | (1L << (REAL_LITERAL - 65)) | (1L << (CHARACTER_LITERAL - 65)) | (1L << (REGULAR_STRING - 65)) | (1L << (VERBATIUM_STRING - 65)) | (1L << (INTERPOLATED_REGULAR_STRING_START - 65)) | (1L << (INTERPOLATED_VERBATIUM_STRING_START - 65)) | (1L << (OPEN_BRACE - 65)) | (1L << (OPEN_PARENS - 65)))) != 0) || ((((_la - 131)) & ~0x3f) == 0 && ((1L << (_la - 131)) & ((1L << (PLUS - 131)) | (1L << (MINUS - 131)) | (1L << (STAR - 131)) | (1L << (AMP - 131)) | (1L << (BANG - 131)) | (1L << (TILDE - 131)) | (1L << (OP_INC - 131)) | (1L << (OP_DEC - 131)))) != 0)) {
				{
				setState(1795);
				variable_initializer();
				setState(1800);
				_errHandler.sync(this);
				_alt = getInterpreter().adaptivePredict(_input,203,_ctx);
				while ( _alt!=2 && _alt!=org.antlr.v4.runtime.atn.ATN.INVALID_ALT_NUMBER ) {
					if ( _alt==1 ) {
						{
						{
						setState(1796);
						match(COMMA);
						setState(1797);
						variable_initializer();
						}
						} 
					}
					setState(1802);
					_errHandler.sync(this);
					_alt = getInterpreter().adaptivePredict(_input,203,_ctx);
				}
				setState(1804);
				_errHandler.sync(this);
				_la = _input.LA(1);
				if (_la==COMMA) {
					{
					setState(1803);
					match(COMMA);
					}
				}

				}
			}

			setState(1808);
			match(CLOSE_BRACE);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Variant_type_parameter_listContext extends ParserRuleContext {
		public List<Variant_type_parameterContext> variant_type_parameter() {
			return getRuleContexts(Variant_type_parameterContext.class);
		}
		public Variant_type_parameterContext variant_type_parameter(int i) {
			return getRuleContext(Variant_type_parameterContext.class,i);
		}
		public Variant_type_parameter_listContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_variant_type_parameter_list; }
	}

	public final Variant_type_parameter_listContext variant_type_parameter_list() throws RecognitionException {
		Variant_type_parameter_listContext _localctx = new Variant_type_parameter_listContext(_ctx, getState());
		enterRule(_localctx, 290, RULE_variant_type_parameter_list);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(1810);
			match(LT);
			setState(1811);
			variant_type_parameter();
			setState(1816);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while (_la==COMMA) {
				{
				{
				setState(1812);
				match(COMMA);
				setState(1813);
				variant_type_parameter();
				}
				}
				setState(1818);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			setState(1819);
			match(GT);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Variant_type_parameterContext extends ParserRuleContext {
		public IdentifierContext identifier() {
			return getRuleContext(IdentifierContext.class,0);
		}
		public AttributesContext attributes() {
			return getRuleContext(AttributesContext.class,0);
		}
		public Variance_annotationContext variance_annotation() {
			return getRuleContext(Variance_annotationContext.class,0);
		}
		public Variant_type_parameterContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_variant_type_parameter; }
	}

	public final Variant_type_parameterContext variant_type_parameter() throws RecognitionException {
		Variant_type_parameterContext _localctx = new Variant_type_parameterContext(_ctx, getState());
		enterRule(_localctx, 292, RULE_variant_type_parameter);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(1822);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==OPEN_BRACKET) {
				{
				setState(1821);
				attributes();
				}
			}

			setState(1825);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==IN || _la==OUT) {
				{
				setState(1824);
				variance_annotation();
				}
			}

			setState(1827);
			identifier();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Variance_annotationContext extends ParserRuleContext {
		public TerminalNode IN() { return getToken(CSharpParser.IN, 0); }
		public TerminalNode OUT() { return getToken(CSharpParser.OUT, 0); }
		public Variance_annotationContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_variance_annotation; }
	}

	public final Variance_annotationContext variance_annotation() throws RecognitionException {
		Variance_annotationContext _localctx = new Variance_annotationContext(_ctx, getState());
		enterRule(_localctx, 294, RULE_variance_annotation);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(1829);
			_la = _input.LA(1);
			if ( !(_la==IN || _la==OUT) ) {
			_errHandler.recoverInline(this);
			}
			else {
				if ( _input.LA(1)==Token.EOF ) matchedEOF = true;
				_errHandler.reportMatch(this);
				consume();
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Interface_baseContext extends ParserRuleContext {
		public Interface_type_listContext interface_type_list() {
			return getRuleContext(Interface_type_listContext.class,0);
		}
		public Interface_baseContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_interface_base; }
	}

	public final Interface_baseContext interface_base() throws RecognitionException {
		Interface_baseContext _localctx = new Interface_baseContext(_ctx, getState());
		enterRule(_localctx, 296, RULE_interface_base);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(1831);
			match(COLON);
			setState(1832);
			interface_type_list();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Interface_bodyContext extends ParserRuleContext {
		public TerminalNode OPEN_BRACE() { return getToken(CSharpParser.OPEN_BRACE, 0); }
		public TerminalNode CLOSE_BRACE() { return getToken(CSharpParser.CLOSE_BRACE, 0); }
		public List<Interface_member_declarationContext> interface_member_declaration() {
			return getRuleContexts(Interface_member_declarationContext.class);
		}
		public Interface_member_declarationContext interface_member_declaration(int i) {
			return getRuleContext(Interface_member_declarationContext.class,i);
		}
		public Interface_bodyContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_interface_body; }
	}

	public final Interface_bodyContext interface_body() throws RecognitionException {
		Interface_bodyContext _localctx = new Interface_bodyContext(_ctx, getState());
		enterRule(_localctx, 298, RULE_interface_body);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(1834);
			match(OPEN_BRACE);
			setState(1838);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while ((((_la) & ~0x3f) == 0 && ((1L << _la) & ((1L << ADD) | (1L << ALIAS) | (1L << ARGLIST) | (1L << ASCENDING) | (1L << ASYNC) | (1L << AWAIT) | (1L << BOOL) | (1L << BY) | (1L << BYTE) | (1L << CHAR) | (1L << DECIMAL) | (1L << DESCENDING) | (1L << DOUBLE) | (1L << DYNAMIC) | (1L << EQUALS) | (1L << EVENT) | (1L << FLOAT) | (1L << FROM) | (1L << GET) | (1L << GROUP) | (1L << INT) | (1L << INTO) | (1L << JOIN) | (1L << LET) | (1L << LONG) | (1L << NAMEOF))) != 0) || ((((_la - 65)) & ~0x3f) == 0 && ((1L << (_la - 65)) & ((1L << (NEW - 65)) | (1L << (OBJECT - 65)) | (1L << (ON - 65)) | (1L << (ORDERBY - 65)) | (1L << (PARTIAL - 65)) | (1L << (REMOVE - 65)) | (1L << (SBYTE - 65)) | (1L << (SELECT - 65)) | (1L << (SET - 65)) | (1L << (SHORT - 65)) | (1L << (STRING - 65)) | (1L << (UINT - 65)) | (1L << (ULONG - 65)) | (1L << (UNSAFE - 65)) | (1L << (USHORT - 65)) | (1L << (VOID - 65)) | (1L << (WHEN - 65)) | (1L << (WHERE - 65)) | (1L << (YIELD - 65)) | (1L << (IDENTIFIER - 65)) | (1L << (OPEN_BRACKET - 65)))) != 0)) {
				{
				{
				setState(1835);
				interface_member_declaration();
				}
				}
				setState(1840);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			setState(1841);
			match(CLOSE_BRACE);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Interface_member_declarationContext extends ParserRuleContext {
		public TypeContext type() {
			return getRuleContext(TypeContext.class,0);
		}
		public TerminalNode VOID() { return getToken(CSharpParser.VOID, 0); }
		public IdentifierContext identifier() {
			return getRuleContext(IdentifierContext.class,0);
		}
		public TerminalNode OPEN_PARENS() { return getToken(CSharpParser.OPEN_PARENS, 0); }
		public TerminalNode CLOSE_PARENS() { return getToken(CSharpParser.CLOSE_PARENS, 0); }
		public TerminalNode EVENT() { return getToken(CSharpParser.EVENT, 0); }
		public AttributesContext attributes() {
			return getRuleContext(AttributesContext.class,0);
		}
		public TerminalNode NEW() { return getToken(CSharpParser.NEW, 0); }
		public TerminalNode OPEN_BRACE() { return getToken(CSharpParser.OPEN_BRACE, 0); }
		public Interface_accessorsContext interface_accessors() {
			return getRuleContext(Interface_accessorsContext.class,0);
		}
		public TerminalNode CLOSE_BRACE() { return getToken(CSharpParser.CLOSE_BRACE, 0); }
		public TerminalNode THIS() { return getToken(CSharpParser.THIS, 0); }
		public Formal_parameter_listContext formal_parameter_list() {
			return getRuleContext(Formal_parameter_listContext.class,0);
		}
		public TerminalNode UNSAFE() { return getToken(CSharpParser.UNSAFE, 0); }
		public Type_parameter_listContext type_parameter_list() {
			return getRuleContext(Type_parameter_listContext.class,0);
		}
		public Type_parameter_constraints_clausesContext type_parameter_constraints_clauses() {
			return getRuleContext(Type_parameter_constraints_clausesContext.class,0);
		}
		public Interface_member_declarationContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_interface_member_declaration; }
	}

	public final Interface_member_declarationContext interface_member_declaration() throws RecognitionException {
		Interface_member_declarationContext _localctx = new Interface_member_declarationContext(_ctx, getState());
		enterRule(_localctx, 300, RULE_interface_member_declaration);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(1844);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==OPEN_BRACKET) {
				{
				setState(1843);
				attributes();
				}
			}

			setState(1847);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==NEW) {
				{
				setState(1846);
				match(NEW);
				}
			}

			setState(1905);
			_errHandler.sync(this);
			switch ( getInterpreter().adaptivePredict(_input,221,_ctx) ) {
			case 1:
				{
				setState(1850);
				_errHandler.sync(this);
				_la = _input.LA(1);
				if (_la==UNSAFE) {
					{
					setState(1849);
					match(UNSAFE);
					}
				}

				setState(1852);
				type();
				setState(1880);
				_errHandler.sync(this);
				switch ( getInterpreter().adaptivePredict(_input,216,_ctx) ) {
				case 1:
					{
					setState(1853);
					identifier();
					setState(1855);
					_errHandler.sync(this);
					_la = _input.LA(1);
					if (_la==LT) {
						{
						setState(1854);
						type_parameter_list();
						}
					}

					setState(1857);
					match(OPEN_PARENS);
					setState(1859);
					_errHandler.sync(this);
					_la = _input.LA(1);
					if ((((_la) & ~0x3f) == 0 && ((1L << _la) & ((1L << ADD) | (1L << ALIAS) | (1L << ARGLIST) | (1L << ASCENDING) | (1L << ASYNC) | (1L << AWAIT) | (1L << BOOL) | (1L << BY) | (1L << BYTE) | (1L << CHAR) | (1L << DECIMAL) | (1L << DESCENDING) | (1L << DOUBLE) | (1L << DYNAMIC) | (1L << EQUALS) | (1L << FLOAT) | (1L << FROM) | (1L << GET) | (1L << GROUP) | (1L << INT) | (1L << INTO) | (1L << JOIN) | (1L << LET) | (1L << LONG) | (1L << NAMEOF))) != 0) || ((((_la - 67)) & ~0x3f) == 0 && ((1L << (_la - 67)) & ((1L << (OBJECT - 67)) | (1L << (ON - 67)) | (1L << (ORDERBY - 67)) | (1L << (OUT - 67)) | (1L << (PARAMS - 67)) | (1L << (PARTIAL - 67)) | (1L << (REF - 67)) | (1L << (REMOVE - 67)) | (1L << (SBYTE - 67)) | (1L << (SELECT - 67)) | (1L << (SET - 67)) | (1L << (SHORT - 67)) | (1L << (STRING - 67)) | (1L << (THIS - 67)) | (1L << (UINT - 67)) | (1L << (ULONG - 67)) | (1L << (USHORT - 67)) | (1L << (VOID - 67)) | (1L << (WHEN - 67)) | (1L << (WHERE - 67)) | (1L << (YIELD - 67)) | (1L << (IDENTIFIER - 67)) | (1L << (OPEN_BRACKET - 67)))) != 0)) {
						{
						setState(1858);
						formal_parameter_list();
						}
					}

					setState(1861);
					match(CLOSE_PARENS);
					setState(1863);
					_errHandler.sync(this);
					_la = _input.LA(1);
					if (_la==WHERE) {
						{
						setState(1862);
						type_parameter_constraints_clauses();
						}
					}

					setState(1865);
					match(SEMICOLON);
					}
					break;
				case 2:
					{
					setState(1867);
					identifier();
					setState(1868);
					match(OPEN_BRACE);
					setState(1869);
					interface_accessors();
					setState(1870);
					match(CLOSE_BRACE);
					}
					break;
				case 3:
					{
					setState(1872);
					match(THIS);
					setState(1873);
					match(OPEN_BRACKET);
					setState(1874);
					formal_parameter_list();
					setState(1875);
					match(CLOSE_BRACKET);
					setState(1876);
					match(OPEN_BRACE);
					setState(1877);
					interface_accessors();
					setState(1878);
					match(CLOSE_BRACE);
					}
					break;
				}
				}
				break;
			case 2:
				{
				setState(1883);
				_errHandler.sync(this);
				_la = _input.LA(1);
				if (_la==UNSAFE) {
					{
					setState(1882);
					match(UNSAFE);
					}
				}

				setState(1885);
				match(VOID);
				setState(1886);
				identifier();
				setState(1888);
				_errHandler.sync(this);
				_la = _input.LA(1);
				if (_la==LT) {
					{
					setState(1887);
					type_parameter_list();
					}
				}

				setState(1890);
				match(OPEN_PARENS);
				setState(1892);
				_errHandler.sync(this);
				_la = _input.LA(1);
				if ((((_la) & ~0x3f) == 0 && ((1L << _la) & ((1L << ADD) | (1L << ALIAS) | (1L << ARGLIST) | (1L << ASCENDING) | (1L << ASYNC) | (1L << AWAIT) | (1L << BOOL) | (1L << BY) | (1L << BYTE) | (1L << CHAR) | (1L << DECIMAL) | (1L << DESCENDING) | (1L << DOUBLE) | (1L << DYNAMIC) | (1L << EQUALS) | (1L << FLOAT) | (1L << FROM) | (1L << GET) | (1L << GROUP) | (1L << INT) | (1L << INTO) | (1L << JOIN) | (1L << LET) | (1L << LONG) | (1L << NAMEOF))) != 0) || ((((_la - 67)) & ~0x3f) == 0 && ((1L << (_la - 67)) & ((1L << (OBJECT - 67)) | (1L << (ON - 67)) | (1L << (ORDERBY - 67)) | (1L << (OUT - 67)) | (1L << (PARAMS - 67)) | (1L << (PARTIAL - 67)) | (1L << (REF - 67)) | (1L << (REMOVE - 67)) | (1L << (SBYTE - 67)) | (1L << (SELECT - 67)) | (1L << (SET - 67)) | (1L << (SHORT - 67)) | (1L << (STRING - 67)) | (1L << (THIS - 67)) | (1L << (UINT - 67)) | (1L << (ULONG - 67)) | (1L << (USHORT - 67)) | (1L << (VOID - 67)) | (1L << (WHEN - 67)) | (1L << (WHERE - 67)) | (1L << (YIELD - 67)) | (1L << (IDENTIFIER - 67)) | (1L << (OPEN_BRACKET - 67)))) != 0)) {
					{
					setState(1891);
					formal_parameter_list();
					}
				}

				setState(1894);
				match(CLOSE_PARENS);
				setState(1896);
				_errHandler.sync(this);
				_la = _input.LA(1);
				if (_la==WHERE) {
					{
					setState(1895);
					type_parameter_constraints_clauses();
					}
				}

				setState(1898);
				match(SEMICOLON);
				}
				break;
			case 3:
				{
				setState(1900);
				match(EVENT);
				setState(1901);
				type();
				setState(1902);
				identifier();
				setState(1903);
				match(SEMICOLON);
				}
				break;
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Interface_accessorsContext extends ParserRuleContext {
		public TerminalNode GET() { return getToken(CSharpParser.GET, 0); }
		public TerminalNode SET() { return getToken(CSharpParser.SET, 0); }
		public List<AttributesContext> attributes() {
			return getRuleContexts(AttributesContext.class);
		}
		public AttributesContext attributes(int i) {
			return getRuleContext(AttributesContext.class,i);
		}
		public Interface_accessorsContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_interface_accessors; }
	}

	public final Interface_accessorsContext interface_accessors() throws RecognitionException {
		Interface_accessorsContext _localctx = new Interface_accessorsContext(_ctx, getState());
		enterRule(_localctx, 302, RULE_interface_accessors);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(1908);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==OPEN_BRACKET) {
				{
				setState(1907);
				attributes();
				}
			}

			setState(1928);
			_errHandler.sync(this);
			switch (_input.LA(1)) {
			case GET:
				{
				setState(1910);
				match(GET);
				setState(1911);
				match(SEMICOLON);
				setState(1917);
				_errHandler.sync(this);
				_la = _input.LA(1);
				if (_la==SET || _la==OPEN_BRACKET) {
					{
					setState(1913);
					_errHandler.sync(this);
					_la = _input.LA(1);
					if (_la==OPEN_BRACKET) {
						{
						setState(1912);
						attributes();
						}
					}

					setState(1915);
					match(SET);
					setState(1916);
					match(SEMICOLON);
					}
				}

				}
				break;
			case SET:
				{
				setState(1919);
				match(SET);
				setState(1920);
				match(SEMICOLON);
				setState(1926);
				_errHandler.sync(this);
				_la = _input.LA(1);
				if (_la==GET || _la==OPEN_BRACKET) {
					{
					setState(1922);
					_errHandler.sync(this);
					_la = _input.LA(1);
					if (_la==OPEN_BRACKET) {
						{
						setState(1921);
						attributes();
						}
					}

					setState(1924);
					match(GET);
					setState(1925);
					match(SEMICOLON);
					}
				}

				}
				break;
			default:
				throw new NoViableAltException(this);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Enum_baseContext extends ParserRuleContext {
		public TypeContext type() {
			return getRuleContext(TypeContext.class,0);
		}
		public Enum_baseContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_enum_base; }
	}

	public final Enum_baseContext enum_base() throws RecognitionException {
		Enum_baseContext _localctx = new Enum_baseContext(_ctx, getState());
		enterRule(_localctx, 304, RULE_enum_base);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(1930);
			match(COLON);
			setState(1931);
			type();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Enum_bodyContext extends ParserRuleContext {
		public TerminalNode OPEN_BRACE() { return getToken(CSharpParser.OPEN_BRACE, 0); }
		public TerminalNode CLOSE_BRACE() { return getToken(CSharpParser.CLOSE_BRACE, 0); }
		public List<Enum_member_declarationContext> enum_member_declaration() {
			return getRuleContexts(Enum_member_declarationContext.class);
		}
		public Enum_member_declarationContext enum_member_declaration(int i) {
			return getRuleContext(Enum_member_declarationContext.class,i);
		}
		public Enum_bodyContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_enum_body; }
	}

	public final Enum_bodyContext enum_body() throws RecognitionException {
		Enum_bodyContext _localctx = new Enum_bodyContext(_ctx, getState());
		enterRule(_localctx, 306, RULE_enum_body);
		int _la;
		try {
			int _alt;
			enterOuterAlt(_localctx, 1);
			{
			setState(1933);
			match(OPEN_BRACE);
			setState(1945);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if ((((_la) & ~0x3f) == 0 && ((1L << _la) & ((1L << ADD) | (1L << ALIAS) | (1L << ARGLIST) | (1L << ASCENDING) | (1L << ASYNC) | (1L << AWAIT) | (1L << BY) | (1L << DESCENDING) | (1L << DYNAMIC) | (1L << EQUALS) | (1L << FROM) | (1L << GET) | (1L << GROUP) | (1L << INTO) | (1L << JOIN) | (1L << LET) | (1L << NAMEOF))) != 0) || ((((_la - 68)) & ~0x3f) == 0 && ((1L << (_la - 68)) & ((1L << (ON - 68)) | (1L << (ORDERBY - 68)) | (1L << (PARTIAL - 68)) | (1L << (REMOVE - 68)) | (1L << (SELECT - 68)) | (1L << (SET - 68)) | (1L << (WHEN - 68)) | (1L << (WHERE - 68)) | (1L << (YIELD - 68)) | (1L << (IDENTIFIER - 68)) | (1L << (OPEN_BRACKET - 68)))) != 0)) {
				{
				setState(1934);
				enum_member_declaration();
				setState(1939);
				_errHandler.sync(this);
				_alt = getInterpreter().adaptivePredict(_input,228,_ctx);
				while ( _alt!=2 && _alt!=org.antlr.v4.runtime.atn.ATN.INVALID_ALT_NUMBER ) {
					if ( _alt==1 ) {
						{
						{
						setState(1935);
						match(COMMA);
						setState(1936);
						enum_member_declaration();
						}
						} 
					}
					setState(1941);
					_errHandler.sync(this);
					_alt = getInterpreter().adaptivePredict(_input,228,_ctx);
				}
				setState(1943);
				_errHandler.sync(this);
				_la = _input.LA(1);
				if (_la==COMMA) {
					{
					setState(1942);
					match(COMMA);
					}
				}

				}
			}

			setState(1947);
			match(CLOSE_BRACE);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Enum_member_declarationContext extends ParserRuleContext {
		public IdentifierContext identifier() {
			return getRuleContext(IdentifierContext.class,0);
		}
		public AttributesContext attributes() {
			return getRuleContext(AttributesContext.class,0);
		}
		public ExpressionContext expression() {
			return getRuleContext(ExpressionContext.class,0);
		}
		public Enum_member_declarationContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_enum_member_declaration; }
	}

	public final Enum_member_declarationContext enum_member_declaration() throws RecognitionException {
		Enum_member_declarationContext _localctx = new Enum_member_declarationContext(_ctx, getState());
		enterRule(_localctx, 308, RULE_enum_member_declaration);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(1950);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==OPEN_BRACKET) {
				{
				setState(1949);
				attributes();
				}
			}

			setState(1952);
			identifier();
			setState(1955);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==ASSIGNMENT) {
				{
				setState(1953);
				match(ASSIGNMENT);
				setState(1954);
				expression();
				}
			}

			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Global_attribute_sectionContext extends ParserRuleContext {
		public Global_attribute_targetContext global_attribute_target() {
			return getRuleContext(Global_attribute_targetContext.class,0);
		}
		public Attribute_listContext attribute_list() {
			return getRuleContext(Attribute_listContext.class,0);
		}
		public Global_attribute_sectionContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_global_attribute_section; }
	}

	public final Global_attribute_sectionContext global_attribute_section() throws RecognitionException {
		Global_attribute_sectionContext _localctx = new Global_attribute_sectionContext(_ctx, getState());
		enterRule(_localctx, 310, RULE_global_attribute_section);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(1957);
			match(OPEN_BRACKET);
			setState(1958);
			global_attribute_target();
			setState(1959);
			match(COLON);
			setState(1960);
			attribute_list();
			setState(1962);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==COMMA) {
				{
				setState(1961);
				match(COMMA);
				}
			}

			setState(1964);
			match(CLOSE_BRACKET);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Global_attribute_targetContext extends ParserRuleContext {
		public KeywordContext keyword() {
			return getRuleContext(KeywordContext.class,0);
		}
		public IdentifierContext identifier() {
			return getRuleContext(IdentifierContext.class,0);
		}
		public Global_attribute_targetContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_global_attribute_target; }
	}

	public final Global_attribute_targetContext global_attribute_target() throws RecognitionException {
		Global_attribute_targetContext _localctx = new Global_attribute_targetContext(_ctx, getState());
		enterRule(_localctx, 312, RULE_global_attribute_target);
		try {
			setState(1968);
			_errHandler.sync(this);
			switch (_input.LA(1)) {
			case ABSTRACT:
			case AS:
			case BASE:
			case BOOL:
			case BREAK:
			case BYTE:
			case CASE:
			case CATCH:
			case CHAR:
			case CHECKED:
			case CLASS:
			case CONST:
			case CONTINUE:
			case DECIMAL:
			case DEFAULT:
			case DELEGATE:
			case DO:
			case DOUBLE:
			case ELSE:
			case ENUM:
			case EVENT:
			case EXPLICIT:
			case EXTERN:
			case FALSE:
			case FINALLY:
			case FIXED:
			case FLOAT:
			case FOR:
			case FOREACH:
			case GOTO:
			case IF:
			case IMPLICIT:
			case IN:
			case INT:
			case INTERFACE:
			case INTERNAL:
			case IS:
			case LOCK:
			case LONG:
			case NAMESPACE:
			case NEW:
			case NULL:
			case OBJECT:
			case OPERATOR:
			case OUT:
			case OVERRIDE:
			case PARAMS:
			case PRIVATE:
			case PROTECTED:
			case PUBLIC:
			case READONLY:
			case REF:
			case RETURN:
			case SBYTE:
			case SEALED:
			case SHORT:
			case SIZEOF:
			case STACKALLOC:
			case STATIC:
			case STRING:
			case STRUCT:
			case SWITCH:
			case THIS:
			case THROW:
			case TRUE:
			case TRY:
			case TYPEOF:
			case UINT:
			case ULONG:
			case UNCHECKED:
			case UNSAFE:
			case USHORT:
			case USING:
			case VIRTUAL:
			case VOID:
			case VOLATILE:
			case WHILE:
				enterOuterAlt(_localctx, 1);
				{
				setState(1966);
				keyword();
				}
				break;
			case ADD:
			case ALIAS:
			case ARGLIST:
			case ASCENDING:
			case ASYNC:
			case AWAIT:
			case BY:
			case DESCENDING:
			case DYNAMIC:
			case EQUALS:
			case FROM:
			case GET:
			case GROUP:
			case INTO:
			case JOIN:
			case LET:
			case NAMEOF:
			case ON:
			case ORDERBY:
			case PARTIAL:
			case REMOVE:
			case SELECT:
			case SET:
			case WHEN:
			case WHERE:
			case YIELD:
			case IDENTIFIER:
				enterOuterAlt(_localctx, 2);
				{
				setState(1967);
				identifier();
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class AttributesContext extends ParserRuleContext {
		public List<Attribute_sectionContext> attribute_section() {
			return getRuleContexts(Attribute_sectionContext.class);
		}
		public Attribute_sectionContext attribute_section(int i) {
			return getRuleContext(Attribute_sectionContext.class,i);
		}
		public AttributesContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_attributes; }
	}

	public final AttributesContext attributes() throws RecognitionException {
		AttributesContext _localctx = new AttributesContext(_ctx, getState());
		enterRule(_localctx, 314, RULE_attributes);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(1971); 
			_errHandler.sync(this);
			_la = _input.LA(1);
			do {
				{
				{
				setState(1970);
				attribute_section();
				}
				}
				setState(1973); 
				_errHandler.sync(this);
				_la = _input.LA(1);
			} while ( _la==OPEN_BRACKET );
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Attribute_sectionContext extends ParserRuleContext {
		public Attribute_listContext attribute_list() {
			return getRuleContext(Attribute_listContext.class,0);
		}
		public Attribute_targetContext attribute_target() {
			return getRuleContext(Attribute_targetContext.class,0);
		}
		public Attribute_sectionContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_attribute_section; }
	}

	public final Attribute_sectionContext attribute_section() throws RecognitionException {
		Attribute_sectionContext _localctx = new Attribute_sectionContext(_ctx, getState());
		enterRule(_localctx, 316, RULE_attribute_section);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(1975);
			match(OPEN_BRACKET);
			setState(1979);
			_errHandler.sync(this);
			switch ( getInterpreter().adaptivePredict(_input,236,_ctx) ) {
			case 1:
				{
				setState(1976);
				attribute_target();
				setState(1977);
				match(COLON);
				}
				break;
			}
			setState(1981);
			attribute_list();
			setState(1983);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==COMMA) {
				{
				setState(1982);
				match(COMMA);
				}
			}

			setState(1985);
			match(CLOSE_BRACKET);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Attribute_targetContext extends ParserRuleContext {
		public KeywordContext keyword() {
			return getRuleContext(KeywordContext.class,0);
		}
		public IdentifierContext identifier() {
			return getRuleContext(IdentifierContext.class,0);
		}
		public Attribute_targetContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_attribute_target; }
	}

	public final Attribute_targetContext attribute_target() throws RecognitionException {
		Attribute_targetContext _localctx = new Attribute_targetContext(_ctx, getState());
		enterRule(_localctx, 318, RULE_attribute_target);
		try {
			setState(1989);
			_errHandler.sync(this);
			switch (_input.LA(1)) {
			case ABSTRACT:
			case AS:
			case BASE:
			case BOOL:
			case BREAK:
			case BYTE:
			case CASE:
			case CATCH:
			case CHAR:
			case CHECKED:
			case CLASS:
			case CONST:
			case CONTINUE:
			case DECIMAL:
			case DEFAULT:
			case DELEGATE:
			case DO:
			case DOUBLE:
			case ELSE:
			case ENUM:
			case EVENT:
			case EXPLICIT:
			case EXTERN:
			case FALSE:
			case FINALLY:
			case FIXED:
			case FLOAT:
			case FOR:
			case FOREACH:
			case GOTO:
			case IF:
			case IMPLICIT:
			case IN:
			case INT:
			case INTERFACE:
			case INTERNAL:
			case IS:
			case LOCK:
			case LONG:
			case NAMESPACE:
			case NEW:
			case NULL:
			case OBJECT:
			case OPERATOR:
			case OUT:
			case OVERRIDE:
			case PARAMS:
			case PRIVATE:
			case PROTECTED:
			case PUBLIC:
			case READONLY:
			case REF:
			case RETURN:
			case SBYTE:
			case SEALED:
			case SHORT:
			case SIZEOF:
			case STACKALLOC:
			case STATIC:
			case STRING:
			case STRUCT:
			case SWITCH:
			case THIS:
			case THROW:
			case TRUE:
			case TRY:
			case TYPEOF:
			case UINT:
			case ULONG:
			case UNCHECKED:
			case UNSAFE:
			case USHORT:
			case USING:
			case VIRTUAL:
			case VOID:
			case VOLATILE:
			case WHILE:
				enterOuterAlt(_localctx, 1);
				{
				setState(1987);
				keyword();
				}
				break;
			case ADD:
			case ALIAS:
			case ARGLIST:
			case ASCENDING:
			case ASYNC:
			case AWAIT:
			case BY:
			case DESCENDING:
			case DYNAMIC:
			case EQUALS:
			case FROM:
			case GET:
			case GROUP:
			case INTO:
			case JOIN:
			case LET:
			case NAMEOF:
			case ON:
			case ORDERBY:
			case PARTIAL:
			case REMOVE:
			case SELECT:
			case SET:
			case WHEN:
			case WHERE:
			case YIELD:
			case IDENTIFIER:
				enterOuterAlt(_localctx, 2);
				{
				setState(1988);
				identifier();
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Attribute_listContext extends ParserRuleContext {
		public List<AttributeContext> attribute() {
			return getRuleContexts(AttributeContext.class);
		}
		public AttributeContext attribute(int i) {
			return getRuleContext(AttributeContext.class,i);
		}
		public Attribute_listContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_attribute_list; }
	}

	public final Attribute_listContext attribute_list() throws RecognitionException {
		Attribute_listContext _localctx = new Attribute_listContext(_ctx, getState());
		enterRule(_localctx, 320, RULE_attribute_list);
		try {
			int _alt;
			enterOuterAlt(_localctx, 1);
			{
			setState(1991);
			attribute();
			setState(1996);
			_errHandler.sync(this);
			_alt = getInterpreter().adaptivePredict(_input,239,_ctx);
			while ( _alt!=2 && _alt!=org.antlr.v4.runtime.atn.ATN.INVALID_ALT_NUMBER ) {
				if ( _alt==1 ) {
					{
					{
					setState(1992);
					match(COMMA);
					setState(1993);
					attribute();
					}
					} 
				}
				setState(1998);
				_errHandler.sync(this);
				_alt = getInterpreter().adaptivePredict(_input,239,_ctx);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class AttributeContext extends ParserRuleContext {
		public Namespace_or_type_nameContext namespace_or_type_name() {
			return getRuleContext(Namespace_or_type_nameContext.class,0);
		}
		public TerminalNode OPEN_PARENS() { return getToken(CSharpParser.OPEN_PARENS, 0); }
		public TerminalNode CLOSE_PARENS() { return getToken(CSharpParser.CLOSE_PARENS, 0); }
		public List<Attribute_argumentContext> attribute_argument() {
			return getRuleContexts(Attribute_argumentContext.class);
		}
		public Attribute_argumentContext attribute_argument(int i) {
			return getRuleContext(Attribute_argumentContext.class,i);
		}
		public AttributeContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_attribute; }
	}

	public final AttributeContext attribute() throws RecognitionException {
		AttributeContext _localctx = new AttributeContext(_ctx, getState());
		enterRule(_localctx, 322, RULE_attribute);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(1999);
			namespace_or_type_name();
			setState(2012);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==OPEN_PARENS) {
				{
				setState(2000);
				match(OPEN_PARENS);
				setState(2009);
				_errHandler.sync(this);
				_la = _input.LA(1);
				if ((((_la) & ~0x3f) == 0 && ((1L << _la) & ((1L << ADD) | (1L << ALIAS) | (1L << ARGLIST) | (1L << ASCENDING) | (1L << ASYNC) | (1L << AWAIT) | (1L << BASE) | (1L << BOOL) | (1L << BY) | (1L << BYTE) | (1L << CHAR) | (1L << CHECKED) | (1L << DECIMAL) | (1L << DEFAULT) | (1L << DELEGATE) | (1L << DESCENDING) | (1L << DOUBLE) | (1L << DYNAMIC) | (1L << EQUALS) | (1L << FALSE) | (1L << FLOAT) | (1L << FROM) | (1L << GET) | (1L << GROUP) | (1L << INT) | (1L << INTO) | (1L << JOIN) | (1L << LET) | (1L << LONG) | (1L << NAMEOF))) != 0) || ((((_la - 65)) & ~0x3f) == 0 && ((1L << (_la - 65)) & ((1L << (NEW - 65)) | (1L << (NULL - 65)) | (1L << (OBJECT - 65)) | (1L << (ON - 65)) | (1L << (ORDERBY - 65)) | (1L << (PARTIAL - 65)) | (1L << (REMOVE - 65)) | (1L << (SBYTE - 65)) | (1L << (SELECT - 65)) | (1L << (SET - 65)) | (1L << (SHORT - 65)) | (1L << (SIZEOF - 65)) | (1L << (STRING - 65)) | (1L << (THIS - 65)) | (1L << (TRUE - 65)) | (1L << (TYPEOF - 65)) | (1L << (UINT - 65)) | (1L << (ULONG - 65)) | (1L << (UNCHECKED - 65)) | (1L << (USHORT - 65)) | (1L << (WHEN - 65)) | (1L << (WHERE - 65)) | (1L << (YIELD - 65)) | (1L << (IDENTIFIER - 65)) | (1L << (LITERAL_ACCESS - 65)) | (1L << (INTEGER_LITERAL - 65)) | (1L << (HEX_INTEGER_LITERAL - 65)) | (1L << (REAL_LITERAL - 65)) | (1L << (CHARACTER_LITERAL - 65)) | (1L << (REGULAR_STRING - 65)) | (1L << (VERBATIUM_STRING - 65)) | (1L << (INTERPOLATED_REGULAR_STRING_START - 65)) | (1L << (INTERPOLATED_VERBATIUM_STRING_START - 65)) | (1L << (OPEN_PARENS - 65)))) != 0) || ((((_la - 131)) & ~0x3f) == 0 && ((1L << (_la - 131)) & ((1L << (PLUS - 131)) | (1L << (MINUS - 131)) | (1L << (STAR - 131)) | (1L << (AMP - 131)) | (1L << (BANG - 131)) | (1L << (TILDE - 131)) | (1L << (OP_INC - 131)) | (1L << (OP_DEC - 131)))) != 0)) {
					{
					setState(2001);
					attribute_argument();
					setState(2006);
					_errHandler.sync(this);
					_la = _input.LA(1);
					while (_la==COMMA) {
						{
						{
						setState(2002);
						match(COMMA);
						setState(2003);
						attribute_argument();
						}
						}
						setState(2008);
						_errHandler.sync(this);
						_la = _input.LA(1);
					}
					}
				}

				setState(2011);
				match(CLOSE_PARENS);
				}
			}

			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Attribute_argumentContext extends ParserRuleContext {
		public ExpressionContext expression() {
			return getRuleContext(ExpressionContext.class,0);
		}
		public IdentifierContext identifier() {
			return getRuleContext(IdentifierContext.class,0);
		}
		public Attribute_argumentContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_attribute_argument; }
	}

	public final Attribute_argumentContext attribute_argument() throws RecognitionException {
		Attribute_argumentContext _localctx = new Attribute_argumentContext(_ctx, getState());
		enterRule(_localctx, 324, RULE_attribute_argument);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(2017);
			_errHandler.sync(this);
			switch ( getInterpreter().adaptivePredict(_input,243,_ctx) ) {
			case 1:
				{
				setState(2014);
				identifier();
				setState(2015);
				match(COLON);
				}
				break;
			}
			setState(2019);
			expression();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Pointer_typeContext extends ParserRuleContext {
		public Simple_typeContext simple_type() {
			return getRuleContext(Simple_typeContext.class,0);
		}
		public Class_typeContext class_type() {
			return getRuleContext(Class_typeContext.class,0);
		}
		public List<Rank_specifierContext> rank_specifier() {
			return getRuleContexts(Rank_specifierContext.class);
		}
		public Rank_specifierContext rank_specifier(int i) {
			return getRuleContext(Rank_specifierContext.class,i);
		}
		public TerminalNode VOID() { return getToken(CSharpParser.VOID, 0); }
		public Pointer_typeContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_pointer_type; }
	}

	public final Pointer_typeContext pointer_type() throws RecognitionException {
		Pointer_typeContext _localctx = new Pointer_typeContext(_ctx, getState());
		enterRule(_localctx, 326, RULE_pointer_type);
		int _la;
		try {
			setState(2036);
			_errHandler.sync(this);
			switch (_input.LA(1)) {
			case ADD:
			case ALIAS:
			case ARGLIST:
			case ASCENDING:
			case ASYNC:
			case AWAIT:
			case BOOL:
			case BY:
			case BYTE:
			case CHAR:
			case DECIMAL:
			case DESCENDING:
			case DOUBLE:
			case DYNAMIC:
			case EQUALS:
			case FLOAT:
			case FROM:
			case GET:
			case GROUP:
			case INT:
			case INTO:
			case JOIN:
			case LET:
			case LONG:
			case NAMEOF:
			case OBJECT:
			case ON:
			case ORDERBY:
			case PARTIAL:
			case REMOVE:
			case SBYTE:
			case SELECT:
			case SET:
			case SHORT:
			case STRING:
			case UINT:
			case ULONG:
			case USHORT:
			case WHEN:
			case WHERE:
			case YIELD:
			case IDENTIFIER:
				enterOuterAlt(_localctx, 1);
				{
				setState(2023);
				_errHandler.sync(this);
				switch (_input.LA(1)) {
				case BOOL:
				case BYTE:
				case CHAR:
				case DECIMAL:
				case DOUBLE:
				case FLOAT:
				case INT:
				case LONG:
				case SBYTE:
				case SHORT:
				case UINT:
				case ULONG:
				case USHORT:
					{
					setState(2021);
					simple_type();
					}
					break;
				case ADD:
				case ALIAS:
				case ARGLIST:
				case ASCENDING:
				case ASYNC:
				case AWAIT:
				case BY:
				case DESCENDING:
				case DYNAMIC:
				case EQUALS:
				case FROM:
				case GET:
				case GROUP:
				case INTO:
				case JOIN:
				case LET:
				case NAMEOF:
				case OBJECT:
				case ON:
				case ORDERBY:
				case PARTIAL:
				case REMOVE:
				case SELECT:
				case SET:
				case STRING:
				case WHEN:
				case WHERE:
				case YIELD:
				case IDENTIFIER:
					{
					setState(2022);
					class_type();
					}
					break;
				default:
					throw new NoViableAltException(this);
				}
				setState(2029);
				_errHandler.sync(this);
				_la = _input.LA(1);
				while (_la==OPEN_BRACKET || _la==INTERR) {
					{
					setState(2027);
					_errHandler.sync(this);
					switch (_input.LA(1)) {
					case OPEN_BRACKET:
						{
						setState(2025);
						rank_specifier();
						}
						break;
					case INTERR:
						{
						setState(2026);
						match(INTERR);
						}
						break;
					default:
						throw new NoViableAltException(this);
					}
					}
					setState(2031);
					_errHandler.sync(this);
					_la = _input.LA(1);
				}
				setState(2032);
				match(STAR);
				}
				break;
			case VOID:
				enterOuterAlt(_localctx, 2);
				{
				setState(2034);
				match(VOID);
				setState(2035);
				match(STAR);
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Fixed_pointer_declaratorsContext extends ParserRuleContext {
		public List<Fixed_pointer_declaratorContext> fixed_pointer_declarator() {
			return getRuleContexts(Fixed_pointer_declaratorContext.class);
		}
		public Fixed_pointer_declaratorContext fixed_pointer_declarator(int i) {
			return getRuleContext(Fixed_pointer_declaratorContext.class,i);
		}
		public Fixed_pointer_declaratorsContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_fixed_pointer_declarators; }
	}

	public final Fixed_pointer_declaratorsContext fixed_pointer_declarators() throws RecognitionException {
		Fixed_pointer_declaratorsContext _localctx = new Fixed_pointer_declaratorsContext(_ctx, getState());
		enterRule(_localctx, 328, RULE_fixed_pointer_declarators);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(2038);
			fixed_pointer_declarator();
			setState(2043);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while (_la==COMMA) {
				{
				{
				setState(2039);
				match(COMMA);
				setState(2040);
				fixed_pointer_declarator();
				}
				}
				setState(2045);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Fixed_pointer_declaratorContext extends ParserRuleContext {
		public IdentifierContext identifier() {
			return getRuleContext(IdentifierContext.class,0);
		}
		public Fixed_pointer_initializerContext fixed_pointer_initializer() {
			return getRuleContext(Fixed_pointer_initializerContext.class,0);
		}
		public Fixed_pointer_declaratorContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_fixed_pointer_declarator; }
	}

	public final Fixed_pointer_declaratorContext fixed_pointer_declarator() throws RecognitionException {
		Fixed_pointer_declaratorContext _localctx = new Fixed_pointer_declaratorContext(_ctx, getState());
		enterRule(_localctx, 330, RULE_fixed_pointer_declarator);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(2046);
			identifier();
			setState(2047);
			match(ASSIGNMENT);
			setState(2048);
			fixed_pointer_initializer();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Fixed_pointer_initializerContext extends ParserRuleContext {
		public ExpressionContext expression() {
			return getRuleContext(ExpressionContext.class,0);
		}
		public Local_variable_initializer_unsafeContext local_variable_initializer_unsafe() {
			return getRuleContext(Local_variable_initializer_unsafeContext.class,0);
		}
		public Fixed_pointer_initializerContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_fixed_pointer_initializer; }
	}

	public final Fixed_pointer_initializerContext fixed_pointer_initializer() throws RecognitionException {
		Fixed_pointer_initializerContext _localctx = new Fixed_pointer_initializerContext(_ctx, getState());
		enterRule(_localctx, 332, RULE_fixed_pointer_initializer);
		try {
			setState(2055);
			_errHandler.sync(this);
			switch (_input.LA(1)) {
			case ADD:
			case ALIAS:
			case ARGLIST:
			case ASCENDING:
			case ASYNC:
			case AWAIT:
			case BASE:
			case BOOL:
			case BY:
			case BYTE:
			case CHAR:
			case CHECKED:
			case DECIMAL:
			case DEFAULT:
			case DELEGATE:
			case DESCENDING:
			case DOUBLE:
			case DYNAMIC:
			case EQUALS:
			case FALSE:
			case FLOAT:
			case FROM:
			case GET:
			case GROUP:
			case INT:
			case INTO:
			case JOIN:
			case LET:
			case LONG:
			case NAMEOF:
			case NEW:
			case NULL:
			case OBJECT:
			case ON:
			case ORDERBY:
			case PARTIAL:
			case REMOVE:
			case SBYTE:
			case SELECT:
			case SET:
			case SHORT:
			case SIZEOF:
			case STRING:
			case THIS:
			case TRUE:
			case TYPEOF:
			case UINT:
			case ULONG:
			case UNCHECKED:
			case USHORT:
			case WHEN:
			case WHERE:
			case YIELD:
			case IDENTIFIER:
			case LITERAL_ACCESS:
			case INTEGER_LITERAL:
			case HEX_INTEGER_LITERAL:
			case REAL_LITERAL:
			case CHARACTER_LITERAL:
			case REGULAR_STRING:
			case VERBATIUM_STRING:
			case INTERPOLATED_REGULAR_STRING_START:
			case INTERPOLATED_VERBATIUM_STRING_START:
			case OPEN_PARENS:
			case PLUS:
			case MINUS:
			case STAR:
			case AMP:
			case BANG:
			case TILDE:
			case OP_INC:
			case OP_DEC:
				enterOuterAlt(_localctx, 1);
				{
				setState(2051);
				_errHandler.sync(this);
				switch ( getInterpreter().adaptivePredict(_input,249,_ctx) ) {
				case 1:
					{
					setState(2050);
					match(AMP);
					}
					break;
				}
				setState(2053);
				expression();
				}
				break;
			case STACKALLOC:
				enterOuterAlt(_localctx, 2);
				{
				setState(2054);
				local_variable_initializer_unsafe();
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Fixed_size_buffer_declaratorContext extends ParserRuleContext {
		public IdentifierContext identifier() {
			return getRuleContext(IdentifierContext.class,0);
		}
		public ExpressionContext expression() {
			return getRuleContext(ExpressionContext.class,0);
		}
		public Fixed_size_buffer_declaratorContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_fixed_size_buffer_declarator; }
	}

	public final Fixed_size_buffer_declaratorContext fixed_size_buffer_declarator() throws RecognitionException {
		Fixed_size_buffer_declaratorContext _localctx = new Fixed_size_buffer_declaratorContext(_ctx, getState());
		enterRule(_localctx, 334, RULE_fixed_size_buffer_declarator);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(2057);
			identifier();
			setState(2058);
			match(OPEN_BRACKET);
			setState(2059);
			expression();
			setState(2060);
			match(CLOSE_BRACKET);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Local_variable_initializer_unsafeContext extends ParserRuleContext {
		public TerminalNode STACKALLOC() { return getToken(CSharpParser.STACKALLOC, 0); }
		public TypeContext type() {
			return getRuleContext(TypeContext.class,0);
		}
		public ExpressionContext expression() {
			return getRuleContext(ExpressionContext.class,0);
		}
		public Local_variable_initializer_unsafeContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_local_variable_initializer_unsafe; }
	}

	public final Local_variable_initializer_unsafeContext local_variable_initializer_unsafe() throws RecognitionException {
		Local_variable_initializer_unsafeContext _localctx = new Local_variable_initializer_unsafeContext(_ctx, getState());
		enterRule(_localctx, 336, RULE_local_variable_initializer_unsafe);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(2062);
			match(STACKALLOC);
			setState(2063);
			type();
			setState(2064);
			match(OPEN_BRACKET);
			setState(2065);
			expression();
			setState(2066);
			match(CLOSE_BRACKET);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Right_arrowContext extends ParserRuleContext {
		public Token first;
		public Token second;
		public Right_arrowContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_right_arrow; }
	}

	public final Right_arrowContext right_arrow() throws RecognitionException {
		Right_arrowContext _localctx = new Right_arrowContext(_ctx, getState());
		enterRule(_localctx, 338, RULE_right_arrow);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(2068);
			((Right_arrowContext)_localctx).first = match(ASSIGNMENT);
			setState(2069);
			((Right_arrowContext)_localctx).second = match(GT);
			setState(2070);
			if (!((((Right_arrowContext)_localctx).first!=null?((Right_arrowContext)_localctx).first.getTokenIndex():0) + 1 == (((Right_arrowContext)_localctx).second!=null?((Right_arrowContext)_localctx).second.getTokenIndex():0))) throw new FailedPredicateException(this, "$first.index + 1 == $second.index");
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Right_shiftContext extends ParserRuleContext {
		public Token first;
		public Token second;
		public Right_shiftContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_right_shift; }
	}

	public final Right_shiftContext right_shift() throws RecognitionException {
		Right_shiftContext _localctx = new Right_shiftContext(_ctx, getState());
		enterRule(_localctx, 340, RULE_right_shift);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(2072);
			((Right_shiftContext)_localctx).first = match(GT);
			setState(2073);
			((Right_shiftContext)_localctx).second = match(GT);
			setState(2074);
			if (!((((Right_shiftContext)_localctx).first!=null?((Right_shiftContext)_localctx).first.getTokenIndex():0) + 1 == (((Right_shiftContext)_localctx).second!=null?((Right_shiftContext)_localctx).second.getTokenIndex():0))) throw new FailedPredicateException(this, "$first.index + 1 == $second.index");
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Right_shift_assignmentContext extends ParserRuleContext {
		public Token first;
		public Token second;
		public Right_shift_assignmentContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_right_shift_assignment; }
	}

	public final Right_shift_assignmentContext right_shift_assignment() throws RecognitionException {
		Right_shift_assignmentContext _localctx = new Right_shift_assignmentContext(_ctx, getState());
		enterRule(_localctx, 342, RULE_right_shift_assignment);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(2076);
			((Right_shift_assignmentContext)_localctx).first = match(GT);
			setState(2077);
			((Right_shift_assignmentContext)_localctx).second = match(OP_GE);
			setState(2078);
			if (!((((Right_shift_assignmentContext)_localctx).first!=null?((Right_shift_assignmentContext)_localctx).first.getTokenIndex():0) + 1 == (((Right_shift_assignmentContext)_localctx).second!=null?((Right_shift_assignmentContext)_localctx).second.getTokenIndex():0))) throw new FailedPredicateException(this, "$first.index + 1 == $second.index");
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class LiteralContext extends ParserRuleContext {
		public Boolean_literalContext boolean_literal() {
			return getRuleContext(Boolean_literalContext.class,0);
		}
		public String_literalContext string_literal() {
			return getRuleContext(String_literalContext.class,0);
		}
		public TerminalNode INTEGER_LITERAL() { return getToken(CSharpParser.INTEGER_LITERAL, 0); }
		public TerminalNode HEX_INTEGER_LITERAL() { return getToken(CSharpParser.HEX_INTEGER_LITERAL, 0); }
		public TerminalNode REAL_LITERAL() { return getToken(CSharpParser.REAL_LITERAL, 0); }
		public TerminalNode CHARACTER_LITERAL() { return getToken(CSharpParser.CHARACTER_LITERAL, 0); }
		public TerminalNode NULL() { return getToken(CSharpParser.NULL, 0); }
		public LiteralContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_literal; }
	}

	public final LiteralContext literal() throws RecognitionException {
		LiteralContext _localctx = new LiteralContext(_ctx, getState());
		enterRule(_localctx, 344, RULE_literal);
		try {
			setState(2087);
			_errHandler.sync(this);
			switch (_input.LA(1)) {
			case FALSE:
			case TRUE:
				enterOuterAlt(_localctx, 1);
				{
				setState(2080);
				boolean_literal();
				}
				break;
			case REGULAR_STRING:
			case VERBATIUM_STRING:
			case INTERPOLATED_REGULAR_STRING_START:
			case INTERPOLATED_VERBATIUM_STRING_START:
				enterOuterAlt(_localctx, 2);
				{
				setState(2081);
				string_literal();
				}
				break;
			case INTEGER_LITERAL:
				enterOuterAlt(_localctx, 3);
				{
				setState(2082);
				match(INTEGER_LITERAL);
				}
				break;
			case HEX_INTEGER_LITERAL:
				enterOuterAlt(_localctx, 4);
				{
				setState(2083);
				match(HEX_INTEGER_LITERAL);
				}
				break;
			case REAL_LITERAL:
				enterOuterAlt(_localctx, 5);
				{
				setState(2084);
				match(REAL_LITERAL);
				}
				break;
			case CHARACTER_LITERAL:
				enterOuterAlt(_localctx, 6);
				{
				setState(2085);
				match(CHARACTER_LITERAL);
				}
				break;
			case NULL:
				enterOuterAlt(_localctx, 7);
				{
				setState(2086);
				match(NULL);
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Boolean_literalContext extends ParserRuleContext {
		public TerminalNode TRUE() { return getToken(CSharpParser.TRUE, 0); }
		public TerminalNode FALSE() { return getToken(CSharpParser.FALSE, 0); }
		public Boolean_literalContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_boolean_literal; }
	}

	public final Boolean_literalContext boolean_literal() throws RecognitionException {
		Boolean_literalContext _localctx = new Boolean_literalContext(_ctx, getState());
		enterRule(_localctx, 346, RULE_boolean_literal);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(2089);
			_la = _input.LA(1);
			if ( !(_la==FALSE || _la==TRUE) ) {
			_errHandler.recoverInline(this);
			}
			else {
				if ( _input.LA(1)==Token.EOF ) matchedEOF = true;
				_errHandler.reportMatch(this);
				consume();
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class String_literalContext extends ParserRuleContext {
		public Interpolated_regular_stringContext interpolated_regular_string() {
			return getRuleContext(Interpolated_regular_stringContext.class,0);
		}
		public Interpolated_verbatium_stringContext interpolated_verbatium_string() {
			return getRuleContext(Interpolated_verbatium_stringContext.class,0);
		}
		public TerminalNode REGULAR_STRING() { return getToken(CSharpParser.REGULAR_STRING, 0); }
		public TerminalNode VERBATIUM_STRING() { return getToken(CSharpParser.VERBATIUM_STRING, 0); }
		public String_literalContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_string_literal; }
	}

	public final String_literalContext string_literal() throws RecognitionException {
		String_literalContext _localctx = new String_literalContext(_ctx, getState());
		enterRule(_localctx, 348, RULE_string_literal);
		try {
			setState(2095);
			_errHandler.sync(this);
			switch (_input.LA(1)) {
			case INTERPOLATED_REGULAR_STRING_START:
				enterOuterAlt(_localctx, 1);
				{
				setState(2091);
				interpolated_regular_string();
				}
				break;
			case INTERPOLATED_VERBATIUM_STRING_START:
				enterOuterAlt(_localctx, 2);
				{
				setState(2092);
				interpolated_verbatium_string();
				}
				break;
			case REGULAR_STRING:
				enterOuterAlt(_localctx, 3);
				{
				setState(2093);
				match(REGULAR_STRING);
				}
				break;
			case VERBATIUM_STRING:
				enterOuterAlt(_localctx, 4);
				{
				setState(2094);
				match(VERBATIUM_STRING);
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Interpolated_regular_stringContext extends ParserRuleContext {
		public TerminalNode INTERPOLATED_REGULAR_STRING_START() { return getToken(CSharpParser.INTERPOLATED_REGULAR_STRING_START, 0); }
		public TerminalNode DOUBLE_QUOTE_INSIDE() { return getToken(CSharpParser.DOUBLE_QUOTE_INSIDE, 0); }
		public List<Interpolated_regular_string_partContext> interpolated_regular_string_part() {
			return getRuleContexts(Interpolated_regular_string_partContext.class);
		}
		public Interpolated_regular_string_partContext interpolated_regular_string_part(int i) {
			return getRuleContext(Interpolated_regular_string_partContext.class,i);
		}
		public Interpolated_regular_stringContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_interpolated_regular_string; }
	}

	public final Interpolated_regular_stringContext interpolated_regular_string() throws RecognitionException {
		Interpolated_regular_stringContext _localctx = new Interpolated_regular_stringContext(_ctx, getState());
		enterRule(_localctx, 350, RULE_interpolated_regular_string);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(2097);
			match(INTERPOLATED_REGULAR_STRING_START);
			setState(2101);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while ((((_la) & ~0x3f) == 0 && ((1L << _la) & ((1L << ADD) | (1L << ALIAS) | (1L << ARGLIST) | (1L << ASCENDING) | (1L << ASYNC) | (1L << AWAIT) | (1L << BASE) | (1L << BOOL) | (1L << BY) | (1L << BYTE) | (1L << CHAR) | (1L << CHECKED) | (1L << DECIMAL) | (1L << DEFAULT) | (1L << DELEGATE) | (1L << DESCENDING) | (1L << DOUBLE) | (1L << DYNAMIC) | (1L << EQUALS) | (1L << FALSE) | (1L << FLOAT) | (1L << FROM) | (1L << GET) | (1L << GROUP) | (1L << INT) | (1L << INTO) | (1L << JOIN) | (1L << LET) | (1L << LONG) | (1L << NAMEOF))) != 0) || ((((_la - 65)) & ~0x3f) == 0 && ((1L << (_la - 65)) & ((1L << (NEW - 65)) | (1L << (NULL - 65)) | (1L << (OBJECT - 65)) | (1L << (ON - 65)) | (1L << (ORDERBY - 65)) | (1L << (PARTIAL - 65)) | (1L << (REMOVE - 65)) | (1L << (SBYTE - 65)) | (1L << (SELECT - 65)) | (1L << (SET - 65)) | (1L << (SHORT - 65)) | (1L << (SIZEOF - 65)) | (1L << (STRING - 65)) | (1L << (THIS - 65)) | (1L << (TRUE - 65)) | (1L << (TYPEOF - 65)) | (1L << (UINT - 65)) | (1L << (ULONG - 65)) | (1L << (UNCHECKED - 65)) | (1L << (USHORT - 65)) | (1L << (WHEN - 65)) | (1L << (WHERE - 65)) | (1L << (YIELD - 65)) | (1L << (IDENTIFIER - 65)) | (1L << (LITERAL_ACCESS - 65)) | (1L << (INTEGER_LITERAL - 65)) | (1L << (HEX_INTEGER_LITERAL - 65)) | (1L << (REAL_LITERAL - 65)) | (1L << (CHARACTER_LITERAL - 65)) | (1L << (REGULAR_STRING - 65)) | (1L << (VERBATIUM_STRING - 65)) | (1L << (INTERPOLATED_REGULAR_STRING_START - 65)) | (1L << (INTERPOLATED_VERBATIUM_STRING_START - 65)) | (1L << (OPEN_PARENS - 65)))) != 0) || ((((_la - 131)) & ~0x3f) == 0 && ((1L << (_la - 131)) & ((1L << (PLUS - 131)) | (1L << (MINUS - 131)) | (1L << (STAR - 131)) | (1L << (AMP - 131)) | (1L << (BANG - 131)) | (1L << (TILDE - 131)) | (1L << (OP_INC - 131)) | (1L << (OP_DEC - 131)) | (1L << (DOUBLE_CURLY_INSIDE - 131)) | (1L << (REGULAR_CHAR_INSIDE - 131)) | (1L << (REGULAR_STRING_INSIDE - 131)))) != 0)) {
				{
				{
				setState(2098);
				interpolated_regular_string_part();
				}
				}
				setState(2103);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			setState(2104);
			match(DOUBLE_QUOTE_INSIDE);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Interpolated_verbatium_stringContext extends ParserRuleContext {
		public TerminalNode INTERPOLATED_VERBATIUM_STRING_START() { return getToken(CSharpParser.INTERPOLATED_VERBATIUM_STRING_START, 0); }
		public TerminalNode DOUBLE_QUOTE_INSIDE() { return getToken(CSharpParser.DOUBLE_QUOTE_INSIDE, 0); }
		public List<Interpolated_verbatium_string_partContext> interpolated_verbatium_string_part() {
			return getRuleContexts(Interpolated_verbatium_string_partContext.class);
		}
		public Interpolated_verbatium_string_partContext interpolated_verbatium_string_part(int i) {
			return getRuleContext(Interpolated_verbatium_string_partContext.class,i);
		}
		public Interpolated_verbatium_stringContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_interpolated_verbatium_string; }
	}

	public final Interpolated_verbatium_stringContext interpolated_verbatium_string() throws RecognitionException {
		Interpolated_verbatium_stringContext _localctx = new Interpolated_verbatium_stringContext(_ctx, getState());
		enterRule(_localctx, 352, RULE_interpolated_verbatium_string);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(2106);
			match(INTERPOLATED_VERBATIUM_STRING_START);
			setState(2110);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while ((((_la) & ~0x3f) == 0 && ((1L << _la) & ((1L << ADD) | (1L << ALIAS) | (1L << ARGLIST) | (1L << ASCENDING) | (1L << ASYNC) | (1L << AWAIT) | (1L << BASE) | (1L << BOOL) | (1L << BY) | (1L << BYTE) | (1L << CHAR) | (1L << CHECKED) | (1L << DECIMAL) | (1L << DEFAULT) | (1L << DELEGATE) | (1L << DESCENDING) | (1L << DOUBLE) | (1L << DYNAMIC) | (1L << EQUALS) | (1L << FALSE) | (1L << FLOAT) | (1L << FROM) | (1L << GET) | (1L << GROUP) | (1L << INT) | (1L << INTO) | (1L << JOIN) | (1L << LET) | (1L << LONG) | (1L << NAMEOF))) != 0) || ((((_la - 65)) & ~0x3f) == 0 && ((1L << (_la - 65)) & ((1L << (NEW - 65)) | (1L << (NULL - 65)) | (1L << (OBJECT - 65)) | (1L << (ON - 65)) | (1L << (ORDERBY - 65)) | (1L << (PARTIAL - 65)) | (1L << (REMOVE - 65)) | (1L << (SBYTE - 65)) | (1L << (SELECT - 65)) | (1L << (SET - 65)) | (1L << (SHORT - 65)) | (1L << (SIZEOF - 65)) | (1L << (STRING - 65)) | (1L << (THIS - 65)) | (1L << (TRUE - 65)) | (1L << (TYPEOF - 65)) | (1L << (UINT - 65)) | (1L << (ULONG - 65)) | (1L << (UNCHECKED - 65)) | (1L << (USHORT - 65)) | (1L << (WHEN - 65)) | (1L << (WHERE - 65)) | (1L << (YIELD - 65)) | (1L << (IDENTIFIER - 65)) | (1L << (LITERAL_ACCESS - 65)) | (1L << (INTEGER_LITERAL - 65)) | (1L << (HEX_INTEGER_LITERAL - 65)) | (1L << (REAL_LITERAL - 65)) | (1L << (CHARACTER_LITERAL - 65)) | (1L << (REGULAR_STRING - 65)) | (1L << (VERBATIUM_STRING - 65)) | (1L << (INTERPOLATED_REGULAR_STRING_START - 65)) | (1L << (INTERPOLATED_VERBATIUM_STRING_START - 65)) | (1L << (OPEN_PARENS - 65)))) != 0) || ((((_la - 131)) & ~0x3f) == 0 && ((1L << (_la - 131)) & ((1L << (PLUS - 131)) | (1L << (MINUS - 131)) | (1L << (STAR - 131)) | (1L << (AMP - 131)) | (1L << (BANG - 131)) | (1L << (TILDE - 131)) | (1L << (OP_INC - 131)) | (1L << (OP_DEC - 131)) | (1L << (DOUBLE_CURLY_INSIDE - 131)) | (1L << (VERBATIUM_DOUBLE_QUOTE_INSIDE - 131)) | (1L << (VERBATIUM_INSIDE_STRING - 131)))) != 0)) {
				{
				{
				setState(2107);
				interpolated_verbatium_string_part();
				}
				}
				setState(2112);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			setState(2113);
			match(DOUBLE_QUOTE_INSIDE);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Interpolated_regular_string_partContext extends ParserRuleContext {
		public Interpolated_string_expressionContext interpolated_string_expression() {
			return getRuleContext(Interpolated_string_expressionContext.class,0);
		}
		public TerminalNode DOUBLE_CURLY_INSIDE() { return getToken(CSharpParser.DOUBLE_CURLY_INSIDE, 0); }
		public TerminalNode REGULAR_CHAR_INSIDE() { return getToken(CSharpParser.REGULAR_CHAR_INSIDE, 0); }
		public TerminalNode REGULAR_STRING_INSIDE() { return getToken(CSharpParser.REGULAR_STRING_INSIDE, 0); }
		public Interpolated_regular_string_partContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_interpolated_regular_string_part; }
	}

	public final Interpolated_regular_string_partContext interpolated_regular_string_part() throws RecognitionException {
		Interpolated_regular_string_partContext _localctx = new Interpolated_regular_string_partContext(_ctx, getState());
		enterRule(_localctx, 354, RULE_interpolated_regular_string_part);
		try {
			setState(2119);
			_errHandler.sync(this);
			switch (_input.LA(1)) {
			case ADD:
			case ALIAS:
			case ARGLIST:
			case ASCENDING:
			case ASYNC:
			case AWAIT:
			case BASE:
			case BOOL:
			case BY:
			case BYTE:
			case CHAR:
			case CHECKED:
			case DECIMAL:
			case DEFAULT:
			case DELEGATE:
			case DESCENDING:
			case DOUBLE:
			case DYNAMIC:
			case EQUALS:
			case FALSE:
			case FLOAT:
			case FROM:
			case GET:
			case GROUP:
			case INT:
			case INTO:
			case JOIN:
			case LET:
			case LONG:
			case NAMEOF:
			case NEW:
			case NULL:
			case OBJECT:
			case ON:
			case ORDERBY:
			case PARTIAL:
			case REMOVE:
			case SBYTE:
			case SELECT:
			case SET:
			case SHORT:
			case SIZEOF:
			case STRING:
			case THIS:
			case TRUE:
			case TYPEOF:
			case UINT:
			case ULONG:
			case UNCHECKED:
			case USHORT:
			case WHEN:
			case WHERE:
			case YIELD:
			case IDENTIFIER:
			case LITERAL_ACCESS:
			case INTEGER_LITERAL:
			case HEX_INTEGER_LITERAL:
			case REAL_LITERAL:
			case CHARACTER_LITERAL:
			case REGULAR_STRING:
			case VERBATIUM_STRING:
			case INTERPOLATED_REGULAR_STRING_START:
			case INTERPOLATED_VERBATIUM_STRING_START:
			case OPEN_PARENS:
			case PLUS:
			case MINUS:
			case STAR:
			case AMP:
			case BANG:
			case TILDE:
			case OP_INC:
			case OP_DEC:
				enterOuterAlt(_localctx, 1);
				{
				setState(2115);
				interpolated_string_expression();
				}
				break;
			case DOUBLE_CURLY_INSIDE:
				enterOuterAlt(_localctx, 2);
				{
				setState(2116);
				match(DOUBLE_CURLY_INSIDE);
				}
				break;
			case REGULAR_CHAR_INSIDE:
				enterOuterAlt(_localctx, 3);
				{
				setState(2117);
				match(REGULAR_CHAR_INSIDE);
				}
				break;
			case REGULAR_STRING_INSIDE:
				enterOuterAlt(_localctx, 4);
				{
				setState(2118);
				match(REGULAR_STRING_INSIDE);
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Interpolated_verbatium_string_partContext extends ParserRuleContext {
		public Interpolated_string_expressionContext interpolated_string_expression() {
			return getRuleContext(Interpolated_string_expressionContext.class,0);
		}
		public TerminalNode DOUBLE_CURLY_INSIDE() { return getToken(CSharpParser.DOUBLE_CURLY_INSIDE, 0); }
		public TerminalNode VERBATIUM_DOUBLE_QUOTE_INSIDE() { return getToken(CSharpParser.VERBATIUM_DOUBLE_QUOTE_INSIDE, 0); }
		public TerminalNode VERBATIUM_INSIDE_STRING() { return getToken(CSharpParser.VERBATIUM_INSIDE_STRING, 0); }
		public Interpolated_verbatium_string_partContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_interpolated_verbatium_string_part; }
	}

	public final Interpolated_verbatium_string_partContext interpolated_verbatium_string_part() throws RecognitionException {
		Interpolated_verbatium_string_partContext _localctx = new Interpolated_verbatium_string_partContext(_ctx, getState());
		enterRule(_localctx, 356, RULE_interpolated_verbatium_string_part);
		try {
			setState(2125);
			_errHandler.sync(this);
			switch (_input.LA(1)) {
			case ADD:
			case ALIAS:
			case ARGLIST:
			case ASCENDING:
			case ASYNC:
			case AWAIT:
			case BASE:
			case BOOL:
			case BY:
			case BYTE:
			case CHAR:
			case CHECKED:
			case DECIMAL:
			case DEFAULT:
			case DELEGATE:
			case DESCENDING:
			case DOUBLE:
			case DYNAMIC:
			case EQUALS:
			case FALSE:
			case FLOAT:
			case FROM:
			case GET:
			case GROUP:
			case INT:
			case INTO:
			case JOIN:
			case LET:
			case LONG:
			case NAMEOF:
			case NEW:
			case NULL:
			case OBJECT:
			case ON:
			case ORDERBY:
			case PARTIAL:
			case REMOVE:
			case SBYTE:
			case SELECT:
			case SET:
			case SHORT:
			case SIZEOF:
			case STRING:
			case THIS:
			case TRUE:
			case TYPEOF:
			case UINT:
			case ULONG:
			case UNCHECKED:
			case USHORT:
			case WHEN:
			case WHERE:
			case YIELD:
			case IDENTIFIER:
			case LITERAL_ACCESS:
			case INTEGER_LITERAL:
			case HEX_INTEGER_LITERAL:
			case REAL_LITERAL:
			case CHARACTER_LITERAL:
			case REGULAR_STRING:
			case VERBATIUM_STRING:
			case INTERPOLATED_REGULAR_STRING_START:
			case INTERPOLATED_VERBATIUM_STRING_START:
			case OPEN_PARENS:
			case PLUS:
			case MINUS:
			case STAR:
			case AMP:
			case BANG:
			case TILDE:
			case OP_INC:
			case OP_DEC:
				enterOuterAlt(_localctx, 1);
				{
				setState(2121);
				interpolated_string_expression();
				}
				break;
			case DOUBLE_CURLY_INSIDE:
				enterOuterAlt(_localctx, 2);
				{
				setState(2122);
				match(DOUBLE_CURLY_INSIDE);
				}
				break;
			case VERBATIUM_DOUBLE_QUOTE_INSIDE:
				enterOuterAlt(_localctx, 3);
				{
				setState(2123);
				match(VERBATIUM_DOUBLE_QUOTE_INSIDE);
				}
				break;
			case VERBATIUM_INSIDE_STRING:
				enterOuterAlt(_localctx, 4);
				{
				setState(2124);
				match(VERBATIUM_INSIDE_STRING);
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Interpolated_string_expressionContext extends ParserRuleContext {
		public List<ExpressionContext> expression() {
			return getRuleContexts(ExpressionContext.class);
		}
		public ExpressionContext expression(int i) {
			return getRuleContext(ExpressionContext.class,i);
		}
		public List<TerminalNode> FORMAT_STRING() { return getTokens(CSharpParser.FORMAT_STRING); }
		public TerminalNode FORMAT_STRING(int i) {
			return getToken(CSharpParser.FORMAT_STRING, i);
		}
		public Interpolated_string_expressionContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_interpolated_string_expression; }
	}

	public final Interpolated_string_expressionContext interpolated_string_expression() throws RecognitionException {
		Interpolated_string_expressionContext _localctx = new Interpolated_string_expressionContext(_ctx, getState());
		enterRule(_localctx, 358, RULE_interpolated_string_expression);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(2127);
			expression();
			setState(2132);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while (_la==COMMA) {
				{
				{
				setState(2128);
				match(COMMA);
				setState(2129);
				expression();
				}
				}
				setState(2134);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			setState(2141);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==COLON) {
				{
				setState(2135);
				match(COLON);
				setState(2137); 
				_errHandler.sync(this);
				_la = _input.LA(1);
				do {
					{
					{
					setState(2136);
					match(FORMAT_STRING);
					}
					}
					setState(2139); 
					_errHandler.sync(this);
					_la = _input.LA(1);
				} while ( _la==FORMAT_STRING );
				}
			}

			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class KeywordContext extends ParserRuleContext {
		public TerminalNode ABSTRACT() { return getToken(CSharpParser.ABSTRACT, 0); }
		public TerminalNode AS() { return getToken(CSharpParser.AS, 0); }
		public TerminalNode BASE() { return getToken(CSharpParser.BASE, 0); }
		public TerminalNode BOOL() { return getToken(CSharpParser.BOOL, 0); }
		public TerminalNode BREAK() { return getToken(CSharpParser.BREAK, 0); }
		public TerminalNode BYTE() { return getToken(CSharpParser.BYTE, 0); }
		public TerminalNode CASE() { return getToken(CSharpParser.CASE, 0); }
		public TerminalNode CATCH() { return getToken(CSharpParser.CATCH, 0); }
		public TerminalNode CHAR() { return getToken(CSharpParser.CHAR, 0); }
		public TerminalNode CHECKED() { return getToken(CSharpParser.CHECKED, 0); }
		public TerminalNode CLASS() { return getToken(CSharpParser.CLASS, 0); }
		public TerminalNode CONST() { return getToken(CSharpParser.CONST, 0); }
		public TerminalNode CONTINUE() { return getToken(CSharpParser.CONTINUE, 0); }
		public TerminalNode DECIMAL() { return getToken(CSharpParser.DECIMAL, 0); }
		public TerminalNode DEFAULT() { return getToken(CSharpParser.DEFAULT, 0); }
		public TerminalNode DELEGATE() { return getToken(CSharpParser.DELEGATE, 0); }
		public TerminalNode DO() { return getToken(CSharpParser.DO, 0); }
		public TerminalNode DOUBLE() { return getToken(CSharpParser.DOUBLE, 0); }
		public TerminalNode ELSE() { return getToken(CSharpParser.ELSE, 0); }
		public TerminalNode ENUM() { return getToken(CSharpParser.ENUM, 0); }
		public TerminalNode EVENT() { return getToken(CSharpParser.EVENT, 0); }
		public TerminalNode EXPLICIT() { return getToken(CSharpParser.EXPLICIT, 0); }
		public TerminalNode EXTERN() { return getToken(CSharpParser.EXTERN, 0); }
		public TerminalNode FALSE() { return getToken(CSharpParser.FALSE, 0); }
		public TerminalNode FINALLY() { return getToken(CSharpParser.FINALLY, 0); }
		public TerminalNode FIXED() { return getToken(CSharpParser.FIXED, 0); }
		public TerminalNode FLOAT() { return getToken(CSharpParser.FLOAT, 0); }
		public TerminalNode FOR() { return getToken(CSharpParser.FOR, 0); }
		public TerminalNode FOREACH() { return getToken(CSharpParser.FOREACH, 0); }
		public TerminalNode GOTO() { return getToken(CSharpParser.GOTO, 0); }
		public TerminalNode IF() { return getToken(CSharpParser.IF, 0); }
		public TerminalNode IMPLICIT() { return getToken(CSharpParser.IMPLICIT, 0); }
		public TerminalNode IN() { return getToken(CSharpParser.IN, 0); }
		public TerminalNode INT() { return getToken(CSharpParser.INT, 0); }
		public TerminalNode INTERFACE() { return getToken(CSharpParser.INTERFACE, 0); }
		public TerminalNode INTERNAL() { return getToken(CSharpParser.INTERNAL, 0); }
		public TerminalNode IS() { return getToken(CSharpParser.IS, 0); }
		public TerminalNode LOCK() { return getToken(CSharpParser.LOCK, 0); }
		public TerminalNode LONG() { return getToken(CSharpParser.LONG, 0); }
		public TerminalNode NAMESPACE() { return getToken(CSharpParser.NAMESPACE, 0); }
		public TerminalNode NEW() { return getToken(CSharpParser.NEW, 0); }
		public TerminalNode NULL() { return getToken(CSharpParser.NULL, 0); }
		public TerminalNode OBJECT() { return getToken(CSharpParser.OBJECT, 0); }
		public TerminalNode OPERATOR() { return getToken(CSharpParser.OPERATOR, 0); }
		public TerminalNode OUT() { return getToken(CSharpParser.OUT, 0); }
		public TerminalNode OVERRIDE() { return getToken(CSharpParser.OVERRIDE, 0); }
		public TerminalNode PARAMS() { return getToken(CSharpParser.PARAMS, 0); }
		public TerminalNode PRIVATE() { return getToken(CSharpParser.PRIVATE, 0); }
		public TerminalNode PROTECTED() { return getToken(CSharpParser.PROTECTED, 0); }
		public TerminalNode PUBLIC() { return getToken(CSharpParser.PUBLIC, 0); }
		public TerminalNode READONLY() { return getToken(CSharpParser.READONLY, 0); }
		public TerminalNode REF() { return getToken(CSharpParser.REF, 0); }
		public TerminalNode RETURN() { return getToken(CSharpParser.RETURN, 0); }
		public TerminalNode SBYTE() { return getToken(CSharpParser.SBYTE, 0); }
		public TerminalNode SEALED() { return getToken(CSharpParser.SEALED, 0); }
		public TerminalNode SHORT() { return getToken(CSharpParser.SHORT, 0); }
		public TerminalNode SIZEOF() { return getToken(CSharpParser.SIZEOF, 0); }
		public TerminalNode STACKALLOC() { return getToken(CSharpParser.STACKALLOC, 0); }
		public TerminalNode STATIC() { return getToken(CSharpParser.STATIC, 0); }
		public TerminalNode STRING() { return getToken(CSharpParser.STRING, 0); }
		public TerminalNode STRUCT() { return getToken(CSharpParser.STRUCT, 0); }
		public TerminalNode SWITCH() { return getToken(CSharpParser.SWITCH, 0); }
		public TerminalNode THIS() { return getToken(CSharpParser.THIS, 0); }
		public TerminalNode THROW() { return getToken(CSharpParser.THROW, 0); }
		public TerminalNode TRUE() { return getToken(CSharpParser.TRUE, 0); }
		public TerminalNode TRY() { return getToken(CSharpParser.TRY, 0); }
		public TerminalNode TYPEOF() { return getToken(CSharpParser.TYPEOF, 0); }
		public TerminalNode UINT() { return getToken(CSharpParser.UINT, 0); }
		public TerminalNode ULONG() { return getToken(CSharpParser.ULONG, 0); }
		public TerminalNode UNCHECKED() { return getToken(CSharpParser.UNCHECKED, 0); }
		public TerminalNode UNSAFE() { return getToken(CSharpParser.UNSAFE, 0); }
		public TerminalNode USHORT() { return getToken(CSharpParser.USHORT, 0); }
		public TerminalNode USING() { return getToken(CSharpParser.USING, 0); }
		public TerminalNode VIRTUAL() { return getToken(CSharpParser.VIRTUAL, 0); }
		public TerminalNode VOID() { return getToken(CSharpParser.VOID, 0); }
		public TerminalNode VOLATILE() { return getToken(CSharpParser.VOLATILE, 0); }
		public TerminalNode WHILE() { return getToken(CSharpParser.WHILE, 0); }
		public KeywordContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_keyword; }
	}

	public final KeywordContext keyword() throws RecognitionException {
		KeywordContext _localctx = new KeywordContext(_ctx, getState());
		enterRule(_localctx, 360, RULE_keyword);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(2143);
			_la = _input.LA(1);
			if ( !((((_la) & ~0x3f) == 0 && ((1L << _la) & ((1L << ABSTRACT) | (1L << AS) | (1L << BASE) | (1L << BOOL) | (1L << BREAK) | (1L << BYTE) | (1L << CASE) | (1L << CATCH) | (1L << CHAR) | (1L << CHECKED) | (1L << CLASS) | (1L << CONST) | (1L << CONTINUE) | (1L << DECIMAL) | (1L << DEFAULT) | (1L << DELEGATE) | (1L << DO) | (1L << DOUBLE) | (1L << ELSE) | (1L << ENUM) | (1L << EVENT) | (1L << EXPLICIT) | (1L << EXTERN) | (1L << FALSE) | (1L << FINALLY) | (1L << FIXED) | (1L << FLOAT) | (1L << FOR) | (1L << FOREACH) | (1L << GOTO) | (1L << IF) | (1L << IMPLICIT) | (1L << IN) | (1L << INT) | (1L << INTERFACE) | (1L << INTERNAL) | (1L << IS) | (1L << LOCK) | (1L << LONG))) != 0) || ((((_la - 64)) & ~0x3f) == 0 && ((1L << (_la - 64)) & ((1L << (NAMESPACE - 64)) | (1L << (NEW - 64)) | (1L << (NULL - 64)) | (1L << (OBJECT - 64)) | (1L << (OPERATOR - 64)) | (1L << (OUT - 64)) | (1L << (OVERRIDE - 64)) | (1L << (PARAMS - 64)) | (1L << (PRIVATE - 64)) | (1L << (PROTECTED - 64)) | (1L << (PUBLIC - 64)) | (1L << (READONLY - 64)) | (1L << (REF - 64)) | (1L << (RETURN - 64)) | (1L << (SBYTE - 64)) | (1L << (SEALED - 64)) | (1L << (SHORT - 64)) | (1L << (SIZEOF - 64)) | (1L << (STACKALLOC - 64)) | (1L << (STATIC - 64)) | (1L << (STRING - 64)) | (1L << (STRUCT - 64)) | (1L << (SWITCH - 64)) | (1L << (THIS - 64)) | (1L << (THROW - 64)) | (1L << (TRUE - 64)) | (1L << (TRY - 64)) | (1L << (TYPEOF - 64)) | (1L << (UINT - 64)) | (1L << (ULONG - 64)) | (1L << (UNCHECKED - 64)) | (1L << (UNSAFE - 64)) | (1L << (USHORT - 64)) | (1L << (USING - 64)) | (1L << (VIRTUAL - 64)) | (1L << (VOID - 64)) | (1L << (VOLATILE - 64)) | (1L << (WHILE - 64)))) != 0)) ) {
			_errHandler.recoverInline(this);
			}
			else {
				if ( _input.LA(1)==Token.EOF ) matchedEOF = true;
				_errHandler.reportMatch(this);
				consume();
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Class_definitionContext extends ParserRuleContext {
		public TerminalNode CLASS() { return getToken(CSharpParser.CLASS, 0); }
		public IdentifierContext identifier() {
			return getRuleContext(IdentifierContext.class,0);
		}
		public Class_bodyContext class_body() {
			return getRuleContext(Class_bodyContext.class,0);
		}
		public Type_parameter_listContext type_parameter_list() {
			return getRuleContext(Type_parameter_listContext.class,0);
		}
		public Class_baseContext class_base() {
			return getRuleContext(Class_baseContext.class,0);
		}
		public Type_parameter_constraints_clausesContext type_parameter_constraints_clauses() {
			return getRuleContext(Type_parameter_constraints_clausesContext.class,0);
		}
		public Class_definitionContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_class_definition; }
	}

	public final Class_definitionContext class_definition() throws RecognitionException {
		Class_definitionContext _localctx = new Class_definitionContext(_ctx, getState());
		enterRule(_localctx, 362, RULE_class_definition);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(2145);
			match(CLASS);
			setState(2146);
			identifier();
			setState(2148);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==LT) {
				{
				setState(2147);
				type_parameter_list();
				}
			}

			setState(2151);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==COLON) {
				{
				setState(2150);
				class_base();
				}
			}

			setState(2154);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==WHERE) {
				{
				setState(2153);
				type_parameter_constraints_clauses();
				}
			}

			setState(2156);
			class_body();
			setState(2158);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==SEMICOLON) {
				{
				setState(2157);
				match(SEMICOLON);
				}
			}

			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Struct_definitionContext extends ParserRuleContext {
		public TerminalNode STRUCT() { return getToken(CSharpParser.STRUCT, 0); }
		public IdentifierContext identifier() {
			return getRuleContext(IdentifierContext.class,0);
		}
		public Struct_bodyContext struct_body() {
			return getRuleContext(Struct_bodyContext.class,0);
		}
		public Type_parameter_listContext type_parameter_list() {
			return getRuleContext(Type_parameter_listContext.class,0);
		}
		public Struct_interfacesContext struct_interfaces() {
			return getRuleContext(Struct_interfacesContext.class,0);
		}
		public Type_parameter_constraints_clausesContext type_parameter_constraints_clauses() {
			return getRuleContext(Type_parameter_constraints_clausesContext.class,0);
		}
		public Struct_definitionContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_struct_definition; }
	}

	public final Struct_definitionContext struct_definition() throws RecognitionException {
		Struct_definitionContext _localctx = new Struct_definitionContext(_ctx, getState());
		enterRule(_localctx, 364, RULE_struct_definition);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(2160);
			match(STRUCT);
			setState(2161);
			identifier();
			setState(2163);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==LT) {
				{
				setState(2162);
				type_parameter_list();
				}
			}

			setState(2166);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==COLON) {
				{
				setState(2165);
				struct_interfaces();
				}
			}

			setState(2169);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==WHERE) {
				{
				setState(2168);
				type_parameter_constraints_clauses();
				}
			}

			setState(2171);
			struct_body();
			setState(2173);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==SEMICOLON) {
				{
				setState(2172);
				match(SEMICOLON);
				}
			}

			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Interface_definitionContext extends ParserRuleContext {
		public TerminalNode INTERFACE() { return getToken(CSharpParser.INTERFACE, 0); }
		public IdentifierContext identifier() {
			return getRuleContext(IdentifierContext.class,0);
		}
		public Interface_bodyContext interface_body() {
			return getRuleContext(Interface_bodyContext.class,0);
		}
		public Variant_type_parameter_listContext variant_type_parameter_list() {
			return getRuleContext(Variant_type_parameter_listContext.class,0);
		}
		public Interface_baseContext interface_base() {
			return getRuleContext(Interface_baseContext.class,0);
		}
		public Type_parameter_constraints_clausesContext type_parameter_constraints_clauses() {
			return getRuleContext(Type_parameter_constraints_clausesContext.class,0);
		}
		public Interface_definitionContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_interface_definition; }
	}

	public final Interface_definitionContext interface_definition() throws RecognitionException {
		Interface_definitionContext _localctx = new Interface_definitionContext(_ctx, getState());
		enterRule(_localctx, 366, RULE_interface_definition);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(2175);
			match(INTERFACE);
			setState(2176);
			identifier();
			setState(2178);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==LT) {
				{
				setState(2177);
				variant_type_parameter_list();
				}
			}

			setState(2181);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==COLON) {
				{
				setState(2180);
				interface_base();
				}
			}

			setState(2184);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==WHERE) {
				{
				setState(2183);
				type_parameter_constraints_clauses();
				}
			}

			setState(2186);
			interface_body();
			setState(2188);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==SEMICOLON) {
				{
				setState(2187);
				match(SEMICOLON);
				}
			}

			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Enum_definitionContext extends ParserRuleContext {
		public TerminalNode ENUM() { return getToken(CSharpParser.ENUM, 0); }
		public IdentifierContext identifier() {
			return getRuleContext(IdentifierContext.class,0);
		}
		public Enum_bodyContext enum_body() {
			return getRuleContext(Enum_bodyContext.class,0);
		}
		public Enum_baseContext enum_base() {
			return getRuleContext(Enum_baseContext.class,0);
		}
		public Enum_definitionContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_enum_definition; }
	}

	public final Enum_definitionContext enum_definition() throws RecognitionException {
		Enum_definitionContext _localctx = new Enum_definitionContext(_ctx, getState());
		enterRule(_localctx, 368, RULE_enum_definition);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(2190);
			match(ENUM);
			setState(2191);
			identifier();
			setState(2193);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==COLON) {
				{
				setState(2192);
				enum_base();
				}
			}

			setState(2195);
			enum_body();
			setState(2197);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==SEMICOLON) {
				{
				setState(2196);
				match(SEMICOLON);
				}
			}

			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Delegate_definitionContext extends ParserRuleContext {
		public TerminalNode DELEGATE() { return getToken(CSharpParser.DELEGATE, 0); }
		public Return_typeContext return_type() {
			return getRuleContext(Return_typeContext.class,0);
		}
		public IdentifierContext identifier() {
			return getRuleContext(IdentifierContext.class,0);
		}
		public TerminalNode OPEN_PARENS() { return getToken(CSharpParser.OPEN_PARENS, 0); }
		public TerminalNode CLOSE_PARENS() { return getToken(CSharpParser.CLOSE_PARENS, 0); }
		public Variant_type_parameter_listContext variant_type_parameter_list() {
			return getRuleContext(Variant_type_parameter_listContext.class,0);
		}
		public Formal_parameter_listContext formal_parameter_list() {
			return getRuleContext(Formal_parameter_listContext.class,0);
		}
		public Type_parameter_constraints_clausesContext type_parameter_constraints_clauses() {
			return getRuleContext(Type_parameter_constraints_clausesContext.class,0);
		}
		public Delegate_definitionContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_delegate_definition; }
	}

	public final Delegate_definitionContext delegate_definition() throws RecognitionException {
		Delegate_definitionContext _localctx = new Delegate_definitionContext(_ctx, getState());
		enterRule(_localctx, 370, RULE_delegate_definition);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(2199);
			match(DELEGATE);
			setState(2200);
			return_type();
			setState(2201);
			identifier();
			setState(2203);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==LT) {
				{
				setState(2202);
				variant_type_parameter_list();
				}
			}

			setState(2205);
			match(OPEN_PARENS);
			setState(2207);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if ((((_la) & ~0x3f) == 0 && ((1L << _la) & ((1L << ADD) | (1L << ALIAS) | (1L << ARGLIST) | (1L << ASCENDING) | (1L << ASYNC) | (1L << AWAIT) | (1L << BOOL) | (1L << BY) | (1L << BYTE) | (1L << CHAR) | (1L << DECIMAL) | (1L << DESCENDING) | (1L << DOUBLE) | (1L << DYNAMIC) | (1L << EQUALS) | (1L << FLOAT) | (1L << FROM) | (1L << GET) | (1L << GROUP) | (1L << INT) | (1L << INTO) | (1L << JOIN) | (1L << LET) | (1L << LONG) | (1L << NAMEOF))) != 0) || ((((_la - 67)) & ~0x3f) == 0 && ((1L << (_la - 67)) & ((1L << (OBJECT - 67)) | (1L << (ON - 67)) | (1L << (ORDERBY - 67)) | (1L << (OUT - 67)) | (1L << (PARAMS - 67)) | (1L << (PARTIAL - 67)) | (1L << (REF - 67)) | (1L << (REMOVE - 67)) | (1L << (SBYTE - 67)) | (1L << (SELECT - 67)) | (1L << (SET - 67)) | (1L << (SHORT - 67)) | (1L << (STRING - 67)) | (1L << (THIS - 67)) | (1L << (UINT - 67)) | (1L << (ULONG - 67)) | (1L << (USHORT - 67)) | (1L << (VOID - 67)) | (1L << (WHEN - 67)) | (1L << (WHERE - 67)) | (1L << (YIELD - 67)) | (1L << (IDENTIFIER - 67)) | (1L << (OPEN_BRACKET - 67)))) != 0)) {
				{
				setState(2206);
				formal_parameter_list();
				}
			}

			setState(2209);
			match(CLOSE_PARENS);
			setState(2211);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==WHERE) {
				{
				setState(2210);
				type_parameter_constraints_clauses();
				}
			}

			setState(2213);
			match(SEMICOLON);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Event_declarationContext extends ParserRuleContext {
		public TerminalNode EVENT() { return getToken(CSharpParser.EVENT, 0); }
		public TypeContext type() {
			return getRuleContext(TypeContext.class,0);
		}
		public Variable_declaratorsContext variable_declarators() {
			return getRuleContext(Variable_declaratorsContext.class,0);
		}
		public Member_nameContext member_name() {
			return getRuleContext(Member_nameContext.class,0);
		}
		public TerminalNode OPEN_BRACE() { return getToken(CSharpParser.OPEN_BRACE, 0); }
		public Event_accessor_declarationsContext event_accessor_declarations() {
			return getRuleContext(Event_accessor_declarationsContext.class,0);
		}
		public TerminalNode CLOSE_BRACE() { return getToken(CSharpParser.CLOSE_BRACE, 0); }
		public Event_declarationContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_event_declaration; }
	}

	public final Event_declarationContext event_declaration() throws RecognitionException {
		Event_declarationContext _localctx = new Event_declarationContext(_ctx, getState());
		enterRule(_localctx, 372, RULE_event_declaration);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(2215);
			match(EVENT);
			setState(2216);
			type();
			setState(2225);
			_errHandler.sync(this);
			switch ( getInterpreter().adaptivePredict(_input,277,_ctx) ) {
			case 1:
				{
				setState(2217);
				variable_declarators();
				setState(2218);
				match(SEMICOLON);
				}
				break;
			case 2:
				{
				setState(2220);
				member_name();
				setState(2221);
				match(OPEN_BRACE);
				setState(2222);
				event_accessor_declarations();
				setState(2223);
				match(CLOSE_BRACE);
				}
				break;
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Field_declarationContext extends ParserRuleContext {
		public Variable_declaratorsContext variable_declarators() {
			return getRuleContext(Variable_declaratorsContext.class,0);
		}
		public Field_declarationContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_field_declaration; }
	}

	public final Field_declarationContext field_declaration() throws RecognitionException {
		Field_declarationContext _localctx = new Field_declarationContext(_ctx, getState());
		enterRule(_localctx, 374, RULE_field_declaration);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(2227);
			variable_declarators();
			setState(2228);
			match(SEMICOLON);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Property_declarationContext extends ParserRuleContext {
		public Member_nameContext member_name() {
			return getRuleContext(Member_nameContext.class,0);
		}
		public TerminalNode OPEN_BRACE() { return getToken(CSharpParser.OPEN_BRACE, 0); }
		public Accessor_declarationsContext accessor_declarations() {
			return getRuleContext(Accessor_declarationsContext.class,0);
		}
		public TerminalNode CLOSE_BRACE() { return getToken(CSharpParser.CLOSE_BRACE, 0); }
		public Right_arrowContext right_arrow() {
			return getRuleContext(Right_arrowContext.class,0);
		}
		public ExpressionContext expression() {
			return getRuleContext(ExpressionContext.class,0);
		}
		public Variable_initializerContext variable_initializer() {
			return getRuleContext(Variable_initializerContext.class,0);
		}
		public Property_declarationContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_property_declaration; }
	}

	public final Property_declarationContext property_declaration() throws RecognitionException {
		Property_declarationContext _localctx = new Property_declarationContext(_ctx, getState());
		enterRule(_localctx, 376, RULE_property_declaration);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(2230);
			member_name();
			setState(2244);
			_errHandler.sync(this);
			switch (_input.LA(1)) {
			case OPEN_BRACE:
				{
				setState(2231);
				match(OPEN_BRACE);
				setState(2232);
				accessor_declarations();
				setState(2233);
				match(CLOSE_BRACE);
				setState(2238);
				_errHandler.sync(this);
				_la = _input.LA(1);
				if (_la==ASSIGNMENT) {
					{
					setState(2234);
					match(ASSIGNMENT);
					setState(2235);
					variable_initializer();
					setState(2236);
					match(SEMICOLON);
					}
				}

				}
				break;
			case ASSIGNMENT:
				{
				setState(2240);
				right_arrow();
				setState(2241);
				expression();
				setState(2242);
				match(SEMICOLON);
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Constant_declarationContext extends ParserRuleContext {
		public TerminalNode CONST() { return getToken(CSharpParser.CONST, 0); }
		public TypeContext type() {
			return getRuleContext(TypeContext.class,0);
		}
		public Constant_declaratorsContext constant_declarators() {
			return getRuleContext(Constant_declaratorsContext.class,0);
		}
		public Constant_declarationContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_constant_declaration; }
	}

	public final Constant_declarationContext constant_declaration() throws RecognitionException {
		Constant_declarationContext _localctx = new Constant_declarationContext(_ctx, getState());
		enterRule(_localctx, 378, RULE_constant_declaration);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(2246);
			match(CONST);
			setState(2247);
			type();
			setState(2248);
			constant_declarators();
			setState(2249);
			match(SEMICOLON);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Indexer_declarationContext extends ParserRuleContext {
		public TerminalNode THIS() { return getToken(CSharpParser.THIS, 0); }
		public Formal_parameter_listContext formal_parameter_list() {
			return getRuleContext(Formal_parameter_listContext.class,0);
		}
		public TerminalNode OPEN_BRACE() { return getToken(CSharpParser.OPEN_BRACE, 0); }
		public Accessor_declarationsContext accessor_declarations() {
			return getRuleContext(Accessor_declarationsContext.class,0);
		}
		public TerminalNode CLOSE_BRACE() { return getToken(CSharpParser.CLOSE_BRACE, 0); }
		public Right_arrowContext right_arrow() {
			return getRuleContext(Right_arrowContext.class,0);
		}
		public ExpressionContext expression() {
			return getRuleContext(ExpressionContext.class,0);
		}
		public Indexer_declarationContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_indexer_declaration; }
	}

	public final Indexer_declarationContext indexer_declaration() throws RecognitionException {
		Indexer_declarationContext _localctx = new Indexer_declarationContext(_ctx, getState());
		enterRule(_localctx, 380, RULE_indexer_declaration);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(2251);
			match(THIS);
			setState(2252);
			match(OPEN_BRACKET);
			setState(2253);
			formal_parameter_list();
			setState(2254);
			match(CLOSE_BRACKET);
			setState(2263);
			_errHandler.sync(this);
			switch (_input.LA(1)) {
			case OPEN_BRACE:
				{
				setState(2255);
				match(OPEN_BRACE);
				setState(2256);
				accessor_declarations();
				setState(2257);
				match(CLOSE_BRACE);
				}
				break;
			case ASSIGNMENT:
				{
				setState(2259);
				right_arrow();
				setState(2260);
				expression();
				setState(2261);
				match(SEMICOLON);
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Destructor_definitionContext extends ParserRuleContext {
		public IdentifierContext identifier() {
			return getRuleContext(IdentifierContext.class,0);
		}
		public TerminalNode OPEN_PARENS() { return getToken(CSharpParser.OPEN_PARENS, 0); }
		public TerminalNode CLOSE_PARENS() { return getToken(CSharpParser.CLOSE_PARENS, 0); }
		public BodyContext body() {
			return getRuleContext(BodyContext.class,0);
		}
		public Destructor_definitionContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_destructor_definition; }
	}

	public final Destructor_definitionContext destructor_definition() throws RecognitionException {
		Destructor_definitionContext _localctx = new Destructor_definitionContext(_ctx, getState());
		enterRule(_localctx, 382, RULE_destructor_definition);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(2265);
			match(TILDE);
			setState(2266);
			identifier();
			setState(2267);
			match(OPEN_PARENS);
			setState(2268);
			match(CLOSE_PARENS);
			setState(2269);
			body();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Constructor_declarationContext extends ParserRuleContext {
		public IdentifierContext identifier() {
			return getRuleContext(IdentifierContext.class,0);
		}
		public TerminalNode OPEN_PARENS() { return getToken(CSharpParser.OPEN_PARENS, 0); }
		public TerminalNode CLOSE_PARENS() { return getToken(CSharpParser.CLOSE_PARENS, 0); }
		public BodyContext body() {
			return getRuleContext(BodyContext.class,0);
		}
		public Formal_parameter_listContext formal_parameter_list() {
			return getRuleContext(Formal_parameter_listContext.class,0);
		}
		public Constructor_initializerContext constructor_initializer() {
			return getRuleContext(Constructor_initializerContext.class,0);
		}
		public Constructor_declarationContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_constructor_declaration; }
	}

	public final Constructor_declarationContext constructor_declaration() throws RecognitionException {
		Constructor_declarationContext _localctx = new Constructor_declarationContext(_ctx, getState());
		enterRule(_localctx, 384, RULE_constructor_declaration);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(2271);
			identifier();
			setState(2272);
			match(OPEN_PARENS);
			setState(2274);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if ((((_la) & ~0x3f) == 0 && ((1L << _la) & ((1L << ADD) | (1L << ALIAS) | (1L << ARGLIST) | (1L << ASCENDING) | (1L << ASYNC) | (1L << AWAIT) | (1L << BOOL) | (1L << BY) | (1L << BYTE) | (1L << CHAR) | (1L << DECIMAL) | (1L << DESCENDING) | (1L << DOUBLE) | (1L << DYNAMIC) | (1L << EQUALS) | (1L << FLOAT) | (1L << FROM) | (1L << GET) | (1L << GROUP) | (1L << INT) | (1L << INTO) | (1L << JOIN) | (1L << LET) | (1L << LONG) | (1L << NAMEOF))) != 0) || ((((_la - 67)) & ~0x3f) == 0 && ((1L << (_la - 67)) & ((1L << (OBJECT - 67)) | (1L << (ON - 67)) | (1L << (ORDERBY - 67)) | (1L << (OUT - 67)) | (1L << (PARAMS - 67)) | (1L << (PARTIAL - 67)) | (1L << (REF - 67)) | (1L << (REMOVE - 67)) | (1L << (SBYTE - 67)) | (1L << (SELECT - 67)) | (1L << (SET - 67)) | (1L << (SHORT - 67)) | (1L << (STRING - 67)) | (1L << (THIS - 67)) | (1L << (UINT - 67)) | (1L << (ULONG - 67)) | (1L << (USHORT - 67)) | (1L << (VOID - 67)) | (1L << (WHEN - 67)) | (1L << (WHERE - 67)) | (1L << (YIELD - 67)) | (1L << (IDENTIFIER - 67)) | (1L << (OPEN_BRACKET - 67)))) != 0)) {
				{
				setState(2273);
				formal_parameter_list();
				}
			}

			setState(2276);
			match(CLOSE_PARENS);
			setState(2278);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==COLON) {
				{
				setState(2277);
				constructor_initializer();
				}
			}

			setState(2280);
			body();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Method_declarationContext extends ParserRuleContext {
		public Method_member_nameContext method_member_name() {
			return getRuleContext(Method_member_nameContext.class,0);
		}
		public TerminalNode OPEN_PARENS() { return getToken(CSharpParser.OPEN_PARENS, 0); }
		public TerminalNode CLOSE_PARENS() { return getToken(CSharpParser.CLOSE_PARENS, 0); }
		public Method_bodyContext method_body() {
			return getRuleContext(Method_bodyContext.class,0);
		}
		public Right_arrowContext right_arrow() {
			return getRuleContext(Right_arrowContext.class,0);
		}
		public ExpressionContext expression() {
			return getRuleContext(ExpressionContext.class,0);
		}
		public Type_parameter_listContext type_parameter_list() {
			return getRuleContext(Type_parameter_listContext.class,0);
		}
		public Formal_parameter_listContext formal_parameter_list() {
			return getRuleContext(Formal_parameter_listContext.class,0);
		}
		public Type_parameter_constraints_clausesContext type_parameter_constraints_clauses() {
			return getRuleContext(Type_parameter_constraints_clausesContext.class,0);
		}
		public Method_declarationContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_method_declaration; }
	}

	public final Method_declarationContext method_declaration() throws RecognitionException {
		Method_declarationContext _localctx = new Method_declarationContext(_ctx, getState());
		enterRule(_localctx, 386, RULE_method_declaration);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(2282);
			method_member_name();
			setState(2284);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==LT) {
				{
				setState(2283);
				type_parameter_list();
				}
			}

			setState(2286);
			match(OPEN_PARENS);
			setState(2288);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if ((((_la) & ~0x3f) == 0 && ((1L << _la) & ((1L << ADD) | (1L << ALIAS) | (1L << ARGLIST) | (1L << ASCENDING) | (1L << ASYNC) | (1L << AWAIT) | (1L << BOOL) | (1L << BY) | (1L << BYTE) | (1L << CHAR) | (1L << DECIMAL) | (1L << DESCENDING) | (1L << DOUBLE) | (1L << DYNAMIC) | (1L << EQUALS) | (1L << FLOAT) | (1L << FROM) | (1L << GET) | (1L << GROUP) | (1L << INT) | (1L << INTO) | (1L << JOIN) | (1L << LET) | (1L << LONG) | (1L << NAMEOF))) != 0) || ((((_la - 67)) & ~0x3f) == 0 && ((1L << (_la - 67)) & ((1L << (OBJECT - 67)) | (1L << (ON - 67)) | (1L << (ORDERBY - 67)) | (1L << (OUT - 67)) | (1L << (PARAMS - 67)) | (1L << (PARTIAL - 67)) | (1L << (REF - 67)) | (1L << (REMOVE - 67)) | (1L << (SBYTE - 67)) | (1L << (SELECT - 67)) | (1L << (SET - 67)) | (1L << (SHORT - 67)) | (1L << (STRING - 67)) | (1L << (THIS - 67)) | (1L << (UINT - 67)) | (1L << (ULONG - 67)) | (1L << (USHORT - 67)) | (1L << (VOID - 67)) | (1L << (WHEN - 67)) | (1L << (WHERE - 67)) | (1L << (YIELD - 67)) | (1L << (IDENTIFIER - 67)) | (1L << (OPEN_BRACKET - 67)))) != 0)) {
				{
				setState(2287);
				formal_parameter_list();
				}
			}

			setState(2290);
			match(CLOSE_PARENS);
			setState(2292);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==WHERE) {
				{
				setState(2291);
				type_parameter_constraints_clauses();
				}
			}

			setState(2299);
			_errHandler.sync(this);
			switch (_input.LA(1)) {
			case OPEN_BRACE:
			case SEMICOLON:
				{
				setState(2294);
				method_body();
				}
				break;
			case ASSIGNMENT:
				{
				setState(2295);
				right_arrow();
				setState(2296);
				expression();
				setState(2297);
				match(SEMICOLON);
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Method_member_nameContext extends ParserRuleContext {
		public List<IdentifierContext> identifier() {
			return getRuleContexts(IdentifierContext.class);
		}
		public IdentifierContext identifier(int i) {
			return getRuleContext(IdentifierContext.class,i);
		}
		public List<Type_argument_listContext> type_argument_list() {
			return getRuleContexts(Type_argument_listContext.class);
		}
		public Type_argument_listContext type_argument_list(int i) {
			return getRuleContext(Type_argument_listContext.class,i);
		}
		public Method_member_nameContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_method_member_name; }
	}

	public final Method_member_nameContext method_member_name() throws RecognitionException {
		Method_member_nameContext _localctx = new Method_member_nameContext(_ctx, getState());
		enterRule(_localctx, 388, RULE_method_member_name);
		int _la;
		try {
			int _alt;
			enterOuterAlt(_localctx, 1);
			{
			setState(2306);
			_errHandler.sync(this);
			switch ( getInterpreter().adaptivePredict(_input,287,_ctx) ) {
			case 1:
				{
				setState(2301);
				identifier();
				}
				break;
			case 2:
				{
				setState(2302);
				identifier();
				setState(2303);
				match(DOUBLE_COLON);
				setState(2304);
				identifier();
				}
				break;
			}
			setState(2315);
			_errHandler.sync(this);
			_alt = getInterpreter().adaptivePredict(_input,289,_ctx);
			while ( _alt!=2 && _alt!=org.antlr.v4.runtime.atn.ATN.INVALID_ALT_NUMBER ) {
				if ( _alt==1 ) {
					{
					{
					setState(2309);
					_errHandler.sync(this);
					_la = _input.LA(1);
					if (_la==LT) {
						{
						setState(2308);
						type_argument_list();
						}
					}

					setState(2311);
					match(DOT);
					setState(2312);
					identifier();
					}
					} 
				}
				setState(2317);
				_errHandler.sync(this);
				_alt = getInterpreter().adaptivePredict(_input,289,_ctx);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Operator_declarationContext extends ParserRuleContext {
		public TerminalNode OPERATOR() { return getToken(CSharpParser.OPERATOR, 0); }
		public Overloadable_operatorContext overloadable_operator() {
			return getRuleContext(Overloadable_operatorContext.class,0);
		}
		public TerminalNode OPEN_PARENS() { return getToken(CSharpParser.OPEN_PARENS, 0); }
		public List<Arg_declarationContext> arg_declaration() {
			return getRuleContexts(Arg_declarationContext.class);
		}
		public Arg_declarationContext arg_declaration(int i) {
			return getRuleContext(Arg_declarationContext.class,i);
		}
		public TerminalNode CLOSE_PARENS() { return getToken(CSharpParser.CLOSE_PARENS, 0); }
		public BodyContext body() {
			return getRuleContext(BodyContext.class,0);
		}
		public Right_arrowContext right_arrow() {
			return getRuleContext(Right_arrowContext.class,0);
		}
		public ExpressionContext expression() {
			return getRuleContext(ExpressionContext.class,0);
		}
		public Operator_declarationContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_operator_declaration; }
	}

	public final Operator_declarationContext operator_declaration() throws RecognitionException {
		Operator_declarationContext _localctx = new Operator_declarationContext(_ctx, getState());
		enterRule(_localctx, 390, RULE_operator_declaration);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(2318);
			match(OPERATOR);
			setState(2319);
			overloadable_operator();
			setState(2320);
			match(OPEN_PARENS);
			setState(2321);
			arg_declaration();
			setState(2324);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==COMMA) {
				{
				setState(2322);
				match(COMMA);
				setState(2323);
				arg_declaration();
				}
			}

			setState(2326);
			match(CLOSE_PARENS);
			setState(2332);
			_errHandler.sync(this);
			switch (_input.LA(1)) {
			case OPEN_BRACE:
			case SEMICOLON:
				{
				setState(2327);
				body();
				}
				break;
			case ASSIGNMENT:
				{
				setState(2328);
				right_arrow();
				setState(2329);
				expression();
				setState(2330);
				match(SEMICOLON);
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Arg_declarationContext extends ParserRuleContext {
		public TypeContext type() {
			return getRuleContext(TypeContext.class,0);
		}
		public IdentifierContext identifier() {
			return getRuleContext(IdentifierContext.class,0);
		}
		public ExpressionContext expression() {
			return getRuleContext(ExpressionContext.class,0);
		}
		public Arg_declarationContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_arg_declaration; }
	}

	public final Arg_declarationContext arg_declaration() throws RecognitionException {
		Arg_declarationContext _localctx = new Arg_declarationContext(_ctx, getState());
		enterRule(_localctx, 392, RULE_arg_declaration);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(2334);
			type();
			setState(2335);
			identifier();
			setState(2338);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==ASSIGNMENT) {
				{
				setState(2336);
				match(ASSIGNMENT);
				setState(2337);
				expression();
				}
			}

			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Method_invocationContext extends ParserRuleContext {
		public TerminalNode OPEN_PARENS() { return getToken(CSharpParser.OPEN_PARENS, 0); }
		public TerminalNode CLOSE_PARENS() { return getToken(CSharpParser.CLOSE_PARENS, 0); }
		public Argument_listContext argument_list() {
			return getRuleContext(Argument_listContext.class,0);
		}
		public Method_invocationContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_method_invocation; }
	}

	public final Method_invocationContext method_invocation() throws RecognitionException {
		Method_invocationContext _localctx = new Method_invocationContext(_ctx, getState());
		enterRule(_localctx, 394, RULE_method_invocation);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(2340);
			match(OPEN_PARENS);
			setState(2342);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if ((((_la) & ~0x3f) == 0 && ((1L << _la) & ((1L << ADD) | (1L << ALIAS) | (1L << ARGLIST) | (1L << ASCENDING) | (1L << ASYNC) | (1L << AWAIT) | (1L << BASE) | (1L << BOOL) | (1L << BY) | (1L << BYTE) | (1L << CHAR) | (1L << CHECKED) | (1L << DECIMAL) | (1L << DEFAULT) | (1L << DELEGATE) | (1L << DESCENDING) | (1L << DOUBLE) | (1L << DYNAMIC) | (1L << EQUALS) | (1L << FALSE) | (1L << FLOAT) | (1L << FROM) | (1L << GET) | (1L << GROUP) | (1L << INT) | (1L << INTO) | (1L << JOIN) | (1L << LET) | (1L << LONG) | (1L << NAMEOF))) != 0) || ((((_la - 65)) & ~0x3f) == 0 && ((1L << (_la - 65)) & ((1L << (NEW - 65)) | (1L << (NULL - 65)) | (1L << (OBJECT - 65)) | (1L << (ON - 65)) | (1L << (ORDERBY - 65)) | (1L << (OUT - 65)) | (1L << (PARTIAL - 65)) | (1L << (REF - 65)) | (1L << (REMOVE - 65)) | (1L << (SBYTE - 65)) | (1L << (SELECT - 65)) | (1L << (SET - 65)) | (1L << (SHORT - 65)) | (1L << (SIZEOF - 65)) | (1L << (STRING - 65)) | (1L << (THIS - 65)) | (1L << (TRUE - 65)) | (1L << (TYPEOF - 65)) | (1L << (UINT - 65)) | (1L << (ULONG - 65)) | (1L << (UNCHECKED - 65)) | (1L << (USHORT - 65)) | (1L << (WHEN - 65)) | (1L << (WHERE - 65)) | (1L << (YIELD - 65)) | (1L << (IDENTIFIER - 65)) | (1L << (LITERAL_ACCESS - 65)) | (1L << (INTEGER_LITERAL - 65)) | (1L << (HEX_INTEGER_LITERAL - 65)) | (1L << (REAL_LITERAL - 65)) | (1L << (CHARACTER_LITERAL - 65)) | (1L << (REGULAR_STRING - 65)) | (1L << (VERBATIUM_STRING - 65)) | (1L << (INTERPOLATED_REGULAR_STRING_START - 65)) | (1L << (INTERPOLATED_VERBATIUM_STRING_START - 65)) | (1L << (OPEN_PARENS - 65)))) != 0) || ((((_la - 131)) & ~0x3f) == 0 && ((1L << (_la - 131)) & ((1L << (PLUS - 131)) | (1L << (MINUS - 131)) | (1L << (STAR - 131)) | (1L << (AMP - 131)) | (1L << (BANG - 131)) | (1L << (TILDE - 131)) | (1L << (OP_INC - 131)) | (1L << (OP_DEC - 131)))) != 0)) {
				{
				setState(2341);
				argument_list();
				}
			}

			setState(2344);
			match(CLOSE_PARENS);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Object_creation_expressionContext extends ParserRuleContext {
		public TerminalNode OPEN_PARENS() { return getToken(CSharpParser.OPEN_PARENS, 0); }
		public TerminalNode CLOSE_PARENS() { return getToken(CSharpParser.CLOSE_PARENS, 0); }
		public Argument_listContext argument_list() {
			return getRuleContext(Argument_listContext.class,0);
		}
		public Object_or_collection_initializerContext object_or_collection_initializer() {
			return getRuleContext(Object_or_collection_initializerContext.class,0);
		}
		public Object_creation_expressionContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_object_creation_expression; }
	}

	public final Object_creation_expressionContext object_creation_expression() throws RecognitionException {
		Object_creation_expressionContext _localctx = new Object_creation_expressionContext(_ctx, getState());
		enterRule(_localctx, 396, RULE_object_creation_expression);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(2346);
			match(OPEN_PARENS);
			setState(2348);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if ((((_la) & ~0x3f) == 0 && ((1L << _la) & ((1L << ADD) | (1L << ALIAS) | (1L << ARGLIST) | (1L << ASCENDING) | (1L << ASYNC) | (1L << AWAIT) | (1L << BASE) | (1L << BOOL) | (1L << BY) | (1L << BYTE) | (1L << CHAR) | (1L << CHECKED) | (1L << DECIMAL) | (1L << DEFAULT) | (1L << DELEGATE) | (1L << DESCENDING) | (1L << DOUBLE) | (1L << DYNAMIC) | (1L << EQUALS) | (1L << FALSE) | (1L << FLOAT) | (1L << FROM) | (1L << GET) | (1L << GROUP) | (1L << INT) | (1L << INTO) | (1L << JOIN) | (1L << LET) | (1L << LONG) | (1L << NAMEOF))) != 0) || ((((_la - 65)) & ~0x3f) == 0 && ((1L << (_la - 65)) & ((1L << (NEW - 65)) | (1L << (NULL - 65)) | (1L << (OBJECT - 65)) | (1L << (ON - 65)) | (1L << (ORDERBY - 65)) | (1L << (OUT - 65)) | (1L << (PARTIAL - 65)) | (1L << (REF - 65)) | (1L << (REMOVE - 65)) | (1L << (SBYTE - 65)) | (1L << (SELECT - 65)) | (1L << (SET - 65)) | (1L << (SHORT - 65)) | (1L << (SIZEOF - 65)) | (1L << (STRING - 65)) | (1L << (THIS - 65)) | (1L << (TRUE - 65)) | (1L << (TYPEOF - 65)) | (1L << (UINT - 65)) | (1L << (ULONG - 65)) | (1L << (UNCHECKED - 65)) | (1L << (USHORT - 65)) | (1L << (WHEN - 65)) | (1L << (WHERE - 65)) | (1L << (YIELD - 65)) | (1L << (IDENTIFIER - 65)) | (1L << (LITERAL_ACCESS - 65)) | (1L << (INTEGER_LITERAL - 65)) | (1L << (HEX_INTEGER_LITERAL - 65)) | (1L << (REAL_LITERAL - 65)) | (1L << (CHARACTER_LITERAL - 65)) | (1L << (REGULAR_STRING - 65)) | (1L << (VERBATIUM_STRING - 65)) | (1L << (INTERPOLATED_REGULAR_STRING_START - 65)) | (1L << (INTERPOLATED_VERBATIUM_STRING_START - 65)) | (1L << (OPEN_PARENS - 65)))) != 0) || ((((_la - 131)) & ~0x3f) == 0 && ((1L << (_la - 131)) & ((1L << (PLUS - 131)) | (1L << (MINUS - 131)) | (1L << (STAR - 131)) | (1L << (AMP - 131)) | (1L << (BANG - 131)) | (1L << (TILDE - 131)) | (1L << (OP_INC - 131)) | (1L << (OP_DEC - 131)))) != 0)) {
				{
				setState(2347);
				argument_list();
				}
			}

			setState(2350);
			match(CLOSE_PARENS);
			setState(2352);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==OPEN_BRACE) {
				{
				setState(2351);
				object_or_collection_initializer();
				}
			}

			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class IdentifierContext extends ParserRuleContext {
		public TerminalNode IDENTIFIER() { return getToken(CSharpParser.IDENTIFIER, 0); }
		public TerminalNode ADD() { return getToken(CSharpParser.ADD, 0); }
		public TerminalNode ALIAS() { return getToken(CSharpParser.ALIAS, 0); }
		public TerminalNode ARGLIST() { return getToken(CSharpParser.ARGLIST, 0); }
		public TerminalNode ASCENDING() { return getToken(CSharpParser.ASCENDING, 0); }
		public TerminalNode ASYNC() { return getToken(CSharpParser.ASYNC, 0); }
		public TerminalNode AWAIT() { return getToken(CSharpParser.AWAIT, 0); }
		public TerminalNode BY() { return getToken(CSharpParser.BY, 0); }
		public TerminalNode DESCENDING() { return getToken(CSharpParser.DESCENDING, 0); }
		public TerminalNode DYNAMIC() { return getToken(CSharpParser.DYNAMIC, 0); }
		public TerminalNode EQUALS() { return getToken(CSharpParser.EQUALS, 0); }
		public TerminalNode FROM() { return getToken(CSharpParser.FROM, 0); }
		public TerminalNode GET() { return getToken(CSharpParser.GET, 0); }
		public TerminalNode GROUP() { return getToken(CSharpParser.GROUP, 0); }
		public TerminalNode INTO() { return getToken(CSharpParser.INTO, 0); }
		public TerminalNode JOIN() { return getToken(CSharpParser.JOIN, 0); }
		public TerminalNode LET() { return getToken(CSharpParser.LET, 0); }
		public TerminalNode NAMEOF() { return getToken(CSharpParser.NAMEOF, 0); }
		public TerminalNode ON() { return getToken(CSharpParser.ON, 0); }
		public TerminalNode ORDERBY() { return getToken(CSharpParser.ORDERBY, 0); }
		public TerminalNode PARTIAL() { return getToken(CSharpParser.PARTIAL, 0); }
		public TerminalNode REMOVE() { return getToken(CSharpParser.REMOVE, 0); }
		public TerminalNode SELECT() { return getToken(CSharpParser.SELECT, 0); }
		public TerminalNode SET() { return getToken(CSharpParser.SET, 0); }
		public TerminalNode WHEN() { return getToken(CSharpParser.WHEN, 0); }
		public TerminalNode WHERE() { return getToken(CSharpParser.WHERE, 0); }
		public TerminalNode YIELD() { return getToken(CSharpParser.YIELD, 0); }
		public IdentifierContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_identifier; }
	}

	public final IdentifierContext identifier() throws RecognitionException {
		IdentifierContext _localctx = new IdentifierContext(_ctx, getState());
		enterRule(_localctx, 398, RULE_identifier);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(2354);
			_la = _input.LA(1);
			if ( !((((_la) & ~0x3f) == 0 && ((1L << _la) & ((1L << ADD) | (1L << ALIAS) | (1L << ARGLIST) | (1L << ASCENDING) | (1L << ASYNC) | (1L << AWAIT) | (1L << BY) | (1L << DESCENDING) | (1L << DYNAMIC) | (1L << EQUALS) | (1L << FROM) | (1L << GET) | (1L << GROUP) | (1L << INTO) | (1L << JOIN) | (1L << LET) | (1L << NAMEOF))) != 0) || ((((_la - 68)) & ~0x3f) == 0 && ((1L << (_la - 68)) & ((1L << (ON - 68)) | (1L << (ORDERBY - 68)) | (1L << (PARTIAL - 68)) | (1L << (REMOVE - 68)) | (1L << (SELECT - 68)) | (1L << (SET - 68)) | (1L << (WHEN - 68)) | (1L << (WHERE - 68)) | (1L << (YIELD - 68)) | (1L << (IDENTIFIER - 68)))) != 0)) ) {
			_errHandler.recoverInline(this);
			}
			else {
				if ( _input.LA(1)==Token.EOF ) matchedEOF = true;
				_errHandler.reportMatch(this);
				consume();
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public boolean sempred(RuleContext _localctx, int ruleIndex, int predIndex) {
		switch (ruleIndex) {
		case 169:
			return right_arrow_sempred((Right_arrowContext)_localctx, predIndex);
		case 170:
			return right_shift_sempred((Right_shiftContext)_localctx, predIndex);
		case 171:
			return right_shift_assignment_sempred((Right_shift_assignmentContext)_localctx, predIndex);
		}
		return true;
	}
	private boolean right_arrow_sempred(Right_arrowContext _localctx, int predIndex) {
		switch (predIndex) {
		case 0:
			return (((Right_arrowContext)_localctx).first!=null?((Right_arrowContext)_localctx).first.getTokenIndex():0) + 1 == (((Right_arrowContext)_localctx).second!=null?((Right_arrowContext)_localctx).second.getTokenIndex():0);
		}
		return true;
	}
	private boolean right_shift_sempred(Right_shiftContext _localctx, int predIndex) {
		switch (predIndex) {
		case 1:
			return (((Right_shiftContext)_localctx).first!=null?((Right_shiftContext)_localctx).first.getTokenIndex():0) + 1 == (((Right_shiftContext)_localctx).second!=null?((Right_shiftContext)_localctx).second.getTokenIndex():0);
		}
		return true;
	}
	private boolean right_shift_assignment_sempred(Right_shift_assignmentContext _localctx, int predIndex) {
		switch (predIndex) {
		case 2:
			return (((Right_shift_assignmentContext)_localctx).first!=null?((Right_shift_assignmentContext)_localctx).first.getTokenIndex():0) + 1 == (((Right_shift_assignmentContext)_localctx).second!=null?((Right_shift_assignmentContext)_localctx).second.getTokenIndex():0);
		}
		return true;
	}

	public static final String _serializedATN =
		"\3\u608b\ua72a\u8133\ub9ed\u417c\u3be7\u7786\u5964\3\u00c1\u0937\4\2\t"+
		"\2\4\3\t\3\4\4\t\4\4\5\t\5\4\6\t\6\4\7\t\7\4\b\t\b\4\t\t\t\4\n\t\n\4\13"+
		"\t\13\4\f\t\f\4\r\t\r\4\16\t\16\4\17\t\17\4\20\t\20\4\21\t\21\4\22\t\22"+
		"\4\23\t\23\4\24\t\24\4\25\t\25\4\26\t\26\4\27\t\27\4\30\t\30\4\31\t\31"+
		"\4\32\t\32\4\33\t\33\4\34\t\34\4\35\t\35\4\36\t\36\4\37\t\37\4 \t \4!"+
		"\t!\4\"\t\"\4#\t#\4$\t$\4%\t%\4&\t&\4\'\t\'\4(\t(\4)\t)\4*\t*\4+\t+\4"+
		",\t,\4-\t-\4.\t.\4/\t/\4\60\t\60\4\61\t\61\4\62\t\62\4\63\t\63\4\64\t"+
		"\64\4\65\t\65\4\66\t\66\4\67\t\67\48\t8\49\t9\4:\t:\4;\t;\4<\t<\4=\t="+
		"\4>\t>\4?\t?\4@\t@\4A\tA\4B\tB\4C\tC\4D\tD\4E\tE\4F\tF\4G\tG\4H\tH\4I"+
		"\tI\4J\tJ\4K\tK\4L\tL\4M\tM\4N\tN\4O\tO\4P\tP\4Q\tQ\4R\tR\4S\tS\4T\tT"+
		"\4U\tU\4V\tV\4W\tW\4X\tX\4Y\tY\4Z\tZ\4[\t[\4\\\t\\\4]\t]\4^\t^\4_\t_\4"+
		"`\t`\4a\ta\4b\tb\4c\tc\4d\td\4e\te\4f\tf\4g\tg\4h\th\4i\ti\4j\tj\4k\t"+
		"k\4l\tl\4m\tm\4n\tn\4o\to\4p\tp\4q\tq\4r\tr\4s\ts\4t\tt\4u\tu\4v\tv\4"+
		"w\tw\4x\tx\4y\ty\4z\tz\4{\t{\4|\t|\4}\t}\4~\t~\4\177\t\177\4\u0080\t\u0080"+
		"\4\u0081\t\u0081\4\u0082\t\u0082\4\u0083\t\u0083\4\u0084\t\u0084\4\u0085"+
		"\t\u0085\4\u0086\t\u0086\4\u0087\t\u0087\4\u0088\t\u0088\4\u0089\t\u0089"+
		"\4\u008a\t\u008a\4\u008b\t\u008b\4\u008c\t\u008c\4\u008d\t\u008d\4\u008e"+
		"\t\u008e\4\u008f\t\u008f\4\u0090\t\u0090\4\u0091\t\u0091\4\u0092\t\u0092"+
		"\4\u0093\t\u0093\4\u0094\t\u0094\4\u0095\t\u0095\4\u0096\t\u0096\4\u0097"+
		"\t\u0097\4\u0098\t\u0098\4\u0099\t\u0099\4\u009a\t\u009a\4\u009b\t\u009b"+
		"\4\u009c\t\u009c\4\u009d\t\u009d\4\u009e\t\u009e\4\u009f\t\u009f\4\u00a0"+
		"\t\u00a0\4\u00a1\t\u00a1\4\u00a2\t\u00a2\4\u00a3\t\u00a3\4\u00a4\t\u00a4"+
		"\4\u00a5\t\u00a5\4\u00a6\t\u00a6\4\u00a7\t\u00a7\4\u00a8\t\u00a8\4\u00a9"+
		"\t\u00a9\4\u00aa\t\u00aa\4\u00ab\t\u00ab\4\u00ac\t\u00ac\4\u00ad\t\u00ad"+
		"\4\u00ae\t\u00ae\4\u00af\t\u00af\4\u00b0\t\u00b0\4\u00b1\t\u00b1\4\u00b2"+
		"\t\u00b2\4\u00b3\t\u00b3\4\u00b4\t\u00b4\4\u00b5\t\u00b5\4\u00b6\t\u00b6"+
		"\4\u00b7\t\u00b7\4\u00b8\t\u00b8\4\u00b9\t\u00b9\4\u00ba\t\u00ba\4\u00bb"+
		"\t\u00bb\4\u00bc\t\u00bc\4\u00bd\t\u00bd\4\u00be\t\u00be\4\u00bf\t\u00bf"+
		"\4\u00c0\t\u00c0\4\u00c1\t\u00c1\4\u00c2\t\u00c2\4\u00c3\t\u00c3\4\u00c4"+
		"\t\u00c4\4\u00c5\t\u00c5\4\u00c6\t\u00c6\4\u00c7\t\u00c7\4\u00c8\t\u00c8"+
		"\4\u00c9\t\u00c9\3\2\5\2\u0194\n\2\3\2\5\2\u0197\n\2\3\2\5\2\u019a\n\2"+
		"\3\2\7\2\u019d\n\2\f\2\16\2\u01a0\13\2\3\2\5\2\u01a3\n\2\3\2\3\2\3\3\3"+
		"\3\5\3\u01a9\n\3\3\3\5\3\u01ac\n\3\3\3\3\3\3\3\5\3\u01b1\n\3\7\3\u01b3"+
		"\n\3\f\3\16\3\u01b6\13\3\3\4\3\4\3\4\3\4\7\4\u01bc\n\4\f\4\16\4\u01bf"+
		"\13\4\3\5\3\5\3\5\3\5\5\5\u01c5\n\5\3\6\3\6\5\6\u01c9\n\6\3\7\3\7\3\7"+
		"\5\7\u01ce\n\7\3\b\3\b\3\t\3\t\3\n\3\n\3\n\3\n\5\n\u01d8\n\n\3\13\3\13"+
		"\3\13\3\13\7\13\u01de\n\13\f\13\16\13\u01e1\13\13\3\13\3\13\3\f\3\f\3"+
		"\f\7\f\u01e8\n\f\f\f\16\f\u01eb\13\f\3\r\3\r\3\r\5\r\u01f0\n\r\3\r\5\r"+
		"\u01f3\n\r\3\r\3\r\3\16\3\16\5\16\u01f9\n\16\3\17\3\17\3\17\5\17\u01fe"+
		"\n\17\3\20\3\20\3\20\3\20\3\21\3\21\3\21\3\21\3\21\3\21\3\21\3\21\3\21"+
		"\3\21\3\21\5\21\u020f\n\21\3\22\3\22\3\22\3\22\3\22\3\22\5\22\u0217\n"+
		"\22\3\23\3\23\3\23\5\23\u021c\n\23\3\24\3\24\3\24\7\24\u0221\n\24\f\24"+
		"\16\24\u0224\13\24\3\25\3\25\3\25\7\25\u0229\n\25\f\25\16\25\u022c\13"+
		"\25\3\26\3\26\3\26\7\26\u0231\n\26\f\26\16\26\u0234\13\26\3\27\3\27\3"+
		"\27\7\27\u0239\n\27\f\27\16\27\u023c\13\27\3\30\3\30\3\30\7\30\u0241\n"+
		"\30\f\30\16\30\u0244\13\30\3\31\3\31\3\31\7\31\u0249\n\31\f\31\16\31\u024c"+
		"\13\31\3\32\3\32\3\32\3\32\3\32\3\32\3\32\7\32\u0255\n\32\f\32\16\32\u0258"+
		"\13\32\3\33\3\33\3\33\5\33\u025d\n\33\3\33\7\33\u0260\n\33\f\33\16\33"+
		"\u0263\13\33\3\34\3\34\3\34\7\34\u0268\n\34\f\34\16\34\u026b\13\34\3\35"+
		"\3\35\3\35\7\35\u0270\n\35\f\35\16\35\u0273\13\35\3\36\3\36\3\36\3\36"+
		"\3\36\3\36\3\36\3\36\3\36\3\36\3\36\3\36\3\36\3\36\3\36\3\36\3\36\3\36"+
		"\3\36\3\36\3\36\3\36\3\36\3\36\5\36\u028d\n\36\3\37\3\37\7\37\u0291\n"+
		"\37\f\37\16\37\u0294\13\37\3\37\3\37\3\37\3\37\3\37\3\37\5\37\u029c\n"+
		"\37\3\37\7\37\u029f\n\37\f\37\16\37\u02a2\13\37\7\37\u02a4\n\37\f\37\16"+
		"\37\u02a7\13\37\3 \3 \3 \5 \u02ac\n \3 \3 \3 \3 \3 \3 \3 \3 \3 \3 \3 "+
		"\3 \5 \u02ba\n \3 \3 \3 \3 \5 \u02c0\n \3 \3 \3 \3 \3 \3 \3 \3 \7 \u02ca"+
		"\n \f \16 \u02cd\13 \3 \5 \u02d0\n \3 \6 \u02d3\n \r \16 \u02d4\3 \3 "+
		"\5 \u02d9\n \3 \3 \3 \3 \5 \u02df\n \3 \3 \3 \3 \3 \5 \u02e6\n \3 \3 "+
		"\3 \3 \3 \3 \3 \3 \3 \3 \3 \3 \3 \3 \3 \3 \3 \5 \u02f9\n \3 \3 \3 \5 "+
		"\u02fe\n \3 \5 \u0301\n \3 \3 \3 \3 \3 \3 \3 \3 \3 \3 \3 \7 \u030e\n "+
		"\f \16 \u0311\13 \3 \3 \3 \5 \u0316\n \3!\5!\u0319\n!\3!\3!\3!\5!\u031e"+
		"\n!\3\"\5\"\u0321\n\"\3\"\3\"\3\"\3\"\7\"\u0327\n\"\f\"\16\"\u032a\13"+
		"\"\3\"\3\"\3#\3#\3#\5#\u0331\n#\3#\3#\3$\3$\3%\3%\3%\7%\u033a\n%\f%\16"+
		"%\u033d\13%\3&\3&\5&\u0341\n&\3\'\3\'\3\'\5\'\u0346\n\'\5\'\u0348\n\'"+
		"\3\'\3\'\3(\3(\3(\7(\u034f\n(\f(\16(\u0352\13(\3)\3)\3)\3)\3)\5)\u0359"+
		"\n)\3)\3)\3)\3*\3*\5*\u0360\n*\3+\3+\3+\3+\7+\u0366\n+\f+\16+\u0369\13"+
		"+\3+\5+\u036c\n+\3+\3+\3,\3,\3,\3,\3,\5,\u0375\n,\3-\3-\3-\5-\u037a\n"+
		"-\5-\u037c\n-\3-\3-\3.\3.\3.\7.\u0383\n.\f.\16.\u0386\13.\3/\3/\3/\3/"+
		"\3/\5/\u038d\n/\3\60\3\60\5\60\u0391\n\60\3\60\3\60\3\60\5\60\u0396\n"+
		"\60\5\60\u0398\n\60\3\60\3\60\3\60\5\60\u039d\n\60\7\60\u039f\n\60\f\60"+
		"\16\60\u03a2\13\60\3\61\3\61\7\61\u03a6\n\61\f\61\16\61\u03a9\13\61\3"+
		"\61\3\61\3\62\3\62\3\62\7\62\u03b0\n\62\f\62\16\62\u03b3\13\62\3\62\5"+
		"\62\u03b6\n\62\3\63\5\63\u03b9\n\63\3\63\3\63\3\63\3\63\3\64\3\64\3\64"+
		"\3\64\3\64\3\64\3\64\3\64\3\64\3\64\3\64\5\64\u03ca\n\64\3\65\3\65\3\65"+
		"\7\65\u03cf\n\65\f\65\16\65\u03d2\13\65\3\66\5\66\u03d5\n\66\3\66\3\66"+
		"\3\66\3\67\3\67\3\67\7\67\u03dd\n\67\f\67\16\67\u03e0\13\67\38\38\58\u03e4"+
		"\n8\39\39\39\3:\3:\5:\u03eb\n:\3:\3:\3:\3:\3;\7;\u03f2\n;\f;\16;\u03f5"+
		"\13;\3;\3;\5;\u03f9\n;\3<\3<\3<\3<\3<\5<\u0400\n<\3=\3=\3=\3=\3=\3>\3"+
		">\3>\3?\3?\5?\u040c\n?\3?\3?\3?\3?\3?\3?\3?\3?\3?\5?\u0417\n?\3@\3@\3"+
		"@\3@\7@\u041d\n@\f@\16@\u0420\13@\3A\3A\5A\u0424\nA\3B\3B\3B\3B\3B\3B"+
		"\3B\5B\u042d\nB\3C\3C\3C\3C\3D\3D\3D\3D\3D\3D\5D\u0439\nD\3D\3D\3D\5D"+
		"\u043e\nD\3E\3E\5E\u0442\nE\3F\3F\3F\3F\3F\3F\3F\3F\3F\3F\3F\5F\u044f"+
		"\nF\3F\3F\3F\3F\3F\3F\7F\u0457\nF\fF\16F\u045a\13F\3F\3F\3F\3F\3F\3F\3"+
		"F\3F\3F\3F\3F\3F\3F\3F\3F\3F\3F\3F\3F\5F\u046f\nF\3F\3F\5F\u0473\nF\3"+
		"F\3F\5F\u0477\nF\3F\3F\3F\3F\3F\3F\3F\3F\3F\3F\3F\3F\3F\3F\3F\3F\3F\3"+
		"F\3F\3F\5F\u048d\nF\3F\3F\3F\5F\u0492\nF\3F\3F\3F\5F\u0497\nF\3F\3F\3"+
		"F\3F\3F\5F\u049e\nF\3F\5F\u04a1\nF\3F\3F\3F\3F\3F\3F\3F\3F\3F\3F\3F\3"+
		"F\3F\3F\3F\3F\3F\3F\3F\3F\5F\u04b7\nF\3F\3F\3F\3F\3F\3F\3F\3F\3F\3F\5"+
		"F\u04c3\nF\3G\3G\5G\u04c7\nG\3G\3G\3H\3H\3H\3H\7H\u04cf\nH\fH\16H\u04d2"+
		"\13H\3I\3I\3I\5I\u04d7\nI\3J\3J\3J\5J\u04dc\nJ\3K\3K\3K\3K\3L\3L\5L\u04e4"+
		"\nL\3M\6M\u04e7\nM\rM\16M\u04e8\3M\3M\3N\3N\3N\3N\3N\3N\5N\u04f3\nN\3"+
		"O\6O\u04f6\nO\rO\16O\u04f7\3P\3P\3P\3P\7P\u04fe\nP\fP\16P\u0501\13P\5"+
		"P\u0503\nP\3Q\3Q\3Q\7Q\u0508\nQ\fQ\16Q\u050b\13Q\3R\3R\7R\u050f\nR\fR"+
		"\16R\u0512\13R\3R\5R\u0515\nR\3R\5R\u0518\nR\3S\3S\3S\3S\5S\u051e\nS\3"+
		"S\3S\5S\u0522\nS\3S\3S\3T\3T\5T\u0528\nT\3T\3T\3U\3U\3U\3U\3U\3V\3V\3"+
		"V\3W\3W\5W\u0536\nW\3X\3X\3X\3X\5X\u053c\nX\3Y\3Y\3Y\7Y\u0541\nY\fY\16"+
		"Y\u0544\13Y\3Z\3Z\5Z\u0548\nZ\3Z\5Z\u054b\nZ\3Z\5Z\u054e\nZ\3Z\3Z\3[\6"+
		"[\u0553\n[\r[\16[\u0554\3\\\3\\\3\\\3\\\3\\\3]\6]\u055d\n]\r]\16]\u055e"+
		"\3^\3^\3^\3^\3^\3^\3^\3^\3^\3^\3^\3^\3^\3^\3^\5^\u0570\n^\3_\6_\u0573"+
		"\n_\r_\16_\u0574\3`\3`\5`\u0579\n`\3a\5a\u057c\na\3a\5a\u057f\na\3a\3"+
		"a\3a\3a\3a\5a\u0586\na\3b\3b\3b\3b\5b\u058c\nb\3c\3c\3c\3c\7c\u0592\n"+
		"c\fc\16c\u0595\13c\3c\3c\3d\5d\u059a\nd\3d\3d\3e\3e\3e\3e\7e\u05a2\ne"+
		"\fe\16e\u05a5\13e\3f\3f\3f\7f\u05aa\nf\ff\16f\u05ad\13f\3g\6g\u05b0\n"+
		"g\rg\16g\u05b1\3h\3h\3h\3h\3h\3i\3i\3i\3i\5i\u05bd\ni\3i\3i\5i\u05c1\n"+
		"i\5i\u05c3\ni\3j\3j\3j\5j\u05c8\nj\3k\3k\3k\7k\u05cd\nk\fk\16k\u05d0\13"+
		"k\3l\3l\3l\3l\3m\3m\5m\u05d8\nm\3m\3m\3n\6n\u05dd\nn\rn\16n\u05de\3o\5"+
		"o\u05e2\no\3o\5o\u05e5\no\3o\3o\5o\u05e9\no\3p\6p\u05ec\np\rp\16p\u05ed"+
		"\3q\3q\3r\3r\3r\3r\3r\3r\3r\3r\3r\5r\u05fb\nr\3r\3r\3r\3r\3r\3r\3r\3r"+
		"\5r\u0605\nr\3s\3s\3s\3s\3s\3s\3s\3s\3s\3s\5s\u0611\ns\3t\3t\3t\7t\u0616"+
		"\nt\ft\16t\u0619\13t\3u\3u\3u\3u\3v\3v\3v\7v\u0622\nv\fv\16v\u0625\13"+
		"v\3w\3w\3w\5w\u062a\nw\3x\3x\5x\u062e\nx\3y\3y\5y\u0632\ny\3z\3z\3{\3"+
		"{\5{\u0638\n{\3|\3|\3|\3|\5|\u063e\n|\5|\u0640\n|\3}\3}\3}\7}\u0645\n"+
		"}\f}\16}\u0648\13}\3~\5~\u064b\n~\3~\5~\u064e\n~\3~\3~\5~\u0652\n~\3\177"+
		"\3\177\3\u0080\5\u0080\u0657\n\u0080\3\u0080\3\u0080\3\u0080\3\u0080\3"+
		"\u0081\5\u0081\u065e\n\u0081\3\u0081\5\u0081\u0661\n\u0081\3\u0081\3\u0081"+
		"\3\u0081\5\u0081\u0666\n\u0081\3\u0081\3\u0081\3\u0081\5\u0081\u066b\n"+
		"\u0081\5\u0081\u066d\n\u0081\3\u0082\5\u0082\u0670\n\u0082\3\u0082\5\u0082"+
		"\u0673\n\u0082\3\u0082\3\u0082\3\u0082\3\u0083\5\u0083\u0679\n\u0083\3"+
		"\u0083\5\u0083\u067c\n\u0083\3\u0083\3\u0083\3\u0083\3\u0084\3\u0084\3"+
		"\u0084\3\u0084\3\u0084\3\u0084\3\u0084\5\u0084\u0688\n\u0084\3\u0085\3"+
		"\u0085\5\u0085\u068c\n\u0085\3\u0086\5\u0086\u068f\n\u0086\3\u0086\3\u0086"+
		"\3\u0086\3\u0086\3\u0086\3\u0086\3\u0086\3\u0086\5\u0086\u0699\n\u0086"+
		"\3\u0087\5\u0087\u069c\n\u0087\3\u0087\3\u0087\3\u0087\3\u0088\5\u0088"+
		"\u06a2\n\u0088\3\u0088\3\u0088\3\u0088\3\u0089\3\u0089\3\u0089\3\u0089"+
		"\3\u0089\3\u0089\3\u0089\3\u0089\3\u0089\3\u0089\3\u0089\3\u0089\3\u0089"+
		"\3\u0089\3\u0089\3\u0089\3\u0089\3\u0089\3\u0089\3\u0089\3\u0089\3\u0089"+
		"\5\u0089\u06bd\n\u0089\3\u008a\3\u008a\3\u008a\3\u008a\3\u008a\3\u008a"+
		"\3\u008a\3\u008b\3\u008b\3\u008b\3\u008b\5\u008b\u06ca\n\u008b\3\u008b"+
		"\3\u008b\3\u008c\3\u008c\5\u008c\u06d0\n\u008c\3\u008d\3\u008d\3\u008d"+
		"\3\u008e\3\u008e\7\u008e\u06d7\n\u008e\f\u008e\16\u008e\u06da\13\u008e"+
		"\3\u008e\3\u008e\3\u008f\5\u008f\u06df\n\u008f\3\u008f\5\u008f\u06e2\n"+
		"\u008f\3\u008f\3\u008f\3\u008f\3\u008f\6\u008f\u06e8\n\u008f\r\u008f\16"+
		"\u008f\u06e9\3\u008f\3\u008f\5\u008f\u06ee\n\u008f\3\u0090\3\u0090\7\u0090"+
		"\u06f2\n\u0090\f\u0090\16\u0090\u06f5\13\u0090\3\u0090\6\u0090\u06f8\n"+
		"\u0090\r\u0090\16\u0090\u06f9\3\u0091\3\u0091\7\u0091\u06fe\n\u0091\f"+
		"\u0091\16\u0091\u0701\13\u0091\3\u0091\3\u0091\3\u0092\3\u0092\3\u0092"+
		"\3\u0092\7\u0092\u0709\n\u0092\f\u0092\16\u0092\u070c\13\u0092\3\u0092"+
		"\5\u0092\u070f\n\u0092\5\u0092\u0711\n\u0092\3\u0092\3\u0092\3\u0093\3"+
		"\u0093\3\u0093\3\u0093\7\u0093\u0719\n\u0093\f\u0093\16\u0093\u071c\13"+
		"\u0093\3\u0093\3\u0093\3\u0094\5\u0094\u0721\n\u0094\3\u0094\5\u0094\u0724"+
		"\n\u0094\3\u0094\3\u0094\3\u0095\3\u0095\3\u0096\3\u0096\3\u0096\3\u0097"+
		"\3\u0097\7\u0097\u072f\n\u0097\f\u0097\16\u0097\u0732\13\u0097\3\u0097"+
		"\3\u0097\3\u0098\5\u0098\u0737\n\u0098\3\u0098\5\u0098\u073a\n\u0098\3"+
		"\u0098\5\u0098\u073d\n\u0098\3\u0098\3\u0098\3\u0098\5\u0098\u0742\n\u0098"+
		"\3\u0098\3\u0098\5\u0098\u0746\n\u0098\3\u0098\3\u0098\5\u0098\u074a\n"+
		"\u0098\3\u0098\3\u0098\3\u0098\3\u0098\3\u0098\3\u0098\3\u0098\3\u0098"+
		"\3\u0098\3\u0098\3\u0098\3\u0098\3\u0098\3\u0098\3\u0098\5\u0098\u075b"+
		"\n\u0098\3\u0098\5\u0098\u075e\n\u0098\3\u0098\3\u0098\3\u0098\5\u0098"+
		"\u0763\n\u0098\3\u0098\3\u0098\5\u0098\u0767\n\u0098\3\u0098\3\u0098\5"+
		"\u0098\u076b\n\u0098\3\u0098\3\u0098\3\u0098\3\u0098\3\u0098\3\u0098\3"+
		"\u0098\5\u0098\u0774\n\u0098\3\u0099\5\u0099\u0777\n\u0099\3\u0099\3\u0099"+
		"\3\u0099\5\u0099\u077c\n\u0099\3\u0099\3\u0099\5\u0099\u0780\n\u0099\3"+
		"\u0099\3\u0099\3\u0099\5\u0099\u0785\n\u0099\3\u0099\3\u0099\5\u0099\u0789"+
		"\n\u0099\5\u0099\u078b\n\u0099\3\u009a\3\u009a\3\u009a\3\u009b\3\u009b"+
		"\3\u009b\3\u009b\7\u009b\u0794\n\u009b\f\u009b\16\u009b\u0797\13\u009b"+
		"\3\u009b\5\u009b\u079a\n\u009b\5\u009b\u079c\n\u009b\3\u009b\3\u009b\3"+
		"\u009c\5\u009c\u07a1\n\u009c\3\u009c\3\u009c\3\u009c\5\u009c\u07a6\n\u009c"+
		"\3\u009d\3\u009d\3\u009d\3\u009d\3\u009d\5\u009d\u07ad\n\u009d\3\u009d"+
		"\3\u009d\3\u009e\3\u009e\5\u009e\u07b3\n\u009e\3\u009f\6\u009f\u07b6\n"+
		"\u009f\r\u009f\16\u009f\u07b7\3\u00a0\3\u00a0\3\u00a0\3\u00a0\5\u00a0"+
		"\u07be\n\u00a0\3\u00a0\3\u00a0\5\u00a0\u07c2\n\u00a0\3\u00a0\3\u00a0\3"+
		"\u00a1\3\u00a1\5\u00a1\u07c8\n\u00a1\3\u00a2\3\u00a2\3\u00a2\7\u00a2\u07cd"+
		"\n\u00a2\f\u00a2\16\u00a2\u07d0\13\u00a2\3\u00a3\3\u00a3\3\u00a3\3\u00a3"+
		"\3\u00a3\7\u00a3\u07d7\n\u00a3\f\u00a3\16\u00a3\u07da\13\u00a3\5\u00a3"+
		"\u07dc\n\u00a3\3\u00a3\5\u00a3\u07df\n\u00a3\3\u00a4\3\u00a4\3\u00a4\5"+
		"\u00a4\u07e4\n\u00a4\3\u00a4\3\u00a4\3\u00a5\3\u00a5\5\u00a5\u07ea\n\u00a5"+
		"\3\u00a5\3\u00a5\7\u00a5\u07ee\n\u00a5\f\u00a5\16\u00a5\u07f1\13\u00a5"+
		"\3\u00a5\3\u00a5\3\u00a5\3\u00a5\5\u00a5\u07f7\n\u00a5\3\u00a6\3\u00a6"+
		"\3\u00a6\7\u00a6\u07fc\n\u00a6\f\u00a6\16\u00a6\u07ff\13\u00a6\3\u00a7"+
		"\3\u00a7\3\u00a7\3\u00a7\3\u00a8\5\u00a8\u0806\n\u00a8\3\u00a8\3\u00a8"+
		"\5\u00a8\u080a\n\u00a8\3\u00a9\3\u00a9\3\u00a9\3\u00a9\3\u00a9\3\u00aa"+
		"\3\u00aa\3\u00aa\3\u00aa\3\u00aa\3\u00aa\3\u00ab\3\u00ab\3\u00ab\3\u00ab"+
		"\3\u00ac\3\u00ac\3\u00ac\3\u00ac\3\u00ad\3\u00ad\3\u00ad\3\u00ad\3\u00ae"+
		"\3\u00ae\3\u00ae\3\u00ae\3\u00ae\3\u00ae\3\u00ae\5\u00ae\u082a\n\u00ae"+
		"\3\u00af\3\u00af\3\u00b0\3\u00b0\3\u00b0\3\u00b0\5\u00b0\u0832\n\u00b0"+
		"\3\u00b1\3\u00b1\7\u00b1\u0836\n\u00b1\f\u00b1\16\u00b1\u0839\13\u00b1"+
		"\3\u00b1\3\u00b1\3\u00b2\3\u00b2\7\u00b2\u083f\n\u00b2\f\u00b2\16\u00b2"+
		"\u0842\13\u00b2\3\u00b2\3\u00b2\3\u00b3\3\u00b3\3\u00b3\3\u00b3\5\u00b3"+
		"\u084a\n\u00b3\3\u00b4\3\u00b4\3\u00b4\3\u00b4\5\u00b4\u0850\n\u00b4\3"+
		"\u00b5\3\u00b5\3\u00b5\7\u00b5\u0855\n\u00b5\f\u00b5\16\u00b5\u0858\13"+
		"\u00b5\3\u00b5\3\u00b5\6\u00b5\u085c\n\u00b5\r\u00b5\16\u00b5\u085d\5"+
		"\u00b5\u0860\n\u00b5\3\u00b6\3\u00b6\3\u00b7\3\u00b7\3\u00b7\5\u00b7\u0867"+
		"\n\u00b7\3\u00b7\5\u00b7\u086a\n\u00b7\3\u00b7\5\u00b7\u086d\n\u00b7\3"+
		"\u00b7\3\u00b7\5\u00b7\u0871\n\u00b7\3\u00b8\3\u00b8\3\u00b8\5\u00b8\u0876"+
		"\n\u00b8\3\u00b8\5\u00b8\u0879\n\u00b8\3\u00b8\5\u00b8\u087c\n\u00b8\3"+
		"\u00b8\3\u00b8\5\u00b8\u0880\n\u00b8\3\u00b9\3\u00b9\3\u00b9\5\u00b9\u0885"+
		"\n\u00b9\3\u00b9\5\u00b9\u0888\n\u00b9\3\u00b9\5\u00b9\u088b\n\u00b9\3"+
		"\u00b9\3\u00b9\5\u00b9\u088f\n\u00b9\3\u00ba\3\u00ba\3\u00ba\5\u00ba\u0894"+
		"\n\u00ba\3\u00ba\3\u00ba\5\u00ba\u0898\n\u00ba\3\u00bb\3\u00bb\3\u00bb"+
		"\3\u00bb\5\u00bb\u089e\n\u00bb\3\u00bb\3\u00bb\5\u00bb\u08a2\n\u00bb\3"+
		"\u00bb\3\u00bb\5\u00bb\u08a6\n\u00bb\3\u00bb\3\u00bb\3\u00bc\3\u00bc\3"+
		"\u00bc\3\u00bc\3\u00bc\3\u00bc\3\u00bc\3\u00bc\3\u00bc\3\u00bc\5\u00bc"+
		"\u08b4\n\u00bc\3\u00bd\3\u00bd\3\u00bd\3\u00be\3\u00be\3\u00be\3\u00be"+
		"\3\u00be\3\u00be\3\u00be\3\u00be\5\u00be\u08c1\n\u00be\3\u00be\3\u00be"+
		"\3\u00be\3\u00be\5\u00be\u08c7\n\u00be\3\u00bf\3\u00bf\3\u00bf\3\u00bf"+
		"\3\u00bf\3\u00c0\3\u00c0\3\u00c0\3\u00c0\3\u00c0\3\u00c0\3\u00c0\3\u00c0"+
		"\3\u00c0\3\u00c0\3\u00c0\3\u00c0\5\u00c0\u08da\n\u00c0\3\u00c1\3\u00c1"+
		"\3\u00c1\3\u00c1\3\u00c1\3\u00c1\3\u00c2\3\u00c2\3\u00c2\5\u00c2\u08e5"+
		"\n\u00c2\3\u00c2\3\u00c2\5\u00c2\u08e9\n\u00c2\3\u00c2\3\u00c2\3\u00c3"+
		"\3\u00c3\5\u00c3\u08ef\n\u00c3\3\u00c3\3\u00c3\5\u00c3\u08f3\n\u00c3\3"+
		"\u00c3\3\u00c3\5\u00c3\u08f7\n\u00c3\3\u00c3\3\u00c3\3\u00c3\3\u00c3\3"+
		"\u00c3\5\u00c3\u08fe\n\u00c3\3\u00c4\3\u00c4\3\u00c4\3\u00c4\3\u00c4\5"+
		"\u00c4\u0905\n\u00c4\3\u00c4\5\u00c4\u0908\n\u00c4\3\u00c4\3\u00c4\7\u00c4"+
		"\u090c\n\u00c4\f\u00c4\16\u00c4\u090f\13\u00c4\3\u00c5\3\u00c5\3\u00c5"+
		"\3\u00c5\3\u00c5\3\u00c5\5\u00c5\u0917\n\u00c5\3\u00c5\3\u00c5\3\u00c5"+
		"\3\u00c5\3\u00c5\3\u00c5\5\u00c5\u091f\n\u00c5\3\u00c6\3\u00c6\3\u00c6"+
		"\3\u00c6\5\u00c6\u0925\n\u00c6\3\u00c7\3\u00c7\5\u00c7\u0929\n\u00c7\3"+
		"\u00c7\3\u00c7\3\u00c8\3\u00c8\5\u00c8\u092f\n\u00c8\3\u00c8\3\u00c8\5"+
		"\u00c8\u0933\n\u00c8\3\u00c9\3\u00c9\3\u00c9\2\2\u00ca\2\4\6\b\n\f\16"+
		"\20\22\24\26\30\32\34\36 \"$&(*,.\60\62\64\668:<>@BDFHJLNPRTVXZ\\^`bd"+
		"fhjlnprtvxz|~\u0080\u0082\u0084\u0086\u0088\u008a\u008c\u008e\u0090\u0092"+
		"\u0094\u0096\u0098\u009a\u009c\u009e\u00a0\u00a2\u00a4\u00a6\u00a8\u00aa"+
		"\u00ac\u00ae\u00b0\u00b2\u00b4\u00b6\u00b8\u00ba\u00bc\u00be\u00c0\u00c2"+
		"\u00c4\u00c6\u00c8\u00ca\u00cc\u00ce\u00d0\u00d2\u00d4\u00d6\u00d8\u00da"+
		"\u00dc\u00de\u00e0\u00e2\u00e4\u00e6\u00e8\u00ea\u00ec\u00ee\u00f0\u00f2"+
		"\u00f4\u00f6\u00f8\u00fa\u00fc\u00fe\u0100\u0102\u0104\u0106\u0108\u010a"+
		"\u010c\u010e\u0110\u0112\u0114\u0116\u0118\u011a\u011c\u011e\u0120\u0122"+
		"\u0124\u0126\u0128\u012a\u012c\u012e\u0130\u0132\u0134\u0136\u0138\u013a"+
		"\u013c\u013e\u0140\u0142\u0144\u0146\u0148\u014a\u014c\u014e\u0150\u0152"+
		"\u0154\u0156\u0158\u015a\u015c\u015e\u0160\u0162\u0164\u0166\u0168\u016a"+
		"\u016c\u016e\u0170\u0172\u0174\u0176\u0178\u017a\u017c\u017e\u0180\u0182"+
		"\u0184\u0186\u0188\u018a\u018c\u018e\u0190\2\24\n\2\26\26\31\3188@@TT"+
		"XXdehh\4\2##..\4\2IIQQ\3\2\u009a\u009b\4\2\u0090\u0091\u009c\u009d\3\2"+
		"\u0085\u0086\3\2\u0087\u0089\20\2\23\23\26\26\31\31\36\36##..88@@EETT"+
		"XX\\\\dehh\4\2\17\17!!\16\2\n\n\20\20**::CCJJLPUU[[ggjjll\5\2IIQQ__\4"+
		"\2))\66\66\4\2\22\22__\4\2\u0087\u0087\u0092\u0092\4\2\67\67II\4\2++a"+
		"a\24\2\n\n\16\16\22\24\26 \"#%&(\60\63\63\65:<<?@BEGGIKMQSUXloo\24\2\13"+
		"\r\17\21\25\25!!$$\'\'\61\62\64\64;;=>AAFFHHLLRRVWmnpq\2\u0a15\2\u0193"+
		"\3\2\2\2\4\u01ab\3\2\2\2\6\u01b7\3\2\2\2\b\u01c4\3\2\2\2\n\u01c8\3\2\2"+
		"\2\f\u01cd\3\2\2\2\16\u01cf\3\2\2\2\20\u01d1\3\2\2\2\22\u01d7\3\2\2\2"+
		"\24\u01d9\3\2\2\2\26\u01e4\3\2\2\2\30\u01ef\3\2\2\2\32\u01f8\3\2\2\2\34"+
		"\u01fd\3\2\2\2\36\u01ff\3\2\2\2 \u020e\3\2\2\2\"\u0210\3\2\2\2$\u0218"+
		"\3\2\2\2&\u021d\3\2\2\2(\u0225\3\2\2\2*\u022d\3\2\2\2,\u0235\3\2\2\2."+
		"\u023d\3\2\2\2\60\u0245\3\2\2\2\62\u024d\3\2\2\2\64\u0259\3\2\2\2\66\u0264"+
		"\3\2\2\28\u026c\3\2\2\2:\u028c\3\2\2\2<\u028e\3\2\2\2>\u0315\3\2\2\2@"+
		"\u0318\3\2\2\2B\u0320\3\2\2\2D\u0330\3\2\2\2F\u0334\3\2\2\2H\u0336\3\2"+
		"\2\2J\u0340\3\2\2\2L\u0342\3\2\2\2N\u034b\3\2\2\2P\u0358\3\2\2\2R\u035f"+
		"\3\2\2\2T\u0361\3\2\2\2V\u0374\3\2\2\2X\u0376\3\2\2\2Z\u037f\3\2\2\2\\"+
		"\u038c\3\2\2\2^\u038e\3\2\2\2`\u03a3\3\2\2\2b\u03ac\3\2\2\2d\u03b8\3\2"+
		"\2\2f\u03c9\3\2\2\2h\u03cb\3\2\2\2j\u03d4\3\2\2\2l\u03d9\3\2\2\2n\u03e3"+
		"\3\2\2\2p\u03e5\3\2\2\2r\u03e8\3\2\2\2t\u03f3\3\2\2\2v\u03ff\3\2\2\2x"+
		"\u0401\3\2\2\2z\u0406\3\2\2\2|\u0409\3\2\2\2~\u0418\3\2\2\2\u0080\u0421"+
		"\3\2\2\2\u0082\u042c\3\2\2\2\u0084\u042e\3\2\2\2\u0086\u043d\3\2\2\2\u0088"+
		"\u0441\3\2\2\2\u008a\u04c2\3\2\2\2\u008c\u04c4\3\2\2\2\u008e\u04ca\3\2"+
		"\2\2\u0090\u04d3\3\2\2\2\u0092\u04db\3\2\2\2\u0094\u04dd\3\2\2\2\u0096"+
		"\u04e3\3\2\2\2\u0098\u04e6\3\2\2\2\u009a\u04f2\3\2\2\2\u009c\u04f5\3\2"+
		"\2\2\u009e\u0502\3\2\2\2\u00a0\u0504\3\2\2\2\u00a2\u0517\3\2\2\2\u00a4"+
		"\u0519\3\2\2\2\u00a6\u0525\3\2\2\2\u00a8\u052b\3\2\2\2\u00aa\u0530\3\2"+
		"\2\2\u00ac\u0535\3\2\2\2\u00ae\u0537\3\2\2\2\u00b0\u053d\3\2\2\2\u00b2"+
		"\u0545\3\2\2\2\u00b4\u0552\3\2\2\2\u00b6\u0556\3\2\2\2\u00b8\u055c\3\2"+
		"\2\2\u00ba\u056f\3\2\2\2\u00bc\u0572\3\2\2\2\u00be\u0578\3\2\2\2\u00c0"+
		"\u057b\3\2\2\2\u00c2\u0587\3\2\2\2\u00c4\u058d\3\2\2\2\u00c6\u0599\3\2"+
		"\2\2\u00c8\u059d\3\2\2\2\u00ca\u05a6\3\2\2\2\u00cc\u05af\3\2\2\2\u00ce"+
		"\u05b3\3\2\2\2\u00d0\u05c2\3\2\2\2\u00d2\u05c7\3\2\2\2\u00d4\u05c9\3\2"+
		"\2\2\u00d6\u05d1\3\2\2\2\u00d8\u05d5\3\2\2\2\u00da\u05dc\3\2\2\2\u00dc"+
		"\u05e1\3\2\2\2\u00de\u05eb\3\2\2\2\u00e0\u05ef\3\2\2\2\u00e2\u0604\3\2"+
		"\2\2\u00e4\u0606\3\2\2\2\u00e6\u0612\3\2\2\2\u00e8\u061a\3\2\2\2\u00ea"+
		"\u061e\3\2\2\2\u00ec\u0626\3\2\2\2\u00ee\u062d\3\2\2\2\u00f0\u0631\3\2"+
		"\2\2\u00f2\u0633\3\2\2\2\u00f4\u0637\3\2\2\2\u00f6\u063f\3\2\2\2\u00f8"+
		"\u0641\3\2\2\2\u00fa\u0651\3\2\2\2\u00fc\u0653\3\2\2\2\u00fe\u0656\3\2"+
		"\2\2\u0100\u065d\3\2\2\2\u0102\u066f\3\2\2\2\u0104\u0678\3\2\2\2\u0106"+
		"\u0687\3\2\2\2\u0108\u068b\3\2\2\2\u010a\u068e\3\2\2\2\u010c\u069b\3\2"+
		"\2\2\u010e\u06a1\3\2\2\2\u0110\u06bc\3\2\2\2\u0112\u06be\3\2\2\2\u0114"+
		"\u06c5\3\2\2\2\u0116\u06cf\3\2\2\2\u0118\u06d1\3\2\2\2\u011a\u06d4\3\2"+
		"\2\2\u011c\u06de\3\2\2\2\u011e\u06ef\3\2\2\2\u0120\u06fb\3\2\2\2\u0122"+
		"\u0704\3\2\2\2\u0124\u0714\3\2\2\2\u0126\u0720\3\2\2\2\u0128\u0727\3\2"+
		"\2\2\u012a\u0729\3\2\2\2\u012c\u072c\3\2\2\2\u012e\u0736\3\2\2\2\u0130"+
		"\u0776\3\2\2\2\u0132\u078c\3\2\2\2\u0134\u078f\3\2\2\2\u0136\u07a0\3\2"+
		"\2\2\u0138\u07a7\3\2\2\2\u013a\u07b2\3\2\2\2\u013c\u07b5\3\2\2\2\u013e"+
		"\u07b9\3\2\2\2\u0140\u07c7\3\2\2\2\u0142\u07c9\3\2\2\2\u0144\u07d1\3\2"+
		"\2\2\u0146\u07e3\3\2\2\2\u0148\u07f6\3\2\2\2\u014a\u07f8\3\2\2\2\u014c"+
		"\u0800\3\2\2\2\u014e\u0809\3\2\2\2\u0150\u080b\3\2\2\2\u0152\u0810\3\2"+
		"\2\2\u0154\u0816\3\2\2\2\u0156\u081a\3\2\2\2\u0158\u081e\3\2\2\2\u015a"+
		"\u0829\3\2\2\2\u015c\u082b\3\2\2\2\u015e\u0831\3\2\2\2\u0160\u0833\3\2"+
		"\2\2\u0162\u083c\3\2\2\2\u0164\u0849\3\2\2\2\u0166\u084f\3\2\2\2\u0168"+
		"\u0851\3\2\2\2\u016a\u0861\3\2\2\2\u016c\u0863\3\2\2\2\u016e\u0872\3\2"+
		"\2\2\u0170\u0881\3\2\2\2\u0172\u0890\3\2\2\2\u0174\u0899\3\2\2\2\u0176"+
		"\u08a9\3\2\2\2\u0178\u08b5\3\2\2\2\u017a\u08b8\3\2\2\2\u017c\u08c8\3\2"+
		"\2\2\u017e\u08cd\3\2\2\2\u0180\u08db\3\2\2\2\u0182\u08e1\3\2\2\2\u0184"+
		"\u08ec\3\2\2\2\u0186\u0904\3\2\2\2\u0188\u0910\3\2\2\2\u018a\u0920\3\2"+
		"\2\2\u018c\u0926\3\2\2\2\u018e\u092c\3\2\2\2\u0190\u0934\3\2\2\2\u0192"+
		"\u0194\7\3\2\2\u0193\u0192\3\2\2\2\u0193\u0194\3\2\2\2\u0194\u0196\3\2"+
		"\2\2\u0195\u0197\5\u00b4[\2\u0196\u0195\3\2\2\2\u0196\u0197\3\2\2\2\u0197"+
		"\u0199\3\2\2\2\u0198\u019a\5\u00b8]\2\u0199\u0198\3\2\2\2\u0199\u019a"+
		"\3\2\2\2\u019a\u019e\3\2\2\2\u019b\u019d\5\u0138\u009d\2\u019c\u019b\3"+
		"\2\2\2\u019d\u01a0\3\2\2\2\u019e\u019c\3\2\2\2\u019e\u019f\3\2\2\2\u019f"+
		"\u01a2\3\2\2\2\u01a0\u019e\3\2\2\2\u01a1\u01a3\5\u00bc_\2\u01a2\u01a1"+
		"\3\2\2\2\u01a2\u01a3\3\2\2\2\u01a3\u01a4\3\2\2\2\u01a4\u01a5\7\2\2\3\u01a5"+
		"\3\3\2\2\2\u01a6\u01a8\5\u0190\u00c9\2\u01a7\u01a9\5\24\13\2\u01a8\u01a7"+
		"\3\2\2\2\u01a8\u01a9\3\2\2\2\u01a9\u01ac\3\2\2\2\u01aa\u01ac\5\u00c2b"+
		"\2\u01ab\u01a6\3\2\2\2\u01ab\u01aa\3\2\2\2\u01ac\u01b4\3\2\2\2\u01ad\u01ae"+
		"\7\u0081\2\2\u01ae\u01b0\5\u0190\u00c9\2\u01af\u01b1\5\24\13\2\u01b0\u01af"+
		"\3\2\2\2\u01b0\u01b1\3\2\2\2\u01b1\u01b3\3\2\2\2\u01b2\u01ad\3\2\2\2\u01b3"+
		"\u01b6\3\2\2\2\u01b4\u01b2\3\2\2\2\u01b4\u01b5\3\2\2\2\u01b5\5\3\2\2\2"+
		"\u01b6\u01b4\3\2\2\2\u01b7\u01bd\5\b\5\2\u01b8\u01bc\7\u0092\2\2\u01b9"+
		"\u01bc\5\u0120\u0091\2\u01ba\u01bc\7\u0087\2\2\u01bb\u01b8\3\2\2\2\u01bb"+
		"\u01b9\3\2\2\2\u01bb\u01ba\3\2\2\2\u01bc\u01bf\3\2\2\2\u01bd\u01bb\3\2"+
		"\2\2\u01bd\u01be\3\2\2\2\u01be\7\3\2\2\2\u01bf\u01bd\3\2\2\2\u01c0\u01c5"+
		"\5\n\6\2\u01c1\u01c5\5\22\n\2\u01c2\u01c3\7k\2\2\u01c3\u01c5\7\u0087\2"+
		"\2\u01c4\u01c0\3\2\2\2\u01c4\u01c1\3\2\2\2\u01c4\u01c2\3\2\2\2\u01c5\t"+
		"\3\2\2\2\u01c6\u01c9\5\f\7\2\u01c7\u01c9\7\23\2\2\u01c8\u01c6\3\2\2\2"+
		"\u01c8\u01c7\3\2\2\2\u01c9\13\3\2\2\2\u01ca\u01ce\5\16\b\2\u01cb\u01ce"+
		"\5\20\t\2\u01cc\u01ce\7\36\2\2\u01cd\u01ca\3\2\2\2\u01cd\u01cb\3\2\2\2"+
		"\u01cd\u01cc\3\2\2\2\u01ce\r\3\2\2\2\u01cf\u01d0\t\2\2\2\u01d0\17\3\2"+
		"\2\2\u01d1\u01d2\t\3\2\2\u01d2\21\3\2\2\2\u01d3\u01d8\5\4\3\2\u01d4\u01d8"+
		"\7E\2\2\u01d5\u01d8\7$\2\2\u01d6\u01d8\7\\\2\2\u01d7\u01d3\3\2\2\2\u01d7"+
		"\u01d4\3\2\2\2\u01d7\u01d5\3\2\2\2\u01d7\u01d6\3\2\2\2\u01d8\23\3\2\2"+
		"\2\u01d9\u01da\7\u0090\2\2\u01da\u01df\5\6\4\2\u01db\u01dc\7\u0082\2\2"+
		"\u01dc\u01de\5\6\4\2\u01dd\u01db\3\2\2\2\u01de\u01e1\3\2\2\2\u01df\u01dd"+
		"\3\2\2\2\u01df\u01e0\3\2\2\2\u01e0\u01e2\3\2\2\2\u01e1\u01df\3\2\2\2\u01e2"+
		"\u01e3\7\u0091\2\2\u01e3\25\3\2\2\2\u01e4\u01e9\5\30\r\2\u01e5\u01e6\7"+
		"\u0082\2\2\u01e6\u01e8\5\30\r\2\u01e7\u01e5\3\2\2\2\u01e8\u01eb\3\2\2"+
		"\2\u01e9\u01e7\3\2\2\2\u01e9\u01ea\3\2\2\2\u01ea\27\3\2\2\2\u01eb\u01e9"+
		"\3\2\2\2\u01ec\u01ed\5\u0190\u00c9\2\u01ed\u01ee\7\u0083\2\2\u01ee\u01f0"+
		"\3\2\2\2\u01ef\u01ec\3\2\2\2\u01ef\u01f0\3\2\2\2\u01f0\u01f2\3\2\2\2\u01f1"+
		"\u01f3\t\4\2\2\u01f2\u01f1\3\2\2\2\u01f2\u01f3\3\2\2\2\u01f3\u01f4\3\2"+
		"\2\2\u01f4\u01f5\5\32\16\2\u01f5\31\3\2\2\2\u01f6\u01f9\5\36\20\2\u01f7"+
		"\u01f9\5\34\17\2\u01f8\u01f6\3\2\2\2\u01f8\u01f7\3\2\2\2\u01f9\33\3\2"+
		"\2\2\u01fa\u01fe\5d\63\2\u01fb\u01fe\5p9\2\u01fc\u01fe\5\"\22\2\u01fd"+
		"\u01fa\3\2\2\2\u01fd\u01fb\3\2\2\2\u01fd\u01fc\3\2\2\2\u01fe\35\3\2\2"+
		"\2\u01ff\u0200\5:\36\2\u0200\u0201\5 \21\2\u0201\u0202\5\32\16\2\u0202"+
		"\37\3\2\2\2\u0203\u020f\7\u008f\2\2\u0204\u020f\7\u009e\2\2\u0205\u020f"+
		"\7\u009f\2\2\u0206\u020f\7\u00a0\2\2\u0207\u020f\7\u00a1\2\2\u0208\u020f"+
		"\7\u00a2\2\2\u0209\u020f\7\u00a3\2\2\u020a\u020f\7\u00a4\2\2\u020b\u020f"+
		"\7\u00a5\2\2\u020c\u020f\7\u00a7\2\2\u020d\u020f\5\u0158\u00ad\2\u020e"+
		"\u0203\3\2\2\2\u020e\u0204\3\2\2\2\u020e\u0205\3\2\2\2\u020e\u0206\3\2"+
		"\2\2\u020e\u0207\3\2\2\2\u020e\u0208\3\2\2\2\u020e\u0209\3\2\2\2\u020e"+
		"\u020a\3\2\2\2\u020e\u020b\3\2\2\2\u020e\u020c\3\2\2\2\u020e\u020d\3\2"+
		"\2\2\u020f!\3\2\2\2\u0210\u0216\5$\23\2\u0211\u0212\7\u0092\2\2\u0212"+
		"\u0213\5\32\16\2\u0213\u0214\7\u0083\2\2\u0214\u0215\5\32\16\2\u0215\u0217"+
		"\3\2\2\2\u0216\u0211\3\2\2\2\u0216\u0217\3\2\2\2\u0217#\3\2\2\2\u0218"+
		"\u021b\5&\24\2\u0219\u021a\7\u0094\2\2\u021a\u021c\5$\23\2\u021b\u0219"+
		"\3\2\2\2\u021b\u021c\3\2\2\2\u021c%\3\2\2\2\u021d\u0222\5(\25\2\u021e"+
		"\u021f\7\u0098\2\2\u021f\u0221\5(\25\2\u0220\u021e\3\2\2\2\u0221\u0224"+
		"\3\2\2\2\u0222\u0220\3\2\2\2\u0222\u0223\3\2\2\2\u0223\'\3\2\2\2\u0224"+
		"\u0222\3\2\2\2\u0225\u022a\5*\26\2\u0226\u0227\7\u0097\2\2\u0227\u0229"+
		"\5*\26\2\u0228\u0226\3\2\2\2\u0229\u022c\3\2\2\2\u022a\u0228\3\2\2\2\u022a"+
		"\u022b\3\2\2\2\u022b)\3\2\2\2\u022c\u022a\3\2\2\2\u022d\u0232\5,\27\2"+
		"\u022e\u022f\7\u008b\2\2\u022f\u0231\5,\27\2\u0230\u022e\3\2\2\2\u0231"+
		"\u0234\3\2\2\2\u0232\u0230\3\2\2\2\u0232\u0233\3\2\2\2\u0233+\3\2\2\2"+
		"\u0234\u0232\3\2\2\2\u0235\u023a\5.\30\2\u0236\u0237\7\u008c\2\2\u0237"+
		"\u0239\5.\30\2\u0238\u0236\3\2\2\2\u0239\u023c\3\2\2\2\u023a\u0238\3\2"+
		"\2\2\u023a\u023b\3\2\2\2\u023b-\3\2\2\2\u023c\u023a\3\2\2\2\u023d\u0242"+
		"\5\60\31\2\u023e\u023f\7\u008a\2\2\u023f\u0241\5\60\31\2\u0240\u023e\3"+
		"\2\2\2\u0241\u0244\3\2\2\2\u0242\u0240\3\2\2\2\u0242\u0243\3\2\2\2\u0243"+
		"/\3\2\2\2\u0244\u0242\3\2\2\2\u0245\u024a\5\62\32\2\u0246\u0247\t\5\2"+
		"\2\u0247\u0249\5\62\32\2\u0248\u0246\3\2\2\2\u0249\u024c\3\2\2\2\u024a"+
		"\u0248\3\2\2\2\u024a\u024b\3\2\2\2\u024b\61\3\2\2\2\u024c\u024a\3\2\2"+
		"\2\u024d\u0256\5\64\33\2\u024e\u024f\t\6\2\2\u024f\u0255\5\64\33\2\u0250"+
		"\u0251\7<\2\2\u0251\u0255\5b\62\2\u0252\u0253\7\16\2\2\u0253\u0255\5\6"+
		"\4\2\u0254\u024e\3\2\2\2\u0254\u0250\3\2\2\2\u0254\u0252\3\2\2\2\u0255"+
		"\u0258\3\2\2\2\u0256\u0254\3\2\2\2\u0256\u0257\3\2\2\2\u0257\63\3\2\2"+
		"\2\u0258\u0256\3\2\2\2\u0259\u0261\5\66\34\2\u025a\u025d\7\u00a6\2\2\u025b"+
		"\u025d\5\u0156\u00ac\2\u025c\u025a\3\2\2\2\u025c\u025b\3\2\2\2\u025d\u025e"+
		"\3\2\2\2\u025e\u0260\5\66\34\2\u025f\u025c\3\2\2\2\u0260\u0263\3\2\2\2"+
		"\u0261\u025f\3\2\2\2\u0261\u0262\3\2\2\2\u0262\65\3\2\2\2\u0263\u0261"+
		"\3\2\2\2\u0264\u0269\58\35\2\u0265\u0266\t\7\2\2\u0266\u0268\58\35\2\u0267"+
		"\u0265\3\2\2\2\u0268\u026b\3\2\2\2\u0269\u0267\3\2\2\2\u0269\u026a\3\2"+
		"\2\2\u026a\67\3\2\2\2\u026b\u0269\3\2\2\2\u026c\u0271\5:\36\2\u026d\u026e"+
		"\t\b\2\2\u026e\u0270\5:\36\2\u026f\u026d\3\2\2\2\u0270\u0273\3\2\2\2\u0271"+
		"\u026f\3\2\2\2\u0271\u0272\3\2\2\2\u02729\3\2\2\2\u0273\u0271\3\2\2\2"+
		"\u0274\u028d\5<\37\2\u0275\u0276\7\u0085\2\2\u0276\u028d\5:\36\2\u0277"+
		"\u0278\7\u0086\2\2\u0278\u028d\5:\36\2\u0279\u027a\7\u008d\2\2\u027a\u028d"+
		"\5:\36\2\u027b\u027c\7\u008e\2\2\u027c\u028d\5:\36\2\u027d\u027e\7\u0095"+
		"\2\2\u027e\u028d\5:\36\2\u027f\u0280\7\u0096\2\2\u0280\u028d\5:\36\2\u0281"+
		"\u0282\7\177\2\2\u0282\u0283\5\6\4\2\u0283\u0284\7\u0080\2\2\u0284\u0285"+
		"\5:\36\2\u0285\u028d\3\2\2\2\u0286\u0287\7\21\2\2\u0287\u028d\5:\36\2"+
		"\u0288\u0289\7\u008a\2\2\u0289\u028d\5:\36\2\u028a\u028b\7\u0087\2\2\u028b"+
		"\u028d\5:\36\2\u028c\u0274\3\2\2\2\u028c\u0275\3\2\2\2\u028c\u0277\3\2"+
		"\2\2\u028c\u0279\3\2\2\2\u028c\u027b\3\2\2\2\u028c\u027d\3\2\2\2\u028c"+
		"\u027f\3\2\2\2\u028c\u0281\3\2\2\2\u028c\u0286\3\2\2\2\u028c\u0288\3\2"+
		"\2\2\u028c\u028a\3\2\2\2\u028d;\3\2\2\2\u028e\u0292\5> \2\u028f\u0291"+
		"\5B\"\2\u0290\u028f\3\2\2\2\u0291\u0294\3\2\2\2\u0292\u0290\3\2\2\2\u0292"+
		"\u0293\3\2\2\2\u0293\u02a5\3\2\2\2\u0294\u0292\3\2\2\2\u0295\u029c\5@"+
		"!\2\u0296\u029c\5\u018c\u00c7\2\u0297\u029c\7\u0095\2\2\u0298\u029c\7"+
		"\u0096\2\2\u0299\u029a\7\u0099\2\2\u029a\u029c\5\u0190\u00c9\2\u029b\u0295"+
		"\3\2\2\2\u029b\u0296\3\2\2\2\u029b\u0297\3\2\2\2\u029b\u0298\3\2\2\2\u029b"+
		"\u0299\3\2\2\2\u029c\u02a0\3\2\2\2\u029d\u029f\5B\"\2\u029e\u029d\3\2"+
		"\2\2\u029f\u02a2\3\2\2\2\u02a0\u029e\3\2\2\2\u02a0\u02a1\3\2\2\2\u02a1"+
		"\u02a4\3\2\2\2\u02a2\u02a0\3\2\2\2\u02a3\u029b\3\2\2\2\u02a4\u02a7\3\2"+
		"\2\2\u02a5\u02a3\3\2\2\2\u02a5\u02a6\3\2\2\2\u02a6=\3\2\2\2\u02a7\u02a5"+
		"\3\2\2\2\u02a8\u0316\5\u015a\u00ae\2\u02a9\u02ab\5\u0190\u00c9\2\u02aa"+
		"\u02ac\5\24\13\2\u02ab\u02aa\3\2\2\2\u02ab\u02ac\3\2\2\2\u02ac\u0316\3"+
		"\2\2\2\u02ad\u02ae\7\177\2\2\u02ae\u02af\5\32\16\2\u02af\u02b0\7\u0080"+
		"\2\2\u02b0\u0316\3\2\2\2\u02b1\u0316\5F$\2\u02b2\u0316\5\u00c2b\2\u02b3"+
		"\u0316\7r\2\2\u02b4\u0316\7_\2\2\u02b5\u02bf\7\22\2\2\u02b6\u02b7\7\u0081"+
		"\2\2\u02b7\u02b9\5\u0190\u00c9\2\u02b8\u02ba\5\24\13\2\u02b9\u02b8\3\2"+
		"\2\2\u02b9\u02ba\3\2\2\2\u02ba\u02c0\3\2\2\2\u02bb\u02bc\7}\2\2\u02bc"+
		"\u02bd\5H%\2\u02bd\u02be\7~\2\2\u02be\u02c0\3\2\2\2\u02bf\u02b6\3\2\2"+
		"\2\u02bf\u02bb\3\2\2\2\u02c0\u0316\3\2\2\2\u02c1\u02de\7C\2\2\u02c2\u02d8"+
		"\5\6\4\2\u02c3\u02d9\5\u018e\u00c8\2\u02c4\u02d9\5J&\2\u02c5\u02c6\7}"+
		"\2\2\u02c6\u02c7\5H%\2\u02c7\u02cb\7~\2\2\u02c8\u02ca\5\u0120\u0091\2"+
		"\u02c9\u02c8\3\2\2\2\u02ca\u02cd\3\2\2\2\u02cb\u02c9\3\2\2\2\u02cb\u02cc"+
		"\3\2\2\2\u02cc\u02cf\3\2\2\2\u02cd\u02cb\3\2\2\2\u02ce\u02d0\5\u0122\u0092"+
		"\2\u02cf\u02ce\3\2\2\2\u02cf\u02d0\3\2\2\2\u02d0\u02d9\3\2\2\2\u02d1\u02d3"+
		"\5\u0120\u0091\2\u02d2\u02d1\3\2\2\2\u02d3\u02d4\3\2\2\2\u02d4\u02d2\3"+
		"\2\2\2\u02d4\u02d5\3\2\2\2\u02d5\u02d6\3\2\2\2\u02d6\u02d7\5\u0122\u0092"+
		"\2\u02d7\u02d9\3\2\2\2\u02d8\u02c3\3\2\2\2\u02d8\u02c4\3\2\2\2\u02d8\u02c5"+
		"\3\2\2\2\u02d8\u02d2\3\2\2\2\u02d9\u02df\3\2\2\2\u02da\u02df\5X-\2\u02db"+
		"\u02dc\5\u0120\u0091\2\u02dc\u02dd\5\u0122\u0092\2\u02dd\u02df\3\2\2\2"+
		"\u02de\u02c2\3\2\2\2\u02de\u02da\3\2\2\2\u02de\u02db\3\2\2\2\u02df\u0316"+
		"\3\2\2\2\u02e0\u02e1\7c\2\2\u02e1\u02e5\7\177\2\2\u02e2\u02e6\5^\60\2"+
		"\u02e3\u02e6\5\6\4\2\u02e4\u02e6\7k\2\2\u02e5\u02e2\3\2\2\2\u02e5\u02e3"+
		"\3\2\2\2\u02e5\u02e4\3\2\2\2\u02e6\u02e7\3\2\2\2\u02e7\u0316\7\u0080\2"+
		"\2\u02e8\u02e9\7\32\2\2\u02e9\u02ea\7\177\2\2\u02ea\u02eb\5\32\16\2\u02eb"+
		"\u02ec\7\u0080\2\2\u02ec\u0316\3\2\2\2\u02ed\u02ee\7f\2\2\u02ee\u02ef"+
		"\7\177\2\2\u02ef\u02f0\5\32\16\2\u02f0\u02f1\7\u0080\2\2\u02f1\u0316\3"+
		"\2\2\2\u02f2\u02f3\7\37\2\2\u02f3\u02f4\7\177\2\2\u02f4\u02f5\5\6\4\2"+
		"\u02f5\u02f6\7\u0080\2\2\u02f6\u0316\3\2\2\2\u02f7\u02f9\7\20\2\2\u02f8"+
		"\u02f7\3\2\2\2\u02f8\u02f9\3\2\2\2\u02f9\u02fa\3\2\2\2\u02fa\u0300\7 "+
		"\2\2\u02fb\u02fd\7\177\2\2\u02fc\u02fe\5h\65\2\u02fd\u02fc\3\2\2\2\u02fd"+
		"\u02fe\3\2\2\2\u02fe\u02ff\3\2\2\2\u02ff\u0301\7\u0080\2\2\u0300\u02fb"+
		"\3\2\2\2\u0300\u0301\3\2\2\2\u0301\u0302\3\2\2\2\u0302\u0316\5\u008cG"+
		"\2\u0303\u0304\7Y\2\2\u0304\u0305\7\177\2\2\u0305\u0306\5\6\4\2\u0306"+
		"\u0307\7\u0080\2\2\u0307\u0316\3\2\2\2\u0308\u0309\7A\2\2\u0309\u030f"+
		"\7\177\2\2\u030a\u030b\5\u0190\u00c9\2\u030b\u030c\7\u0081\2\2\u030c\u030e"+
		"\3\2\2\2\u030d\u030a\3\2\2\2\u030e\u0311\3\2\2\2\u030f\u030d\3\2\2\2\u030f"+
		"\u0310\3\2\2\2\u0310\u0312\3\2\2\2\u0311\u030f\3\2\2\2\u0312\u0313\5\u0190"+
		"\u00c9\2\u0313\u0314\7\u0080\2\2\u0314\u0316\3\2\2\2\u0315\u02a8\3\2\2"+
		"\2\u0315\u02a9\3\2\2\2\u0315\u02ad\3\2\2\2\u0315\u02b1\3\2\2\2\u0315\u02b2"+
		"\3\2\2\2\u0315\u02b3\3\2\2\2\u0315\u02b4\3\2\2\2\u0315\u02b5\3\2\2\2\u0315"+
		"\u02c1\3\2\2\2\u0315\u02e0\3\2\2\2\u0315\u02e8\3\2\2\2\u0315\u02ed\3\2"+
		"\2\2\u0315\u02f2\3\2\2\2\u0315\u02f8\3\2\2\2\u0315\u0303\3\2\2\2\u0315"+
		"\u0308\3\2\2\2\u0316?\3\2\2\2\u0317\u0319\7\u0092\2\2\u0318\u0317\3\2"+
		"\2\2\u0318\u0319\3\2\2\2\u0319\u031a\3\2\2\2\u031a\u031b\7\u0081\2\2\u031b"+
		"\u031d\5\u0190\u00c9\2\u031c\u031e\5\24\13\2\u031d\u031c\3\2\2\2\u031d"+
		"\u031e\3\2\2\2\u031eA\3\2\2\2\u031f\u0321\7\u0092\2\2\u0320\u031f\3\2"+
		"\2\2\u0320\u0321\3\2\2\2\u0321\u0322\3\2\2\2\u0322\u0323\7}\2\2\u0323"+
		"\u0328\5D#\2\u0324\u0325\7\u0082\2\2\u0325\u0327\5D#\2\u0326\u0324\3\2"+
		"\2\2\u0327\u032a\3\2\2\2\u0328\u0326\3\2\2\2\u0328\u0329\3\2\2\2\u0329"+
		"\u032b\3\2\2\2\u032a\u0328\3\2\2\2\u032b\u032c\7~\2\2\u032cC\3\2\2\2\u032d"+
		"\u032e\5\u0190\u00c9\2\u032e\u032f\7\u0083\2\2\u032f\u0331\3\2\2\2\u0330"+
		"\u032d\3\2\2\2\u0330\u0331\3\2\2\2\u0331\u0332\3\2\2\2\u0332\u0333\5\32"+
		"\16\2\u0333E\3\2\2\2\u0334\u0335\t\t\2\2\u0335G\3\2\2\2\u0336\u033b\5"+
		"\32\16\2\u0337\u0338\7\u0082\2\2\u0338\u033a\5\32\16\2\u0339\u0337\3\2"+
		"\2\2\u033a\u033d\3\2\2\2\u033b\u0339\3\2\2\2\u033b\u033c\3\2\2\2\u033c"+
		"I\3\2\2\2\u033d\u033b\3\2\2\2\u033e\u0341\5L\'\2\u033f\u0341\5T+\2\u0340"+
		"\u033e\3\2\2\2\u0340\u033f\3\2\2\2\u0341K\3\2\2\2\u0342\u0347\7{\2\2\u0343"+
		"\u0345\5N(\2\u0344\u0346\7\u0082\2\2\u0345\u0344\3\2\2\2\u0345\u0346\3"+
		"\2\2\2\u0346\u0348\3\2\2\2\u0347\u0343\3\2\2\2\u0347\u0348\3\2\2\2\u0348"+
		"\u0349\3\2\2\2\u0349\u034a\7|\2\2\u034aM\3\2\2\2\u034b\u0350\5P)\2\u034c"+
		"\u034d\7\u0082\2\2\u034d\u034f\5P)\2\u034e\u034c\3\2\2\2\u034f\u0352\3"+
		"\2\2\2\u0350\u034e\3\2\2\2\u0350\u0351\3\2\2\2\u0351O\3\2\2\2\u0352\u0350"+
		"\3\2\2\2\u0353\u0359\5\u0190\u00c9\2\u0354\u0355\7}\2\2\u0355\u0356\5"+
		"\32\16\2\u0356\u0357\7~\2\2\u0357\u0359\3\2\2\2\u0358\u0353\3\2\2\2\u0358"+
		"\u0354\3\2\2\2\u0359\u035a\3\2\2\2\u035a\u035b\7\u008f\2\2\u035b\u035c"+
		"\5R*\2\u035cQ\3\2\2\2\u035d\u0360\5\32\16\2\u035e\u0360\5J&\2\u035f\u035d"+
		"\3\2\2\2\u035f\u035e\3\2\2\2\u0360S\3\2\2\2\u0361\u0362\7{\2\2\u0362\u0367"+
		"\5V,\2\u0363\u0364\7\u0082\2\2\u0364\u0366\5V,\2\u0365\u0363\3\2\2\2\u0366"+
		"\u0369\3\2\2\2\u0367\u0365\3\2\2\2\u0367\u0368\3\2\2\2\u0368\u036b\3\2"+
		"\2\2\u0369\u0367\3\2\2\2\u036a\u036c\7\u0082\2\2\u036b\u036a\3\2\2\2\u036b"+
		"\u036c\3\2\2\2\u036c\u036d\3\2\2\2\u036d\u036e\7|\2\2\u036eU\3\2\2\2\u036f"+
		"\u0375\5\34\17\2\u0370\u0371\7{\2\2\u0371\u0372\5H%\2\u0372\u0373\7|\2"+
		"\2\u0373\u0375\3\2\2\2\u0374\u036f\3\2\2\2\u0374\u0370\3\2\2\2\u0375W"+
		"\3\2\2\2\u0376\u037b\7{\2\2\u0377\u0379\5Z.\2\u0378\u037a\7\u0082\2\2"+
		"\u0379\u0378\3\2\2\2\u0379\u037a\3\2\2\2\u037a\u037c\3\2\2\2\u037b\u0377"+
		"\3\2\2\2\u037b\u037c\3\2\2\2\u037c\u037d\3\2\2\2\u037d\u037e\7|\2\2\u037e"+
		"Y\3\2\2\2\u037f\u0384\5\\/\2\u0380\u0381\7\u0082\2\2\u0381\u0383\5\\/"+
		"\2\u0382\u0380\3\2\2\2\u0383\u0386\3\2\2\2\u0384\u0382\3\2\2\2\u0384\u0385"+
		"\3\2\2\2\u0385[\3\2\2\2\u0386\u0384\3\2\2\2\u0387\u038d\5<\37\2\u0388"+
		"\u0389\5\u0190\u00c9\2\u0389\u038a\7\u008f\2\2\u038a\u038b\5\32\16\2\u038b"+
		"\u038d\3\2\2\2\u038c\u0387\3\2\2\2\u038c\u0388\3\2\2\2\u038d]\3\2\2\2"+
		"\u038e\u0397\5\u0190\u00c9\2\u038f\u0391\5`\61\2\u0390\u038f\3\2\2\2\u0390"+
		"\u0391\3\2\2\2\u0391\u0398\3\2\2\2\u0392\u0393\7\u0093\2\2\u0393\u0395"+
		"\5\u0190\u00c9\2\u0394\u0396\5`\61\2\u0395\u0394\3\2\2\2\u0395\u0396\3"+
		"\2\2\2\u0396\u0398\3\2\2\2\u0397\u0390\3\2\2\2\u0397\u0392\3\2\2\2\u0398"+
		"\u03a0\3\2\2\2\u0399\u039a\7\u0081\2\2\u039a\u039c\5\u0190\u00c9\2\u039b"+
		"\u039d\5`\61\2\u039c\u039b\3\2\2\2\u039c\u039d\3\2\2\2\u039d\u039f\3\2"+
		"\2\2\u039e\u0399\3\2\2\2\u039f\u03a2\3\2\2\2\u03a0\u039e\3\2\2\2\u03a0"+
		"\u03a1\3\2\2\2\u03a1_\3\2\2\2\u03a2\u03a0\3\2\2\2\u03a3\u03a7\7\u0090"+
		"\2\2\u03a4\u03a6\7\u0082\2\2\u03a5\u03a4\3\2\2\2\u03a6\u03a9\3\2\2\2\u03a7"+
		"\u03a5\3\2\2\2\u03a7\u03a8\3\2\2\2\u03a8\u03aa\3\2\2\2\u03a9\u03a7\3\2"+
		"\2\2\u03aa\u03ab\7\u0091\2\2\u03aba\3\2\2\2\u03ac\u03b1\5\b\5\2\u03ad"+
		"\u03b0\5\u0120\u0091\2\u03ae\u03b0\7\u0087\2\2\u03af\u03ad\3\2\2\2\u03af"+
		"\u03ae\3\2\2\2\u03b0\u03b3\3\2\2\2\u03b1\u03af\3\2\2\2\u03b1\u03b2\3\2"+
		"\2\2\u03b2\u03b5\3\2\2\2\u03b3\u03b1\3\2\2\2\u03b4\u03b6\7\u0092\2\2\u03b5"+
		"\u03b4\3\2\2\2\u03b5\u03b6\3\2\2\2\u03b6c\3\2\2\2\u03b7\u03b9\7\20\2\2"+
		"\u03b8\u03b7\3\2\2\2\u03b8\u03b9\3\2\2\2\u03b9\u03ba\3\2\2\2\u03ba\u03bb"+
		"\5f\64\2\u03bb\u03bc\5\u0154\u00ab\2\u03bc\u03bd\5n8\2\u03bde\3\2\2\2"+
		"\u03be\u03bf\7\177\2\2\u03bf\u03ca\7\u0080\2\2\u03c0\u03c1\7\177\2\2\u03c1"+
		"\u03c2\5h\65\2\u03c2\u03c3\7\u0080\2\2\u03c3\u03ca\3\2\2\2\u03c4\u03c5"+
		"\7\177\2\2\u03c5\u03c6\5l\67\2\u03c6\u03c7\7\u0080\2\2\u03c7\u03ca\3\2"+
		"\2\2\u03c8\u03ca\5\u0190\u00c9\2\u03c9\u03be\3\2\2\2\u03c9\u03c0\3\2\2"+
		"\2\u03c9\u03c4\3\2\2\2\u03c9\u03c8\3\2\2\2\u03cag\3\2\2\2\u03cb\u03d0"+
		"\5j\66\2\u03cc\u03cd\7\u0082\2\2\u03cd\u03cf\5j\66\2\u03ce\u03cc\3\2\2"+
		"\2\u03cf\u03d2\3\2\2\2\u03d0\u03ce\3\2\2\2\u03d0\u03d1\3\2\2\2\u03d1i"+
		"\3\2\2\2\u03d2\u03d0\3\2\2\2\u03d3\u03d5\t\4\2\2\u03d4\u03d3\3\2\2\2\u03d4"+
		"\u03d5\3\2\2\2\u03d5\u03d6\3\2\2\2\u03d6\u03d7\5\6\4\2\u03d7\u03d8\5\u0190"+
		"\u00c9\2\u03d8k\3\2\2\2\u03d9\u03de\5\u0190\u00c9\2\u03da\u03db\7\u0082"+
		"\2\2\u03db\u03dd\5\u0190\u00c9\2\u03dc\u03da\3\2\2\2\u03dd\u03e0\3\2\2"+
		"\2\u03de\u03dc\3\2\2\2\u03de\u03df\3\2\2\2\u03dfm\3\2\2\2\u03e0\u03de"+
		"\3\2\2\2\u03e1\u03e4\5\32\16\2\u03e2\u03e4\5\u008cG\2\u03e3\u03e1\3\2"+
		"\2\2\u03e3\u03e2\3\2\2\2\u03e4o\3\2\2\2\u03e5\u03e6\5r:\2\u03e6\u03e7"+
		"\5t;\2\u03e7q\3\2\2\2\u03e8\u03ea\7\61\2\2\u03e9\u03eb\5\6\4\2\u03ea\u03e9"+
		"\3\2\2\2\u03ea\u03eb\3\2\2\2\u03eb\u03ec\3\2\2\2\u03ec\u03ed\5\u0190\u00c9"+
		"\2\u03ed\u03ee\7\67\2\2\u03ee\u03ef\5\32\16\2\u03efs\3\2\2\2\u03f0\u03f2"+
		"\5v<\2\u03f1\u03f0\3\2\2\2\u03f2\u03f5\3\2\2\2\u03f3\u03f1\3\2\2\2\u03f3"+
		"\u03f4\3\2\2\2\u03f4\u03f6\3\2\2\2\u03f5\u03f3\3\2\2\2\u03f6\u03f8\5\u0082"+
		"B\2\u03f7\u03f9\5\u0084C\2\u03f8\u03f7\3\2\2\2\u03f8\u03f9\3\2\2\2\u03f9"+
		"u\3\2\2\2\u03fa\u0400\5r:\2\u03fb\u0400\5x=\2\u03fc\u0400\5z>\2\u03fd"+
		"\u0400\5|?\2\u03fe\u0400\5~@\2\u03ff\u03fa\3\2\2\2\u03ff\u03fb\3\2\2\2"+
		"\u03ff\u03fc\3\2\2\2\u03ff\u03fd\3\2\2\2\u03ff\u03fe\3\2\2\2\u0400w\3"+
		"\2\2\2\u0401\u0402\7>\2\2\u0402\u0403\5\u0190\u00c9\2\u0403\u0404\7\u008f"+
		"\2\2\u0404\u0405\5\32\16\2\u0405y\3\2\2\2\u0406\u0407\7n\2\2\u0407\u0408"+
		"\5\32\16\2\u0408{\3\2\2\2\u0409\u040b\7=\2\2\u040a\u040c\5\6\4\2\u040b"+
		"\u040a\3\2\2\2\u040b\u040c\3\2\2\2\u040c\u040d\3\2\2\2\u040d\u040e\5\u0190"+
		"\u00c9\2\u040e\u040f\7\67\2\2\u040f\u0410\5\32\16\2\u0410\u0411\7F\2\2"+
		"\u0411\u0412\5\32\16\2\u0412\u0413\7\'\2\2\u0413\u0416\5\32\16\2\u0414"+
		"\u0415\7;\2\2\u0415\u0417\5\u0190\u00c9\2\u0416\u0414\3\2\2\2\u0416\u0417"+
		"\3\2\2\2\u0417}\3\2\2\2\u0418\u0419\7H\2\2\u0419\u041e\5\u0080A\2\u041a"+
		"\u041b\7\u0082\2\2\u041b\u041d\5\u0080A\2\u041c\u041a\3\2\2\2\u041d\u0420"+
		"\3\2\2\2\u041e\u041c\3\2\2\2\u041e\u041f\3\2\2\2\u041f\177\3\2\2\2\u0420"+
		"\u041e\3\2\2\2\u0421\u0423\5\32\16\2\u0422\u0424\t\n\2\2\u0423\u0422\3"+
		"\2\2\2\u0423\u0424\3\2\2\2\u0424\u0081\3\2\2\2\u0425\u0426\7V\2\2\u0426"+
		"\u042d\5\32\16\2\u0427\u0428\7\64\2\2\u0428\u0429\5\32\16\2\u0429\u042a"+
		"\7\25\2\2\u042a\u042b\5\32\16\2\u042b\u042d\3\2\2\2\u042c\u0425\3\2\2"+
		"\2\u042c\u0427\3\2\2\2\u042d\u0083\3\2\2\2\u042e\u042f\7;\2\2\u042f\u0430"+
		"\5\u0190\u00c9\2\u0430\u0431\5t;\2\u0431\u0085\3\2\2\2\u0432\u0433\5\u0190"+
		"\u00c9\2\u0433\u0434\7\u0083\2\2\u0434\u0435\5\u0086D\2\u0435\u043e\3"+
		"\2\2\2\u0436\u0439\5\u008eH\2\u0437\u0439\5\u0094K\2\u0438\u0436\3\2\2"+
		"\2\u0438\u0437\3\2\2\2\u0439\u043a\3\2\2\2\u043a\u043b\7\u0084\2\2\u043b"+
		"\u043e\3\2\2\2\u043c\u043e\5\u0088E\2\u043d\u0432\3\2\2\2\u043d\u0438"+
		"\3\2\2\2\u043d\u043c\3\2\2\2\u043e\u0087\3\2\2\2\u043f\u0442\5\u008cG"+
		"\2\u0440\u0442\5\u008aF\2\u0441\u043f\3\2\2\2\u0441\u0440\3\2\2\2\u0442"+
		"\u0089\3\2\2\2\u0443\u04c3\7\u0084\2\2\u0444\u0445\5\32\16\2\u0445\u0446"+
		"\7\u0084\2\2\u0446\u04c3\3\2\2\2\u0447\u0448\7\65\2\2\u0448\u0449\7\177"+
		"\2\2\u0449\u044a\5\32\16\2\u044a\u044b\7\u0080\2\2\u044b\u044e\5\u0096"+
		"L\2\u044c\u044d\7%\2\2\u044d\u044f\5\u0096L\2\u044e\u044c\3\2\2\2\u044e"+
		"\u044f\3\2\2\2\u044f\u04c3\3\2\2\2\u0450\u0451\7^\2\2\u0451\u0452\7\177"+
		"\2\2\u0452\u0453\5\32\16\2\u0453\u0454\7\u0080\2\2\u0454\u0458\7{\2\2"+
		"\u0455\u0457\5\u0098M\2\u0456\u0455\3\2\2\2\u0457\u045a\3\2\2\2\u0458"+
		"\u0456\3\2\2\2\u0458\u0459\3\2\2\2\u0459\u045b\3\2\2\2\u045a\u0458\3\2"+
		"\2\2\u045b\u045c\7|\2\2\u045c\u04c3\3\2\2\2\u045d\u045e\7o\2\2\u045e\u045f"+
		"\7\177\2\2\u045f\u0460\5\32\16\2\u0460\u0461\7\u0080\2\2\u0461\u0462\5"+
		"\u0088E\2\u0462\u04c3\3\2\2\2\u0463\u0464\7\"\2\2\u0464\u0465\5\u0088"+
		"E\2\u0465\u0466\7o\2\2\u0466\u0467\7\177\2\2\u0467\u0468\5\32\16\2\u0468"+
		"\u0469\7\u0080\2\2\u0469\u046a\7\u0084\2\2\u046a\u04c3\3\2\2\2\u046b\u046c"+
		"\7/\2\2\u046c\u046e\7\177\2\2\u046d\u046f\5\u009eP\2\u046e\u046d\3\2\2"+
		"\2\u046e\u046f\3\2\2\2\u046f\u0470\3\2\2\2\u0470\u0472\7\u0084\2\2\u0471"+
		"\u0473\5\32\16\2\u0472\u0471\3\2\2\2\u0472\u0473\3\2\2\2\u0473\u0474\3"+
		"\2\2\2\u0474\u0476\7\u0084\2\2\u0475\u0477\5\u00a0Q\2\u0476\u0475\3\2"+
		"\2\2\u0476\u0477\3\2\2\2\u0477\u0478\3\2\2\2\u0478\u0479\7\u0080\2\2\u0479"+
		"\u04c3\5\u0088E\2\u047a\u047b\7\60\2\2\u047b\u047c\7\177\2\2\u047c\u047d"+
		"\5\6\4\2\u047d\u047e\5\u0190\u00c9\2\u047e\u047f\7\67\2\2\u047f\u0480"+
		"\5\32\16\2\u0480\u0481\7\u0080\2\2\u0481\u0482\5\u0088E\2\u0482\u04c3"+
		"\3\2\2\2\u0483\u0484\7\24\2\2\u0484\u04c3\7\u0084\2\2\u0485\u0486\7\35"+
		"\2\2\u0486\u04c3\7\u0084\2\2\u0487\u048c\7\63\2\2\u0488\u048d\5\u0190"+
		"\u00c9\2\u0489\u048a\7\27\2\2\u048a\u048d\5\32\16\2\u048b\u048d\7\37\2"+
		"\2\u048c\u0488\3\2\2\2\u048c\u0489\3\2\2\2\u048c\u048b\3\2\2\2\u048d\u048e"+
		"\3\2\2\2\u048e\u04c3\7\u0084\2\2\u048f\u0491\7S\2\2\u0490\u0492\5\32\16"+
		"\2\u0491\u0490\3\2\2\2\u0491\u0492\3\2\2\2\u0492\u0493\3\2\2\2\u0493\u04c3"+
		"\7\u0084\2\2\u0494\u0496\7`\2\2\u0495\u0497\5\32\16\2\u0496\u0495\3\2"+
		"\2\2\u0496\u0497\3\2\2\2\u0497\u0498\3\2\2\2\u0498\u04c3\7\u0084\2\2\u0499"+
		"\u049a\7b\2\2\u049a\u04a0\5\u008cG\2\u049b\u049d\5\u00a2R\2\u049c\u049e"+
		"\5\u00aaV\2\u049d\u049c\3\2\2\2\u049d\u049e\3\2\2\2\u049e\u04a1\3\2\2"+
		"\2\u049f\u04a1\5\u00aaV\2\u04a0\u049b\3\2\2\2\u04a0\u049f\3\2\2\2\u04a1"+
		"\u04c3\3\2\2\2\u04a2\u04a3\7\32\2\2\u04a3\u04c3\5\u008cG\2\u04a4\u04a5"+
		"\7f\2\2\u04a5\u04c3\5\u008cG\2\u04a6\u04a7\7?\2\2\u04a7\u04a8\7\177\2"+
		"\2\u04a8\u04a9\5\32\16\2\u04a9\u04aa\7\u0080\2\2\u04aa\u04ab\5\u0088E"+
		"\2\u04ab\u04c3\3\2\2\2\u04ac\u04ad\7i\2\2\u04ad\u04ae\7\177\2\2\u04ae"+
		"\u04af\5\u00acW\2\u04af\u04b0\7\u0080\2\2\u04b0\u04b1\5\u0088E\2\u04b1"+
		"\u04c3\3\2\2\2\u04b2\u04b6\7p\2\2\u04b3\u04b4\7S\2\2\u04b4\u04b7\5\32"+
		"\16\2\u04b5\u04b7\7\24\2\2\u04b6\u04b3\3\2\2\2\u04b6\u04b5\3\2\2\2\u04b7"+
		"\u04b8\3\2\2\2\u04b8\u04c3\7\u0084\2\2\u04b9\u04ba\7g\2\2\u04ba\u04c3"+
		"\5\u008cG\2\u04bb\u04bc\7-\2\2\u04bc\u04bd\7\177\2\2\u04bd\u04be\5\u0148"+
		"\u00a5\2\u04be\u04bf\5\u014a\u00a6\2\u04bf\u04c0\7\u0080\2\2\u04c0\u04c1"+
		"\5\u0088E\2\u04c1\u04c3\3\2\2\2\u04c2\u0443\3\2\2\2\u04c2\u0444\3\2\2"+
		"\2\u04c2\u0447\3\2\2\2\u04c2\u0450\3\2\2\2\u04c2\u045d\3\2\2\2\u04c2\u0463"+
		"\3\2\2\2\u04c2\u046b\3\2\2\2\u04c2\u047a\3\2\2\2\u04c2\u0483\3\2\2\2\u04c2"+
		"\u0485\3\2\2\2\u04c2\u0487\3\2\2\2\u04c2\u048f\3\2\2\2\u04c2\u0494\3\2"+
		"\2\2\u04c2\u0499\3\2\2\2\u04c2\u04a2\3\2\2\2\u04c2\u04a4\3\2\2\2\u04c2"+
		"\u04a6\3\2\2\2\u04c2\u04ac\3\2\2\2\u04c2\u04b2\3\2\2\2\u04c2\u04b9\3\2"+
		"\2\2\u04c2\u04bb\3\2\2\2\u04c3\u008b\3\2\2\2\u04c4\u04c6\7{\2\2\u04c5"+
		"\u04c7\5\u009cO\2\u04c6\u04c5\3\2\2\2\u04c6\u04c7\3\2\2\2\u04c7\u04c8"+
		"\3\2\2\2\u04c8\u04c9\7|\2\2\u04c9\u008d\3\2\2\2\u04ca\u04cb\5\6\4\2\u04cb"+
		"\u04d0\5\u0090I\2\u04cc\u04cd\7\u0082\2\2\u04cd\u04cf\5\u0090I\2\u04ce"+
		"\u04cc\3\2\2\2\u04cf\u04d2\3\2\2\2\u04d0\u04ce\3\2\2\2\u04d0\u04d1\3\2"+
		"\2\2\u04d1\u008f\3\2\2\2\u04d2\u04d0\3\2\2\2\u04d3\u04d6\5\u0190\u00c9"+
		"\2\u04d4\u04d5\7\u008f\2\2\u04d5\u04d7\5\u0092J\2\u04d6\u04d4\3\2\2\2"+
		"\u04d6\u04d7\3\2\2\2\u04d7\u0091\3\2\2\2\u04d8\u04dc\5\32\16\2\u04d9\u04dc"+
		"\5\u0122\u0092\2\u04da\u04dc\5\u0152\u00aa\2\u04db\u04d8\3\2\2\2\u04db"+
		"\u04d9\3\2\2\2\u04db\u04da\3\2\2\2\u04dc\u0093\3\2\2\2\u04dd\u04de\7\34"+
		"\2\2\u04de\u04df\5\6\4\2\u04df\u04e0\5\u00e6t\2\u04e0\u0095\3\2\2\2\u04e1"+
		"\u04e4\5\u008cG\2\u04e2\u04e4\5\u008aF\2\u04e3\u04e1\3\2\2\2\u04e3\u04e2"+
		"\3\2\2\2\u04e4\u0097\3\2\2\2\u04e5\u04e7\5\u009aN\2\u04e6\u04e5\3\2\2"+
		"\2\u04e7\u04e8\3\2\2\2\u04e8\u04e6\3\2\2\2\u04e8\u04e9\3\2\2\2\u04e9\u04ea"+
		"\3\2\2\2\u04ea\u04eb\5\u009cO\2\u04eb\u0099\3\2\2\2\u04ec\u04ed\7\27\2"+
		"\2\u04ed\u04ee\5\32\16\2\u04ee\u04ef\7\u0083\2\2\u04ef\u04f3\3\2\2\2\u04f0"+
		"\u04f1\7\37\2\2\u04f1\u04f3\7\u0083\2\2\u04f2\u04ec\3\2\2\2\u04f2\u04f0"+
		"\3\2\2\2\u04f3\u009b\3\2\2\2\u04f4\u04f6\5\u0086D\2\u04f5\u04f4\3\2\2"+
		"\2\u04f6\u04f7\3\2\2\2\u04f7\u04f5\3\2\2\2\u04f7\u04f8\3\2\2\2\u04f8\u009d"+
		"\3\2\2\2\u04f9\u0503\5\u008eH\2\u04fa\u04ff\5\32\16\2\u04fb\u04fc\7\u0082"+
		"\2\2\u04fc\u04fe\5\32\16\2\u04fd\u04fb\3\2\2\2\u04fe\u0501\3\2\2\2\u04ff"+
		"\u04fd\3\2\2\2\u04ff\u0500\3\2\2\2\u0500\u0503\3\2\2\2\u0501\u04ff\3\2"+
		"\2\2\u0502\u04f9\3\2\2\2\u0502\u04fa\3\2\2\2\u0503\u009f\3\2\2\2\u0504"+
		"\u0509\5\32\16\2\u0505\u0506\7\u0082\2\2\u0506\u0508\5\32\16\2\u0507\u0505"+
		"\3\2\2\2\u0508\u050b\3\2\2\2\u0509\u0507\3\2\2\2\u0509\u050a\3\2\2\2\u050a"+
		"\u00a1\3\2\2\2\u050b\u0509\3\2\2\2\u050c\u0510\5\u00a4S\2\u050d\u050f"+
		"\5\u00a4S\2\u050e\u050d\3\2\2\2\u050f\u0512\3\2\2\2\u0510\u050e\3\2\2"+
		"\2\u0510\u0511\3\2\2\2\u0511\u0514\3\2\2\2\u0512\u0510\3\2\2\2\u0513\u0515"+
		"\5\u00a6T\2\u0514\u0513\3\2\2\2\u0514\u0515\3\2\2\2\u0515\u0518\3\2\2"+
		"\2\u0516\u0518\5\u00a6T\2\u0517\u050c\3\2\2\2\u0517\u0516\3\2\2\2\u0518"+
		"\u00a3\3\2\2\2\u0519\u051a\7\30\2\2\u051a\u051b\7\177\2\2\u051b\u051d"+
		"\5\22\n\2\u051c\u051e\5\u0190\u00c9\2\u051d\u051c\3\2\2\2\u051d\u051e"+
		"\3\2\2\2\u051e\u051f\3\2\2\2\u051f\u0521\7\u0080\2\2\u0520\u0522\5\u00a8"+
		"U\2\u0521\u0520\3\2\2\2\u0521\u0522\3\2\2\2\u0522\u0523\3\2\2\2\u0523"+
		"\u0524\5\u008cG\2\u0524\u00a5\3\2\2\2\u0525\u0527\7\30\2\2\u0526\u0528"+
		"\5\u00a8U\2\u0527\u0526\3\2\2\2\u0527\u0528\3\2\2\2\u0528\u0529\3\2\2"+
		"\2\u0529\u052a\5\u008cG\2\u052a\u00a7\3\2\2\2\u052b\u052c\7m\2\2\u052c"+
		"\u052d\7\177\2\2\u052d\u052e\5\32\16\2\u052e\u052f\7\u0080\2\2\u052f\u00a9"+
		"\3\2\2\2\u0530\u0531\7,\2\2\u0531\u0532\5\u008cG\2\u0532\u00ab\3\2\2\2"+
		"\u0533\u0536\5\u008eH\2\u0534\u0536\5\32\16\2\u0535\u0533\3\2\2\2\u0535"+
		"\u0534\3\2\2\2\u0536\u00ad\3\2\2\2\u0537\u0538\7B\2\2\u0538\u0539\5\u00b0"+
		"Y\2\u0539\u053b\5\u00b2Z\2\u053a\u053c\7\u0084\2\2\u053b\u053a\3\2\2\2"+
		"\u053b\u053c\3\2\2\2\u053c\u00af\3\2\2\2\u053d\u0542\5\u0190\u00c9\2\u053e"+
		"\u053f\7\u0081\2\2\u053f\u0541\5\u0190\u00c9\2\u0540\u053e\3\2\2\2\u0541"+
		"\u0544\3\2\2\2\u0542\u0540\3\2\2\2\u0542\u0543\3\2\2\2\u0543\u00b1\3\2"+
		"\2\2\u0544\u0542\3\2\2\2\u0545\u0547\7{\2\2\u0546\u0548\5\u00b4[\2\u0547"+
		"\u0546\3\2\2\2\u0547\u0548\3\2\2\2\u0548\u054a\3\2\2\2\u0549\u054b\5\u00b8"+
		"]\2\u054a\u0549\3\2\2\2\u054a\u054b\3\2\2\2\u054b\u054d\3\2\2\2\u054c"+
		"\u054e\5\u00bc_\2\u054d\u054c\3\2\2\2\u054d\u054e\3\2\2\2\u054e\u054f"+
		"\3\2\2\2\u054f\u0550\7|\2\2\u0550\u00b3\3\2\2\2\u0551\u0553\5\u00b6\\"+
		"\2\u0552\u0551\3\2\2\2\u0553\u0554\3\2\2\2\u0554\u0552\3\2\2\2\u0554\u0555"+
		"\3\2\2\2\u0555\u00b5\3\2\2\2\u0556\u0557\7*\2\2\u0557\u0558\7\f\2\2\u0558"+
		"\u0559\5\u0190\u00c9\2\u0559\u055a\7\u0084\2\2\u055a\u00b7\3\2\2\2\u055b"+
		"\u055d\5\u00ba^\2\u055c\u055b\3\2\2\2\u055d\u055e\3\2\2\2\u055e\u055c"+
		"\3\2\2\2\u055e\u055f\3\2\2\2\u055f\u00b9\3\2\2\2\u0560\u0561\7i\2\2\u0561"+
		"\u0562\5\u0190\u00c9\2\u0562\u0563\7\u008f\2\2\u0563\u0564\5\4\3\2\u0564"+
		"\u0565\7\u0084\2\2\u0565\u0570\3\2\2\2\u0566\u0567\7i\2\2\u0567\u0568"+
		"\5\4\3\2\u0568\u0569\7\u0084\2\2\u0569\u0570\3\2\2\2\u056a\u056b\7i\2"+
		"\2\u056b\u056c\7[\2\2\u056c\u056d\5\4\3\2\u056d\u056e\7\u0084\2\2\u056e"+
		"\u0570\3\2\2\2\u056f\u0560\3\2\2\2\u056f\u0566\3\2\2\2\u056f\u056a\3\2"+
		"\2\2\u0570\u00bb\3\2\2\2\u0571\u0573\5\u00be`\2\u0572\u0571\3\2\2\2\u0573"+
		"\u0574\3\2\2\2\u0574\u0572\3\2\2\2\u0574\u0575\3\2\2\2\u0575\u00bd\3\2"+
		"\2\2\u0576\u0579\5\u00aeX\2\u0577\u0579\5\u00c0a\2\u0578\u0576\3\2\2\2"+
		"\u0578\u0577\3\2\2\2\u0579\u00bf\3\2\2\2\u057a\u057c\5\u013c\u009f\2\u057b"+
		"\u057a\3\2\2\2\u057b\u057c\3\2\2\2\u057c\u057e\3\2\2\2\u057d\u057f\5\u00de"+
		"p\2\u057e\u057d\3\2\2\2\u057e\u057f\3\2\2\2\u057f\u0585\3\2\2\2\u0580"+
		"\u0586\5\u016c\u00b7\2\u0581\u0586\5\u016e\u00b8\2\u0582\u0586\5\u0170"+
		"\u00b9\2\u0583\u0586\5\u0172\u00ba\2\u0584\u0586\5\u0174\u00bb\2\u0585"+
		"\u0580\3\2\2\2\u0585\u0581\3\2\2\2\u0585\u0582\3\2\2\2\u0585\u0583\3\2"+
		"\2\2\u0585\u0584\3\2\2\2\u0586\u00c1\3\2\2\2\u0587\u0588\5\u0190\u00c9"+
		"\2\u0588\u0589\7\u0093\2\2\u0589\u058b\5\u0190\u00c9\2\u058a\u058c\5\24"+
		"\13\2\u058b\u058a\3\2\2\2\u058b\u058c\3\2\2\2\u058c\u00c3\3\2\2\2\u058d"+
		"\u058e\7\u0090\2\2\u058e\u0593\5\u00c6d\2\u058f\u0590\7\u0082\2\2\u0590"+
		"\u0592\5\u00c6d\2\u0591\u058f\3\2\2\2\u0592\u0595\3\2\2\2\u0593\u0591"+
		"\3\2\2\2\u0593\u0594\3\2\2\2\u0594\u0596\3\2\2\2\u0595\u0593\3\2\2\2\u0596"+
		"\u0597\7\u0091\2\2\u0597\u00c5\3\2\2\2\u0598\u059a\5\u013c\u009f\2\u0599"+
		"\u0598\3\2\2\2\u0599\u059a\3\2\2\2\u059a\u059b\3\2\2\2\u059b\u059c\5\u0190"+
		"\u00c9\2\u059c\u00c7\3\2\2\2\u059d\u059e\7\u0083\2\2\u059e\u05a3\5\22"+
		"\n\2\u059f\u05a0\7\u0082\2\2\u05a0\u05a2\5\4\3\2\u05a1\u059f\3\2\2\2\u05a2"+
		"\u05a5\3\2\2\2\u05a3\u05a1\3\2\2\2\u05a3\u05a4\3\2\2\2\u05a4\u00c9\3\2"+
		"\2\2\u05a5\u05a3\3\2\2\2\u05a6\u05ab\5\4\3\2\u05a7\u05a8\7\u0082\2\2\u05a8"+
		"\u05aa\5\4\3\2\u05a9\u05a7\3\2\2\2\u05aa\u05ad\3\2\2\2\u05ab\u05a9\3\2"+
		"\2\2\u05ab\u05ac\3\2\2\2\u05ac\u00cb\3\2\2\2\u05ad\u05ab\3\2\2\2\u05ae"+
		"\u05b0\5\u00ceh\2\u05af\u05ae\3\2\2\2\u05b0\u05b1\3\2\2\2\u05b1\u05af"+
		"\3\2\2\2\u05b1\u05b2\3\2\2\2\u05b2\u00cd\3\2\2\2\u05b3\u05b4\7n\2\2\u05b4"+
		"\u05b5\5\u0190\u00c9\2\u05b5\u05b6\7\u0083\2\2\u05b6\u05b7\5\u00d0i\2"+
		"\u05b7\u00cf\3\2\2\2\u05b8\u05c3\5\u00d6l\2\u05b9\u05bc\5\u00d2j\2\u05ba"+
		"\u05bb\7\u0082\2\2\u05bb\u05bd\5\u00d4k\2\u05bc\u05ba\3\2\2\2\u05bc\u05bd"+
		"\3\2\2\2\u05bd\u05c0\3\2\2\2\u05be\u05bf\7\u0082\2\2\u05bf\u05c1\5\u00d6"+
		"l\2\u05c0\u05be\3\2\2\2\u05c0\u05c1\3\2\2\2\u05c1\u05c3\3\2\2\2\u05c2"+
		"\u05b8\3\2\2\2\u05c2\u05b9\3\2\2\2\u05c3\u00d1\3\2\2\2\u05c4\u05c8\5\22"+
		"\n\2\u05c5\u05c8\7\33\2\2\u05c6\u05c8\7]\2\2\u05c7\u05c4\3\2\2\2\u05c7"+
		"\u05c5\3\2\2\2\u05c7\u05c6\3\2\2\2\u05c8\u00d3\3\2\2\2\u05c9\u05ce\5\4"+
		"\3\2\u05ca\u05cb\7\u0082\2\2\u05cb\u05cd\5\4\3\2\u05cc\u05ca\3\2\2\2\u05cd"+
		"\u05d0\3\2\2\2\u05ce\u05cc\3\2\2\2\u05ce\u05cf\3\2\2\2\u05cf\u00d5\3\2"+
		"\2\2\u05d0\u05ce\3\2\2\2\u05d1\u05d2\7C\2\2\u05d2\u05d3\7\177\2\2\u05d3"+
		"\u05d4\7\u0080\2\2\u05d4\u00d7\3\2\2\2\u05d5\u05d7\7{\2\2\u05d6\u05d8"+
		"\5\u00dan\2\u05d7\u05d6\3\2\2\2\u05d7\u05d8\3\2\2\2\u05d8\u05d9\3\2\2"+
		"\2\u05d9\u05da\7|\2\2\u05da\u00d9\3\2\2\2\u05db\u05dd\5\u00dco\2\u05dc"+
		"\u05db\3\2\2\2\u05dd\u05de\3\2\2\2\u05de\u05dc\3\2\2\2\u05de\u05df\3\2"+
		"\2\2\u05df\u00db\3\2\2\2\u05e0\u05e2\5\u013c\u009f\2\u05e1\u05e0\3\2\2"+
		"\2\u05e1\u05e2\3\2\2\2\u05e2\u05e4\3\2\2\2\u05e3\u05e5\5\u00dep\2\u05e4"+
		"\u05e3\3\2\2\2\u05e4\u05e5\3\2\2\2\u05e5\u05e8\3\2\2\2\u05e6\u05e9\5\u00e2"+
		"r\2\u05e7\u05e9\5\u0180\u00c1\2\u05e8\u05e6\3\2\2\2\u05e8\u05e7\3\2\2"+
		"\2\u05e9\u00dd\3\2\2\2\u05ea\u05ec\5\u00e0q\2\u05eb\u05ea\3\2\2\2\u05ec"+
		"\u05ed\3\2\2\2\u05ed\u05eb\3\2\2\2\u05ed\u05ee\3\2\2\2\u05ee\u00df\3\2"+
		"\2\2\u05ef\u05f0\t\13\2\2\u05f0\u00e1\3\2\2\2\u05f1\u0605\5\u017c\u00bf"+
		"\2\u05f2\u0605\5\u00e4s\2\u05f3\u0605\5\u0176\u00bc\2\u05f4\u05fa\5\u0112"+
		"\u008a\2\u05f5\u05fb\5\u0116\u008c\2\u05f6\u05f7\5\u0154\u00ab\2\u05f7"+
		"\u05f8\5\32\16\2\u05f8\u05f9\7\u0084\2\2\u05f9\u05fb\3\2\2\2\u05fa\u05f5"+
		"\3\2\2\2\u05fa\u05f6\3\2\2\2\u05fb\u0605\3\2\2\2\u05fc\u0605\5\u0182\u00c2"+
		"\2\u05fd\u05fe\7k\2\2\u05fe\u0605\5\u0184\u00c3\2\u05ff\u0605\5\u016c"+
		"\u00b7\2\u0600\u0605\5\u016e\u00b8\2\u0601\u0605\5\u0170\u00b9\2\u0602"+
		"\u0605\5\u0172\u00ba\2\u0603\u0605\5\u0174\u00bb\2\u0604\u05f1\3\2\2\2"+
		"\u0604\u05f2\3\2\2\2\u0604\u05f3\3\2\2\2\u0604\u05f4\3\2\2\2\u0604\u05fc"+
		"\3\2\2\2\u0604\u05fd\3\2\2\2\u0604\u05ff\3\2\2\2\u0604\u0600\3\2\2\2\u0604"+
		"\u0601\3\2\2\2\u0604\u0602\3\2\2\2\u0604\u0603\3\2\2\2\u0605\u00e3\3\2"+
		"\2\2\u0606\u0610\5\6\4\2\u0607\u0608\5\4\3\2\u0608\u0609\7\u0081\2\2\u0609"+
		"\u060a\5\u017e\u00c0\2\u060a\u0611\3\2\2\2\u060b\u0611\5\u0184\u00c3\2"+
		"\u060c\u0611\5\u017a\u00be\2\u060d\u0611\5\u017e\u00c0\2\u060e\u0611\5"+
		"\u0188\u00c5\2\u060f\u0611\5\u0178\u00bd\2\u0610\u0607\3\2\2\2\u0610\u060b"+
		"\3\2\2\2\u0610\u060c\3\2\2\2\u0610\u060d\3\2\2\2\u0610\u060e\3\2\2\2\u0610"+
		"\u060f\3\2\2\2\u0611\u00e5\3\2\2\2\u0612\u0617\5\u00e8u\2\u0613\u0614"+
		"\7\u0082\2\2\u0614\u0616\5\u00e8u\2\u0615\u0613\3\2\2\2\u0616\u0619\3"+
		"\2\2\2\u0617\u0615\3\2\2\2\u0617\u0618\3\2\2\2\u0618\u00e7\3\2\2\2\u0619"+
		"\u0617\3\2\2\2\u061a\u061b\5\u0190\u00c9\2\u061b\u061c\7\u008f\2\2\u061c"+
		"\u061d\5\32\16\2\u061d\u00e9\3\2\2\2\u061e\u0623\5\u00ecw\2\u061f\u0620"+
		"\7\u0082\2\2\u0620\u0622\5\u00ecw\2\u0621\u061f\3\2\2\2\u0622\u0625\3"+
		"\2\2\2\u0623\u0621\3\2\2\2\u0623\u0624\3\2\2\2\u0624\u00eb\3\2\2\2\u0625"+
		"\u0623\3\2\2\2\u0626\u0629\5\u0190\u00c9\2\u0627\u0628\7\u008f\2\2\u0628"+
		"\u062a\5\u00eex\2\u0629\u0627\3\2\2\2\u0629\u062a\3\2\2\2\u062a\u00ed"+
		"\3\2\2\2\u062b\u062e\5\32\16\2\u062c\u062e\5\u0122\u0092\2\u062d\u062b"+
		"\3\2\2\2\u062d\u062c\3\2\2\2\u062e\u00ef\3\2\2\2\u062f\u0632\5\6\4\2\u0630"+
		"\u0632\7k\2\2\u0631\u062f\3\2\2\2\u0631\u0630\3\2\2\2\u0632\u00f1\3\2"+
		"\2\2\u0633\u0634\5\4\3\2\u0634\u00f3\3\2\2\2\u0635\u0638\5\u008cG\2\u0636"+
		"\u0638\7\u0084\2\2\u0637\u0635\3\2\2\2\u0637\u0636\3\2\2\2\u0638\u00f5"+
		"\3\2\2\2\u0639\u0640\5\u00fe\u0080\2\u063a\u063d\5\u00f8}\2\u063b\u063c"+
		"\7\u0082\2\2\u063c\u063e\5\u00fe\u0080\2\u063d\u063b\3\2\2\2\u063d\u063e"+
		"\3\2\2\2\u063e\u0640\3\2\2\2\u063f\u0639\3\2\2\2\u063f\u063a\3\2\2\2\u0640"+
		"\u00f7\3\2\2\2\u0641\u0646\5\u00fa~\2\u0642\u0643\7\u0082\2\2\u0643\u0645"+
		"\5\u00fa~\2\u0644\u0642\3\2\2\2\u0645\u0648\3\2\2\2\u0646\u0644\3\2\2"+
		"\2\u0646\u0647\3\2\2\2\u0647\u00f9\3\2\2\2\u0648\u0646\3\2\2\2\u0649\u064b"+
		"\5\u013c\u009f\2\u064a\u0649\3\2\2\2\u064a\u064b\3\2\2\2\u064b\u064d\3"+
		"\2\2\2\u064c\u064e\5\u00fc\177\2\u064d\u064c\3\2\2\2\u064d\u064e\3\2\2"+
		"\2\u064e\u064f\3\2\2\2\u064f\u0652\5\u018a\u00c6\2\u0650\u0652\7\r\2\2"+
		"\u0651\u064a\3\2\2\2\u0651\u0650\3\2\2\2\u0652\u00fb\3\2\2\2\u0653\u0654"+
		"\t\f\2\2\u0654\u00fd\3\2\2\2\u0655\u0657\5\u013c\u009f\2\u0656\u0655\3"+
		"\2\2\2\u0656\u0657\3\2\2\2\u0657\u0658\3\2\2\2\u0658\u0659\7K\2\2\u0659"+
		"\u065a\5\u011e\u0090\2\u065a\u065b\5\u0190\u00c9\2\u065b\u00ff\3\2\2\2"+
		"\u065c\u065e\5\u013c\u009f\2\u065d\u065c\3\2\2\2\u065d\u065e\3\2\2\2\u065e"+
		"\u0660\3\2\2\2\u065f\u0661\5\u0106\u0084\2\u0660\u065f\3\2\2\2\u0660\u0661"+
		"\3\2\2\2\u0661\u066c\3\2\2\2\u0662\u0663\7\62\2\2\u0663\u0665\5\u0108"+
		"\u0085\2\u0664\u0666\5\u0104\u0083\2\u0665\u0664\3\2\2\2\u0665\u0666\3"+
		"\2\2\2\u0666\u066d\3\2\2\2\u0667\u0668\7W\2\2\u0668\u066a\5\u0108\u0085"+
		"\2\u0669\u066b\5\u0102\u0082\2\u066a\u0669\3\2\2\2\u066a\u066b\3\2\2\2"+
		"\u066b\u066d\3\2\2\2\u066c\u0662\3\2\2\2\u066c\u0667\3\2\2\2\u066d\u0101"+
		"\3\2\2\2\u066e\u0670\5\u013c\u009f\2\u066f\u066e\3\2\2\2\u066f\u0670\3"+
		"\2\2\2\u0670\u0672\3\2\2\2\u0671\u0673\5\u0106\u0084\2\u0672\u0671\3\2"+
		"\2\2\u0672\u0673\3\2\2\2\u0673\u0674\3\2\2\2\u0674\u0675\7\62\2\2\u0675"+
		"\u0676\5\u0108\u0085\2\u0676\u0103\3\2\2\2\u0677\u0679\5\u013c\u009f\2"+
		"\u0678\u0677\3\2\2\2\u0678\u0679\3\2\2\2\u0679\u067b\3\2\2\2\u067a\u067c"+
		"\5\u0106\u0084\2\u067b\u067a\3\2\2\2\u067b\u067c\3\2\2\2\u067c\u067d\3"+
		"\2\2\2\u067d\u067e\7W\2\2\u067e\u067f\5\u0108\u0085\2\u067f\u0105\3\2"+
		"\2\2\u0680\u0688\7N\2\2\u0681\u0688\7:\2\2\u0682\u0688\7M\2\2\u0683\u0684"+
		"\7N\2\2\u0684\u0688\7:\2\2\u0685\u0686\7:\2\2\u0686\u0688\7N\2\2\u0687"+
		"\u0680\3\2\2\2\u0687\u0681\3\2\2\2\u0687\u0682\3\2\2\2\u0687\u0683\3\2"+
		"\2\2\u0687\u0685\3\2\2\2\u0688\u0107\3\2\2\2\u0689\u068c\5\u008cG\2\u068a"+
		"\u068c\7\u0084\2\2\u068b\u0689\3\2\2\2\u068b\u068a\3\2\2\2\u068c\u0109"+
		"\3\2\2\2\u068d\u068f\5\u013c\u009f\2\u068e\u068d\3\2\2\2\u068e\u068f\3"+
		"\2\2\2\u068f\u0698\3\2\2\2\u0690\u0691\7\13\2\2\u0691\u0692\5\u008cG\2"+
		"\u0692\u0693\5\u010e\u0088\2\u0693\u0699\3\2\2\2\u0694\u0695\7R\2\2\u0695"+
		"\u0696\5\u008cG\2\u0696\u0697\5\u010c\u0087\2\u0697\u0699\3\2\2\2\u0698"+
		"\u0690\3\2\2\2\u0698\u0694\3\2\2\2\u0699\u010b\3\2\2\2\u069a\u069c\5\u013c"+
		"\u009f\2\u069b\u069a\3\2\2\2\u069b\u069c\3\2\2\2\u069c\u069d\3\2\2\2\u069d"+
		"\u069e\7\13\2\2\u069e\u069f\5\u008cG\2\u069f\u010d\3\2\2\2\u06a0\u06a2"+
		"\5\u013c\u009f\2\u06a1\u06a0\3\2\2\2\u06a1\u06a2\3\2\2\2\u06a2\u06a3\3"+
		"\2\2\2\u06a3\u06a4\7R\2\2\u06a4\u06a5\5\u008cG\2\u06a5\u010f\3\2\2\2\u06a6"+
		"\u06bd\7\u0085\2\2\u06a7\u06bd\7\u0086\2\2\u06a8\u06bd\7\u008d\2\2\u06a9"+
		"\u06bd\7\u008e\2\2\u06aa\u06bd\7\u0095\2\2\u06ab\u06bd\7\u0096\2\2\u06ac"+
		"\u06bd\7a\2\2\u06ad\u06bd\7+\2\2\u06ae\u06bd\7\u0087\2\2\u06af\u06bd\7"+
		"\u0088\2\2\u06b0\u06bd\7\u0089\2\2\u06b1\u06bd\7\u008a\2\2\u06b2\u06bd"+
		"\7\u008b\2\2\u06b3\u06bd\7\u008c\2\2\u06b4\u06bd\7\u00a6\2\2\u06b5\u06bd"+
		"\5\u0156\u00ac\2\u06b6\u06bd\7\u009a\2\2\u06b7\u06bd\7\u009b\2\2\u06b8"+
		"\u06bd\7\u0091\2\2\u06b9\u06bd\7\u0090\2\2\u06ba\u06bd\7\u009d\2\2\u06bb"+
		"\u06bd\7\u009c\2\2\u06bc\u06a6\3\2\2\2\u06bc\u06a7\3\2\2\2\u06bc\u06a8"+
		"\3\2\2\2\u06bc\u06a9\3\2\2\2\u06bc\u06aa\3\2\2\2\u06bc\u06ab\3\2\2\2\u06bc"+
		"\u06ac\3\2\2\2\u06bc\u06ad\3\2\2\2\u06bc\u06ae\3\2\2\2\u06bc\u06af\3\2"+
		"\2\2\u06bc\u06b0\3\2\2\2\u06bc\u06b1\3\2\2\2\u06bc\u06b2\3\2\2\2\u06bc"+
		"\u06b3\3\2\2\2\u06bc\u06b4\3\2\2\2\u06bc\u06b5\3\2\2\2\u06bc\u06b6\3\2"+
		"\2\2\u06bc\u06b7\3\2\2\2\u06bc\u06b8\3\2\2\2\u06bc\u06b9\3\2\2\2\u06bc"+
		"\u06ba\3\2\2\2\u06bc\u06bb\3\2\2\2\u06bd\u0111\3\2\2\2\u06be\u06bf\t\r"+
		"\2\2\u06bf\u06c0\7G\2\2\u06c0\u06c1\5\6\4\2\u06c1\u06c2\7\177\2\2\u06c2"+
		"\u06c3\5\u018a\u00c6\2\u06c3\u06c4\7\u0080\2\2\u06c4\u0113\3\2\2\2\u06c5"+
		"\u06c6\7\u0083\2\2\u06c6\u06c7\t\16\2\2\u06c7\u06c9\7\177\2\2\u06c8\u06ca"+
		"\5\26\f\2\u06c9\u06c8\3\2\2\2\u06c9\u06ca\3\2\2\2\u06ca\u06cb\3\2\2\2"+
		"\u06cb\u06cc\7\u0080\2\2\u06cc\u0115\3\2\2\2\u06cd\u06d0\5\u008cG\2\u06ce"+
		"\u06d0\7\u0084\2\2\u06cf\u06cd\3\2\2\2\u06cf\u06ce\3\2\2\2\u06d0\u0117"+
		"\3\2\2\2\u06d1\u06d2\7\u0083\2\2\u06d2\u06d3\5\u00caf\2\u06d3\u0119\3"+
		"\2\2\2\u06d4\u06d8\7{\2\2\u06d5\u06d7\5\u011c\u008f\2\u06d6\u06d5\3\2"+
		"\2\2\u06d7\u06da\3\2\2\2\u06d8\u06d6\3\2\2\2\u06d8\u06d9\3\2\2\2\u06d9"+
		"\u06db\3\2\2\2\u06da\u06d8\3\2\2\2\u06db\u06dc\7|\2\2\u06dc\u011b\3\2"+
		"\2\2\u06dd\u06df\5\u013c\u009f\2\u06de\u06dd\3\2\2\2\u06de\u06df\3\2\2"+
		"\2\u06df\u06e1\3\2\2\2\u06e0\u06e2\5\u00dep\2\u06e1\u06e0\3\2\2\2\u06e1"+
		"\u06e2\3\2\2\2\u06e2\u06ed\3\2\2\2\u06e3\u06ee\5\u00e2r\2\u06e4\u06e5"+
		"\7-\2\2\u06e5\u06e7\5\6\4\2\u06e6\u06e8\5\u0150\u00a9\2\u06e7\u06e6\3"+
		"\2\2\2\u06e8\u06e9\3\2\2\2\u06e9\u06e7\3\2\2\2\u06e9\u06ea\3\2\2\2\u06ea"+
		"\u06eb\3\2\2\2\u06eb\u06ec\7\u0084\2\2\u06ec\u06ee\3\2\2\2\u06ed\u06e3"+
		"\3\2\2\2\u06ed\u06e4\3\2\2\2\u06ee\u011d\3\2\2\2\u06ef\u06f7\5\b\5\2\u06f0"+
		"\u06f2\t\17\2\2\u06f1\u06f0\3\2\2\2\u06f2\u06f5\3\2\2\2\u06f3\u06f1\3"+
		"\2\2\2\u06f3\u06f4\3\2\2\2\u06f4\u06f6\3\2\2\2\u06f5\u06f3\3\2\2\2\u06f6"+
		"\u06f8\5\u0120\u0091\2\u06f7\u06f3\3\2\2\2\u06f8\u06f9\3\2\2\2\u06f9\u06f7"+
		"\3\2\2\2\u06f9\u06fa\3\2\2\2\u06fa\u011f\3\2\2\2\u06fb\u06ff\7}\2\2\u06fc"+
		"\u06fe\7\u0082\2\2\u06fd\u06fc\3\2\2\2\u06fe\u0701\3\2\2\2\u06ff\u06fd"+
		"\3\2\2\2\u06ff\u0700\3\2\2\2\u0700\u0702\3\2\2\2\u0701\u06ff\3\2\2\2\u0702"+
		"\u0703\7~\2\2\u0703\u0121\3\2\2\2\u0704\u0710\7{\2\2\u0705\u070a\5\u00ee"+
		"x\2\u0706\u0707\7\u0082\2\2\u0707\u0709\5\u00eex\2\u0708\u0706\3\2\2\2"+
		"\u0709\u070c\3\2\2\2\u070a\u0708\3\2\2\2\u070a\u070b\3\2\2\2\u070b\u070e"+
		"\3\2\2\2\u070c\u070a\3\2\2\2\u070d\u070f\7\u0082\2\2\u070e\u070d\3\2\2"+
		"\2\u070e\u070f\3\2\2\2\u070f\u0711\3\2\2\2\u0710\u0705\3\2\2\2\u0710\u0711"+
		"\3\2\2\2\u0711\u0712\3\2\2\2\u0712\u0713\7|\2\2\u0713\u0123\3\2\2\2\u0714"+
		"\u0715\7\u0090\2\2\u0715\u071a\5\u0126\u0094\2\u0716\u0717\7\u0082\2\2"+
		"\u0717\u0719\5\u0126\u0094\2\u0718\u0716\3\2\2\2\u0719\u071c\3\2\2\2\u071a"+
		"\u0718\3\2\2\2\u071a\u071b\3\2\2\2\u071b\u071d\3\2\2\2\u071c\u071a\3\2"+
		"\2\2\u071d\u071e\7\u0091\2\2\u071e\u0125\3\2\2\2\u071f\u0721\5\u013c\u009f"+
		"\2\u0720\u071f\3\2\2\2\u0720\u0721\3\2\2\2\u0721\u0723\3\2\2\2\u0722\u0724"+
		"\5\u0128\u0095\2\u0723\u0722\3\2\2\2\u0723\u0724\3\2\2\2\u0724\u0725\3"+
		"\2\2\2\u0725\u0726\5\u0190\u00c9\2\u0726\u0127\3\2\2\2\u0727\u0728\t\20"+
		"\2\2\u0728\u0129\3\2\2\2\u0729\u072a\7\u0083\2\2\u072a\u072b\5\u00caf"+
		"\2\u072b\u012b\3\2\2\2\u072c\u0730\7{\2\2\u072d\u072f\5\u012e\u0098\2"+
		"\u072e\u072d\3\2\2\2\u072f\u0732\3\2\2\2\u0730\u072e\3\2\2\2\u0730\u0731"+
		"\3\2\2\2\u0731\u0733\3\2\2\2\u0732\u0730\3\2\2\2\u0733\u0734\7|\2\2\u0734"+
		"\u012d\3\2\2\2\u0735\u0737\5\u013c\u009f\2\u0736\u0735\3\2\2\2\u0736\u0737"+
		"\3\2\2\2\u0737\u0739\3\2\2\2\u0738\u073a\7C\2\2\u0739\u0738\3\2\2\2\u0739"+
		"\u073a\3\2\2\2\u073a\u0773\3\2\2\2\u073b\u073d\7g\2\2\u073c\u073b\3\2"+
		"\2\2\u073c\u073d\3\2\2\2\u073d\u073e\3\2\2\2\u073e\u075a\5\6\4\2\u073f"+
		"\u0741\5\u0190\u00c9\2\u0740\u0742\5\u00c4c\2\u0741\u0740\3\2\2\2\u0741"+
		"\u0742\3\2\2\2\u0742\u0743\3\2\2\2\u0743\u0745\7\177\2\2\u0744\u0746\5"+
		"\u00f6|\2\u0745\u0744\3\2\2\2\u0745\u0746\3\2\2\2\u0746\u0747\3\2\2\2"+
		"\u0747\u0749\7\u0080\2\2\u0748\u074a\5\u00ccg\2\u0749\u0748\3\2\2\2\u0749"+
		"\u074a\3\2\2\2\u074a\u074b\3\2\2\2\u074b\u074c\7\u0084\2\2\u074c\u075b"+
		"\3\2\2\2\u074d\u074e\5\u0190\u00c9\2\u074e\u074f\7{\2\2\u074f\u0750\5"+
		"\u0130\u0099\2\u0750\u0751\7|\2\2\u0751\u075b\3\2\2\2\u0752\u0753\7_\2"+
		"\2\u0753\u0754\7}\2\2\u0754\u0755\5\u00f6|\2\u0755\u0756\7~\2\2\u0756"+
		"\u0757\7{\2\2\u0757\u0758\5\u0130\u0099\2\u0758\u0759\7|\2\2\u0759\u075b"+
		"\3\2\2\2\u075a\u073f\3\2\2\2\u075a\u074d\3\2\2\2\u075a\u0752\3\2\2\2\u075b"+
		"\u0774\3\2\2\2\u075c\u075e\7g\2\2\u075d\u075c\3\2\2\2\u075d\u075e\3\2"+
		"\2\2\u075e\u075f\3\2\2\2\u075f\u0760\7k\2\2\u0760\u0762\5\u0190\u00c9"+
		"\2\u0761\u0763\5\u00c4c\2\u0762\u0761\3\2\2\2\u0762\u0763\3\2\2\2\u0763"+
		"\u0764\3\2\2\2\u0764\u0766\7\177\2\2\u0765\u0767\5\u00f6|\2\u0766\u0765"+
		"\3\2\2\2\u0766\u0767\3\2\2\2\u0767\u0768\3\2\2\2\u0768\u076a\7\u0080\2"+
		"\2\u0769\u076b\5\u00ccg\2\u076a\u0769\3\2\2\2\u076a\u076b\3\2\2\2\u076b"+
		"\u076c\3\2\2\2\u076c\u076d\7\u0084\2\2\u076d\u0774\3\2\2\2\u076e\u076f"+
		"\7(\2\2\u076f\u0770\5\6\4\2\u0770\u0771\5\u0190\u00c9\2\u0771\u0772\7"+
		"\u0084\2\2\u0772\u0774\3\2\2\2\u0773\u073c\3\2\2\2\u0773\u075d\3\2\2\2"+
		"\u0773\u076e\3\2\2\2\u0774\u012f\3\2\2\2\u0775\u0777\5\u013c\u009f\2\u0776"+
		"\u0775\3\2\2\2\u0776\u0777\3\2\2\2\u0777\u078a\3\2\2\2\u0778\u0779\7\62"+
		"\2\2\u0779\u077f\7\u0084\2\2\u077a\u077c\5\u013c\u009f\2\u077b\u077a\3"+
		"\2\2\2\u077b\u077c\3\2\2\2\u077c\u077d\3\2\2\2\u077d\u077e\7W\2\2\u077e"+
		"\u0780\7\u0084\2\2\u077f\u077b\3\2\2\2\u077f\u0780\3\2\2\2\u0780\u078b"+
		"\3\2\2\2\u0781\u0782\7W\2\2\u0782\u0788\7\u0084\2\2\u0783\u0785\5\u013c"+
		"\u009f\2\u0784\u0783\3\2\2\2\u0784\u0785\3\2\2\2\u0785\u0786\3\2\2\2\u0786"+
		"\u0787\7\62\2\2\u0787\u0789\7\u0084\2\2\u0788\u0784\3\2\2\2\u0788\u0789"+
		"\3\2\2\2\u0789\u078b\3\2\2\2\u078a\u0778\3\2\2\2\u078a\u0781\3\2\2\2\u078b"+
		"\u0131\3\2\2\2\u078c\u078d\7\u0083\2\2\u078d\u078e\5\6\4\2\u078e\u0133"+
		"\3\2\2\2\u078f\u079b\7{\2\2\u0790\u0795\5\u0136\u009c\2\u0791\u0792\7"+
		"\u0082\2\2\u0792\u0794\5\u0136\u009c\2\u0793\u0791\3\2\2\2\u0794\u0797"+
		"\3\2\2\2\u0795\u0793\3\2\2\2\u0795\u0796\3\2\2\2\u0796\u0799\3\2\2\2\u0797"+
		"\u0795\3\2\2\2\u0798\u079a\7\u0082\2\2\u0799\u0798\3\2\2\2\u0799\u079a"+
		"\3\2\2\2\u079a\u079c\3\2\2\2\u079b\u0790\3\2\2\2\u079b\u079c\3\2\2\2\u079c"+
		"\u079d\3\2\2\2\u079d\u079e\7|\2\2\u079e\u0135\3\2\2\2\u079f\u07a1\5\u013c"+
		"\u009f\2\u07a0\u079f\3\2\2\2\u07a0\u07a1\3\2\2\2\u07a1\u07a2\3\2\2\2\u07a2"+
		"\u07a5\5\u0190\u00c9\2\u07a3\u07a4\7\u008f\2\2\u07a4\u07a6\5\32\16\2\u07a5"+
		"\u07a3\3\2\2\2\u07a5\u07a6\3\2\2\2\u07a6\u0137\3\2\2\2\u07a7\u07a8\7}"+
		"\2\2\u07a8\u07a9\5\u013a\u009e\2\u07a9\u07aa\7\u0083\2\2\u07aa\u07ac\5"+
		"\u0142\u00a2\2\u07ab\u07ad\7\u0082\2\2\u07ac\u07ab\3\2\2\2\u07ac\u07ad"+
		"\3\2\2\2\u07ad\u07ae\3\2\2\2\u07ae\u07af\7~\2\2\u07af\u0139\3\2\2\2\u07b0"+
		"\u07b3\5\u016a\u00b6\2\u07b1\u07b3\5\u0190\u00c9\2\u07b2\u07b0\3\2\2\2"+
		"\u07b2\u07b1\3\2\2\2\u07b3\u013b\3\2\2\2\u07b4\u07b6\5\u013e\u00a0\2\u07b5"+
		"\u07b4\3\2\2\2\u07b6\u07b7\3\2\2\2\u07b7\u07b5\3\2\2\2\u07b7\u07b8\3\2"+
		"\2\2\u07b8\u013d\3\2\2\2\u07b9\u07bd\7}\2\2\u07ba\u07bb\5\u0140\u00a1"+
		"\2\u07bb\u07bc\7\u0083\2\2\u07bc\u07be\3\2\2\2\u07bd\u07ba\3\2\2\2\u07bd"+
		"\u07be\3\2\2\2\u07be\u07bf\3\2\2\2\u07bf\u07c1\5\u0142\u00a2\2\u07c0\u07c2"+
		"\7\u0082\2\2\u07c1\u07c0\3\2\2\2\u07c1\u07c2\3\2\2\2\u07c2\u07c3\3\2\2"+
		"\2\u07c3\u07c4\7~\2\2\u07c4\u013f\3\2\2\2\u07c5\u07c8\5\u016a\u00b6\2"+
		"\u07c6\u07c8\5\u0190\u00c9\2\u07c7\u07c5\3\2\2\2\u07c7\u07c6\3\2\2\2\u07c8"+
		"\u0141\3\2\2\2\u07c9\u07ce\5\u0144\u00a3\2\u07ca\u07cb\7\u0082\2\2\u07cb"+
		"\u07cd\5\u0144\u00a3\2\u07cc\u07ca\3\2\2\2\u07cd\u07d0\3\2\2\2\u07ce\u07cc"+
		"\3\2\2\2\u07ce\u07cf\3\2\2\2\u07cf\u0143\3\2\2\2\u07d0\u07ce\3\2\2\2\u07d1"+
		"\u07de\5\4\3\2\u07d2\u07db\7\177\2\2\u07d3\u07d8\5\u0146\u00a4\2\u07d4"+
		"\u07d5\7\u0082\2\2\u07d5\u07d7\5\u0146\u00a4\2\u07d6\u07d4\3\2\2\2\u07d7"+
		"\u07da\3\2\2\2\u07d8\u07d6\3\2\2\2\u07d8\u07d9\3\2\2\2\u07d9\u07dc\3\2"+
		"\2\2\u07da\u07d8\3\2\2\2\u07db\u07d3\3\2\2\2\u07db\u07dc\3\2\2\2\u07dc"+
		"\u07dd\3\2\2\2\u07dd\u07df\7\u0080\2\2\u07de\u07d2\3\2\2\2\u07de\u07df"+
		"\3\2\2\2\u07df\u0145\3\2\2\2\u07e0\u07e1\5\u0190\u00c9\2\u07e1\u07e2\7"+
		"\u0083\2\2\u07e2\u07e4\3\2\2\2\u07e3\u07e0\3\2\2\2\u07e3\u07e4\3\2\2\2"+
		"\u07e4\u07e5\3\2\2\2\u07e5\u07e6\5\32\16\2\u07e6\u0147\3\2\2\2\u07e7\u07ea"+
		"\5\n\6\2\u07e8\u07ea\5\22\n\2\u07e9\u07e7\3\2\2\2\u07e9\u07e8\3\2\2\2"+
		"\u07ea\u07ef\3\2\2\2\u07eb\u07ee\5\u0120\u0091\2\u07ec\u07ee\7\u0092\2"+
		"\2\u07ed\u07eb\3\2\2\2\u07ed\u07ec\3\2\2\2\u07ee\u07f1\3\2\2\2\u07ef\u07ed"+
		"\3\2\2\2\u07ef\u07f0\3\2\2\2\u07f0\u07f2\3\2\2\2\u07f1\u07ef\3\2\2\2\u07f2"+
		"\u07f3\7\u0087\2\2\u07f3\u07f7\3\2\2\2\u07f4\u07f5\7k\2\2\u07f5\u07f7"+
		"\7\u0087\2\2\u07f6\u07e9\3\2\2\2\u07f6\u07f4\3\2\2\2\u07f7\u0149\3\2\2"+
		"\2\u07f8\u07fd\5\u014c\u00a7\2\u07f9\u07fa\7\u0082\2\2\u07fa\u07fc\5\u014c"+
		"\u00a7\2\u07fb\u07f9\3\2\2\2\u07fc\u07ff\3\2\2\2\u07fd\u07fb\3\2\2\2\u07fd"+
		"\u07fe\3\2\2\2\u07fe\u014b\3\2\2\2\u07ff\u07fd\3\2\2\2\u0800\u0801\5\u0190"+
		"\u00c9\2\u0801\u0802\7\u008f\2\2\u0802\u0803\5\u014e\u00a8\2\u0803\u014d"+
		"\3\2\2\2\u0804\u0806\7\u008a\2\2\u0805\u0804\3\2\2\2\u0805\u0806\3\2\2"+
		"\2\u0806\u0807\3\2\2\2\u0807\u080a\5\32\16\2\u0808\u080a\5\u0152\u00aa"+
		"\2\u0809\u0805\3\2\2\2\u0809\u0808\3\2\2\2\u080a\u014f\3\2\2\2\u080b\u080c"+
		"\5\u0190\u00c9\2\u080c\u080d\7}\2\2\u080d\u080e\5\32\16\2\u080e\u080f"+
		"\7~\2\2\u080f\u0151\3\2\2\2\u0810\u0811\7Z\2\2\u0811\u0812\5\6\4\2\u0812"+
		"\u0813\7}\2\2\u0813\u0814\5\32\16\2\u0814\u0815\7~\2\2\u0815\u0153\3\2"+
		"\2\2\u0816\u0817\7\u008f\2\2\u0817\u0818\7\u0091\2\2\u0818\u0819\6\u00ab"+
		"\2\3\u0819\u0155\3\2\2\2\u081a\u081b\7\u0091\2\2\u081b\u081c\7\u0091\2"+
		"\2\u081c\u081d\6\u00ac\3\3\u081d\u0157\3\2\2\2\u081e\u081f\7\u0091\2\2"+
		"\u081f\u0820\7\u009d\2\2\u0820\u0821\6\u00ad\4\3\u0821\u0159\3\2\2\2\u0822"+
		"\u082a\5\u015c\u00af\2\u0823\u082a\5\u015e\u00b0\2\u0824\u082a\7s\2\2"+
		"\u0825\u082a\7t\2\2\u0826\u082a\7u\2\2\u0827\u082a\7v\2\2\u0828\u082a"+
		"\7D\2\2\u0829\u0822\3\2\2\2\u0829\u0823\3\2\2\2\u0829\u0824\3\2\2\2\u0829"+
		"\u0825\3\2\2\2\u0829\u0826\3\2\2\2\u0829\u0827\3\2\2\2\u0829\u0828\3\2"+
		"\2\2\u082a\u015b\3\2\2\2\u082b\u082c\t\21\2\2\u082c\u015d\3\2\2\2\u082d"+
		"\u0832\5\u0160\u00b1\2\u082e\u0832\5\u0162\u00b2\2\u082f\u0832\7w\2\2"+
		"\u0830\u0832\7x\2\2\u0831\u082d\3\2\2\2\u0831\u082e\3\2\2\2\u0831\u082f"+
		"\3\2\2\2\u0831\u0830\3\2\2\2\u0832\u015f\3\2\2\2\u0833\u0837\7y\2\2\u0834"+
		"\u0836\5\u0164\u00b3\2\u0835\u0834\3\2\2\2\u0836\u0839\3\2\2\2\u0837\u0835"+
		"\3\2\2\2\u0837\u0838\3\2\2\2\u0838\u083a\3\2\2\2\u0839\u0837\3\2\2\2\u083a"+
		"\u083b\7\u00ac\2\2\u083b\u0161\3\2\2\2\u083c\u0840\7z\2\2\u083d\u083f"+
		"\5\u0166\u00b4\2\u083e\u083d\3\2\2\2\u083f\u0842\3\2\2\2\u0840\u083e\3"+
		"\2\2\2\u0840\u0841\3\2\2\2\u0841\u0843\3\2\2\2\u0842\u0840\3\2\2\2\u0843"+
		"\u0844\7\u00ac\2\2\u0844\u0163\3\2\2\2\u0845\u084a\5\u0168\u00b5\2\u0846"+
		"\u084a\7\u00a8\2\2\u0847\u084a\7\u00aa\2\2\u0848\u084a\7\u00ad\2\2\u0849"+
		"\u0845\3\2\2\2\u0849\u0846\3\2\2\2\u0849\u0847\3\2\2\2\u0849\u0848\3\2"+
		"\2\2\u084a\u0165\3\2\2\2\u084b\u0850\5\u0168\u00b5\2\u084c\u0850\7\u00a8"+
		"\2\2\u084d\u0850\7\u00ab\2\2\u084e\u0850\7\u00ae\2\2\u084f\u084b\3\2\2"+
		"\2\u084f\u084c\3\2\2\2\u084f\u084d\3\2\2\2\u084f\u084e\3\2\2\2\u0850\u0167"+
		"\3\2\2\2\u0851\u0856\5\32\16\2\u0852\u0853\7\u0082\2\2\u0853\u0855\5\32"+
		"\16\2\u0854\u0852\3\2\2\2\u0855\u0858\3\2\2\2\u0856\u0854\3\2\2\2\u0856"+
		"\u0857\3\2\2\2\u0857\u085f\3\2\2\2\u0858\u0856\3\2\2\2\u0859\u085b\7\u0083"+
		"\2\2\u085a\u085c\7\u00b0\2\2\u085b\u085a\3\2\2\2\u085c\u085d\3\2\2\2\u085d"+
		"\u085b\3\2\2\2\u085d\u085e\3\2\2\2\u085e\u0860\3\2\2\2\u085f\u0859\3\2"+
		"\2\2\u085f\u0860\3\2\2\2\u0860\u0169\3\2\2\2\u0861\u0862\t\22\2\2\u0862"+
		"\u016b\3\2\2\2\u0863\u0864\7\33\2\2\u0864\u0866\5\u0190\u00c9\2\u0865"+
		"\u0867\5\u00c4c\2\u0866\u0865\3\2\2\2\u0866\u0867\3\2\2\2\u0867\u0869"+
		"\3\2\2\2\u0868\u086a\5\u00c8e\2\u0869\u0868\3\2\2\2\u0869\u086a\3\2\2"+
		"\2\u086a\u086c\3\2\2\2\u086b\u086d\5\u00ccg\2\u086c\u086b\3\2\2\2\u086c"+
		"\u086d\3\2\2\2\u086d\u086e\3\2\2\2\u086e\u0870\5\u00d8m\2\u086f\u0871"+
		"\7\u0084\2\2\u0870\u086f\3\2\2\2\u0870\u0871\3\2\2\2\u0871\u016d\3\2\2"+
		"\2\u0872\u0873\7]\2\2\u0873\u0875\5\u0190\u00c9\2\u0874\u0876\5\u00c4"+
		"c\2\u0875\u0874\3\2\2\2\u0875\u0876\3\2\2\2\u0876\u0878\3\2\2\2\u0877"+
		"\u0879\5\u0118\u008d\2\u0878\u0877\3\2\2\2\u0878\u0879\3\2\2\2\u0879\u087b"+
		"\3\2\2\2\u087a\u087c\5\u00ccg\2\u087b\u087a\3\2\2\2\u087b\u087c\3\2\2"+
		"\2\u087c\u087d\3\2\2\2\u087d\u087f\5\u011a\u008e\2\u087e\u0880\7\u0084"+
		"\2\2\u087f\u087e\3\2\2\2\u087f\u0880\3\2\2\2\u0880\u016f\3\2\2\2\u0881"+
		"\u0882\79\2\2\u0882\u0884\5\u0190\u00c9\2\u0883\u0885\5\u0124\u0093\2"+
		"\u0884\u0883\3\2\2\2\u0884\u0885\3\2\2\2\u0885\u0887\3\2\2\2\u0886\u0888"+
		"\5\u012a\u0096\2\u0887\u0886\3\2\2\2\u0887\u0888\3\2\2\2\u0888\u088a\3"+
		"\2\2\2\u0889\u088b\5\u00ccg\2\u088a\u0889\3\2\2\2\u088a\u088b\3\2\2\2"+
		"\u088b\u088c\3\2\2\2\u088c\u088e\5\u012c\u0097\2\u088d\u088f\7\u0084\2"+
		"\2\u088e\u088d\3\2\2\2\u088e\u088f\3\2\2\2\u088f\u0171\3\2\2\2\u0890\u0891"+
		"\7&\2\2\u0891\u0893\5\u0190\u00c9\2\u0892\u0894\5\u0132\u009a\2\u0893"+
		"\u0892\3\2\2\2\u0893\u0894\3\2\2\2\u0894\u0895\3\2\2\2\u0895\u0897\5\u0134"+
		"\u009b\2\u0896\u0898\7\u0084\2\2\u0897\u0896\3\2\2\2\u0897\u0898\3\2\2"+
		"\2\u0898\u0173\3\2\2\2\u0899\u089a\7 \2\2\u089a\u089b\5\u00f0y\2\u089b"+
		"\u089d\5\u0190\u00c9\2\u089c\u089e\5\u0124\u0093\2\u089d\u089c\3\2\2\2"+
		"\u089d\u089e\3\2\2\2\u089e\u089f\3\2\2\2\u089f\u08a1\7\177\2\2\u08a0\u08a2"+
		"\5\u00f6|\2\u08a1\u08a0\3\2\2\2\u08a1\u08a2\3\2\2\2\u08a2\u08a3\3\2\2"+
		"\2\u08a3\u08a5\7\u0080\2\2\u08a4\u08a6\5\u00ccg\2\u08a5\u08a4\3\2\2\2"+
		"\u08a5\u08a6\3\2\2\2\u08a6\u08a7\3\2\2\2\u08a7\u08a8\7\u0084\2\2\u08a8"+
		"\u0175\3\2\2\2\u08a9\u08aa\7(\2\2\u08aa\u08b3\5\6\4\2\u08ab\u08ac\5\u00ea"+
		"v\2\u08ac\u08ad\7\u0084\2\2\u08ad\u08b4\3\2\2\2\u08ae\u08af\5\u00f2z\2"+
		"\u08af\u08b0\7{\2\2\u08b0\u08b1\5\u010a\u0086\2\u08b1\u08b2\7|\2\2\u08b2"+
		"\u08b4\3\2\2\2\u08b3\u08ab\3\2\2\2\u08b3\u08ae\3\2\2\2\u08b4\u0177\3\2"+
		"\2\2\u08b5\u08b6\5\u00eav\2\u08b6\u08b7\7\u0084\2\2\u08b7\u0179\3\2\2"+
		"\2\u08b8\u08c6\5\u00f2z\2\u08b9\u08ba\7{\2\2\u08ba\u08bb\5\u0100\u0081"+
		"\2\u08bb\u08c0\7|\2\2\u08bc\u08bd\7\u008f\2\2\u08bd\u08be\5\u00eex\2\u08be"+
		"\u08bf\7\u0084\2\2\u08bf\u08c1\3\2\2\2\u08c0\u08bc\3\2\2\2\u08c0\u08c1"+
		"\3\2\2\2\u08c1\u08c7\3\2\2\2\u08c2\u08c3\5\u0154\u00ab\2\u08c3\u08c4\5"+
		"\32\16\2\u08c4\u08c5\7\u0084\2\2\u08c5\u08c7\3\2\2\2\u08c6\u08b9\3\2\2"+
		"\2\u08c6\u08c2\3\2\2\2\u08c7\u017b\3\2\2\2\u08c8\u08c9\7\34\2\2\u08c9"+
		"\u08ca\5\6\4\2\u08ca\u08cb\5\u00e6t\2\u08cb\u08cc\7\u0084\2\2\u08cc\u017d"+
		"\3\2\2\2\u08cd\u08ce\7_\2\2\u08ce\u08cf\7}\2\2\u08cf\u08d0\5\u00f6|\2"+
		"\u08d0\u08d9\7~\2\2\u08d1\u08d2\7{\2\2\u08d2\u08d3\5\u0100\u0081\2\u08d3"+
		"\u08d4\7|\2\2\u08d4\u08da\3\2\2\2\u08d5\u08d6\5\u0154\u00ab\2\u08d6\u08d7"+
		"\5\32\16\2\u08d7\u08d8\7\u0084\2\2\u08d8\u08da\3\2\2\2\u08d9\u08d1\3\2"+
		"\2\2\u08d9\u08d5\3\2\2\2\u08da\u017f\3\2\2\2\u08db\u08dc\7\u008e\2\2\u08dc"+
		"\u08dd\5\u0190\u00c9\2\u08dd\u08de\7\177\2\2\u08de\u08df\7\u0080\2\2\u08df"+
		"\u08e0\5\u0116\u008c\2\u08e0\u0181\3\2\2\2\u08e1\u08e2\5\u0190\u00c9\2"+
		"\u08e2\u08e4\7\177\2\2\u08e3\u08e5\5\u00f6|\2\u08e4\u08e3\3\2\2\2\u08e4"+
		"\u08e5\3\2\2\2\u08e5\u08e6\3\2\2\2\u08e6\u08e8\7\u0080\2\2\u08e7\u08e9"+
		"\5\u0114\u008b\2\u08e8\u08e7\3\2\2\2\u08e8\u08e9\3\2\2\2\u08e9\u08ea\3"+
		"\2\2\2\u08ea\u08eb\5\u0116\u008c\2\u08eb\u0183\3\2\2\2\u08ec\u08ee\5\u0186"+
		"\u00c4\2\u08ed\u08ef\5\u00c4c\2\u08ee\u08ed\3\2\2\2\u08ee\u08ef\3\2\2"+
		"\2\u08ef\u08f0\3\2\2\2\u08f0\u08f2\7\177\2\2\u08f1\u08f3\5\u00f6|\2\u08f2"+
		"\u08f1\3\2\2\2\u08f2\u08f3\3\2\2\2\u08f3\u08f4\3\2\2\2\u08f4\u08f6\7\u0080"+
		"\2\2\u08f5\u08f7\5\u00ccg\2\u08f6\u08f5\3\2\2\2\u08f6\u08f7\3\2\2\2\u08f7"+
		"\u08fd\3\2\2\2\u08f8\u08fe\5\u00f4{\2\u08f9\u08fa\5\u0154\u00ab\2\u08fa"+
		"\u08fb\5\32\16\2\u08fb\u08fc\7\u0084\2\2\u08fc\u08fe\3\2\2\2\u08fd\u08f8"+
		"\3\2\2\2\u08fd\u08f9\3\2\2\2\u08fe\u0185\3\2\2\2\u08ff\u0905\5\u0190\u00c9"+
		"\2\u0900\u0901\5\u0190\u00c9\2\u0901\u0902\7\u0093\2\2\u0902\u0903\5\u0190"+
		"\u00c9\2\u0903\u0905\3\2\2\2\u0904\u08ff\3\2\2\2\u0904\u0900\3\2\2\2\u0905"+
		"\u090d\3\2\2\2\u0906\u0908\5\24\13\2\u0907\u0906\3\2\2\2\u0907\u0908\3"+
		"\2\2\2\u0908\u0909\3\2\2\2\u0909\u090a\7\u0081\2\2\u090a\u090c\5\u0190"+
		"\u00c9\2\u090b\u0907\3\2\2\2\u090c\u090f\3\2\2\2\u090d\u090b\3\2\2\2\u090d"+
		"\u090e\3\2\2\2\u090e\u0187\3\2\2\2\u090f\u090d\3\2\2\2\u0910\u0911\7G"+
		"\2\2\u0911\u0912\5\u0110\u0089\2\u0912\u0913\7\177\2\2\u0913\u0916\5\u018a"+
		"\u00c6\2\u0914\u0915\7\u0082\2\2\u0915\u0917\5\u018a\u00c6\2\u0916\u0914"+
		"\3\2\2\2\u0916\u0917\3\2\2\2\u0917\u0918\3\2\2\2\u0918\u091e\7\u0080\2"+
		"\2\u0919\u091f\5\u0116\u008c\2\u091a\u091b\5\u0154\u00ab\2\u091b\u091c"+
		"\5\32\16\2\u091c\u091d\7\u0084\2\2\u091d\u091f\3\2\2\2\u091e\u0919\3\2"+
		"\2\2\u091e\u091a\3\2\2\2\u091f\u0189\3\2\2\2\u0920\u0921\5\6\4\2\u0921"+
		"\u0924\5\u0190\u00c9\2\u0922\u0923\7\u008f\2\2\u0923\u0925\5\32\16\2\u0924"+
		"\u0922\3\2\2\2\u0924\u0925\3\2\2\2\u0925\u018b\3\2\2\2\u0926\u0928\7\177"+
		"\2\2\u0927\u0929\5\26\f\2\u0928\u0927\3\2\2\2\u0928\u0929\3\2\2\2\u0929"+
		"\u092a\3\2\2\2\u092a\u092b\7\u0080\2\2\u092b\u018d\3\2\2\2\u092c\u092e"+
		"\7\177\2\2\u092d\u092f\5\26\f\2\u092e\u092d\3\2\2\2\u092e\u092f\3\2\2"+
		"\2\u092f\u0930\3\2\2\2\u0930\u0932\7\u0080\2\2\u0931\u0933\5J&\2\u0932"+
		"\u0931\3\2\2\2\u0932\u0933\3\2\2\2\u0933\u018f\3\2\2\2\u0934\u0935\t\23"+
		"\2\2\u0935\u0191\3\2\2\2\u012a\u0193\u0196\u0199\u019e\u01a2\u01a8\u01ab"+
		"\u01b0\u01b4\u01bb\u01bd\u01c4\u01c8\u01cd\u01d7\u01df\u01e9\u01ef\u01f2"+
		"\u01f8\u01fd\u020e\u0216\u021b\u0222\u022a\u0232\u023a\u0242\u024a\u0254"+
		"\u0256\u025c\u0261\u0269\u0271\u028c\u0292\u029b\u02a0\u02a5\u02ab\u02b9"+
		"\u02bf\u02cb\u02cf\u02d4\u02d8\u02de\u02e5\u02f8\u02fd\u0300\u030f\u0315"+
		"\u0318\u031d\u0320\u0328\u0330\u033b\u0340\u0345\u0347\u0350\u0358\u035f"+
		"\u0367\u036b\u0374\u0379\u037b\u0384\u038c\u0390\u0395\u0397\u039c\u03a0"+
		"\u03a7\u03af\u03b1\u03b5\u03b8\u03c9\u03d0\u03d4\u03de\u03e3\u03ea\u03f3"+
		"\u03f8\u03ff\u040b\u0416\u041e\u0423\u042c\u0438\u043d\u0441\u044e\u0458"+
		"\u046e\u0472\u0476\u048c\u0491\u0496\u049d\u04a0\u04b6\u04c2\u04c6\u04d0"+
		"\u04d6\u04db\u04e3\u04e8\u04f2\u04f7\u04ff\u0502\u0509\u0510\u0514\u0517"+
		"\u051d\u0521\u0527\u0535\u053b\u0542\u0547\u054a\u054d\u0554\u055e\u056f"+
		"\u0574\u0578\u057b\u057e\u0585\u058b\u0593\u0599\u05a3\u05ab\u05b1\u05bc"+
		"\u05c0\u05c2\u05c7\u05ce\u05d7\u05de\u05e1\u05e4\u05e8\u05ed\u05fa\u0604"+
		"\u0610\u0617\u0623\u0629\u062d\u0631\u0637\u063d\u063f\u0646\u064a\u064d"+
		"\u0651\u0656\u065d\u0660\u0665\u066a\u066c\u066f\u0672\u0678\u067b\u0687"+
		"\u068b\u068e\u0698\u069b\u06a1\u06bc\u06c9\u06cf\u06d8\u06de\u06e1\u06e9"+
		"\u06ed\u06f3\u06f9\u06ff\u070a\u070e\u0710\u071a\u0720\u0723\u0730\u0736"+
		"\u0739\u073c\u0741\u0745\u0749\u075a\u075d\u0762\u0766\u076a\u0773\u0776"+
		"\u077b\u077f\u0784\u0788\u078a\u0795\u0799\u079b\u07a0\u07a5\u07ac\u07b2"+
		"\u07b7\u07bd\u07c1\u07c7\u07ce\u07d8\u07db\u07de\u07e3\u07e9\u07ed\u07ef"+
		"\u07f6\u07fd\u0805\u0809\u0829\u0831\u0837\u0840\u0849\u084f\u0856\u085d"+
		"\u085f\u0866\u0869\u086c\u0870\u0875\u0878\u087b\u087f\u0884\u0887\u088a"+
		"\u088e\u0893\u0897\u089d\u08a1\u08a5\u08b3\u08c0\u08c6\u08d9\u08e4\u08e8"+
		"\u08ee\u08f2\u08f6\u08fd\u0904\u0907\u090d\u0916\u091e\u0924\u0928\u092e"+
		"\u0932";
	public static final ATN _ATN =
		new ATNDeserializer().deserialize(_serializedATN.toCharArray());
	static {
		_decisionToDFA = new DFA[_ATN.getNumberOfDecisions()];
		for (int i = 0; i < _ATN.getNumberOfDecisions(); i++) {
			_decisionToDFA[i] = new DFA(_ATN.getDecisionState(i), i);
		}
	}
}