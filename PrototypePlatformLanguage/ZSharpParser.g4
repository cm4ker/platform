
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


methodBody : '{' (statements)* '}';



functionDeclaration:accessModifier? type IDENTIFIER '(' parameters? ')' methodBody;

statement: 
        (variableDeclaration
        | functionCall
        | assigment
        | RETURN expression)*; 

statements: 
    (statement ';')+;

variableDeclaration:
    variableType IDENTIFIER 
    | variableType IDENTIFIER '=' expression
    | variableType IDENTIFIER '=' NEW '[' expression ']'
   ;
    
assigment: 
   name '=' expression
   | name '[' expression ']' '=' expression
   | name '++'
   | name '--' 
;    

functionCall: 
    name '(' arguments? ')'
;

parameters: parameter (',' parameter)*;

parameter:
    (REF)? type IDENTIFIER;

arguments:
    argument (',' argument)*;

argument:
    (REF)? expression? 
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
    expressionTerm
    | expression '+' expressionTerm
    | expression '-' expressionTerm
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
    expressionBinary 
    | expressionFactor '%' expressionBinary
    | expressionFactor '>' expressionBinary
    | expressionFactor '<' expressionBinary
    | expressionFactor '>=' expressionBinary
    | expressionFactor '<=' expressionBinary
    | expressionFactor '==' expressionBinary
    | expressionFactor '!=' expressionBinary
;

expressionTerm:
    expressionFactor
    | expressionTerm '*' expressionFactor
    | expressionTerm '/' expressionFactor
;

expressionPrimary:
    IDENTIFIER
    | functionCall
    | literal
    | '(' expression ')'
    
;

variableType: 
    type | VAR
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

name:
    IDENTIFIER
    | name '.' IDENTIFIER
    ;


