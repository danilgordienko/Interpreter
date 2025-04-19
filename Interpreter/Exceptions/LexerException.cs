using Interpreter.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Exceptions
{
    internal class LexerException : BaseCompilerException
    {
        public LexerException(string message, Position pos) : base(message, pos) {}
    }
}
