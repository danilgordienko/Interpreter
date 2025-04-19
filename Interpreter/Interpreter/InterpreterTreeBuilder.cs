using System;
using Interpreter.Common;
using Interpreter.Interpreter;
using Interpreter.Lexer;
using Interpreter.Parser;
using static Interpreter.Parser.ASTNodes;

namespace Interpreter.Visitors
{
    public unsafe class ConvertASTToInterpretTreeVisitor : IVisitor<NodeI>
    {
        public NodeI VisitNode(Node n) => null;

        public NodeI VisitExprNode(ExprNode ex) => null;

        public NodeI VisitStatementNode(StatementNode st) => null;

        public NodeI VisitStatementList(StatementListNode stl)
        {
            var L = new StatementListNodeI();
            foreach (var x in stl.Statements)
                L.Add(x.Visit(this) as StatementNodeI);
            return L;
        }

        public NodeI VisitExprList(ExprListNode exlist)
        {
            var L = new ExprListNodeI();
            foreach (var x in exlist.Expressions)
                L.Add(x.Visit(this) as ExprNodeI);
            return L;
        }

        public NodeI VisitInt(IntNode n) => new IntNodeI(n.Value);

        public NodeI VisitDouble(DoubleNode d) => new DoubleNodeI(d.Value);

        public NodeI VisitId(IdNode id)
        {
            var variable = SymbolTable.SymTable[id.Name];
            var index = variable.index;
            switch (variable.Type)
            {
                case TokenType.Int:
                    fixed (int* pi = &SymbolTable.VarValues[index].i)
                        return new IdNodeI(pi);
                case TokenType.DoubleLiteral:
                    fixed (double* pr = &SymbolTable.VarValues[index].r)
                        return new IdNodeR(pr);
                case TokenType.Boolean:
                    fixed (bool* pb = &SymbolTable.VarValues[index].b)
                        return new IdNodeB(pb);
                default: throw new Exception("Unknown type");
            }
        }

        public NodeI VisitWhile(WhileNode whn)
        {
            return new WhileNodeI(
                whn.Condition.Visit(this) as ExprNodeI,
                whn.Stat.Visit(this) as StatementNodeI
            );
        }

        public NodeI VisitIf(IfNode ifn)
        {
            var the = ifn.ThenStat.Visit(this) as StatementNodeI;
            StatementNodeI el = null;
            if (ifn.ElseStat != null)
                el = ifn.ElseStat.Visit(this) as StatementNodeI;
            return new IfNodeI(
                ifn.Condition.Visit(this) as ExprNodeI,
                the,
                el
            );
        }

        public NodeI VisitAssign(AssignNode ass)
        {
            var variable = SymbolTable.SymTable[ass.Ident.Name];
            var index = variable.index;
            var expr = ass.Expr.Visit(this) as ExprNodeI;

            switch (variable.Type)
            {
                case TokenType.Int:
                    fixed (int* pi = &SymbolTable.VarValues[index].i)
                        return new AssignIntNodeI(pi, expr);
                case TokenType.DoubleLiteral:
                    fixed (double* pr = &SymbolTable.VarValues[index].r)
                        return new AssignRealNodeI(pr, expr);
                case TokenType.Boolean:
                    fixed (bool* pb = &SymbolTable.VarValues[index].b)
                        return new AssignBoolNodeI(pb, expr);
                default: throw new Exception("Unknown type");
            }
        }

        public NodeI VisitAssignPlus(AssignPlusNode ass)
        {
            var variable = SymbolTable.SymTable[ass.Ident.Name];
            var index = variable.index;
            var expr = ass.Expr;

            if (variable.Type == TokenType.Int)
            {
                if (expr is IntNode intc)
                {
                    fixed (int* pi = &SymbolTable.VarValues[index].i)
                        return new AssignPlusIntCNodeI(pi, intc.Value);
                }
                else
                {
                    fixed (int* pi = &SymbolTable.VarValues[index].i)
                        return new AssignPlusIntNodeI(pi, expr.Visit(this) as ExprNodeI);
                }
            }
            else if (variable.Type == TokenType.DoubleLiteral)
            {
                if (expr is IntNode intc)
                {
                    fixed (double* pr = &SymbolTable.VarValues[index].r)
                        return new AssignPlusRealIntCNodeI(pr, intc.Value);
                }
                else if (expr is DoubleNode dc)
                {
                    fixed (double* pr = &SymbolTable.VarValues[index].r)
                        return new AssignPlusRealCNodeI(pr, dc.Value);
                }
                else
                {
                    fixed (double* pr = &SymbolTable.VarValues[index].r)
                        return new AssignPlusRealNodeI(pr, expr.Visit(this) as ExprNodeI);
                }
            }

            throw new Exception("Unknown type in AssignPlus");
        }

        public NodeI VisitFuncCall(FuncCallNode f) => null;

