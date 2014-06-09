using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

            var st = Path.GetFullPath(args[0]);
            string[] rgfpatIn;
            string fpatOut;

            var translator = new VMTranslator();
                

            if (Directory.Exists(args[0]))
            {
                foreach (var dpat in Directory.EnumerateDirectories(st, "*.*", SearchOption.AllDirectories).ToArray())
                {
                    rgfpatIn = Directory.EnumerateFiles(dpat, "*.vm").ToArray();
                    if (rgfpatIn.Any())
                    {
                        Console.WriteLine(rgfpatIn.StJoin("\n"));
                        fpatOut = Path.Combine(dpat, Path.GetFileName(dpat)) + ".asm";
                        Console.WriteLine(">> " + fpatOut);
                        File.WriteAllLines(fpatOut, translator.ToAsm(rgfpatIn));
                        Console.WriteLine();
                    }
                }
            }
            else
            {
                if (!args[0].EndsWith(".vm", StringComparison.InvariantCultureIgnoreCase))
                    throw new Ufer("Not a vm file");

                rgfpatIn = new[] {st};
                fpatOut = Path.GetFileNameWithoutExtension(st) + ".asm";
                File.WriteAllLines(fpatOut, translator.ToAsm(rgfpatIn));
            }
            
        }
    }

   
    
}
