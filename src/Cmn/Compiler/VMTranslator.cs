using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cmn.Vm;

namespace Cmn.Compiler
{
    public class VMTranslator
    {

        public class Idgen
        {
            private int i;

            public string filn;

            public string Id()
            {
                return "__lbl" + i++;
            }

            public string Static(int i)
            {
                return Path.GetFileNameWithoutExtension(filn) + "." + i;
            }

            public string Fun(string stFn)
            {
                return stFn;
            }
        }

        public IEnumerable<string> ToAsm(IEnumerable<string> rgfpat)
        {
            var idgen = new Idgen();
            foreach (var st in Indent(Bootstrap(idgen)))
                yield return st;
            
            foreach (var fpat in rgfpat)
            {
                var src = File.ReadAllText(fpat);
                idgen.filn = Path.GetFileName(fpat);
                foreach (var st in Indent(ToAsmI(idgen, src)))
                    yield return st;
                idgen.filn = null;
            }
            
        }

        private IEnumerable<string> Bootstrap(Idgen idgen)
        {
            yield return "//SP=256";
            yield return "@256";
            yield return "D=A";
            yield return "@0";
            yield return "M=D";
            idgen.filn = "Sys.vm";
            foreach (var st in ProcessCall(new Vmcmd(Kcmd.Call, Ksegment.Nil, 0, "Sys.init"), idgen))
                yield return st;
            idgen.filn = null;

        }

        public IEnumerable<string> Indent(IEnumerable<string> src)
        {
            foreach (var st in src)
            {
                if (!st.StartsWith("(") && !st.StartsWith("//"))
                    yield return "\t" + st;
                else
                    yield return st;
            }
        }

