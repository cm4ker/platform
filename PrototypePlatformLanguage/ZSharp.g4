
grammar ZSharp;

entryPoitn: DEC_DIGIT;

DEC_DIGIT: '1'..'9' '0'..'9'* ('.' '0'..'9')*;

fragment NEWLINE: '\n' || '\n\r';



