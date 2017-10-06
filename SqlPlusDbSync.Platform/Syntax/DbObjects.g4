grammar DbObjects;

eval	:	definitions+;	
	
definitions	:	objectDefinitionTable
		|	objectDefinitionObject
		|	pointDefinition
		|	ownerDefinition;

pointDefinition
	:	DEFINE POINT pointName FROM tableName ON onFieldName VALUE valueFieldName;

ownerDefinition
	:	DEFINE OWNER ownerName FROM tableName ON onFieldName VALUE valueFieldName;

objectDefinitionTable
	:	DEFINE TRANSFER? OBJECT objectDefinitionName AS TABLE tableExpression whereExpression* objectEvents? objectOptions?;

objectDefinitionObject
	:	DEFINE TRANSFER? OBJECT objectDefinitionName AS OBJECT 
		LeftBracket* objectName RightBracket* objectRelations objectEvents? objectOptions? objectPoints?;

tableExpression
	: tableName LeftBracket 
				( includeExpression 
				| excludeExpression 
				| pointExpression
				| ownerExpression 
				| Comma pointExpression 
				| Comma includeExpression 
				| Comma excludeExpression
				| Comma ownerExpression )*
				(uniqueExpression | (Comma uniqueExpression) | (uniqueExpression Comma))
				( includeExpression 
				| excludeExpression 
				| pointExpression 
				| ownerExpression
				| Comma pointExpression 
				| Comma includeExpression 
				| Comma excludeExpression 
				| Comma ownerExpression)*
				RightBracket ;

uniqueExpression
	: UNIQUE LeftBracket columnList* RightBracket;

includeExpression
	: INCLUDE LeftBracket columnList* RightBracket;

excludeExpression
	: EXCLUDE LeftBracket columnList* RightBracket;

pointExpression
	: POINT LeftBracket pointName Comma fieldName RightBracket;

ownerExpression
	: OWNER LeftBracket ownerName Comma fieldName RightBracket;

objectPoints
	: POINT LeftBracket pointName Comma fieldExpression RightBracket; 

objectRelations
	:	REFERENCES objectRelation+;

objectRelation
	:	RELATIONID REL_TYPE DoubleDot ID ON join+;

objectEvents
	:	EVENTS 	objectEventsTypes*;

objectEventsTypes
	:	(objectOnEvent | objectAfterEvent | objectBeforeEvent);

objectBeforeEvent
	:	BEFORE ( (SAVE DoubleDot beforeSaveStatement*) | ( DELETE DoubleDot beforeDeleteStatement*) );

objectOnEvent
	:	ON  (UPDATE DoubleDot onUpdateStatement* | CREATE DoubleDot onCreateStatement* | ERROR DoubleDot onErrorStatement* ); 

objectAfterEvent
	:	AFTER  (LOAD DoubleDot afterLoadStatement* | UPDATE DoubleDot afterUpdateStatement* | CREATE DoubleDot afterCreateStatement* | DELETE DoubleDot afterDeleteStatement*); 

afterLoadStatement
	:	invokeStatement;

beforeSaveStatement
	:	invokeStatement;

afterUpdateStatement
	:	invokeStatement;

afterCreateStatement
	:	invokeStatement;

afterDeleteStatement
	:	invokeStatement;

onErrorStatement
	:	invokeStatement;

onUpdateStatement
	:	updateAllReferencesStatement |	invokeStatement ;

onCreateStatement
	:	invokeStatement;

beforeDeleteStatement
	:	invokeStatement;
		

updateAllReferencesStatement
	:	UPDATE ALL REFERENCES SEMICOLON;

deleteCascadeStatement
	:	DELETE CASCADE SEMICOLON;

invokeStatement
	:	invokeBeginStatement invokeBodyStatement invokeEndStatement SEMICOLON; //statement_list

invokeBodyStatement
	: ~END_INVOKE*;

invokeBeginStatement
	: BEGIN_INVOKE;

invokeEndStatement
	: END_INVOKE;

//  statement
//  	:	ANY+;

objectOptions
	:	OPTIONS 
			directionOptions?
			transferTimeOptions?;



directionOptions
	:	DIRECTION (UP | DOWN);

transferTimeOptions
	:	TRANSFER_TIME EVERY (NUMBER MIN | HOUR | DAY) | SESSION; 

join
	:	fieldExpression compareOperatorExpression fieldExpression
	|	AND fieldExpression compareOperatorExpression fieldExpression;

fieldExpression
	:	tableName Dot fieldName;
	
whereExpression	
	: 
		WHERE fieldExpression compareOperatorExpression valueExpression;

compareOperatorExpression
	:
		EQUALS | IN;


valueExpression 
	:
		NUMBER | numberArrayExpression;

numberArrayExpression
	:
		LeftBracket 
		(NUMBER | NUMBER Comma)+
		RightBracket;


	
