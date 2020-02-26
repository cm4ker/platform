
parser grammar ZSharpParser;

options { tokenVocab = ZSharpLexer; }


entryPoint: 
    (moduleDefinition
    | typeDefinition
    | namespaceDefinition
    | usingSection)*;

usingSection: (usingDefinition | aliasingTypeDefinition)+;

usingDefinition : 
        USING namespace ';';

aliasingTypeDefinition: 
        USING alias=name '=' typeName ';';

namespace :
 IDENTIFIER ('.' IDENTIFIER)*
;



namespaceDefinition : 
    NAMESPACE namespace '{' (moduleDefinition | typeDefinition)* '}'
;

/*
================START MODULE==================
*/

moduleDefinition: MODULE typeName '{' moduleBody '}';

moduleBody: (methodDeclaration)* ;
/*
================END MODULE==================
*/

/*
================START TYPE==================
*/
typeDefinition: attributes? TYPE typeName '{' typeBody '}';

typeBody: (usingSection)* (methodDeclaration | fieldDeclaration | propertyDeclaration)*;
/*
================END TYPE==================
*/

typeName:
    (namespace '.')? IDENTIFIER;

instructionsBody : '{' statements '}';

instructionsOrSingleStatement : 
    instructionsBody | statement;

methodDeclaration:
    attributes? accessModifier? type IDENTIFIER ('<' genericParameters '>')? '(' parameters? ')' instructionsBody;

genericParameters:
    genericParameter (',' genericParameter)*
    ;    

genericParameter:
    IDENTIFIER
;
    
fieldDeclaration : 
    type name ';';
    
propertyDeclaration:
    accessModifier? type name 
            ('{' (GET ';' | GET getInst= instructionsBody)? (SET ';' | SET setInst = instructionsBody)? '}') 
;

statement: 
        ((variableDeclaration 
        | expression
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
   | lookupExpression '=' expression
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
    | lookupExpression
    ;

lookupExpression:
    lookupExpression '.' (name | functionCall) 
    | expressionStructural '.' (name | functionCall) 
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
   typeName;
    
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
    '$' expression;  
        
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