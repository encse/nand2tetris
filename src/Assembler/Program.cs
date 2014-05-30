using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Assembler
{
    internal class Ufer : Exception
    {
        public Ufer(string ust) : base(ust)
        {
        }
    }

    internal static class U
    {
        public static string UstMessage(this Exception er)
        {
            if (er is Ufer)
                return er.Message;
            return er.ToString();
        }

        public static string[] ToLines(this string st)
        {
            return st.Split('\n').Select(stT => stT.Replace("\r", "")).ToArray();
        }
    }

    internal class Program
    {
        private static int Main(string[] args)
        {
            try
            {
                if (args.Length != 1)
                    throw new Ufer(AppDomain.CurrentDomain.FriendlyName + " <xxx.asm>");


                var fpatIn = args[0];
                if (!fpatIn.EndsWith(".asm", StringComparison.InvariantCultureIgnoreCase))
                    throw new Ufer("Not an asm file");

                var src = File.ReadAllText(fpatIn);

                var fpatOut = fpatIn.Substring(0, fpatIn.Length - 4) + ".hack";

                var assembler = new Assembler();

                File.WriteAllLines(fpatOut, assembler.Assemble(src));

                return 0;
            }
            catch (Exception er)
            {
                Console.WriteLine(er.UstMessage());
                return 1;
            }
        }

        class Assembler
        {
            const string rxCInstr = "((?<dst>[MDA]+)=)?(?<comp>[^;]+)(;(?<jump>[A-Z]*))?$";
                
            static Dictionary<string, int> flagsBystComp = FlagsBystComp(new Dictionary<string, string>
            {
                // comp   a  c1  c2  c3  c4  c5  c6                               
                {  "0",  "0   1   0   1   0   1   0" },
                {  "1",  "0   1   1   1   1   1   1" },
                { "-1",  "0   1   1   1   0   1   0" },
                {  "D",  "0   0   0   1   1   0   0" },
                {  "A",  "0   1   1   0   0   0   0" },
                {  "M",  "1   1   1   0   0   0   0" },
                { "!D",  "0   0   0   1   1   0   1" },
                { "!A",  "0   1   1   0   0   0   1" },
                { "!M",  "1   1   1   0   0   0   1" },
                { "-D",  "0   0   0   1   1   1   1" },
                { "-A",  "0   1   1   0   0   1   1" },
                { "-M",  "1   1   1   0   0   1   1" },
                {"D+1",  "0   0   1   1   1   1   1" },
                {"A+1",  "0   1   1   0   1   1   1" },
                {"M+1",  "1   1   1   0   1   1   1" },
                {"D-1",  "0   0   0   1   1   1   0" },
                {"A-1",  "0   1   1   0   0   1   0" },
                {"M-1",  "1   1   1   0   0   1   0" },
                {"D+A",  "0   0   0   0   0   1   0" },
                {"D+M",  "1   0   0   0   0   1   0" },
                {"D-A",  "0   0   1   0   0   1   1" },
                {"D-M",  "1   0   1   0   0   1   1" },
                {"A-D",  "0   0   0   0   1   1   1" },
                {"M-D",  "1   0   0   0   1   1   1" },
                {"D&A",  "0   0   0   0   0   0   0" },
                {"D&M",  "1   0   0   0   0   0   0" },
                {"D|A",  "0   0   1   1   1   0   1" },
                {"D|M",  "1   0   1   0   1   0   1" },
            });

            static Dictionary<string, int> FlagsBystComp(Dictionary<string, string> stflagsBystComp)
            {
                var flagsBystComp = new Dictionary<string, int>();
                foreach (var kvp in stflagsBystComp)
                {
                    var stComp = kvp.Key;
                    var stFlags = kvp.Value;

                    flagsBystComp[stComp] = Convert.ToInt32(stFlags.Replace(" ", ""), 2);
                }
                return flagsBystComp;
            }

            public IEnumerable<string> Assemble(string src)
            {
                var syt = new Dictionary<string, int>
                {
                    {"SP", 0},
                    {"LCL", 1},
                    {"ARG", 2},
                    {"THIS", 3},
                    {"THAT", 4},
                    {"R0", 0},
                    {"R1", 1},
                    {"R2", 2},
                    {"R3", 3},
                    {"R4", 4},
                    {"R5", 5},
                    {"R6", 6},
                    {"R7", 7},
                    {"R8", 8},
                    {"R9", 9},
                    {"R10", 10},
                    {"R11", 11},
                    {"R12", 12},
                    {"R13", 13},
                    {"R14", 14},
                    {"R15", 15},
                    {"SCREEN", 0x4000},
                    {"KBD", 0x6000},
                };
                var rginstr = EninstrFromSrc(src, syt).ToList();

                var i = 16;
                foreach (var asymbinstr in rginstr.OfType<AsymbInstr>())
                {
                    if (!syt.ContainsKey(asymbinstr.stSymbol))
                        syt[asymbinstr.stSymbol] = i++;
                }

                foreach (var instr in rginstr)
                    yield return Convert.ToString(instr.ByteCode(syt), 2).PadLeft(16, '0');
            }

            private IEnumerable<Instr> EninstrFromSrc(string src, Dictionary<string, int> syt)
            {
                const string rxLabel = "^\\((?<symbol>[a-zA-Z_$:\\.][0-9a-zA-Z_$:\\.]*)\\)$";

                int istm = 0;
                foreach (var stLineX in src.ToLines())
                {
                    var stLine = stLineX.Replace(" ", "").Replace("\t", "");
                    var ichComment = stLine.IndexOf("//");
                    if (ichComment >= 0)
                        stLine = stLine.Substring(0, ichComment);

                    if(string.IsNullOrWhiteSpace(stLine))
                        continue;

                    var matchLabel = Regex.Match(stLine, rxLabel);
                    if (matchLabel.Success)
                    {
                        syt.Add(matchLabel.Groups["symbol"].Value, istm);
                        continue;
                        
                    }
                    istm++;

                    var instr = AsymbInstr.Parse(stLine) ?? (Instr)AconsInstr.Parse(stLine) ?? Cinstr.Parse(stLine);

                    if (instr == null)
                        throw new Ufer(stLineX);
                    yield return instr;
                }
            } 

            abstract class Instr
            {
                public abstract int ByteCode(Dictionary<string, int> syt);
            }

            class AsymbInstr : Instr
            {
                const string rxSymbol = "^@(?<symbol>[a-zA-Z_$:\\.][0-9a-zA-Z_$:\\.]*)$";

                public readonly string stSymbol;

                private AsymbInstr(string stSymbol)
                {
                    this.stSymbol = stSymbol;
                }

                public override int ByteCode(Dictionary<string, int> syt)
                {
                    return syt[stSymbol];
                }

                public static AsymbInstr Parse(string st)
                {
                    var mc = Regex.Match(st, rxSymbol);
                    if (!mc.Success)
                        return null;

                    return new AsymbInstr(mc.Groups["symbol"].Value);
                }
            }

            class AconsInstr : Instr
            {
                const string rx = "^@(?<val>[0-9]+)$";

                private readonly int val;

                public AconsInstr(int val)
                {
                    this.val = val;
                }

                public override int ByteCode(Dictionary<string, int> syt)
                {
                    return val;
                }

                public static AconsInstr Parse(string st)
                {
                    var mc = Regex.Match(st, rx);
                    if (!mc.Success)
                        return null;

                    return new AconsInstr(int.Parse(mc.Groups["val"].Value));
                }
            }

            class Cinstr : Instr
            {
                private readonly Dst dst;
                private readonly int flags;
                private readonly Jump jump;

                public Cinstr(Dst dst, int flags, Jump jump)
                {
                    this.dst = dst;
                    this.flags = flags;
                    this.jump = jump;
                }

                public override int ByteCode(Dictionary<string, int> syt)
                {
                    return (7 << 13) + (flags << 6) + ((int) dst << 3) + ((int) jump);
                }

                public static Cinstr Parse(string st)
                {
                    var dst = Dst.Nil;
                    var jump = Jump.Nil;

                    var mc = Regex.Match(st, rxCInstr);
                    if (!mc.Success)
                        return null;

                    var stDst = mc.Groups["dst"].Value;
                    var stComp = mc.Groups["comp"].Value;
                    var stJump = mc.Groups["jump"].Value;
                    if (!string.IsNullOrEmpty(stDst))
                    {
                        if (!Dst.TryParse(stDst, out dst))
                            throw new Ufer(stDst);
                    }

                    if (!string.IsNullOrEmpty(stJump))
                    {
                        if (!Dst.TryParse(stJump, out jump))
                            throw new Ufer(stJump);
                    }

                    int flags;
                    if (!flagsBystComp.TryGetValue(stComp, out flags))
                        throw new Ufer(stComp);

                    return new Cinstr(dst, flags, jump);
                }
            }

            private enum Dst
            {
                Nil = 0,
                M = 1,
                D = 2,
                MD = 3,
                A = 4,
                AM = 5,
                AD = 6,
                AMD = 7,
            }

            private enum Jump
            {
                Nil = 0,
                JGT = 1,
                JEQ = 2,
                JGE = 3,
                JLT = 4,
                JNE = 5,
                JLE = 6,
                JMP = 7,
            }
        }
        
    }
}
