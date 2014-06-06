using System.Collections.Generic;

namespace Cmn.Asm
{
    abstract class Instruction
    {
        public abstract int Opcode(Dictionary<string, int> syt);
    }

}