using Interpreter;
using Interpreter.Exceptions;
using Interpreter.Lexer;
using Interpreter.Parser;
using Interpreter.SemanticAnalyzer;

/*var lex = new Lexer("a = 35; if\na > 40 && True\n\nthen a = 40 else \n{ a = 0; print(a * 223 + 10) }");

Token t;
do
{
    t = lex.NextToken();
    Console.WriteLine(t);
} while (t.type != TokenType.Eof);*/


string text = "int i = 1; double d = 2.0; double c = i + d; int sum = 0; while  i < 100000000 do { sum += 1 / i; i += 1 }; print(sum)";
var lexer = new Lexer(text);

try
{
    var parser = new ParserUnit(lexer);
    var program = parser.MainProgram();
    var semanticAnalyzer = new SemanticAnalyzerVisitor();
    semanticAnalyzer.Analyze(program);


    //Console.WriteLine();
    //var startTime = DateTime.Now;
    // program.Execute();
    //var elapsedTime = DateTime.Now - startTime;

    //Console.WriteLine($"\n{elapsedTime.TotalSeconds} с");
}
catch (LexerException e)
{
    CompilerErrors.OutputError("Lexer error:", e, lexer.lines());
}
catch (SyntaxException e)
{
    CompilerErrors.OutputError("Parser error:", e, lexer.lines());
}
catch (SemanticException e)
{
    CompilerErrors.OutputError("Semantic error:", e, lexer.lines());
}