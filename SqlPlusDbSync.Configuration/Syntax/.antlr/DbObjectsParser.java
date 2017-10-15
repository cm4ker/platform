// Generated from c:\Users\n.zenkov\Documents\DEV_ASNA\ASNA\Devs\SqlPlusDbSync\SqlPlusDbSync\SqlPlusDbSync.Platform\Syntax\DbObjects.g4 by ANTLR 4.7
import org.antlr.v4.runtime.atn.*;
import org.antlr.v4.runtime.dfa.DFA;
import org.antlr.v4.runtime.*;
import org.antlr.v4.runtime.misc.*;
import org.antlr.v4.runtime.tree.*;
import java.util.List;
import java.util.Iterator;
import java.util.ArrayList;

@SuppressWarnings({"all", "warnings", "unchecked", "unused", "cast"})
public class DbObjectsParser extends Parser {
	static { RuntimeMetaData.checkVersion("4.7", RuntimeMetaData.VERSION); }

	protected static final DFA[] _decisionToDFA;
	protected static final PredictionContextCache _sharedContextCache =
		new PredictionContextCache();
	public static final int
		EQUALS_SYMBOL=1, MORE_THAN=2, LESS_THAN=3, NOT_EQUALS=4, MINUS=5, IN=6, 
		EQUALS=7, NEWLINE=8, SPACE=9, TABS=10, POINTER=11, SEMICOLON=12, DOUBLE_QUOTION=13, 
		QUOTION=14, EXCLAMATION=15, STAR=16, AT=17, Comma=18, Dot=19, DoubleDot=20, 
		ALL=21, DEFINE=22, OBJECT=23, AS=24, TABLE=25, AND=26, ON=27, AFTER=28, 
		REL=29, WITH=30, TYPE=31, WHERE=32, UNIQUE=33, REFERENCES=34, OPTIONS=35, 
		DIRECTION=36, UP=37, DOWN=38, TRANSFER_TIME=39, MIN=40, HOUR=41, DAY=42, 
		SESSION=43, EVERY=44, TRANSFER=45, POINT=46, JOIN=47, VALUE=48, FROM=49, 
		INCLUDE=50, EXCLUDE=51, LOAD=52, BEFORE=53, SAVE=54, EVENTS=55, CREATE=56, 
		DELETE=57, UPDATE=58, INVOKE=59, CASCADE=60, ERROR=61, BEGIN_INVOKE=62, 
		END_INVOKE=63, REL_TYPE=64, RELATIONID=65, LeftBracket=66, RightBracket=67, 
		LeftSquadBracket=68, RightSquadBracket=69, Dollar=70, ID=71, NUMBER=72, 
		COMMENT=73;
	public static final int
		RULE_eval = 0, RULE_definitions = 1, RULE_pointDefinition = 2, RULE_objectDefinitionTable = 3, 
		RULE_objectDefinitionObject = 4, RULE_tableExpression = 5, RULE_uniqueExpression = 6, 
		RULE_includeExpression = 7, RULE_excludeExpression = 8, RULE_pointExpression = 9, 
		RULE_objectPoints = 10, RULE_objectRelations = 11, RULE_objectRelation = 12, 
		RULE_objectEvents = 13, RULE_objectEventsTypes = 14, RULE_objectBeforeEvent = 15, 
		RULE_objectOnEvent = 16, RULE_objectAfterEvent = 17, RULE_afterLoadStatement = 18, 
		RULE_beforeSaveStatement = 19, RULE_afterUpdateStatement = 20, RULE_afterCreateStatement = 21, 
		RULE_afterDeleteStatement = 22, RULE_onErrorStatement = 23, RULE_onUpdateStatement = 24, 
		RULE_onCreateStatement = 25, RULE_onDeleteStatement = 26, RULE_updateAllReferencesStatement = 27, 
		RULE_deleteCascadeStatement = 28, RULE_invokeStatement = 29, RULE_invokeBodyStatement = 30, 
		RULE_invokeBeginStatement = 31, RULE_invokeEndStatement = 32, RULE_objectOptions = 33, 
		RULE_directionOptions = 34, RULE_transferTimeOptions = 35, RULE_join = 36, 
		RULE_fieldExpression = 37, RULE_whereExpression = 38, RULE_compareOperatorExpression = 39, 
		RULE_valueExpression = 40, RULE_numberArrayExpression = 41, RULE_columnList = 42, 
		RULE_tableName = 43, RULE_fieldName = 44, RULE_onFieldName = 45, RULE_valueFieldName = 46, 
		RULE_objectList = 47, RULE_objectDefinitionName = 48, RULE_objectName = 49, 
		RULE_pointName = 50;
	public static final String[] ruleNames = {
		"eval", "definitions", "pointDefinition", "objectDefinitionTable", "objectDefinitionObject", 
		"tableExpression", "uniqueExpression", "includeExpression", "excludeExpression", 
		"pointExpression", "objectPoints", "objectRelations", "objectRelation", 
		"objectEvents", "objectEventsTypes", "objectBeforeEvent", "objectOnEvent", 
		"objectAfterEvent", "afterLoadStatement", "beforeSaveStatement", "afterUpdateStatement", 
		"afterCreateStatement", "afterDeleteStatement", "onErrorStatement", "onUpdateStatement", 
		"onCreateStatement", "onDeleteStatement", "updateAllReferencesStatement", 
		"deleteCascadeStatement", "invokeStatement", "invokeBodyStatement", "invokeBeginStatement", 
		"invokeEndStatement", "objectOptions", "directionOptions", "transferTimeOptions", 
		"join", "fieldExpression", "whereExpression", "compareOperatorExpression", 
		"valueExpression", "numberArrayExpression", "columnList", "tableName", 
		"fieldName", "onFieldName", "valueFieldName", "objectList", "objectDefinitionName", 
		"objectName", "pointName"
	};

	private static final String[] _LITERAL_NAMES = {
		null, "'='", "'>'", "'<'", "'<>'", "'-'", null, null, null, "' '", "'\t'", 
		"'^'", "';'", "'\"'", "'''", "'!'", "'*'", "'@'", "','", "'.'", "':'", 
		null, null, null, null, null, null, null, null, null, null, null, null, 
		null, null, null, null, null, null, null, null, null, null, null, null, 
		null, null, null, null, null, null, null, null, null, null, null, null, 
		null, null, null, null, null, null, null, null, null, "'('", "')'", "'['", 
		"']'", "'$'"
	};
	private static final String[] _SYMBOLIC_NAMES = {
		null, "EQUALS_SYMBOL", "MORE_THAN", "LESS_THAN", "NOT_EQUALS", "MINUS", 
		"IN", "EQUALS", "NEWLINE", "SPACE", "TABS", "POINTER", "SEMICOLON", "DOUBLE_QUOTION", 
		"QUOTION", "EXCLAMATION", "STAR", "AT", "Comma", "Dot", "DoubleDot", "ALL", 
		"DEFINE", "OBJECT", "AS", "TABLE", "AND", "ON", "AFTER", "REL", "WITH", 
		"TYPE", "WHERE", "UNIQUE", "REFERENCES", "OPTIONS", "DIRECTION", "UP", 
		"DOWN", "TRANSFER_TIME", "MIN", "HOUR", "DAY", "SESSION", "EVERY", "TRANSFER", 
		"POINT", "JOIN", "VALUE", "FROM", "INCLUDE", "EXCLUDE", "LOAD", "BEFORE", 
		"SAVE", "EVENTS", "CREATE", "DELETE", "UPDATE", "INVOKE", "CASCADE", "ERROR", 
		"BEGIN_INVOKE", "END_INVOKE", "REL_TYPE", "RELATIONID", "LeftBracket", 
		"RightBracket", "LeftSquadBracket", "RightSquadBracket", "Dollar", "ID", 
		"NUMBER", "COMMENT"
	};
	public static final Vocabulary VOCABULARY = new VocabularyImpl(_LITERAL_NAMES, _SYMBOLIC_NAMES);

	/**
	 * @deprecated Use {@link #VOCABULARY} instead.
	 */
	@Deprecated
	public static final String[] tokenNames;
	static {
		tokenNames = new String[_SYMBOLIC_NAMES.length];
		for (int i = 0; i < tokenNames.length; i++) {
			tokenNames[i] = VOCABULARY.getLiteralName(i);
			if (tokenNames[i] == null) {
				tokenNames[i] = VOCABULARY.getSymbolicName(i);
			}

			if (tokenNames[i] == null) {
				tokenNames[i] = "<INVALID>";
			}
		}
	}

	@Override
	@Deprecated
	public String[] getTokenNames() {
		return tokenNames;
	}

	@Override

	public Vocabulary getVocabulary() {
		return VOCABULARY;
	}

	@Override
	public String getGrammarFileName() { return "DbObjects.g4"; }

	@Override
	public String[] getRuleNames() { return ruleNames; }

	@Override
	public String getSerializedATN() { return _serializedATN; }

	@Override
	public ATN getATN() { return _ATN; }

