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

    public class SemanticAnalyzerVisitor : IVisitor<string>
    {
        private int index = 0;

        public int CountVariables() => index + 1;

        public void Analyze(StatementNode node)
        {
            node.Visit(this);
        }

        public string VisitNode(Node n) => n.Visit(this);

        public string VisitExprNode(ExprNode ex) => ex.Visit(this);

        public string VisitStatementNode(StatementNode st) => st.Visit(this);

        public string VisitBinOp(BinOpNode bin)
        {
            bin.Left.Visit(this);
            bin.Right.Visit(this);
            TokenType leftType = GetExprType(bin.Left);
            TokenType rightType = GetExprType(bin.Right);

            if (leftType == TokenType.Boolean && (rightType == TokenType.DoubleLiteral || rightType == TokenType.Int) ||
                rightType == TokenType.Boolean && (leftType == TokenType.DoubleLiteral || leftType == TokenType.Int))
            {
                CompilerErrors.SemanticError($"Типы несовместимы: {leftType} и {rightType} для операции '{bin.Op}'", bin.Pos);
            }

            if (bin.Op == '>' || bin.Op == '<' || bin.Op == '=')
            {
                if (leftType != TokenType.Int && leftType != TokenType.DoubleLiteral)
                {
                    CompilerErrors.SemanticError($"Операция '{bin.Op}' требует числовые типы, но найден тип {leftType}", bin.Pos);
                }
            }

            if (bin.Op == '&' || bin.Op == '|') 
            {
                if (leftType != TokenType.Boolean || rightType != TokenType.Boolean)
                {
                    CompilerErrors.SemanticError($"Операция '{bin.Op}' требует тип Boolean, но найден тип {leftType}", bin.Pos);
                }
            }
            return "";
        }

        public string VisitStatementList(StatementListNode stl)
        {
            foreach (var stmt in stl.Statements)
            {
                stmt.Visit(this);
            }
            return "";
        }

        public string VisitExprList(ExprListNode exlist)
        {
            foreach (var expr in exlist.Expressions)
            {
                expr.Visit(this);
            }
            return "";
        }

        public string VisitInt(IntNode n) => n.Value.ToString();

        public string VisitDouble(DoubleNode d) => d.Value.ToString();

        public string VisitId(IdNode id)
        {
            if (!SymbolTable.SymTable.ContainsKey(id.Name))
            {
                CompilerErrors.SemanticError($"Переменная '{id.Name}' не объявлена", id.Pos);
            }
            return id.Name;
        }

        public string VisitBoolean(BooleanNode b) => b.Value.ToString();

        public string VisitAssign(AssignNode ass)
        {
            ass.Expr.Visit(this);

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

            return ass.Ident.Name;
        }

        public string VisitAssignPlus(AssignPlusNode ass)
        {
            if (!SymbolTable.SymTable.ContainsKey(ass.Ident.Name))
            {
                CompilerErrors.SemanticError($"Переменная '{ass.Ident.Name}' не объявлена", ass.Pos);
            }
            ass.Expr.Visit(this);
            TokenType exprType = GetExprType(ass.Expr);
            if (SymbolTable.SymTable[ass.Ident.Name].Type != exprType)
            {
                CompilerErrors.SemanticError($"Типы несовместимы: переменная '{ass.Ident.Name}' имеет тип {SymbolTable.SymTable[ass.Ident.Name]}, а выражение типа {exprType}", ass.Pos);
            }
            return ass.Ident.Name + " += " + ass.Expr.Visit(this);
        }

        public string VisitIf(IfNode ifn)
        {
            ifn.Condition.Visit(this);
            TokenType condType = GetExprType(ifn.Condition);
            if (condType != TokenType.Boolean)
            {
                CompilerErrors.SemanticError($"Условие должно быть типа {TokenType.Boolean}, но найден тип {condType}", ifn.Pos);
            }
            ifn.ThenStat.Visit(this);
            if (ifn.ElseStat != null) ifn.ElseStat.Visit(this);
            return "";
        }

        public string VisitWhile(WhileNode whn)
        {
            whn.Condition.Visit(this);
            TokenType condType = GetExprType(whn.Condition);
            if (condType != TokenType.Boolean)
            {
                CompilerErrors.SemanticError($"Условие должно быть типа {TokenType.Boolean}, но найден тип {condType}", whn.Pos);
            }
            whn.Stat.Visit(this);
            return "";
        }

        public string VisitProcCall(ProcCallNode p)
            => p.Name.Name + "(" + p.Parameters.Visit(this) + ")";

        public string VisitFuncCall(FuncCallNode f)
            => f.Name.Name + "(" + f.Parameters.Visit(this) + ")";

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
                    '<' or '>' or '=' or '!' => TokenType.Boolean,
                    '&' or '|' => TokenType.Boolean,
                    _ => GetExprType(binOp.Left)
                },
                _ => TokenType.Undefined
            };
        }
    }
}
