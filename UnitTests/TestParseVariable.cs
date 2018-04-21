﻿//-----------------------------------------------------------------------
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
    }
}