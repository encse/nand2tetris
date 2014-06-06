using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Cmn.Asm;

namespace Cmn.Compiler
{
    public class Assembler
    {
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
            foreach (var asymbinstr in rginstr.OfType<AInstructionWithSymbol>())
            {
                if (!syt.ContainsKey(asymbinstr.stSymbol))
                    syt[asymbinstr.stSymbol] = i++;
            }

            foreach (var instr in rginstr)
                yield return Convert.ToString(instr.Opcode(syt), 2).PadLeft(16, '0');
        }

        private IEnumerable<Instruction> EninstrFromSrc(string src, Dictionary<string, int> syt)
        {
            const string rxLabel = "^\\((?<symbol>[a-zA-Z_$:\\.][0-9a-zA-Z_$:\\.]*)\\)$";

            int istm = 0;
            foreach (var stLineX in src.ToLines())
            {
                var stLine = stLineX.Replace(" ", "").Replace("\t", "").SkipComment();

                if (string.IsNullOrWhiteSpace(stLine))
                    continue;

                var matchLabel = Regex.Match(stLine, rxLabel);
                if (matchLabel.Success)
                {
                    syt.Add(matchLabel.Groups["symbol"].Value, istm);
                    continue;
                }
                istm++;

                var instr = AInstructionWithSymbol.Parse(stLine) ?? (Instruction)AInstructionWithConstant.Parse(stLine) ?? CInstruction.Parse(stLine);

                if (instr == null)
                    throw new Ufer(stLineX);
                yield return instr;
            }
        }
    }
}
