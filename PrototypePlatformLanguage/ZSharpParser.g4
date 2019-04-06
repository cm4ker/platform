
parser grammar ZSharpParser;

options { tokenVocab = ZSharpLexer; }


entryPoint: 
    moduleDefinition
    | typeDefinition;


/*
================START MODULE==================
*/

moduleDefinition: MODULE IDENTIFIER '{' moduleBody '}';

moduleBody: (functionDeclaration)* ;
/*
================END MODULE==================
*/

/*
================START TYPE==================
*/

typeDefinition: TYPE IDENTIFIER '{' '}';

/*
================END TYPE==================
*/


block : '{' (statements)* '}';



functionDeclaration:accessModifier? type IDENTIFIER '(' parameters? ')' block;

statement: functionCall; 

statements: 
    (statement ';')+;

variableDeclaration:
    type IDENTIFIER 
    | type IDENTIFIER '=' ;
    

functionCall: 
    IDENTIFIER'(' arguments? ')'
;

parameters: parameter (',' parameter)*;

parameter:
    (REF)? type IDENTIFIER;

arguments:
    argument (',' argument)*;

argument:
    (REF)? 
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



expression:
    
;

expressionUnary:
    '+' expressionPrimary
    | '-' expressionPrimary
    | '!' expressionPrimary
    | expressionPrimary
    | expressionPrimary '[' expression ']'
;

expressionBinary:
    expressionUnary
    | expressionBinary '&&' expressionUnary
    | expressionBinary '||' expressionUnary
;


expressionFactor: 
    expressionBinary |
;

expressionPrimary:
    IDENTIFIER
    | functionCall
    | literal
    | expression
    
;

type:
    structureType | primitiveType | arrayType;

structureType:
    IDENTIFIER;
    
primitiveType:
    BOOL 
    | INT
    | STRING 
    | CHAR 
    | DOUBLE
    | VOID;

accessModifier: 
    PUBLIC
    | PRIVATE;

arrayType: 
    (structureType | primitiveType )'[' ']';



