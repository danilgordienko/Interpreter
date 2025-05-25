using Interpreter.Common;
using Interpreter.Lexer;
using static Interpreter.Parser.ASTNodes;

namespace Interpreter.Parser
{
    public enum OpType
    {
        opPlus,opMinus,opMultiply,opDivide,opEqual,opLess,opLessEqual,
        opGreater, opGreaterEqual, opNotEqual,opAnd, opOr, opNot, opBad // для некорректной операци
    }

    public interface IVisitor<T>
    {
        T VisitNode(Node bin);
        T VisitExprNode(ExprNode bin);
        T VisitStatementNode(StatementNode bin);
        T VisitBinOp(BinOpNode bin);
        T VisitStatementList(StatementListNode stl);
        T VisitExprList(ExprListNode exlist);
        T VisitInt(IntNode n);
        T VisitDouble(DoubleNode d);
        T VisitBoolean(BooleanNode b);
        T VisitId(IdNode id);
        T VisitAssign(AssignNode ass);
        T VisitAssignPlus(AssignPlusNode ass);
        T VisitIf(IfNode ifn);
        T VisitWhile(WhileNode whn);
        T VisitProcCall(ProcCallNode p);
        T VisitFuncCall(FuncCallNode f);
    }

    public interface IVisitorP
    {
        void VisitNode(Node bin);
        void VisitExprNode(ExprNode bin);
        void VisitStatementNode(StatementNode bin);
        void VisitBinOp(BinOpNode bin);
        void VisitStatementList(StatementListNode stl);
        void VisitExprList(ExprListNode exlist);
        void VisitInt(IntNode n);
        void VisitDouble(DoubleNode d);

        void VisitBoolean(BooleanNode b);
        void VisitId(IdNode id);
        void VisitAssign(AssignNode ass);
        void VisitAssignPlus(AssignPlusNode ass);
        void VisitIf(IfNode ifn);
        void VisitWhile(WhileNode whn);
        void VisitProcCall(ProcCallNode p);
        void VisitFuncCall(FuncCallNode f);
    }



    public class ASTNodes
    {
        public class Node
        {
            public Position Pos { get; set; }
            public virtual T Visit<T>(IVisitor<T> visitor)
            {
                return visitor.VisitNode(this);
            }

            public virtual void VisitP(IVisitorP visitor)
            {
                visitor.VisitNode(this);
            }
        }

        public class ExprNode : Node
        {
            public virtual double Eval() => 0;
            public override T Visit<T>(IVisitor<T> visitor)
            {
                return visitor.VisitExprNode(this);
            }

            public override void VisitP(IVisitorP visitor)
            {
                visitor.VisitExprNode(this);
            }
        }

        public class StatementNode : Node
        {

            public override T Visit<T>(IVisitor<T> visitor)
            {
                return visitor.VisitStatementNode(this);
            }

            public override void VisitP(IVisitorP visitor)
            {
                visitor.VisitStatementNode(this);
            }
        }

        public class BinOpNode : ExprNode
        {
            public ExprNode Left { get; }
            public ExprNode Right { get; }
            public OpType Op { get; }

            public BinOpNode(ExprNode left, ExprNode right, OpType op, Position pos)
            {
                Left = left;
                Right = right;
                Op = op;
                Pos = pos;
            }

            public override T Visit<T>(IVisitor<T> visitor)
            {
                return visitor.VisitBinOp(this);
            }

            public override void VisitP(IVisitorP visitor)
            {
                visitor.VisitBinOp(this);
            }

            //public override double Eval()
            //{
            //    var l = Left.Eval();
            //    var r = Right.Eval();
            //    return Op switch
            //    {
            //        '+' => l + r,
            //        '*' => l * r,
            //        '/' => l / r,
            //        '<' => l < r ? 1 : 0,
            //        //_ => throw new InvalidOperationException($"Unsupported operation: {Op}")
            //    };
            //}
        }

        public class StatementListNode : StatementNode
        {
            public List<StatementNode> Statements { get; set; } = new();

            public void Add(StatementNode statement) => Statements.Add(statement);

