parser grammar ZSharpParser;

options { tokenVocab = ZSharpLexer; }

entryPoint: 
    import_directives?
    (  
       global_method_declaration
     | namespace_declaration
     | component_declaration
    )*     
    EOF;

// #region Basic concepts
namespace_or_type_name 
	: (identifier type_argument_list?) ('.' identifier type_argument_list?)*
	;
// #endregion

// #region Namespaces
namespace_declaration
	: NAMESPACE qi=qualified_identifier namespace_body ';'?
	;

qualified_identifier
	: identifier ( '.'  identifier )*
	;

namespace_body
	: OPEN_BRACE extern_alias_directives? namespace_member_declarations? CLOSE_BRACE
	;
	
extern_alias_directives
    : extern_alias_directive+
    ;

extern_alias_directive
    : EXTERN ALIAS identifier ';'
    ;

qualified_alias_member
	: identifier '::' identifier type_argument_list?
	;

import_directives
	: import_directive+
	;

import_directive
	: IMPORT identifier '=' namespace_or_type_name ';'            #usingAliasDirective
	| IMPORT namespace_or_type_name ';'                           #usingNamespaceDirective
	| IMPORT STATIC namespace_or_type_name ';'                    #usingStaticDirective
	;

namespace_member_declarations
	: namespace_member_declaration+
	;

namespace_member_declaration
	: namespace_declaration
	;

class_body
	: OPEN_BRACE class_member_declarations? CLOSE_BRACE
	;

class_member_declarations
	: class_member_declaration+
	;	
	
class_member_declaration
	: all_member_modifiers? common_member_declaration
	;

all_member_modifiers
	: all_member_modifier+
	;

all_member_modifier
	: NEW
	| PUBLIC
	| PRIVATE
	| STATIC

	;

common_member_declaration
	: 
	  method_declaration
	;
	
// #endregion

// #region Components
component_declaration
	: COMPONENT qi=qualified_identifier component_body ';'?
	;

component_body
	: OPEN_BRACE extern_alias_directives? component_member_declarations? CLOSE_BRACE
	;
	
component_member_declarations
	: component_member_declaration+
	;

component_member_declaration
	: type_extend
	;

type_extend
	: EXTEND identifier extend_body ';'?
	; 	

extend_body
	: OPEN_BRACE extend_member_declarations? CLOSE_BRACE
	;

extend_member_declarations
	: extend_member_declaration+
	;	
	
extend_member_declaration
	: all_member_modifiers? common_member_declaration
	;
	
// #endregion

// #region Types
type_
	: base_type ('?' | rank_specifier | '*')*
	;

base_type
	: simple_type
	| class_type  // represents types: enum, class, interface, delegate, type_parameter
	| VOID '*'
	| tuple_type
	| union_type
	;

tuple_type
    : '(' tuple_element (',' tuple_element)+ ')'
    ;
union_type
    : (simple_type | class_type) ('|' base_type)* ;

tuple_element
    : type_ identifier?
    ;

simple_type 
	: numeric_type
	| BOOL
	;

numeric_type 
	: integral_type
	| floating_point_type
	| DECIMAL
	;
integral_type 
	: SBYTE
	| BYTE
	| SHORT
	| USHORT
	| INT
	| UINT
	| LONG
	| ULONG
	| CHAR
	;

floating_point_type 
	: FLOAT
	| DOUBLE
	;

/** namespace_or_type_name, OBJECT, STRING */
class_type 
	: namespace_or_type_name
	| OBJECT
	| DYNAMIC
	| STRING
	;

type_argument_list 
	: '<' type_ ( ',' type_)* '>'
	;
// #endregion

// #region Attributes
global_attribute_section
	: '[' global_attribute_target ':' attribute_list ','? ']'
	;

global_attribute_target
	: 
	 identifier
	;

attributes
	: attribute_section+
	;

attribute_section
	: '[' (attribute_target ':')? attribute_list ','? ']'
	;

attribute_target
	: identifier
	;

attribute_list
	: attribute (','  attribute)*
	;

