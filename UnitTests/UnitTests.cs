//-----------------------------------------------------------------------
// <copyright file="UnitTests.cs" company="Masaru Tsuchiyama">
//     Copyright (c) Masaru Tsuchiyama. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace ReplaceGuid
{
    using System;
    using System.Text;
    using NUnit.Framework;

    /// <summary>
    /// unit test for ReplaceWithNewGuid
    /// </summary>
    [TestFixture]
    public class UnitTests
    {
        /// <summary>
        /// input data for test1
        /// </summary>
        private const string Input1 = @"[Guid(""00000000-0000-0000-0000-000000000000"")]";

        /// <summary>
        /// expected output data for test1
        /// </summary>
        private const string Result1 = @"[Guid(""01020304-0506-0708-090a-0b0c0d0e0f10"")]";

        /// <summary>
        /// input data for test2
        /// </summary>
        private const string Input2 = @"""00000000-0000-0000-0000-000000000000""";

        /// <summary>
        /// expected output data for test2
        /// </summary>
        private const string Result2 = @"""01020304-0506-0708-090a-0b0c0d0e0f10""";

        /// <summary>
        /// input data for test3
        /// </summary>
        private const string Input3 = @"{00000000-0000-0000-0000-000000000000}";

        /// <summary>
        /// expected output data for test3
        /// </summary>
        private const string Result3 = @"{01020304-0506-0708-090a-0b0c0d0e0f10}";

        /// <summary>
        /// input data for IMPLEMENT_OLECREATE
        /// </summary>
        private const string InputOLECREATE = @"IMPLEMENT_OLECREATE(<<class>>, <<external_name>>, 0x00000000, 0x0000, 0x0000, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00);";

        /// <summary>
        /// expected output data for IMPLEMENT_OLECREATE
        /// </summary>
        private const string ResultOLECREATE = @"IMPLEMENT_OLECREATE(<<class>>, <<external_name>>, 0x01020304, 0x0506, 0x0708, 0x09, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f, 0x10);";

        /// <summary>
        /// input data for DEFINE_GUID
        /// </summary>
        private const string InputDefineGuid = @"DEFINE_GUID(<<name>>, 0x00000000, 0x0000, 0x0000, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00);";

        /// <summary>
        /// expected output data for DEFINE_GUID
        /// </summary>
        private const string ResultDefineGuid = @"DEFINE_GUID(<<name>>, 0x01020304, 0x0506, 0x0708, 0x09, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f, 0x10);";

        /// <summary>
        /// input data for GUID structure definition
        /// </summary>
        private const string InputGUIDStruct = @"static const GUID <<name>> = {0x00000000,0x0000,0x0000,{0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00}};";

        /// <summary>
        /// expected output data for GUID structure definition
        /// </summary>
        private const string ResultGUIDStruct = @"static const GUID <<name>> = {0x01020304,0x0506,0x0708,{0x09,0x0a,0x0b,0x0c,0x0d,0x0e,0x0f,0x10}};";

        /// <summary>
        /// GUID generator class to make unit-testing easier
        /// </summary>
        private GuidGenerater guidGenerator;

        /// <summary>
        /// setup method to be called to initialize unit tests
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            this.guidGenerator = new GuidGenerater();
        }

        /// <summary>
        /// cleanup method to be called to cleanup unit tests
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            this.guidGenerator = null;
        }

        /// <summary>
        /// unit test method
        /// </summary>
        /// <see href="https://github.com/nunit/docs/wiki/TestCase-Attribute">TestCase Attribute</see>
        /// <see href="https://github.com/nunit/docs/wiki/TestCaseSource-Attribute">TestCaseSource Attribute</see>
        /// <see href="https://github.com/nunit/docs/wiki/TestCaseData">TestCaseData</see>
        [TestCase(Input1, Result1)]
        [TestCase(Input2, Result2)]
        [TestCase(Input3, Result3)]
        [TestCase(InputOLECREATE, ResultOLECREATE)]
        [TestCase(InputDefineGuid, ResultDefineGuid)]
        [TestCase(InputGUIDStruct, ResultGUIDStruct)]
        public void TestGuidByStaticPattern(string input, string expected)
        {
            var replaceWithGuid = new ReplaceWithNewGuid(this.guidGenerator.NewGuid);
            var output = replaceWithGuid.ReplaceSameGuidToSameGuid(input);
            Console.WriteLine("input   : " + input);
            Console.WriteLine("output  : " + output);
            Console.WriteLine("expected: " + expected);
            Assert.That(output, Is.EqualTo(expected));
        }
    }
}
