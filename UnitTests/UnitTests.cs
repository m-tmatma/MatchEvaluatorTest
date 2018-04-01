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
        [Test]
        public void TestGuidWithBrackets()
        {
            var replaceWithGuid = new ReplaceWithNewGuid(this.guidGenerator.NewGuid);
            var output1 = replaceWithGuid.ReplaceSameGuidToSameGuid(Input1);
            Console.WriteLine("input   : " + Input1);
            Console.WriteLine("output  : " + output1);
            Console.WriteLine("expected: " + Result1);
            Assert.That(output1, Is.EqualTo(Result1));

            var output2 = replaceWithGuid.ReplaceSameGuidToSameGuid(Input2);
            Console.WriteLine("input   : " + Input2);
            Console.WriteLine("output  : " + output2);
            Console.WriteLine("expected: " + Result2);
            Assert.That(output2, Is.EqualTo(Result2));

            var output3 = replaceWithGuid.ReplaceSameGuidToSameGuid(Input3);
            Console.WriteLine("input   : " + Input3);
            Console.WriteLine("output  : " + output3);
            Console.WriteLine("expected: " + Result3);
            Assert.That(output3, Is.EqualTo(Result3));

            var outputOLECreate = replaceWithGuid.ReplaceSameGuidToSameGuid(InputOLECREATE);
            Console.WriteLine("input   : " + InputOLECREATE);
            Console.WriteLine("output  : " + outputOLECreate);
            Console.WriteLine("expected: " + ResultOLECREATE);
            Assert.That(outputOLECreate, Is.EqualTo(ResultOLECREATE));

            var outputDefineGuid = replaceWithGuid.ReplaceSameGuidToSameGuid(InputDefineGuid);
            Console.WriteLine("input   : " + InputDefineGuid);
            Console.WriteLine("output  : " + outputDefineGuid);
            Console.WriteLine("expected: " + ResultDefineGuid);
            Assert.That(outputDefineGuid, Is.EqualTo(ResultDefineGuid));

            var outputGUIDStruct = replaceWithGuid.ReplaceSameGuidToSameGuid(InputGUIDStruct);
            Console.WriteLine("input   : " + InputGUIDStruct);
            Console.WriteLine("output  : " + outputGUIDStruct);
            Console.WriteLine("expected: " + ResultGUIDStruct);
            Assert.That(outputGUIDStruct, Is.EqualTo(ResultGUIDStruct));
        }
    }
}
