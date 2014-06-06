using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Cmn.Asm
{
    class AInstructionWithConstant : Instruction
    {
        const string rx = "^@(?<val>[0-9]+)$";

        private readonly int val;

        public AInstructionWithConstant(int val)
        {
            this.val = val;
        }

        public override int Opcode(Dictionary<string, int> syt)
        {
            return val;
        }

        public static AInstructionWithConstant Parse(string st)
        {
            var mc = Regex.Match(st, rx);
            if (!mc.Success)
                return null;

            return new AInstructionWithConstant(Int32.Parse(mc.Groups["val"].Value));
        }
    }
}