attribute
	: namespace_or_type_name (OPEN_PARENS (attribute_argument (','  attribute_argument)*)? CLOSE_PARENS)?
	;

attribute_argument
	: (identifier ':')? expression
	;
// #endregion


// #region Arrays
rank_specifier
	: '[' ','* ']'
	;
// #endregion

// #region Structural
//usingSection: (usingDefinition | aliasingTypeDefinition)+;
 
usingDefinition : 
        USING namespace ';';



namespace :
 IDENTIFIER ('.' IDENTIFIER)*
;

typeName:
    (namespace '.')? IDENTIFIER;



block
	: OPEN_BRACE statement_list? CLOSE_BRACE
	;
// #endregion

// #region Statements
statement_list
	: statement+
	;

labeled_Statement
	: identifier ':' statement  
	;

statement
	: labeled_Statement
	| declarationStatement
	| embedded_statement
	;

local_variable_type 
	: VAR
	| type_
	;

local_variable_declaration
	: local_variable_type local_variable_declarator ( ','  local_variable_declarator)*
	;
	
local_variable_declarator
    : identifier ('=' REF? local_variable_initializer)?
    ;

local_variable_initializer
	: expression
	//| array_initializer
	//| stackalloc_initializer
	;

declarationStatement
	: local_variable_declaration ';'
	;

return_type
    : type_
    | VOID
    ;

method_declaration:
    return_type IDENTIFIER '(' parameters? ')' method_body;

global_method_declaration:
    attributes? all_member_modifiers? method_declaration;

parameters: parameter (',' parameter)*
;

parameter:
    (REF)? type_ IDENTIFIER
;

method_body
	: block
	| ';'
	;

embedded_statement
	: block
	| simple_embedded_statement
	;
	
for_initializer
    : local_variable_declaration
    | expression (','  expression)*
    ;
    
for_iterator
	: expression (','  expression)*
	;    

if_body
	: block
	| simple_embedded_statement
	;

simple_embedded_statement
	: ';'                                                         #theEmptyStatement
	| expression ';'                                              #expressionStatement

	// selection statements
	| IF OPEN_PARENS expression CLOSE_PARENS if_body (ELSE if_body)?               #ifStatement
//    | SWITCH OPEN_PARENS expression CLOSE_PARENS OPEN_BRACE switch_section* CLOSE_BRACE           #switchStatement

    // iteration statements
	| WHILE OPEN_PARENS expression CLOSE_PARENS embedded_statement                                        #whileStatement
	| DO embedded_statement WHILE OPEN_PARENS expression CLOSE_PARENS ';'                                 #doStatement
	| FOR OPEN_PARENS for_initializer? ';' expression? ';' for_iterator? CLOSE_PARENS embedded_statement  #forStatement
	| AWAIT? FOREACH OPEN_PARENS local_variable_type identifier IN expression CLOSE_PARENS embedded_statement    #foreachStatement

    // jump statements
	| BREAK ';'                                                   #breakStatement
	| CONTINUE ';'                                                #continueStatement
	| GOTO (identifier | CASE expression | DEFAULT) ';'           #gotoStatement
	| RETURN expression? ';'                                      #returnStatement
	| THROW expression? ';'                                       #throwStatement

//	| TRY block (catch_clauses finally_clause? | finally_clause)  #tryStatement
//	| CHECKED block                                               #checkedStatement
//	| UNCHECKED block                                                               #uncheckedStatement
//	| LOCK OPEN_PARENS expression CLOSE_PARENS embedded_statement                  #lockStatement
//	| USING OPEN_PARENS resource_acquisition CLOSE_PARENS embedded_statement       #usingStatement
	| YIELD (RETURN expression | BREAK) ';'                                         #yieldStatement
	;
//#endregion

// #region Expressions
argument_list 
	: 
	| (argument ( ',' argument)*)
	;

argument
	: (identifier ':')? refout=(REF | OUT | IN)? (VAR | type_)? expression
	;

expression
	: assignment
	| non_assignment_expression
	| REF non_assignment_expression
	;

