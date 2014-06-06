using System;
using System.IO;
using Cmn;
using Cmn.Compiler;

namespace NsAssembler
{
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

      
    }
}
