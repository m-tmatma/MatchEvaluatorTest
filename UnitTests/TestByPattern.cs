//-----------------------------------------------------------------------
// <copyright file="TestPattern.cs" company="Masaru Tsuchiyama">
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
    public class TestPattern
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
        /// input data for test4
        /// </summary>
        private const string Input4 = @"00000000000000000000000000000000";

        /// <summary>
        /// expected output data for test3
        /// </summary>
        private const string Result4 = @"0102030405060708090a0b0c0d0e0f10";

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
        /// Input strings
        /// </summary>
        private static readonly string[] Inputs = new string[]
        {
            Input1,
            Input2,
            Input3,
            Input4,
            InputOLECREATE ,
            InputDefineGuid,
            InputGUIDStruct,
        };

        /// <summary>
        /// Result strings
        /// </summary>
        private static readonly string[] Results = new string[]
        {
            Result1,
            Result2,
            Result3,
            Result4,
            ResultOLECREATE,
            ResultDefineGuid,
            ResultGUIDStruct,
        };

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
        [TestCase(Input4, Result4)]
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

        /// <summary>
        /// Randomly concatenated pattern test
        /// </summary>
        /// <param name="count">loop count</param>
        /// <see href="https://github.com/nunit/docs/wiki/TestCase-Attribute">TestCase Attribute</see>
        [TestCase(3)]
        [TestCase(5)]
        [TestCase(7)]
        [TestCase(10)]
        [TestCase(20)]
        [TestCase(50)]
        [TestCase(100)]
        public void TestGuidByRandomlyConcatenatedPattern(int count)
        {
            // check data size
            Assert.That(Inputs.Length, Is.GreaterThan(0));
            Assert.That(Results.Length, Is.GreaterThan(0));
            Assert.That(Inputs.Length, Is.EqualTo(Results.Length));

            var builderInput = new StringBuilder();
            var builderResult = new StringBuilder();
            var random = new Random();

            for (int i = 0; i < count; i++)
            {
                int index = random.Next(0, Inputs.Length - 1);
                var separator = string.Format("------ {0} ------", i);
                builderInput.Append(separator);
                builderInput.Append(Environment.NewLine);
                builderInput.Append(Inputs[index]);
                builderInput.Append(Environment.NewLine);
                builderInput.Append(Environment.NewLine);

                builderResult.Append(separator);
                builderResult.Append(Environment.NewLine);
                builderResult.Append(Results[index]);
                builderResult.Append(Environment.NewLine);
                builderResult.Append(Environment.NewLine);
            }

            var input = builderInput.ToString();
            var expected = builderResult.ToString();

            var replaceWithGuid = new ReplaceWithNewGuid(this.guidGenerator.NewGuid);
            var output = replaceWithGuid.ReplaceSameGuidToSameGuid(input);

#if PRINTF_DEBUG
            Console.WriteLine("input: ");
            Console.WriteLine(input);

            Console.WriteLine("output: ");
            Console.WriteLine(output);

            Console.WriteLine("expected: ");
            Console.WriteLine(expected);
#endif // PRINTF_DEBUG

            Assert.That(output, Is.EqualTo(expected));
        }
    }
}
