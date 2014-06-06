using System;
using Cmn.Compiler;

namespace Cmn.Vm
{
    enum Kcmd
    {
        [Mnemonic("add")]Add,
        [Mnemonic("sub")]Sub,
        [Mnemonic("neq")]Neg,
        [Mnemonic("eq")]Eq,
        [Mnemonic("gt")]Gt,
        [Mnemonic("lt")]Lt,
        [Mnemonic("and")]And,
        [Mnemonic("or")]Or,
        [Mnemonic("not")]Not,
        [Mnemonic("pop")]Pop,
        [Mnemonic("push")]Push,
        [Mnemonic("label")]Label,
        [Mnemonic("goto")]Goto,
        [Mnemonic("if-goto")]IfGoto,
    }

    enum Ksegment
    {
        Nil, Constant, Local, Argument, That, Pointer, Temp, This, Static
    }

    class Vmcmd
    {
        public readonly Kcmd Kcmd;
        public readonly string StLabel;
        public readonly Ksegment Ksegment;
        public readonly int I;

        public Vmcmd(Kcmd kcmd, Ksegment ksegment, int i, string stLabel)
        {
            Kcmd = kcmd;
            Ksegment = ksegment;
            I = i;
            StLabel = stLabel;
        }

        public static Vmcmd Fetch(string[] st)
        {
            var kcmd = U.KFromMnemonic<Kcmd>(st[0]);
            if (kcmd == Kcmd.Goto || kcmd == Kcmd.IfGoto || kcmd == Kcmd.Label)
                return new Vmcmd(kcmd, Vm.Ksegment.Nil, 0, st[1]);
            
            var ksegment = st.Length > 1 ? U.ParseKIgnoreCase<Ksegment>(st[1]) : Ksegment.Nil;
            var i = st.Length > 2 ? int.Parse(st[2]) : 0;
            return new Vmcmd(kcmd, ksegment, i, null);
            
        }

        public string Unparse()
        {
            switch (Kcmd)
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
                    return Kcmd.ToMnemonic();
                case Kcmd.Pop:
                case Kcmd.Push:
                    return Kcmd.ToMnemonic() + " " + Ksegment + " " + I;
                case Kcmd.Goto:
                case Kcmd.IfGoto:
                case Kcmd.Label:
                    return Kcmd.ToMnemonic() + " " + StLabel;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
