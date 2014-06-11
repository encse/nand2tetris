using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Cmn;
using Cmn.Compiler;

namespace NsVmTranslator
{
    class Program
    {
        static void Main(string[] args)
        {
          //  new JackParser().Parse("1+1");
            var n = new JackParser().Parse(
                @"
/** Computes the ave

rage of a sequence of integers. */

/*

dsada

*/
// This file is part of www.nand2tetris.org
// and the book ""The Elements of Computing Systems""
// by Nisan and Schocken, MIT Press.
// File name: projects/10/ArrayTest/Main.jack


class Main {

    /** Computes the average of a sequence of integers. */
    function void main() {
        var Array a;
        var int length;
	 var int i, sum;

	let length = Keyboard.readInt(""HOW MANY NUMBERS? "");
	
	let i = 0;
	let a = Array.new(length);
       
	
	while (i < length) {
	    let a[i] = Keyboard.readInt(""ENTER THE NEXT NUMBER: "");
	    let i = i + 1;
    }
	
	let i = 0;
	let sum = 0;
	
	while (i < length) {
	    let sum = sum + a[i];
	    let i = i + 1;
	}
	
	do Output.printString(""THE AVERAGE IS: "");
	do Output.printInt(sum / length);
	do Output.println();

	return;
    }
}");

         

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
