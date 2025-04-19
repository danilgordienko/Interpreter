using Interpreter.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Exceptions
{
    internal class SemanticException: BaseCompilerException
    {
        public SemanticException(string message, Position pos) : base(message, pos) { }
    }
}
