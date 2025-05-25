using Interpreter.Exceptions;
using Interpreter.Lexer;
using System;
using static Interpreter.Parser.ASTNodes;

namespace Interpreter.Parser
{
    internal class ParserUnit : ParserUnitBase
    {
        public OpType TokenToOp(TokenType t)
        {
            switch (t)
            {
                case TokenType.Plus: return OpType.opPlus;
                case TokenType.Minus: return OpType.opMinus;
                case TokenType.Multiply: return OpType.opMultiply;
                case TokenType.Divide: return OpType.opDivide;
                case TokenType.Equal: return OpType.opEqual;
                case TokenType.Less: return OpType.opLess;
                case TokenType.LessEqual: return OpType.opLessEqual;
                case TokenType.Greater: return OpType.opGreater;
                case TokenType.GreaterEqual: return OpType.opGreaterEqual;
                case TokenType.NotEqual: return OpType.opNotEqual;
                case TokenType.tkAnd: return OpType.opAnd;
                case TokenType.tkOr: return OpType.opOr;
                case TokenType.tkNot: return OpType.opNot;
                default: return OpType.opBad;
            }
        }

        public ParserUnit(Lexer.Lexer lexer) : base(lexer) { }

        /// <summary>
        /// Главный метод парсера, запускающий разбор программы.
        /// </summary>
        public StatementNode MainProgram()
        {
            Current = 0;
            var result = StatementList();
            Requires(TokenType.Eof);
            return result;
        }

        /// <summary>
        /// Разбор списка инструкций.
        /// </summary>
        public StatementNode StatementList()
        {
            var stl = new StatementListNode();
            stl.Add(Statement());
            while (IsMatch(TokenType.Semicolon))
            {
                stl.Add(Statement());
            }
            return stl;
        }

        /// <summary>
        /// Разбор отдельной инструкции.
        /// </summary>
        public StatementNode Statement()
        {
            var pos = CurrentToken.position;

            if (At(TokenType.Int) || At(TokenType.DoubleLiteral) || At(TokenType.Boolean))
            {
                var varType = Advance().type;
                var ident = Ident();
                Requires(TokenType.Assign);
                var expr = Expr();
                return new AssignNode(ident, expr, varType, pos);
            }

            Check(TokenType.Id, TokenType.tkIf, TokenType.tkWhile, TokenType.LBrace);

            if (IsMatch(TokenType.tkIf))
            {
                var condition = Expr();
                Requires(TokenType.tkThen);
                var thenStatement = Statement();
                var elseStatement = IsMatch(TokenType.tkElse) ? Statement() : null;
                return new IfNode(condition, thenStatement, elseStatement, pos);
            }
            else if (IsMatch(TokenType.tkWhile))
            {
                var condition = Expr();
                Requires(TokenType.tkDo);
                var statement = Statement();
                return new WhileNode(condition, statement, pos);
            }
            else if (IsMatch(TokenType.LBrace))
            {
                var stl = StatementList();
                Requires(TokenType.RBrace);
                stl.Pos = pos;
                return stl;
            }

            var id = Ident();
            if (IsMatch(TokenType.Assign))
            {
                var expr = Expr();
                return new AssignNode(id, expr, TokenType.Undefined, pos);
            }
            else if (IsMatch(TokenType.AssignPlus))
            {
                var expr = Expr();
                return new AssignPlusNode(id, expr, pos);
            }
            else if (IsMatch(TokenType.LPar))
            {
                var exprList = ExprList();
                Requires(TokenType.RPar);
                return new ProcCallNode(id, exprList, pos);
            }
            else
            {
                ExpectedError(TokenType.Assign, TokenType.LPar);
                return null; // Не будет достигнуто
            }
        }

        /// <summary>
        /// Разбор списка выражений.
        /// </summary>
        public ExprListNode ExprList()
        {
            var list = new ExprListNode();
            list.Add(Expr());
            while (IsMatch(TokenType.Comma))
            {
                list.Add(Expr());
            }
            return list;
        }

        /// <summary>
        /// Разбор идентификатора.
        /// </summary>
        public IdNode Ident()
        {
            var token = Requires(TokenType.Id);
            return new IdNode((string)token.value, token.position);
        }

        /// <summary>
        /// Разбор выражения.
        /// </summary>
        public ExprNode Expr()
        {
            var expr = Comp();
            while (At(TokenType.Greater, TokenType.GreaterEqual, TokenType.Less, TokenType.LessEqual, TokenType.Equal, TokenType.NotEqual))
            {
                var op = Advance();
                var right = Comp();
                expr = new BinOpNode(expr, right, TokenToOp(op.type), expr.Pos);
            }
            return expr;
        }

        /// <summary>
        /// Разбор составного выражения.
        /// </summary>
        public ExprNode Comp()
        {
            var expr = Term();
            while (At(TokenType.Plus, TokenType.Minus, TokenType.tkOr))
            {
                var op = Advance();
                var right = Term();
                expr = new BinOpNode(expr, right, TokenToOp(op.type), expr.Pos);
            }
            return expr;
        }

        /// <summary>
        /// Разбор терма.
        /// </summary>
        public ExprNode Term()
        {
            var expr = Factor();
            while (At(TokenType.Multiply, TokenType.Divide, TokenType.tkAnd))
            {
                var op = Advance();
                var right = Factor();
                expr = new BinOpNode(expr, right, TokenToOp(op.type), expr.Pos);
            }
            return expr;
        }

        /// <summary>
        /// Разбор фактора.
        /// </summary>
        public ExprNode Factor()
        {
            var pos = CurrentToken.position;

            if (At(TokenType.Int))
            {
                return new IntNode((int)Advance().value, pos);
            }
            else if (At(TokenType.DoubleLiteral))
            {
                return new DoubleNode((double)Advance().value, pos);
            }
            else if (IsMatch(TokenType.LPar))
            {
                var expr = Expr();
                Requires(TokenType.RPar);
                return expr;
            }
            else if (At(TokenType.Boolean))
            {
                return new BooleanNode(bool.Parse(Advance().value.ToString().ToLower()), pos);
            }
            else if (At(TokenType.Id))
            {
                var id = Ident();
                if (IsMatch(TokenType.LPar))
                {
                    var exprList = ExprList();
                    Requires(TokenType.RPar);
                    return new FuncCallNode(id, exprList, id.Pos);
                }
                else
                {
                    return id;
                }
            }
            else
            {
                CompilerErrors.SyntaxError($"Expected INT or ( or id but {PeekToken.type} found", PeekToken.position);
                return null;
            }
        }
    }
}
