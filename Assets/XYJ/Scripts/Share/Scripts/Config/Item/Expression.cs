using System.Collections.Generic;

namespace Config
{
    public interface IParam
    {
        int this[string name] { get; }
    }

    public interface IExpression
    {
        double GetValue(IParam param);
        string toString();
    }

    public class DoubleValue : IExpression
    {
        public DoubleValue(double value)
        {
            this.value = value;
        }

        public double value;

        public double GetValue(IParam param)
        {
            return value;
        }

        public string toString()
        {
            return value.ToString("0.00");
        }
    }

    public class ParamValue : IExpression
    {
        public ParamValue(string value)
        {
            this.value = value;
        }

        public string value { get; private set; }

        public double GetValue(IParam param)
        {
            return param[value];
        }

        public string toString()
        {
            return value;
        }
    }

    // +-*/支持加减乘除运算
    public class AddExp : IExpression
    {
        public IExpression left; // 左运算
        public IExpression right; // 右运算

        public double GetValue(IParam param)
        {
            return left.GetValue(param) + right.GetValue(param);
        }

        public string toString()
        {
            return string.Format("{0} + {1}", left.toString(), right.toString());
        }
    }

    public class SubExp : IExpression
    {
        public IExpression left; // 左运算
        public IExpression right; // 右运算

        public double GetValue(IParam param)
        {
            return left.GetValue(param) - right.GetValue(param);
        }

        public string toString()
        {
            return string.Format("{0} - {1}", left.toString(), right.toString());
        }
    }

    public class MulExp : IExpression
    {
        public IExpression left; // 左运算
        public IExpression right; // 右运算

        public double GetValue(IParam param)
        {
            return left.GetValue(param) * right.GetValue(param);
        }

        public string toString()
        {
            return string.Format("{0} * {1}", left.toString(), right.toString());
        }
    }

    public class DivExp : IExpression
    {
        public IExpression left; // 左运算
        public IExpression right; // 右运算

        public double GetValue(IParam param)
        {
            return left.GetValue(param) / right.GetValue(param);
        }

        public string toString()
        {
            return string.Format("{0} / {1}", left.toString(), right.toString());
        }
    }

    // 操作类型
    enum OptType
    {
        Null, // 无效
        Add, // +
        Sub, // -
        Mul, // *
        Div, // /
        Left, // (
        Right, // )
        Value, // 实际数值
    }

    // 临时结点
    struct Node
    {
        public OptType type;
        public IExpression exp;

        public override string ToString()
        {
            return string.Format("type:{0} exp:{1}", type, exp == null ? "null" : exp.toString());
        }
    }

    // 表达式
    public struct Expression : IExpression
    {
        static int Priority(OptType type)
        {
            switch (type)
            {
            case OptType.Add:
            case OptType.Sub:
                return 1;
            case OptType.Mul:
            case OptType.Div:
                return 2;
            case OptType.Left:
                return 100;
            }

            throw new System.Exception("");
        }

        IExpression exp;

        public bool isVaild { get { return exp == null ? false : true; } }

        public string toString()
        {
            return string.Format("exp:{0}", exp == null ? "null" : exp.toString());
        }

        public double GetValue(IParam param)
        {
            return exp.GetValue(param);
        }

        static bool isOpt(char c, out OptType type)
        {
            switch (c)
            {
            case '+':
                type = OptType.Add;
                return true;
            case '-':
                type = OptType.Sub;
                return true;
            case '*':
                type = OptType.Mul;
                return true;
            case '/':
                type = OptType.Div;
                return true;
            case '(':
                type = OptType.Left;
                return true;
            case ')':
                type = OptType.Right;
                return true;
            }

            type = OptType.Null;
            return false;
        }

        static IExpression CreateValue(string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;

            for (int i = 0; i < value.Length; ++i)
            {
                if (value[i] >= '0' && value[i] <= '9')
                    continue;
                if (value[i] == '.' || value[i] == '-' || value[i] == '+')
                    continue;

                // 参数类型的
                return new ParamValue(value);
            }

            // 全部是数字
            return new DoubleValue(double.Parse(value));
        }

        // 表达式与操作符
        static List<Node> CreateExp(string text)
        {
            List<Node> exps = new List<Node>();
            Stack<OptType> opts = new Stack<OptType>();
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for (int i = 0; i < text.Length; ++i)
            {
                OptType type = OptType.Null;
                char c = text[i];
                if (isOpt(c, out type))
                {
                    // 符号
                    if (sb.Length != 0)
                    {
                        exps.Add(new Node() { type = OptType.Value, exp = CreateValue(sb.ToString()) });
                        sb.Length = 0;
                    }

                    switch (type)
                    {
                    case OptType.Left:
                        {
                            opts.Push(OptType.Left);
                        }
                        break;
                    case OptType.Right:
                        {
                            while (true)
                            {
                                var v = opts.Pop();
                                if (v == OptType.Left)
                                {
                                    break;
                                }
                                else
                                {
                                    exps.Add(new Node() { type = v });
                                }
                            }
                        }
                        break;
                    default:
                        {
                            if (opts.Count == 0)
                            {
                                opts.Push(type);
                            }
                            else
                            {
                                var otype = opts.Peek();
                                if (otype == OptType.Left)
                                {

                                }
                                else
                                {
                                    int cv = Priority(opts.Peek());
                                    int nv = Priority(type);
                                    if (nv > cv)
                                    {

                                    }
                                    else
                                    {
                                        // 要新增加的符号，优先级高于等于当前栈顶的，把当前的栈加入到表达式当中
                                        exps.Add(new Node() { type = opts.Pop() });
                                    }
                                }

                                opts.Push(type);
                            }
                        }
                        break;
                    }
                }
                else
                {
                    if (c == ' ')
                        continue;

                    sb.Append(c);
                }
            }

            if (sb.Length != 0)
            {
                exps.Add(new Node() { type = OptType.Value, exp = CreateValue(sb.ToString()) });
            }

            while (opts.Count != 0)
            {
                exps.Add(new Node() { type = opts.Pop() });
            }

            return exps;
        }

        static IExpression Create(OptType type, IExpression left, IExpression right)
        {
            switch (type)
            {
            case OptType.Add: return new AddExp() { left = left, right = right };
            case OptType.Sub: return new SubExp() { left = left, right = right };
            case OptType.Mul: return new MulExp() { left = left, right = right };
            case OptType.Div: return new DivExp() { left = left, right = right };
            }

            throw new System.Exception("type:" + type);
        }

        public static Expression InitConfig(string text)
        {
            if (string.IsNullOrEmpty(text))
                return new Expression();

            // 转换成逆波兰形式
            List<Node> nodes = CreateExp(text);
            Stack<IExpression> exps = new Stack<IExpression>();
            for (int i = 0; i < nodes.Count; ++i)
            {
                switch (nodes[i].type)
                {
                case OptType.Value:
                    exps.Push(nodes[i].exp);
                    break;
                default:
                    {
                        var nright = exps.Pop();
                        var nleft = exps.Pop();

                        IExpression n = Create(nodes[i].type, nleft, nright);
                        exps.Push(n);
                    }
                    break;
                }
            }

            return new Expression() { exp = exps.Pop() };
        }
    }
}


