using Interpreter.Common;
using Interpreter.Exceptions;
using Interpreter.Visitors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interpreter.Lexer;
using Interpreter.Parser;
using Interpreter.SemanticAnalyzer;

namespace Interpreter.Interpreter
{
    internal class Interpreter
    {
        public static List<string> Lines = new List<string>();

        public static void OutputError(string prefix, BaseCompilerException e)
        {
            CompilerErrors.OutputError(prefix, e, Lines.ToArray());
        }

        public static StatementNodeI ConvertToInterpretTree(string text)
        {
            var lex = new Lexer.Lexer(text);
            Lines = new List<string>(lex.lines());

            var parser = new ParserUnit(lex);

            // Парсинг
            var root = parser.MainProgram();

            // Семантические проверки
            var semcheck = new SemanticAnalyzerVisitor();
            root.VisitP(semcheck);
            //semcheck.Analyze(root);

            for (int i = 0; i < semcheck.CountVariables(); i++)
                SymbolTable.VarValues.Add(new RuntimeValue(0));

            return root.Visit(new ConvertASTToInterpretTreeVisitor()) as StatementNodeI;
        }
    }
}
