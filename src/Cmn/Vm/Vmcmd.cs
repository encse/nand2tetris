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

        [Mnemonic("call")]Call,
        [Mnemonic("function")]Function,
        [Mnemonic("return")]Return,
    }

    enum Ksegment
    {
        Nil, Constant, Local, Argument, That, Pointer, Temp, This, Static
    }

    class Vmcmd
    {
        public readonly Kcmd Kcmd;
        public readonly string StLabel;
        public string StFn { get { return StLabel; } }
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
            switch (kcmd)
            {
                case Kcmd.Goto: 
                case Kcmd.IfGoto:    
                case Kcmd.Label:   
                    return new Vmcmd(kcmd, Ksegment.Nil, 0, st[1]);
                case Kcmd.Return:
                case Kcmd.Add:
                case Kcmd.Sub:   
                case Kcmd.Neg:  
                case Kcmd.Eq:  
                case Kcmd.Gt:  
                case Kcmd.Lt:  
                case Kcmd.And:  
                case Kcmd.Or:  
                case Kcmd.Not:  
                    return new Vmcmd(kcmd, Ksegment.Nil, 0, null);
                case Kcmd.Pop:  
                case Kcmd.Push:  
                    return new Vmcmd(kcmd, U.ParseKIgnoreCase<Ksegment>(st[1]), int.Parse(st[2]), null);
                case Kcmd.Call:
                case Kcmd.Function:
                    return new Vmcmd(kcmd, Ksegment.Nil, int.Parse(st[2]), st[1]);
                default:
                    throw new ArgumentException(kcmd.ToString());
            }
        }

        public string Unparse()
        {
            switch (Kcmd)
            {
                case Kcmd.Return:
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
                    return Kcmd.ToMnemonic() + " " + Ksegment.ToString().ToLowerInvariant() + " " + I;
                case Kcmd.Goto:
                case Kcmd.IfGoto:
                case Kcmd.Label:
                    return Kcmd.ToMnemonic() + " " + StLabel;
                case Kcmd.Function:
                case Kcmd.Call:
                    return Kcmd.ToMnemonic() + " " + StFn + " " + I;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
