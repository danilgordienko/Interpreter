using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Interpreter
{
    public abstract class NodeI { }

    public abstract class ExprNodeI : NodeI
    {
        public virtual int EvalInt() => 0;
        public virtual double EvalReal() => 0.0;
        public virtual bool EvalBool() => false;
    }

    public abstract class StatementNodeI : NodeI
    {
        public virtual void Execute() { }
    }

    public abstract class BinOpNodeI : ExprNodeI
    {
        public ExprNodeI Left, Right;

        protected BinOpNodeI(ExprNodeI left, ExprNodeI right)
        {
            Left = left;
            Right = right;
        }
    }

    public class StatementListNodeI : StatementNodeI
    {
        public List<StatementNodeI> List { get; } = new List<StatementNodeI>();

        public void Add(StatementNodeI st) => List.Add(st);

        public override void Execute()
        {
            foreach (var statement in List)
                statement.Execute();
        }
    }

    public class ExprListNodeI : NodeI
    {
        public List<ExprNodeI> List { get; } = new List<ExprNodeI>();

        public void Add(ExprNodeI ex) => List.Add(ex);
    }

    public class PlusII : BinOpNodeI
    {
        public PlusII(ExprNodeI left, ExprNodeI right) : base(left, right) { }
        public override int EvalInt() => Left.EvalInt() + Right.EvalInt();
    }

    public class PlusIR : BinOpNodeI
    {
        public PlusIR(ExprNodeI left, ExprNodeI right) : base(left, right) { }
        public override double EvalReal() => Left.EvalInt() + Right.EvalReal();
    }

    public class PlusRI : BinOpNodeI
    {
        public PlusRI(ExprNodeI left, ExprNodeI right) : base(left, right) { }
        public override double EvalReal() => Left.EvalReal() + Right.EvalInt();
    }

    public class PlusRR : BinOpNodeI
    {
        public PlusRR(ExprNodeI left, ExprNodeI right) : base(left, right) { }
        public override double EvalReal() => Left.EvalReal() + Right.EvalReal();
    }

    public class PlusIC : BinOpNodeI
    {
        private int value;
        public PlusIC(ExprNodeI left, int value) : base(left, null) => this.value = value;
        public override int EvalInt() => Left.EvalInt() + value;
    }

    public class PlusRC : BinOpNodeI
    {
        private double value;
        public PlusRC(ExprNodeI left, double value) : base(left, null) => this.value = value;
        public override double EvalReal() => Left.EvalReal() + value;
    }

    // Minus (вычитание)
    public class MinusII : BinOpNodeI
    {
        public MinusII(ExprNodeI left, ExprNodeI right) : base(left, right) { }
        public override int EvalInt() => Left.EvalInt() - Right.EvalInt();
    }

    public class MinusIR : BinOpNodeI
    {
        public MinusIR(ExprNodeI left, ExprNodeI right) : base(left, right) { }
        public override double EvalReal() => Left.EvalInt() - Right.EvalReal();
    }

    public class MinusRI : BinOpNodeI
    {
        public MinusRI(ExprNodeI left, ExprNodeI right) : base(left, right) { }
        public override double EvalReal() => Left.EvalReal() - Right.EvalInt();
    }

    public class MinusRR : BinOpNodeI
    {
        public MinusRR(ExprNodeI left, ExprNodeI right) : base(left, right) { }
        public override double EvalReal() => Left.EvalReal() - Right.EvalReal();
    }

    public class MinusIC : BinOpNodeI
    {
        private int value;
        public MinusIC(ExprNodeI left, int value) : base(left, null) => this.value = value;
        public override int EvalInt() => Left.EvalInt() - value;
    }

    public class MinusRC : BinOpNodeI
    {
        private double value;
        public MinusRC(ExprNodeI left, double value) : base(left, null) => this.value = value;
        public override double EvalReal() => Left.EvalReal() - value;
    }

    // Mult (умножение)
    public class MultII : BinOpNodeI
    {
        public MultII(ExprNodeI left, ExprNodeI right) : base(left, right) { }
        public override int EvalInt() => Left.EvalInt() * Right.EvalInt();
    }

    public class MultIR : BinOpNodeI
    {
        public MultIR(ExprNodeI left, ExprNodeI right) : base(left, right) { }
        public override double EvalReal() => Left.EvalInt() * Right.EvalReal();
    }

    public class MultRI : BinOpNodeI
    {
        public MultRI(ExprNodeI left, ExprNodeI right) : base(left, right) { }
        public override double EvalReal() => Left.EvalReal() * Right.EvalInt();
    }

    public class MultRR : BinOpNodeI
    {
        public MultRR(ExprNodeI left, ExprNodeI right) : base(left, right) { }
        public override double EvalReal() => Left.EvalReal() * Right.EvalReal();
    }

    public class MultIC : BinOpNodeI
    {
        private int value;
        public MultIC(ExprNodeI left, int value) : base(left, null) => this.value = value;
        public override int EvalInt() => Left.EvalInt() * value;
    }

    public class MultRC : BinOpNodeI
    {
        private double value;
        public MultRC(ExprNodeI left, double value) : base(left, null) => this.value = value;
        public override double EvalReal() => Left.EvalReal() * value;
    }

    // Div (деление)
    public class DivII : BinOpNodeI
    {
        public DivII(ExprNodeI left, ExprNodeI right) : base(left, right) { }
        public override double EvalReal() => (double)Left.EvalInt() / Right.EvalInt();
    }

    public class DivIR : BinOpNodeI
    {
        public DivIR(ExprNodeI left, ExprNodeI right) : base(left, right) { }
        public override double EvalReal() => Left.EvalInt() / Right.EvalReal();
    }

    public class DivRI : BinOpNodeI
    {
        public DivRI(ExprNodeI left, ExprNodeI right) : base(left, right) { }
        public override double EvalReal() => Left.EvalReal() / Right.EvalInt();
    }

    public class DivRR : BinOpNodeI
    {
        public DivRR(ExprNodeI left, ExprNodeI right) : base(left, right) { }
        public override double EvalReal() => Left.EvalReal() / Right.EvalReal();
    }

    public class DivIC : BinOpNodeI
    {
        private int value;
        public DivIC(ExprNodeI left, int value) : base(left, null) => this.value = value;
        public override double EvalReal() => (double)Left.EvalInt() / value;
    }

    public class DivRC : BinOpNodeI
    {
        private double value;
        public DivRC(ExprNodeI left, double value) : base(left, null) => this.value = value;
        public override double EvalReal() => Left.EvalReal() / value;
    }

    public class DivRCI : BinOpNodeI
    {
        private double value;
        public DivRCI(double value, ExprNodeI right) : base(null, right) => this.value = value;
        public override double EvalReal() => value / Right.EvalInt();
    }
    // Логическое И (And)
    public class AndBB : BinOpNodeI
    {
        public AndBB(ExprNodeI left, ExprNodeI right) : base(left, right) { }
        public override bool EvalBool() => Left.EvalBool() && Right.EvalBool();
    }

    // Логическое ИЛИ (Or)
    public class OrBB : BinOpNodeI
    {
        public OrBB(ExprNodeI left, ExprNodeI right) : base(left, right) { }
        public override bool EvalBool() => Left.EvalBool() || Right.EvalBool();
    }

    // Меньше (<)
    public class LessII : BinOpNodeI
    {
        public LessII(ExprNodeI left, ExprNodeI right) : base(left, right) { }
        public override bool EvalBool() => Left.EvalInt() < Right.EvalInt();
    }

    public class LessIR : BinOpNodeI
    {
        public LessIR(ExprNodeI left, ExprNodeI right) : base(left, right) { }
        public override bool EvalBool() => Left.EvalInt() < Right.EvalReal();
    }

    public class LessRI : BinOpNodeI
    {
        public LessRI(ExprNodeI left, ExprNodeI right) : base(left, right) { }
        public override bool EvalBool() => Left.EvalReal() < Right.EvalInt();
    }

    public class LessRR : BinOpNodeI
    {
        public LessRR(ExprNodeI left, ExprNodeI right) : base(left, right) { }
        public override bool EvalBool() => Left.EvalReal() < Right.EvalReal();
    }

    public class LessIC : BinOpNodeI
    {
        public int Value { get; }

        public LessIC(ExprNodeI left, int value): base(left, null)
        {
            Value = value;
        }

        public override bool EvalBool()
        {
            return Left.EvalInt() < Value;
        }
    }

    // Меньше или равно (<=)
    public class LessEqII : BinOpNodeI
    {
        public LessEqII(ExprNodeI left, ExprNodeI right) : base(left, right) { }
        public override bool EvalBool() => Left.EvalInt() <= Right.EvalInt();
    }

    public class LessEqIR : BinOpNodeI
    {
        public LessEqIR(ExprNodeI left, ExprNodeI right) : base(left, right) { }
        public override bool EvalBool() => Left.EvalInt() <= Right.EvalReal();
    }

    public class LessEqRI : BinOpNodeI
    {
        public LessEqRI(ExprNodeI left, ExprNodeI right) : base(left, right) { }
        public override bool EvalBool() => Left.EvalReal() <= Right.EvalInt();
    }

    public class LessEqRR : BinOpNodeI
    {
        public LessEqRR(ExprNodeI left, ExprNodeI right) : base(left, right) { }
        public override bool EvalBool() => Left.EvalReal() <= Right.EvalReal();
    }

    // Больше (>)
    public class GreaterII : BinOpNodeI
    {
        public GreaterII(ExprNodeI left, ExprNodeI right) : base(left, right) { }
        public override bool EvalBool() => Left.EvalInt() > Right.EvalInt();
    }

    public class GreaterIR : BinOpNodeI
    {
        public GreaterIR(ExprNodeI left, ExprNodeI right) : base(left, right) { }
        public override bool EvalBool() => Left.EvalInt() > Right.EvalReal();
    }

    public class GreaterRI : BinOpNodeI
    {
        public GreaterRI(ExprNodeI left, ExprNodeI right) : base(left, right) { }
        public override bool EvalBool() => Left.EvalReal() > Right.EvalInt();
    }

    public class GreaterRR : BinOpNodeI
    {
        public GreaterRR(ExprNodeI left, ExprNodeI right) : base(left, right) { }
        public override bool EvalBool() => Left.EvalReal() > Right.EvalReal();
    }

    // Больше или равно (>=)
    public class GreaterEqII : BinOpNodeI
    {
        public GreaterEqII(ExprNodeI left, ExprNodeI right) : base(left, right) { }
        public override bool EvalBool() => Left.EvalInt() >= Right.EvalInt();
    }

    public class GreaterEqIR : BinOpNodeI
    {
        public GreaterEqIR(ExprNodeI left, ExprNodeI right) : base(left, right) { }
        public override bool EvalBool() => Left.EvalInt() >= Right.EvalReal();
    }

    public class GreaterEqRI : BinOpNodeI
    {
        public GreaterEqRI(ExprNodeI left, ExprNodeI right) : base(left, right) { }
        public override bool EvalBool() => Left.EvalReal() >= Right.EvalInt();
    }

    public class GreaterEqRR : BinOpNodeI
    {
        public GreaterEqRR(ExprNodeI left, ExprNodeI right) : base(left, right) { }
        public override bool EvalBool() => Left.EvalReal() >= Right.EvalReal();
    }

    // Равно (=)
    public class EqualII : BinOpNodeI
    {
        public EqualII(ExprNodeI left, ExprNodeI right) : base(left, right) { }
        public override bool EvalBool() => Left.EvalInt() == Right.EvalInt();
    }

    public class EqualRR : BinOpNodeI
    {
        public EqualRR(ExprNodeI left, ExprNodeI right) : base(left, right) { }
        public override bool EvalBool() => Left.EvalReal() == Right.EvalReal();
    }

    public class EqualIR : BinOpNodeI
    {
        public EqualIR(ExprNodeI left, ExprNodeI right) : base(left, right) { }
        public override bool EvalBool() => Left.EvalInt() == Right.EvalReal();
    }

    public class EqualRI : BinOpNodeI
    {
        public EqualRI(ExprNodeI left, ExprNodeI right) : base(left, right) { }
        public override bool EvalBool() => Left.EvalReal() == Right.EvalInt();
    }

    public class EqualBB : BinOpNodeI
    {
        public EqualBB(ExprNodeI left, ExprNodeI right) : base(left, right) { }
        public override bool EvalBool() => Left.EvalBool() == Right.EvalBool();
    }

    // Не равно (<>)
    public class NotEqualII : BinOpNodeI
    {
        public NotEqualII(ExprNodeI left, ExprNodeI right) : base(left, right) { }
        public override bool EvalBool() => Left.EvalInt() != Right.EvalInt();
    }

    public class NotEqualRR : BinOpNodeI
    {
        public NotEqualRR(ExprNodeI left, ExprNodeI right) : base(left, right) { }
        public override bool EvalBool() => Left.EvalReal() != Right.EvalReal();
    }

    public class NotEqualIR : BinOpNodeI
    {
        public NotEqualIR(ExprNodeI left, ExprNodeI right) : base(left, right) { }
        public override bool EvalBool() => Left.EvalInt() != Right.EvalReal();
    }

    public class NotEqualRI : BinOpNodeI
    {
        public NotEqualRI(ExprNodeI left, ExprNodeI right) : base(left, right) { }
        public override bool EvalBool() => Left.EvalReal() != Right.EvalInt();
    }

    public class NotEqualBB : BinOpNodeI
    {
        public NotEqualBB(ExprNodeI left, ExprNodeI right) : base(left, right) { }
        public override bool EvalBool() => Left.EvalBool() != Right.EvalBool();
    }

    public class IntNodeI : ExprNodeI
    {
        public int Val;
        public IntNodeI(int value) => Val = value;
        public override int EvalInt() => Val;
    }

    public class BooleanNodeI : ExprNodeI
    {
        public bool Val;
        public BooleanNodeI(bool value) => Val = value;
        public override bool EvalBool() => Val;
    }

    public class DoubleNodeI : ExprNodeI
    {
        public double Val;
        public DoubleNodeI(double value) => Val = value;
        public override double EvalReal() => Val;
    }

    public unsafe class IdNodeI : ExprNodeI
    {
        public int* pi;
        public IdNodeI(int* pi) => this.pi = pi;
        public override int EvalInt() => *pi;
    }

    public unsafe class IdNodeR : ExprNodeI
    {
        public double* pr;
        public IdNodeR(double* pr) => this.pr = pr;
        public override double EvalReal() => *pr;
    }

    public unsafe class IdNodeB : ExprNodeI
    {
        public bool* pb;
        public IdNodeB(bool* pb) => this.pb = pb;
        public override bool EvalBool() => *pb;
    }

    public class IdNodeFun : ExprNodeI
    {
        public string Name;
        public IdNodeFun(string name) => Name = name;
    }

    public unsafe class AssignIntNodeI : StatementNodeI
    {
        public int* pi;
        public ExprNodeI Expr;

        public AssignIntNodeI(int* pi, ExprNodeI expr)
        {
            this.pi = pi;
            Expr = expr;
        }

        public override void Execute() => *pi = Expr.EvalInt();
    }

    public unsafe class AssignIntCNodeI : StatementNodeI
    {
        public int* pi;
        public int val;

        public AssignIntCNodeI(int* pi, int val)
        {
            this.pi = pi;
            this.val = val;
        }

        public override void Execute() => *pi = val;
    }

    public unsafe class AssignRealNodeI : StatementNodeI
    {
        public double* pr;
        public ExprNodeI Expr;

        public AssignRealNodeI(double* pr, ExprNodeI expr)
        {
            this.pr = pr;
            Expr = expr;
        }

        public override void Execute() => *pr = Expr.EvalReal();
    }

    public unsafe class AssignRealCNodeI : StatementNodeI
    {
        public double* pr;
        public double val;

        public AssignRealCNodeI(double* pr, double val)
        {
            this.pr = pr;
            this.val = val;
        }

        public override void Execute() => *pr = val;
    }

    public unsafe class AssignRealIntCNodeI : StatementNodeI
    {
        public double* pr;
        public int val;

        public AssignRealIntCNodeI(double* pr, int val)
        {
            this.pr = pr;
            this.val = val;
        }

        public override void Execute() => *pr = val;
    }

    public unsafe class AssignBoolNodeI : StatementNodeI
    {
        public bool* pb;
        public ExprNodeI Expr;

        public AssignBoolNodeI(bool* pb, ExprNodeI expr)
        {
            this.pb = pb;
            Expr = expr;
        }

        public override void Execute() => *pb = Expr.EvalBool();
    }
    public unsafe class AssignPlusIntNodeI : AssignIntNodeI
    {
        public AssignPlusIntNodeI(int* pi, ExprNodeI expr) : base(pi, expr) { }
        public override void Execute() => *pi += Expr.EvalInt();
    }

    public unsafe class AssignPlusRealNodeI : AssignRealNodeI
    {
        public AssignPlusRealNodeI(double* pr, ExprNodeI expr) : base(pr, expr) { }
        public override void Execute() => *pr += Expr.EvalReal();
    }

    public unsafe class AssignPlusIntCNodeI : AssignIntCNodeI
    {
        public AssignPlusIntCNodeI(int* pi, int val) : base(pi, val) { }
        public override void Execute() => *pi += val;
    }

    public unsafe class AssignPlusRealCNodeI : AssignRealCNodeI
    {
        public AssignPlusRealCNodeI(double* pr, double val) : base(pr, val) { }
        public override void Execute() => *pr += val;
    }

    public unsafe class AssignPlusRealIntCNodeI : AssignRealIntCNodeI
    {
        public AssignPlusRealIntCNodeI(double* pr, int val) : base(pr, val) { }
        public override void Execute() => *pr += val;
    }

    public class IfNodeI : StatementNodeI
    {
        public ExprNodeI Condition;
        public StatementNodeI ThenStat, ElseStat;

        public IfNodeI(ExprNodeI condition, StatementNodeI thenStat, StatementNodeI elseStat)
        {
            Condition = condition;
            ThenStat = thenStat;
            ElseStat = elseStat;
        }

        public override void Execute()
        {
            if (Condition.EvalBool())
                ThenStat.Execute();
            else if (ElseStat != null)
                ElseStat.Execute();
        }
    }

    public class WhileNodeI : StatementNodeI
    {
        public ExprNodeI Condition;
        public StatementNodeI Stat;

        public WhileNodeI(ExprNodeI condition, StatementNodeI stat)
        {
            Condition = condition;
            Stat = stat;
        }

        public override void Execute()
        {
            while (Condition.EvalBool())
                Stat.Execute();
        }
    }
    public class ProcCallNodeI : StatementNodeI
    {
        public string Name;
        public ExprListNodeI Pars;

        public ProcCallNodeI(string name, ExprListNodeI pars)
        {
            Name = name;
            Pars = pars;
        }

        public override void Execute()
        {
            switch (Name)
            {
                case "PrintReal":
                    Console.WriteLine(Pars.List[0].EvalReal());
                    break;
                case "PrintInt":
                    Console.WriteLine(Pars.List[0].EvalInt());
                    break;
                case "PrintBool":
                    Console.WriteLine(Pars.List[0].EvalBool());
                    break;
            }
        }
    }
}