        public NodeI VisitBinOp(BinOpNode bin)
        {
            var lt = CalcType.Get(bin.Left);
            var rt = CalcType.Get(bin.Right);
            var linterpr = bin.Left.Visit(this) as ExprNodeI;
            var rinterpr = bin.Right.Visit(this) as ExprNodeI;

            var sit = 0;
            if (rt == TokenType.DoubleLiteral) sit += 1;
            else if (rt == TokenType.Boolean) sit += 2;
            if (lt == TokenType.DoubleLiteral) sit += 3;
            else if (lt == TokenType.Boolean) sit += 6;

            switch (bin.Op)
            {
                case '+':
                    return sit switch
                    {
                        0 => rinterpr is IntNodeI ric ? new PlusIC(linterpr, ric.Val) : new PlusII(linterpr, rinterpr),
                        4 => new PlusRR(linterpr, rinterpr),
                        1 => new PlusIR(linterpr, rinterpr),
                        3 => new PlusRI(linterpr, rinterpr),
                        _ => throw new Exception("Invalid opPlus sit"),
                    };
                case '-':
                    return sit switch
                    {
                        0 => new MinusII(linterpr, rinterpr),
                        4 => new MinusRR(linterpr, rinterpr),
                        1 => new MinusIR(linterpr, rinterpr),
                        3 => new MinusRI(linterpr, rinterpr),
                        _ => throw new Exception("Invalid opMinus sit"),
                    };
                case '*':
                    return sit switch
                    {
                        0 => new MultII(linterpr, rinterpr),
                        4 => new MultRR(linterpr, rinterpr),
                        1 => new MultIR(linterpr, rinterpr),
                        3 => new MultRI(linterpr, rinterpr),
                        _ => throw new Exception("Invalid opMultiply sit"),
                    };
                case '/':
                    return sit switch
                    {
                        0 => new DivII(linterpr, rinterpr),
                        4 => new DivRR(linterpr, rinterpr),
                        1 => new DivIR(linterpr, rinterpr),
                        3 => linterpr is DoubleNodeI ric ? new DivRCI(ric.Val, rinterpr) : new DivRI(linterpr, rinterpr),
                        _ => throw new Exception("Invalid opDivide sit"),
                    };
                //case OpType.opEqual:
                //    return sit switch
                //    {
                //        0 => new EqII(linterpr, rinterpr),
                //        4 => new EqRR(linterpr, rinterpr),
                //        1 => new EqIR(linterpr, rinterpr),
                //        3 => new EqRI(linterpr, rinterpr),
                //        8 => new EqBB(linterpr, rinterpr),
                //        _ => throw new Exception("Invalid opEqual sit"),
                //    };
                //case OpType.NotEqual:
                //    return sit switch
                //    {
                //        0 => new NotEqII(linterpr, rinterpr),
                //        4 => new NotEqRR(linterpr, rinterpr),
                //        1 => new NotEqIR(linterpr, rinterpr),
                //        3 => new NotEqRI(linterpr, rinterpr),
                //        8 => new NotEqBB(linterpr, rinterpr),
                //        _ => throw new Exception("Invalid NotEqual sit"),
                //    };
                case '<':
                    return sit switch
                    {
                        //0 => rinterpr is IntNodeI ric ? new LessIC(linterpr, ric.Val) : new LessII(linterpr, rinterpr),
                        4 => new LessRR(linterpr, rinterpr),
                        1 => new LessIR(linterpr, rinterpr),
                        3 => new LessRI(linterpr, rinterpr),
                        _ => throw new Exception("Invalid opLess sit"),
                    };
                //case OpType.opLessEqual:
                //    return sit switch
                //    {
                //        0 => new LessEqII(linterpr, rinterpr),
                //        4 => new LessEqRR(linterpr, rinterpr),
                //        1 => new LessEqIR(linterpr, rinterpr),
                //        3 => new LessEqRI(linterpr, rinterpr),
                //        _ => throw new Exception("Invalid opLessEqual sit"),
                //    };
                case '>':
                    return sit switch
                    {
                        0 => new GreaterII(linterpr, rinterpr),
                        4 => new GreaterRR(linterpr, rinterpr),
                        1 => new GreaterIR(linterpr, rinterpr),
                        3 => new GreaterRI(linterpr, rinterpr),
                        _ => throw new Exception("Invalid opGreater sit"),
                    };
                //case OpType.opGreaterEqual:
                //    return sit switch
                //    {
                //        0 => new GreaterEqII(linterpr, rinterpr),
                //        4 => new GreaterEqRR(linterpr, rinterpr),
                //        1 => new GreaterEqIR(linterpr, rinterpr),
                //        3 => new GreaterEqRI(linterpr, rinterpr),
                //        _ => throw new Exception("Invalid opGreaterEqual sit"),
                //    };
                default:
                    throw new Exception("Unsupported binary operator");
            }
        }

        //public NodeI VisitProcCall(ProcCallNode p)
        //{
        //    return new ProcCallNodeI(
        //        p.Name.Name,
        //        p.Pars.Visit(this) as ExprListNodeI
        //    );
        //}

        public NodeI VisitBoolean(BooleanNode b) => new BooleanNodeI(n.Value);

        public NodeI VisitProcCall(ProcCallNode p)
        {
            throw new NotImplementedException();
        }
    }
}