columnList
	:	ID
	|	ID Comma
	|	ID UNIQUE Comma?
	|	ID POINT LeftBracket pointName RightBracket Comma?;

tableName 
	:	ID;

fieldName
	: ID;

onFieldName
	:	ID;

valueFieldName
	:	ID;

objectList
	:	(objectName Comma?)+;

objectDefinitionName
	:	ID;

objectName
	:	ID;

pointName
	:	ID;

ownerName
	: 	ID;

      

EQUALS_SYMBOL	: '=';
MORE_THAN 		: '>';
LESS_THAN 		: '<';
NOT_EQUALS 		: '<>';
MINUS			: '-';
IN				: I N;
EQUALS			: E Q U A L S;

NEWLINE  : '\r'? '\n' -> channel(HIDDEN);
SPACE	: ' ' -> channel(HIDDEN);
TABS	: '\t' -> channel(HIDDEN);

POINTER : '^';
SEMICOLON	: ';';
DOUBLE_QUOTION	: '"';
QUOTION			: '\'';
EXCLAMATION		: '!';
STAR			: '*';
AT				: '@';

//ANY		: .;
Comma 	:	 ',';
Dot	:	'.';
DoubleDot:	':';
ALL			:   A L L;
DEFINE 		:	D E F I N E;
OBJECT 		:	O B J E C T;
AS			:	A S;
TABLE		:	T A B L E;
AND			:	A N D;
ON			: 	O N;
AFTER		:	A F T E R;
REL			:	R E L;
WITH		: 	W I T H;
TYPE		:	T Y P E;
WHERE		: 	W H E R E;
UNIQUE 		:	U N I Q U E;
REFERENCES 	: 	R E F E R E N C E S;
OPTIONS		: 	O P T I O N S;
DIRECTION	: 	D I R E C T I O N;
UP			: 	U P;
DOWN		: 	D O W N;
TRANSFER_TIME : T R A N S F E R '_' T I M E;
MIN			:	M I N;
HOUR		: 	H O U R;
DAY			: 	D A Y;
SESSION		:	S E S S I O N;
EVERY		:	E V E R Y;
TRANSFER	:	T R A N S F E R;
POINT		: 	P O I N T;
OWNER		: 	O W N E R;
JOIN		:	J O I N;
VALUE		:	V A L U E;
FROM		:	F R O M;
INCLUDE		:   I N C L U D E;
EXCLUDE		:	E X C L U D E;
LOAD		:	L O A D;
BEFORE		:	B E F O R E;
SAVE		:	S A V E;


EVENTS		: 	E V E N T S;
CREATE		:	C R E A T E;
DELETE		:	D E L E T E;
UPDATE		:	U P D A T E;
INVOKE		:	I N V O K E;
CASCADE		:	C A S C A D E;
ERROR		:	E R R O R;
BEGIN_INVOKE		:	B E G I N '_' INVOKE;
END_INVOKE			:	E N D '_' INVOKE;

REL_TYPE
	:	ENUM_TYPES;

RELATIONID 
	:	'R' DEC_DIGIT ;

LeftBracket 
	:	'(';
RightBracket
	:	')';
LeftBrace
	: 	'{';
RightBrace
	:	'}';
LeftSquadBracket
	:	'[';
RightSquadBracket
	:	']';
Dollar
	:	'$';

ID  :	('a'..'z'|'A'..'Z'|'_'|[\u0410-\u042F]|[\u0430-\u044F]) ('a'..'z'|'A'..'Z'|'0'..'9'|'_'|[\u0410-\u042F]|[\u0430-\u044F])*
    ;
NUMBER	:	MINUS* DEC_DIGIT+;
COMMENT
    :   '//' ~('\n'|'\r')* '\r'? '\n'
    ;
    
fragment LETTER:       [a-zA-Z_];
fragment DEC_DOT_DEC:  (DEC_DIGIT+ '.' DEC_DIGIT+ |  DEC_DIGIT+ '.' | '.' DEC_DIGIT+);
fragment HEX_DIGIT:    [0-9A-Fa-f];
fragment DEC_DIGIT:    [0-9];

fragment A: [aA];
fragment B: [bB];
fragment C: [cC];
fragment D: [dD];
fragment E: [eE];
fragment F: [fF];
fragment G: [gG];
fragment H: [hH];
fragment I: [iI];
fragment J: [jJ];
fragment K: [kK];
fragment L: [lL];
fragment M: [mM];
fragment N: [nN];
fragment O: [oO];
fragment P: [pP];
fragment Q: [qQ];
fragment R: [rR];
fragment S: [sS];
fragment T: [tT];
fragment U: [uU];
fragment V: [vV];
fragment W: [wW];
fragment X: [xX];
fragment Y: [yY];
fragment Z: [zZ];

fragment ENUM_TYPES
	:'ONE_TO_MANY'
	|'ONE_TO_ONE';
