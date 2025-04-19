using Interpreter.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Exceptions
{
    internal class BaseCompilerException: Exception
    {
        public Position Pos { get; }

        public BaseCompilerException(string message, Position pos): base(message)
        {
            Pos = pos;
        }
    }
}
