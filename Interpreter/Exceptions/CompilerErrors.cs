using Interpreter.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Exceptions
{
    internal class CompilerErrors
    {
        public static void LexerError(string message, Position pos)
        {
            throw new LexerException(message, pos);
        }

        public static void SyntaxError(string message, Position pos)
        {
            throw new SyntaxException(message, pos);
        }

        public static void SemanticError(string message, Position pos)
        {
            throw new SemanticException(message, pos);
        }

        public static void OutputError(string prefix, BaseCompilerException exception, string[] lines)
        {
            string lineContent = lines[exception.Pos.Line - 1];
            Console.WriteLine(lineContent);
            Console.WriteLine(new string(' ', exception.Pos.Column - 1) + '^');
            Console.WriteLine($"{prefix} {exception.Pos}: {exception.Message}");
        }
    }
}
