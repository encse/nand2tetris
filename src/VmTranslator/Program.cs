using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Cmn;
using Cmn.Compiler;

namespace NsVmTranslator
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
                throw new Ufer(AppDomain.CurrentDomain.FriendlyName + " <xxx.vm>");

            var fpatIn = args[0];
            if (!fpatIn.EndsWith(".vm", StringComparison.InvariantCultureIgnoreCase))
                throw new Ufer("Not a vm file");

            var src = File.ReadAllText(fpatIn);

            var fpatOut = fpatIn.Substring(0, fpatIn.Length - ".vm".Length) + ".asm";
            var filn = Path.GetFileName(fpatIn);
            var translator = new VMTranslator();

            File.WriteAllLines(fpatOut, translator.ToAsm(filn, src));
        }
    }

   
    
}
