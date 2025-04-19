using Interpreter.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Interpreter.Parser.ASTNodes;

namespace Interpreter.SemanticAnalyzer
{
    public class SimplePrintVisitor : IVisitor<string>
    {
        public string VisitNode(Node n) => n.Visit(this);

        public string VisitExprNode(ExprNode ex) => ex.Visit(this);

        public string VisitStatementNode(StatementNode st) => st.Visit(this);

        public string VisitBinOp(BinOpNode bin)
            => bin.Left.Visit(this) + bin.Op + bin.Right.Visit(this);

        public string VisitStatementList(StatementListNode stl)
            => string.Join("\n", stl.Statements.Select(x => x.Visit(this)));

        public string VisitExprList(ExprListNode exlist)
            => string.Join(",", exlist.Expressions.Select(x => x.Visit(this)));

        public string VisitInt(IntNode n) => n.Value.ToString();

        public string VisitDouble(DoubleNode d) => d.Value.ToString();

        public string VisitId(IdNode id) => id.Name;

        public string VisitAssign(AssignNode ass)
            => ass.Ident.Name + " := " + ass.Expr.Visit(this);

        public string VisitAssignPlus(AssignPlusNode ass)
            => ass.Ident.Name + " += " + ass.Expr.Visit(this);

        public string VisitIf(IfNode ifn)
            => "if " + ifn.Condition.Visit(this) + " then " + ifn.ThenStat.Visit(this)
               + (ifn.ElseStat == null ? "" : ifn.ElseStat.Visit(this));

        public string VisitWhile(WhileNode whn)
            => "while " + whn.Condition.Visit(this) + " do \n" + whn.Stat.Visit(this);

        public string VisitProcCall(ProcCallNode p)
            => p.Name.Name + "(" + p.Parameters.Visit(this) + ")";

        public string VisitFuncCall(FuncCallNode f)
            => f.Name.Name + "(" + f.Parameters.Visit(this) + ")";

        public string VisitBoolean(BooleanNode b)
        {
            throw new NotImplementedException();
        }
    }
}