            public override T Visit<T>(IVisitor<T> visitor)
            {
                return visitor.VisitStatementList(this);
            }

            public override void VisitP(IVisitorP visitor)
            {
                visitor.VisitStatementList(this);
            }

            //public override void Execute()
            //{
            //    foreach (var statement in Statements)
            //    {
            //        statement.Execute();
            //    }
            //}
        }

        public class ExprListNode : Node
        {
            public List<ExprNode> Expressions { get; set; } = new();

            public void Add(ExprNode expression) => Expressions.Add(expression);

            public override T Visit<T>(IVisitor<T> visitor)
            {
                return visitor.VisitExprList(this);
            }

            public override void VisitP(IVisitorP visitor)
            {
                visitor.VisitExprList(this);
            }
        }

        public class BooleanNode : ExprNode
        {
            public bool Value { get; }

            public BooleanNode(bool value, Position pos)
            {
                Value = value;
                Pos = pos;
            }

            public override T Visit<T>(IVisitor<T> visitor)
            {
                return visitor.VisitBoolean(this);
            }

            public override void VisitP(IVisitorP visitor)
            {
                visitor.VisitBoolean(this);
            }

            //public override double Eval() => Value ? 1 : 0;
        }

        public class IntNode : ExprNode
        {
            public int Value { get; }

            public IntNode(int value, Position pos)
            {
                Value = value;
                Pos = pos;
            }

            public override T Visit<T>(IVisitor<T> visitor)
            {
                return visitor.VisitInt(this);
            }

            public override void VisitP(IVisitorP visitor)
            {
                visitor.VisitInt(this);
            }

            //public override double Eval() => Value;
        }

        public class DoubleNode : ExprNode
        {
            public double Value { get; }

            public DoubleNode(double value, Position pos)
            {
                Value = value;
                Pos = pos;
            }

            public override T Visit<T>(IVisitor<T> visitor)
            {
                return visitor.VisitDouble(this);
            }

            public override void VisitP(IVisitorP visitor)
            {
                visitor.VisitDouble(this);
            }

            //public override double Eval() => Value;
        }

        public class IdNode : ExprNode
        {
            public string Name { get; }
            //public unsafe double* Ptr { get; private set; }

            public IdNode(string name, Position pos)
            {
                Name = name;
                Pos = pos;
                unsafe
                {
                    //Ptr = SymbolTable.GetOrCreateVariable(name);
                }
            }

            public override T Visit<T>(IVisitor<T> visitor)
            {
                return visitor.VisitId(this);
            }

            public override void VisitP(IVisitorP visitor)
            {
                visitor.VisitId(this);
            }


            //public override double Eval()
            //{
            //    unsafe
            //    {
            //        return *Ptr;
            //    }
            //}
        }


        public class AssignNode : StatementNode
        {
            public IdNode Ident { get; }
            public ExprNode Expr { get; }

            public TokenType Type { get; }

            public AssignNode(IdNode ident, ExprNode expr, TokenType type, Position pos)
            {
                Ident = ident;
                Expr = expr;
                Pos = pos;
                Type = type;
            }

            public override T Visit<T>(IVisitor<T> visitor)
            {
                return visitor.VisitAssign(this);
            }

            public override void VisitP(IVisitorP visitor)
            {
                visitor.VisitAssign(this);
            }

            //public override void Execute()
            //{
            //    unsafe
            //    {
            //        *Ident.Ptr = Expr.Eval();
            //    }
            //}
        }


        public class AssignPlusNode : StatementNode
        {
            public IdNode Ident { get; }
            public ExprNode Expr { get; }

            public AssignPlusNode(IdNode ident, ExprNode expr, Position pos)
            {
                Ident = ident;
                Expr = expr;
                Pos = pos;
            }

            public override T Visit<T>(IVisitor<T> v)
            {
                return v.VisitAssignPlus(this);
            }

            public override void VisitP(IVisitorP visitor)
            {
                visitor.VisitAssignPlus(this);
            }

            //public override void Execute()
            //{
            //    unsafe
            //    {
            //        *Ident.Ptr += Expr.Eval();
            //    }
            //}
        }

