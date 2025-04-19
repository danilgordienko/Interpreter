using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Interpreter.Lexer
{

    public class Position
    { 
        public int Line;
        public int Column;

        public Position(int line, int column) { Line = line; Column = column; }
    }

    public class TokenBase
    {
        public Position position;
        public Object value;

        public TokenBase(Position pos, Object val) { position = pos; value = val; }
    }


    public abstract class LexerBase
    {
        protected string code;
        protected int line = 1;
        protected int column = 0;
        protected int cur = 0;
        protected int start = 0;
        public bool atEoln = false;

        public LexerBase(string code) { this.code = code; }

        protected Position curentPosition() { return new Position(line, column); }

        protected bool isAtEnd() { return cur >= code.Length; }

        protected char peekChar() { return isAtEnd()? '\0': code[cur]; }

        protected char peekNextChar() {
            var pos = cur + 1;
            return pos > code.Length? '\0': code[pos];
        }

        protected static bool isAlpha(char c) { return  Regex.IsMatch(c.ToString(), "[A-Za-zА-Яа-яёЁ_]"); }

        protected static bool isAlphaNumeric(char c) { return  isAlpha(c) || char.IsDigit(c); }

        protected char nextChar()
        {
            char res = peekChar();
            if (atEoln)
            {
                atEoln= false;
                line += 1;
                column = 0;
            }
            if (res == '\0')
                return '\0';
            if (res == '\n')
                atEoln = true;
            cur += 1;
            column += 1;
            return res;

        }

        protected bool isMatch(char expected)
        {
            bool res = peekChar() == expected;
            if (res)
               nextChar();
            return res;
        }

        public string[] lines() { return code.Split("\n"); }



    }
}
