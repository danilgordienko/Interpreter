using Interpreter.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Common
{
    public static class SymbolTable
    {
        public struct Variable
        {
            public string Name { get; }
            public TokenType Type { get; set; }

            public int index { get; set; }

            public Variable(string name, TokenType type, int ind)
            {
                Name = name;
                Type = type;
                index = ind;
            }
        }

        public static List<RuntimeValue> VarValues = new List<RuntimeValue>();
        public static int VarValuesCount = 0;
        public static Dictionary<string, Variable> SymTable = new Dictionary<string, Variable>();

    }


    [StructLayout(LayoutKind.Explicit)]
    public class RuntimeValue
    {
        [FieldOffset(0)]
        public int i;

        [FieldOffset(0)]
        public double r;

        [FieldOffset(0)]
        public bool b;

        public RuntimeValue(int ii)
        {
            i = ii;
        }

        public RuntimeValue(double rr)
        {
            r = rr;
        }

        public RuntimeValue(bool bb)
        {
            b = bb;
        }
    }
}