        public class IfNode : StatementNode
        {
            public ExprNode Condition { get; }
            public StatementNode ThenStat { get; }
            public StatementNode ElseStat { get; }

            public IfNode(ExprNode condition, StatementNode thenStat, StatementNode elseStat, Position pos)
            {
                Condition = condition;
                ThenStat = thenStat;
                ElseStat = elseStat;
                Pos = pos;
            }

            public override T Visit<T>(IVisitor<T> v)
            {
                return v.VisitIf(this);
            }

            public override void VisitP(IVisitorP visitor)
            {
                visitor.VisitIf(this);
            }

            //public override void Execute()
            //{
            //    if (Condition.Eval() > 0)
            //    {
            //        ThenStat.Execute();
            //    }
            //    else if (ElseStat != null)
            //    {
            //        ElseStat.Execute();
            //    }
            //}
        }

        public class WhileNode : StatementNode
        {
            public ExprNode Condition { get; }
            public StatementNode Stat { get; }

            public WhileNode(ExprNode condition, StatementNode stat, Position pos)
            {
                Condition = condition;
                Stat = stat;
                Pos = pos;
            }

            public override T Visit<T>(IVisitor<T> v)
            {
                return v.VisitWhile(this);
            }

            public override void VisitP(IVisitorP visitor)
            {
                visitor.VisitWhile(this);
            }

            //public override void Execute()
            //{
            //    while (Condition.Eval() > 0)
            //    {
            //        Stat.Execute();
            //    }
            //}
        }

        public class ProcCallNode : StatementNode
        {
            public IdNode Name { get; }
            public ExprListNode Parameters { get; }

            public ProcCallNode(IdNode name, ExprListNode parameters, Position pos)
            {
                Name = name;
                Parameters = parameters;
                Pos = pos;
            }

            public override T Visit<T>(IVisitor<T> v)
            {
                return v.VisitProcCall(this);
            }

            public override void VisitP(IVisitorP visitor)
            {
                visitor.VisitProcCall(this);
            }

            //public override void Execute()
            //{
            //    if (Name.Name.ToLower() == "print")
            //    {
            //        Console.WriteLine(Parameters.Expressions[0].Eval());
            //    }
            //}
        }

        public class FuncCallNode : ExprNode
        {
            public IdNode Name { get; }
            public ExprListNode Parameters { get; }

            public FuncCallNode(IdNode name, ExprListNode parameters, Position pos)
            {
                Name = name;
                Parameters = parameters;
                Pos = pos;
            }

            public override T Visit<T>(IVisitor<T> v)
            {
                return v.VisitFuncCall(this);
            }

            public override void VisitP(IVisitorP visitor)
            {
                visitor.VisitFuncCall(this);
            }
        }

        public static class NodeHelpers
        {
            public static BinOpNode Bin(ExprNode left, OpType op, ExprNode right, Position pos = null)
                => new BinOpNode(left, right, op, pos);

            public static AssignNode Ass(IdNode ident, ExprNode expr, TokenType type, Position pos = null)
                => new AssignNode(ident, expr,type, pos);

            public static IdNode Idd(string name, Position pos = null)
                => new IdNode(name, pos);

            public static DoubleNode Num(double value, Position pos = null)
                => new DoubleNode(value, pos);

            public static IfNode Iff(ExprNode cond, StatementNode th, StatementNode el, Position pos = null)
                => new IfNode(cond, th, el, pos);

            public static WhileNode Wh(ExprNode cond, StatementNode body, Position pos = null)
                => new WhileNode(cond, body, pos);

            public static StatementListNode StL(params StatementNode[] ss)
            {
                return new StatementListNode
                {
                    Statements = ss.ToList()
                };
            }

            public static ExprListNode ExL(params ExprNode[] ee)
            {
                return new ExprListNode
                {
                    Expressions = ee.ToList()
                };
            }

            public static ProcCallNode ProcCall(IdNode name, ExprListNode exlist, Position pos = null)
                => new ProcCallNode(name, exlist, pos);
        }

    }
}