non_assignment_expression
	: 
	conditional_expression
	;

assignment
	: unary_expression assignment_operator expression
	//| unary_expression '??=' throwable_expression
	;

assignment_operator
	: '=' | '+=' | '-=' | '*=' | '/=' | '%=' | '&=' | '|=' | '^=' | '<<=' | right_shift_assignment
	;

conditional_expression
	: null_coalescing_expression ('?' throwable_expression ':' throwable_expression)?
	;

null_coalescing_expression
	: conditional_or_expression ('??' (null_coalescing_expression | throw_expression))?
	;

conditional_or_expression
	: conditional_and_expression (OP_OR conditional_and_expression)*
	;

conditional_and_expression
	: inclusive_or_expression (OP_AND inclusive_or_expression)*
	;

inclusive_or_expression
	: exclusive_or_expression ('|' exclusive_or_expression)*
	;

exclusive_or_expression
	: and_expression ('^' and_expression)*
	;

and_expression
	: equality_expression ('&' equality_expression)*
	;

equality_expression
	: relational_expression (op=(OP_EQ | OP_NE)  relational_expression)*
	;

relational_expression
	: shift_expression (op=(LT | GT | OP_LE | OP_GE) shift_expression | IS isType | AS type_)*
	;

shift_expression
	: additive_expression (('<<' | right_shift)  additive_expression)*
	;

additive_expression
	: multiplicative_expression ((PLUS | MINUS)  multiplicative_expression)*
	;

multiplicative_expression
	: match_expression (('*' | '/' | '%' ) match_expression)*
	;
/*

match (expression)
    |  (literal)  
    |  (literal)  
    |  (literal)                   => print( "found 1, 2 or 3" )  
    |  (variable) when (condition) => print("even number more than 3")
    |  _                           => print("found not even number more than 3")  



match (expression)
    |  first..tail -> print(first)   
    | { } -> // error not allow collections


*/
match_expression
     :  'match' '(' expression ')' match_expression_arms
     |  range_expression ('match' '(' expression ')' match_expression_arms)?
    ;

match_expression_arms
    :
        match_expression_arm+
    ;    
    
match_expression_arm
    :
        '|' pattern=expression when_guard? right_arrow (result=expression)?
    ;    

when_guard
	: WHEN expression
	;

//switch_expression
//    : range_expression ('switch' '{' (switch_expression_arms ','?)? '}')?
//    ;
//
//switch_expression_arms
//    : switch_expression_arm (',' switch_expression_arm)*
//    ;
//
//switch_expression_arm
//    : expression case_guard? right_arrow throwable_expression
//    ;

range_expression
    : unary_expression
    | unary_expression? OP_RANGE unary_expression?
    ;

// https://msdn.microsoft.com/library/6a71f45d(v=vs.110).aspx
unary_expression
	: primary_expression                      
	| PLUS unary_expression                   
	| MINUS unary_expression 
	| BANG unary_expression 
	//| '~' unary_expression
	| OP_INC unary_expression 
	| OP_DEC unary_expression 
	| OPEN_PARENS type_ CLOSE_PARENS unary_expression 
//	| AWAIT unary_expression // C# 5
	| '&' unary_expression 
	| '*' unary_expression 
	| '^' unary_expression 
	;

primary_expression  // Null-conditional operators C# 6: https://msdn.microsoft.com/en-us/library/dn986595.aspx
	: pe=primary_expression_start '!'? bracket_expression* '!'?
	  (((member_access | method_invocation | OP_INC | OP_DEC | '->' identifier) '!'?) bracket_expression* '!'?)*
	;

