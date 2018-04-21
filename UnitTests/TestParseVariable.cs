//-----------------------------------------------------------------------
// <copyright file="TestParseVariable.cs" company="Masaru Tsuchiyama">
//     Copyright (c) Masaru Tsuchiyama. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Unittest
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using NUnit.Framework;
    using ParseVariable;

    [TestFixture]
    public class TestParseVariable
    {
        [SetUp]
        public void SetUp()
        {
        }

        [TearDown]
        public void TearDown()
        {
        }

        [TestCase("Variable1", "VariableA")]
        [TestCase("Variable2", "VariableB")]
        [TestCase("Variable3", "VariableC")]
        [TestCase("Variable4", "VariableD")]
        public void Test_ParseVariable(string inputKeyword, string outputKeyword)
        {
            var translationTable = new Dictionary<string, string>();
            translationTable[inputKeyword] = outputKeyword;

            var input = "{" + inputKeyword + "}";
            var expected = "{" + outputKeyword + "}";
            var output = ProcessVariable.ReplaceVariable(input, translationTable);
            Console.WriteLine("input   : " + input);
            Console.WriteLine("output  : " + output);
            Console.WriteLine("expected: " + expected);
            Assert.That(output, Is.EqualTo(expected));
        }

        [TestCase("Variable1", "VariableA", "[", "]")]
        [TestCase("Variable2", "VariableB", "-", "-")]
        [TestCase("Variable3", "VariableC", "<", ">")]
        [TestCase("Variable4", "VariableD", "[", "")]
        [TestCase("Variable5", "VariableE", "-", "")]
        [TestCase("Variable6", "VariableF", "(", "")]
        [TestCase("Variable7", "VariableG", "", "]")]
        [TestCase("Variable8", "VariableH", "", "-")]
        [TestCase("Variable9", "VariableI", "", ")")]
        public void Test_ParseInvalidVariable(string inputKeyword, string outputKeyword, string left, string right)
        {
            var translationTable = new Dictionary<string, string>();
            translationTable[inputKeyword] = outputKeyword;

            var input = "{" + left + inputKeyword + right + "}";
            var expected = input;
            var output = ProcessVariable.ReplaceVariable(input, translationTable);
            Console.WriteLine("input   : " + input);
            Console.WriteLine("output  : " + output);
            Console.WriteLine("expected: " + expected);
            Assert.That(output, Is.EqualTo(expected));
        }

        [TestCase("Variable1", "VariableA", "", "")]
        [TestCase("Variable2", "VariableB", "{", "")]
        [TestCase("Variable3", "VariableC", "", "}")]
        public void Test_ParseInvalidVariableNoCurlyBracket(string inputKeyword, string outputKeyword, string left, string right)
        {
            var translationTable = new Dictionary<string, string>();
            translationTable[inputKeyword] = outputKeyword;

            var input = left + inputKeyword + right;
            var expected = input;
            var output = ProcessVariable.ReplaceVariable(input, translationTable);
            Console.WriteLine("input   : " + input);
            Console.WriteLine("output  : " + output);
            Console.WriteLine("expected: " + expected);
            Assert.That(output, Is.EqualTo(expected));
        }

        [TestCase("Variable1", "VariableA", 1)]
        [TestCase("Variable2", "VariableB", 2)]
        [TestCase("Variable3", "VariableC", 3)]
        [TestCase("Variable4", "VariableD", 4)]
        public void Test_ParseVariableIndex(string inputKeyword, string outputKeyword, int index)
        {
            var translationTable = new Dictionary<string, string>();
            translationTable[inputKeyword] = outputKeyword;

            var indexStr = "(" + index.ToString() + ")";
            var input = "{" + inputKeyword + indexStr + "}";
            var expected = "{" + outputKeyword + indexStr + "}";
            var output = ProcessVariable.ReplaceVariable(input, translationTable);
            Console.WriteLine("input   : " + input);
            Console.WriteLine("output  : " + output);
            Console.WriteLine("expected: " + expected);
            Assert.That(output, Is.EqualTo(expected));
        }


        [TestCase("Variable1", "VariableA", 1, "[", "]")]
        [TestCase("Variable2", "VariableB", 2, "{", "}")]
        [TestCase("Variable3", "VariableC", 3, "<", ">")]
        [TestCase("Variable4", "VariableD", 4, "[", "")]
        [TestCase("Variable5", "VariableE", 5, "{", "")]
        [TestCase("Variable6", "VariableF", 6, "(", "")]
        [TestCase("Variable7", "VariableG", 7, "", "]")]
        [TestCase("Variable8", "VariableH", 8, "", "}")]
        [TestCase("Variable9", "VariableI", 9, "", ")")]
        public void Test_ParseVariableInvalidIndex(string inputKeyword, string outputKeyword, int index, string left, string right)
        {
            var translationTable = new Dictionary<string, string>();
            translationTable[inputKeyword] = outputKeyword;

            var indexStr = left + index.ToString() + right;
            var input = "{" + inputKeyword + indexStr + "}";
            var expected = input;
            var output = ProcessVariable.ReplaceVariable(input, translationTable);
            Console.WriteLine("input   : " + input);
            Console.WriteLine("output  : " + output);
            Console.WriteLine("expected: " + expected);
            Assert.That(output, Is.EqualTo(expected));
        }
    }
}
