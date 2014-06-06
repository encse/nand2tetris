using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Cmn.Asm
{
    class AInstructionWithSymbol : Instruction
    {
        const string rxSymbol = "^@(?<symbol>[a-zA-Z_$:\\.][0-9a-zA-Z_$:\\.]*)$";

        public readonly string stSymbol;

        private AInstructionWithSymbol(string stSymbol)
        {
            this.stSymbol = stSymbol;
        }

        public override int Opcode(Dictionary<string, int> syt)
        {
            return syt[stSymbol];
        }

        public static AInstructionWithSymbol Parse(string st)
        {
            var mc = Regex.Match(st, rxSymbol);
            if (!mc.Success)
                return null;

            return new AInstructionWithSymbol(mc.Groups["symbol"].Value);
        }
    }
}