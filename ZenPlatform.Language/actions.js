"use strict"

var serverVersion = 123;
function doesItBlend() { return true; }

module.exports.evaluateLexerPredicate = function (lexer, ruleIndex, actionIndex, predicate) {
    return eval(predicate);
}

module.exports.evaluateParserPredicate = function (parser, ruleIndex, actionIndex, predicate) {
    return eval(predicate);
}