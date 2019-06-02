// Generated from c:\Users\n.zenkov\Source\Repos\ThePlatform\SqlPlusDbSync\SqlPlusDbSync\PrototypePlatformLanguage\ZSharp.g4 by ANTLR 4.7.1
import org.antlr.v4.runtime.Lexer;
import org.antlr.v4.runtime.CharStream;
import org.antlr.v4.runtime.Token;
import org.antlr.v4.runtime.TokenStream;
import org.antlr.v4.runtime.*;
import org.antlr.v4.runtime.atn.*;
import org.antlr.v4.runtime.dfa.DFA;
import org.antlr.v4.runtime.misc.*;

@SuppressWarnings({"all", "warnings", "unchecked", "unused", "cast"})
public class ZSharpLexer extends Lexer {
	static { RuntimeMetaData.checkVersion("4.7.1", RuntimeMetaData.VERSION); }

	protected static final DFA[] _decisionToDFA;
	protected static final PredictionContextCache _sharedContextCache =
		new PredictionContextCache();
	public static final int
		T__0=1, T__1=2, WHITESPACES=3, VAR=4, IDENTIFIER=5, DEC_DIGIT=6;
	public static String[] channelNames = {
		"DEFAULT_TOKEN_CHANNEL", "HIDDEN"
	};

	public static String[] modeNames = {
		"DEFAULT_MODE"
	};

	public static final String[] ruleNames = {
		"T__0", "T__1", "WHITESPACES", "VAR", "IDENTIFIER", "DEC_DIGIT", "NewLine", 
		"Whitespace", "UnicodeClassZS"
	};

	private static final String[] _LITERAL_NAMES = {
		null, "';'", "'='", null, "'var'"
	};
	private static final String[] _SYMBOLIC_NAMES = {
		null, null, null, "WHITESPACES", "VAR", "IDENTIFIER", "DEC_DIGIT"
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


	public ZSharpLexer(CharStream input) {
		super(input);
		_interp = new LexerATNSimulator(this,_ATN,_decisionToDFA,_sharedContextCache);
	}

	@Override
	public String getGrammarFileName() { return "ZSharp.g4"; }

	@Override
	public String[] getRuleNames() { return ruleNames; }

	@Override
	public String getSerializedATN() { return _serializedATN; }

	@Override
	public String[] getChannelNames() { return channelNames; }

	@Override
	public String[] getModeNames() { return modeNames; }

	@Override
	public ATN getATN() { return _ATN; }

	public static final String _serializedATN =
		"\3\u608b\ua72a\u8133\ub9ed\u417c\u3be7\u7786\u5964\2\bC\b\1\4\2\t\2\4"+
		"\3\t\3\4\4\t\4\4\5\t\5\4\6\t\6\4\7\t\7\4\b\t\b\4\t\t\t\4\n\t\n\3\2\3\2"+
		"\3\3\3\3\3\4\3\4\6\4\34\n\4\r\4\16\4\35\3\4\3\4\3\5\3\5\3\5\3\5\3\6\6"+
		"\6\'\n\6\r\6\16\6(\3\7\3\7\7\7-\n\7\f\7\16\7\60\13\7\3\7\3\7\7\7\64\n"+
		"\7\f\7\16\7\67\13\7\3\b\3\b\3\b\5\b<\n\b\3\t\3\t\5\t@\n\t\3\n\3\n\2\2"+
		"\13\3\3\5\4\7\5\t\6\13\7\r\b\17\2\21\2\23\2\3\2\5\6\2\f\f\17\17\u0087"+
		"\u0087\u202a\u202b\4\2\13\13\r\16\13\2\"\"\u00a2\u00a2\u1682\u1682\u1810"+
		"\u1810\u2002\u2008\u200a\u200c\u2031\u2031\u2061\u2061\u3002\u3002\2F"+
		"\2\3\3\2\2\2\2\5\3\2\2\2\2\7\3\2\2\2\2\t\3\2\2\2\2\13\3\2\2\2\2\r\3\2"+
		"\2\2\3\25\3\2\2\2\5\27\3\2\2\2\7\33\3\2\2\2\t!\3\2\2\2\13&\3\2\2\2\r*"+
		"\3\2\2\2\17;\3\2\2\2\21?\3\2\2\2\23A\3\2\2\2\25\26\7=\2\2\26\4\3\2\2\2"+
		"\27\30\7?\2\2\30\6\3\2\2\2\31\34\5\21\t\2\32\34\5\17\b\2\33\31\3\2\2\2"+
		"\33\32\3\2\2\2\34\35\3\2\2\2\35\33\3\2\2\2\35\36\3\2\2\2\36\37\3\2\2\2"+
		"\37 \b\4\2\2 \b\3\2\2\2!\"\7x\2\2\"#\7c\2\2#$\7t\2\2$\n\3\2\2\2%\'\4c"+
		"|\2&%\3\2\2\2\'(\3\2\2\2(&\3\2\2\2()\3\2\2\2)\f\3\2\2\2*.\4\63;\2+-\4"+
		"\62;\2,+\3\2\2\2-\60\3\2\2\2.,\3\2\2\2./\3\2\2\2/\65\3\2\2\2\60.\3\2\2"+
		"\2\61\62\7\60\2\2\62\64\4\62;\2\63\61\3\2\2\2\64\67\3\2\2\2\65\63\3\2"+
		"\2\2\65\66\3\2\2\2\66\16\3\2\2\2\67\65\3\2\2\289\7\17\2\29<\7\f\2\2:<"+
		"\t\2\2\2;8\3\2\2\2;:\3\2\2\2<\20\3\2\2\2=@\5\23\n\2>@\t\3\2\2?=\3\2\2"+
		"\2?>\3\2\2\2@\22\3\2\2\2AB\t\4\2\2B\24\3\2\2\2\n\2\33\35(.\65;?\3\2\3"+
		"\2";
	public static final ATN _ATN =
		new ATNDeserializer().deserialize(_serializedATN.toCharArray());
	static {
		_decisionToDFA = new DFA[_ATN.getNumberOfDecisions()];
		for (int i = 0; i < _ATN.getNumberOfDecisions(); i++) {
			_decisionToDFA[i] = new DFA(_ATN.getDecisionState(i), i);
		}
	}
}