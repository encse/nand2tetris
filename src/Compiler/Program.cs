using System;
using System.IO;
using System.Linq;
using Cmn;
using Cmn.Compiler;

namespace Compiler
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
                throw new Ufer(AppDomain.CurrentDomain.FriendlyName + " <xxx.jack>");

            var compiler = new JackCompiler();

            if (Directory.Exists(args[0]))
            {
                foreach (var dpat in args[0].EnCons(Directory.EnumerateDirectories(args[0], "*.*", SearchOption.AllDirectories)).ToArray())
                {
                    var rgfpatIn = Directory.EnumerateFiles(dpat, "*.jack").ToArray();
                    foreach (var fpatIn in rgfpatIn)
                    {
                        Console.WriteLine(fpatIn);
                        var fpatOut = Path.GetFileNameWithoutExtension(fpatIn) + ".vm";
                        Console.WriteLine(">> " + fpatOut);
                        File.WriteAllText(fpatOut, compiler.Compile(File.ReadAllText(fpatIn)));
                        Console.WriteLine();
                    }
                }
            }
            else
            {
                var fpatIn = Path.GetFullPath(args[0]);

                if (!args[0].EndsWith(".jack", StringComparison.InvariantCultureIgnoreCase))
                    throw new Ufer("Not a jack file");

                var fpatOut = Path.GetFileNameWithoutExtension(fpatIn) + ".vm";
                File.WriteAllText(fpatOut, compiler.Compile(File.ReadAllText(fpatIn)));
            }
        }
    }
}