        private IEnumerable<string> ToAsmI(Idgen idgen, string src)
        {
            foreach (var vmcmd in EnvmcdParse(src))
            {
                yield return "//" + vmcmd.Unparse();

                Func<Vmcmd, Idgen, IEnumerable<string>> dg;
                switch (vmcmd.Kcmd)
                {
                    case Kcmd.Label: dg = ProcessLabel; break;
                    case Kcmd.Goto: dg = ProcessGoto; break;
                    case Kcmd.IfGoto: dg = ProcessIfGoto; break;
                    case Kcmd.Add:
                    case Kcmd.Sub:
                    case Kcmd.And:
                    case Kcmd.Or:
                        dg = ProcessAddSubAndOr;
                        break;
                    case Kcmd.Neg:
                    case Kcmd.Not:
                        dg = ProcessNegNot;
                        break;
                    case Kcmd.Eq:
                    case Kcmd.Gt:
                    case Kcmd.Lt:
                        dg = ProcessEqGtLt;
                        break;
                    case Kcmd.Push:
                        dg = ProcessPush;
                        break;
                    case Kcmd.Pop:
                        dg = ProcessPop;
                        break;
                    case Kcmd.Call:
                        dg = ProcessCall;
                        break;
                    case Kcmd.Function:
                        dg = ProcessFunction;
                        break;
                    case Kcmd.Return:
                        dg = ProcessReturn;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                foreach (var st in dg(vmcmd, idgen))
                    yield return st;
            }
        }

        private static IEnumerable<string> ProcessReturn(Vmcmd vmcmd, Idgen idgen)
        {
            yield return "//FRAME = LCL";
            yield return "@LCL";
            yield return "D=M";
            yield return "@R13";
            yield return "M=D";

            yield return "//RET = *(FRAME-5)";
            yield return "@5";
            yield return "A=D-A";
            yield return "D=M";
            yield return "@R14";
            yield return "M=D";

            yield return "//*ARG = pop()";
            yield return "@SP";
            yield return "A=M-1";
            yield return "D=M";
            yield return "@ARG";
            yield return "A=M";
            yield return "M=D";

            yield return "//SP=ARG+1";
            yield return "@ARG";
            yield return "D=M+1";
            yield return "@SP";
            yield return "M=D";

            int k = 1;
            foreach (var lbl in new[] {"THAT", "THIS", "ARG", "LCL"})
            {
                yield return "//{0} = *(FRAME-{1})".StFormat(lbl, k++);
                yield return "@R13";
                yield return "M=M-1";
                yield return "A=M";
                yield return "D=M";
                yield return "@" + lbl;
                yield return "M=D";
            }

            yield return "@R14";
            yield return "A=M";
            yield return "D;JMP";
        }

        private static IEnumerable<string> ProcessFunction(Vmcmd vmcmd, Idgen idgen)
        {
            yield return "(" + idgen.Fun(vmcmd.StFn) + ")";

            for (int k = 0; k < vmcmd.I; k++)
            {
                yield return "//push 0";
                yield return "@SP";
                yield return "M=M+1";
                yield return "A=M-1";
                yield return "M=0";
            }
        }

        private static IEnumerable<string> ProcessCall(Vmcmd vmcmd, Idgen idgen)
        {
            var lblNext = idgen.Id();

            yield return "//push " + lblNext;
            yield return "@" + lblNext;
            yield return "D=A";

            yield return "@SP";
            yield return "M=M+1";
            yield return "A=M-1";
            yield return "M=D";

            foreach (var lbl in new[] {"LCL", "ARG", "THIS", "THAT"})
            {
                yield return "//push " + lbl;
                yield return "@" + lbl;
                yield return "D=M";

                yield return "@SP";
                yield return "M=M+1";
                yield return "A=M-1";
                yield return "M=D";
            }

            yield return "//ARG=SP-n-5";
            yield return "@SP";
            yield return "D=M";
            yield return "@" + (vmcmd.I + 5);
            yield return "D=D-A";
            yield return "@ARG";
            yield return "M=D";

            yield return "//LCL=SP";
            yield return "@SP";
            yield return "D=M";
            yield return "@LCL";
            yield return "M=D";

            yield return "@" + idgen.Fun(vmcmd.StFn);
            yield return "D;JMP";

            yield return "(" + lblNext + ")";
        }

        private static IEnumerable<string> ProcessPop(Vmcmd vmcmd, Idgen idgen)
        {
            switch (vmcmd.Ksegment)
            {
                case Ksegment.Local:
                    yield return "@LCL";
                    yield return "D=M";
                    break;
                case Ksegment.Argument:
                    yield return "@ARG";
                    yield return "D=M";
                    break;
                case Ksegment.This:
                    yield return "@THIS";
                    yield return "D=M";
                    break;
                case Ksegment.That:
                    yield return "@THAT";
                    yield return "D=M";
                    break;
                case Ksegment.Temp:
                    yield return "@R5";
                    yield return "D=A";
                    break;
                case Ksegment.Pointer:
                    yield return "@THIS";
                    yield return "D=A";
                    break;
                case Ksegment.Static:
                    yield return "@" + idgen.Static(vmcmd.I);
                    yield return "D=A";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }


            if (vmcmd.Ksegment != Ksegment.Static)
            {
                yield return "@" + vmcmd.I;
                yield return "D=D+A";
            }

            yield return "@R13";
            yield return "M=D";


            yield return "@SP"; //a = &sp
            yield return "M=M-1"; //sp--
            yield return "A=M"; //a = sp
            yield return "D=M"; //d = sp[0]

            yield return "@R13";
            yield return "A=M";
            yield return "M=D";
        }

        private static IEnumerable<string> ProcessPush(Vmcmd vmcmd, Idgen idgen)
        {
            switch (vmcmd.Ksegment)
            {
                case Ksegment.Constant:
                    yield return "@" + vmcmd.I;
                    yield return "D=A"; //d = vmcmd.I
                    break;
                case Ksegment.Local:
                    yield return "@LCL";
                    yield return "D=M"; //d = vmcmd.I
                    yield return "@" + vmcmd.I;
                    yield return "A=D+A"; //d = vmcmd.I
                    yield return "D=M";
                    break;
                case Ksegment.Argument:
                    yield return "@ARG";
                    yield return "D=M"; //d = vmcmd.I
                    yield return "@" + vmcmd.I;
                    yield return "A=D+A"; //d = vmcmd.I
                    yield return "D=M";
                    break;
                case Ksegment.This:
                    yield return "@THIS";
                    yield return "D=M"; //d = vmcmd.I
                    yield return "@" + vmcmd.I;
                    yield return "A=D+A"; //d = vmcmd.I
                    yield return "D=M";
                    break;
                case Ksegment.That:
                    yield return "@THAT";
                    yield return "D=M"; //d = vmcmd.I
                    yield return "@" + vmcmd.I;
                    yield return "A=D+A"; //d = vmcmd.I
                    yield return "D=M";
                    break;
                case Ksegment.Temp:
                    yield return "@R5";
                    yield return "D=A"; //d = vmcmd.I
                    yield return "@" + vmcmd.I;
                    yield return "A=D+A"; //d = vmcmd.I
                    yield return "D=M";
                    break;
                case Ksegment.Pointer:
                    yield return "@THIS";
                    yield return "D=A"; //d = vmcmd.I
                    yield return "@" + vmcmd.I;
                    yield return "A=D+A"; //d = vmcmd.I
                    yield return "D=M";
                    break;
                case Ksegment.Static:
                    yield return "@" + idgen.Static(vmcmd.I);
                    yield return "D=M";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            yield return "@SP"; //a = &sp
            yield return "M=M+1"; //sp++
            yield return "A=M-1"; //a = sp-1
            yield return "M=D"; //sp[-1] = d
        }

        private static IEnumerable<string> ProcessEqGtLt(Vmcmd vmcmd, Idgen idgen)
        {
            yield return "@SP"; //a = &sp
            yield return "A=M-1"; //a = sp - 1
            yield return "D=M"; //d = sp[1]

            yield return "A=A-1"; //a = sp - 2

            yield return "D=M-D";

            var idTrue = idgen.Id();
            var idAfter = idgen.Id();

            yield return "@" + idTrue;

            switch (vmcmd.Kcmd)
            {
                case Kcmd.Eq:
                    yield return "D;JEQ";
                    break;
                case Kcmd.Gt:
                    yield return "D;JGT";
                    break;
                case Kcmd.Lt:
                    yield return "D;JLT";
                    break;
            }
            yield return "@" + idAfter;
            yield return "D=0;JEQ";
            yield return "(" + idTrue + ")";
            yield return "D=-1";
            yield return "(" + idAfter + ")";

            yield return "@SP"; //a = &sp
            yield return "M=M-1"; //sp--
            yield return "A=M-1"; //a = sp-1
            yield return "M=D"; //sp[-1] = d
        }

        private static IEnumerable<string> ProcessNegNot(Vmcmd vmcmd, Idgen idgen)
        {
            yield return "@SP"; //a = &sp
            yield return "A=M-1"; //a = sp - 1
            if (vmcmd.Kcmd == Kcmd.Neg)
                yield return "M=-M"; //d = -sp[-1]
            else
                yield return "M=!M"; //d = !sp[-1]
        }

        private static IEnumerable<string> ProcessAddSubAndOr(Vmcmd vmcmd, Idgen idgen)
        {
            yield return "@SP"; //a = &sp
            yield return "A=M-1"; //a = sp - 1
            yield return "D=M"; //d = sp[-1]

            yield return "A=A-1"; //a = sp - 2

            switch (vmcmd.Kcmd)
            {
                case Kcmd.Add:
                    yield return "M=D+M"; //sp[-2] = sp[-1] + sp[-2]
                    break;
                case Kcmd.Sub:
                    yield return "M=M-D"; //sp[-2] = sp[-2] - sp[-1]
                    break;
                case Kcmd.And:
                    yield return "M=D&M"; //sp[-2] = sp[-1] + sp[-2]
                    break;
                case Kcmd.Or:
                    yield return "M=D|M"; //sp[-2] = sp[-1] + sp[-2]
                    break;
            }

            yield return "@SP"; //a = &sp
            yield return "M=M-1"; //sp = sp - 1
        }

        private static IEnumerable<string> ProcessIfGoto(Vmcmd vmcmd, Idgen idgen)
        {
            yield return "@SP";
            yield return "M=M-1";
            yield return "A=M";
            yield return "D=M";

            yield return "@" + vmcmd.StLabel;
            yield return "D;JNE";
        }

        private static IEnumerable<string> ProcessGoto(Vmcmd vmcmd, Idgen idgen)
        {
            yield return "@" + vmcmd.StLabel;
            yield return "D;JMP";
        }

        private static IEnumerable<string> ProcessLabel(Vmcmd vmcmd, Idgen idgen)
        {
            yield return "(" + vmcmd.StLabel + ")";
        }

        private IEnumerable<Vmcmd> EnvmcdParse(string src)
        {
            return src.ToLinesSkipComments().Select(stLine => Vmcmd.Fetch(stLine.ToWords()));
        }
    }
}