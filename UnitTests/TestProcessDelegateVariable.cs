//-----------------------------------------------------------------------
// <copyright file="TestProcessDelegateVariable.cs" company="Masaru Tsuchiyama">
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
    public class TestProcessDelegateVariable
    {
        private static Dictionary<string, string> keywordMap = new Dictionary<string, string>()
        {
            { "Variable1", "VariableA" },
            { "Variable2", "VariableB" },
            { "Variable3", "VariableC" },
            { "Variable4", "VariableD" },
            { "Variable5", "VariableE" },
            { "Variable6", "VariableF" },
            { "Variable7", "VariableG" },
            { "Variable8", "VariableH" },
            { "Variable9", "VariableI" },
        };

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
            var translationTable = GetTranslationTable();
            var input = "{" + inputKeyword + "}";
            var expected = "{" + outputKeyword + "}";
            var output = ProcessDelegateVariable.ReplaceVariable(input, translationTable);
            Console.WriteLine("input   : " + input);
            Console.WriteLine("output  : " + output);
            Console.WriteLine("expected: " + expected);
            Assert.That(output, Is.EqualTo(expected));
        }

        public void Test_ParseInvalidVariable(string inputKeyword, string outputKeyword, string left, string right)
        {
            var translationTable = GetTranslationTable();
            var input = "{" + left + inputKeyword + right + "}";
            var expected = input;
            var output = ProcessDelegateVariable.ReplaceVariable(input, translationTable);
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
            var translationTable = GetTranslationTable();
            var input = left + inputKeyword + right;
            var expected = input;
            var output = ProcessDelegateVariable.ReplaceVariable(input, translationTable);
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
            var translationTable = GetTranslationTable();
            var indexStr = "(" + index.ToString() + ")";
            var input = "{" + inputKeyword + indexStr + "}";
            var expected = "{" + outputKeyword + indexStr + "}";
            var output = ProcessDelegateVariable.ReplaceVariable(input, translationTable);
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
            var translationTable = GetTranslationTable();
            var indexStr = left + index.ToString() + right;
            var input = "{" + inputKeyword + indexStr + "}";
            var expected = input;
            var output = ProcessDelegateVariable.ReplaceVariable(input, translationTable);
            Console.WriteLine("input   : " + input);
            Console.WriteLine("output  : " + output);
            Console.WriteLine("expected: " + expected);
            Assert.That(output, Is.EqualTo(expected));
        }

        /// <summary>
        /// create dictionary for delegate
        /// </summary>
        /// <returns></returns>
        internal Dictionary<string, ProcessDelegateVariable.GetNewText> GetTranslationTable()
        {
            var translationTable = new Dictionary<string, ProcessDelegateVariable.GetNewText>();
            foreach(string keyword in keywordMap.Keys)
            {
                translationTable[keyword] = delegateGetNewText;
            }
            return translationTable;
        }

        /// <summary>
        /// delegate to substitute keywords
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        internal string delegateGetNewText(string keyword, int index)
        {
            string newkeyword = string.Empty;
            if (keywordMap.ContainsKey(keyword))
            {
                newkeyword = keywordMap[keyword];
            }

            if (index < 0)
            {
                return newkeyword;
            }
            else
            {
                return newkeyword + "(" + index.ToString() + ")";
            }
        }
    }
}
