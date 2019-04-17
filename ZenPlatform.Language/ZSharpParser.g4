
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


instructionsBody : '{' (statements)* '}';

functionDeclaration:accessModifier? type IDENTIFIER '(' parameters? ')' instructionsBody;

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

functionCallExpression:
   functionCall
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
    | expression PLUS expressionTerm
    | expression MINUS expressionTerm
;

expressionUnary:
    PLUS expressionPrimary
    | MINUS expressionPrimary
    | BANG expressionPrimary
    | expressionPrimary
    | expressionPrimary '[' expression ']'
;

expressionBinary:
    expressionUnary
    | expressionBinary OP_AND expressionUnary
    | expressionBinary OP_OR expressionUnary
;

expressionFactor: 
    expressionBinary 
    | expressionFactor PERCENT expressionBinary
    | expressionFactor GT expressionBinary
    | expressionFactor LT expressionBinary
    | expressionFactor OP_GT expressionBinary
    | expressionFactor OP_LE expressionBinary
    | expressionFactor OP_EQ expressionBinary
    | expressionFactor OP_NE expressionBinary
;

expressionTerm:
    expressionFactor
    | expressionTerm STAR expressionFactor
    | expressionTerm DIV expressionFactor
;

expressionPrimary:
    literal
    | functionCallExpression
    | name
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

extensionExpression:
    DOLLAR ('{'statements'}' || name)  
   



