using System;

namespace Cmn.Vm
{
    enum Kcmd
    {
        Add,
        Sub,
        Neg,
        Eq,
        Gt,
        Lt,
        And,
        Or,
        Not,
        Pop,
        Push
    }

    enum Ksegment
    {
        Nil, Constant, Local, Argument, That, Pointer, Temp, This, Static
    }

    class Vmcmd
    {
        public Kcmd kcmd;
        public Ksegment Ksegment;
        public int I;

        public Vmcmd(Kcmd kcmd, Ksegment ksegment, int i)
        {
            this.kcmd = kcmd;
            this.Ksegment = ksegment;
            this.I = i;
        }

        public static Vmcmd Fetch(string[] st)
        {
            var kcmd = U.ParseKIgnoreCase<Kcmd>(st[0]);
            var ksegment = st.Length > 1 ? U.ParseKIgnoreCase<Ksegment>(st[1]) : Ksegment.Nil;
            var i = st.Length > 2 ? int.Parse(st[2]) : 0;

            return new Vmcmd(kcmd, ksegment, i);
        }

        public string Unparse()
        {
            switch (kcmd)
            {
                case Kcmd.Add:
                case Kcmd.Sub:
                case Kcmd.Neg:
                case Kcmd.Eq:
                case Kcmd.Gt:
                case Kcmd.Lt:
                case Kcmd.And:
                case Kcmd.Or:
                case Kcmd.Not:
                    return kcmd.ToString();
                case Kcmd.Pop:
                case Kcmd.Push:
                    return kcmd +" " + Ksegment+" " +I;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
