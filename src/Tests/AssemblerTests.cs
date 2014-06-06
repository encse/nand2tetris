using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cmn;
using Cmn.Compiler;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    class Tests
    {
        [TestCase("asm.Add.asm", "asm.Add.hack")]
        [TestCase("asm.Max.asm", "asm.Max.hack")]
        [TestCase("asm.MaxL.asm", "asm.MaxL.hack")]
        [TestCase("asm.Pong.asm", "asm.Pong.hack")]
        [TestCase("asm.PongL.asm", "asm.PongL.hack")]
        [TestCase("asm.Rect.asm", "asm.Rect.hack")]
        [TestCase("asm.RectL.asm", "asm.RectL.hack")]
        public static void Assembler(string fpatAsm, string fpatHack)
        {
            var stAsm = U.StFromResource(typeof (Tests), fpatAsm);
            var stHackExpected = U.StFromResource(typeof (Tests), fpatHack);

            var stHackOut = new Assembler().Assemble(stAsm).StJoin("\r\n") + "\r\n";
            Console.WriteLine(stHackOut);
            Assert.AreEqual(stHackExpected, stHackOut);
        }

    }
}
