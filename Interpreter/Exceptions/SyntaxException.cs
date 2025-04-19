using Interpreter.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Exceptions
{
    internal class SyntaxException: BaseCompilerException
    {
        public SyntaxException(string message, Position pos): base(message, pos) { }
    }
}
