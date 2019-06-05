
parser grammar ZSharpParser;

options { tokenVocab = ZSharpLexer; }


entryPoint: 
    moduleDefinition
    | typeDefinition
    | usingDefinition;

usingDefinition : 
        USING name ';'
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

typeDefinition: attributes? TYPE IDENTIFIER '{' '}';

/*
================END TYPE==================
*/


instructionsBody : '{' statements '}';

instructionsOrSingleStatement : 
    instructionsBody | statement;

functionDeclaration:attributes? accessModifier? type IDENTIFIER '(' parameters? ')' instructionsBody;

/*чертовски сложное правило*/
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
    name '(' arguments? ')' ( '.' propertyExpression = name )* ('.' functionCall)*
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
    | functionCallExpression
    | name
    
    
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
    '$' 
        (name ('{'statements'}')?)
        | functionCall;  
        
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
    (FINALLY finallyExp=instructionsOrSingleStatement)? ;