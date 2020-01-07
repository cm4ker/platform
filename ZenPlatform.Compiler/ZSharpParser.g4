
parser grammar ZSharpParser;

options { tokenVocab = ZSharpLexer; }


entryPoint: 
    (moduleDefinition
    | typeDefinition
    | usingDefinition
    | namespaceDefinition
    | aliasingTypeDefinition)*;

usingDefinition : 
        USING namespace ';';

aliasingTypeDefinition: 
        USING alias=name '=' namespace ('.' typeName = name) ';';

namespace :
 name ('.' name)*
;



namespaceDefinition : 
    NAMESPACE namespace '{' (moduleDefinition | typeDefinition)* '}'
;

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
typeDefinition: attributes? TYPE IDENTIFIER '{' typeBody '}';

typeBody: (functionDeclaration | fieldDeclaration | propertyDeclaration)*;
/*
================END TYPE==================
*/


instructionsBody : '{' statements '}';

instructionsOrSingleStatement : 
    instructionsBody | statement;

functionDeclaration:
    attributes? accessModifier? type IDENTIFIER '(' parameters? ')' instructionsBody;
    
fieldDeclaration : 
    type name ';';
    
propertyDeclaration:
    accessModifier? type name 
            ('{' (GET ';' | GET getInst= instructionsBody)? (SET ';' | SET setInst = instructionsBody)? '}') 
;

statement: 
        ((variableDeclaration 
        | functionCall
        | assigment
        | (RETURN returnExpression = expression))
        ';'+ )
         | (ifStatement | forStatement | whileStatement | tryStatement)
        ; 

statements: 
    (statement)*;

variableDeclaration:
    variableType IDENTIFIER 
    | variableType IDENTIFIER '=' expression
    | variableType IDENTIFIER '=' NEW '[' expression ']'
   ;
    
assigment: 
   name '=' expression
   | name '[' indexExpression=expression ']' '=' assigmentExpression=expression
   | name OP_INC
   | name OP_DEC
;   



functionCall: 
    functionName=name '(' arguments? ')'
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
    expressionStructural
    | lookupExpression
    ;

lookupExpression:
    lookupExpression '.' expressionStructural    
    | expressionStructural '.' expressionStructural
;

expressionStructural:
   functionCall
   | expressionPrimitive 
   ;

expressionPrimitive:
    expressionBinary
;

castExpression: 
    '(' type ')' expression
;

expressionBinary:
    expressionEquality
    | expressionBinary OP_AND expressionEquality
    | expressionBinary OP_OR expressionEquality
;

expressionEquality: 
    expressionRelational
    | expressionEquality OP_EQ expressionRelational
    | expressionEquality OP_NE expressionRelational
;

expressionRelational:
       expressionAdditive 
       | expressionRelational GT expressionAdditive
       | expressionRelational LT expressionAdditive
       | expressionRelational OP_GT expressionAdditive
       | expressionRelational OP_LE expressionAdditive 
;

expressionAdditive:
   expressionMultiplicative
        | expressionAdditive PLUS expressionMultiplicative
        | expressionAdditive MINUS expressionMultiplicative
;

expressionMultiplicative:
    expressionUnary
    | expressionMultiplicative STAR expressionUnary
    | expressionMultiplicative DIV expressionUnary
    | expressionMultiplicative  PERCENT expressionUnary
;

expressionUnary:
    expressionPostfix
    | PLUS expressionAtom
    | MINUS expressionAtom
    | BANG expressionAtom
;

expressionPostfix: 
    expressionAtom
    | castExpression 
    | '(' expression ')'
    | expressionAtom '[' indexerExpression=expression ']'
;

expressionAtom:
    literal
    | name
    | globalVar
;

variableType: 
    type | VAR
    ;
    

type:
    structureType | primitiveType | arrayType | multitype;

multitype : 
    '<' typeList '>';

typeList: 
    type (',' type)*
;

structureType:
    IDENTIFIER;
    
primitiveType:
    BOOL 
    | INT
    | STRING 
    | CHAR 
    | DOUBLE
    | VOID
    | OBJECT;

accessModifier: 
    PUBLIC
    | PRIVATE;

arrayType: 
    (structureType | primitiveType )'[' ']';

name:
    IDENTIFIER;


globalVar:
    '$' ('.' (name | functionCall))*;  
        
ifStatement:
    IF '(' expression ')' instructionsOrSingleStatement (ELSE instructionsOrSingleStatement)?;
    
forStatement:
    FOR '('variableDeclaration ';' conditionExpression=expression ';' assigment ')' instructionsOrSingleStatement;

whileStatement:
    WHILE '(' expression ')' instructionsOrSingleStatement;
    
attribute:
    '[' type ('(' arguments? ')')? ']';
    
attributes:
    attribute+;   
    
tryStatement:
    TRY instructionsOrSingleStatement 
    (CATCH catchExp=instructionsOrSingleStatement)? 
    (FINALLY finallyExp=instructionsOrSingleStatement)?;