	public DbObjectsParser(TokenStream input) {
		super(input);
		_interp = new ParserATNSimulator(this,_ATN,_decisionToDFA,_sharedContextCache);
	}
	public static class EvalContext extends ParserRuleContext {
		public List<DefinitionsContext> definitions() {
			return getRuleContexts(DefinitionsContext.class);
		}
		public DefinitionsContext definitions(int i) {
			return getRuleContext(DefinitionsContext.class,i);
		}
		public EvalContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_eval; }
	}

	public final EvalContext eval() throws RecognitionException {
		EvalContext _localctx = new EvalContext(_ctx, getState());
		enterRule(_localctx, 0, RULE_eval);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(103); 
			_errHandler.sync(this);
			_la = _input.LA(1);
			do {
				{
				{
				setState(102);
				definitions();
				}
				}
				setState(105); 
				_errHandler.sync(this);
				_la = _input.LA(1);
			} while ( _la==DEFINE );
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class DefinitionsContext extends ParserRuleContext {
		public ObjectDefinitionTableContext objectDefinitionTable() {
			return getRuleContext(ObjectDefinitionTableContext.class,0);
		}
		public ObjectDefinitionObjectContext objectDefinitionObject() {
			return getRuleContext(ObjectDefinitionObjectContext.class,0);
		}
		public PointDefinitionContext pointDefinition() {
			return getRuleContext(PointDefinitionContext.class,0);
		}
		public DefinitionsContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_definitions; }
	}

	public final DefinitionsContext definitions() throws RecognitionException {
		DefinitionsContext _localctx = new DefinitionsContext(_ctx, getState());
		enterRule(_localctx, 2, RULE_definitions);
		try {
			setState(110);
			_errHandler.sync(this);
			switch ( getInterpreter().adaptivePredict(_input,1,_ctx) ) {
			case 1:
				enterOuterAlt(_localctx, 1);
				{
				setState(107);
				objectDefinitionTable();
				}
				break;
			case 2:
				enterOuterAlt(_localctx, 2);
				{
				setState(108);
				objectDefinitionObject();
				}
				break;
			case 3:
				enterOuterAlt(_localctx, 3);
				{
				setState(109);
				pointDefinition();
				}
				break;
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class PointDefinitionContext extends ParserRuleContext {
		public TerminalNode DEFINE() { return getToken(DbObjectsParser.DEFINE, 0); }
		public TerminalNode POINT() { return getToken(DbObjectsParser.POINT, 0); }
		public PointNameContext pointName() {
			return getRuleContext(PointNameContext.class,0);
		}
		public TerminalNode FROM() { return getToken(DbObjectsParser.FROM, 0); }
		public TableNameContext tableName() {
			return getRuleContext(TableNameContext.class,0);
		}
		public TerminalNode ON() { return getToken(DbObjectsParser.ON, 0); }
		public OnFieldNameContext onFieldName() {
			return getRuleContext(OnFieldNameContext.class,0);
		}
		public TerminalNode VALUE() { return getToken(DbObjectsParser.VALUE, 0); }
		public ValueFieldNameContext valueFieldName() {
			return getRuleContext(ValueFieldNameContext.class,0);
		}
		public PointDefinitionContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_pointDefinition; }
	}

	public final PointDefinitionContext pointDefinition() throws RecognitionException {
		PointDefinitionContext _localctx = new PointDefinitionContext(_ctx, getState());
		enterRule(_localctx, 4, RULE_pointDefinition);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(112);
			match(DEFINE);
			setState(113);
			match(POINT);
			setState(114);
			pointName();
			setState(115);
			match(FROM);
			setState(116);
			tableName();
			setState(117);
			match(ON);
			setState(118);
			onFieldName();
			setState(119);
			match(VALUE);
			setState(120);
			valueFieldName();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class ObjectDefinitionTableContext extends ParserRuleContext {
		public TerminalNode DEFINE() { return getToken(DbObjectsParser.DEFINE, 0); }
		public TerminalNode OBJECT() { return getToken(DbObjectsParser.OBJECT, 0); }
		public ObjectDefinitionNameContext objectDefinitionName() {
			return getRuleContext(ObjectDefinitionNameContext.class,0);
		}
		public TerminalNode AS() { return getToken(DbObjectsParser.AS, 0); }
		public TerminalNode TABLE() { return getToken(DbObjectsParser.TABLE, 0); }
		public TableExpressionContext tableExpression() {
			return getRuleContext(TableExpressionContext.class,0);
		}
		public TerminalNode TRANSFER() { return getToken(DbObjectsParser.TRANSFER, 0); }
		public List<WhereExpressionContext> whereExpression() {
			return getRuleContexts(WhereExpressionContext.class);
		}
		public WhereExpressionContext whereExpression(int i) {
			return getRuleContext(WhereExpressionContext.class,i);
		}
		public ObjectEventsContext objectEvents() {
			return getRuleContext(ObjectEventsContext.class,0);
		}
		public ObjectOptionsContext objectOptions() {
			return getRuleContext(ObjectOptionsContext.class,0);
		}
		public ObjectDefinitionTableContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_objectDefinitionTable; }
	}

	public final ObjectDefinitionTableContext objectDefinitionTable() throws RecognitionException {
		ObjectDefinitionTableContext _localctx = new ObjectDefinitionTableContext(_ctx, getState());
		enterRule(_localctx, 6, RULE_objectDefinitionTable);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(122);
			match(DEFINE);
			setState(124);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==TRANSFER) {
				{
				setState(123);
				match(TRANSFER);
				}
			}

			setState(126);
			match(OBJECT);
			setState(127);
			objectDefinitionName();
			setState(128);
			match(AS);
			setState(129);
			match(TABLE);
			setState(130);
			tableExpression();
			setState(134);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while (_la==WHERE) {
				{
				{
				setState(131);
				whereExpression();
				}
				}
				setState(136);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			setState(138);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==EVENTS) {
				{
				setState(137);
				objectEvents();
				}
			}

			setState(141);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==OPTIONS) {
				{
				setState(140);
				objectOptions();
				}
			}

			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class ObjectDefinitionObjectContext extends ParserRuleContext {
		public TerminalNode DEFINE() { return getToken(DbObjectsParser.DEFINE, 0); }
		public List<TerminalNode> OBJECT() { return getTokens(DbObjectsParser.OBJECT); }
		public TerminalNode OBJECT(int i) {
			return getToken(DbObjectsParser.OBJECT, i);
		}
		public ObjectDefinitionNameContext objectDefinitionName() {
			return getRuleContext(ObjectDefinitionNameContext.class,0);
		}
		public TerminalNode AS() { return getToken(DbObjectsParser.AS, 0); }
		public ObjectNameContext objectName() {
			return getRuleContext(ObjectNameContext.class,0);
		}
		public ObjectRelationsContext objectRelations() {
			return getRuleContext(ObjectRelationsContext.class,0);
		}
		public TerminalNode TRANSFER() { return getToken(DbObjectsParser.TRANSFER, 0); }
		public List<TerminalNode> LeftBracket() { return getTokens(DbObjectsParser.LeftBracket); }
		public TerminalNode LeftBracket(int i) {
			return getToken(DbObjectsParser.LeftBracket, i);
		}
		public List<TerminalNode> RightBracket() { return getTokens(DbObjectsParser.RightBracket); }
		public TerminalNode RightBracket(int i) {
			return getToken(DbObjectsParser.RightBracket, i);
		}
		public ObjectEventsContext objectEvents() {
			return getRuleContext(ObjectEventsContext.class,0);
		}
		public ObjectOptionsContext objectOptions() {
			return getRuleContext(ObjectOptionsContext.class,0);
		}
		public ObjectPointsContext objectPoints() {
			return getRuleContext(ObjectPointsContext.class,0);
		}
		public ObjectDefinitionObjectContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_objectDefinitionObject; }
	}

	public final ObjectDefinitionObjectContext objectDefinitionObject() throws RecognitionException {
		ObjectDefinitionObjectContext _localctx = new ObjectDefinitionObjectContext(_ctx, getState());
		enterRule(_localctx, 8, RULE_objectDefinitionObject);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(143);
			match(DEFINE);
			setState(145);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==TRANSFER) {
				{
				setState(144);
				match(TRANSFER);
				}
			}

			setState(147);
			match(OBJECT);
			setState(148);
			objectDefinitionName();
			setState(149);
			match(AS);
			setState(150);
			match(OBJECT);
			setState(154);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while (_la==LeftBracket) {
				{
				{
				setState(151);
				match(LeftBracket);
				}
				}
				setState(156);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			setState(157);
			objectName();
			setState(161);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while (_la==RightBracket) {
				{
				{
				setState(158);
				match(RightBracket);
				}
				}
				setState(163);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			setState(164);
			objectRelations();
			setState(166);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==EVENTS) {
				{
				setState(165);
				objectEvents();
				}
			}

			setState(169);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==OPTIONS) {
				{
				setState(168);
				objectOptions();
				}
			}

			setState(172);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==POINT) {
				{
				setState(171);
				objectPoints();
				}
			}

			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class TableExpressionContext extends ParserRuleContext {
		public TableNameContext tableName() {
			return getRuleContext(TableNameContext.class,0);
		}
		public TerminalNode LeftBracket() { return getToken(DbObjectsParser.LeftBracket, 0); }
		public TerminalNode RightBracket() { return getToken(DbObjectsParser.RightBracket, 0); }
		public UniqueExpressionContext uniqueExpression() {
			return getRuleContext(UniqueExpressionContext.class,0);
		}
		public List<IncludeExpressionContext> includeExpression() {
			return getRuleContexts(IncludeExpressionContext.class);
		}
		public IncludeExpressionContext includeExpression(int i) {
			return getRuleContext(IncludeExpressionContext.class,i);
		}
		public List<ExcludeExpressionContext> excludeExpression() {
			return getRuleContexts(ExcludeExpressionContext.class);
		}
		public ExcludeExpressionContext excludeExpression(int i) {
			return getRuleContext(ExcludeExpressionContext.class,i);
		}
		public List<PointExpressionContext> pointExpression() {
			return getRuleContexts(PointExpressionContext.class);
		}
		public PointExpressionContext pointExpression(int i) {
			return getRuleContext(PointExpressionContext.class,i);
		}
		public List<TerminalNode> Comma() { return getTokens(DbObjectsParser.Comma); }
		public TerminalNode Comma(int i) {
			return getToken(DbObjectsParser.Comma, i);
		}
		public TableExpressionContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_tableExpression; }
	}

	public final TableExpressionContext tableExpression() throws RecognitionException {
		TableExpressionContext _localctx = new TableExpressionContext(_ctx, getState());
		enterRule(_localctx, 10, RULE_tableExpression);
		int _la;
		try {
			int _alt;
			enterOuterAlt(_localctx, 1);
			{
			setState(174);
			tableName();
			setState(175);
			match(LeftBracket);
			setState(187);
			_errHandler.sync(this);
			_alt = getInterpreter().adaptivePredict(_input,13,_ctx);
			while ( _alt!=2 && _alt!=org.antlr.v4.runtime.atn.ATN.INVALID_ALT_NUMBER ) {
				if ( _alt==1 ) {
					{
					setState(185);
					_errHandler.sync(this);
					switch ( getInterpreter().adaptivePredict(_input,12,_ctx) ) {
					case 1:
						{
						setState(176);
						includeExpression();
						}
						break;
					case 2:
						{
						setState(177);
						excludeExpression();
						}
						break;
					case 3:
						{
						setState(178);
						pointExpression();
						}
						break;
					case 4:
						{
						setState(179);
						match(Comma);
						setState(180);
						pointExpression();
						}
						break;
					case 5:
						{
						setState(181);
						match(Comma);
						setState(182);
						includeExpression();
						}
						break;
					case 6:
						{
						setState(183);
						match(Comma);
						setState(184);
						excludeExpression();
						}
						break;
					}
					} 
				}
				setState(189);
				_errHandler.sync(this);
				_alt = getInterpreter().adaptivePredict(_input,13,_ctx);
			}
			setState(196);
			_errHandler.sync(this);
			switch ( getInterpreter().adaptivePredict(_input,14,_ctx) ) {
			case 1:
				{
				setState(190);
				uniqueExpression();
				}
				break;
			case 2:
				{
				{
				setState(191);
				match(Comma);
				setState(192);
				uniqueExpression();
				}
				}
				break;
			case 3:
				{
				{
				setState(193);
				uniqueExpression();
				setState(194);
				match(Comma);
				}
				}
				break;
			}
			setState(209);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while ((((_la) & ~0x3f) == 0 && ((1L << _la) & ((1L << Comma) | (1L << POINT) | (1L << INCLUDE) | (1L << EXCLUDE))) != 0)) {
				{
				setState(207);
				_errHandler.sync(this);
				switch ( getInterpreter().adaptivePredict(_input,15,_ctx) ) {
				case 1:
					{
					setState(198);
					includeExpression();
					}
					break;
				case 2:
					{
					setState(199);
					excludeExpression();
					}
					break;
				case 3:
					{
					setState(200);
					pointExpression();
					}
					break;
				case 4:
					{
					setState(201);
					match(Comma);
					setState(202);
					pointExpression();
					}
					break;
				case 5:
					{
					setState(203);
					match(Comma);
					setState(204);
					includeExpression();
					}
					break;
				case 6:
					{
					setState(205);
					match(Comma);
					setState(206);
					excludeExpression();
					}
					break;
				}
				}
				setState(211);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			setState(212);
			match(RightBracket);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class UniqueExpressionContext extends ParserRuleContext {
		public TerminalNode UNIQUE() { return getToken(DbObjectsParser.UNIQUE, 0); }
		public TerminalNode LeftBracket() { return getToken(DbObjectsParser.LeftBracket, 0); }
		public TerminalNode RightBracket() { return getToken(DbObjectsParser.RightBracket, 0); }
		public List<ColumnListContext> columnList() {
			return getRuleContexts(ColumnListContext.class);
		}
		public ColumnListContext columnList(int i) {
			return getRuleContext(ColumnListContext.class,i);
		}
		public UniqueExpressionContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_uniqueExpression; }
	}

	public final UniqueExpressionContext uniqueExpression() throws RecognitionException {
		UniqueExpressionContext _localctx = new UniqueExpressionContext(_ctx, getState());
		enterRule(_localctx, 12, RULE_uniqueExpression);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(214);
			match(UNIQUE);
			setState(215);
			match(LeftBracket);
			setState(219);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while (_la==ID) {
				{
				{
				setState(216);
				columnList();
				}
				}
				setState(221);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			setState(222);
			match(RightBracket);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class IncludeExpressionContext extends ParserRuleContext {
		public TerminalNode INCLUDE() { return getToken(DbObjectsParser.INCLUDE, 0); }
		public TerminalNode LeftBracket() { return getToken(DbObjectsParser.LeftBracket, 0); }
		public TerminalNode RightBracket() { return getToken(DbObjectsParser.RightBracket, 0); }
		public List<ColumnListContext> columnList() {
			return getRuleContexts(ColumnListContext.class);
		}
		public ColumnListContext columnList(int i) {
			return getRuleContext(ColumnListContext.class,i);
		}
		public IncludeExpressionContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_includeExpression; }
	}

	public final IncludeExpressionContext includeExpression() throws RecognitionException {
		IncludeExpressionContext _localctx = new IncludeExpressionContext(_ctx, getState());
		enterRule(_localctx, 14, RULE_includeExpression);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(224);
			match(INCLUDE);
			setState(225);
			match(LeftBracket);
			setState(229);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while (_la==ID) {
				{
				{
				setState(226);
				columnList();
				}
				}
				setState(231);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			setState(232);
			match(RightBracket);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class ExcludeExpressionContext extends ParserRuleContext {
		public TerminalNode EXCLUDE() { return getToken(DbObjectsParser.EXCLUDE, 0); }
		public TerminalNode LeftBracket() { return getToken(DbObjectsParser.LeftBracket, 0); }
		public TerminalNode RightBracket() { return getToken(DbObjectsParser.RightBracket, 0); }
		public List<ColumnListContext> columnList() {
			return getRuleContexts(ColumnListContext.class);
		}
		public ColumnListContext columnList(int i) {
			return getRuleContext(ColumnListContext.class,i);
		}
		public ExcludeExpressionContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_excludeExpression; }
	}

	public final ExcludeExpressionContext excludeExpression() throws RecognitionException {
		ExcludeExpressionContext _localctx = new ExcludeExpressionContext(_ctx, getState());
		enterRule(_localctx, 16, RULE_excludeExpression);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(234);
			match(EXCLUDE);
			setState(235);
			match(LeftBracket);
			setState(239);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while (_la==ID) {
				{
				{
				setState(236);
				columnList();
				}
				}
				setState(241);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			setState(242);
			match(RightBracket);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class PointExpressionContext extends ParserRuleContext {
		public TerminalNode POINT() { return getToken(DbObjectsParser.POINT, 0); }
		public TerminalNode LeftBracket() { return getToken(DbObjectsParser.LeftBracket, 0); }
		public PointNameContext pointName() {
			return getRuleContext(PointNameContext.class,0);
		}
		public TerminalNode Comma() { return getToken(DbObjectsParser.Comma, 0); }
		public FieldNameContext fieldName() {
			return getRuleContext(FieldNameContext.class,0);
		}
		public TerminalNode RightBracket() { return getToken(DbObjectsParser.RightBracket, 0); }
		public PointExpressionContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_pointExpression; }
	}

	public final PointExpressionContext pointExpression() throws RecognitionException {
		PointExpressionContext _localctx = new PointExpressionContext(_ctx, getState());
		enterRule(_localctx, 18, RULE_pointExpression);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(244);
			match(POINT);
			setState(245);
			match(LeftBracket);
			setState(246);
			pointName();
			setState(247);
			match(Comma);
			setState(248);
			fieldName();
			setState(249);
			match(RightBracket);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class ObjectPointsContext extends ParserRuleContext {
		public TerminalNode POINT() { return getToken(DbObjectsParser.POINT, 0); }
		public TerminalNode LeftBracket() { return getToken(DbObjectsParser.LeftBracket, 0); }
		public PointNameContext pointName() {
			return getRuleContext(PointNameContext.class,0);
		}
		public TerminalNode Comma() { return getToken(DbObjectsParser.Comma, 0); }
		public FieldExpressionContext fieldExpression() {
			return getRuleContext(FieldExpressionContext.class,0);
		}
		public TerminalNode RightBracket() { return getToken(DbObjectsParser.RightBracket, 0); }
		public ObjectPointsContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_objectPoints; }
	}

	public final ObjectPointsContext objectPoints() throws RecognitionException {
		ObjectPointsContext _localctx = new ObjectPointsContext(_ctx, getState());
		enterRule(_localctx, 20, RULE_objectPoints);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(251);
			match(POINT);
			setState(252);
			match(LeftBracket);
			setState(253);
			pointName();
			setState(254);
			match(Comma);
			setState(255);
			fieldExpression();
			setState(256);
			match(RightBracket);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class ObjectRelationsContext extends ParserRuleContext {
		public TerminalNode REFERENCES() { return getToken(DbObjectsParser.REFERENCES, 0); }
		public List<ObjectRelationContext> objectRelation() {
			return getRuleContexts(ObjectRelationContext.class);
		}
		public ObjectRelationContext objectRelation(int i) {
			return getRuleContext(ObjectRelationContext.class,i);
		}
		public ObjectRelationsContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_objectRelations; }
	}

	public final ObjectRelationsContext objectRelations() throws RecognitionException {
		ObjectRelationsContext _localctx = new ObjectRelationsContext(_ctx, getState());
		enterRule(_localctx, 22, RULE_objectRelations);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(258);
			match(REFERENCES);
			setState(260); 
			_errHandler.sync(this);
			_la = _input.LA(1);
			do {
				{
				{
				setState(259);
				objectRelation();
				}
				}
				setState(262); 
				_errHandler.sync(this);
				_la = _input.LA(1);
			} while ( _la==RELATIONID );
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class ObjectRelationContext extends ParserRuleContext {
		public TerminalNode RELATIONID() { return getToken(DbObjectsParser.RELATIONID, 0); }
		public TerminalNode REL_TYPE() { return getToken(DbObjectsParser.REL_TYPE, 0); }
		public TerminalNode DoubleDot() { return getToken(DbObjectsParser.DoubleDot, 0); }
		public TerminalNode ID() { return getToken(DbObjectsParser.ID, 0); }
		public TerminalNode ON() { return getToken(DbObjectsParser.ON, 0); }
		public List<JoinContext> join() {
			return getRuleContexts(JoinContext.class);
		}
		public JoinContext join(int i) {
			return getRuleContext(JoinContext.class,i);
		}
		public ObjectRelationContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_objectRelation; }
	}

	public final ObjectRelationContext objectRelation() throws RecognitionException {
		ObjectRelationContext _localctx = new ObjectRelationContext(_ctx, getState());
		enterRule(_localctx, 24, RULE_objectRelation);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(264);
			match(RELATIONID);
			setState(265);
			match(REL_TYPE);
			setState(266);
			match(DoubleDot);
			setState(267);
			match(ID);
			setState(268);
			match(ON);
			setState(270); 
			_errHandler.sync(this);
			_la = _input.LA(1);
			do {
				{
				{
				setState(269);
				join();
				}
				}
				setState(272); 
				_errHandler.sync(this);
				_la = _input.LA(1);
			} while ( _la==AND || _la==ID );
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class ObjectEventsContext extends ParserRuleContext {
		public TerminalNode EVENTS() { return getToken(DbObjectsParser.EVENTS, 0); }
		public List<ObjectEventsTypesContext> objectEventsTypes() {
			return getRuleContexts(ObjectEventsTypesContext.class);
		}
		public ObjectEventsTypesContext objectEventsTypes(int i) {
			return getRuleContext(ObjectEventsTypesContext.class,i);
		}
		public ObjectEventsContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_objectEvents; }
	}

	public final ObjectEventsContext objectEvents() throws RecognitionException {
		ObjectEventsContext _localctx = new ObjectEventsContext(_ctx, getState());
		enterRule(_localctx, 26, RULE_objectEvents);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(274);
			match(EVENTS);
			setState(278);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while ((((_la) & ~0x3f) == 0 && ((1L << _la) & ((1L << ON) | (1L << AFTER) | (1L << BEFORE))) != 0)) {
				{
				{
				setState(275);
				objectEventsTypes();
				}
				}
				setState(280);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class ObjectEventsTypesContext extends ParserRuleContext {
		public ObjectOnEventContext objectOnEvent() {
			return getRuleContext(ObjectOnEventContext.class,0);
		}
		public ObjectAfterEventContext objectAfterEvent() {
			return getRuleContext(ObjectAfterEventContext.class,0);
		}
		public ObjectBeforeEventContext objectBeforeEvent() {
			return getRuleContext(ObjectBeforeEventContext.class,0);
		}
		public ObjectEventsTypesContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_objectEventsTypes; }
	}

	public final ObjectEventsTypesContext objectEventsTypes() throws RecognitionException {
		ObjectEventsTypesContext _localctx = new ObjectEventsTypesContext(_ctx, getState());
		enterRule(_localctx, 28, RULE_objectEventsTypes);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(284);
			_errHandler.sync(this);
			switch (_input.LA(1)) {
			case ON:
				{
				setState(281);
				objectOnEvent();
				}
				break;
			case AFTER:
				{
				setState(282);
				objectAfterEvent();
				}
				break;
			case BEFORE:
				{
				setState(283);
				objectBeforeEvent();
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class ObjectBeforeEventContext extends ParserRuleContext {
		public TerminalNode BEFORE() { return getToken(DbObjectsParser.BEFORE, 0); }
		public TerminalNode SAVE() { return getToken(DbObjectsParser.SAVE, 0); }
		public TerminalNode DoubleDot() { return getToken(DbObjectsParser.DoubleDot, 0); }
		public List<BeforeSaveStatementContext> beforeSaveStatement() {
			return getRuleContexts(BeforeSaveStatementContext.class);
		}
		public BeforeSaveStatementContext beforeSaveStatement(int i) {
			return getRuleContext(BeforeSaveStatementContext.class,i);
		}
		public ObjectBeforeEventContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_objectBeforeEvent; }
	}

	public final ObjectBeforeEventContext objectBeforeEvent() throws RecognitionException {
		ObjectBeforeEventContext _localctx = new ObjectBeforeEventContext(_ctx, getState());
		enterRule(_localctx, 30, RULE_objectBeforeEvent);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(286);
			match(BEFORE);
			{
			setState(287);
			match(SAVE);
			setState(288);
			match(DoubleDot);
			setState(292);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while (_la==BEGIN_INVOKE) {
				{
				{
				setState(289);
				beforeSaveStatement();
				}
				}
				setState(294);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class ObjectOnEventContext extends ParserRuleContext {
		public TerminalNode ON() { return getToken(DbObjectsParser.ON, 0); }
		public TerminalNode UPDATE() { return getToken(DbObjectsParser.UPDATE, 0); }
		public TerminalNode DoubleDot() { return getToken(DbObjectsParser.DoubleDot, 0); }
		public TerminalNode CREATE() { return getToken(DbObjectsParser.CREATE, 0); }
		public TerminalNode DELETE() { return getToken(DbObjectsParser.DELETE, 0); }
		public TerminalNode ERROR() { return getToken(DbObjectsParser.ERROR, 0); }
		public List<OnUpdateStatementContext> onUpdateStatement() {
			return getRuleContexts(OnUpdateStatementContext.class);
		}
		public OnUpdateStatementContext onUpdateStatement(int i) {
			return getRuleContext(OnUpdateStatementContext.class,i);
		}
		public List<OnCreateStatementContext> onCreateStatement() {
			return getRuleContexts(OnCreateStatementContext.class);
		}
		public OnCreateStatementContext onCreateStatement(int i) {
			return getRuleContext(OnCreateStatementContext.class,i);
		}
		public List<OnDeleteStatementContext> onDeleteStatement() {
			return getRuleContexts(OnDeleteStatementContext.class);
		}
		public OnDeleteStatementContext onDeleteStatement(int i) {
			return getRuleContext(OnDeleteStatementContext.class,i);
		}
		public List<OnErrorStatementContext> onErrorStatement() {
			return getRuleContexts(OnErrorStatementContext.class);
		}
		public OnErrorStatementContext onErrorStatement(int i) {
			return getRuleContext(OnErrorStatementContext.class,i);
		}
		public ObjectOnEventContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_objectOnEvent; }
	}

	public final ObjectOnEventContext objectOnEvent() throws RecognitionException {
		ObjectOnEventContext _localctx = new ObjectOnEventContext(_ctx, getState());
		enterRule(_localctx, 32, RULE_objectOnEvent);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(295);
			match(ON);
			setState(328);
			_errHandler.sync(this);
			switch (_input.LA(1)) {
			case UPDATE:
				{
				setState(296);
				match(UPDATE);
				setState(297);
				match(DoubleDot);
				setState(301);
				_errHandler.sync(this);
				_la = _input.LA(1);
				while (_la==UPDATE || _la==BEGIN_INVOKE) {
					{
					{
					setState(298);
					onUpdateStatement();
					}
					}
					setState(303);
					_errHandler.sync(this);
					_la = _input.LA(1);
				}
				}
				break;
			case CREATE:
				{
				setState(304);
				match(CREATE);
				setState(305);
				match(DoubleDot);
				setState(309);
				_errHandler.sync(this);
				_la = _input.LA(1);
				while (_la==BEGIN_INVOKE) {
					{
					{
					setState(306);
					onCreateStatement();
					}
					}
					setState(311);
					_errHandler.sync(this);
					_la = _input.LA(1);
				}
				}
				break;
			case DELETE:
				{
				setState(312);
				match(DELETE);
				setState(313);
				match(DoubleDot);
				setState(317);
				_errHandler.sync(this);
				_la = _input.LA(1);
				while (_la==DELETE || _la==BEGIN_INVOKE) {
					{
					{
					setState(314);
					onDeleteStatement();
					}
					}
					setState(319);
					_errHandler.sync(this);
					_la = _input.LA(1);
				}
				}
				break;
			case ERROR:
				{
				setState(320);
				match(ERROR);
				setState(321);
				match(DoubleDot);
				setState(325);
				_errHandler.sync(this);
				_la = _input.LA(1);
				while (_la==BEGIN_INVOKE) {
					{
					{
					setState(322);
					onErrorStatement();
					}
					}
					setState(327);
					_errHandler.sync(this);
					_la = _input.LA(1);
				}
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class ObjectAfterEventContext extends ParserRuleContext {
		public TerminalNode AFTER() { return getToken(DbObjectsParser.AFTER, 0); }
		public TerminalNode LOAD() { return getToken(DbObjectsParser.LOAD, 0); }
		public TerminalNode DoubleDot() { return getToken(DbObjectsParser.DoubleDot, 0); }
		public TerminalNode UPDATE() { return getToken(DbObjectsParser.UPDATE, 0); }
		public TerminalNode CREATE() { return getToken(DbObjectsParser.CREATE, 0); }
		public TerminalNode DELETE() { return getToken(DbObjectsParser.DELETE, 0); }
		public List<AfterLoadStatementContext> afterLoadStatement() {
			return getRuleContexts(AfterLoadStatementContext.class);
		}
		public AfterLoadStatementContext afterLoadStatement(int i) {
			return getRuleContext(AfterLoadStatementContext.class,i);
		}
		public List<AfterUpdateStatementContext> afterUpdateStatement() {
			return getRuleContexts(AfterUpdateStatementContext.class);
		}
		public AfterUpdateStatementContext afterUpdateStatement(int i) {
			return getRuleContext(AfterUpdateStatementContext.class,i);
		}
		public List<AfterCreateStatementContext> afterCreateStatement() {
			return getRuleContexts(AfterCreateStatementContext.class);
		}
		public AfterCreateStatementContext afterCreateStatement(int i) {
			return getRuleContext(AfterCreateStatementContext.class,i);
		}
		public List<AfterDeleteStatementContext> afterDeleteStatement() {
			return getRuleContexts(AfterDeleteStatementContext.class);
		}
		public AfterDeleteStatementContext afterDeleteStatement(int i) {
			return getRuleContext(AfterDeleteStatementContext.class,i);
		}
		public ObjectAfterEventContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_objectAfterEvent; }
	}

	public final ObjectAfterEventContext objectAfterEvent() throws RecognitionException {
		ObjectAfterEventContext _localctx = new ObjectAfterEventContext(_ctx, getState());
		enterRule(_localctx, 34, RULE_objectAfterEvent);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(330);
			match(AFTER);
			setState(363);
			_errHandler.sync(this);
			switch (_input.LA(1)) {
			case LOAD:
				{
				setState(331);
				match(LOAD);
				setState(332);
				match(DoubleDot);
				setState(336);
				_errHandler.sync(this);
				_la = _input.LA(1);
				while (_la==BEGIN_INVOKE) {
					{
					{
					setState(333);
					afterLoadStatement();
					}
					}
					setState(338);
					_errHandler.sync(this);
					_la = _input.LA(1);
				}
				}
				break;
			case UPDATE:
				{
				setState(339);
				match(UPDATE);
				setState(340);
				match(DoubleDot);
				setState(344);
				_errHandler.sync(this);
				_la = _input.LA(1);
				while (_la==BEGIN_INVOKE) {
					{
					{
					setState(341);
					afterUpdateStatement();
					}
					}
					setState(346);
					_errHandler.sync(this);
					_la = _input.LA(1);
				}
				}
				break;
			case CREATE:
				{
				setState(347);
				match(CREATE);
				setState(348);
				match(DoubleDot);
				setState(352);
				_errHandler.sync(this);
				_la = _input.LA(1);
				while (_la==BEGIN_INVOKE) {
					{
					{
					setState(349);
					afterCreateStatement();
					}
					}
					setState(354);
					_errHandler.sync(this);
					_la = _input.LA(1);
				}
				}
				break;
			case DELETE:
				{
				setState(355);
				match(DELETE);
				setState(356);
				match(DoubleDot);
				setState(360);
				_errHandler.sync(this);
				_la = _input.LA(1);
				while (_la==BEGIN_INVOKE) {
					{
					{
					setState(357);
					afterDeleteStatement();
					}
					}
					setState(362);
					_errHandler.sync(this);
					_la = _input.LA(1);
				}
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class AfterLoadStatementContext extends ParserRuleContext {
		public InvokeStatementContext invokeStatement() {
			return getRuleContext(InvokeStatementContext.class,0);
		}
		public AfterLoadStatementContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_afterLoadStatement; }
	}

	public final AfterLoadStatementContext afterLoadStatement() throws RecognitionException {
		AfterLoadStatementContext _localctx = new AfterLoadStatementContext(_ctx, getState());
		enterRule(_localctx, 36, RULE_afterLoadStatement);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(365);
			invokeStatement();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class BeforeSaveStatementContext extends ParserRuleContext {
		public InvokeStatementContext invokeStatement() {
			return getRuleContext(InvokeStatementContext.class,0);
		}
		public BeforeSaveStatementContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_beforeSaveStatement; }
	}

	public final BeforeSaveStatementContext beforeSaveStatement() throws RecognitionException {
		BeforeSaveStatementContext _localctx = new BeforeSaveStatementContext(_ctx, getState());
		enterRule(_localctx, 38, RULE_beforeSaveStatement);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(367);
			invokeStatement();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class AfterUpdateStatementContext extends ParserRuleContext {
		public InvokeStatementContext invokeStatement() {
			return getRuleContext(InvokeStatementContext.class,0);
		}
		public AfterUpdateStatementContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_afterUpdateStatement; }
	}

	public final AfterUpdateStatementContext afterUpdateStatement() throws RecognitionException {
		AfterUpdateStatementContext _localctx = new AfterUpdateStatementContext(_ctx, getState());
		enterRule(_localctx, 40, RULE_afterUpdateStatement);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(369);
			invokeStatement();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class AfterCreateStatementContext extends ParserRuleContext {
		public InvokeStatementContext invokeStatement() {
			return getRuleContext(InvokeStatementContext.class,0);
		}
		public AfterCreateStatementContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_afterCreateStatement; }
	}

	public final AfterCreateStatementContext afterCreateStatement() throws RecognitionException {
		AfterCreateStatementContext _localctx = new AfterCreateStatementContext(_ctx, getState());
		enterRule(_localctx, 42, RULE_afterCreateStatement);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(371);
			invokeStatement();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class AfterDeleteStatementContext extends ParserRuleContext {
		public InvokeStatementContext invokeStatement() {
			return getRuleContext(InvokeStatementContext.class,0);
		}
		public AfterDeleteStatementContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_afterDeleteStatement; }
	}

	public final AfterDeleteStatementContext afterDeleteStatement() throws RecognitionException {
		AfterDeleteStatementContext _localctx = new AfterDeleteStatementContext(_ctx, getState());
		enterRule(_localctx, 44, RULE_afterDeleteStatement);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(373);
			invokeStatement();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class OnErrorStatementContext extends ParserRuleContext {
		public InvokeStatementContext invokeStatement() {
			return getRuleContext(InvokeStatementContext.class,0);
		}
		public OnErrorStatementContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_onErrorStatement; }
	}

	public final OnErrorStatementContext onErrorStatement() throws RecognitionException {
		OnErrorStatementContext _localctx = new OnErrorStatementContext(_ctx, getState());
		enterRule(_localctx, 46, RULE_onErrorStatement);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(375);
			invokeStatement();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class OnUpdateStatementContext extends ParserRuleContext {
		public UpdateAllReferencesStatementContext updateAllReferencesStatement() {
			return getRuleContext(UpdateAllReferencesStatementContext.class,0);
		}
		public InvokeStatementContext invokeStatement() {
			return getRuleContext(InvokeStatementContext.class,0);
		}
		public OnUpdateStatementContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_onUpdateStatement; }
	}

	public final OnUpdateStatementContext onUpdateStatement() throws RecognitionException {
		OnUpdateStatementContext _localctx = new OnUpdateStatementContext(_ctx, getState());
		enterRule(_localctx, 48, RULE_onUpdateStatement);
		try {
			setState(379);
			_errHandler.sync(this);
			switch (_input.LA(1)) {
			case UPDATE:
				enterOuterAlt(_localctx, 1);
				{
				setState(377);
				updateAllReferencesStatement();
				}
				break;
			case BEGIN_INVOKE:
				enterOuterAlt(_localctx, 2);
				{
				setState(378);
				invokeStatement();
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class OnCreateStatementContext extends ParserRuleContext {
		public InvokeStatementContext invokeStatement() {
			return getRuleContext(InvokeStatementContext.class,0);
		}
		public OnCreateStatementContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_onCreateStatement; }
	}

	public final OnCreateStatementContext onCreateStatement() throws RecognitionException {
		OnCreateStatementContext _localctx = new OnCreateStatementContext(_ctx, getState());
		enterRule(_localctx, 50, RULE_onCreateStatement);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(381);
			invokeStatement();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class OnDeleteStatementContext extends ParserRuleContext {
		public DeleteCascadeStatementContext deleteCascadeStatement() {
			return getRuleContext(DeleteCascadeStatementContext.class,0);
		}
		public InvokeStatementContext invokeStatement() {
			return getRuleContext(InvokeStatementContext.class,0);
		}
		public OnDeleteStatementContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_onDeleteStatement; }
	}

	public final OnDeleteStatementContext onDeleteStatement() throws RecognitionException {
		OnDeleteStatementContext _localctx = new OnDeleteStatementContext(_ctx, getState());
		enterRule(_localctx, 52, RULE_onDeleteStatement);
		try {
			setState(385);
			_errHandler.sync(this);
			switch (_input.LA(1)) {
			case DELETE:
				enterOuterAlt(_localctx, 1);
				{
				setState(383);
				deleteCascadeStatement();
				}
				break;
			case BEGIN_INVOKE:
				enterOuterAlt(_localctx, 2);
				{
				setState(384);
				invokeStatement();
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class UpdateAllReferencesStatementContext extends ParserRuleContext {
		public TerminalNode UPDATE() { return getToken(DbObjectsParser.UPDATE, 0); }
		public TerminalNode ALL() { return getToken(DbObjectsParser.ALL, 0); }
		public TerminalNode REFERENCES() { return getToken(DbObjectsParser.REFERENCES, 0); }
		public TerminalNode SEMICOLON() { return getToken(DbObjectsParser.SEMICOLON, 0); }
		public UpdateAllReferencesStatementContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_updateAllReferencesStatement; }
	}

	public final UpdateAllReferencesStatementContext updateAllReferencesStatement() throws RecognitionException {
		UpdateAllReferencesStatementContext _localctx = new UpdateAllReferencesStatementContext(_ctx, getState());
		enterRule(_localctx, 54, RULE_updateAllReferencesStatement);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(387);
			match(UPDATE);
			setState(388);
			match(ALL);
			setState(389);
			match(REFERENCES);
			setState(390);
			match(SEMICOLON);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class DeleteCascadeStatementContext extends ParserRuleContext {
		public TerminalNode DELETE() { return getToken(DbObjectsParser.DELETE, 0); }
		public TerminalNode CASCADE() { return getToken(DbObjectsParser.CASCADE, 0); }
		public TerminalNode SEMICOLON() { return getToken(DbObjectsParser.SEMICOLON, 0); }
		public DeleteCascadeStatementContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_deleteCascadeStatement; }
	}

	public final DeleteCascadeStatementContext deleteCascadeStatement() throws RecognitionException {
		DeleteCascadeStatementContext _localctx = new DeleteCascadeStatementContext(_ctx, getState());
		enterRule(_localctx, 56, RULE_deleteCascadeStatement);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(392);
			match(DELETE);
			setState(393);
			match(CASCADE);
			setState(394);
			match(SEMICOLON);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class InvokeStatementContext extends ParserRuleContext {
		public InvokeBeginStatementContext invokeBeginStatement() {
			return getRuleContext(InvokeBeginStatementContext.class,0);
		}
		public InvokeBodyStatementContext invokeBodyStatement() {
			return getRuleContext(InvokeBodyStatementContext.class,0);
		}
		public InvokeEndStatementContext invokeEndStatement() {
			return getRuleContext(InvokeEndStatementContext.class,0);
		}
		public TerminalNode SEMICOLON() { return getToken(DbObjectsParser.SEMICOLON, 0); }
		public InvokeStatementContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_invokeStatement; }
	}

	public final InvokeStatementContext invokeStatement() throws RecognitionException {
		InvokeStatementContext _localctx = new InvokeStatementContext(_ctx, getState());
		enterRule(_localctx, 58, RULE_invokeStatement);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(396);
			invokeBeginStatement();
			setState(397);
			invokeBodyStatement();
			setState(398);
			invokeEndStatement();
			setState(399);
			match(SEMICOLON);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class InvokeBodyStatementContext extends ParserRuleContext {
		public List<TerminalNode> END_INVOKE() { return getTokens(DbObjectsParser.END_INVOKE); }
		public TerminalNode END_INVOKE(int i) {
			return getToken(DbObjectsParser.END_INVOKE, i);
		}
		public InvokeBodyStatementContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_invokeBodyStatement; }
	}

	public final InvokeBodyStatementContext invokeBodyStatement() throws RecognitionException {
		InvokeBodyStatementContext _localctx = new InvokeBodyStatementContext(_ctx, getState());
		enterRule(_localctx, 60, RULE_invokeBodyStatement);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(404);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while ((((_la) & ~0x3f) == 0 && ((1L << _la) & ((1L << EQUALS_SYMBOL) | (1L << MORE_THAN) | (1L << LESS_THAN) | (1L << NOT_EQUALS) | (1L << MINUS) | (1L << IN) | (1L << EQUALS) | (1L << NEWLINE) | (1L << SPACE) | (1L << TABS) | (1L << POINTER) | (1L << SEMICOLON) | (1L << DOUBLE_QUOTION) | (1L << QUOTION) | (1L << EXCLAMATION) | (1L << STAR) | (1L << AT) | (1L << Comma) | (1L << Dot) | (1L << DoubleDot) | (1L << ALL) | (1L << DEFINE) | (1L << OBJECT) | (1L << AS) | (1L << TABLE) | (1L << AND) | (1L << ON) | (1L << AFTER) | (1L << REL) | (1L << WITH) | (1L << TYPE) | (1L << WHERE) | (1L << UNIQUE) | (1L << REFERENCES) | (1L << OPTIONS) | (1L << DIRECTION) | (1L << UP) | (1L << DOWN) | (1L << TRANSFER_TIME) | (1L << MIN) | (1L << HOUR) | (1L << DAY) | (1L << SESSION) | (1L << EVERY) | (1L << TRANSFER) | (1L << POINT) | (1L << JOIN) | (1L << VALUE) | (1L << FROM) | (1L << INCLUDE) | (1L << EXCLUDE) | (1L << LOAD) | (1L << BEFORE) | (1L << SAVE) | (1L << EVENTS) | (1L << CREATE) | (1L << DELETE) | (1L << UPDATE) | (1L << INVOKE) | (1L << CASCADE) | (1L << ERROR) | (1L << BEGIN_INVOKE))) != 0) || ((((_la - 64)) & ~0x3f) == 0 && ((1L << (_la - 64)) & ((1L << (REL_TYPE - 64)) | (1L << (RELATIONID - 64)) | (1L << (LeftBracket - 64)) | (1L << (RightBracket - 64)) | (1L << (LeftSquadBracket - 64)) | (1L << (RightSquadBracket - 64)) | (1L << (Dollar - 64)) | (1L << (ID - 64)) | (1L << (NUMBER - 64)) | (1L << (COMMENT - 64)))) != 0)) {
				{
				{
				setState(401);
				_la = _input.LA(1);
				if ( _la <= 0 || (_la==END_INVOKE) ) {
				_errHandler.recoverInline(this);
				}
				else {
					if ( _input.LA(1)==Token.EOF ) matchedEOF = true;
					_errHandler.reportMatch(this);
					consume();
				}
				}
				}
				setState(406);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class InvokeBeginStatementContext extends ParserRuleContext {
		public TerminalNode BEGIN_INVOKE() { return getToken(DbObjectsParser.BEGIN_INVOKE, 0); }
		public InvokeBeginStatementContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_invokeBeginStatement; }
	}

	public final InvokeBeginStatementContext invokeBeginStatement() throws RecognitionException {
		InvokeBeginStatementContext _localctx = new InvokeBeginStatementContext(_ctx, getState());
		enterRule(_localctx, 62, RULE_invokeBeginStatement);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(407);
			match(BEGIN_INVOKE);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class InvokeEndStatementContext extends ParserRuleContext {
		public TerminalNode END_INVOKE() { return getToken(DbObjectsParser.END_INVOKE, 0); }
		public InvokeEndStatementContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_invokeEndStatement; }
	}

	public final InvokeEndStatementContext invokeEndStatement() throws RecognitionException {
		InvokeEndStatementContext _localctx = new InvokeEndStatementContext(_ctx, getState());
		enterRule(_localctx, 64, RULE_invokeEndStatement);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(409);
			match(END_INVOKE);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class ObjectOptionsContext extends ParserRuleContext {
		public TerminalNode OPTIONS() { return getToken(DbObjectsParser.OPTIONS, 0); }
		public DirectionOptionsContext directionOptions() {
			return getRuleContext(DirectionOptionsContext.class,0);
		}
		public TransferTimeOptionsContext transferTimeOptions() {
			return getRuleContext(TransferTimeOptionsContext.class,0);
		}
		public ObjectOptionsContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_objectOptions; }
	}

	public final ObjectOptionsContext objectOptions() throws RecognitionException {
		ObjectOptionsContext _localctx = new ObjectOptionsContext(_ctx, getState());
		enterRule(_localctx, 66, RULE_objectOptions);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(411);
			match(OPTIONS);
			setState(413);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==DIRECTION) {
				{
				setState(412);
				directionOptions();
				}
			}

			setState(416);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==TRANSFER_TIME || _la==SESSION) {
				{
				setState(415);
				transferTimeOptions();
				}
			}

			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class DirectionOptionsContext extends ParserRuleContext {
		public TerminalNode DIRECTION() { return getToken(DbObjectsParser.DIRECTION, 0); }
		public TerminalNode UP() { return getToken(DbObjectsParser.UP, 0); }
		public TerminalNode DOWN() { return getToken(DbObjectsParser.DOWN, 0); }
		public DirectionOptionsContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_directionOptions; }
	}

	public final DirectionOptionsContext directionOptions() throws RecognitionException {
		DirectionOptionsContext _localctx = new DirectionOptionsContext(_ctx, getState());
		enterRule(_localctx, 68, RULE_directionOptions);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(418);
			match(DIRECTION);
			setState(419);
			_la = _input.LA(1);
			if ( !(_la==UP || _la==DOWN) ) {
			_errHandler.recoverInline(this);
			}
			else {
				if ( _input.LA(1)==Token.EOF ) matchedEOF = true;
				_errHandler.reportMatch(this);
				consume();
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class TransferTimeOptionsContext extends ParserRuleContext {
		public TerminalNode TRANSFER_TIME() { return getToken(DbObjectsParser.TRANSFER_TIME, 0); }
		public TerminalNode EVERY() { return getToken(DbObjectsParser.EVERY, 0); }
		public TerminalNode NUMBER() { return getToken(DbObjectsParser.NUMBER, 0); }
		public TerminalNode MIN() { return getToken(DbObjectsParser.MIN, 0); }
		public TerminalNode HOUR() { return getToken(DbObjectsParser.HOUR, 0); }
		public TerminalNode DAY() { return getToken(DbObjectsParser.DAY, 0); }
		public TerminalNode SESSION() { return getToken(DbObjectsParser.SESSION, 0); }
		public TransferTimeOptionsContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_transferTimeOptions; }
	}

	public final TransferTimeOptionsContext transferTimeOptions() throws RecognitionException {
		TransferTimeOptionsContext _localctx = new TransferTimeOptionsContext(_ctx, getState());
		enterRule(_localctx, 70, RULE_transferTimeOptions);
		try {
			setState(430);
			_errHandler.sync(this);
			switch (_input.LA(1)) {
			case TRANSFER_TIME:
				enterOuterAlt(_localctx, 1);
				{
				setState(421);
				match(TRANSFER_TIME);
				setState(422);
				match(EVERY);
				setState(427);
				_errHandler.sync(this);
				switch (_input.LA(1)) {
				case NUMBER:
					{
					setState(423);
					match(NUMBER);
					setState(424);
					match(MIN);
					}
					break;
				case HOUR:
					{
					setState(425);
					match(HOUR);
					}
					break;
				case DAY:
					{
					setState(426);
					match(DAY);
					}
					break;
				default:
					throw new NoViableAltException(this);
				}
				}
				break;
			case SESSION:
				enterOuterAlt(_localctx, 2);
				{
				setState(429);
				match(SESSION);
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class JoinContext extends ParserRuleContext {
		public List<FieldExpressionContext> fieldExpression() {
			return getRuleContexts(FieldExpressionContext.class);
		}
		public FieldExpressionContext fieldExpression(int i) {
			return getRuleContext(FieldExpressionContext.class,i);
		}
		public CompareOperatorExpressionContext compareOperatorExpression() {
			return getRuleContext(CompareOperatorExpressionContext.class,0);
		}
		public TerminalNode AND() { return getToken(DbObjectsParser.AND, 0); }
		public JoinContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_join; }
	}

	public final JoinContext join() throws RecognitionException {
		JoinContext _localctx = new JoinContext(_ctx, getState());
		enterRule(_localctx, 72, RULE_join);
		try {
			setState(441);
			_errHandler.sync(this);
			switch (_input.LA(1)) {
			case ID:
				enterOuterAlt(_localctx, 1);
				{
				setState(432);
				fieldExpression();
				setState(433);
				compareOperatorExpression();
				setState(434);
				fieldExpression();
				}
				break;
			case AND:
				enterOuterAlt(_localctx, 2);
				{
				setState(436);
				match(AND);
				setState(437);
				fieldExpression();
				setState(438);
				compareOperatorExpression();
				setState(439);
				fieldExpression();
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class FieldExpressionContext extends ParserRuleContext {
		public TableNameContext tableName() {
			return getRuleContext(TableNameContext.class,0);
		}
		public TerminalNode Dot() { return getToken(DbObjectsParser.Dot, 0); }
		public FieldNameContext fieldName() {
			return getRuleContext(FieldNameContext.class,0);
		}
		public FieldExpressionContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_fieldExpression; }
	}

	public final FieldExpressionContext fieldExpression() throws RecognitionException {
		FieldExpressionContext _localctx = new FieldExpressionContext(_ctx, getState());
		enterRule(_localctx, 74, RULE_fieldExpression);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(443);
			tableName();
			setState(444);
			match(Dot);
			setState(445);
			fieldName();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class WhereExpressionContext extends ParserRuleContext {
		public TerminalNode WHERE() { return getToken(DbObjectsParser.WHERE, 0); }
		public FieldExpressionContext fieldExpression() {
			return getRuleContext(FieldExpressionContext.class,0);
		}
		public CompareOperatorExpressionContext compareOperatorExpression() {
			return getRuleContext(CompareOperatorExpressionContext.class,0);
		}
		public ValueExpressionContext valueExpression() {
			return getRuleContext(ValueExpressionContext.class,0);
		}
		public WhereExpressionContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_whereExpression; }
	}

	public final WhereExpressionContext whereExpression() throws RecognitionException {
		WhereExpressionContext _localctx = new WhereExpressionContext(_ctx, getState());
		enterRule(_localctx, 76, RULE_whereExpression);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(447);
			match(WHERE);
			setState(448);
			fieldExpression();
			setState(449);
			compareOperatorExpression();
			setState(450);
			valueExpression();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class CompareOperatorExpressionContext extends ParserRuleContext {
		public TerminalNode EQUALS() { return getToken(DbObjectsParser.EQUALS, 0); }
		public TerminalNode IN() { return getToken(DbObjectsParser.IN, 0); }
		public CompareOperatorExpressionContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_compareOperatorExpression; }
	}

	public final CompareOperatorExpressionContext compareOperatorExpression() throws RecognitionException {
		CompareOperatorExpressionContext _localctx = new CompareOperatorExpressionContext(_ctx, getState());
		enterRule(_localctx, 78, RULE_compareOperatorExpression);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(452);
			_la = _input.LA(1);
			if ( !(_la==IN || _la==EQUALS) ) {
			_errHandler.recoverInline(this);
			}
			else {
				if ( _input.LA(1)==Token.EOF ) matchedEOF = true;
				_errHandler.reportMatch(this);
				consume();
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class ValueExpressionContext extends ParserRuleContext {
		public TerminalNode NUMBER() { return getToken(DbObjectsParser.NUMBER, 0); }
		public NumberArrayExpressionContext numberArrayExpression() {
			return getRuleContext(NumberArrayExpressionContext.class,0);
		}
		public ValueExpressionContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_valueExpression; }
	}

	public final ValueExpressionContext valueExpression() throws RecognitionException {
		ValueExpressionContext _localctx = new ValueExpressionContext(_ctx, getState());
		enterRule(_localctx, 80, RULE_valueExpression);
		try {
			setState(456);
			_errHandler.sync(this);
			switch (_input.LA(1)) {
			case NUMBER:
				enterOuterAlt(_localctx, 1);
				{
				setState(454);
				match(NUMBER);
				}
				break;
			case LeftBracket:
				enterOuterAlt(_localctx, 2);
				{
				setState(455);
				numberArrayExpression();
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class NumberArrayExpressionContext extends ParserRuleContext {
		public TerminalNode LeftBracket() { return getToken(DbObjectsParser.LeftBracket, 0); }
		public TerminalNode RightBracket() { return getToken(DbObjectsParser.RightBracket, 0); }
		public List<TerminalNode> NUMBER() { return getTokens(DbObjectsParser.NUMBER); }
		public TerminalNode NUMBER(int i) {
			return getToken(DbObjectsParser.NUMBER, i);
		}
		public List<TerminalNode> Comma() { return getTokens(DbObjectsParser.Comma); }
		public TerminalNode Comma(int i) {
			return getToken(DbObjectsParser.Comma, i);
		}
		public NumberArrayExpressionContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_numberArrayExpression; }
	}

	public final NumberArrayExpressionContext numberArrayExpression() throws RecognitionException {
		NumberArrayExpressionContext _localctx = new NumberArrayExpressionContext(_ctx, getState());
		enterRule(_localctx, 82, RULE_numberArrayExpression);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(458);
			match(LeftBracket);
			setState(462); 
			_errHandler.sync(this);
			_la = _input.LA(1);
			do {
				{
				setState(462);
				_errHandler.sync(this);
				switch ( getInterpreter().adaptivePredict(_input,44,_ctx) ) {
				case 1:
					{
					setState(459);
					match(NUMBER);
					}
					break;
				case 2:
					{
					setState(460);
					match(NUMBER);
					setState(461);
					match(Comma);
					}
					break;
				}
				}
				setState(464); 
				_errHandler.sync(this);
				_la = _input.LA(1);
			} while ( _la==NUMBER );
			setState(466);
			match(RightBracket);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class ColumnListContext extends ParserRuleContext {
		public TerminalNode ID() { return getToken(DbObjectsParser.ID, 0); }
		public TerminalNode Comma() { return getToken(DbObjectsParser.Comma, 0); }
		public TerminalNode UNIQUE() { return getToken(DbObjectsParser.UNIQUE, 0); }
		public TerminalNode POINT() { return getToken(DbObjectsParser.POINT, 0); }
		public TerminalNode LeftBracket() { return getToken(DbObjectsParser.LeftBracket, 0); }
		public PointNameContext pointName() {
			return getRuleContext(PointNameContext.class,0);
		}
		public TerminalNode RightBracket() { return getToken(DbObjectsParser.RightBracket, 0); }
		public ColumnListContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_columnList; }
	}

	public final ColumnListContext columnList() throws RecognitionException {
		ColumnListContext _localctx = new ColumnListContext(_ctx, getState());
		enterRule(_localctx, 84, RULE_columnList);
		int _la;
		try {
			setState(484);
			_errHandler.sync(this);
			switch ( getInterpreter().adaptivePredict(_input,48,_ctx) ) {
			case 1:
				enterOuterAlt(_localctx, 1);
				{
				setState(468);
				match(ID);
				}
				break;
			case 2:
				enterOuterAlt(_localctx, 2);
				{
				setState(469);
				match(ID);
				setState(470);
				match(Comma);
				}
				break;
			case 3:
				enterOuterAlt(_localctx, 3);
				{
				setState(471);
				match(ID);
				setState(472);
				match(UNIQUE);
				setState(474);
				_errHandler.sync(this);
				_la = _input.LA(1);
				if (_la==Comma) {
					{
					setState(473);
					match(Comma);
					}
				}

				}
				break;
			case 4:
				enterOuterAlt(_localctx, 4);
				{
				setState(476);
				match(ID);
				setState(477);
				match(POINT);
				setState(478);
				match(LeftBracket);
				setState(479);
				pointName();
				setState(480);
				match(RightBracket);
				setState(482);
				_errHandler.sync(this);
				_la = _input.LA(1);
				if (_la==Comma) {
					{
					setState(481);
					match(Comma);
					}
				}

				}
				break;
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class TableNameContext extends ParserRuleContext {
		public TerminalNode ID() { return getToken(DbObjectsParser.ID, 0); }
		public TableNameContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_tableName; }
	}

	public final TableNameContext tableName() throws RecognitionException {
		TableNameContext _localctx = new TableNameContext(_ctx, getState());
		enterRule(_localctx, 86, RULE_tableName);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(486);
			match(ID);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class FieldNameContext extends ParserRuleContext {
		public TerminalNode ID() { return getToken(DbObjectsParser.ID, 0); }
		public FieldNameContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_fieldName; }
	}

	public final FieldNameContext fieldName() throws RecognitionException {
		FieldNameContext _localctx = new FieldNameContext(_ctx, getState());
		enterRule(_localctx, 88, RULE_fieldName);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(488);
			match(ID);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class OnFieldNameContext extends ParserRuleContext {
		public TerminalNode ID() { return getToken(DbObjectsParser.ID, 0); }
		public OnFieldNameContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_onFieldName; }
	}

	public final OnFieldNameContext onFieldName() throws RecognitionException {
		OnFieldNameContext _localctx = new OnFieldNameContext(_ctx, getState());
		enterRule(_localctx, 90, RULE_onFieldName);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(490);
			match(ID);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class ValueFieldNameContext extends ParserRuleContext {
		public TerminalNode ID() { return getToken(DbObjectsParser.ID, 0); }
		public ValueFieldNameContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_valueFieldName; }
	}

	public final ValueFieldNameContext valueFieldName() throws RecognitionException {
		ValueFieldNameContext _localctx = new ValueFieldNameContext(_ctx, getState());
		enterRule(_localctx, 92, RULE_valueFieldName);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(492);
			match(ID);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class ObjectListContext extends ParserRuleContext {
		public List<ObjectNameContext> objectName() {
			return getRuleContexts(ObjectNameContext.class);
		}
		public ObjectNameContext objectName(int i) {
			return getRuleContext(ObjectNameContext.class,i);
		}
		public List<TerminalNode> Comma() { return getTokens(DbObjectsParser.Comma); }
		public TerminalNode Comma(int i) {
			return getToken(DbObjectsParser.Comma, i);
		}
		public ObjectListContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_objectList; }
	}

	public final ObjectListContext objectList() throws RecognitionException {
		ObjectListContext _localctx = new ObjectListContext(_ctx, getState());
		enterRule(_localctx, 94, RULE_objectList);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(498); 
			_errHandler.sync(this);
			_la = _input.LA(1);
			do {
				{
				{
				setState(494);
				objectName();
				setState(496);
				_errHandler.sync(this);
				_la = _input.LA(1);
				if (_la==Comma) {
					{
					setState(495);
					match(Comma);
					}
				}

				}
				}
				setState(500); 
				_errHandler.sync(this);
				_la = _input.LA(1);
			} while ( _la==ID );
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class ObjectDefinitionNameContext extends ParserRuleContext {
		public TerminalNode ID() { return getToken(DbObjectsParser.ID, 0); }
		public ObjectDefinitionNameContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_objectDefinitionName; }
	}

	public final ObjectDefinitionNameContext objectDefinitionName() throws RecognitionException {
		ObjectDefinitionNameContext _localctx = new ObjectDefinitionNameContext(_ctx, getState());
		enterRule(_localctx, 96, RULE_objectDefinitionName);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(502);
			match(ID);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class ObjectNameContext extends ParserRuleContext {
		public TerminalNode ID() { return getToken(DbObjectsParser.ID, 0); }
		public ObjectNameContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_objectName; }
	}

	public final ObjectNameContext objectName() throws RecognitionException {
		ObjectNameContext _localctx = new ObjectNameContext(_ctx, getState());
		enterRule(_localctx, 98, RULE_objectName);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(504);
			match(ID);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class PointNameContext extends ParserRuleContext {
		public TerminalNode ID() { return getToken(DbObjectsParser.ID, 0); }
		public PointNameContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_pointName; }
	}

	public final PointNameContext pointName() throws RecognitionException {
		PointNameContext _localctx = new PointNameContext(_ctx, getState());
		enterRule(_localctx, 100, RULE_pointName);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(506);
			match(ID);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static final String _serializedATN =
		"\3\u608b\ua72a\u8133\ub9ed\u417c\u3be7\u7786\u5964\3K\u01ff\4\2\t\2\4"+
		"\3\t\3\4\4\t\4\4\5\t\5\4\6\t\6\4\7\t\7\4\b\t\b\4\t\t\t\4\n\t\n\4\13\t"+
		"\13\4\f\t\f\4\r\t\r\4\16\t\16\4\17\t\17\4\20\t\20\4\21\t\21\4\22\t\22"+
		"\4\23\t\23\4\24\t\24\4\25\t\25\4\26\t\26\4\27\t\27\4\30\t\30\4\31\t\31"+
		"\4\32\t\32\4\33\t\33\4\34\t\34\4\35\t\35\4\36\t\36\4\37\t\37\4 \t \4!"+
		"\t!\4\"\t\"\4#\t#\4$\t$\4%\t%\4&\t&\4\'\t\'\4(\t(\4)\t)\4*\t*\4+\t+\4"+
		",\t,\4-\t-\4.\t.\4/\t/\4\60\t\60\4\61\t\61\4\62\t\62\4\63\t\63\4\64\t"+
		"\64\3\2\6\2j\n\2\r\2\16\2k\3\3\3\3\3\3\5\3q\n\3\3\4\3\4\3\4\3\4\3\4\3"+
		"\4\3\4\3\4\3\4\3\4\3\5\3\5\5\5\177\n\5\3\5\3\5\3\5\3\5\3\5\3\5\7\5\u0087"+
		"\n\5\f\5\16\5\u008a\13\5\3\5\5\5\u008d\n\5\3\5\5\5\u0090\n\5\3\6\3\6\5"+
		"\6\u0094\n\6\3\6\3\6\3\6\3\6\3\6\7\6\u009b\n\6\f\6\16\6\u009e\13\6\3\6"+
		"\3\6\7\6\u00a2\n\6\f\6\16\6\u00a5\13\6\3\6\3\6\5\6\u00a9\n\6\3\6\5\6\u00ac"+
		"\n\6\3\6\5\6\u00af\n\6\3\7\3\7\3\7\3\7\3\7\3\7\3\7\3\7\3\7\3\7\3\7\7\7"+
		"\u00bc\n\7\f\7\16\7\u00bf\13\7\3\7\3\7\3\7\3\7\3\7\3\7\5\7\u00c7\n\7\3"+
		"\7\3\7\3\7\3\7\3\7\3\7\3\7\3\7\3\7\7\7\u00d2\n\7\f\7\16\7\u00d5\13\7\3"+
		"\7\3\7\3\b\3\b\3\b\7\b\u00dc\n\b\f\b\16\b\u00df\13\b\3\b\3\b\3\t\3\t\3"+
		"\t\7\t\u00e6\n\t\f\t\16\t\u00e9\13\t\3\t\3\t\3\n\3\n\3\n\7\n\u00f0\n\n"+
		"\f\n\16\n\u00f3\13\n\3\n\3\n\3\13\3\13\3\13\3\13\3\13\3\13\3\13\3\f\3"+
		"\f\3\f\3\f\3\f\3\f\3\f\3\r\3\r\6\r\u0107\n\r\r\r\16\r\u0108\3\16\3\16"+
		"\3\16\3\16\3\16\3\16\6\16\u0111\n\16\r\16\16\16\u0112\3\17\3\17\7\17\u0117"+
		"\n\17\f\17\16\17\u011a\13\17\3\20\3\20\3\20\5\20\u011f\n\20\3\21\3\21"+
		"\3\21\3\21\7\21\u0125\n\21\f\21\16\21\u0128\13\21\3\22\3\22\3\22\3\22"+
		"\7\22\u012e\n\22\f\22\16\22\u0131\13\22\3\22\3\22\3\22\7\22\u0136\n\22"+
		"\f\22\16\22\u0139\13\22\3\22\3\22\3\22\7\22\u013e\n\22\f\22\16\22\u0141"+
		"\13\22\3\22\3\22\3\22\7\22\u0146\n\22\f\22\16\22\u0149\13\22\5\22\u014b"+
		"\n\22\3\23\3\23\3\23\3\23\7\23\u0151\n\23\f\23\16\23\u0154\13\23\3\23"+
		"\3\23\3\23\7\23\u0159\n\23\f\23\16\23\u015c\13\23\3\23\3\23\3\23\7\23"+
		"\u0161\n\23\f\23\16\23\u0164\13\23\3\23\3\23\3\23\7\23\u0169\n\23\f\23"+
		"\16\23\u016c\13\23\5\23\u016e\n\23\3\24\3\24\3\25\3\25\3\26\3\26\3\27"+
		"\3\27\3\30\3\30\3\31\3\31\3\32\3\32\5\32\u017e\n\32\3\33\3\33\3\34\3\34"+
		"\5\34\u0184\n\34\3\35\3\35\3\35\3\35\3\35\3\36\3\36\3\36\3\36\3\37\3\37"+
		"\3\37\3\37\3\37\3 \7 \u0195\n \f \16 \u0198\13 \3!\3!\3\"\3\"\3#\3#\5"+
		"#\u01a0\n#\3#\5#\u01a3\n#\3$\3$\3$\3%\3%\3%\3%\3%\3%\5%\u01ae\n%\3%\5"+
		"%\u01b1\n%\3&\3&\3&\3&\3&\3&\3&\3&\3&\5&\u01bc\n&\3\'\3\'\3\'\3\'\3(\3"+
		"(\3(\3(\3(\3)\3)\3*\3*\5*\u01cb\n*\3+\3+\3+\3+\6+\u01d1\n+\r+\16+\u01d2"+
		"\3+\3+\3,\3,\3,\3,\3,\3,\5,\u01dd\n,\3,\3,\3,\3,\3,\3,\5,\u01e5\n,\5,"+
		"\u01e7\n,\3-\3-\3.\3.\3/\3/\3\60\3\60\3\61\3\61\5\61\u01f3\n\61\6\61\u01f5"+
		"\n\61\r\61\16\61\u01f6\3\62\3\62\3\63\3\63\3\64\3\64\3\64\2\2\65\2\4\6"+
		"\b\n\f\16\20\22\24\26\30\32\34\36 \"$&(*,.\60\62\64\668:<>@BDFHJLNPRT"+
		"VXZ\\^`bdf\2\5\3\2AA\3\2\'(\3\2\b\t\2\u0210\2i\3\2\2\2\4p\3\2\2\2\6r\3"+
		"\2\2\2\b|\3\2\2\2\n\u0091\3\2\2\2\f\u00b0\3\2\2\2\16\u00d8\3\2\2\2\20"+
		"\u00e2\3\2\2\2\22\u00ec\3\2\2\2\24\u00f6\3\2\2\2\26\u00fd\3\2\2\2\30\u0104"+
		"\3\2\2\2\32\u010a\3\2\2\2\34\u0114\3\2\2\2\36\u011e\3\2\2\2 \u0120\3\2"+
		"\2\2\"\u0129\3\2\2\2$\u014c\3\2\2\2&\u016f\3\2\2\2(\u0171\3\2\2\2*\u0173"+
		"\3\2\2\2,\u0175\3\2\2\2.\u0177\3\2\2\2\60\u0179\3\2\2\2\62\u017d\3\2\2"+
		"\2\64\u017f\3\2\2\2\66\u0183\3\2\2\28\u0185\3\2\2\2:\u018a\3\2\2\2<\u018e"+
		"\3\2\2\2>\u0196\3\2\2\2@\u0199\3\2\2\2B\u019b\3\2\2\2D\u019d\3\2\2\2F"+
		"\u01a4\3\2\2\2H\u01b0\3\2\2\2J\u01bb\3\2\2\2L\u01bd\3\2\2\2N\u01c1\3\2"+
		"\2\2P\u01c6\3\2\2\2R\u01ca\3\2\2\2T\u01cc\3\2\2\2V\u01e6\3\2\2\2X\u01e8"+
		"\3\2\2\2Z\u01ea\3\2\2\2\\\u01ec\3\2\2\2^\u01ee\3\2\2\2`\u01f4\3\2\2\2"+
		"b\u01f8\3\2\2\2d\u01fa\3\2\2\2f\u01fc\3\2\2\2hj\5\4\3\2ih\3\2\2\2jk\3"+
		"\2\2\2ki\3\2\2\2kl\3\2\2\2l\3\3\2\2\2mq\5\b\5\2nq\5\n\6\2oq\5\6\4\2pm"+
		"\3\2\2\2pn\3\2\2\2po\3\2\2\2q\5\3\2\2\2rs\7\30\2\2st\7\60\2\2tu\5f\64"+
		"\2uv\7\63\2\2vw\5X-\2wx\7\35\2\2xy\5\\/\2yz\7\62\2\2z{\5^\60\2{\7\3\2"+
		"\2\2|~\7\30\2\2}\177\7/\2\2~}\3\2\2\2~\177\3\2\2\2\177\u0080\3\2\2\2\u0080"+
		"\u0081\7\31\2\2\u0081\u0082\5b\62\2\u0082\u0083\7\32\2\2\u0083\u0084\7"+
		"\33\2\2\u0084\u0088\5\f\7\2\u0085\u0087\5N(\2\u0086\u0085\3\2\2\2\u0087"+
		"\u008a\3\2\2\2\u0088\u0086\3\2\2\2\u0088\u0089\3\2\2\2\u0089\u008c\3\2"+
		"\2\2\u008a\u0088\3\2\2\2\u008b\u008d\5\34\17\2\u008c\u008b\3\2\2\2\u008c"+
		"\u008d\3\2\2\2\u008d\u008f\3\2\2\2\u008e\u0090\5D#\2\u008f\u008e\3\2\2"+
		"\2\u008f\u0090\3\2\2\2\u0090\t\3\2\2\2\u0091\u0093\7\30\2\2\u0092\u0094"+
		"\7/\2\2\u0093\u0092\3\2\2\2\u0093\u0094\3\2\2\2\u0094\u0095\3\2\2\2\u0095"+
		"\u0096\7\31\2\2\u0096\u0097\5b\62\2\u0097\u0098\7\32\2\2\u0098\u009c\7"+
		"\31\2\2\u0099\u009b\7D\2\2\u009a\u0099\3\2\2\2\u009b\u009e\3\2\2\2\u009c"+
		"\u009a\3\2\2\2\u009c\u009d\3\2\2\2\u009d\u009f\3\2\2\2\u009e\u009c\3\2"+
		"\2\2\u009f\u00a3\5d\63\2\u00a0\u00a2\7E\2\2\u00a1\u00a0\3\2\2\2\u00a2"+
		"\u00a5\3\2\2\2\u00a3\u00a1\3\2\2\2\u00a3\u00a4\3\2\2\2\u00a4\u00a6\3\2"+
		"\2\2\u00a5\u00a3\3\2\2\2\u00a6\u00a8\5\30\r\2\u00a7\u00a9\5\34\17\2\u00a8"+
		"\u00a7\3\2\2\2\u00a8\u00a9\3\2\2\2\u00a9\u00ab\3\2\2\2\u00aa\u00ac\5D"+
		"#\2\u00ab\u00aa\3\2\2\2\u00ab\u00ac\3\2\2\2\u00ac\u00ae\3\2\2\2\u00ad"+
		"\u00af\5\26\f\2\u00ae\u00ad\3\2\2\2\u00ae\u00af\3\2\2\2\u00af\13\3\2\2"+
		"\2\u00b0\u00b1\5X-\2\u00b1\u00bd\7D\2\2\u00b2\u00bc\5\20\t\2\u00b3\u00bc"+
		"\5\22\n\2\u00b4\u00bc\5\24\13\2\u00b5\u00b6\7\24\2\2\u00b6\u00bc\5\24"+
		"\13\2\u00b7\u00b8\7\24\2\2\u00b8\u00bc\5\20\t\2\u00b9\u00ba\7\24\2\2\u00ba"+
		"\u00bc\5\22\n\2\u00bb\u00b2\3\2\2\2\u00bb\u00b3\3\2\2\2\u00bb\u00b4\3"+
		"\2\2\2\u00bb\u00b5\3\2\2\2\u00bb\u00b7\3\2\2\2\u00bb\u00b9\3\2\2\2\u00bc"+
		"\u00bf\3\2\2\2\u00bd\u00bb\3\2\2\2\u00bd\u00be\3\2\2\2\u00be\u00c6\3\2"+
		"\2\2\u00bf\u00bd\3\2\2\2\u00c0\u00c7\5\16\b\2\u00c1\u00c2\7\24\2\2\u00c2"+
		"\u00c7\5\16\b\2\u00c3\u00c4\5\16\b\2\u00c4\u00c5\7\24\2\2\u00c5\u00c7"+
		"\3\2\2\2\u00c6\u00c0\3\2\2\2\u00c6\u00c1\3\2\2\2\u00c6\u00c3\3\2\2\2\u00c7"+
		"\u00d3\3\2\2\2\u00c8\u00d2\5\20\t\2\u00c9\u00d2\5\22\n\2\u00ca\u00d2\5"+
		"\24\13\2\u00cb\u00cc\7\24\2\2\u00cc\u00d2\5\24\13\2\u00cd\u00ce\7\24\2"+
		"\2\u00ce\u00d2\5\20\t\2\u00cf\u00d0\7\24\2\2\u00d0\u00d2\5\22\n\2\u00d1"+
		"\u00c8\3\2\2\2\u00d1\u00c9\3\2\2\2\u00d1\u00ca\3\2\2\2\u00d1\u00cb\3\2"+
		"\2\2\u00d1\u00cd\3\2\2\2\u00d1\u00cf\3\2\2\2\u00d2\u00d5\3\2\2\2\u00d3"+
		"\u00d1\3\2\2\2\u00d3\u00d4\3\2\2\2\u00d4\u00d6\3\2\2\2\u00d5\u00d3\3\2"+
		"\2\2\u00d6\u00d7\7E\2\2\u00d7\r\3\2\2\2\u00d8\u00d9\7#\2\2\u00d9\u00dd"+
		"\7D\2\2\u00da\u00dc\5V,\2\u00db\u00da\3\2\2\2\u00dc\u00df\3\2\2\2\u00dd"+
		"\u00db\3\2\2\2\u00dd\u00de\3\2\2\2\u00de\u00e0\3\2\2\2\u00df\u00dd\3\2"+
		"\2\2\u00e0\u00e1\7E\2\2\u00e1\17\3\2\2\2\u00e2\u00e3\7\64\2\2\u00e3\u00e7"+
		"\7D\2\2\u00e4\u00e6\5V,\2\u00e5\u00e4\3\2\2\2\u00e6\u00e9\3\2\2\2\u00e7"+
		"\u00e5\3\2\2\2\u00e7\u00e8\3\2\2\2\u00e8\u00ea\3\2\2\2\u00e9\u00e7\3\2"+
		"\2\2\u00ea\u00eb\7E\2\2\u00eb\21\3\2\2\2\u00ec\u00ed\7\65\2\2\u00ed\u00f1"+
		"\7D\2\2\u00ee\u00f0\5V,\2\u00ef\u00ee\3\2\2\2\u00f0\u00f3\3\2\2\2\u00f1"+
		"\u00ef\3\2\2\2\u00f1\u00f2\3\2\2\2\u00f2\u00f4\3\2\2\2\u00f3\u00f1\3\2"+
		"\2\2\u00f4\u00f5\7E\2\2\u00f5\23\3\2\2\2\u00f6\u00f7\7\60\2\2\u00f7\u00f8"+
		"\7D\2\2\u00f8\u00f9\5f\64\2\u00f9\u00fa\7\24\2\2\u00fa\u00fb\5Z.\2\u00fb"+
		"\u00fc\7E\2\2\u00fc\25\3\2\2\2\u00fd\u00fe\7\60\2\2\u00fe\u00ff\7D\2\2"+
		"\u00ff\u0100\5f\64\2\u0100\u0101\7\24\2\2\u0101\u0102\5L\'\2\u0102\u0103"+
		"\7E\2\2\u0103\27\3\2\2\2\u0104\u0106\7$\2\2\u0105\u0107\5\32\16\2\u0106"+
		"\u0105\3\2\2\2\u0107\u0108\3\2\2\2\u0108\u0106\3\2\2\2\u0108\u0109\3\2"+
		"\2\2\u0109\31\3\2\2\2\u010a\u010b\7C\2\2\u010b\u010c\7B\2\2\u010c\u010d"+
		"\7\26\2\2\u010d\u010e\7I\2\2\u010e\u0110\7\35\2\2\u010f\u0111\5J&\2\u0110"+
		"\u010f\3\2\2\2\u0111\u0112\3\2\2\2\u0112\u0110\3\2\2\2\u0112\u0113\3\2"+
		"\2\2\u0113\33\3\2\2\2\u0114\u0118\79\2\2\u0115\u0117\5\36\20\2\u0116\u0115"+
		"\3\2\2\2\u0117\u011a\3\2\2\2\u0118\u0116\3\2\2\2\u0118\u0119\3\2\2\2\u0119"+
		"\35\3\2\2\2\u011a\u0118\3\2\2\2\u011b\u011f\5\"\22\2\u011c\u011f\5$\23"+
		"\2\u011d\u011f\5 \21\2\u011e\u011b\3\2\2\2\u011e\u011c\3\2\2\2\u011e\u011d"+
		"\3\2\2\2\u011f\37\3\2\2\2\u0120\u0121\7\67\2\2\u0121\u0122\78\2\2\u0122"+
		"\u0126\7\26\2\2\u0123\u0125\5(\25\2\u0124\u0123\3\2\2\2\u0125\u0128\3"+
		"\2\2\2\u0126\u0124\3\2\2\2\u0126\u0127\3\2\2\2\u0127!\3\2\2\2\u0128\u0126"+
		"\3\2\2\2\u0129\u014a\7\35\2\2\u012a\u012b\7<\2\2\u012b\u012f\7\26\2\2"+
		"\u012c\u012e\5\62\32\2\u012d\u012c\3\2\2\2\u012e\u0131\3\2\2\2\u012f\u012d"+
		"\3\2\2\2\u012f\u0130\3\2\2\2\u0130\u014b\3\2\2\2\u0131\u012f\3\2\2\2\u0132"+
		"\u0133\7:\2\2\u0133\u0137\7\26\2\2\u0134\u0136\5\64\33\2\u0135\u0134\3"+
		"\2\2\2\u0136\u0139\3\2\2\2\u0137\u0135\3\2\2\2\u0137\u0138\3\2\2\2\u0138"+
		"\u014b\3\2\2\2\u0139\u0137\3\2\2\2\u013a\u013b\7;\2\2\u013b\u013f\7\26"+
		"\2\2\u013c\u013e\5\66\34\2\u013d\u013c\3\2\2\2\u013e\u0141\3\2\2\2\u013f"+
		"\u013d\3\2\2\2\u013f\u0140\3\2\2\2\u0140\u014b\3\2\2\2\u0141\u013f\3\2"+
		"\2\2\u0142\u0143\7?\2\2\u0143\u0147\7\26\2\2\u0144\u0146\5\60\31\2\u0145"+
		"\u0144\3\2\2\2\u0146\u0149\3\2\2\2\u0147\u0145\3\2\2\2\u0147\u0148\3\2"+
		"\2\2\u0148\u014b\3\2\2\2\u0149\u0147\3\2\2\2\u014a\u012a\3\2\2\2\u014a"+
		"\u0132\3\2\2\2\u014a\u013a\3\2\2\2\u014a\u0142\3\2\2\2\u014b#\3\2\2\2"+
		"\u014c\u016d\7\36\2\2\u014d\u014e\7\66\2\2\u014e\u0152\7\26\2\2\u014f"+
		"\u0151\5&\24\2\u0150\u014f\3\2\2\2\u0151\u0154\3\2\2\2\u0152\u0150\3\2"+
		"\2\2\u0152\u0153\3\2\2\2\u0153\u016e\3\2\2\2\u0154\u0152\3\2\2\2\u0155"+
		"\u0156\7<\2\2\u0156\u015a\7\26\2\2\u0157\u0159\5*\26\2\u0158\u0157\3\2"+
		"\2\2\u0159\u015c\3\2\2\2\u015a\u0158\3\2\2\2\u015a\u015b\3\2\2\2\u015b"+
		"\u016e\3\2\2\2\u015c\u015a\3\2\2\2\u015d\u015e\7:\2\2\u015e\u0162\7\26"+
		"\2\2\u015f\u0161\5,\27\2\u0160\u015f\3\2\2\2\u0161\u0164\3\2\2\2\u0162"+
		"\u0160\3\2\2\2\u0162\u0163\3\2\2\2\u0163\u016e\3\2\2\2\u0164\u0162\3\2"+
		"\2\2\u0165\u0166\7;\2\2\u0166\u016a\7\26\2\2\u0167\u0169\5.\30\2\u0168"+
		"\u0167\3\2\2\2\u0169\u016c\3\2\2\2\u016a\u0168\3\2\2\2\u016a\u016b\3\2"+
		"\2\2\u016b\u016e\3\2\2\2\u016c\u016a\3\2\2\2\u016d\u014d\3\2\2\2\u016d"+
		"\u0155\3\2\2\2\u016d\u015d\3\2\2\2\u016d\u0165\3\2\2\2\u016e%\3\2\2\2"+
		"\u016f\u0170\5<\37\2\u0170\'\3\2\2\2\u0171\u0172\5<\37\2\u0172)\3\2\2"+
		"\2\u0173\u0174\5<\37\2\u0174+\3\2\2\2\u0175\u0176\5<\37\2\u0176-\3\2\2"+
		"\2\u0177\u0178\5<\37\2\u0178/\3\2\2\2\u0179\u017a\5<\37\2\u017a\61\3\2"+
		"\2\2\u017b\u017e\58\35\2\u017c\u017e\5<\37\2\u017d\u017b\3\2\2\2\u017d"+
		"\u017c\3\2\2\2\u017e\63\3\2\2\2\u017f\u0180\5<\37\2\u0180\65\3\2\2\2\u0181"+
		"\u0184\5:\36\2\u0182\u0184\5<\37\2\u0183\u0181\3\2\2\2\u0183\u0182\3\2"+
		"\2\2\u0184\67\3\2\2\2\u0185\u0186\7<\2\2\u0186\u0187\7\27\2\2\u0187\u0188"+
		"\7$\2\2\u0188\u0189\7\16\2\2\u01899\3\2\2\2\u018a\u018b\7;\2\2\u018b\u018c"+
		"\7>\2\2\u018c\u018d\7\16\2\2\u018d;\3\2\2\2\u018e\u018f\5@!\2\u018f\u0190"+
		"\5> \2\u0190\u0191\5B\"\2\u0191\u0192\7\16\2\2\u0192=\3\2\2\2\u0193\u0195"+
		"\n\2\2\2\u0194\u0193\3\2\2\2\u0195\u0198\3\2\2\2\u0196\u0194\3\2\2\2\u0196"+
		"\u0197\3\2\2\2\u0197?\3\2\2\2\u0198\u0196\3\2\2\2\u0199\u019a\7@\2\2\u019a"+
		"A\3\2\2\2\u019b\u019c\7A\2\2\u019cC\3\2\2\2\u019d\u019f\7%\2\2\u019e\u01a0"+
		"\5F$\2\u019f\u019e\3\2\2\2\u019f\u01a0\3\2\2\2\u01a0\u01a2\3\2\2\2\u01a1"+
		"\u01a3\5H%\2\u01a2\u01a1\3\2\2\2\u01a2\u01a3\3\2\2\2\u01a3E\3\2\2\2\u01a4"+
		"\u01a5\7&\2\2\u01a5\u01a6\t\3\2\2\u01a6G\3\2\2\2\u01a7\u01a8\7)\2\2\u01a8"+
		"\u01ad\7.\2\2\u01a9\u01aa\7J\2\2\u01aa\u01ae\7*\2\2\u01ab\u01ae\7+\2\2"+
		"\u01ac\u01ae\7,\2\2\u01ad\u01a9\3\2\2\2\u01ad\u01ab\3\2\2\2\u01ad\u01ac"+
		"\3\2\2\2\u01ae\u01b1\3\2\2\2\u01af\u01b1\7-\2\2\u01b0\u01a7\3\2\2\2\u01b0"+
		"\u01af\3\2\2\2\u01b1I\3\2\2\2\u01b2\u01b3\5L\'\2\u01b3\u01b4\5P)\2\u01b4"+
		"\u01b5\5L\'\2\u01b5\u01bc\3\2\2\2\u01b6\u01b7\7\34\2\2\u01b7\u01b8\5L"+
		"\'\2\u01b8\u01b9\5P)\2\u01b9\u01ba\5L\'\2\u01ba\u01bc\3\2\2\2\u01bb\u01b2"+
		"\3\2\2\2\u01bb\u01b6\3\2\2\2\u01bcK\3\2\2\2\u01bd\u01be\5X-\2\u01be\u01bf"+
		"\7\25\2\2\u01bf\u01c0\5Z.\2\u01c0M\3\2\2\2\u01c1\u01c2\7\"\2\2\u01c2\u01c3"+
		"\5L\'\2\u01c3\u01c4\5P)\2\u01c4\u01c5\5R*\2\u01c5O\3\2\2\2\u01c6\u01c7"+
		"\t\4\2\2\u01c7Q\3\2\2\2\u01c8\u01cb\7J\2\2\u01c9\u01cb\5T+\2\u01ca\u01c8"+
		"\3\2\2\2\u01ca\u01c9\3\2\2\2\u01cbS\3\2\2\2\u01cc\u01d0\7D\2\2\u01cd\u01d1"+
		"\7J\2\2\u01ce\u01cf\7J\2\2\u01cf\u01d1\7\24\2\2\u01d0\u01cd\3\2\2\2\u01d0"+
		"\u01ce\3\2\2\2\u01d1\u01d2\3\2\2\2\u01d2\u01d0\3\2\2\2\u01d2\u01d3\3\2"+
		"\2\2\u01d3\u01d4\3\2\2\2\u01d4\u01d5\7E\2\2\u01d5U\3\2\2\2\u01d6\u01e7"+
		"\7I\2\2\u01d7\u01d8\7I\2\2\u01d8\u01e7\7\24\2\2\u01d9\u01da\7I\2\2\u01da"+
		"\u01dc\7#\2\2\u01db\u01dd\7\24\2\2\u01dc\u01db\3\2\2\2\u01dc\u01dd\3\2"+
		"\2\2\u01dd\u01e7\3\2\2\2\u01de\u01df\7I\2\2\u01df\u01e0\7\60\2\2\u01e0"+
		"\u01e1\7D\2\2\u01e1\u01e2\5f\64\2\u01e2\u01e4\7E\2\2\u01e3\u01e5\7\24"+
		"\2\2\u01e4\u01e3\3\2\2\2\u01e4\u01e5\3\2\2\2\u01e5\u01e7\3\2\2\2\u01e6"+
		"\u01d6\3\2\2\2\u01e6\u01d7\3\2\2\2\u01e6\u01d9\3\2\2\2\u01e6\u01de\3\2"+
		"\2\2\u01e7W\3\2\2\2\u01e8\u01e9\7I\2\2\u01e9Y\3\2\2\2\u01ea\u01eb\7I\2"+
		"\2\u01eb[\3\2\2\2\u01ec\u01ed\7I\2\2\u01ed]\3\2\2\2\u01ee\u01ef\7I\2\2"+
		"\u01ef_\3\2\2\2\u01f0\u01f2\5d\63\2\u01f1\u01f3\7\24\2\2\u01f2\u01f1\3"+
		"\2\2\2\u01f2\u01f3\3\2\2\2\u01f3\u01f5\3\2\2\2\u01f4\u01f0\3\2\2\2\u01f5"+
		"\u01f6\3\2\2\2\u01f6\u01f4\3\2\2\2\u01f6\u01f7\3\2\2\2\u01f7a\3\2\2\2"+
		"\u01f8\u01f9\7I\2\2\u01f9c\3\2\2\2\u01fa\u01fb\7I\2\2\u01fbe\3\2\2\2\u01fc"+
		"\u01fd\7I\2\2\u01fdg\3\2\2\2\65kp~\u0088\u008c\u008f\u0093\u009c\u00a3"+
		"\u00a8\u00ab\u00ae\u00bb\u00bd\u00c6\u00d1\u00d3\u00dd\u00e7\u00f1\u0108"+
		"\u0112\u0118\u011e\u0126\u012f\u0137\u013f\u0147\u014a\u0152\u015a\u0162"+
		"\u016a\u016d\u017d\u0183\u0196\u019f\u01a2\u01ad\u01b0\u01bb\u01ca\u01d0"+
		"\u01d2\u01dc\u01e4\u01e6\u01f2\u01f6";
	public static final ATN _ATN =
		new ATNDeserializer().deserialize(_serializedATN.toCharArray());
	static {
		_decisionToDFA = new DFA[_ATN.getNumberOfDecisions()];
		for (int i = 0; i < _ATN.getNumberOfDecisions(); i++) {
			_decisionToDFA[i] = new DFA(_ATN.getDecisionState(i), i);
		}
	}
}