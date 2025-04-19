using Interpreter.Exceptions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Lexer
{


    public enum TokenType
    {
        Int, DoubleLiteral, StringLiteral, Boolean, Undefined,
        Id,Plus, Minus, Multiply, Divide, Dot,
        Semicolon, LPar, RPar, LBrace, RBrace, Comma, Colon,
        Assign, AssignPlus, AssignMinus, AssignMult, AssignDiv,
        Equal, Less, LessEqual, Greater, GreaterEqual, NotEqual,
        tkAnd, tkOr, tkNot, Eof, tkIf, tkThen, tkElse, tkWhile, tkDo
    }

    public class Token : TokenBase
    {
        public TokenType type;

        public Token(TokenType type, Position pos, Object val = null): base(pos, val) 
        {
            this.type = type;
        }

        public override string? ToString()
        {
            return $"({position.Line}, {position.Column}), {value}, {type}";
        }
    }

    public class Lexer: LexerBase
    {
        public Lexer(string code): base(code) { }

        private static readonly Dictionary<string, TokenType> Keywords = new Dictionary<string, TokenType>
        {
            { "true", TokenType.Boolean },
            { "false", TokenType.Boolean },
            { "if", TokenType.tkIf },
            { "then", TokenType.tkThen },
            { "else", TokenType.tkElse },
            { "while", TokenType.tkWhile },
            { "do", TokenType.tkDo },
            { "int", TokenType.Int },
            { "double", TokenType.DoubleLiteral },
            { "bool", TokenType.Boolean }
        };


        public Token GetString(Position startPos)
        {
            while (peekChar() != '"' && !isAtEnd())
            {
                nextChar();
            }
            nextChar(); // Пропустить символ `"`

            string value = code.Substring(start + 1, cur - start - 2);
            return new Token(TokenType.StringLiteral, startPos, value);
        }

        public Token GetIdentifier(Position startPos)
        {
            while (isAlphaNumeric(peekChar()))
            {
                nextChar();
            }

            string value = code.Substring(start, cur - start);
            TokenType type = TokenType.Id;

            if (Keywords.ContainsKey(value))
            {
                type = Keywords[value];
            }

            return new Token(type, startPos, value);
        }

        public Token GetNumber(Position startPos)
        {
            while (char.IsDigit(peekChar()))
            {
                nextChar();
            }

            if (peekChar() == '.' && char.IsDigit(peekNextChar()))
            {
                nextChar();
                while (char.IsDigit(peekChar()))
                {
                    nextChar();
                }

                string value = code.Substring(start, cur - start);
                double doubleValue = double.Parse(value, CultureInfo.InvariantCulture);
                return new Token(TokenType.DoubleLiteral, startPos, doubleValue);
            }

            string intValue = code.Substring(start, cur - start);
            return new Token(TokenType.Int, startPos, int.Parse(intValue));
        }



        public Token NextToken()
        {
            char c = nextChar();
            while (c == '\r' || c == '\a' || c == ' ' || c == '\n')
            {
                c = nextChar();
            }

            var pos = curentPosition();
            start = cur - 1;

            Token result;

            switch (c)
            {
                case '\0':
                    result = new Token(TokenType.Eof, pos, "Eof");
                    break;
                case ',':
                    result = new Token(TokenType.Comma, pos);
                    break;
                case ')':
                    result = new Token(TokenType.RPar, pos);
                    break;
                case '(':
                    result = new Token(TokenType.LPar, pos);
                    break;
                case '}':
                    result = new Token(TokenType.RBrace, pos);
                    break;
                case '{':
                    result = new Token(TokenType.LBrace, pos);
                    break;
                case '+':
                    result = new Token(isMatch('=') ? TokenType.AssignPlus : TokenType.Plus, pos);
                    break;
                case '-':
                    result = new Token(isMatch('=') ? TokenType.AssignMinus : TokenType.Minus, pos);
                    break;
                case '*':
                    result = new Token(isMatch('=') ? TokenType.AssignMult : TokenType.Multiply, pos);
                    break;
                case '/':
                    if (isMatch('/'))
                    {
                        while (peekChar() != '\n' && !isAtEnd())
                        {
                            nextChar();
                        }
                        result = null; // Комментарий не порождает токен.
                    }
                    else
                    {
                        result = new Token(isMatch('=') ? TokenType.AssignDiv : TokenType.Divide, pos);
                    }
                    break;
                case ';':
                    result = new Token(TokenType.Semicolon, pos);
                    break;
                case '!':
                    result = new Token(isMatch('=') ? TokenType.NotEqual : TokenType.tkNot, pos);
                    break;
                case '=':
                    result = new Token(isMatch('=') ? TokenType.Equal : TokenType.Assign, pos);
                    break;
                case '>':
                    result = new Token(isMatch('=') ? TokenType.GreaterEqual : TokenType.Greater, pos);
                    break;
                case '<':
                    result = new Token(isMatch('=') ? TokenType.LessEqual : TokenType.Less, pos);
                    break;
                case '&':
                    if (isMatch('&'))
                    {
                        result = new Token(TokenType.tkAnd, pos);
                    }
                    else
                    {
                        throw new LexerException($"Неверный символ после &: {c}", curentPosition());
                    }
                    break;
                case '|':
                    if (isMatch('|'))
                    {
                        result = new Token(TokenType.tkOr, pos);
                    }
                    else
                    {
                        throw new LexerException($"Неверный символ после |: {c}", curentPosition());
                    }
                    break;
                case '"':
                    result = GetString(pos);
                    break;
                default:
                    if (char.IsDigit(c))
                    {
                        result = GetNumber(pos);
                    }
                    else if (isAlpha(c))
                    {
                        result = GetIdentifier(pos);
                    }
                    else
                    {
                        throw new LexerException($"Неизвестный символ {c} в позиции {pos.Line}, {pos.Column}", pos);
                    }
                    break;
            }

            if (result != null && result.value == null)
            {
                result.value = code.Substring(start, cur - start);
            }

            return result;
        }

    }
}