primary_expression_start
	: literal                                                               #literalExpression
	| identifier type_argument_list?                                        #simpleNameExpression
	| OPEN_PARENS expression CLOSE_PARENS                                   #parenthesisExpressions
	| predefined_type                                                       #memberAccessExpression
	| qualified_alias_member                                                #memberAccessExpression
	| LITERAL_ACCESS                                                        #literalAccessExpression
	| THIS                                                                  #thisReferenceExpression
	| BASE ('.' identifier type_argument_list? | '[' expression_list ']')   #baseAccessExpression
	| OPEN_PARENS argument ( ',' argument )+ CLOSE_PARENS                   #tupleExpression
	| TYPEOF OPEN_PARENS (unbound_type_name | type_ | VOID) CLOSE_PARENS    #typeofExpression
	| CHECKED OPEN_PARENS expression CLOSE_PARENS                           #checkedExpression
	| UNCHECKED OPEN_PARENS expression CLOSE_PARENS                         #uncheckedExpression
	| DEFAULT (OPEN_PARENS type_ CLOSE_PARENS)?                             #defaultValueExpression
	| SIZEOF OPEN_PARENS type_ CLOSE_PARENS                                 #sizeofExpression
	| NAMEOF OPEN_PARENS (identifier '.')* identifier CLOSE_PARENS          #nameofExpression
	;

throwable_expression
	: expression
	| throw_expression
	;

throw_expression
	: THROW expression
	;

member_access
	: '?'? '.' identifier type_argument_list?
	;

bracket_expression
	: '?'? '[' indexer_argument ( ',' indexer_argument)* ']'
	;

indexer_argument
	: (identifier ':')? expression
	;

predefined_type
	: BOOL | BYTE | CHAR | DECIMAL | DOUBLE | FLOAT | INT | LONG
	| OBJECT | SBYTE | SHORT | STRING | UINT | ULONG | USHORT
	;

expression_list
	: expression (',' expression)*
	;

object_or_collection_initializer
	: object_initializer
	| collection_initializer
	;

object_initializer
	: OPEN_BRACE (member_initializer_list ','?)? CLOSE_BRACE
	;

member_initializer_list
	: member_initializer (',' member_initializer)*
	;

member_initializer
	: (identifier | '[' expression ']') '=' initializer_value // C# 6
	;

initializer_value
	: expression
	| object_or_collection_initializer
	;

collection_initializer
	: OPEN_BRACE element_initializer (',' element_initializer)* ','? CLOSE_BRACE
	;

element_initializer
	: non_assignment_expression
	| OPEN_BRACE expression_list CLOSE_BRACE
	;

anonymous_object_initializer
	: OPEN_BRACE (member_declarator_list ','?)? CLOSE_BRACE
	;

member_declarator_list
	: member_declarator ( ',' member_declarator)*
	;

member_declarator
	: primary_expression
	| identifier '=' expression
	;

unbound_type_name
	: identifier ( generic_dimension_specifier? | '::' identifier generic_dimension_specifier?)
	  ('.' identifier generic_dimension_specifier?)*
	;

generic_dimension_specifier
	: '<' ','* '>'
	;

isType
	: base_type (rank_specifier | '*')* '?'? isTypePatternArms? identifier?
	;

isTypePatternArms
	: '{' isTypePatternArm (',' isTypePatternArm)* '}'
	;

isTypePatternArm
	: identifier ':' expression
	;

// #endregion

// #region MISC 
right_arrow
	: first='=' second='>' {$first.index + 1 == $second.index}? // Nothing between the tokens?
	;

right_shift
	: first='>' second='>' {$first.index + 1 == $second.index}? // Nothing between the tokens?
	;

right_shift_assignment
	: first='>' second='>=' {$first.index + 1 == $second.index}? // Nothing between the tokens?
	;

literal
	: boolean_literal
	| string_literal
	| INTEGER_LITERAL
	| HEX_INTEGER_LITERAL
	//| BIN_INTEGER_LITERAL
	| REAL_LITERAL
	| CHARACTER_LITERAL
	//| NULL
	;

boolean_literal
	: TRUE
	| FALSE
	;

string_literal
	: REGULAR_STRING
	| VERBATIUM_STRING
	| SQL_STRING
	;
	
method_invocation
	: OPEN_PARENS argument_list? CLOSE_PARENS
	;

identifier :
    IDENTIFIER;
	
// #endregion
