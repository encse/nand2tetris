using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Cmn;
using Cmn.Compiler;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    class ParserTests
    {
        [TestCase("parser.array.Main.jack", "parser.array.Main.xml")]
        [TestCase("parser.expressionlessSquare.Main.jack", "parser.expressionlessSquare.Main.xml")]
        [TestCase("parser.expressionlessSquare.Square.jack", "parser.expressionlessSquare.Square.xml")]
        [TestCase("parser.expressionlessSquare.SquareGame.jack", "parser.expressionlessSquare.SquareGame.xml")]
        [TestCase("parser.square.Main.jack", "parser.square.Main.xml")]
        [TestCase("parser.square.Square.jack", "parser.square.Square.xml")]
        [TestCase("parser.square.SquareGame.jack", "parser.square.SquareGame.xml")]
        public static void Parse(string fpatJack, string fpatXml)
        {
            var stJack = U.StFromResource(typeof (Tests), fpatJack);
            var stXml = XDocument.Parse(U.StFromResource(typeof (Tests), fpatXml)).ToString();

            var n = new JackParser().Parse(stJack);
            //var xdoc = new XDocument();
            //using (var xw = XmlWriter.Create(xdoc.CreateWriter(), new XmlWriterSettings {IndentChars = "  "}))
            //{
            //    n.ToXml(xw);
            //}

            //Console.WriteLine(xdoc.ToString());
           // Assert.AreEqual(stXml, xdoc.ToString());
        }

    }
}
