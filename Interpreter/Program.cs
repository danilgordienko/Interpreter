using Interpreter;
using Interpreter.Exceptions;
using Interpreter.Lexer;



//string text = "bool i = 1; double sum = 0.0; while  i < 100000000 do { sum += 1.0/i; i += 1 }; PrintReal(sum)";
string text = "bool i = 1; int a = 1; PrintReal(i)";
var lexer = new Lexer(text);

try
{
    var root = Interpreter.Interpreter.Interpreter.ConvertToInterpretTree(text);
    root.Execute();
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