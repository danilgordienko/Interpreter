using Interpreter.Common;
using Interpreter.Exceptions;
using Interpreter.Lexer;
using Interpreter.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Interpreter.Common.SymbolTable;
using static Interpreter.Parser.ASTNodes;

namespace Interpreter.SemanticAnalyzer
{

    public class SemanticAnalyzerVisitor : IVisitorP
    {
        private int index = 0;

        public int CountVariables() => index + 1;

        public void Analyze(StatementNode node)
        {
            node.VisitP(this);
        }

        public void VisitNode(Node n) => n.VisitP(this);

        public void VisitExprNode(ExprNode ex) => ex.VisitP(this);

        public void VisitStatementNode(StatementNode st) => st.VisitP(this);

        public void VisitBinOp(BinOpNode bin)
        {
            bin.Left.VisitP(this);
            bin.Right.VisitP(this);
            TokenType leftType = GetExprType(bin.Left);
            TokenType rightType = GetExprType(bin.Right);

            if (leftType == TokenType.Boolean && (rightType == TokenType.DoubleLiteral || rightType == TokenType.Int) ||
                rightType == TokenType.Boolean && (leftType == TokenType.DoubleLiteral || leftType == TokenType.Int))
            {
                CompilerErrors.SemanticError($"Типы несовместимы: {leftType} и {rightType} для операции '{bin.Op}'", bin.Pos);
            }

            if (bin.Op.Equals(OpType.opGreater) || bin.Op.Equals(OpType.opLess) || bin.Op.Equals(OpType.opEqual))
            {
                if (leftType != TokenType.Int && leftType != TokenType.DoubleLiteral)
                {
                    CompilerErrors.SemanticError($"Операция '{bin.Op}' требует числовые типы, но найден тип {leftType}", bin.Pos);
                }
            }

            //if (bin.Op == '&'bin.Op.Equals(OpType.opGreater) || bin.Op == '|') 
            //{
            //    if (leftType != TokenType.Boolean || rightType != TokenType.Boolean)
            //    {
            //        CompilerErrors.SemanticError($"Операция '{bin.Op}' требует тип Boolean, но найден тип {leftType}", bin.Pos);
            //    }
            //}
        }

        public void VisitStatementList(StatementListNode stl)
        {
            foreach (var stmt in stl.Statements)
            {
                stmt.VisitP(this);
            }
        }

        public void VisitExprList(ExprListNode exlist)
        {
            foreach (var expr in exlist.Expressions)
            {
                expr.VisitP(this);
            }
        }

        public void VisitInt(IntNode n) => n.Value.ToString();

        public void VisitDouble(DoubleNode d) => d.Value.ToString();

        public void VisitId(IdNode id)
        {
            if (!SymbolTable.SymTable.ContainsKey(id.Name))
            {
                CompilerErrors.SemanticError($"Переменная '{id.Name}' не объявлена", id.Pos);
            }
        }

        public void VisitBoolean(BooleanNode b) => b.Value.ToString();

        public void VisitAssign(AssignNode ass)
        {
            ass.Expr.VisitP(this);

            TokenType exprType = GetExprType(ass.Expr);
            if (SymbolTable.SymTable.ContainsKey(ass.Ident.Name))
            {
                if (ass.Type != TokenType.Undefined)
                {
                    CompilerErrors.SemanticError($"Ошибка: переменная {ass.Ident.Name} уже объявлена!", ass.Pos);
                }
            }
            else
            {
                if(ass.Type == TokenType.Undefined)
                {
                    CompilerErrors.SemanticError($"Ошибка: переменная {ass.Ident.Name} не имет тип!", ass.Pos);
                }
                SymTable[ass.Ident.Name] = new Variable(ass.Ident.Name, ass.Type, index);
                index++;
            }


            if (SymbolTable.SymTable[ass.Ident.Name].Type != exprType)
            {
                CompilerErrors.SemanticError($"Ошибка: нельзя присвоить {exprType} переменной {ass.Ident.Name} типа {SymbolTable.SymTable[ass.Ident.Name].Type}", ass.Pos);
            }

        }

        public void VisitAssignPlus(AssignPlusNode ass)
        {
            if (!SymbolTable.SymTable.ContainsKey(ass.Ident.Name))
            {
                CompilerErrors.SemanticError($"Переменная '{ass.Ident.Name}' не объявлена", ass.Pos);
            }
            ass.Expr.VisitP(this);
            TokenType exprType = GetExprType(ass.Expr);
            if (SymbolTable.SymTable[ass.Ident.Name].Type != exprType)
            {
                CompilerErrors.SemanticError($"Типы несовместимы: переменная '{ass.Ident.Name}' имеет тип {SymbolTable.SymTable[ass.Ident.Name]}, а выражение типа {exprType}", ass.Pos);
            }
        }

        public void VisitIf(IfNode ifn)
        {
            ifn.Condition.VisitP(this);
            TokenType condType = GetExprType(ifn.Condition);
            if (condType != TokenType.Boolean)
            {
                CompilerErrors.SemanticError($"Условие должно быть типа {TokenType.Boolean}, но найден тип {condType}", ifn.Pos);
            }
            ifn.ThenStat.VisitP(this);
            if (ifn.ElseStat != null) ifn.ElseStat.VisitP(this);
        }

        public void VisitWhile(WhileNode whn)
        {
            whn.Condition.VisitP(this);
            TokenType condType = GetExprType(whn.Condition);
            if (condType != TokenType.Boolean)
            {
                CompilerErrors.SemanticError($"Условие должно быть типа {TokenType.Boolean}, но найден тип {condType}", whn.Pos);
            }
            whn.Stat.VisitP(this);
        }

        public void VisitProcCall(ProcCallNode p) { }
            //=> p.Name.Name + "(" + p.Parameters.VisitP(this) + ")";

        public void VisitFuncCall(FuncCallNode f) { }
            //=> f.Name.Name + "(" + f.Parameters.VisitP(this) + ")";

        private TokenType GetExprType(ExprNode expr)
        {
            return expr switch
            {
                IntNode => TokenType.Int,
                DoubleNode => TokenType.DoubleLiteral,
                BooleanNode => TokenType.Boolean,
                IdNode idNode => SymbolTable.SymTable.ContainsKey(idNode.Name) ? SymbolTable.SymTable[idNode.Name].Type : TokenType.Undefined,
                BinOpNode binOp => binOp.Op switch
                {
                    OpType.opGreater or OpType.opLess or OpType.opEqual or OpType.opNot => TokenType.Boolean,
                    //'&' or '|' => TokenType.Boolean,
                    _ => GetExprType(binOp.Left)
                },
                _ => TokenType.Undefined
            };
        }
    }
}
