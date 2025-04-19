using Interpreter.Lexer;
using Interpreter.Exceptions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Parser
{
    public class ParserUnitBase
    {
        private Lexer.Lexer lex;
        protected Token curToken;
        protected int Current = 0;

        public ParserUnitBase(Lexer.Lexer lexer)
        {
            this.lex = lexer;
            Advance();
        }

        /// <summary>
        /// Возвращает текущий токен и переходит к следующему.
        /// </summary>
        public Token Advance()
        {
            Token result = curToken;
            curToken = lex.NextToken();
            if (!IsAtEnd)
                Current += 1;
            return result;
        }

        /// <summary>
        /// Проверяет, что тип текущего токена совпадает с одним из указанных типов.
        /// </summary>
        public bool At(params TokenType[] types)
        {
           // return types.Any(type => PeekToken.type == type);
            return types.Contains(PeekToken.type);
        }

        /// <summary>
        /// Проверяет, что тип текущего токена совпадает с одним из указанных типов.
        /// Если нет, вызывает ошибку.
        /// </summary>
        public void Check(params TokenType[] types)
        {
            if (!At(types))
            {
                ExpectedError(types);
            }
        }

        /// <summary>
        /// Проверяет, что тип текущего токена совпадает с одним из указанных типов.
        /// Если совпадает, переходит к следующему токену.
        /// </summary>
        public bool IsMatch(params TokenType[] types)
        {
            if (At(types))
            {
                Advance();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Проверяет, что тип текущего токена совпадает с одним из указанных типов.
        /// Если совпадает, возвращает текущий токен и переходит к следующему.
        /// В противном случае вызывает ошибку.
        /// </summary>
        public Token Requires(params TokenType[] types)
        {
            if (At(types))
            {
                return Advance();
            }
            ExpectedError(types);
            return null; // Никогда не достигнется, так как вызов ExpectedError выбрасывает исключение.
        }

        /// <summary>
        /// Проверяет, что достигнут конец файла.
        /// </summary>
        public bool IsAtEnd => PeekToken.type == TokenType.Eof;

        /// <summary>
        /// Возвращает текущий токен.
        /// </summary>
        public Token PeekToken => curToken;

        /// <summary>
        /// Возвращает текущий токен.
        /// </summary>
        public Token CurrentToken => curToken;

        /// <summary>
        /// Вызывает ошибку, если ожидаемый токен не соответствует текущему.
        /// </summary>
        protected void ExpectedError(params TokenType[] types)
        {
            string expected = string.Join(" или ", types);
            CompilerErrors.SyntaxError($"Ожидалось '{expected}', но найдено '{PeekToken.type}'", PeekToken.position);
        }
    }
}
