
parser grammar ZSharpParser;

options { tokenVocab = ZSharpLexer; }


entryPoint: 
    ( usingSection | method_declaration)*;

usingSection: (usingDefinition | aliasingTypeDefinition)+;

usingDefinition : 
        USING namespace ';';

aliasingTypeDefinition: 
        USING alias=name '=' typeName ';';

namespace :
 IDENTIFIER ('.' IDENTIFIER)*
;

typeName:
    (namespace '.')? IDENTIFIER;

block
	: OPEN_BRACE statement_list? CLOSE_BRACE
	;

identifier :
    IDENTIFIER;

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
	| type
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

method_declaration:
    attributes? accessModifier? type IDENTIFIER '(' parameters? ')' method_body;

method_body
	: block
	| ';'
	;

   
fieldDeclaration : 
    type name ';';
    
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
	| CHECKED block                                               #checkedStatement
	| UNCHECKED block                                                               #uncheckedStatement
	| LOCK OPEN_PARENS expression CLOSE_PARENS embedded_statement                  #lockStatement
//	| USING OPEN_PARENS resource_acquisition CLOSE_PARENS embedded_statement       #usingStatement
	| YIELD (RETURN expression | BREAK) ';'                                         #yieldStatement
	;

assignment
	: expression_unary assignment_operator expression
	;

assignment_operator
	: '=' | '+=' | '-=' | '*=' | '/=' | '%=' | '&=' | '|=' | '^=' 
	; 

parameters: parameter (',' parameter)*
;

parameter:
    (REF)? type IDENTIFIER
;

argument_list:
    argument (',' argument)*
;

argument:
    (REF)? expression 
;

literal
	: boolean_literal
	| string_literal
	| INTEGER_LITERAL
	| HEX_INTEGER_LITERAL
	| REAL_LITERAL
	| CHARACTER_LITERAL
	| NULL
	;

boolean_literal
	: TRUE
	| FALSE
	;

string_literal
	: REGULAR_STRING
	| VERBATIUM_STRING
	;
	
sql_literal:
    SQL_STRING;	

expression:
    expressionStructural
    | assignment
    ;

invocation: 
    '(' argument_list? ')'
;

expressionStructural:
 expressionPrimitive 
   ;

expressionPrimitive:
    expression_binary
;

castExpression: 
    '(' type ')' expression
;

newExpression: 
    NEW typeName '(' ')';

expression_binary:
    expression_equality
    | expression_binary OP_AND expression_equality
    | expression_binary OP_OR expression_equality
;

expression_equality: 
    expression_relational
    | expression_equality OP_EQ expression_relational
    | expression_equality OP_NE expression_relational
;

expression_relational:
       expression_additive 
       | expression_relational GT expression_additive
       | expression_relational LT expression_additive
       | expression_relational OP_GE expression_additive
       | expression_relational OP_LE expression_additive 
;

expression_additive:
   expression_multiplicative
        | expression_additive PLUS expression_multiplicative
        | expression_additive MINUS expression_multiplicative
;

expression_multiplicative:
    expression_unary
    | expression_multiplicative STAR expression_unary
    | expression_multiplicative DIV expression_unary
    | expression_multiplicative  PERCENT expression_unary
;

expression_unary:
    expressionPrimary
    | PLUS expressionAtom
    | MINUS expressionAtom
    | BANG expressionAtom
    | 
;


expressionPrimary:
    expressionPostfix
    ( invocation | memberAccess)*
;

expressionPostfix: 
    | expressionAtom
    | globalVar 
    | castExpression 
    | newExpression
    | '(' expression ')'
    | expressionAtom '[' indexerExpression=expression ']'
 
;

memberAccess:
    '.' name 
;

expressionAtom:
    literal
    | sql_literal
    | name
    
;

variableType: 
    type | VAR
    ;
    

type:
    structureType | arrayType;

//anonimousDeclaration:
//   '{' 
//    (name '=' expression) (',' name '=' expression)*
//    '}'
//;

structureType:
   typeName
    | aliacedTypes;
    
aliacedTypes:
    BOOL 
    | INT
    | UID
    | STRING 
    | CHAR 
    | DOUBLE
    | VOID
    | OBJECT;

accessModifier: 
    PUBLIC
    | PRIVATE;

arrayType: 
    (structureType)'[' ']';

name:
    IDENTIFIER;


globalVar:
    '$' expression;  
        

attribute:
    '[' type ('(' argument_list? ')')? ']';
    
attributes:
    attribute+;   
    