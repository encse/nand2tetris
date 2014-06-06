using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Cmn.Asm
{
    enum Kdst
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

    enum Kjmp
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

    class CInstruction : Instruction
    {
        const string rxCInstr = "((?<dst>[MDA]+)=)?(?<comp>[^;]+)(;(?<jump>[A-Z]*))?$";

        class MnemonicAttribute : Attribute
        {
            public string st;

            public MnemonicAttribute(string st)
            {
                this.st = st;
            }
        }
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

        enum Comp
        {
            [Mnemonic("0")]Zero,
            [Mnemonic("1")]One,
            [Mnemonic("-1")]MinusOne,
            [Mnemonic("D")]D,
            [Mnemonic("A")]A,
            [Mnemonic("M")]M,
            [Mnemonic("!D")]NotD,
            [Mnemonic("!A")]NotA,
            [Mnemonic("!M")]NotM,
            [Mnemonic("-D")]MinusD,
            [Mnemonic("-A")]MinusA,
            [Mnemonic("-M")]MinusM,
            [Mnemonic("D+1")]DPlusOne,
            [Mnemonic("A+1")]APlusOne,
            [Mnemonic("M+1")]MPlusOne,
            [Mnemonic("D-1")]DMinusOne,
            [Mnemonic("A-1")]AMinusOne,
            [Mnemonic("M-1")]MMinusOne,
            [Mnemonic("D+A")]DPlusA,
            [Mnemonic("D+M")]DPlusM,
            [Mnemonic("D-A")]DMinusA,
            [Mnemonic("D-M")]DMinusM,
            [Mnemonic("A-D")]AMinusD,
            [Mnemonic("M-D")]MMinusD,
            [Mnemonic("D&A")]DAndA,
            [Mnemonic("D&M")]DAndM,
            [Mnemonic("D|A")]DOrA,
            [Mnemonic("D|M")]DOrM,
        }

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

        private readonly Kdst kdst;
        private readonly int flags;
        private readonly Kjmp kjmp;

        public CInstruction(Kdst kdst, int flags, Kjmp kjmp)
        {
            this.kdst = kdst;
            this.flags = flags;
            this.kjmp = kjmp;
        }

        public override int Opcode(Dictionary<string, int> syt)
        {
            return (7 << 13) + (flags << 6) + ((int)kdst << 3) + ((int)kjmp);
        }

        public static CInstruction Parse(string st)
        {
            var dst = Kdst.Nil;
            var jump = Kjmp.Nil;

            var mc = Regex.Match(st, rxCInstr);
            if (!mc.Success)
                return null;

            var stDst = mc.Groups["dst"].Value;
            var stComp = mc.Groups["comp"].Value;
            var stJump = mc.Groups["jump"].Value;
            if (!String.IsNullOrEmpty(stDst))
            {
                if (!Enum.TryParse(stDst, out dst))
                    throw new Ufer(stDst);
            }

            if (!String.IsNullOrEmpty(stJump))
            {
                if (!Enum.TryParse(stJump, out jump))
                    throw new Ufer(stJump);
            }

            int flags;
            if (!flagsBystComp.TryGetValue(stComp, out flags))
                throw new Ufer(stComp);

            return new CInstruction(dst, flags, jump);
        }
    }
}