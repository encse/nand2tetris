using System;

namespace Cmn.Compiler
{
    internal class MnemonicAttribute : Attribute
    {
        public string st;

        public MnemonicAttribute(string st)
        {
            this.st = st;
        }

      
    }
}