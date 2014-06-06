using System;
using System.Collections.Generic;
using System.Linq;
using Cmn.Asm;
using Cmn.Vm;

namespace Cmn.Compiler
{
    public class VMTranslator
    {

        public class Idgen
        {
            private int i;

            public string Id()
            {
                return "l" + i++;
            }
        }

        public IEnumerable<string> ToAsm(string src)
        {
            return Indent(ToAsmI(src));
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

        public IEnumerable<string> ToAsmI(string src)
        {
            var idgen = new Idgen();
            foreach (var vmcmd in EnvmcdParse(src))
            {
                yield return "//" + vmcmd.Unparse();
                        
                switch (vmcmd.kcmd)
                {
                    case Kcmd.Add:
                    case Kcmd.Sub:
                    case Kcmd.And:
                    case Kcmd.Or:
                    {
                        yield return "@SP"; //a = &sp
                        yield return "A=M-1"; //a = sp - 1
                        yield return "D=M"; //d = sp[-1]

                        yield return "A=A-1"; //a = sp - 2

                        switch (vmcmd.kcmd)
                        {
                            case Kcmd.Add:
                                yield return "M=D+M";  //sp[-2] = sp[-1] + sp[-2]
                                break;
                            case Kcmd.Sub:
                                yield return "M=M-D";  //sp[-2] = sp[-2] - sp[-1]
                                break;
                            case Kcmd.And:
                                yield return "M=D&M";  //sp[-2] = sp[-1] + sp[-2]
                                break;
                            case Kcmd.Or:
                                yield return "M=D|M";  //sp[-2] = sp[-1] + sp[-2]
                                break;
                        }

                        yield return "@SP";   //a = &sp
                        yield return "M=M-1";   //sp = sp - 1
                        break;
                    }
                    case Kcmd.Neg:
                    case Kcmd.Not:
                    {
                        yield return "@SP"; //a = &sp
                        yield return "A=M-1"; //a = sp - 1
                        if (vmcmd.kcmd == Kcmd.Neg)
                            yield return "M=-M"; //d = -sp[-1]
                        else
                            yield return "M=!M"; //d = !sp[-1]
                        break;
                    }

                    case Kcmd.Eq:
                    case Kcmd.Gt:
                    case Kcmd.Lt:
                    {
                        yield return "@SP"; //a = &sp
                        yield return "A=M-1"; //a = sp - 1
                        yield return "D=M"; //d = sp[1]

                        yield return "A=A-1"; //a = sp - 2

                        yield return "D=M-D";

                        var idTrue = idgen.Id();
                        var idAfter = idgen.Id();
                        
                        yield return "@" + idTrue;
                        
                        switch (vmcmd.kcmd)
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

                        break;
                    }
                    case Kcmd.Push:
                    {
                        switch (vmcmd.Ksegment)
                        {
                            case Ksegment.Constant:
                                yield return "@" + vmcmd.I;
                                yield return "D=A"; //d = vmcmd.I

                                yield return "@SP"; //a = &sp
                                yield return "A=M"; //a = sp
                                yield return "M=D"; //*sp = d

                                yield return "D=A+1"; //d = sp + 1
                                yield return "@SP";   //a = &sp
                                yield return "M=D";   //sp = sp + 1
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                        break;
                    }
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private IEnumerable<Vmcmd> EnvmcdParse(string src)
        {
            return src.ToLinesSkipComments().Select(stLine => Vmcmd.Fetch(stLine.ToWords()));
        }
    }
}